using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TesterGUI
{
    public partial class CHostInterpreter
    {
        public ulong startTime;
        public CAskStatusMsg AskStatusMsg = new CAskStatusMsg();
        public CHostCom HostCom = new CHostCom(mutex);
        public bool VerMsgNew = false;
        public bool PeriodicStatusMsgNew = false;
        public uint OutMessageCounter = 0;

        // Arrays to store graphic information 
        public uint nGrGet;
        public uint nGrPut;

        ushort[] ExceptionRead = new ushort[Literals.N_VALVES];

        public bool ServicePeriodicStatusOn = false; // Item definition: indicates the activation of periodic messages. Initially false.
        public bool RecordingFlag = false; //true if we're currently recording. false otherwise.

        public uint GuiInCharge;
        public ulong HostTimeRead;
        public double RemainTimeRead;
        public bool PeriodicUpdateAvailable = false; // Notify that a new periodic message arrived

        public CSetGUIPeriodicMessage SetGUIPeriodicMessage = new CSetGUIPeriodicMessage();
        public static Mutex mutex;


        public int SolicitedMsgCntr; 

        // Private static instance of the class
        private static readonly CHostInterpreter _instance = new CHostInterpreter();
        public bool InOfflineSession = false;
        public List<CMsgTrap> traps = new List<CMsgTrap> { }; // List initally rmpty 
        public Stopwatch stopwatch = new Stopwatch();
        public CAckInfo AckInfo = new CAckInfo();

        public bool IsComOpen = false;
        CAnswer_AxisVersion Answer_AxisVersion = new CAnswer_AxisVersion();

        // Private static instance of the class
        // Private constructor to prevent instantiation from outside the class
        private CHostInterpreter()
        {
            // Console.WriteLine("Singleton instance created");
            startTime = GetLongTime();
            mutex = new Mutex();
        }
        public Mutex GetMutex()
        {
            return mutex;
        }
        public static CHostInterpreter Instance
        {
            get { return _instance; }
        }

        public ulong GetLongTime()
        {
            DateTime currentTime = DateTime.Now;
            return (ulong)(currentTime.Millisecond * 1000.0 + currentTime.Second * 1000000.0
                + currentTime.Minute * 60000000.0 + currentTime.Hour * 3.6000e+09);
        }

        public void SetTrap(ushort opcode, Func<ushort[], int> handler, uint cntr)
        {
            traps.Add(new CMsgTrap(opcode, handler, cntr));
        }


        public void ClearTraps(bool agressive = true)
        {
            HostCom.Messages.Clear();
            traps.Clear();
            if (agressive)
            {
                HostCom.FlushComBuffer();
            }
        }

        public bool ServiceTraps()
        {
            CMsgTrap NextTrap;
            DateTime now = DateTime.Now;
            int nTraps = traps.Count;
            bool GotSomething = false;
            // Go over all the traps 
            int nMsg = HostCom.Messages.Count;
            for (int cnt = nTraps - 1; cnt >= 0; cnt--)
            {
                NextTrap = traps[cnt];
                // Go over the TX receive buffers 

                for (int c1 = nMsg - 1; c1 >= 0; c1--)
                {
                    if ((NextTrap.OpCode == HostCom.Messages[c1].OpCode) && (NextTrap.MsgCntr == HostCom.Messages[c1].MsgCount))
                    {
                        NextTrap.handler(HostCom.Messages[c1].Buf);
                        traps.RemoveAt(cnt);
                        SolicitedMsgCntr += 1;
                        GotSomething = true;
                        break;
                    }
                }
                if (now > NextTrap.tOut)
                {
                    traps.RemoveAt(cnt); // Glean outdated messages
                }
            }
            // Messages are either unsolicited or trapped 
            // All untrapped non-unsolicited messages go to hell
            for (int c1 = nMsg - 1; c1 >= 0; c1--)
            {
                if (HostCom.Messages[c1].OpCode != (ushort)Literals.Reply2GUIOpCodes.GUIAnswer_PeriodicStatus)
                {

                    HostCom.Messages.RemoveAt(c1);
                }
            }

            return GotSomething;
        }

        // General treatment of an offline transaction. 
        // The mutex blocks the periodic process till the transaction concludes
        // Apply timeout with care as in the meantime incoming periodics will be rejected. 
        public bool OfflineTransaction(int TimeOutMsec, out int ErrorCode, byte[] buf, int ExpecteDopCode, Func<ushort[], int> handler)
        {

            //mutex.WaitOne();
            InOfflineSession = true;
            ClearTraps(agressive: false);
            HostCom.SetMessage2TxQueue(buf);
            ushort opcode = (ushort)ExpecteDopCode;// ((int)buf[16] + 256 * (int)buf[17]);
            uint cntr = BitConverter.ToUInt32(buf, 10);
            SetTrap(opcode, handler, (uint)cntr);// ((uint)buf[14] + 256 * (uint)buf[15]));

            HostCom.TransmitComMsg();

            stopwatch.Reset();
            stopwatch.Start();
            ErrorCode = 0xffff;
            AckInfo.ExpCode = 0;
            while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(TimeOutMsec))
            {
                //Search answers for inquiries we sent
                HostCom.EnhancedInterceptCOMMsg();
                if (ServiceTraps())
                {
                    ErrorCode = AckInfo.ExpCode;
                    break;
                }
                Thread.Sleep(2);
            }

            // See if there is any periodic update , not destroying any accepted message

            //mutex.ReleaseMutex();
            InOfflineSession = false;
            return (ErrorCode == 0)  ;
        }

        public bool ConnectHostCom(bool action , string com, out string msg)
        {
            msg = "Ok" ;
            int colonIndex = com.IndexOf(':');
            if (colonIndex != -1)
                com = com.Substring(0, colonIndex);

            if ( action != IsComOpen)
            {
                if (IsComOpen == false)
                {
                    IsComOpen = HostCom.OpenSerialPort(com, (int)Literals.System.HostBaudRate, out _);
                    if (IsComOpen)
                    {
                        msg = string.Format("Port opened successfully");
                    }
                    else
                    {
                        msg = string.Format("Could not open port");
                    }
                }
                else
                {

                    IsComOpen = false;
                    HostCom.mySerialPort.Close();
                    msg = string.Format("Closed com port");
                }
            }

            return IsComOpen ; 
        }
        unsafe public void DataRequestMsg(ushort opcode, out byte[] buf, out uint cntr)
        {
            byte[] mbuf = new byte[20];
            fixed (byte* ptr = mbuf)
            {
                DataRequest* sPtr = (DataRequest*)ptr;
                cntr = ++OutMessageCounter;
                sPtr->Header.Fill(0, opcode, cntr, CompensateHeader: true);
                ushort* uPtr = (ushort*)ptr;
                uPtr[0] = 0xa5a5;
                uint cs = 0x10000000; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs -= *uPtr++;
                }
                *uPtr = (ushort)(cs & 0xffff);
            }
            buf = mbuf;
        }

        unsafe public int ReadAxisVersionMsg(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CAnswer_AxisVersion* sPtr = (CAnswer_AxisVersion*)ptr;
                Answer_AxisVersion = *sPtr;
            }
            return 0;
        }
    }


}
