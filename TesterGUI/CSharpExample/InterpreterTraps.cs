using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using DocumentFormat.OpenXml.Office.CustomUI;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PvsGUI
{

    public class ManualResetEventHandler
    {
        public ManualResetEvent _manualResetEvent;

        public ManualResetEventHandler()
        {
            _manualResetEvent = new ManualResetEvent(false);
        }

        // Method to trigger the event and wait for it with a timeout
        public bool TriggerAndWait(int timeoutMilliseconds)
        {
            _manualResetEvent.Reset(); // Ensure the event is not set before starting

            // Simulate some work or set the event in another thread
            // Example: ThreadPool.QueueUserWorkItem(state => SimulateWork());

            // Wait for the event to be signaled or timeout
            bool isSignaled = _manualResetEvent.WaitOne(timeoutMilliseconds);

            return isSignaled; // Return true if the event was signaled, false if it timed out
        }
    }
    public partial class CInterpreter
    {
        public static uint NWORDS_PREAMBLE = 8; 
        public uint PeriodicMsgCntr = 0;
        public uint SolicitedMsgCntr = 0;
        public List<CMsgTrap> traps = new List<CMsgTrap> { }; // List initally rmpty 
        public Stopwatch stopwatch = new Stopwatch();
        public CMsgHeader gHeader = new CMsgHeader();
        public ManualResetEventHandler VersionMsgManualResetEvent = new ManualResetEventHandler();
        public ManualResetEvent GetVersionManualEvent;
        public ManualResetEvent SetAckManualEvent;
        public CSystemBit SystemBIT;
        public CGetSetCalibParam CalibParam = new CGetSetCalibParam() ; 
        public CSetFanSSRValues SetFanSSRValues = new CSetFanSSRValues();
        public CSetFwTestSerialFlash SetFwTestSerialFlash = new CSetFwTestSerialFlash(); 
        public CSetPressure SetPressure = new CSetPressure(); 
        public CSetValveCurrentTestValue SetValveCurrentTestValue = new CSetValveCurrentTestValue();
        public CSetDiscreteTestValues SetDiscreteTestValues = new CSetDiscreteTestValues();
        public GUIAnswer_RequestValveStatusReport GUIAnswer_RequestValveStatusReport = new GUIAnswer_RequestValveStatusReport();
        public CGUI_SetInstallation GUI_SetInstallation = new CGUI_SetInstallation();
        public GUI_SetUnitProductionData CSetUnitProductionData = new GUI_SetUnitProductionData();
        public GUIAnswer_GetUnitData Answer_GetUnitData = new GUIAnswer_GetUnitData(); 
        public CSetID SetID = new CSetID();
        public CSetMapping SetMappingEUT = new CSetMapping();
        public CSetActiveMapping SetActiveMap = new CSetActiveMapping();
        public CSetSingleModelParamsSet SetSingleModelParamsSet = new CSetSingleModelParamsSet();
        public CSetAtpConversion SetAtpConversion = new CSetAtpConversion();
        public CGetMapping GetMappingEUT = new CGetMapping();
        public CGetSingleModelParamsSet GetSingleModeParamsSet = new CGetSingleModelParamsSet();
        public List<CSlaveBit> SlaveBitList = new List<CSlaveBit>(); 
        public double[] SlaveTemperature = new double[Literals.N_VALVES] ;
        public bool[] IDvalues = new bool[4];
        public int activeCurrentMapping = new int();

        public bool InOfflineSession = false;
        public int CpuHwVer;
        public int CpuHwSubVer;
        public int CpuSerialNumber;
        public DateTime CpuProdDate;

        // Set a new trap, and return the index of the trap last set 
        public int SetTrap(ushort opcode, Func<ushort[], int> handler, uint cntr )
        {
            traps.Add(new CMsgTrap(opcode, handler,cntr));
            return traps.Count - 1; 
        }


        /* 
         *  Define waveforms ot be transmitted at the periodic message instead of true measurements. Meant for debugging of the graphics
         */
        public bool ProgramDebugPeriodicSignals(bool On, double AmplitudeCurrent, double AmplitudePos, double AmplitudePressure, double AmplitudeVolts,
                    double Frequency)
        {
            CGUI_SetDebugPeriodicReport rp = new CGUI_SetDebugPeriodicReport();
            rp.Fill(On,  AmplitudeCurrent,  AmplitudePos,  AmplitudePressure,  AmplitudeVolts,Frequency, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge); 
        }


        public bool SetPressureJ1J7(double PressureVolts, int index)
        {
            SetPressure.Fill(PressureVolts, index, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }


        public bool SetUnitProductionData(int HwVer, int HwSubver, int SerialNumber, DateTime ProdTime )
        {
            CSetUnitProductionData.Fill(HwVer, HwSubver , SerialNumber, ProdTime , AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool SetAtpConversionCmd(int[] volts,  bool [] ConvertOn , bool ManualMode = true )
        {
            SetAtpConversion.Fill(volts, ConvertOn ,AllocateMessageCounter(), out byte[] buf, ManualMode);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool SetAnalogCurrentReport( double volts, int index )
        {
            SetValveCurrentTestValue.Fill(volts, index, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool SetGUIInCharge()
        {
			PrepTakeOverMsg(TakeOver, out byte[] buf, out uint cntr);
			return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
		}

		public bool GetSimulatorState()
        {
            // Prep the request message
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetSimState, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_SimulatorState, ReadSimulatorState);
        }


        public bool GetCalibration()
        {
            // Prep the request message
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetCalibration, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetCalibration, ReadCalibration);
        }


        public bool SetAdcCalibration(double[] gain, double[] offset, bool burn)
        {
            CalibParam.FillAdcCalib(gain, offset,burn, AllocateMessageCounter(), out byte[] buf);

            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }


        public bool SetTPCalibration(double[] gain, double[] offset, bool burn)
        {
            CalibParam.FillTPCalib(gain, offset, burn, AllocateMessageCounter(), out byte[] buf);

            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }


        public bool GetActiveMapping()
        {
            // Prep the request message
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetMappingIndexFromFlash, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetMappingIndexFromFlash, ReadActiveMapping);
        }

        public bool GetSingleModeParamsSetFromEUT(int model)
        {
            // Prep the request message
            GetSingleModeParamsSet.Fill(model, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetSingleModelParamsSet, ReadSingleModelParamsSet);

            //DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetModel2ValveMapping, out byte[] buf, out uint cntr);
            //return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetModel2ValveMapping, ReadValveMapping);
        }
        public bool GetMappingFromEUT(int ParSet)
        {
            // Prep the request message
            GetMappingEUT.Fill(ParSet, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetModel2ValveMapping, ReadValveMapping);

            //DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetModel2ValveMapping, out byte[] buf, out uint cntr);
            //return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetModel2ValveMapping, ReadValveMapping);
        }
        public bool GetSimulatorId()
        {
            // Prep the request message
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetSimID, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_SimulatorId, ReadSimulatorId);
        }

        public bool GetSlaveStatus()
        {
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_RequestValveStatusReport, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_RequestValveStatusReport, ReadSlaveStatusReport);
        }


        public bool GetUnitProductionData()
        {
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetUnitData, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_GetUnitData, ReadUnitData);
        }

        public bool SetDiscreteSignals(bool[] values)
        {
            // Prep the request message
            SetDiscreteTestValues.Fill(values, AllocateMessageCounter(), out byte[] buf, DiscreteManualMode : true); 
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }
        public bool SetSimulatorId(bool[] values, bool ManualMode, bool TestMode)
        {
            // Prep the request message
            SetID.Fill(values, AllocateMessageCounter(), out byte[] buf, ManualMode, TestMode);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool SetMappingToEUT(int[] map, int valid, int parSet)
        {
            //        public void Fill(int [] map, int valid, int parSet, uint cntr, out byte[] buf)

            // Prep the request message
            SetMappingEUT.Fill(map, valid, parSet, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        //SetSingleModelParamsSet
        public bool SetSingleModelParamsetersSet(float[] parameters, int model, int valid)
        {
            // Prep the request message
            SetSingleModelParamsSet.Fill(parameters, model, valid, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool SetActiveMapping(int activeMapping)
        {
            // Prep the request message
            SetActiveMap.Fill(activeMapping, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool SetSSRAndFans(bool SetSSR, bool SSRValue, bool SetFan, bool Fan1Value, bool Fan2Value)
        {
            SetFanSSRValues.Fill(SetSSR, SSRValue, SetFan, Fan1Value, Fan2Value, AllocateMessageCounter(), out byte [] buf);
			return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
		}

        public bool TestSerialFlash(int RandomSeed)
        {
            SetFwTestSerialFlash.Fill(RandomSeed, AllocateMessageCounter(), out byte[] buf);
            return OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool GetAtpSignals()
        {
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_GetAtpSignals, out byte[] buf, out uint _);
            return OfflineTransaction(100, out int ErrorCode0, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerGetAtpSignals, ReadAtpSignals);
        }

        public bool SetInstallation(bool[] values, bool valid = true )
        {
            GUI_SetInstallation.Fill(values, AllocateMessageCounter(), out byte[] buf , valid ); 
            return OfflineTransaction(100, out int ErrorCode0, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge);
        }

        public bool GetVersionOffline()
        {
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_RequestVersion, out byte[] buf, out uint cntr);
            return OfflineTransaction(100, out int ErrorCode0, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_VersionReport, ReadVersionMsg);
        }


        public int ReadSimulatorState(ushort[] buf)
        {
            SimulatorState.Fill(buf, out SystemBIT);
            //Interpreter.ServicePeriodicStatusOn = SystemBIT.GUITxPeriodic;
            return 0;
        }

        public int ReadCalibration(ushort[] buf)
        {
            CalibParam.Fill(buf);
            //Interpreter.ServicePeriodicStatusOn = SystemBIT.GUITxPeriodic;
            return 0;
        }

        public int ReadActiveMapping(ushort[] buf)
        {
            ActiveMap.Fill(buf, out activeCurrentMapping);
            return activeCurrentMapping;
        }

        public int ReadSingleModelParamsSet(ushort[] buf)
        {
            SingleModelParamsSet.Fill(buf);
            return 0;
        }
        public int ReadValveMapping(ushort[] buf)
        {
            ValveMapping.Fill(buf);
            return 0 ;
        }
        
        public int ReadSimulatorId(ushort[] buf)
        {
            SimulatorId.Fill(buf, out IDvalues);
            //SimulatorId.GetIDValues(out IDvalues);
            //Interpreter.ServicePeriodicStatusOn = SystemBIT.GUITxPeriodic;
            return 0;
        }
 
        public int ReadSlaveStatusReport(ushort[] buf)
        {
            GUIAnswer_RequestValveStatusReport.Fill(buf, out SlaveBitList, out  SlaveTemperature); 
            return 0;
        }


        public int ReadUnitData(ushort[] buf)
        {
            Answer_GetUnitData.Fill(buf, out CpuHwVer, out CpuHwSubVer, out CpuSerialNumber, out CpuProdDate);
            return 0; 
        }

        public unsafe void SetAcknowledgeTrap( byte []buf , uint cntr , bool DoTrap = true )
        {
            //MemoryStream stream = new MemoryStream(); 
            ushort cs = 0 ; 
            fixed (byte* ptr = buf)
            {
                gHeader = *(CMsgHeader*)ptr;
                gHeader.MessageNum = cntr ;
                ushort* uPtr = (ushort*)ptr;
                for (int cnt = 0; cnt < (buf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            HostCom.SetMessage2TxQueue(buf);
            if ( DoTrap )
            {
                SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge, gHeader.MessageNum);
            }
        }


        void AddDvec2GList(List <double> [] L , double [] vec)
        {
            for ( int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                L[cnt].Add(vec[cnt]); 
            }
            if (L[0].Count > Literals.MaxListLength)
            {
                for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
                {
                    L[cnt].RemoveAt(0);
                }
            } 
        }

        // Allocate a counter for a new message 
        public uint AllocateMessageCounter()
        {
            OutMessageCounter += 1;
            return OutMessageCounter;
        }

        // General treatment of an offline transaction. 
        // The mutex blocks the periodic process till the transaction concludes
        // Apply timeout with care as in the meantime incoming periodics will be rejected. 
        public bool OfflineTransaction(int TimeOutMsec, out int ErrorCode, byte[] buf, int ExpecteDopCode , Func<ushort[], int> handler)
        {

            //mutex.WaitOne();
            InOfflineSession = true; 
            ClearTraps(agressive: false);
            HostCom.SetMessage2TxQueue(buf);
            ushort opcode = (ushort)ExpecteDopCode;// ((int)buf[16] + 256 * (int)buf[17]);
            uint cntr = BitConverter.ToUInt32(buf, 10);
            int nTrap = SetTrap(opcode, handler, (uint)cntr);// ((uint)buf[14] + 256 * (uint)buf[15]));

            HostCom.TransmitComMsg();

            stopwatch.Reset();
            stopwatch.Start();
            ErrorCode = 0xffff;
            AckInfo.ExpCode = 0;
            while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(TimeOutMsec))
            {
                //Search answers for inquiries we sent
                HostCom.EnhancedInterceptCOMMsg();
                AckExpCode = 0;
                if (ServiceSpecificTrap(nTrap))
                {
                    ErrorCode = AckExpCode;
                    break; 
                }
                Thread.Sleep(2);
            }

            // See if there is any periodic update , not destroying any accepted message

            //mutex.ReleaseMutex();
            InOfflineSession = false ;
            return (ErrorCode == 0);
         }

        /*
          * Read a periodic message. If graphic bufferes are installed, add the contents to the graphic buffers
          * Returns: 
          * true: Contents added to the graphics buffers
          */


        unsafe public bool ReadUnsolicitedGUIPeriodMsg(ushort[] buf, ref CSystemBit SystemBit, CSlaveBit[] SlaveBit, bool CollectGraphics)
        {

            double[] OutVoltRaw = new double[Literals.N_VALVES];
            //CGUIPeriodicMessage msg = new CGUIPeriodicMessage() ;

            fixed (ushort* ptr = buf)
            {
                CGUIPeriodicMessage* sPtr = (CGUIPeriodicMessage*)ptr;
                PeriodicStatus = *sPtr;
            }


            mutex.WaitOne();
            SystemBit.Fill(PeriodicStatus.Bit);
            // Out volts can be either output voltage or exception 
            PeriodicStatus.OutVolts.CopyToDouble(OutVoltRaw, 1);


            // Update interpreter internal state 
            PeriodicStatus.InCurrent.CopyToDouble(CurRead, 0.001);
            PeriodicStatus.InVoltage.CopyToDouble(InVoltage, 0.01);
            PeriodicStatus.Position.CopyToDouble(PosRead, 0.0001);

            PeriodicStatus.SlaveStatus.CopyToUint(ValveStatRead);

            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                SlaveBit[cnt].Fill(ValveStatRead[cnt]); // fills info from ValveStatRead to SlaveBit
                //ValveStateRead[cnt] = ((msg.OpenedStatus>> cnt) & 1) + ((msg.OpenedStatus >> cnt) & 1) * 2; 
                if (SlaveBit[cnt].ConverterReady)
                {
                    ExceptionRead[cnt] = 0 ;
                    OutVolts[cnt] = OutVoltRaw[cnt] * 0.01 ;
                }
                else
                {
                    ExceptionRead[cnt] = (ushort)  OutVolts[cnt]   ;
                    OutVolts[cnt] = 0; 
                }
            }



            RemainTimeRead = PeriodicStatus.TimeRemain * 0.001;
            HostTimeRead = PeriodicStatus.Header.Time; //time in usec since midnight 
            LoadTempRead = PeriodicStatus.LoadTemperature - 60 ;
            ElectronicsTempRead = PeriodicStatus.ElectTemperature - 60 ;




            //extract valve statuses
            bool[] OpenStatusBooleans = new bool[Literals.N_VALVES];
            bool[] ClosedStatusBooleans = new bool[Literals.N_VALVES];
            //string[] ValveStatus = new string[Literals.N_VALVES];
            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                OpenStatusBooleans[cnt] = Convert.ToBoolean((PeriodicStatus.OpenedStatus >> cnt) & 1);
                ClosedStatusBooleans[cnt] = Convert.ToBoolean((PeriodicStatus.ClosedStatus >> cnt) & 1);

                if (OpenStatusBooleans[cnt] && !ClosedStatusBooleans[cnt])
                {
                    ValveOpenCloseStat[cnt] = 1; // opened
                }
                else if (!OpenStatusBooleans[cnt] && ClosedStatusBooleans[cnt])
                {
                    ValveOpenCloseStat[cnt] = 0; //closed
                }
                else if ((!OpenStatusBooleans[cnt] && !ClosedStatusBooleans[cnt]))
                {
                    ValveOpenCloseStat[cnt] = 2; // middle OR uninstalled 
                    //this issue is references in the presentation of this field to screen.
                }
                else //both equal 1
                //status will be no simulation, or converter is off. same for us.
                {
                    ValveOpenCloseStat[cnt] = 3; //no simulation
                    //MessageBox.Show("Invalid information regarding valves open / close status");
                    //return false;
                }
            }

            if (!CollectGraphics)
            {
                // No wish to collect graphics, nothing more to do 
                mutex.ReleaseMutex();
                return true ;  
            }


            mutex.ReleaseMutex();

            //SlaveBit = new CSlaveBit();

            /*
            double[] dPosRead = new double[14];
            double[] dCurRead = new double[14];
            ushort[] dSimFaultModeRead = new ushort[14];
            ushort[] dFaultCodeRead = new ushort[14];
            ValveTime[nGrPut] = (double)PeriodicStatus.Header.Time * 1e-6;
            // Decode position and current for message 
            DecodePeriodicPosCur(buf, dPosRead, dCurRead, dSimFaultModeRead, dFaultCodeRead);

            // Put to graphic buffer 
            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                ValvePosition[cnt, nGrPut] = dPosRead[cnt];
                ValveCurrent[cnt, nGrPut] = dCurRead[cnt];
            }
            nGrPut = (nGrPut + 1) & (GR_BUF_SIZE - 1);
            if (nGrPut == nGrGet)
            {
                nGrGet = (nGrGet + 1) & (GR_BUF_SIZE - 1);
            }
            */
            return true;
        }

       
        // Go over all the accepted periodic status messages and extract their information, then glean them from input queue  
        // Return valu: true if installing graphic collection is required 
        public bool UpdatePeriodicStatus( bool CollectGraphics )
        {
            bool RequiredGrInstall = false;
            int nMsg = HostCom.Messages.Count;
            bool[] KillList = new bool[nMsg]; 

            //for (int c1 = nMsg - 1; c1 >= 0; c1--)
            for (int c1 = 0 ; c1 < nMsg ; c1++)
            {
                if ((ushort)Literals.Reply2GUIOpCodes.GUIAnswer_PeriodicStatus == HostCom.Messages[c1].OpCode) 
                {
                    if  (!ReadUnsolicitedGUIPeriodMsg(HostCom.Messages[c1].Buf, ref SystemBit , SlaveBit, CollectGraphics))
                    {
                        RequiredGrInstall = true; 
                    }
                    //HostCom.Messages.RemoveAt(c1); // Dispose, no further need of message
                    KillList[c1] = true; 
                    PeriodicMsgCntr += 1;
                    PeriodicUpdateAvailable = true;
                }
            }
            for (int c1 = nMsg - 1; c1 >= 0; c1--)
            {
                if ( KillList[c1])
                {
                    HostCom.Messages.RemoveAt(c1);
                }
            }

            return RequiredGrInstall;
        }

        public void ClearTraps( bool agressive = true )
        {
            HostCom.Messages.Clear();
            traps.Clear();
            if (agressive)
            {
                HostCom.FlushComBuffer();
            }
        }

        public bool ServiceTraps(  )
        {
            CMsgTrap NextTrap;
            DateTime now = DateTime.Now;
            int nTraps = traps.Count;
            int _AckExpCode;
            bool GotSomething = false; 
            // Go over all the traps 
            int nMsg = HostCom.Messages.Count;
            for (int cnt = nTraps - 1; cnt >= 0; cnt--)
            {
                NextTrap = traps[cnt];
                // Go over the TX receive buffers 

                for (int c1 = nMsg - 1; c1 >= 0; c1--)
                {
                    if (NextTrap.MsgCntr == HostCom.Messages[c1].MsgCount)
                    {// Thats for us 
                        // Test for acknowledge with error                         

                        if (((int) NextTrap.OpCode == (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge ) ||
                                ((int)NextTrap.OpCode == (int)Literals.Reply2HostOpCodes.PvsAcknowledge))
                        {
                            _AckExpCode = ReadErrorCodeFromAcknowledge(HostCom.Messages[c1].Buf);
                        }
                        else
                        {
                            _AckExpCode = 0;
                        }
                        if ((NextTrap.OpCode == HostCom.Messages[c1].OpCode))
                        {
                            NextTrap.handler(HostCom.Messages[c1].Buf);
                        }

                        traps.RemoveAt(cnt); // Dispose used trap 
                        SolicitedMsgCntr += 1;

                        GotSomething = true;
                        break; 
                    }
                }
                if (now > NextTrap.tOut)
                {
                    traps.RemoveAt(cnt); // Glean outdated traps
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

            return GotSomething  ; 
        }


        public bool ServiceSpecificTrap(int nTrap)
        {
            CMsgTrap NextTrap;
            DateTime now = DateTime.Now;
            int nTraps = traps.Count;
            // Go over all the traps 
            int nMsg = HostCom.Messages.Count;
            NextTrap = traps[nTrap];
            // Go over the TX receive buffers 

            for (int c1 = nMsg - 1; c1 >= 0; c1--)
            {
                if (NextTrap.MsgCntr == HostCom.Messages[c1].MsgCount)
                {// Thats for us 
                    // Test for acknowledge with error                         

                    if (((int)NextTrap.OpCode == (int)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge) ||
                            ((int)NextTrap.OpCode == (int)Literals.Reply2HostOpCodes.PvsAcknowledge))
                    {
                        AckExpCode = ReadErrorCodeFromAcknowledge(HostCom.Messages[c1].Buf);
                    }
                    else
                    {
                        AckExpCode = 0;
                    }
                    if ((NextTrap.OpCode == HostCom.Messages[c1].OpCode))
                    {
                        NextTrap.handler(HostCom.Messages[c1].Buf);
                    }

                    traps.RemoveAt(nTrap); // Dispose used trap 
                    SolicitedMsgCntr += 1;

                    return true;
                }
            }
            return false;
        }

        public void GetPeriodicStatus(bool isbg, ushort value)
        {
            // Prep the GetBiT
            ushort[] payload = { 0 };
            payload[0] = value;

            DataSetMsg((ushort) Literals.HostOpCodes.HostOpStatusPeriod, payload, out byte[] buf, out uint cntr);
            // Send it 
            if (isbg)
                mutex.WaitOne();


            // Prep a trap ; 
            if (value == 0)
            {
                SetTrap(0xb001, ReadStatusPeriodMsg, cntr);
            }
            HostCom.SetMessage2TxQueue(buf);

            if (isbg)
                mutex.ReleaseMutex();
        }


        public void SetDeviceMode(bool isbg, ushort value)
        {
            // Prep the GetBiT
            ushort[] payload = { 0, 1333 };
            payload[0] = value;

            DataSetMsg(( ushort) Literals.HostOpCodes.HostOpSetMode , payload, out  byte[] buf, out  uint cntr);
            // Send it 
            if (isbg)
                mutex.WaitOne();


            // Prep a trap ; 
            SetTrap((ushort)Literals.Reply2HostOpCodes.PvsAcknowledge , ReadAcknowledge, cntr);
            HostCom.SetMessage2TxQueue(buf);

            if (isbg)
                mutex.ReleaseMutex();
        }


        public void SetParametersSet(bool isbg, ushort value)
        {
            // Prep the GetBiT
            ushort[] payload = { 0  };
            payload[0] = value;

            DataSetMsg((ushort)Literals.HostOpCodes.HostOpSetDeviceConfig, payload, out byte[] buf, out   uint cntr);
            // Send it 
            if (isbg)
                mutex.WaitOne();


            // Prep a trap ; 
            SetTrap((ushort)Literals.Reply2HostOpCodes.PvsAcknowledge, ReadAcknowledge, cntr);

            //HostCom.mySerialPort.Write(buf, 0, buf.Length );

            HostCom.SetMessage2TxQueue(buf);

            if (isbg)
                mutex.ReleaseMutex();
        }

        public void SetIdOut(bool isbg, ushort value)
        {
            // Prep the GetBiT
            ushort[] payload = { 0 , 0, 0, 0 };
            payload[0] = (((int)value & 1) == 0) ? (ushort)0 : (ushort)1;
            payload[1] = (((int)value & 2) == 0) ? (ushort)0 : (ushort)1;
            payload[2] = (((int)value & 4) == 0) ? (ushort)0 : (ushort)1;
            payload[3] = (((int)value & 8) == 0) ? (ushort)0 : (ushort)1;

            DataSetMsg((ushort)Literals.HostOpCodes.HostOpSetIdOut, payload, out byte[]  buf, out uint cntr);
            // Send it 
            if (isbg)
                mutex.WaitOne();


            // Prep a trap ; 
            SetTrap((ushort)Literals.Reply2HostOpCodes.PvsAcknowledge, ReadAcknowledge, cntr);

            //HostCom.mySerialPort.Write(buf, 0, buf.Length );

            HostCom.SetMessage2TxQueue(buf);

            if (isbg)
                mutex.ReleaseMutex();
        }


        // Send a do nothing, just for verifying communications
        public bool SetDoNothing(bool isbg = true)
        {
            // Prep the GetBiT
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUIDoNothing, out  byte[] buf, out  uint cntr);
            // Send it 
            if (isbg)
                mutex.WaitOne();

            SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge, cntr);

            HostCom.SetMessage2TxQueue(buf);
            if (isbg)
                mutex.ReleaseMutex();
            return true;
        }


        /* 
         * Callback for version message received. Read the verasion message and issue an acceptance event 
         */
        unsafe public int ReadVersionMsgWait(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CVersionMsg* sPtr = (CVersionMsg*)ptr;
                VersionMsg = *sPtr;
            }
            try
            {
                GetVersionManualEvent.Set();
            }
            catch
            {

            }
            VerMsgNew = true;
            return 0;
        }

        /* 
         * Get a version message. If a manual event is provided as argument, it will be signalled
         * upon reply received.
         */
        public bool GetVersion(bool isbg, ManualResetEvent evt = null )
        {
            // Prep the GetBiT
            DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_RequestVersion, out byte[] buf, out uint cntr);
            // Send it 
            if (isbg)
                mutex.WaitOne();


            // Prep a trap ; 
            if  (evt != null)
            {
                evt.Reset();
                GetVersionManualEvent = evt; 
                SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIAnswer_VersionReport, ReadVersionMsgWait, cntr);
            }
            else
            {
                SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIAnswer_VersionReport, ReadVersionMsg, cntr);
            }

            HostCom.SetMessage2TxQueue(buf);

            if (isbg)
                mutex.ReleaseMutex();
            return true; 

        }


        unsafe public int ReadAcknowledgeWait(ushort[] buf)
        {
            fixed (ushort* ptr = buf)
            {
                CAckInfo* sPtr = (CAckInfo*)ptr;
                AckInfo = *sPtr;
            }
            try
            {
                SetAckManualEvent.Set();
            }
            catch
            {

            }
            return 0;
        }



        //GUI_SetGUIPeriodicMessage
        /*
        If a manual event is provided as argument, it will be signalled
         * upon reply received.
        */ 
        public bool SendSetGUIPeriodicMessage(int msec, bool isbg = true, ManualResetEvent evt = null)
        {
            // Prep the exec message
            SetGUIPeriodicMessage.PeriodMsec =  (ushort)msec;
            SetGUIPeriodicMessage.PassKey = (ushort)1234;

            PrepSetGUIPeriodicMessage(SetGUIPeriodicMessage, out byte[] buf, out uint cntr);

            // Send it 
            if (isbg)
                mutex.WaitOne();

            // Prep a trap ; 
            if (evt != null)
            {
                evt.Reset();
                SetAckManualEvent = evt;
                SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledgeWait, cntr);
            }
            else
            {
                SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, ReadAcknowledge, cntr);
            }

            HostCom.SetMessage2TxQueue(buf);

            if (isbg)
                mutex.ReleaseMutex();

            return true;
        }


        public void DecodePeriodicPosCur(ushort[] buf, double[] pos, double[] cur , ushort [] SimFaultMode , ushort[] faultCode)
        {
            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                ushort word1 = (ushort) buf[cnt * 2 + NWORDS_PREAMBLE];
                ushort word2 = (ushort) buf[cnt * 2 + 1 + NWORDS_PREAMBLE];
                short sword2 = (short)buf[cnt * 2 + 1 + NWORDS_PREAMBLE];
                if ( (uint)(word1 & 0x80) != 0 )
                {
                    pos[cnt] = 0;
                    cur[cnt] = 0;
                    SimFaultMode[cnt] = 0 ; 
                    if ( word2 == 0 )
                    {
                        faultCode[cnt] = 0xffff;
                    }
                    else
                    {  
                        faultCode[cnt] = word2;
                    }
                }
                else
                {
                    faultCode[cnt] = 0;
                    SimFaultMode[cnt] = (ushort)( word1 & 0xff ) ;
                    pos[cnt] = (double)(word1>>8);
                    cur[cnt] = (double)(sword2 * 0.001);
                }
            }
        }
        public void DecodePeriodicSimState(ushort[] buf, out ulong HostTimeRead, out ulong RemainTimeRead, uint[] ValveSimStateRead )
        {
            ushort unext;
            HostTimeRead = (ulong)buf[41] + ((ulong)buf[42] << 16) + ((ulong)buf[43] << 32) + ((ulong)buf[44] << 48);
            RemainTimeRead = (ulong)buf[45] + ((ulong)buf[46] << 16) + ((ulong)buf[47] << 32) + ((ulong)buf[48] << 48);
            for (int cnt = 0; cnt < Literals.N_VALVES/2; cnt++)
            {
                unext = buf[cnt + 41+ NWORDS_PREAMBLE];
                ValveSimStateRead[cnt*2] = (uint) (unext & 0xff) ;
                ValveSimStateRead[cnt * 2+1] = (uint)((unext>>8) & 0xff);
            }

        }

        public void DecodePeriodicPressure(ushort[] buf, double[] press, uint[] sysLevelError, out uint guiIsMaster)
        {
            for (int cnt = 0; cnt < Literals.N_PRESSURE; cnt++)
            {
                press[cnt] = (double)(buf[cnt + 28 + NWORDS_PREAMBLE ]);
            }
            uint stat = buf[32 + NWORDS_PREAMBLE];
            uint a = 1;
            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                sysLevelError[cnt] = (uint)(stat & a);
                a *= 2;
            }
            guiIsMaster = (stat >> 15) &1;
        }

        //GUI_SetGUIPeriodicMessage
        public void SendKillAndRebootMsg(bool isbg = true)
        {

            // Prep the exec message

            PrepKillAndRebootMsg(KillAndReboot, out byte[] buf, out _);

            // Send it 
            if (isbg)
                mutex.WaitOne();
            // Dont Prep a trap ; The acknowledge means nothing, as following reboot we dont expect any acknowledge
            HostCom.SetMessage2TxQueue(buf);


            if (isbg)
                mutex.ReleaseMutex();
        }



        public void SendTakeOverMsg(bool isbg , Func<ushort[], int> handler)
        {
            // Prep the exec message

            PrepTakeOverMsg(TakeOver, out byte[] buf, out _);

            // Send it 
            if (isbg)
                mutex.WaitOne();
            // Dont Prep a trap ; The acknowledge means nothing, as following reboot we dont expect any acknowledge
            HostCom.SetMessage2TxQueue(buf);
            SetTrap((ushort) Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, handler, OutMessageCounter); 


            if (isbg)
                mutex.ReleaseMutex();
        }


        public void SendYieldControlMsg(bool isbg, Func<ushort[], int> handler)
        {

            // Prep the exec message

            PrepYieldControlMsg(YieldControl, out byte[] buf, out _);

            // Send it 
            if (isbg)
                mutex.WaitOne();
            // Dont Prep a trap ; The acknowledge means nothing, as following reboot we dont expect any acknowledge
            HostCom.SetMessage2TxQueue(buf);
            SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIOpCode_Acknowledge, handler, OutMessageCounter);

            if (isbg)
                mutex.ReleaseMutex();
        }

        public void SendGetSimulatorStateMsg(bool isbg, Func<ushort[], int> handler)
        {
            // Prep the exec message
            DataRequestMsg((ushort) Literals.GUIOpCodes.GUI_GetSimState , out byte[] buf ,out uint cntr); 

            // Send it 
            if (isbg)
                mutex.WaitOne();
            // Dont Prep a trap ; The acknowledge means nothing, as following reboot we dont expect any acknowledge
            HostCom.SetMessage2TxQueue(buf);
            SetTrap((ushort)Literals.Reply2GUIOpCodes.GUIAnswer_SimulatorState, handler, cntr);

            if (isbg)
                mutex.ReleaseMutex();
        }

    }


}
