using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.IO;
using ClosedXML.Excel;
using static PvsGUI.CAdc8;

namespace PvsGUI
{
    // Structure of the parts of a MODBUS message 
    public struct CModBusStr
    {
        public byte DeviceAddress;
        public byte Command;
        public byte ByteCount;
        public byte[] Payload;
        public byte[] Answer ;
        public ushort crc;
        public int ExpectedReturnLength; 
    }

    public static class ModbusCRC
    {
        /// <summary>
        /// Computes the Modbus RTU CRC16 for a given byte array.
        /// </summary>
        /// <param name="data">The input byte array for CRC calculation.</param>
        /// <returns>A 2-byte array containing the CRC low byte and high byte.</returns>
        public static byte[] ComputeCRC(byte[] data)
        {
            ushort crc = 0xFFFF; // Initial value

            foreach (byte b in data)
            {
                crc ^= b; // XOR byte into least significant byte of crc

                for (int i = 0; i < 8; i++) // Process 8 bits
                {
                    if ((crc & 0x0001) != 0) // If LSB is set
                    {
                        crc = (ushort)((crc >> 1) ^ 0xA001); // Shift right and XOR with polynomial
                    }
                    else
                    {
                        crc >>= 1; // Just shift right
                    }
                }
            }

            // Return the CRC as two bytes: low byte first, high byte second
            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
        }
    }
    // Class to handle MODBUS communication 
    public class CModbusCom
    {
        public SerialPort mySerialPort;//= new SerialPort("COM1");  // Specify the COM port name (e.g., COM1, COM2, etc.)
        public static Mutex mutex;
        public byte[] ReadBuffer = new byte[256];
        int BytesInBuf;
        public bool bIsOpen;

        byte[] crc;
        public CModbusCom(Mutex _mutex)
        {
            mutex = _mutex;
            byte[] modbusFrame = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };

            // Calculate CRC
            crc = ModbusCRC.ComputeCRC(modbusFrame);
        }

        // Open a serial port (RS485) 
        public bool OpenSerialPort(string port, int Baudrate  , out string message)
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
                mySerialPort = new SerialPort(port)
                {
                    BaudRate = Baudrate,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None
                };
                mySerialPort.Open();
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
                message = "Could not open  port " + port + " : " + ex.Message;
                return false;
            }
            return true;
        }


        // Transmit a MODBUS message  
        public bool TransmitModbusMsg(byte [] msg )
        {
            uint bytesAvailable;
            try
            {
                bytesAvailable = (uint)(mySerialPort.WriteBufferSize - mySerialPort.BytesToWrite);
            }
            catch
            {
                return false;
            }
            int n = msg.Length;
            if ( ( n < 3 ) || ( n >= bytesAvailable) ) 
            {
                return false; 
            }

            try
            {
                mySerialPort.DiscardInBuffer();  // Clears the input buffer
                mySerialPort.DiscardOutBuffer(); // Clears the output buffer
            }
            catch
            {
                return false;
            }
            BytesInBuf = 0; 
            mySerialPort.Write(msg, 0, n);
            mySerialPort.BaseStream.Flush(); // Force immediate transmission 
            return true; 
        }


        // Collect a well-formatted MODBUS message  
        public bool CollectRtuMessage(ref CModBusStr str)
        {
            int bytesAvailable;
            try
            {
                bytesAvailable = mySerialPort.BytesToRead;
                if (bytesAvailable >= 250 - BytesInBuf )
                { // No use, its overflowing
                    return false;
                }
            }
            catch
            {
                return false;
            }

            if (bytesAvailable == 0)
            {
                return false;
            }
            mySerialPort.Read(ReadBuffer, BytesInBuf , bytesAvailable);
            BytesInBuf += bytesAvailable;
            // See if this makes a full Modbus return message
            if (BytesInBuf == ( str.ExpectedReturnLength + 2) )
            {
                if (((str.DeviceAddress==0) ||( ReadBuffer[0] == str.DeviceAddress ) ) && (ReadBuffer[1] == str.Command ) )
                {
                    ushort crc = CModbusCrc.ModbusCRC_Calc(ReadBuffer, str.ExpectedReturnLength-1 );
                    if ( ( (crc&0xff) == (int)ReadBuffer[str.ExpectedReturnLength] ) && (((crc>>8) & 0xff) == (int)ReadBuffer[str.ExpectedReturnLength + 1])) 
                    {
                        Array.Copy(ReadBuffer, 0, str.Answer, 0, str.ExpectedReturnLength);
                        return true;
                    }
                }
            }     

            return false ; 
        }
    }

    // Class to compose a MODBUS message given payload, and calculate the MODBUS CRC
    class CModbusCrc
    {
          static byte[] CRCTableLow = {
          0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
          0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
          0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
          0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
          0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
          0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
          0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
          0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
          0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
          0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
          0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
          0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
          0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
          0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
          0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
          0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
          0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
          0x40
        };
        static byte[] CRCTableHigh = {
          0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
          0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
          0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
          0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
          0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
          0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
          0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
          0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
          0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
          0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
          0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
          0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
          0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
          0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
          0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
          0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
          0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
          0x40
        };


        public static ushort ModbusCRC_Calc(byte[] ptr, int len = -1)
        {
            if (len < 0)
            {
                len = ptr.Length;
            }
            byte crcHigh = 0xff;
            byte crcLow = 0xff;
            int index;
            int cnt = 0;
            while (len >= 0)
            {
                len -= 1;
                index = crcLow ^ ptr[cnt++];
                crcLow = (byte)(crcHigh ^ CRCTableHigh[index]);
                crcHigh = CRCTableLow[index];
            }
            return (ushort)((crcHigh << 8) | crcLow);
        }

 
        static public void AddCrc (byte [] ptr, out byte [] msg)
        {
            int n = ptr.Length;
            msg = new byte[n+2];
            Array.Copy(ptr, 0, msg, 0, n);
            if (n < 1)
            {
                return;
            }

            ushort crc = ModbusCRC_Calc(ptr, ptr.Length - 2);

            msg[n - 2] = (byte)( crc & 0xff  ) ;
            msg[n - 1] = (byte)((crc>>8) & 0xff);
        }

        static public void BuildMessage( CModBusStr str, out byte[] msg)
        {
            int n = str.Payload.Length ;
            msg = new byte[n + 4];

            if ( n > 0 )
                Array.Copy(str.Payload, 0, msg, 2 , n);

            msg[0] = str.DeviceAddress;
            msg[1] = str.Command;
            
            ushort crc = ModbusCRC_Calc( msg , n+1);
            msg[2+n] = (byte)(crc & 0xff);
            msg[3+n] = (byte)((crc >> 8) & 0xff);
        }
    }


    // Base for a single instance of a WaveShare 32-relay modbus unit

    public class CRelay32
    {
        public static class RelayCmds
        {
            public static byte ReadStatus = 1;
            public static byte ReadAddressAndVersion = 3;
            public static byte WriteSingleRelay = 5;
            public static byte SetBaudRateAndAddress = 6;
            public static byte WriteAllRelays = 0xf;
        }
        public struct CRelayIndex
        {
            public int K1; public int K2; public int K3; public int K4; public int K5; public int K6; public int K7; public int K8; public int K9; public int K10; public int K11; public int K12; public int K13; public int K14; public int K15;
            public int K16; public int K17; public int K18; public int K19; public int K20; public int K21; public int K22; public int K23; public int K24; public int K25; public int K26; public int K27; public int K28; public int K29; public int K30; public int K31; public int K32;
            public void Init()
            {
                K1 = 0; K2 = 1; K3 = 2; K4 = 3; K5 = 4; K6 = 5; K7 = 6; K8 = 7; K9 = 8; K10 = 9; K11 = 10; K12 = 11; K13 = 12; K14 = 13; K15 = 14; K16 = 15;
                K17 = 16; K18 = 17; K19 = 18; K20 = 19; K21 = 20; K22 = 21; K23 = 22; K24 = 23; K25 = 24; K26 = 25; K27 = 26; K28 = 27; K29 = 28; K30 = 29; K31 = 30; K32 = 31;
            }
        };
        public CModBusStr ReadState = new CModBusStr();
        public CModBusStr ReadVersion = new CModBusStr();
        public CModBusStr WriteState = new CModBusStr();
        public CModBusStr ReadAddress = new CModBusStr();
        public CModBusStr SetDeviceAddressMsg = new CModBusStr();

        public bool[] RelayState = new bool[32];
        public bool[] RelayNextState = new bool[32];
        public CRelayIndex Index;

        public CModbusInterpreter Interpreter = CModbusInterpreter.Instance; //  new CInterpreter(mutex);
        public byte address;
        public int SwRevision;
        bool LittleEndian; 

        public void SetEndianness(bool _LittleEndian)
        {
            LittleEndian = _LittleEndian;
        }

        public void SetAddress( byte Address)
        { 
            if ( Address < 1 || Address >  7 )
            {
                return; 
            }
            address = (byte)Address; 
            DefineCommands(Address); 

        } 

        public CRelay32( byte address_in, bool _LittleEndian   )
        {
            Index.Init();
            LittleEndian = _LittleEndian; 

            for (int cnt = 0; cnt < 32; cnt++) RelayNextState[cnt] = false; 

            address = address_in;
            DefineCommands(address);

        }

        private void DefineCommands(byte _address)
        {
            ReadState.DeviceAddress = _address;
            ReadState.Command = RelayCmds.ReadStatus;
            ReadState.Payload = new byte[] { 0, 0, 0, 32 };
            ReadState.ExpectedReturnLength = 5;
            ReadState.Answer = new byte[ReadState.ExpectedReturnLength];

            ReadVersion.DeviceAddress = _address;
            ReadVersion.Command = RelayCmds.ReadAddressAndVersion;
            ReadVersion.Payload = new byte[] { 128, 0, 0, 1 };
            ReadVersion.ExpectedReturnLength = 5;
            ReadVersion.Answer = new byte[ReadVersion.ExpectedReturnLength];

            ReadAddress.DeviceAddress = 0;
            ReadAddress.Command = RelayCmds.ReadAddressAndVersion;
            ReadAddress.Payload = new byte[] { 64, 0, 0, 1 };
            ReadAddress.ExpectedReturnLength = 5;
            ReadAddress.Answer = new byte[ReadAddress.ExpectedReturnLength];

            WriteState.DeviceAddress = _address;
            WriteState.Command = RelayCmds.WriteAllRelays;
            WriteState.Payload = new byte[] { 0, 0, 0, 32, 4, 0, 0, 0, 0 };
            WriteState.ExpectedReturnLength = 6;
            WriteState.Answer = new byte[WriteState.ExpectedReturnLength];

            SetDeviceAddressMsg.DeviceAddress = _address;
            SetDeviceAddressMsg.Command = AdcCmds.WriteHoldingReg;
            SetDeviceAddressMsg.Payload = new byte[] { 64, 0, 0, 0 };
            SetDeviceAddressMsg.ExpectedReturnLength = 6;
            SetDeviceAddressMsg.Answer = new byte[SetDeviceAddressMsg.ExpectedReturnLength];


        }

        public bool ReadRelayState()
        {
            if (  Interpreter.SendModbus(ref ReadState))
            {
                if ( ReadState.Answer[0] != 4)
                {
                    return false; 
                }
                int bassa;
                if (LittleEndian)
                {
                    bassa = ReadState.Answer[1] + ((int)ReadState.Answer[2] << 8) + ((int)ReadState.Answer[3] << 16) + ((int)ReadState.Answer[4] << 24);
                }
                else
                {
                    bassa = ReadState.Answer[4] + ((int)ReadState.Answer[3] << 8) + ((int)ReadState.Answer[2] << 16) + ((int)ReadState.Answer[1] << 24);
                }
                for ( int cnt = 0; cnt < 32; cnt++ )
                {
                    RelayState[cnt] = (((bassa >> cnt) & 1) != 0);
                }
                return true; 
            }
            return false; 
        }


        public bool SetDeviceAddress(byte NewAddress)
        {
            SetDeviceAddressMsg.Payload[3] = NewAddress;
            if (Interpreter.SendModbus(ref SetDeviceAddressMsg))
            {
                if ((SetDeviceAddressMsg.Answer[5] != NewAddress) || (SetDeviceAddressMsg.Answer[1] != 6))
                {
                    return false;
                }
                address = NewAddress;
                DefineCommands(NewAddress);
                return true;
            }
            return false;
        }


        public bool ReadDeviceAddress(out byte address)
        {
            address = 0; 
            if (Interpreter.SendModbus(ref ReadAddress))
            {
                if (( ReadAddress.Answer[1] == 3) && (ReadAddress.Answer[2] == 2)) 
                {
                    address = ReadAddress.Answer[4];
                    return true; 
                }
            }
            return false;
        }


        public bool ReadDeviceSwVersion(byte _address = 255 )
        {
            if (_address != 255 )
            {
                if (_address != ReadVersion.DeviceAddress )
                {
                    SetAddress(_address);
                }
            }
            if (Interpreter.SendModbus(ref ReadVersion))
            {
                if ((ReadVersion.Answer[0] == address) && (ReadVersion.Answer[1] == ReadVersion.Command))
                {
                    SwRevision = (int)ReadVersion.Answer[3] * 256 + ReadVersion.Answer[4];
                    return true;
                }
            }
            return false;
        }


        public bool WriteRelayState(byte _DeviceAddress = 255)
        {
            if (_DeviceAddress != 255)
            {
                if (_DeviceAddress != WriteState.DeviceAddress)
                {
                    SetAddress(_DeviceAddress);
                }
            }
            uint NextWrite = 0;
            for (int cnt = 0; cnt < 32; cnt++)
            {
                if (RelayNextState[cnt])
                {
                    NextWrite |= ((uint)1 << cnt);
                }
            }
            return WriteRelayStateAsNumber(NextWrite);
        }

        public bool WriteRelayStateAsNumber(uint NextWrite, byte _DeviceAddress = 255 )
        {
            if (_DeviceAddress != 255 )
            {
                if (_DeviceAddress != WriteState.DeviceAddress)
                {
                    SetAddress(_DeviceAddress); 
                }
            }


            byte p0;
            if (LittleEndian )
            {
                p0 = (byte)(NextWrite & 0xff);
                WriteState.Payload[8] = p0;
                p0 = (byte)((NextWrite >> 8) & 0xff);
                WriteState.Payload[7] = p0;
                p0 = (byte)((NextWrite >> 16) & 0xff);
                WriteState.Payload[6] = p0;
                p0 = (byte)((NextWrite >> 24) & 0xff);
                WriteState.Payload[5] = p0;
            }
            else
            {
                p0 = (byte)(NextWrite & 0xff);
                WriteState.Payload[5] = p0;
                p0 = (byte)((NextWrite >> 8) & 0xff);
                WriteState.Payload[6] = p0;
                p0 = (byte)((NextWrite >> 16) & 0xff);
                WriteState.Payload[7] = p0;
                p0 = (byte)((NextWrite >> 24) & 0xff);
                WriteState.Payload[8] = p0;
            }
            //p0 = (byte)((p0 << 4) | (p0 >> 4));

            if (Interpreter.SendModbus(ref WriteState))
            {
                if ((WriteState.Answer[2] != 0) || (WriteState.Answer[3] != 0) || (WriteState.Answer[4] != 0) || (WriteState.Answer[5] != 32))
                {
                    return false;
                }
                Array.Copy(RelayNextState, RelayState,32);
                Thread.Sleep(20); // Allow the relay time to execute switching
                return true;
            }
            return false;
        }

    }


    // Base class for a single instance of a WaveShare 8-analog (or digital as analog)-in device
    public class CAdc8
    {
        public byte address;
        public double [] AdcOffset = new double[8];
        public double[] AdcGain = new double[8];
        public double[] AdcScale = new double[8];

        public int SwRevision = 0; 

        public CModbusInterpreter Interpreter = CModbusInterpreter.Instance; //  new CInterpreter(mutex);
        public static class AdcCmds
        {
            public static byte ReadHoldingReg = 3;
            public static byte ReadInputReg   = 4;
            public static byte WriteHoldingReg = 6;
            public static byte WriteMultipleHoldingRegs = 0x10;
        }
        public struct CAdc
        {
            public int A1; public int A2; public int A3; public int A4; public int A5; public int A6; public int A7; public int A8; 
            public void Init()
            {
                A1 = 0; A2 = 1; A3 = 2; A4 = 3; A5 = 4; A6 = 5; A7 = 6; A8 = 7;
            }
        };
        public CModBusStr ReadState = new CModBusStr();
        public CModBusStr WriteState = new CModBusStr();
        public CModBusStr ReadChannelDataType = new CModBusStr();
        public CModBusStr SetChannelDataType = new CModBusStr();
        public CModBusStr SetDeviceAddressMsg = new CModBusStr();
        public CModBusStr ReadAddress = new CModBusStr();
        public CModBusStr ReadVersion = new CModBusStr();
        public CModBusStr SetBaudRateMsg = new CModBusStr();

        public CAdc Index;
        public int [] DataBits = new int[8] ;
        public int[] DataTypeReadback = new int[8];
        public double[] Volts = new double[8];
        public double[] RawVolts = new double[8];

        public CAdc8(byte address_in  )
        {
            Index.Init();

            address = address_in;
            DefineCommands(address); 
            for ( int cnt = 0; cnt < 8; cnt++ )
            {
                AdcOffset[cnt] = 0;
                AdcGain[cnt] = 0;
                AdcScale[cnt] = 1.0 / 1000.0;
            }
        }

        public bool LoadCalibrationFile( string xlsfilePath, int ADCIdentifier)
        {
            try
            {
                if (!File.Exists(xlsfilePath))
                {
                    // Open the existing file
                    return false;
                }

                XLWorkbook workbook = new XLWorkbook(xlsfilePath);
                IXLWorksheet worksheet;
                string sheetName = "ADC" + ADCIdentifier.ToString(); 

                if (!workbook.Worksheets.Contains(sheetName))
                {
                    return false;
                }
                worksheet = workbook.Worksheet(sheetName);
                for ( int cnt = 0; cnt < 8; cnt++)
                {
                    string colstr = (2 + cnt).ToString();

                    worksheet.Cell("A" + colstr).TryGetValue(out double cc); 
                    if (cc != cnt+1)
                    {
                        return false;
                    }
                    worksheet.Cell("B" + colstr).TryGetValue(out double g );  
                    worksheet.Cell("C" + colstr).TryGetValue(out double off);
                    if (Math.Abs(g-1) < 0.1 && Math.Abs(off) < 0.3)
                    {
                        AdcGain[cnt] = g - 1.0 ;
                        AdcOffset[cnt] = off;
                    } 
                    else
                    {
                        return false; 
                    }
                }

            }
            catch
            {
                return false; 
            }
            return true; 
        }


        private void DefineCommands(byte address_in)
        {
            ReadState.DeviceAddress = address_in;
            ReadState.Command = AdcCmds.ReadInputReg;
            ReadState.Payload = new byte[] {  0, 0, 0, 8 };
            ReadState.ExpectedReturnLength = 19;
            ReadState.Answer = new byte[ReadState.ExpectedReturnLength];

            ReadChannelDataType.DeviceAddress = address_in;
            ReadChannelDataType.Command = AdcCmds.ReadHoldingReg;
            ReadChannelDataType.Payload = new byte[] {16 , 0 , 0 , 8 };
            ReadChannelDataType.ExpectedReturnLength = 19;
            ReadChannelDataType.Answer = new byte[ReadChannelDataType.ExpectedReturnLength];

            SetChannelDataType.DeviceAddress = address_in;
            SetChannelDataType.Command = AdcCmds.WriteMultipleHoldingRegs;
            SetChannelDataType.Payload = new byte[] { 16, 0, 0, 8 , 16 , 0, 4 , 0 , 4 , 0,4,0,4,0,4,0,4,0,4,0,4 };
            SetChannelDataType.ExpectedReturnLength = 6;
            SetChannelDataType.Answer = new byte[SetChannelDataType.ExpectedReturnLength];

            SetDeviceAddressMsg.DeviceAddress = address_in;
            SetDeviceAddressMsg.Command = AdcCmds.WriteHoldingReg;
            SetDeviceAddressMsg.Payload = new byte[] { 64, 0, 0, 0 };
            SetDeviceAddressMsg.ExpectedReturnLength = 6;
            SetDeviceAddressMsg.Answer = new byte[SetDeviceAddressMsg.ExpectedReturnLength];

            WriteState.DeviceAddress = address_in;
            WriteState.Command = AdcCmds.WriteHoldingReg;
            WriteState.Payload = new byte[] { 0x10, 0, 0, 0, 4 };
            WriteState.ExpectedReturnLength = 4;
            WriteState.Answer = new byte[WriteState.ExpectedReturnLength];

            ReadAddress.DeviceAddress = 0;
            ReadAddress.Command = AdcCmds.ReadHoldingReg;
            ReadAddress.Payload = new byte[] { 64, 0, 0, 1 };
            ReadAddress.ExpectedReturnLength = 5;
            ReadAddress.Answer = new byte[ReadAddress.ExpectedReturnLength];

            ReadVersion.DeviceAddress = address;
            ReadVersion.Command = AdcCmds.ReadHoldingReg;
            ReadVersion.Payload = new byte[] { 128, 0, 0, 1 };
            ReadVersion.ExpectedReturnLength = 5;
            ReadVersion.Answer = new byte[ReadVersion.ExpectedReturnLength];

            SetBaudRateMsg.DeviceAddress = address;
            SetBaudRateMsg.Command = AdcCmds.WriteHoldingReg;
            SetBaudRateMsg.Payload = new byte[] { 32, 0, 0, 1 };
            SetBaudRateMsg.ExpectedReturnLength = 6;
            SetBaudRateMsg.Answer = new byte[SetBaudRateMsg.ExpectedReturnLength];

            
        }
        public bool ReadAdcData()
        {
            if (Interpreter.SendModbus(ref ReadState))
            {
                if ((ReadState.Answer[0] != address) || (ReadState.Answer[1] != 4) || (ReadState.Answer[2] != 0x10) )
                {
                    return false;
                }
                for ( int cnt = 0; cnt < 8; cnt++)
                {
                    DataBits[cnt] = (ReadState.Answer[4 + cnt * 2] + (int)ReadState.Answer[3 + cnt * 2] * 256);
                    RawVolts[cnt] = (double)DataBits[cnt] * AdcScale[cnt];
                    Volts[cnt] = RawVolts[cnt] * (1 + AdcGain[cnt]) +  AdcOffset[cnt];
                }
                return true; 
            }
            return false; 
        }


        public bool SetBaudRate( int baudrate )
        {
            byte baudcode; 
            switch ( baudrate )
            {
                case 9600:
                    baudcode = 1;
                    break;
                case 19200:
                    baudcode = 2;
                    break;
                case 38400:
                    baudcode = 3;
                    break;
                default: 
                    return false;
            }
            SetBaudRateMsg.Payload[3] = baudcode; 

            if (Interpreter.SendModbus(ref SetBaudRateMsg))
            {
                if ((ReadState.Answer[0] != address) || (ReadState.Answer[1] != 6) || (ReadState.Answer[2] != 0x20))
                {
                    return false;
                }
                return true;
            }
            return false;
        }


        public bool GetAdcDataType()
        {
            if (Interpreter.SendModbus(ref ReadChannelDataType))
            {
                if ((ReadChannelDataType.Answer[0] != address) || (ReadChannelDataType.Answer[1] != 3) || (ReadChannelDataType.Answer[2] != 0x10))
                {
                    return false;
                }
                for (int cnt = 0; cnt < 8; cnt++)
                {
                    DataTypeReadback[cnt] = (int)ReadChannelDataType.Answer[4 + cnt * 2]  ;
                }
                return true;
            }
            return false;
        }

        public bool SetMultipleAdcDataType(byte DataType = 0 )
        {
            for ( int cnt = 0; cnt < 8; cnt++)
            {
                SetChannelDataType.Payload[6 + 2 * cnt] = DataType;
                SetChannelDataType.Payload[5 + 2 * cnt] = 0 ;
            }
            if (Interpreter.SendModbus(ref SetChannelDataType))
            {
                if ((SetChannelDataType.Answer[0] != address) || (SetChannelDataType.Answer[1] != 16 )  )
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool SetDeviceAddress( byte NewAddress )
        {
            SetDeviceAddressMsg.Payload[3] = NewAddress;
            if (Interpreter.SendModbus(ref SetDeviceAddressMsg))
            {
                if ((SetDeviceAddressMsg.Answer[5] != NewAddress) || (SetDeviceAddressMsg.Answer[1] != 6))
                {
                    return false;
                }
                address = NewAddress;
                DefineCommands(NewAddress); 
                return true;
            }
            return false;
        }

        public void SetAddress(byte Address)
        {
            if (Address < 1 || Address > 7)
            {
                return;
            }
            address = (byte)Address;
            DefineCommands(address);

        }
        public bool ReadDeviceAddress(out byte address)
        {
            address = 0;
            if (Interpreter.SendModbus(ref ReadAddress))
            {
                if (ReadAddress.Answer[1] == 3 ) 
                {
                    address = (byte)ReadAddress.Answer[4];
                    return true; 
                }
            }
            return false;
        }
        public bool ReadDeviceSwVersion()
        {
            if (Interpreter.SendModbus(ref ReadVersion))
            {
                if ( (ReadVersion.Answer[0] == address )&& (ReadVersion.Answer[1] == ReadVersion.Command)) 
                {
                    SwRevision = (int)ReadVersion.Answer[3] * 256 + ReadVersion.Answer[4];
                    return true; 
                }
            }
            return false;
        }
/*
        public bool SetDataType()
        {
            for ( byte channel = 0; channel < 8; channel++)
            {
                if ( !SetSingleDataType(channel))
                {
                    return false; 
                }
            }
            return true; 
        }

        private bool SetSingleDataType(byte channel)
        {
            WriteState.Payload[1] = channel;
            return Interpreter.SendModbus(ref WriteState);
        }
*/
        // Get a digital value 
        public bool GetDigital( byte channel , out bool value  )
        {
            value = false;
            if ( channel >= 8 )
            {
                return false;
            }
            if ( Volts[channel] < 0.8 )
            {
                return true; 
            }
            if (Volts[channel] > 2.4 )
            {
                value = true; 
                return true;
            }
            return false; 
        }
    }

    public partial class CModbusInterpreter
    {

        private static readonly CModbusInterpreter _instance = new CModbusInterpreter();
        // Private constructor to prevent instantiation from outside the class
        public static Mutex mutex;

        public CModbusCom ModbusCom = new CModbusCom(mutex); 
        private CModbusInterpreter()
        {
            // Console.WriteLine("Singleton instance created");
            mutex = new Mutex();
        }
        public static CModbusInterpreter Instance
        {
            get { return _instance; }
        }

        public bool SendModbus(ref CModBusStr str )
        {
            if (!ModbusCom.bIsOpen)
            {
                return false; 
            }

            CModbusCrc.BuildMessage( str,  out byte [] msg); 

            ModbusCom.TransmitModbusMsg(msg); 

            // Wait a timeout for receiving the response 
            for ( int cnt = 0; cnt < 75; cnt++)
            {
                Thread.Sleep(2); 
                if (ModbusCom.CollectRtuMessage(ref str) )
                {
                    return true; 
                }
            }
            return false;
        }

    }
}
