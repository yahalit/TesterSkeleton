using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using MathNet.Numerics;

namespace PvsGUI
{
    public partial class CPressureInCalibTest : Form
    {
        public CInterpreter Interpreter = CInterpreter.Instance; //  new CInterpreter(mutex);
        private ManualResetEvent NextButtonClickedEvent = new ManualResetEvent(false);
        private ManualResetEvent AbortButtonClickedEvent = new ManualResetEvent(false);
        bool JustANext = false; 
        double ExpectedVolts = 0; 
        int nChannel = 4 ; 
        CTester Tester = CTester.Instance;
        public bool TestFail = false;
        double[] Gain;
        double[] Offset ;
        double[] MaxAbsErr ;
        double[] RmsErr   ;
        bool AdcTestBodyDone;
        bool automatic;
        bool AskApprove;
        double[] TestVoltages ;
        string[] Instruction;
        int CalibType;
        int nAdcSamps = 10;
        string JsonFile;


        /* 
         * CalibType: 0 for ADC input, pressure calibration  
         *            1 for input voltages (about SSR) 
         */
        public CPressureInCalibTest(bool _automatic = false , int _CalibType = 0   )
        {
            InitializeComponent();

            CalibType = _CalibType;
            switch (CalibType)
            {
                default:
                    Instruction = new string[] { "Connect the calibration connector to an external tuneable PS", "Connected via Blue (+) and Black (-) Bananas", "Press next" };
                    TestVoltages = new double[] { 1, 2, 4, 8, 10 };
                    nChannel = Literals.N_PRESSURE;
                    nAdcSamps = 10;
                    JsonFile = "AutoPressureTest.json";
                    break; 
                case 1:
                    Instruction = new string[] { "Connect the EUT supply to tuneable PS", "Press next" };
                    TestVoltages = new double[] {20 , 30 };
                    nChannel = 2;
                    nAdcSamps = 4 ;
                    JsonFile = "AutoTPTest.json";
                    break; 
            }

            // Delete possible old copy of results to avoid confusion 
            if (File.Exists(JsonFile))
            {
                File.Delete(JsonFile);
            } 

            MaxAbsErr = new double[nChannel];
            RmsErr = new double[nChannel];

            dataGridViewAdcResults.Columns.Add("Volts", "Volts");
            dataGridViewAdcResults.RowHeadersVisible = false;
            dataGridViewAdcResults.Columns[0].Width = dataGridViewAdcResults.Width ; // Half of DataGridView width
            // Populate 8 rows of data
            for (int i = 1; i <= nChannel; i++)
            {
                dataGridViewAdcResults.Rows.Add(0,0); // Example data
            }
            automatic = _automatic;
        }
        
        //CPressureRslt data = new CPressureRslt();

        protected override void OnShown(EventArgs e)
        {
            //bool stat = false;       

            base.OnShown(e);
            Refresh(); 
            if (automatic )
            {
                TestAdcInputs(out _, out _  ,out _, out _);
            }
        }


        public bool TestAdcInputs( out double[] _Gain , out double[] _Offset , out double[] _MaxAbsErr , out double [] _RmsErr)
        {
            _Gain = new double[nChannel];
            _Offset = new double[nChannel];
            _MaxAbsErr = new double[nChannel];
            _RmsErr = new double[nChannel];
            AdcTestBodyDone = false;
            TestAdcInputsBody(); 

            return !TestFail;
        }

        async void TestAdcInputsBody()
        {
            ATPTest_textBoxMessage2Humanity.Lines = Instruction ;
            PrepNext();
            await(Task.Run(BlockTillNext));
            if (!CleanNext())
            {
                AdcTestBodyDone =true; 
                return;
            } 
            Gain = new double[nChannel];
            Offset = new double[nChannel];
            //double[] MaxAbsErr = new double[nChannel];
            //double[] RmsErr = new double[nChannel];

            //double[] TestVoltages = new double[] { 1, 8 };//{ 1, 2, 4, 8, 10 };
            double[] ActVoltages = new double[TestVoltages.Length];
            double[][] MeasVoltages = new double[nChannel][];
            double[,] rslt = new double[nChannel, nAdcSamps];
            for (int cnt = 0; cnt < nChannel; cnt++) MeasVoltages[cnt] = new double[TestVoltages.Length];


            TestFail = false;
            // Loop over all the voltages
            for (int cnt = 0; cnt < TestVoltages.Length; cnt++)
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { $"Set about {TestVoltages[cnt]}V", "to the variable PS", "Write the measured voltage in the result box", "Press next" };
                PrepNext();
                ExpectedVolts = TestVoltages[cnt];
                await(Task.Run(BlockTillNextWithValue));
                if (!CleanNext())
                {
                    AdcTestBodyDone = true;
                    return;
                }
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { "Sampling data", "Wait..." };
                Refresh();
                ActVoltages[cnt] = (double)numericUpDownResult.Value;

                for (int nInput = 0; nInput < nChannel; nInput++)
                {
                    switch (CalibType)
                    {
                        default:
                            Tester.SignalRelay.SetAnalogPressIn(nInput);
                            for (int c1 = 0; c1 < nAdcSamps; c1++)
                            {
                                if (!Interpreter.GetAtpSignals())
                                {
                                    TestFail = true;
                                    AdcTestBodyDone = true;
                                    return;
                                }
                                Interpreter.AtpSignals.GetAdcVoltage(out double[] volts);
                                rslt[nInput, c1] = volts[nInput];
                            }
                            break;
                        case 1:

                            for (int c1 = 0; c1 < nAdcSamps; c1++)
                            {
                                if (!Interpreter.GetAtpSignals())
                                {
                                    TestFail = true;
                                    AdcTestBodyDone = true;
                                    return;
                                }
                                Interpreter.AtpSignals.GetTpVolts(out double[] volts);
                                rslt[nInput, c1] = volts[nInput];
                            }

                            break;

                    }
                }


                // Get averaged measurement voltages
                for (int c2 = 0; c2 < nChannel; c2++)
                {
                    for (int c1 = 0; c1 < nAdcSamps; c1++)
                        MeasVoltages[c2][cnt] += rslt[c2, c1];
                    MeasVoltages[c2][cnt] /= nAdcSamps;
                    dataGridViewAdcResults.Rows[c2].Cells[0].Value = (int)(1000 * MeasVoltages[c2][cnt]) * 0.001 ;
                    //dataGridViewAdcResults.Rows[c2].Cells[1].Value = MeasVoltages[c2][cnt];
                }
            } // End voltages loop

            CleanNext(new string[] { "Displaying result" });

            /*
            double[] coefficients  ;
            double[] Fitted = new double[nAdcSamps]; ;
            double[] FitError = new double[nAdcSamps];
            for (int cnt = 0; cnt < nChannel; cnt++)
            {
                coefficients = Fit.Polynomial(MeasVoltages[cnt], ActVoltages, 1);
                Gain[cnt] = coefficients[1];
                Offset[cnt] = coefficients[0];
                for ( int c1 = 0; c1 < nAdcSamps; c1++)
                {
                    Fitted[c1] = coefficients[0] + coefficients[1] * MeasVoltages[cnt][c1];
                    FitError[c1] = ActVoltages[c1] - Fitted[c1];
                    MaxAbsErr[cnt] = Math.Max(MaxAbsErr[cnt], Math.Abs(FitError[c1]));
                    RmsErr[cnt] += FitError[c1] * FitError[c1];
                }
                RmsErr[cnt] = Math.Sqrt(RmsErr[cnt] / nAdcSamps);
            }
            */

            CRegression Regression;
            string filePath;
            string CalibDate;
            bool[] Approve = new bool[nChannel];
            CalibDate = "Undefined";
            for (int cnt = 0; cnt < nChannel; cnt++)
            {
                string grlabel = " ADC Channel : " + (cnt + 1).ToString();
                Regression = new CRegression(MeasVoltages[cnt], ActVoltages, cnt + 1, grlabel);
                Regression.ShowDialog();
                filePath = "RegressionOut_" + (cnt + 1).ToString() + ".txt";
                string[] lines = File.ReadAllLines(filePath);
                CalibDate = lines[1];
                Gain[cnt] = Convert.ToDouble(lines[2]);
                Offset[cnt] = Convert.ToDouble(lines[3]);
                MaxAbsErr[cnt] = Convert.ToDouble(lines[4]);
                RmsErr[cnt] = Convert.ToDouble(lines[5]);
                Approve[cnt] = Convert.ToBoolean(lines[6].ToLower());
                if (!Approve[cnt])
                {
                    TestFail = true;
                    CleanNext(new string[] { "At least one calibration was rejected by user", "Goodbye" });
                }
            }

            CPressureRslt data = new CPressureRslt();

            data.RGain = (double[])Gain.Clone();
            data.ROffset = (double[])Offset.Clone();
            data.RMaxAbsErr = (double[])MaxAbsErr.Clone();
            data.RRmsErr = (double[])RmsErr.Clone();
            data.RActVoltages = (double[])ActVoltages.Clone();
            data.Rstatus = !TestFail;
            for (int cnt = 0; cnt < nChannel; cnt++)
            {
                data.RMeasVoltages[cnt] = (double[])MeasVoltages[cnt].Clone();
            }
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
            File.WriteAllText(JsonFile, jsonString);
            CleanNext(new string[] { "Calibration resuls written into calibration file", "Goodbye" });
            if (automatic)
            {
                this.Close();
            }
 
            AdcTestBodyDone = true;
        }

        void PrepNext()
        {
            buttonCalibNext.Visible = true;
            buttonCalibAbort.Visible = true;
            numericUpDownResult.Visible = true;
            labelResults.Visible = true;
            NextButtonClickedEvent.Reset();   // Clear the event for the next use
            AbortButtonClickedEvent.Reset();   // Clear the event for the next use
        }

        bool CleanNext(string[] msg = null)
        {
            if (msg == null)
            {
                msg = new string[] { "User aborted process", "Retrun on Black Friday for discount" };
            }
            else
            {
                JustANext = false;
            }
            buttonCalibNext.Visible = false;
            buttonCalibAbort.Visible = false;
            numericUpDownResult.Visible = false;
            labelResults.Visible = false;
            if (!JustANext)
            {
                ATPTest_textBoxMessage2Humanity.Lines = msg;
                return false;
            }
            JustANext = false;
            return true;
        }

        private void BlockTillNextWithValue()
        {
            int index;
            while (true)
            {
                index = WaitHandle.WaitAny(new WaitHandle[] { NextButtonClickedEvent, AbortButtonClickedEvent });
                if (index != 0)
                {
                    break; // That's an abort 
                }
                if (Math.Abs((double)numericUpDownResult.Value - ExpectedVolts) < 0.5)
                {
                    break; // That's good 
                }
                NextButtonClickedEvent.Reset();   // Clear the event for the next use
                MessageBox.Show("Entered result must be within 0.5V of target \nPlease try again, then press next Press next");
            }
            JustANext = (index == 0);
        }

        private void BlockTillNext()
        {
            int index = WaitHandle.WaitAny(new WaitHandle[] { NextButtonClickedEvent, AbortButtonClickedEvent });
            JustANext = (index == 0);
        }

        private void ButtonCalibNext_Click(object sender, EventArgs e)
        {
            _ = NextButtonClickedEvent.Set();
        }

        private void Click_AbortCalib(object sender, EventArgs e)
        {
            AbortButtonClickedEvent.Set();
        }
    }

    [Serializable]
    public class CPressureRslt
    {
        public double[] RGain { get; set; }
        public double[] ROffset { get; set; }
        public double[] RMaxAbsErr { get; set; }
        public double[] RRmsErr { get; set; }

        public double[] RActVoltages { get; set; }
        public double[][] RMeasVoltages { get; set; }
        public bool Rstatus { get; set; }

        public CPressureRslt()
        {
            int nChannel = Literals.N_PRESSURE; 
            RGain = new double[0];
            ROffset = new double[0];
            RRmsErr = new double[0];
            RMaxAbsErr = new double[0];
            RMeasVoltages = new double[nChannel][]; 
            for ( int cnt = 0; cnt < nChannel; cnt++ )
            {
                RMeasVoltages[cnt] = new double[0];
            }
            Rstatus = false;
        } 
    }

}
