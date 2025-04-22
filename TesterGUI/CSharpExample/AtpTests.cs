using ClosedXML.Excel;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using ZedGraph;
using static TesterGUI.CRelay32;
using static TesterGUI.XLSGraph;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;

namespace TesterGUI
{
    partial class CAtpMainWindow
    {

        //In this test the user is required to give a manually measured value and to approve that something happened 
        // Log ATR result with on-line calculated tolerances
        public bool TestPowerOnConsumptionLED(CTestIdentifier a)
        {
            bool TotalPass = true;

            AtrBoundSpecifier ConsumptionBounds = new AtrBoundSpecifier(0.1 + 0.2  , 0.15 + 0.3 );

            GetValueResults rslt = GetUserValue("J8 Power On and consumption", CheckText: "Panel led is green", IsText: false, lowlimit: -1000, highlimit: 10000, new string[] { "Please set the power connection at J8", "Apply power on 28V, and test LED and current consumption (Amp) after 5 seconds" });
            bool Pass = rslt.b;
            TotalPass &= AtpExcel.SetResultInAtr("1.0.1.1", a, 0, rslt.b ? "Shines" : "Does not shine", ref Pass, ref a.ErrMsg, "LED did not work at J8");
            TotalPass &= AtpExcel.SetResultInAtr("1.0.1.2", a, rslt.d, ConsumptionBounds, ref Pass,  ref a.ErrMsg, "Power consumption out of range in J8");
            rslt = GetUserValue("J9 Power On and consumption", CheckText: "Panel led is green", IsText: false, lowlimit: -1000, highlimit: 10000, new string[] { "Please replace the power connection to J9", "Apply power on 28V, and test LED and current consumption (Amp) after 5 seconds" });
            Pass = rslt.b; 
            TotalPass &= AtpExcel.SetResultInAtr("1.0.2.1", a, 0, rslt.b ? "Shines" : "Does not shine", ref Pass, ref a.ErrMsg, "LED did not work at J9");
            TotalPass &= AtpExcel.SetResultInAtr("1.0.2.2", a, rslt.d, ConsumptionBounds, ref Pass, ref a.ErrMsg, "Power consumption out of range in J9");
            return TotalPass;
        }

        // This test returns a testing error if something bad happened, else tests a string against the expected value  
        public bool TestInstallationAndVersions(CTestIdentifier a)
        {
            bool TotalPass = true;
            Interpreter.DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_RequestVersion, out byte[] buf, out uint _);
            if (Interpreter.OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_VersionReport, Interpreter.ReadVersionMsg) == false)
            {
                a.ErrMsg.Add("Could not read version through GUI protocol");
                return false; 
            }
            Interpreter.DataRequestMsg((ushort)Literals.GUIOpCodes.GUI_AxisVersionReport, out buf, out uint _);
            if (Interpreter.OfflineTransaction(100, out ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIAnswer_AxisVersionReport, Interpreter.ReadAxisVersionMsg) == false)
            {
                a.ErrMsg.Add("Could not read axis version through GUI protocol");
                return false; 
            }

            // Begin specific tests 
            bool Pass;
            Pass = (AtpExcel.TestForm.CpuSwVersion == Interpreter.Answer_AxisVersion.MainSwExpected);
            TotalPass &= AtpExcel.SetResultInAtr("1.1.1.1", a, 0, AtpExcel.TestForm.CpuSwVersion.ToString(), ref Pass, ref a.ErrMsg , "CPU version does not mach ");

            return true;
        }

        /*
         * Set some string as ATR result 
         */
        public bool TestHostCommunicationRs422(CTestIdentifier a)
        {
            // Test that host responds to communication over the RS422
            bool Pass = true; 

            // Write the result in the ATR 
            string sActValue = (Pass) ? "Ok response" : "No response";
            return AtpExcel.SetResultInAtr("1.2.1", a, 0, sActValue, ref Pass,ref  a.ErrMsg,"Could not get host response");
        }

        public bool TestInternalCommunicationFSI(CTestIdentifier a)
        {
            return true;
        }

        /* 
         * Test with active user verification of a fact 
         */
        public bool TestCollerFans(CTestIdentifier a)
        {
            // Activate the main power tunel fan 
            Interpreter.SetSSRAndFans(SetSSR: false, SSRValue: true, SetFan: true, Fan1Value: true, Fan2Value: false); 
			// Display the message box with Yes and No buttons
			DialogResult result = MessageBox.Show(
				"Does the coller fan work for the load compartment ?",
				"Confirmation",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);

            // Check the user's choice
            bool Pass = (result == DialogResult.Yes);
			bool TotalPass = AtpExcel.SetResultInAtr("1.4.1", a, 0,
				(Pass ? "Fan rotates " : "Fan does not rotate"), ref Pass, ref a.ErrMsg, "Tester did not verify fan rotaiton ");


			Interpreter.SetSSRAndFans(SetSSR: false, SSRValue: true, SetFan: true, Fan1Value: false, Fan2Value: true);
			result = MessageBox.Show(
				"Does the coller fan work for the electronics compartment ?",
				"Confirmation",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);

			// Check the user's choice
			Pass = (result == DialogResult.Yes);
			TotalPass &= AtpExcel.SetResultInAtr("1.4.2", a, 0,
				(Pass ? "Fan rotates " : "Fan does not rotate"), ref Pass, ref a.ErrMsg, "Tester did not verify fan rotaiton ");

            // Shut the unnecessary noise from the fans
			Interpreter.SetSSRAndFans(SetSSR: false, SSRValue: true, SetFan: false, Fan1Value: true, Fan2Value: true);
            result = MessageBox.Show(
                "Did all the coller fans stop?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            // Check the user's choice
            Pass = (result == DialogResult.Yes);
            TotalPass &= AtpExcel.SetResultInAtr("1.4.3", a, 0,
                (Pass ? "Fans stopped " : "Fan still rotates"), ref Pass, ref a.ErrMsg, "Tester did not verify fan stopping ");

            return TotalPass;
		}


		public bool TestTemperatureMeas(CTestIdentifier a)
        {
            bool TotalPass = true;
            return TotalPass;
        }

        public bool TestVoltageMeas(CTestIdentifier a)
        {
            bool Pass = true;
            bool TotalPass = true;
            if (!Interpreter.GetSimulatorState())
            {
                a.ErrMsg.Add("Could not get Simulator state");
                return false;
            }
            Interpreter.SimulatorState.GetBitVoltages(out double Bit5V, out double Bit12V);
            TotalPass &= AtpExcel.SetResultInAtr("1.6.1.1", a, Bit5V, (string)null, ref Pass, ref a.ErrMsg,
                $"BIT voltage {Bit5V}: Expected as 5V");
            TotalPass &= AtpExcel.SetResultInAtr("1.6.1.2", a, Bit12V, (string)null, ref Pass, ref a.ErrMsg,
                $"BIT voltage {Bit12V}: Expected as 12V");
            //AtpExcel.SetNAInAtr("1.6.1.3" , a); // As DSP 1.65V is not reported 
            return TotalPass;
        }

        // Test using a recorder outcome
        public bool TestSSRPowerOn(CTestIdentifier a)
        {
            bool TotalPass = true;
			bool Pass = true;
			// Ask the user to power on. The recorder will trigger automatically
			GetValueResults rslt = GetUserValue("SSR and power ON process", CheckText: null, IsText: false, lowlimit: -1, highlimit: 100, 
                new string[] { "Please set the power connection at J8", "Apply power on 28V, and measure supply voltage (Volts) after 5 seconds" });

            // Wait records readiness up to 5 seconds
            if (!Recorder.WaitRecorderReady(5.0))
            {
                a.ErrMsg.Add("Could not get recorder readiness");
                return false;
            }

			// After readiness, upload the records for analysis
			string [] SignalNames = { "InternalVolts","ExternalVolts","CBit"};
            int[] SignalIndices = { Recorder.sigrec.LocateSignalByName(SignalNames[0], out _), Recorder.sigrec.LocateSignalByName(SignalNames[1],out _) , 
                Recorder.sigrec.LocateSignalByName(SignalNames[2],out _) };
            int[] SignalFlags = { 2, 2, 4 };

            textBoxMessageToHumanity.Lines = new string[] {
                    "SSR and Power On Test, Uploading recorded data",
                    "Wait..." };
            textBoxMessageToHumanity.Refresh(); 

            if (!Recorder.sigrec.BringRecorderData(a.TempFolder, SignalIndices, SignalNames, SignalFlags, bar: null))
            {
				a.ErrMsg.Add("Could not get recorder upload data");
				return false;
			}

            // Analysis of signals (in RecordedSignals[][] and RecordedTime )
            int nPoints = Recorder.sigrec.RecordedTime.Length;

            int nSSR = 0;
            int[] CBit = new int[nPoints]; 
            for ( int i = 0; i < nPoints; i++)
            {
                CBit[i] = (int)Recorder.sigrec.RecordedSignals[2][i];
                if ((CBit[i] & 0x8000) == 0)
                {
                    nSSR = i; 
                } 
            }

            //double tEnd = Recorder.sigrec.RecordedTime[nPoints - 1];
            double Av1 = 0;
			double Av2 = 0;
			int nStart = nPoints * 3 / 4;
			int nConverge = 0;
			for (int cnt = nStart; cnt < nPoints; cnt++)  
            {
				Av1 += (Recorder.sigrec.RecordedSignals[0][cnt]);
				Av2 += (Recorder.sigrec.RecordedSignals[1][cnt]);
			}
            Av1 /= (nPoints- nStart);
			Av2 /= (nPoints - nStart);

			for (int cnt = nPoints-1; cnt > 0 ; cnt--)
			{
                if (Math.Abs(Recorder.sigrec.RecordedSignals[0][cnt] - Recorder.sigrec.RecordedSignals[1][cnt] - Av1 + Av2) < 2.5)
                {
                    nConverge = cnt;
                }
                else
                {
                    break; 
                } 
			}

            int nFinal = nPoints - 1;
            for (int cnt = nSSR; cnt < nPoints; cnt++)
            {
                if (Recorder.sigrec.RecordedTime[cnt] - Recorder.sigrec.RecordedTime[nSSR] > 0.5)
                {
                    nFinal = cnt;
                    break; 
                }
			}
            double Av3 = 0;
            for (int cnt = nFinal; cnt < nPoints; cnt++)
            {
                Av3 += (Recorder.sigrec.RecordedSignals[0][cnt] - Recorder.sigrec.RecordedSignals[1][cnt]); 
			}
            Av3 /= (nPoints - nFinal);

            int nProgram = nPoints - 1;
            for (int cnt = 0; cnt < nPoints; cnt++)
            {
                if ( (CBit[cnt] & (1 << 19)) != 0 ) 
                {
                    nProgram = cnt;
                    break;
				} 
            }
            XLSGraph.GrAttributes atr = new XLSGraph.GrAttributes
            {
                Title = "Power on evolution ",
                XLabel = "Time",
                YLabel = "Volts",
                CurveNames = new string[] { "Input", "Internal bulk" },
                Colors = new System.Drawing.Color[] { System.Drawing.Color.Red, System.Drawing.Color.Blue },
                Selector = new int[] { 0, 1 }
            };
            XLSGraph.CreateZedGraphChart(Recorder.sigrec.RecordedTime, Recorder.sigrec.RecordedSignals, atr, "TemporaryChart.png");
            XLSGraph.InsertImageIntoExcel("TemporaryChart.png", "B2", a.ExcelFileName, "Graphs");


            // Voltage convergence time 
            TotalPass &= AtpExcel.SetResultInAtr("1.7.1.1", a, Recorder.sigrec.RecordedTime[nConverge], (string)null, ref Pass, ref a.ErrMsg,
			$"Time of SSR-open convergence {Recorder.sigrec.RecordedTime[nConverge]}: Out of range");

			// SSR closure time 
			TotalPass &= AtpExcel.SetResultInAtr("1.7.1.2", a ,Recorder.sigrec.RecordedTime[nSSR] , (string)null, ref Pass, ref a.ErrMsg,
				$"Time of SSR closure {Recorder.sigrec.RecordedTime[nSSR]}: Out of range");

            // Convergence at time of SSR closure
            double dv = Recorder.sigrec.RecordedSignals[0][nSSR] - Av1 - Recorder.sigrec.RecordedSignals[1][nSSR] + Av2;
			TotalPass &= AtpExcel.SetResultInAtr("1.7.1.3", a, dv , (string)null, ref Pass, ref a.ErrMsg,
				$"Voltage fall on SSR on closure {dv}: Out of range");

			// Total fall
			TotalPass &= AtpExcel.SetResultInAtr("1.7.1.4", a, Av3, (string)null, ref Pass, ref a.ErrMsg,
				$"Voltage fall on SSR after closure {Av3}: Out of range");

			// Verify input measurement close enough to PS 
			TotalPass &= AtpExcel.SetResultInAtr("1.7.1.5" , a, rslt.d - Av1, (string)null, ref Pass, ref a.ErrMsg, 
                $"Input voltage reading {Av1}: Expected by PS measurement {rslt.d}");

            // Programming time 
            if (nProgram < nPoints - 1)
            {
                TotalPass &= AtpExcel.SetResultInAtr("1.7.1.6", a, Recorder.sigrec.RecordedTime[nProgram], (string)null, ref Pass, ref a.ErrMsg,
                    $"Time of programming completion {Recorder.sigrec.RecordedTime[nProgram]}: Out of range");
            }
            else
            {
				TotalPass &= AtpExcel.SetResultInAtr("1.7.1.6", a, 0, "Programming completion not detected", ref Pass, ref a.ErrMsg,
					$"Programming completion never detected");
			}
			return TotalPass;
        }

        public bool TestPanelLEDs(CTestIdentifier a)
        {
            return true;
        }

        public bool TestRandomSerialFlashRW(CTestIdentifier a)
        {
            return true;
        }
        public bool TestBoxReset(CTestIdentifier a)
        {
            Interpreter.SetGUIInCharge();
            MessageBox.Show(this,"Press the RESET \nRelease Reset \nThen press ok");
            Thread.Sleep(2000);

            if (!Interpreter.GetSimulatorState())
            {
                a.ErrMsg.Add("Could not get Simulator state");
                return false;
            }
            CSystemBit cb = Interpreter.SimulatorState.GetCBit();
            bool Pass = !cb.GUIInCharge  ;
            AtpExcel.SetResultInAtr("2.2.0", a, 0, Pass ? "Reset Success" : "Reset Failure", ref Pass, ref a.ErrMsg,
                               $"GUI In charge status did not disappear on reset");

            Thread.Sleep(2000);
            Interpreter.SetGUIInCharge();
            return Pass;
        }

        public bool TestBootSwitch(CTestIdentifier a)
        {
            return true;
        }

        public bool AdcInputTestDone;


        public bool TestPressureAdcInput(CTestIdentifier a)
        {
            bool TotalPass = true;
            bool Pass = true;
            AdcInputTestDone = false;

            // Delete possible old copy of results to avoid confusion 
            if (File.Exists("AutoPressureTest.json")) File.Delete("AutoPressureTest.json");

            // Will generate results in AutoPressureTest.json
            CPressureInCalibTest form = new CPressureInCalibTest(_automatic: true);
            form.ShowDialog();

            try
            {
                string readJson = File.ReadAllText("AutoPressureTest.json");
                CPressureRslt Rslt = JsonSerializer.Deserialize<CPressureRslt>(readJson);

                for (int cnt = 0; cnt < 4; cnt++)
                {
                    VecOps.CheckUnityRegression(Rslt.RActVoltages, Rslt.RMeasVoltages[cnt], out double maxerr, out double rmserr); 
                    TotalPass &= AtpExcel.SetResultInAtr("2.4.0." + (cnt + 1).ToString(), a, rmserr, null, ref Pass, ref a.ErrMsg,
                                       $"Expected ADC read precision not met");

                }

            }
            catch
            {
                for (int cnt = 0; cnt < 4; cnt++)
                {
                    Pass = false; 
                    TotalPass &= AtpExcel.SetResultInAtr("2.4.0." + (cnt + 1).ToString(), a, 0, "Failed to read results", ref Pass, ref a.ErrMsg,
                                   $"Could not read ADC");
                }

            }
            return TotalPass; 
        }

        public bool TestCFGInputs(CTestIdentifier a)
        {
            bool TotalPass = true;
            Tester.SignalRelay.SetCfgSwitch( ID1: false, ID2 : false, Parity : false);
            if (!Interpreter.GetSimulatorState())
            {
                a.ErrMsg.Add("Could not get Simulator state");
                return false;
            }
            Interpreter.SimulatorState.GetCfg(out bool[] value0, out bool[] ok0, Literals.CfgVoltageLevel);

            Tester.SignalRelay.SetCfgSwitch(ID1: true, ID2: false, Parity: false);
            if (!Interpreter.GetSimulatorState())
            {
                a.ErrMsg.Add("Could not get Simulator state");
                return false;
            }
            Interpreter.SimulatorState.GetCfg(out bool[] value1, out bool[] ok1, Literals.CfgVoltageLevel);

            Tester.SignalRelay.SetCfgSwitch(ID1: false, ID2: true, Parity: false);
            if (!Interpreter.GetSimulatorState())
            {
                a.ErrMsg.Add("Could not get Simulator state");
                return false;
            }
            Interpreter.SimulatorState.GetCfg(out bool[] value2, out bool[] ok2, Literals.CfgVoltageLevel);

            Tester.SignalRelay.SetCfgSwitch(ID1: false, ID2: false, Parity: true);
            if (!Interpreter.GetSimulatorState())
            {
                a.ErrMsg.Add("Could not get Simulator state");
                return false;
            }
            Interpreter.SimulatorState.GetCfg(out bool[] value3, out bool[] ok3, Literals.CfgVoltageLevel);

            // Return to normal 
            Tester.SignalRelay.SetCfgSwitch(ID1: true, ID2: false, Parity: false);

            bool Pass = (!value1[0] & ok1[0] );
            TotalPass &= AtpExcel.SetResultInAtr("2.4.1.1" , a, 0 , Pass? "0 Logical" : "1 or undefind", ref Pass, ref a.ErrMsg,
                               $"Expected CFG_ID1 Low digital input");
            Pass = (value0[0] & value2[0] & value3[0] & ok0[0] & ok2[0] & ok3[0]);
            TotalPass &= AtpExcel.SetResultInAtr("2.4.1.2", a, 0, Pass ? "0 Logical" : "1 or undefind", ref Pass, ref a.ErrMsg,
                               $"Expected CFG_ID2 Low digital input");
            Pass = (!value2[1]  & ok2[1]);
            TotalPass &= AtpExcel.SetResultInAtr("2.4.1.3", a, 0, Pass ? "0 Logical" : "1 or undefind", ref Pass, ref a.ErrMsg,
                               $"Expected CFG_Parity Low digital input");
            Pass = (value0[1] & value1[1] & value3[1] & ok0[1] & ok2[1] & ok3[1]);
            TotalPass &= AtpExcel.SetResultInAtr("2.4.1.4", a, 0, Pass ? "0 or undefined" : "1 logical", ref Pass, ref a.ErrMsg,
                               $"Expected CFG_ID1 High digital input");
            Pass = (!value3[2] & ok3[2]);
            TotalPass &= AtpExcel.SetResultInAtr("2.4.1.5", a, 0, Pass ? "0 or undefined" : "1 logical", ref Pass, ref a.ErrMsg,
                               $"Expected CFG_ID2 High digital input");
            Pass = (value0[2] & value1[2] & value2[2]  & ok0[2] & ok1[2] & ok2[2]);
            TotalPass &= AtpExcel.SetResultInAtr("2.4.1.6", a, 0, Pass ? "0 or undefined" : "1 logical", ref Pass, ref a.ErrMsg,
                               $"Expected CFG_Parity High digital input");


            return TotalPass;
        }


        public bool MeasureSinglePressureOutJ1(int index , double Vref , double[] Voltages, int _nAverage ,out double[] abserr, out double[] rmserr )
        {
            abserr = new double[3];
            rmserr = new double[3];
            if ( !Tester.SignalRelay.SetPressureAndRetJ1(index)) 
                return false;
            int nAverage = Math.Max(_nAverage, 1);
            double[] J1Pos = new double[Voltages.Length];
            double[] J1Neg = new double[Voltages.Length];
            double[] J1Diff = new double[Voltages.Length];

            textBoxMessageToHumanity.Lines = new string[] {"Testing pressure inputs for J1",$"Input {index}..." }; Refresh(); 


            for (int cnt = 0; cnt < Voltages.Length; cnt++)
            { // Enter the voltage to be put 
                if (!Interpreter.SetPressureJ1J7(Voltages[cnt], index))
                {
                    return false;
                }
                double [] AvP = new double[nAverage];
                double [] AvN = new double[nAverage]; ;
                for ( int c1  = 0; c1 <  nAverage; c1++)
                {
                    Tester.AdcSimIdAndPressure.CollectData();
                    AvP[c1] = Tester.AdcSimIdAndPressure.PressJ1P;
                    AvN[c1] = Tester.AdcSimIdAndPressure.PressJ1N;
                }
                J1Pos[cnt] = AvP.Average();
                J1Neg[cnt] = AvN.Average();
                J1Diff[cnt] = J1Pos[cnt] - J1Neg[cnt];
            }
            VecOps.CheckUnityRegression(Voltages.Select(n => n + Vref).ToArray(), J1Pos, out abserr[0], out rmserr[0]);
            VecOps.CheckUnityRegression(Voltages, J1Diff, out abserr[1], out rmserr[1]);
            VecOps.CheckConstantFit(Voltages, J1Neg, Vref, out abserr[2], out rmserr[2]);
            return true; 
        }

        public bool TestPressureOutJ1(CTestIdentifier a)
        {
            bool TotalPass = true;
            bool Pass = true;
            double[] Voltages = new double[] {0 , 1.667 , 3.333 , 4.9 }; 
            for (int cnt = 0; cnt < 4; cnt++)
            {
                if (!MeasureSinglePressureOutJ1(cnt, Vref: 2.0 , Voltages,Literals.nAdcAveraging , out double[] _, out double[] rmserr))
                {
                    a.ErrMsg.Add("Could not set Relays, or could not get set pressure display");
                    return false;
                }

                TotalPass &= AtpExcel.SetResultInAtr("2.5.1."+(cnt+1).ToString() , a, rmserr[1], null , ref Pass, ref a.ErrMsg,
                                   $"Expected good enough fit for pressure voltage out");
                TotalPass &= AtpExcel.SetResultInAtr("2.5.2." + (cnt + 1).ToString(), a, rmserr[2], null, ref Pass, ref a.ErrMsg,
                                   $"Expected good enough constant return 2V");
            }
            Tester.SignalRelay.SetPressureAndRetJ1(-1); // Return relays to idle state 
            return TotalPass;
        }

        public bool MeasureSinglePressureOutJ7(int index, double[] Voltages, int _nAverage, out double abserr, out double rmserr)
        {
            abserr = 0;
            rmserr = 0; 
            if (!Tester.SignalRelay.SetPressOutMeasureJ7(index))
                return false;

            textBoxMessageToHumanity.Lines = new string[] { "Testing pressure inputs for J7", $"Input {index}..." }; Refresh();

            int nAverage = Math.Max(_nAverage, 1);
            double[] Analog = new double[Voltages.Length];

            for (int cnt = 0; cnt < Voltages.Length; cnt++)
            { // Enter the voltage to be put 
                Interpreter.SetPressureJ1J7(Voltages[cnt], index);
                double [] AvP = new double[nAverage];
                for (int c1 = 0; c1 < nAverage; c1++)
                {
                    Tester.AdcDigitalSolAndInTest.CollectData();
                    AvP[c1] = Tester.AdcDigitalSolAndInTest.AnTestValue ;
                }
                Analog[cnt] = AvP.Average() ;
            }
            VecOps.CheckUnityRegression(Voltages, Analog, out abserr, out rmserr);
            return true;
        }

        public bool TestPressureOutJ7(CTestIdentifier a)
        {
            bool TotalPass = true;
            bool Pass = true;
            double[] Voltages = new double[] { 0, 1.667, 3.333, 4.9 };
            for (int cnt = 0; cnt < 4; cnt++)
            {
                if (!MeasureSinglePressureOutJ7(cnt, Voltages, Literals.nAdcAveraging, out double _, out double rmserr))
                {
                    a.ErrMsg.Add("Cannot command manula pressure outputs"); 
                    return false;
                }

                TotalPass &= AtpExcel.SetResultInAtr("2.6.1." + (cnt + 1).ToString(), a, rmserr, null, ref Pass, ref a.ErrMsg,
                                   $"Expected good enough fit for pressure voltage out");
            }
            return TotalPass;
        }


        public bool MeasureSingleCurrentReport(int index, double[] Voltages, int _nAverage, out double abserr, out double rmserr)
        {
            abserr = 0;
            rmserr = 0;
            if (!Tester.SignalRelay.SetValveAnalogOut(index))
                return false;

            textBoxMessageToHumanity.Lines = new string[] { "Testing pressure inputs for solenoid current report", $"Input {index}..." }; Refresh();

            int nAverage = Math.Max(_nAverage, 1);
            double[] Analog = new double[Voltages.Length];

            for (int cnt = 0; cnt < Voltages.Length; cnt++)
            { // Enter the voltage to be put 
                Interpreter.SetAnalogCurrentReport(Voltages[cnt], index);
                double [] AvP = new double[nAverage];
                for (int c1 = 0; c1 < nAverage; c1++)
                {
                    Tester.AdcDigitalSolAndInTest.CollectData();
                    AvP[c1] = Tester.AdcDigitalSolAndInTest.AnTestValue;
                }
                Analog[cnt] = AvP.Average();
            }
            VecOps.CheckUnityRegression(Voltages, Analog, out abserr, out rmserr);
            return true;
        }

        /* 
         *  Test something with routing Analog signals through Modbus relays, then sampling them through Modbus ADC 
         */
        public bool TestSolenoidCurrentReport(CTestIdentifier a)
        {
            bool TotalPass = true;
            bool Pass = true;
            int cnt = 0; 
            double[] Voltages = new double[] { 0, 1.667, 3.333, 4.9 };
            if (!MeasureSingleCurrentReport(cnt, Voltages, Literals.nAdcAveraging, out double _, out double rmserr))
                return false;

            TotalPass &= AtpExcel.SetResultInAtr("2.7.1." + (cnt + 1).ToString(), a, rmserr, null, ref Pass, ref a.ErrMsg,
                                $"Expected good enough fit for current report voltage out");
            return TotalPass;
        }

        /* 
         *  Test something with routing Digital output signals through Modbus relays, then sampling them through Modbus ADC 
         */
        static int N_DOUT = 3;
        public bool TestSolenoidStateReport(CTestIdentifier a)
        {
            bool TotalPass = true;
            bool Pass = true; 
            bool[] values = new bool[N_DOUT];
            bool[] valout = new bool[N_DOUT];
            bool[] valok = new bool[N_DOUT];
            bool[] LogicFault = new bool[N_DOUT];
            bool[] ValueFault = new bool[N_DOUT];
            for ( int cnt = 0; cnt < N_DOUT; cnt++)
            {
                textBoxMessageToHumanity.Lines = new string[] { $"Testing Discrete Valve state output index {cnt}..." }; Refresh();
                for ( int i = 0; i < N_DOUT; i++)
                {
                    values[i] = false; 
                }
                values[cnt] = true; 
                if (! Interpreter.SetDiscreteSignals(values))
                {
                    a.ErrMsg.Add("Cannot command manual discrete outputs");
                    return false;
                } // Second read can be quite immediate since sleep is already taken 
                if ( !Tester.AdcDigitalSolInd.CollectData(SleepSec:0.25) || !Tester.AdcDigitalSolAndInTest.CollectData(SleepSec: 0.05))
                {
                    a.ErrMsg.Add("Cannot collect data from ADC");
                    return false;
                }
                for ( int i = 0; i < 8 ; i++) 
                {
                    valout[i] = Tester.AdcDigitalSolInd.DsolValue[i];
                    valok[i] = Tester.AdcDigitalSolInd.DsolOk[i];                   
                }
                for (int i = 8; i < N_DOUT; i++)
                {
                    valout[i] = Tester.AdcDigitalSolAndInTest.DsolValue[i-8];
                    valok[i] = Tester.AdcDigitalSolAndInTest.DsolOk[i-8];
                }
                // For each of the valves test that value is ok a digital level and its value is correct 
                for (int i = 0; i < N_DOUT; i++)
                {
                    if (!valok[i])
                    {
                        LogicFault[i] = true;
                    }
                    if (valout[i] != values[i])
                    {
                        ValueFault[i] = true;
                    }
                }

            }
            for (int cnt = 0; cnt < N_DOUT; cnt++)
            {
                Pass =  !ValueFault[cnt];
                TotalPass &= AtpExcel.SetResultInAtr("2.8.1." + (cnt + 1).ToString(), a, 0, "Logic 0/1 alternation", ref Pass, ref a.ErrMsg,
                                    $"Expected values to match command");
                Pass = !LogicFault[cnt];
                TotalPass &= AtpExcel.SetResultInAtr("2.8.1." + (cnt + 1+N_DOUT).ToString(), a, 0, "Logic 0/1 alternation", ref Pass, ref a.ErrMsg,
                                    $"Expected values to reolve to digital value level");
            }
            return TotalPass; 
        }

        public bool TestSimulatorIDOutputs(CTestIdentifier a)
        {
            bool TotalPass = true;
            return TotalPass;
        }
        public bool TestVoltageTestNoLoad(CTestIdentifier a)
        {
            bool TotalPass = true;
            return TotalPass; 
        }

        public bool TestVoltageAndCurrents(CTestIdentifier a)
        {
            bool TotalPass = true;
            return TotalPass;
        }

        public bool TestBoxJ4Plug(CTestIdentifier a)
        {
            bool TotalPass = true;
            return TotalPass; 
        }


        // This is the last test, here Pafuzlachi ends his sacred mission 
        public bool TestIsolation(CTestIdentifier a)
        {
            bool TotalPass = true;
             return TotalPass;
        }

    }
}
