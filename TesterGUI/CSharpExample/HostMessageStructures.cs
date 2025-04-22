using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;


public static partial class Literals
{

    public static int MaxListLength = 65536;



    public static int N_ID = 4;
    public static int N_PRESSURE = 4;
    public static int N_VALVES = 14;
    public static int DefaultUnsolicitedMessagePeriod = 200 ; // Default intervals of unsolicited transmissions when graphics are on 
    public static int N_MAX_SIGS = 16; // Maximum simulaneous recorded signals 
    public static int GUI_COM_READ_BUFFER_SIZE = 300000; // Physical size of buffer
    public static int BYTES_IN_READ_BUFFER = 262144; // Maximu read length before declaring overflow
    public static double CfgVoltageLevel = 3.3;
    public static double CurrentMeasResistorOhm = 3.0; 
    public static int nAdcAveraging = 2 ; // Number of averaged general analog inputs (while power is off) 
    public static int nAdcAveragingPower = 10; // Number of averaged general analog inputs (while power is on) 

    public enum System
    {
        GUIBaudRate = 230400 ,
        HostBaudRate = 230400,
        ModbusBaudRate =  38400, 
    }

    public enum ATRColumn
    {
        Header = 1 ,
        Description = 2 , 
        ExpectedResult = 3 , 
        Units = 4 ,
        LowValue = 5 ,
        HighValue = 6 ,
        ActualValue = 7 ,
        Pass = 8,
        Graph = 9 , 
        Date = 10
    }

    public enum GUIOpCodes
    {
        GUI_GetSimID = 0XB002,
        GUI_SetSimID = 0xB003,
        GUI_SetSingleModelParamsSet = 0xB004,
        GUI_GetSingleModelParamsSet = 0xB005,
        GUI_SetModel2ValveMapping = 0xB006,
        GUI_GetModel2ValveMapping = 0xB007, 
        GUI_RequestValveStatusReport = 0xB00b,
        GUI_SetPressureTestValue = 0xB00c,
        GUI_SetGUIPeriodicMessage = 0xB00d,
        GUI_TakeOverSimulatorFromHost = 0xb00e ,
        GUI_YieldSimulatorToHost = 0xb00f,
        GUIDoNothing = 0xB011,
        GUILoadDataIntoRAM = 0xb013 ,
        GUI_RequestVersion = 0xB015,
        GUI_BootPrepFwLoad = 0xB016 , 
        GUIOpCode_KillReboot = 0xB017 ,
        GUI_SetFwBurn = 0xB018 ,
        GUI_GetSimState = 0xb019,
        GUI_SetValveDiscreteTestValue = 0xB01a, 
		GUI_SetSSRAndFanTestValue = 0xB01c,
        GUI_SetValveCurrentTestValue = 0xB01d,
        GUI_GetCalibration = 0xB01e,
        GUI_SetCalibration = 0xB01f,
        GUI_GetUnitData = 0xB022, 
        GUI_SetUnitProductionData = 0xB023,
        GUI_GetAtpSignals = 0xB025,
        GUI_SetInstallation = 0xb026 , 
        GUI_SetFwBurnDsp = 0xB028,
        GUI_BroadcastSdo = 0xB029,
        GUI_BootPrepFwLoadDsp = 0xB02A,
        GUI_SetFwTestSerialFlash = 0xB02B,
        GUI_SetActiveMappingToSerialFlash = 0xB02C,
        GUI_GetMappingIndexFromFlash = 0xB02D,
        GUI_GetRecordedSignal = 0xB02E,
        GUI_SetRecorderParameters = 0xB02F,
        GUI_AxisVersionReport = 0xB030,
        GUI_SetDebugPeriodicReport = 0xB031, 
        GUI_SetAtpConversion = 0xB032
    }

    public enum Reply2GUIOpCodes
    {
        GUIAnswer_SimulatorId = 0xC002 ,
        GUIAnswer_GetSingleModelParamsSet = 0xC005,
        GUIAnswer_GetModel2ValveMapping = 0xC007,
        GUIAnswer_PeriodicStatus = 0x5001 ,
        GUIAnswer_RequestValveStatusReport = 0xC00b,
        GUIAnswer_VersionReport = 0xC015,
        GUIAnswer_SimulatorState = 0xc019 ,
        GUIAnswer_GetCalibration = 0xc01e,
        GUIOpCode_Acknowledge = 0xD005,
        GUIAnswer_GetUnitData = 0xC022 ,
        GUIOpCode_AnswerGetAtpSignals = 0xC025,
        GUIOpCode_AnswerBroadcastDload2Slave = 0xC029,
        GUIAnswer_GetMappingIndexFromFlash = 0xC02D, 
        GUIOpCode_AnswerGetRecordedSignal = 0xC02E,
        GUIOpCode_AnswerSetRecorderParameters = 0xC02F,
        GUIAnswer_AxisVersionReport = 0xC030 
    }


    public enum HostOpCodes
    {
        HostOpStatusPeriod = 0xA001,
        HostOpVersionRequest = 0xA002,
        HostOpBitRequest = 0xA003,
        HostOpGetDeviceConfig = 0xA004,
        HostOpSetDeviceConfig = 0xA005,
        HostOpExec = 0xA006,
        HostOpSetMode = 0xA007,
        HostOpSetIdOut = 0xA008
    };
    public enum Reply2HostOpCodes
    {
        PvsOpStatusData = 0xB001,
        PvsVersionData = 0xB002,
        PvsBitData = 0xB003,
        PvsExecStatus = 0xB004,
        PvsAcknowledge = 0xB005
    };
    public enum E_DeviceMode
    {
        DM_ProgrammingMode = 0,
        DM_Normal = 1,
        DM_SimulationMode = 2,
       DM_SimulationModePassCode = 1333
    }

    public enum E_ValveFunction
    {
        EV_Disabled = 0,
        EV_Normal = 1,
        EV_FullyOpen = 2,
        EV_FullyClosed = 3,
        EV_TimeDelay = 4,
        EV_SlowReaction = 5
    }

    public enum E_CanId
    {
        CAN_ID_SIM_SDO_BROADCAST = 48 ,
        CAN_ID_SIM_PDO_BROADCAST = 49 ,
        CAN_ID_LLC_SDO_BROADCAST = 78 
    }

    public enum E_ProjTypes
    {
        PROJ_TYPE_SIM = 0x4000 ,
        PROJ_TYPE_SIM_BOOT = 0x3ff0 ,
        PROJ_TYPE_LLC = 0x6000,
        PROJ_TYPE_LLC_BOOT = 0x5ff0
    }

    public enum E_ValveSystemModes
    {
        E_SysConvModeNothing = 1, // Inactive
        E_SysConvModeManual = 2, // Controlled directly over CAN by GUI
        E_SysConvModeAutomatic = 3, // Controlled by the PVS controller
        E_SysConvModeFault = 7, // At fault 
    }

    public enum E_ConverterMode
    {
        ConvMode_Nothing = 0, // Not operating
        ConvMode_FixedPWM = 1, // Directly controlled PWM, for test only
        ConvMode_ClosedLoopVoltage = 2 // Full output voltage control
    }

    public enum E_SimulatingFault 
    {
        E_Disabled = 0, // Not operational
        E_Normal = 1,   // Normal valve simulation
        E_FullyClosed = 2, // Valve stuck the closed position
        E_FullyOpen = 3,// Valve stuck the open position
        E_TimeDelay = 4, // Valve responds with time delay
        E_SlowReact = 5   // Valve has slower than normal reaction
    }
}

namespace TesterGUI
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CMsgHeader
    {
        public ulong Time;
        public uint MessageNum;
        public ushort len;
        public ushort opcode;
        // CompensateHeader = true if the length is of the payload alone , false if the length is given for the entire struct 
        public void Fill(ushort len_in_words, ushort opcode_in, uint MessageNum_in = 0, bool CompensateHeader = true )
        {
            int HeaderLen = CompensateHeader ? 10 :  0; 
            len = (ushort)((len_in_words + HeaderLen) * 2);
            opcode = opcode_in;
            DateTime currentTime = DateTime.Now;
            Time = (ulong)(currentTime.Millisecond * 1000.0 + currentTime.Second * 1000000.0
                + currentTime.Minute * 60000000.0 + currentTime.Hour * 3.6000e+09);
            MessageNum = MessageNum_in;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CExecConfig
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort ValveMode0;
        public ushort ValveMode1;
        public ushort ValveMode2;
        public ushort ValveMode3;
        public ushort ValveMode4;
        public ushort ValveMode5;
        public ushort ValveMode6;
        public ushort ValveMode7;
        public ushort ValveMode8;
        public ushort ValveMode9;
        public ushort ValveMode10;
        public ushort ValveMode11;
        public ushort ValveMode12;
        public ushort ValveMode13;
        public ulong StartExecTime;
        public ulong LifeTime;
        public ushort Spare;
        public ushort cs;
    }



    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CValveStat
    {
        public byte SimHwState;
        public byte ValveRelPos;
        public short ValveCurrent;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CPeriodicStatus
    {
        public CMsgHeader Header;
        public CValveStat ValveStat0;
        public CValveStat ValveStat1;
        public CValveStat ValveStat2;
        public CValveStat ValveStat3;
        public CValveStat ValveStat4;
        public CValveStat ValveStat5;
        public CValveStat ValveStat6;
        public CValveStat ValveStat7;
        public CValveStat ValveStat8;
        public CValveStat ValveStat9;
        public CValveStat ValveStat10;
        public CValveStat ValveStat11;
        public CValveStat ValveStat12;
        public CValveStat ValveStat13;
        public fixed ushort Pressure[4];
        public ushort SysLevelError;
        public ulong SysTime;
        public ulong LifeTime;
        public byte SimModeValve1;
        public byte SimModeValve2;
        public byte SimModeValve3;
        public byte SimModeValve4;
        public byte SimModeValve5;
        public byte SimModeValve6;
        public byte SimModeValve7;
        public byte SimModeValve8;
        public byte SimModeValve9;
        public byte SimModeValve10;
        public byte SimModeValve11;
        public byte SimModeValve12;
        public byte SimModeValve13;
        public byte SimModeValve14;
        public ushort cs;
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CHWversion
    {
        public byte HwSubVersion;
        public byte HwVersion;
        public ushort SerialNumber;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSWversion
    {
        public byte patch;
        public byte subver;
        public ushort ver;
        public byte day;
        public byte month;
        public ushort year;
        public string PrintSwVersion()
        {
            string[] mon = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string strmon = "Bad";
            string BootString;
            if (this.month >= 1 && this.month <= 12)
            {
                strmon = mon[this.month - 1];
            }
            ushort _year = year ; 
            if (_year > 2200)
            {
                _year -= 128;
                BootString = " : Boot";
            }
            else
            {
                BootString = "";
            }

            string ostr = String.Format("{0}:{1}:{2},{3}:{4}:{5}", this.ver, this.subver, this.patch, _year, strmon, this.day) + BootString;
            return ostr;
        }
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetModelSelectMsg
    {
        public ushort Preamble;
        public CMsgHeader Header;
        public ushort modelset;
        public ushort cs;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CAskStatusMsg
    {
        public ushort Preamble;
        public CMsgHeader Header;
        public ushort milisec;
        public ushort cs;
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SingleDeviceConfig
    {
        public byte HardTemperature;
        public byte FailStatus;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CBitInfo
    {
        public CMsgHeader Header;
        public ushort GlobalErrorCode;
        public ushort PowerUnitTemperature;
        public SingleDeviceConfig DevConfig0;
        public SingleDeviceConfig DevConfig1;
        public SingleDeviceConfig DevConfig2;
        public SingleDeviceConfig DevConfig3;
        public SingleDeviceConfig DevConfig4;
        public SingleDeviceConfig DevConfig5;
        public SingleDeviceConfig DevConfig6;
        public SingleDeviceConfig DevConfig7;
        public SingleDeviceConfig DevConfig8;
        public SingleDeviceConfig DevConfig9;
        public SingleDeviceConfig DevConfig10;
        public SingleDeviceConfig DevConfig11;
        public SingleDeviceConfig DevConfig12;
        public SingleDeviceConfig DevConfig13;
        public byte IdOut;
        public byte CfgIn; 
        public ushort cs;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SingleDeviceExecStatus
    {
        public byte ActiveStatus;
        public byte ModelIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CDeviceExecStatus
    {
        public CMsgHeader Header;
        public ushort SetNumber;
        SingleDeviceExecStatus DevStatus1;
        SingleDeviceExecStatus DevStatus2;
        SingleDeviceExecStatus DevStatus3;
        SingleDeviceExecStatus DevStatus4;
        SingleDeviceExecStatus DevStatus5;
        SingleDeviceExecStatus DevStatus6;
        SingleDeviceExecStatus DevStatus7;
        SingleDeviceExecStatus DevStatus8;
        SingleDeviceExecStatus DevStatus9;
        SingleDeviceExecStatus DevStatus10;
        SingleDeviceExecStatus DevStatus11;
        SingleDeviceExecStatus DevStatus12;
        SingleDeviceExecStatus DevStatus13;
        SingleDeviceExecStatus DevStatus14;
        public ulong StartTime;
        public ulong LifeTime;
        public ushort cs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CAckInfo
    {
        public CMsgHeader Header;
        public ushort ExpCode;
        public ushort cs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CTakeOver
    {
        public ushort Preamble;
        public CMsgHeader Header;
        public ushort Code1;
        public ushort Code2;
        public ushort cs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CYieldControl
    {
        public ushort Preamble;
        public CMsgHeader Header;
        public ushort Code1;
        public ushort Code2;
        public ushort cs;
    }

}