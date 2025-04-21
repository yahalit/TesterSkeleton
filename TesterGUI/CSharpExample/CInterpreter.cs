using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace PvsGUI
{




    public partial class CInterpreter
    {
        public ulong startTime;
        public CAskStatusMsg AskStatusMsg = new CAskStatusMsg();
        public CHostCom HostCom = new CHostCom(mutex);
        public CBitInfo BitInfo = new CBitInfo();
        public CVersionMsg VersionMsg = new CVersionMsg();
        public CGetAtpSignals AtpSignals = new CGetAtpSignals(); 
        public CGUIPeriodicMessage PeriodicStatus = new CGUIPeriodicMessage();
        public CExecConfig ExecConfig = new CExecConfig();
        public CAckInfo AckInfo = new CAckInfo();
        public CDeviceExecStatus DeviceExecStatus = new CDeviceExecStatus();
        public CGetSdoBroadcast GetSdoBroadcast = new CGetSdoBroadcast();
        public CAnswer_AxisVersion Answer_AxisVersion = new CAnswer_AxisVersion();
        public CGUI_SetDebugPeriodicReport SetDebugPeriodicReport = new CGUI_SetDebugPeriodicReport();

        public CTakeOver TakeOver = new CTakeOver();
        public CYieldControl YieldControl = new CYieldControl();
        public CSimulatorState SimulatorState = new CSimulatorState();
        public CSimulatorId SimulatorId = new CSimulatorId();
        public CActiveMapping ActiveMap = new CActiveMapping();
        public CValveMapping ValveMapping = new CValveMapping();
        public CSingleParametersSet SingleModelParamsSet = new CSingleParametersSet();

        public CKillAndReboot KillAndReboot = new CKillAndReboot();
        public bool VerMsgNew = false;
        public bool PeriodicStatusMsgNew = false;
        public uint OutMessageCounter = 0;

        // Arrays to store graphic information 
        public uint nGrGet;
        public uint nGrPut;
        public ushort[] SimFaultModeRead = new ushort[Literals.N_VALVES];
        public ushort[] FaultCodeRead = new ushort[Literals.N_VALVES];

        public double[] PosRead = new double[Literals.N_VALVES];
        public double[] CurRead = new double[Literals.N_VALVES];
        public double[] InVoltage = new double[Literals.N_VALVES];
        public double[] OutVolts = new double[Literals.N_VALVES];
        public uint[] ValveStatRead = new uint[Literals.N_VALVES];
        ushort[] ExceptionRead = new ushort[Literals.N_VALVES];

        public bool ServicePeriodicStatusOn = false; // Item definition: indicates the activation of periodic messages. Initially false.
        public bool RecordingFlag = false; //true if we're currently recording. false otherwise.

        public uint GuiInCharge;
        public ulong HostTimeRead;
        public double RemainTimeRead;
        public double LoadTempRead;
        public double ElectronicsTempRead;
        public int []  ValveStateRead = new int [Literals.N_VALVES];
        public uint[] ValveSimStateRead = new uint[14];
        public bool PeriodicUpdateAvailable = false; // Notify that a new periodic message arrived
        public int [] ValveOpenCloseStat = new int[Literals.N_VALVES];
        public int AckExpCode = 0;

        public bool bGUITakesOver = false;

        public bool uploadedModels = false;

        public CSetGUIPeriodicMessage SetGUIPeriodicMessage = new CSetGUIPeriodicMessage();
        public static Mutex mutex;

        //public bool GraphicsOpen = false;
        public bool IsComOpen = false;

        // Private static instance of the class
        private static readonly CInterpreter _instance = new CInterpreter();
        List<string> ExpText;
        List<string> ExpDetail;
        List<int> ExpNum;

        // Private static instance of the class
        // Private constructor to prevent instantiation from outside the class
        private CInterpreter()
        {
            // Console.WriteLine("Singleton instance created");
            startTime = GetLongTime();
            mutex = new Mutex();
            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                SlaveBit[cnt] = new CSlaveBit();
            }
        }
        public Mutex GetMutex()
        {
            return mutex;
        }

        public bool PopulateErrorList(string FilePath)
        {
            return ParseExceptionStatements(FilePath, out ExpText, out ExpDetail ,out ExpNum);
        } 

        // Public static method to access the single instance
        public static CInterpreter Instance
        {
            get { return _instance; }
        }

        public CSystemBit SystemBit = new CSystemBit();
        public CSlaveBit[] SlaveBit = new CSlaveBit[Literals.N_VALVES];


        public ulong GetLongTime()
        {
            DateTime currentTime = DateTime.Now;
            return (ulong)(currentTime.Millisecond * 1000.0 + currentTime.Second * 1000000.0
                + currentTime.Minute * 60000000.0 + currentTime.Hour * 3.6000e+09);
        }

        /*       public void WriteHeader(ulong Time, uint MessageNum , ushort len , ushort opcode , BinaryWriter writer)
               {
                   writer.Write(Time);
                   writer.Write(MessageNum);
                   writer.Write(len);
                   writer.Write(opcode); 
               }
        */
        unsafe public int ReadVersionMsg(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CVersionMsg* sPtr = (CVersionMsg*)ptr;
                VersionMsg = *sPtr;
            }
            VerMsgNew = true;
            return 0;
        }

        unsafe public int ReadAtpSignals(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CGetAtpSignals* sPtr = (CGetAtpSignals*)ptr;
                AtpSignals = *sPtr;
            }
            return 0;
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

        unsafe public int ReadStatusPeriodMsg(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CGUIPeriodicMessage* sPtr = (CGUIPeriodicMessage*)ptr;
                PeriodicStatus = *sPtr;
            }
            PeriodicStatusMsgNew = true;
            return 0;
        }


        public void FillExecConfig(uint[] options, ulong startTime, ulong LifeTime, bool simmode)
        {
            ExecConfig.ValveMode0 = (ushort)options[0];
            ExecConfig.ValveMode1 = (ushort)options[1];
            ExecConfig.ValveMode2 = (ushort)options[2];
            ExecConfig.ValveMode3 = (ushort)options[3];
            ExecConfig.ValveMode4 = (ushort)options[4];
            ExecConfig.ValveMode5 = (ushort)options[5];
            ExecConfig.ValveMode6 = (ushort)options[6];
            ExecConfig.ValveMode7 = (ushort)options[7];
            ExecConfig.ValveMode8 = (ushort)options[8];
            ExecConfig.ValveMode9 = (ushort)options[9];
            ExecConfig.ValveMode10 = (ushort)options[10];
            ExecConfig.ValveMode11 = (ushort)options[11];
            ExecConfig.ValveMode12 = (ushort)options[12];
            ExecConfig.ValveMode13 = (ushort)options[13];
            ExecConfig.StartExecTime = startTime;
            ExecConfig.LifeTime = LifeTime;
            if (simmode)
            {
                ExecConfig.Spare = 9876;
            }
            else
            {
                ExecConfig.Spare = 0;

            }
        }

        unsafe public int ReadBroadcastSDO(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CGetSdoBroadcast* sPtr = (CGetSdoBroadcast*)ptr;
                GetSdoBroadcast = *sPtr;
            }
            return 0;
        }


        unsafe public int ReadErrorCodeFromAcknowledge(ushort[] buf)
        {
            CAckInfo a; 
            fixed (ushort* ptr = buf)
            {
                CAckInfo* sPtr = (CAckInfo*)ptr;
                a = *sPtr;
            }
            return a.ExpCode;
        }

        unsafe public int ReadAcknowledge(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CAckInfo* sPtr = (CAckInfo*)ptr;
                AckInfo = *sPtr;
            }
            return 0;
        }

        /* 
         * Prepare any contents-free data request 
         */
        unsafe public void DataRequestMsg(ushort opcode, out byte[] buf, out uint cntr)
        {
            byte[] mbuf = new byte[20];
            fixed (byte* ptr = mbuf)
            {
                DataRequest* sPtr = (DataRequest*)ptr;
                cntr = ++OutMessageCounter;
                sPtr->Header.Fill(0, opcode, cntr, CompensateHeader:true);
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
        /* 
         * Prepare any set message with payload 
         */
        unsafe public void DataSetMsg(ushort opcode, ushort[] payload, out byte[] buf, out uint cntr)
        {
            byte[] mbuf = new byte[20 + payload.Length * 2];
            uint cs = 0x10000000; // Thats ~0xa5a5 
            fixed (byte* ptr = mbuf)
            {
                ushort* uPtr = (ushort*)ptr;
                {
                    DataRequest* sPtr = (DataRequest*)ptr;
                    cntr = ++OutMessageCounter;
                    sPtr->Header.Fill((ushort)payload.Length, opcode, cntr, CompensateHeader: true);
                    for (int cnt = 0; cnt < payload.Length; cnt++)
                    {
                        uPtr[9 + cnt] = payload[cnt];
                    }
                    uPtr[0] = 0xa5a5;
                    for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                    {
                        cs -= uPtr[cnt];
                    }
                    uPtr[mbuf.Length / 2 - 1] = (ushort)(cs & 0xffff);
                }
            }
            buf = mbuf;
        }

        unsafe public void PrepDeviceConfigMsg(ushort modelset, out byte[] buf, out uint cntr)
        {
            //MemoryStream stream = new MemoryStream(); 
            byte[] mbuf = new byte[Marshal.SizeOf(typeof(CSetModelSelectMsg))];
            fixed (byte* ptr = mbuf)
            {
                CSetModelSelectMsg* sPtr = (CSetModelSelectMsg*)ptr;
                sPtr->Preamble = 0xa5a5;
                cntr = ++OutMessageCounter;
                sPtr->Header.Fill(2, 0xa001, cntr, CompensateHeader:true);
                ushort* uPtr = (ushort*)ptr;
                sPtr->modelset = modelset;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }

        unsafe public void PrepStatusPeriodMsg(ushort msec, out byte[] buf, out uint cntr)
        {
            //MemoryStream stream = new MemoryStream(); 
            byte[] mbuf = new byte[Marshal.SizeOf(typeof(CAskStatusMsg))];
            fixed (byte* ptr = mbuf)
            {
                CAskStatusMsg* sPtr = (CAskStatusMsg*)ptr;
                sPtr->Preamble = 0xa5a5;
                cntr = ++OutMessageCounter;
                sPtr->Header.Fill(2, 0xa001, cntr,true);
                ushort* uPtr = (ushort*)ptr;
                sPtr->milisec = msec;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }



        unsafe public void PrepSetGUIPeriodicMessage(CSetGUIPeriodicMessage cfg, out byte[] buf, out uint cntr)
        {
            //MemoryStream stream = new MemoryStream(); 
            int MsgLen = Marshal.SizeOf(typeof(CSetGUIPeriodicMessage));
            byte[] mbuf = new byte[MsgLen];
            fixed (byte* ptr = mbuf)
            {
                //CExecConfig* sPtr = 
                cfg.preamble = 0xa5a5;
                cntr = ++OutMessageCounter;
                cfg.Header.Fill((ushort)(MsgLen / 2), (ushort)Literals.GUIOpCodes.GUI_SetGUIPeriodicMessage, cntr, CompensateHeader: false);
                *(CSetGUIPeriodicMessage*)ptr = cfg;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }


        unsafe public void PrepKillAndRebootMsg(CKillAndReboot cfg, out byte[] buf, out uint cntr)
        {
            //MemoryStream stream = new MemoryStream(); 
            byte[] mbuf = new byte[Marshal.SizeOf(typeof(CKillAndReboot))];
            fixed (byte* ptr = mbuf)
            {
                //CExecConfig* sPtr = 
                cfg.preamble = 0xa5a5;
                cntr = ++OutMessageCounter;
                cfg.Header.Fill((ushort)(sizeof(CKillAndReboot)/2), (ushort) Literals.GUIOpCodes.GUIOpCode_KillReboot, cntr, CompensateHeader: false);
                cfg.pass = (uint)0x12345678;
                *(CKillAndReboot*)ptr = cfg;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
        unsafe public void PrepTakeOverMsg(CTakeOver cfg, out byte[] buf, out uint cntr)
        {
            //MemoryStream stream = new MemoryStream(); 
            byte[] mbuf = new byte[Marshal.SizeOf(typeof(CTakeOver))];
            fixed (byte* ptr = mbuf)
            {
                cfg.Preamble = 0xa5a5;
                cntr = ++OutMessageCounter;
                cfg.Header.Fill(len_in_words: (ushort)(sizeof(CTakeOver) / 2), opcode_in: (ushort)Literals.GUIOpCodes.GUI_TakeOverSimulatorFromHost,
                    MessageNum_in: cntr, CompensateHeader: false);

                cfg.Code1 = (ushort)1234; 
                cfg.Code2 = (ushort)5678;
                * (CTakeOver*)ptr = cfg;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
        unsafe public void PrepYieldControlMsg(CYieldControl cfg, out byte[] buf, out uint cntr)
        {
            //MemoryStream stream = new MemoryStream(); 
            byte[] mbuf = new byte[Marshal.SizeOf(typeof(CYieldControl))];
            fixed (byte* ptr = mbuf)
            {
                cfg.Preamble = 0xa5a5;
                cntr = ++OutMessageCounter;
                cfg.Header.Fill(len_in_words: (ushort)(sizeof(CYieldControl) / 2), opcode_in: (ushort)Literals.GUIOpCodes.GUI_YieldSimulatorToHost,
                    MessageNum_in: cntr, CompensateHeader: false);

                cfg.Code1 = (ushort)1234;
                cfg.Code2 = (ushort)5678;
                *(CYieldControl*)ptr = cfg;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }

        public void SetLedColor(PictureBox pictureBoxInControl, string color)
        {
            // Dispose of the old image to free up resources (if needed)
            pictureBoxInControl.Image?.Dispose();

            switch (color)
            {
                case "Red":
                    pictureBoxInControl.Image = Properties.Resources.REDLED;
                    break;
                case "Blue":
                    pictureBoxInControl.Image = Properties.Resources.BLUELED;
                    break;
                case "Green":
                    pictureBoxInControl.Image = Properties.Resources.GREENLED;
                    break;
                default:
                    pictureBoxInControl.Image = Properties.Resources.GRAYLED;
                    break;
            }
        }


    }

}

