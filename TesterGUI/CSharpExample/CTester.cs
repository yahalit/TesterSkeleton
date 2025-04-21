using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;

namespace PvsGUI
{

    public class CPowerRelay : CRelay32
    {
        // Inherited methods: ReadRelayState , WriteRelayState
        static byte ModbusAddress = 5;
        static bool _LittleEndian = false; 
        public CPowerRelay() : base(ModbusAddress,_LittleEndian)
        { // Inherited constructor

        }

        public void SelectPositiveSourceValve(int index, bool[] RelayIndex)
        {
            //RelayIndex = new bool[32];
            for (int cnt = 0; cnt < 16; cnt++) { RelayIndex[cnt] = false; }
            switch (index)
            {
                default: // None
                case 0: //SOL1
                    RelayIndex[Index.K14] = true;// check 
                    break;
                case 1: // SOL2
                    RelayIndex[Index.K14] = true;// check 
                    RelayIndex[Index.K1] = true;
                    break;
                case 2: // SOL3
                    RelayIndex[Index.K14] = true; // check 
                    RelayIndex[Index.K8] = true;
                    break;
                case 3: // SOL4
                    RelayIndex[Index.K14] = true; // check 
                    RelayIndex[Index.K8] = true;
                    RelayIndex[Index.K2] = true;
                    break;
                case 4: // SOL5
                    RelayIndex[Index.K14] = true; // check 
                    RelayIndex[Index.K12] = true;
                    break;
                case 5: // SOL6 
                    RelayIndex[Index.K14] = true; // check 
                    RelayIndex[Index.K12] = true;
                    RelayIndex[Index.K3] = true;
                    break;
                case 6: // SOL7
                    RelayIndex[Index.K14] = true; // check 
                    RelayIndex[Index.K12] = true;
                    RelayIndex[Index.K9] = true;
                    break;
                case 7: // SOL8 
                    RelayIndex[Index.K14] = true; // --- kaka
                    RelayIndex[Index.K12] = true;
                    RelayIndex[Index.K9] = true;
                    RelayIndex[Index.K4] = true;
                    break;
                case 8: // SOL9 
                    RelayIndex[Index.K13] = true; // check 
                    break;
                case 9: // SOL10 
                    RelayIndex[Index.K13] = true; // check 
                    RelayIndex[Index.K5] = true;
                    break;
                case 10: // SOL11
                    RelayIndex[Index.K13] = true;// check 
                    RelayIndex[Index.K10] = true;
                    break;
                case 11: // SOL12
                    RelayIndex[Index.K13] = true; // check 
                    RelayIndex[Index.K10] = true;
                    RelayIndex[Index.K6] = true;
                    break;
                case 12: // SOL13 
                    RelayIndex[Index.K11] = true; // check 
                    break;
                case 13: // SOL14 
                    RelayIndex[Index.K11] = true; // check 
                    RelayIndex[Index.K7] = true;
                    break;
            }

        }

        public void SelectNegativeSourceValve(int index, bool[] RelayIndex)
        {
            //RelayIndex = new bool[32];
            for (int cnt = 16; cnt < 30; cnt++) { RelayIndex[cnt] = false; }
            switch (index)
            {
                default: // None
                case 0: //SOL1
                    RelayIndex[Index.K30] = true; // check 
                    break;
                case 1: // SOL2
                    RelayIndex[Index.K30] = true;// check
                    RelayIndex[Index.K17] = true;
                    break;
                case 2: // SOL3
                    RelayIndex[Index.K30] = true; // check 
                    RelayIndex[Index.K24] = true;
                    break;
                case 3: // SOL4
                    RelayIndex[Index.K30] = true;//Checked
                    RelayIndex[Index.K24] = true;
                    RelayIndex[Index.K18] = true;
                    break;
                case 4: // SOL5
                    RelayIndex[Index.K30] = true; //Checked
                    RelayIndex[Index.K28] = true;
                    break;
                case 5: // SOL6 
                    RelayIndex[Index.K30] = true; // checked
                    RelayIndex[Index.K28] = true;
                    RelayIndex[Index.K19] = true;
                    break;
                case 6: // SOL7
                    RelayIndex[Index.K30] = true; // checked
                    RelayIndex[Index.K28] = true;
                    RelayIndex[Index.K25] = true;
                    break;
                case 7: // SOL8 
                    RelayIndex[Index.K30] = true;// check
                    RelayIndex[Index.K28] = true;
                    RelayIndex[Index.K25] = true;
                    RelayIndex[Index.K20] = true;
                    break;
                case 8: // SOL9 
                    RelayIndex[Index.K29] = true; // check
                    break;
                case 9: // SOL10 
                    RelayIndex[Index.K29] = true; // check 
                    RelayIndex[Index.K21] = true;
                    break;
                case 10: // SOL11
                    RelayIndex[Index.K29] = true; // check 
                    RelayIndex[Index.K26] = true;
                    break;
                case 11: // SOL12
                    RelayIndex[Index.K29] = true;  // check 
                    RelayIndex[Index.K26] = true;
                    RelayIndex[Index.K22] = true;
                    break;
                case 12: // SOL13 
                    RelayIndex[Index.K27] = true; // check
                    break;
                case 13: // SOL14 
                    RelayIndex[Index.K27] = true; // check
                    RelayIndex[Index.K23] = true;
                    break;
            }

        }

        public bool SelectSourceValve(int index)
        {
            // There are 32 values for the 32 switches, each index = relay number - 1 
            // An index is true if the relay goes to its NO (normally Open) position 
            for (int cnt = 0; cnt < RelayNextState.Length; cnt++) RelayNextState[cnt] = false;
            SelectPositiveSourceValve(index, RelayNextState);
            SelectNegativeSourceValve(index, RelayNextState);

            return true;
        }


        public void SetMeasurementConfig(bool ConnectGnd2Ret, bool ConnectCurrentMeas, bool ConnectCurrentLoad, bool ConnectVoltageMeas)
        {
            RelayNextState[Index.K16] = ConnectGnd2Ret;
            RelayNextState[Index.K31] = ConnectCurrentMeas;
            RelayNextState[Index.K15] = ConnectCurrentLoad;
            RelayNextState[Index.K32] = ConnectVoltageMeas;
        }
    }

    public enum PanelTestRedBananaSelect
    {
        AN_TESTPlus = 0,
        PressJ1Plus = 1,
        PressJ1Minus = 2
    }

    public enum PanelTestBlackBananaSelect
    {
        BlackOpen = -1,
        CFG_GND = 0,
        GND_PS = 1,
        AN_TESTMinus = 2
    }

    public class CSignalRelay : CRelay32
    {
        // Inherited methods: ReadRelayState , WriteRelayState
        static byte ModbusAddress = 5;
        static bool _LittleEndian = true;


        public CSignalRelay() : base(ModbusAddress,_LittleEndian)
        {

        }

        public bool SetAnalogPressIn(int ind)
        {
            bool[] inputs = new bool [] { false, false, false, false };
            if ( ind >= 0 && ind < 4)
            {
                inputs[ind] = true;
            }
            RelayNextState[Index.K24] = inputs[0];
            RelayNextState[Index.K25] = inputs[1];
            RelayNextState[Index.K26] = inputs[2];
            RelayNextState[Index.K27] = inputs[3];
            return WriteRelayState(); 
        }

        public bool SetPressOutMeasureJ7(int PressSelect)
        {
            bool RetVal = true;
            RelayNextState[Index.K29] = false;
            RelayNextState[Index.K30] = false;
            RelayNextState[Index.K31] = false;
            RelayNextState[Index.K32] = false;

            // Route pressures to ANTEST 
            RelayNextState[Index.K11] = false;
            RelayNextState[Index.K13] = false;
            RelayNextState[Index.K14] = false;

            switch (PressSelect)
            {
                default:
                    RetVal = false;
                    break;
                case -1: // Nothing
                    break;
                case 0: // P1
                    RelayNextState[Index.K29] = true;
                    break;
                case 1: // P2
                    RelayNextState[Index.K30] = true;
                    break;
                case 2: // P3
                    RelayNextState[Index.K31] = true;
                    break;
                case 3: // P4
                    RelayNextState[Index.K32] = true;
                    break;
            }

            if (!RetVal) 
                return false; 
            return WriteRelayState();
        }

        public void SetPanelTestContacts(PanelTestRedBananaSelect red, PanelTestBlackBananaSelect black)
        {
            RelayNextState[Index.K24] = false;
            RelayNextState[Index.K25] = false;
            RelayNextState[Index.K26] = false;
            RelayNextState[Index.K27] = false;
            RelayNextState[Index.K28] = false;

            switch (red)
            {
                case PanelTestRedBananaSelect.AN_TESTPlus:
                    break;
                case PanelTestRedBananaSelect.PressJ1Plus:
                    RelayNextState[Index.K28] = true;
                    break;
                case PanelTestRedBananaSelect.PressJ1Minus:
                    RelayNextState[Index.K27] = true;
                    RelayNextState[Index.K28] = true;
                    break;

            }
            switch (black)
            {
                case PanelTestBlackBananaSelect.BlackOpen:
                    break;
                case PanelTestBlackBananaSelect.CFG_GND:
                    RelayNextState[Index.K25] = true;
                    break;
                case PanelTestBlackBananaSelect.GND_PS:
                    RelayNextState[Index.K26] = true;
                    break;
                case PanelTestBlackBananaSelect.AN_TESTMinus:
                    RelayNextState[Index.K24] = true;
                    RelayNextState[Index.K26] = true;
                    break;

            }

        }

        public bool SetValveAnalogOut(int OutSelect)
        {
            bool RetVal = true;

            RelayNextState[Index.K1] = false;
            RelayNextState[Index.K2] = false;
            RelayNextState[Index.K3] = false;
            RelayNextState[Index.K4] = false;
            RelayNextState[Index.K5] = false;
            RelayNextState[Index.K6] = false;

            RelayNextState[Index.K7] = false;
            RelayNextState[Index.K8] = false;
            RelayNextState[Index.K9] = false;
            RelayNextState[Index.K10] = false;
            RelayNextState[Index.K11] = false;
            RelayNextState[Index.K12] = false;

            RelayNextState[Index.K13] = false;
            RelayNextState[Index.K14] = false;

            switch (OutSelect)
            {
                default:
                    RetVal = false;
                    break;
                case -1: // None
                    break;
                case 0: // O1
                    RelayNextState[Index.K14] = true;
                    break;
                case 1: // O2 
                    RelayNextState[Index.K1] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 2: // O3
                    RelayNextState[Index.K8] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 3: // O4
                    RelayNextState[Index.K2] = true;
                    RelayNextState[Index.K8] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 4: // O5
                    RelayNextState[Index.K12] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 5: // O6 
                    RelayNextState[Index.K3] = true;
                    RelayNextState[Index.K12] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 6: // O7 
                    RelayNextState[Index.K9] = true;
                    RelayNextState[Index.K12] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 7: // O8 
                    RelayNextState[Index.K4] = true;
                    RelayNextState[Index.K9] = true;
                    RelayNextState[Index.K12] = true;
                    RelayNextState[Index.K14] = true;
                    break;
                case 8: // O9
                    RelayNextState[Index.K13] = true;
                    break;
                case 9: // O10 
                    RelayNextState[Index.K5] = true;
                    RelayNextState[Index.K13] = true;
                    break;
                case 10: // O11
                    RelayNextState[Index.K10] = true;
                    RelayNextState[Index.K13] = true;
                    break;
                case 11: // O12
                    RelayNextState[Index.K6] = true;
                    RelayNextState[Index.K10] = true;
                    RelayNextState[Index.K13] = true;
                    break;
                case 12: // O13
                    RelayNextState[Index.K11] = true;
                    break;
                case 13: // O14
                    RelayNextState[Index.K7] = true;
                    RelayNextState[Index.K11] = true;
                    break;
            }

            if (!RetVal)
                return false;
            return WriteRelayState();
        }

        public bool SetCfgSwitch(bool ID1, bool ID2, bool Parity)
        {
            RelayNextState[Index.K21] = ID1;
            RelayNextState[Index.K22] = ID2;
            RelayNextState[Index.K23] = Parity;
            return WriteRelayState(); 
        }

        public bool SetPressureAndRetJ1(int PressSelect)
        {
            bool RetVal = true;
            RelayNextState[Index.K15] = false;
            RelayNextState[Index.K16] = false;
            RelayNextState[Index.K17] = false;
            RelayNextState[Index.K18] = false;
            RelayNextState[Index.K19] = false;
            RelayNextState[Index.K20] = false;
            switch (PressSelect)
            {
                default:
                    RetVal = false;
                    break;
                case 0: // P1
                    break;
                case 1: // P2
                    RelayNextState[Index.K15] = true;
                    RelayNextState[Index.K17] = true;
                    break;
                case 2: // P3
                    RelayNextState[Index.K20] = true;
                    RelayNextState[Index.K19] = true;
                    break;
                case 3: // P4
                    RelayNextState[Index.K20] = true;
                    RelayNextState[Index.K19] = true;
                    RelayNextState[Index.K16] = true;
                    RelayNextState[Index.K18] = true;
                    break;
            }
            if ( RetVal)
            {
                WriteRelayState(); 
            }
            return RetVal;
        }
    }

    class CAdcSimIdAndPressure : CAdc8
    {
        // Inherited functions: ReadAdcData , SetDataType 

        static byte ModbusAddress = 1;
        public bool[] SimIdValue = new bool[4];
        public bool[] SimIdOk = new bool[4];
        public double PressJ1P;
        public double PressJ1N;
        public double J1PGain = 0;
        public double J1POffset = 0;
        public double J1NGain = 0;
        public double J1NOffset = 0;

        public int    SerialNumber;
        public double PressJ1Value;

        public CAdcSimIdAndPressure() : base(ModbusAddress)
        {

        }

        public bool CollectData()
        {
            Thread.Sleep(500); // Wait data stabilization 

            if (!ReadAdcData())
            {
                return false;
            }

            for (byte cnt = 0; cnt < 4; cnt++)
            {
                SimIdOk[cnt] = GetDigital(cnt, out SimIdValue[cnt]);
            }

            PressJ1P = ((Volts[Index.A5] + Volts[Index.A6]) * 0.5 - J1POffset) * (1 + J1PGain);
            PressJ1N = (Volts[Index.A7] - J1NOffset) * (1 + J1NGain);
            PressJ1Value = PressJ1P - PressJ1N;

            //double Vr = Volts[Index.A8] / 24;

            SerialNumber = (int)(Volts[Index.A8] + 0.5 ) + 1 ;  //4.7 * (1 - Vr) / (22.0 * Vr);

            return true;
        }

    }


    class CAdcDigitalSolAndInTest : CAdc8
    {
        // Inherited functions: ReadAdcData , SetDataType 

        static byte ModbusAddress = 2;
        public bool[] DsolValue = new bool[6];
        public bool[] DsolOk = new bool[6];
        public double AnTestValue;
        public double AnTestGain = 0;
        public double AnTestOffset = 0;

        public CAdcDigitalSolAndInTest() : base(ModbusAddress)
        {

        }

        public bool CollectData(double SleepSec = 0.5)
        {

            Thread.Sleep((int)(SleepSec*1000.0)); // Wait data stabilization 
            if (!ReadAdcData())
            {
                return false;
            }

            for (byte cnt = 0; cnt < 6; cnt++)
            {
                DsolOk[cnt] = GetDigital(cnt, out DsolValue[cnt]);
            }
            AnTestValue = ((Volts[Index.A7] + Volts[Index.A8]) * 0.5 - AnTestOffset) * (1 + AnTestGain);

            return true;
        }
    }


    class CAdcDigitalSolInd : CAdc8
    {
        // Inherited functions: ReadAdcData , SetDataType 

        static byte ModbusAddress = 3;
        public bool[] DsolValue = new bool[8];
        public bool[] DsolOk = new bool[8];

        public CAdcDigitalSolInd() : base(ModbusAddress)
        {

        }

        public bool CollectData(double SleepSec = 0.5)
        {

            Thread.Sleep((int)(SleepSec * 1000)); // Wait data stabilization 
            if (!ReadAdcData())
            {
                return false;
            }

            for (byte cnt = 0; cnt < 8; cnt++)
            {
                DsolOk[cnt] = GetDigital(cnt, out DsolValue[cnt]);
            }
            return true;
        }
    }


    class CAdcMeasureValves : CAdc8
    {
        // Inherited functions: ReadAdcData , SetDataType 

        static byte ModbusAddress = 4;
        public double CurrentMeas;
        public double VoltageMeas;

        public double CurrentGain = 0;
        public double CurrentOffset = 0;
        public double VoltageGain = 0;
        public double VoltageOffset = 0;

        public CAdcMeasureValves() : base(ModbusAddress)
        {

        }

        public bool CollectData()
        {

            Thread.Sleep(500); // Wait data stabilization 
            if (!ReadAdcData())
            {
                return false;
            }

            CurrentMeas = ((Volts[Index.A5] + Volts[Index.A6]) * 0.5 - CurrentOffset) * (1 + CurrentGain);
            VoltageMeas = ((Volts[Index.A7] + Volts[Index.A8]) * 0.5 - VoltageOffset) * (1 + VoltageGain);

            return true;
        }
    }


    class CTester
    {
        static int SerialNumber = 0;
        public CPowerRelay PowerRelay = new CPowerRelay();
        public CSignalRelay SignalRelay = new CSignalRelay();
        //CInputRelay InputRelay = new CInputRelay();
        public CAdcSimIdAndPressure AdcSimIdAndPressure = new CAdcSimIdAndPressure();
        public CAdcDigitalSolAndInTest AdcDigitalSolAndInTest = new CAdcDigitalSolAndInTest();
        public CAdcDigitalSolInd AdcDigitalSolInd = new CAdcDigitalSolInd();
        public CAdcMeasureValves AdcMeasureValves = new CAdcMeasureValves();
        private static readonly CTester _instance = new CTester();


        private CTester()
        {
            // Console.WriteLine("Singleton instance created");
        }
        public static CTester Instance
        {
            get { return _instance; }
        }

        public bool InitTester(string CalibfilePath, out string msg )
        {
            //double CurrentGain, CurrentOffset, VoltageGain, VoltageOffset, AnTestGain, AnTestOffset, J1PGain, J1POffset, J1NGain, J1NOffset;
            int _SerialNumber = 0 ;
            msg = "Ok";

            // Read the excel calibration 
            if ( !AdcSimIdAndPressure.LoadCalibrationFile(CalibfilePath, 1))
            {
                msg = "ADC1 calibration absent in calibration file "; 
                return false;
            }
            if ( !AdcDigitalSolAndInTest.LoadCalibrationFile(CalibfilePath, 2)) 
            {
                msg = "ADC2 calibration absent in calibration file ";
                return false; 
            }
            if ( !AdcDigitalSolInd.LoadCalibrationFile(CalibfilePath, 3)) 
            {
                msg = "ADC3 calibration absent in calibration file ";
                return false; 
            }
            if ( !AdcMeasureValves.LoadCalibrationFile(CalibfilePath, 4)) 
            {
                msg = "ADC4 calibration absent in calibration file ";
                return false; 
            }
            DateTime _ExpirationDate;
            try
            {
                using (var workbook = new XLWorkbook(CalibfilePath))
                {
                    var worksheet = workbook.Worksheet("Tester"); // Read the first worksheet
                    _SerialNumber = worksheet.Cell("B1").GetValue<int>();
                    _ExpirationDate = worksheet.Cell("B2").GetDateTime();
                }
                /*
                using (var workbook = new XLWorkbook(CalibfilePath))
                {
                    var worksheet = workbook.Worksheet("CalibData"); // Read the first worksheet
                    CurrentGain = worksheet.Cell("B8").GetValue<double>();
                    CurrentOffset = worksheet.Cell("C8").GetValue<double>();
                    VoltageGain = worksheet.Cell("B9").GetValue<double>();
                    VoltageOffset = worksheet.Cell("C9").GetValue<double>(); ;
                    AnTestGain = worksheet.Cell("B7").GetValue<double>();
                    AnTestOffset = worksheet.Cell("C7").GetValue<double>();
                    J1PGain = worksheet.Cell("B5").GetValue<double>();
                    J1POffset = worksheet.Cell("C5").GetValue<double>();
                    J1NGain = worksheet.Cell("B6").GetValue<double>();
                    J1NOffset = worksheet.Cell("C6").GetValue<double>();

                    _SerialNumber = worksheet.Cell("B3").GetValue<int>();
                }
                */
            }
            catch
            {
                msg = "Tester S/N or expiry date absent in calibration file ";
                return false;
            }
            DateTime currentTime = DateTime.Now;
            if (_ExpirationDate < currentTime)
            {
                msg = $"Tester calibration had expired at {_ExpirationDate}";
                return false;
            }


            // Collect SW version from each each of the members, just to see its threre
            if ( !PowerRelay.ReadDeviceSwVersion() )
            {
                msg = "Cant communicate with power relays ";
                return false; 
            }
            if (!SignalRelay.ReadDeviceSwVersion())
            {
                msg = "Cant communicate with signals relays ";
                return false;
            }
            if (!AdcSimIdAndPressure.ReadDeviceSwVersion())
            {
                msg = $"Cant communicate with SIM ID and Pressure ADC {AdcSimIdAndPressure.address}";
                return false;
            }
            if (!AdcDigitalSolAndInTest.ReadDeviceSwVersion())
            {
                msg = $"Cant communicate with digital solenoid state reports ADC  {AdcDigitalSolAndInTest.address}";
                return false;
            }
            if (!AdcDigitalSolInd.ReadDeviceSwVersion())
            {
                msg = $"Cant communicate with analog solenoid indications ADC  {AdcDigitalSolInd.address}";
                return false;
            }
            if (!AdcMeasureValves.ReadDeviceSwVersion())
            {
                msg = $"Cant communicate with power readouts ADC  {AdcMeasureValves.address}";
                return false;
            }


            // Read the ADC to verify serial number 
            AdcSimIdAndPressure.CollectData();
            if (AdcSimIdAndPressure.SerialNumber == _SerialNumber)
            {
                SerialNumber = _SerialNumber;
                return true;
            }
            msg = $"Expected Simulator S/N {_SerialNumber}, found S/N {AdcSimIdAndPressure.SerialNumber}";
            return false;
        }

        public void GetValveCurrentAndVoltage(int ValveIndex, bool bOpenLoad, out double voltage, out double current)
        {
            // Set connections before having a specific relay 
            // At the time of connection no input muts be selected
            PowerRelay.SelectSourceValve(-1);
            PowerRelay.SetMeasurementConfig(ConnectGnd2Ret: true, ConnectCurrentMeas: !bOpenLoad, ConnectCurrentLoad: !bOpenLoad, ConnectVoltageMeas: true);
            PowerRelay.WriteRelayState();

            //Select the specific valve 
            PowerRelay.SelectSourceValve(ValveIndex);
            PowerRelay.WriteRelayState();

            //Take ADC measurement 
            AdcMeasureValves.CollectData();

            voltage = AdcMeasureValves.VoltageMeas;
            current = AdcMeasureValves.CurrentMeas;
        }

        public bool GetPressureIndication(int Index, out double VoltsJ1, out double VoltsJ1Ret, out double VoltsJ7)
        {
            // Route the correct pressure into the ADC
            bool RetVal;
            RetVal = SignalRelay.SetPressureAndRetJ1(Index);
            RetVal &= SignalRelay.SetPressOutMeasureJ7(Index);
            RetVal &= SignalRelay.WriteRelayState();
            AdcDigitalSolAndInTest.CollectData();
            AdcSimIdAndPressure.CollectData();
            VoltsJ1 = AdcSimIdAndPressure.PressJ1P;
            VoltsJ1Ret = AdcSimIdAndPressure.PressJ1N;
            VoltsJ7 = AdcDigitalSolAndInTest.AnTestValue;

            return RetVal;
        }

        public bool GetDigitalOuts(out bool[] SimIdValue, out bool[] SimIdOk , out bool[] ValveInd, out bool[] ValveIndOk)
        {
            bool RetVal = AdcDigitalSolAndInTest.CollectData();
            RetVal &= AdcSimIdAndPressure.CollectData();
            RetVal &= AdcDigitalSolInd.CollectData();
            SimIdValue = new bool[4];
            SimIdOk = new bool[4];
            ValveInd = new bool[14];
            ValveIndOk = new bool[14];

            for ( int cnt = 0; cnt < 4; cnt++ )
            {
                SimIdValue[cnt] = AdcSimIdAndPressure.SimIdValue[cnt];
                SimIdOk[cnt] = AdcSimIdAndPressure.SimIdOk[cnt];

            }
            for (int cnt = 0; cnt < 8; cnt++)
            {
                ValveInd[cnt] = AdcDigitalSolInd.DsolValue[cnt];
                ValveIndOk[cnt] = AdcDigitalSolInd.DsolOk[cnt];
            }
            for (int cnt = 0; cnt < 4; cnt++)
            {
                ValveInd[cnt+8] = AdcDigitalSolAndInTest.DsolValue[cnt];
                ValveIndOk[cnt+8] = AdcDigitalSolAndInTest.DsolOk[cnt];
            }
            //AdcDigitalSolAndInTest.DsolOk[6]
            return RetVal;
        }


        public bool SetCfgInputs(bool CfgId1 , bool CfgId2 , bool CfgParity)
        {
            SignalRelay.SetCfgSwitch(CfgId1, CfgId2, CfgParity);
            return SignalRelay.WriteRelayState(); 
        }

        public bool SelectDigitalIn(int ind)
        {
            bool RetVal = SignalRelay.SetAnalogPressIn(ind);
            RetVal &= SignalRelay.WriteRelayState();
            return RetVal; 
        }


    }
}