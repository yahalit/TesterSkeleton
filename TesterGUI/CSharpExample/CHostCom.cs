using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace PvsGUI
{

    public class CMessage
    {
        public ulong  SystemTime;
        public uint MsgCount;
        public ushort DataLen;
        public ushort OpCode;
        public ushort[] Buf;

        public CMessage()
        {
            Buf = new ushort[256]; 
        }
        public CMessage( ushort [] buf )
        {
            Buf = buf ;
        }
        public unsafe CMessage(int len , int op , int start , byte [] inbuf )
        {
            fixed ( byte * ptr = &inbuf[start+2]) // 2 compensates for the preamble 
            {
                uint* uPtr = (uint*)ptr; 
                SystemTime = (ulong)uPtr[0] + ( ( ulong) uPtr[1]  << 32 ) ;
                MsgCount = uPtr[2];
                DataLen = (ushort)(uPtr[3] & 0xffff);
                OpCode = (ushort)((uPtr[3]>>16) & 0xffff);
                Buf = new ushort[DataLen / 2 - 1];

                ushort* usPtr = (ushort*)ptr; 
                for ( int cnt = 0; cnt < Buf.Length; cnt++)
                {
                    Buf[cnt] = *usPtr++; 
                }
            }
        }

        public CMessage Clone()
        {
            CMessage msg = new CMessage((ushort[])this.Buf.Clone()) ;
            msg.SystemTime = this.SystemTime;
            msg.MsgCount = this.MsgCount;
            msg.DataLen = this.DataLen;
            msg.OpCode = this.OpCode;
            return msg; 
        }
    };

    //This class simply services the communications. 
    //    It reads data from the COM and strnsmit things ready for transmission 
    public class CHostCom
    {
                                                  // making about 30000. So this is the nearest 2^N
        static ushort Preamble0 = 0xa5;
        static ushort Preamble1 = 0xa5;
        public List<CMessage> Messages = new List<CMessage> { };
        byte[ ] OutByte = new byte[256*256];
        uint[] OutByteCnt = new uint[256];
        ushort nOutPut;
        ushort nOutGet; 
        CMessage StamMsg = new CMessage() ; 
        uint nPutRead = 0;
        uint nFetchRead = 0;
        uint DecodeState = 0;
        uint nextShortCnt = 0 ;
        uint cs = 0; 
        bool EvenByte = false   ; 
        ushort nextShort = 0 ;
        // Create an instance of the SerialPort class
        public SerialPort mySerialPort;//= new SerialPort("COM1");  // Specify the COM port name (e.g., COM1, COM2, etc.)
        public bool bIsOpen =false;
        byte[] ReadBuffer = new byte[Literals.BYTES_IN_READ_BUFFER];
        static private Mutex mutex;

        public CHostCom( Mutex _mutex)
        {
            mutex = _mutex; 
        }

        public void GetAvailablePorts(out string[] ports)
        {
            ports = SerialPort.GetPortNames();
        }

        public bool OpenSerialPort(string port, int baud , out string message)
        {
            message = "Port Opened succesfully";
            if (bIsOpen)
                try
                {
                    mySerialPort.Close();
                }
                catch
                {
                    message = "Could not close previous connection";
                    return false; 
                }
            bIsOpen = false; 
            try
            {
                mySerialPort = new SerialPort(port);
                mySerialPort.BaudRate = baud ;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.ReadBufferSize = Literals.GUI_COM_READ_BUFFER_SIZE; // 262144 will be cosidered overflow
                mySerialPort.Open();
                mySerialPort.DiscardInBuffer();
                mySerialPort.DiscardOutBuffer();
                bIsOpen = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                // This usually occurs when the port is already open or you don't have sufficient permissions.
                message = "Could not open  port " + "UnauthorizedAccessException:" + ex.Message;
                return false;
            }
            catch (ArgumentException ex)
            {
                // This occurs if the port name is incorrect or not valid.
                message = "Could not open  port " + "ArgumentException:" + ex.Message;
                return false;
            }
            catch (Exception ex)
            { 
                message = "Could not open  port " + port + " : " +  ex.Message;
                return false;
            }
            return true; 
        }

        // Place2End: The number of bytes that can be placed in the buffer without modulo considerations 
        // Return value: The number of bytes that may be read without overstriking unread bytes
        private uint PlaceTillEndOfBuffer(out uint Place2End)
        {
            Place2End = (uint)Literals.BYTES_IN_READ_BUFFER - nPutRead;
            return ((uint)Literals.BYTES_IN_READ_BUFFER - 1 - ((nPutRead - nFetchRead) & ((uint)Literals.BYTES_IN_READ_BUFFER -1)));
        } 

        public void FlushComBuffer()
        {
            nOutGet = 0;
            nOutPut = 0;
            nPutRead = 0;
            nFetchRead = 0; 
            DecodeState = 0; 
            try
            {
                mySerialPort.DiscardInBuffer();  // Clears the input buffer
                mySerialPort.DiscardOutBuffer(); // Clears the output buffer
            }
            catch
            {
                return  ;
            }
        }

        // Flush the transmit queue and the receive queue
        public void FlushCommunication()
        {
            nOutGet = 0;
            nOutPut = 0;
            nFetchRead = nPutRead;
            DecodeState = 0;

        }

        // State machine for reading
        public uint ReadCOM() 
        {
            uint bytesAvailable; 
            try
            {
                bytesAvailable = (uint)mySerialPort.BytesToRead;
                if ( bytesAvailable >= Literals.BYTES_IN_READ_BUFFER)
                { // No use, its overflowing
                    FlushComBuffer();
                    return 0; 
                }
            }
            catch
            {
                return 0;
            }

            if (bytesAvailable == 0)
            {
                return 0; 
            }
            uint place2End;
            uint PlaceInBuf = PlaceTillEndOfBuffer(out place2End);
            uint Bytes2Read = Math.Min(bytesAvailable, PlaceInBuf); 
            if (Bytes2Read <= place2End)
            {
                mySerialPort.Read(ReadBuffer, (int)nPutRead, (int)Bytes2Read);
                nPutRead = (nPutRead + Bytes2Read) & ((uint)Literals.BYTES_IN_READ_BUFFER - 1) ;  // Modulo need be taken for the case of re-reaching 0 
            }
            else
            {
                mySerialPort.Read(ReadBuffer, (int)nPutRead, (int)place2End);
                nPutRead = Bytes2Read - place2End;
                mySerialPort.Read(ReadBuffer, 0, (int)nPutRead);
            }
            return Bytes2Read;
        }

        ushort PopByte()
        {
            ushort next = (ushort) ReadBuffer[nFetchRead];
            nFetchRead = (nFetchRead + 1) & ((uint)Literals.BYTES_IN_READ_BUFFER - 1);
            return next;
        }
        public void InterceptCOMMsg()
        {
            uint Bytes2Read = ReadCOM();
            ushort next;
            for  ( uint cnt = 0;  cnt < Bytes2Read; cnt++)
            {
                next = PopByte() ;
                if (DecodeState == 0)
                {
                    if (next == Preamble0)
                        DecodeState = 1;
                    continue;
                }
                else if (DecodeState == 1)
                {
                    if (next == Preamble1)
                        DecodeState = 2;
                    EvenByte = true;
                    nextShortCnt = 0;
                    cs = 0xa5a5 ; 
                    continue;
                }
                else
                {
                    if (EvenByte)
                    {
                        nextShort = next;
                        EvenByte = false;
                        continue;
                    }
                    EvenByte = true; 
                    nextShort = (ushort)(nextShort + ( next << 8 ));
                    if (nextShortCnt + 1 >= StamMsg.Buf.Length )
                    { // Just over cautions
                        DecodeState = 0;
                        continue; 
                    }
                    StamMsg.Buf[nextShortCnt++] = nextShort; 
                    cs += nextShort; 
                    if (DecodeState == 2)
                    {
                        if (nextShortCnt == 8 )
                        {
                            StamMsg.SystemTime = (ulong)StamMsg.Buf[0]
                                + ((ulong)StamMsg.Buf[1] << 16)
                                + ((ulong)StamMsg.Buf[2] << 32)
                                + ((ulong)StamMsg.Buf[3] << 48);
                            StamMsg.MsgCount = (uint)StamMsg.Buf[4]
                                + ((uint)StamMsg.Buf[5] << 16);
                            StamMsg.DataLen = StamMsg.Buf[6];
                            StamMsg.OpCode  = StamMsg.Buf[7];
                            // Check opcode sanity 
                            int good = 0 ;
                            if (( StamMsg.OpCode == 0xD005  ) || (StamMsg.OpCode == (ushort)Literals.Reply2GUIOpCodes.GUIAnswer_PeriodicStatus )) 
                            {
                                good = 1; 
                            }
                            else
                            {// Preamble may be B0 or C0. The last two bytes are specific opcodes, ANDed out
                                if (( (StamMsg.OpCode & 0xff00 ) == 0xb000  ) || ((StamMsg.OpCode & 0xff00) == 0xc000  ))  
                                {
                                    good = 1; 
                                }
                            }
                            if (StamMsg.DataLen < 9*2 ||  StamMsg.DataLen > 246*2 || ( (StamMsg.DataLen&1) != 0) )
                            {
                                good = 0; 
                            }
                            if ( good == 0 )
                            {
                                DecodeState = 0; 
                            }
                            else
                            {
                                DecodeState = 3;
                                StamMsg.DataLen = (ushort)(StamMsg.DataLen >> 1)  ; 
                            }
                        }
                    }
                    else
                    {
                        if (StamMsg.DataLen == (nextShortCnt+1))
                        {
                            if ( (cs & 0xffff) == 0 )
                            {
                                Messages.Add(StamMsg.Clone() ) ;
                            }
                            DecodeState = 0;
                            nextShortCnt = 0;                        }
                    }
                }

            }
        }

        int nUnproccessedBytes = 0 ;
        int nSuspectMsgStart = 0;
        byte[] EReadBuffer = new byte[256000];

        public void EnhancedInterceptCOMMsg()
        {

            int bytesAvailable;
            try
            {
                bytesAvailable =  mySerialPort.BytesToRead;
                if (bytesAvailable + nUnproccessedBytes >= Literals.BYTES_IN_READ_BUFFER)
                { // No use, its overflowing
                    mySerialPort.DiscardInBuffer();
                    nUnproccessedBytes = 0;
                    nSuspectMsgStart = 0;
                    return;
                }
            }
            catch
            {
                return;
            }

            if (bytesAvailable == 0)
            {
                return;
            }
            mySerialPort.Read(ReadBuffer, nUnproccessedBytes, bytesAvailable);
            nUnproccessedBytes += bytesAvailable;

            int state = 0;
            int cnt = 0;
            int d;
            int len = 0 ;
            int op = 0; 
            byte c; 
            while (  cnt < nUnproccessedBytes )
            {
                c = ReadBuffer[cnt];
                cnt += 1; 
                if (state == 0 )
                {
                    if ( c == 0xa5 )
                    {
                        nSuspectMsgStart = cnt-1;
                        state = 1;
                    }
                    else
                    {
                        nSuspectMsgStart = cnt; 
                    }
                    continue;
                }
                if (state == 1)
                {
                    if (c == 0xa5)
                    {
                        state = 2;
                        continue;
                    }
                    else
                    {
                        state = 0; 
                    }
                }
                if (state == 2)
                {
                    d = cnt - nSuspectMsgStart;
                    if (d == 15)
                    {
                        len = c;
                        continue;
                    }
                    if (d == 16)
                    {
                        len = (int)c * 256 + len;
                        if (((len&1)!=0) || (len > 1024))
                        {
                            cnt = nSuspectMsgStart + 2;
                            state = 0; 
                        }
                        continue;
                    }
                    if ( d == 17 )
                    {
                        op = c;
                        continue; 
                    }
                    if ( d == 18 )
                    {
                        op = (int)c * 256 + op;
                        bool good = ((op & 0xff00) == 0xb000) || ((op & 0xff00) == 0xc000) ||
                            (op == 0xD005) || (op == (int)Literals.Reply2GUIOpCodes.GUIAnswer_PeriodicStatus);
                        if ( !good) 
                        {
                            cnt = nSuspectMsgStart + 2;
                            state = 0;
                        }
                        else
                        {
                            state = 3; 
                        }
                    }
                    continue; 
                }
                // State == 3 
                d = cnt - nSuspectMsgStart;
                if ( d == len  )
                {
                    int cs = 0;
                    state = 0;
                    for ( int c1 = 0; c1 < len; c1+=2)
                    {
                        cs += (int)ReadBuffer[nSuspectMsgStart + c1] + (int)ReadBuffer[nSuspectMsgStart + c1 + 1] * 256;
                    }
                    if ( (cs & 0xffff) != 0 )
                    {
                        cnt = nSuspectMsgStart + 2;
                        continue;
                    }
                    Messages.Add(new CMessage(len, op, nSuspectMsgStart, ReadBuffer));
                    nSuspectMsgStart = cnt ;
                }

            }
            // Finally take the unuse count for the next time

            for ( int c1 = 0; c1 < nUnproccessedBytes - nSuspectMsgStart; c1++)
            {
                ReadBuffer[c1] = ReadBuffer[c1 + nSuspectMsgStart];
            }
            nUnproccessedBytes -= nSuspectMsgStart;
        }


        // Get how many messages are there to read
        public uint GetInMessageCount()
        {
            return (uint) Messages.Count ;
        }


        // Send something new to the transmit queue 
        public int SetMessage2TxQueue(byte [] str)
        {
            ushort next = (ushort)((nOutPut+1)&255);
            if ( next == nOutGet)
            {
                return -1; 
            }
            uint len = (uint)str.Length; 
            if ( len == 0 )
            {
                return 0; 
            }
            Array.Copy(str, 0, OutByte, 256* nOutPut, len);
            OutByteCnt[nOutPut] = len;
            nOutPut = next;
            return 0; 
        }

        // Go over the transmit queue and send whatever is there, according to output queue capacity 
        public void TransmitComMsg()
        {
            uint bytesAvailable;
            try
            {
                bytesAvailable = (uint)(mySerialPort.WriteBufferSize - mySerialPort.BytesToWrite);
            }
            catch
            {
                return;
            }
            while (nOutPut != nOutGet)
            {
                if (OutByteCnt[nOutGet] < bytesAvailable)
                {
                    try
                    {
                        mySerialPort.Write(OutByte, 256 * nOutGet, (int)OutByteCnt[nOutGet]);
                        mySerialPort.BaseStream.Flush(); // Force immediate transmission 
                        
                        bytesAvailable -= OutByteCnt[nOutGet];
                        nOutGet = (ushort)((nOutGet + 1) & 255);

                    }
                    catch
                    {
                        bIsOpen = false; 
                    }
                }
                else
                {
                    break; 
                }
            }
        }

    } // Close class
} // Close namespace
