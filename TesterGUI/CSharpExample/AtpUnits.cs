using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Xml;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using DocumentFormat.OpenXml.Math;
using ZedGraph;
using System.IO;
using ClosedXML.Excel;
using static PvsGUI.Gadgets;
using System.Net;
using System.Runtime.CompilerServices;

namespace PvsGUI
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Make field read only", Justification = "Fields are used as non-readonly elsewhere")]

    public partial class CAtpUnits : Form
    {
        bool IsComOpen = false ;
        public CModbusInterpreter MBInterpreter = CModbusInterpreter.Instance; //  new CInterpreter(mutex);
        CRelay32 Relay = new CRelay32(1,true);
        CAdc8 Adc = new CAdc8(1);
        private ManualResetEvent NextButtonClickedEvent = new ManualResetEvent(false) ;
        private ManualResetEvent AbortButtonClickedEvent = new ManualResetEvent(false);
        bool JustANext;
        readonly string DefaultCalibPath;
        string CalibPath;
        double ExpectedVolts;
        bool UpdateSwBoxes = true; 

        public CAtpUnits(string _DefaultCalibPath)
        {
            InitializeComponent();
            //this.FormClosing += MyForm_FormClosing; // Attach the FormClosing event
            buttonCalibNext.Visible = false;
            buttonCalibAbort.Visible = false;
            numericUpDownResult.Visible = false;
            labelResults.Visible = false;
            DefaultCalibPath = _DefaultCalibPath;
            CalibPath = DefaultCalibPath;
            labelCalibDirectoryPath.Text = CalibPath;
            label1CalibFileName.Text = "TesterCalib_" + numericUpDownTesterSerial.Value.ToString() + @".xlsx"; 
        }

        // Handle FormClosing event
        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent the form from being disposed
            e.Cancel = true;
            this.Hide(); // Just hide the form
        }

        private void AtpUnits_Load(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsAtp);
            comboBoxBaudRate.Items.Clear();
            comboBoxBaudRate.Items.AddRange( new string [] { "9600","19200","38400","57600" });
            comboBoxBaudRate.DrawMode = DrawMode.Normal;
            comboBoxBaudRate.SelectedIndex = 2; // default 38400 

            comboBoxNewBaudRate.Items.AddRange(new string[] { "9600", "19200", "38400", "57600" });
            comboBoxNewBaudRate.DrawMode = DrawMode.Normal;
            comboBoxNewBaudRate.SelectedIndex = 2; // default 38400 


            dataGridViewAdcResults.Columns.Add("Counts", "Counts");
            dataGridViewAdcResults.Columns.Add("Volts", "Volts");
            dataGridViewAdcResults.RowHeadersVisible = false;
            dataGridViewAdcResults.Columns[0].Width = dataGridViewAdcResults.Width / 2; // Half of DataGridView width
            dataGridViewAdcResults.Columns[1].Width = dataGridViewAdcResults.Width / 2; // Half of DataGridView width
            // Populate 8 rows of data
            for (int i = 1; i <= 8; i++)
            {
                dataGridViewAdcResults.Rows.Add(0, 0); // Example data
            }
            //comboBoxPortsAtp.SelectedItem = comboBoxPortsAtp.Items[comboBoxPortsAtp.SelectedIndex]; 
            //comboBoxPortsAtp.Update(); 
            //comboBoxPortsAtp.Refresh();

        }

        private void DropDown_ComboAtpPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsAtp);
        }

        private void Enter_ComboAtpPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsAtp);
        }

        private void ButtonConnectATPBody()
        {
            string msg;
            string com = (string)comboBoxPortsAtp.Items[comboBoxPortsAtp.SelectedIndex];

            int colonIndex = com.IndexOf(':');
            if (colonIndex != -1)
                com = com.Substring(0, colonIndex);

            if (IsComOpen == false)
            {
                int baud = Convert.ToInt32(comboBoxBaudRate.SelectedItem); 
                IsComOpen = MBInterpreter.ModbusCom.OpenSerialPort(com, baud , out _);
                if (IsComOpen)
                {
                    Gadgets.SetLedColor(pictureLEDConnectATP, "Green");
                    buttonConnectATP.Text = "Disconnect";
                    msg = string.Format("Port opened successfully");
                    //MBInterpreter.GetVersion(isbg: true);
                }
                else
                {
                    msg = string.Format("Could not open port");
                }
            }
            else
            {
                IsComOpen = false;
                MBInterpreter.ModbusCom.mySerialPort.Close();
                Gadgets.SetLedColor(pictureLEDConnectATP, "Blue");
                buttonConnectATP.Text = "Connect";
                msg = string.Format("Closed com port");
            }
            ATPTest_textBoxMessage2Humanity.Text = msg;
        }


        private void Click_buttonConnectATP(object sender, EventArgs e)
        {
            ButtonConnectATPBody();
        }

        private void Click_ButtonFindAddress(object sender, EventArgs e)
        {
            if ( !IsComOpen )
            {
                MessageBox.Show(this, "Please open communications first");
                return; 
            }
            MessageBox.Show(this,"Make sure that only one device (an ADC) is connected to RS485"); 
            if ( Adc.ReadDeviceAddress(out byte address))
            {
                //Relay.SetAddress(address);
                Adc.SetAddress(address);
                numericUpDownModbusAddress.Value = address;
                ATPTest_textBoxMessage2Humanity.Text = "Succesful read of ADC address";
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "failed to read ADC address";
            }
        }

        private void ButtonSetRelayCommand_Click(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }
            string str = textBoxRelayCommand.Text.Trim();
            int decimalValue;
            try
            {
                if ( str.StartsWith("0x") || str.StartsWith("0X"))
                {
                    decimalValue = Convert.ToInt32(str, 16);
                }
                else
                {
                    decimalValue = Convert.ToInt32(str); 
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(this,"Invalid number format.");
                return;
            }

            Relay.SetEndianness(checkBoxLittleEndian.Checked); 
            Relay.SetAddress((byte) numericUpDownRelayAddress.Value);

            byte DeviceAddress = (byte)numericUpDownRelayAddress.Value; 
            if ( Relay.WriteRelayStateAsNumber((uint) decimalValue , DeviceAddress))
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { $"Succesfully issued switching command", $"to the relay box, ID= {DeviceAddress} " };
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string []{ "Could not write a MODBUS command", "to the relay box" };
            }

        }

        private void ButtonSetUnits_Click(object sender, EventArgs e)
        {
            // Set the ADC units to 4 
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }
            bool stat = Adc.SetMultipleAdcDataType(); 
            if ( stat )
            {
                ATPTest_textBoxMessage2Humanity.Text = "Succesfully set ADC units"; 
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "Failed to set ADC units";
            }
        }

        private void ButtonGetUnits_Click(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }
            bool stat = Adc.GetAdcDataType();
            if (stat)
            {
                string junk = "ADC Units : "; 
                for ( int cnt = 0; cnt < 8; cnt++)
                {
                    junk += Adc.DataTypeReadback[cnt]; 
                    if ( cnt < 7 )
                    {
                        junk += " , "; 
                    }
                }
                ATPTest_textBoxMessage2Humanity.Text =  junk ;
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "Failed to read ADC units";
            }
        }

        private void ButtonAdcGetVersion_Click(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }
            bool stat = Adc.ReadDeviceSwVersion();

            if (stat)
            {
                double rev = Adc.SwRevision / 100; 
                string junk = "ADC SW version : " + rev.ToString("F2") ;
                ATPTest_textBoxMessage2Humanity.Text = junk;
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "Failed to read ADC SW revision";
            }
        }

        private void ButtonReadAdc_Click(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }

            bool stat = Adc.ReadAdcData();

            if (stat)
            {
                ATPTest_textBoxMessage2Humanity.Text = "Succesful read of ADC output data";

                for ( int cnt = 0; cnt < 8; cnt++)
                {
                    dataGridViewAdcResults.Rows[cnt].Cells[0].Value = Adc.DataBits[cnt];
                    dataGridViewAdcResults.Rows[cnt].Cells[1].Value = Adc.Volts[cnt];
                }
                //double rev = Adc.DataBits / 100; Volts
                //string junk = "ADC SW version : " + rev.ToString("F2");
                //ATPTest_textBoxMessage2Humanity.Text = junk;
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "Failed to read ADC output data";
            }

        }

        private async void Click_CalibAdc(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }

            ATPTest_textBoxMessage2Humanity.Lines = new string[] { "Connect the calibration connector to the Waveshare 8-input ADC","Have only the calibrated ADC on the RS485, you could use [Find] to set its address","Press next"};
            PrepNext(); 
            await (Task.Run(BlockTillNext));
            if (!CleanNext()) return;

            double[] TestVoltages = new double[]{1,2,4,8,10 };
            double[] ActVoltages = new double[TestVoltages.Length];
            double[][] MeasVoltages = new double[8][ ];
            int nAdcSamps = 10;
            double[,] rslt = new double[8, nAdcSamps];
            for ( int cnt = 0; cnt < 8; cnt++)  MeasVoltages[cnt] = new double[TestVoltages.Length];

            // Set the correct ADC units
            if ( !Adc.SetMultipleAdcDataType(0) )
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { "Failed to program ADC units", "Sorry, goodbye" };
                return;
            }

            // Loop over all the voltages
            for ( int cnt = 0; cnt < TestVoltages.Length; cnt++)
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { $"Set about {TestVoltages[cnt]}V", "to the Waveshare 8-input ADC", "Write the measured voltage in the result box", "Press next" };
                PrepNext();
                ExpectedVolts = TestVoltages[cnt];
                await (Task.Run(BlockTillNextWithValue ));
                if (!CleanNext()) return;
                ATPTest_textBoxMessage2Humanity.Lines = new string[] {"Sampling data","Wait..." };
                ActVoltages[cnt] = (double) numericUpDownResult.Value;

                bool stat;
                for ( int c1 = 0; c1 < nAdcSamps; c1++)
                {
                    stat = Adc.ReadAdcData();
                    if (!stat )
                    {
                        CleanNext(new string[] {"Sorry ... ","Had problems reading ADC","Aborted" } ) ;
                        return; 
                    }
                    for ( int c2 = 0; c2 < 8; c2++ )
                    {
                        rslt[c2, c1] = Adc.RawVolts[c2];
                    }
                }

                 // Get averaged measurement voltages
                for (int c2 = 0; c2 < 8; c2++)
                {
                    for (int c1 = 0; c1 < nAdcSamps; c1++)
                        MeasVoltages[c2][cnt] += rslt[c2, c1];
                    MeasVoltages[c2][cnt] /= nAdcSamps;
                    dataGridViewAdcResults.Rows[c2].Cells[0].Value = (int) (1000 * MeasVoltages[c2][cnt]);
                    dataGridViewAdcResults.Rows[c2].Cells[1].Value = MeasVoltages[c2][cnt];
                }
            } // End voltages loop

            CleanNext(new string[] { "Displaying result"});
            CRegression Regression;
            string filePath;
            string CalibDate; 
            double[] Gain = new double[8];
            double[] Offset = new double[8];
            double[] MaxAbsErr = new double [8];
            double[] RmsErr = new double [8];
            bool[] Approve = new bool[8];
            CalibDate = "Undefined";
            for (int cnt = 0; cnt < 8; cnt++)
            {
                string grlabel = " ADC Channel : " + (cnt + 1).ToString(); 
                Regression = new CRegression (MeasVoltages[cnt], ActVoltages, cnt+ 1 , grlabel) ;
                Regression.ShowDialog();
                filePath = "RegressionOut_" + (cnt+1).ToString() + ".txt";
                string[] lines = File.ReadAllLines(filePath);
                CalibDate = lines[1];
                Gain[cnt] = Convert.ToDouble(lines[2]);
                Offset[cnt] = Convert.ToDouble(lines[3]);
                MaxAbsErr[cnt] = Convert.ToDouble(lines[4]);
                RmsErr[cnt] = Convert.ToDouble(lines[5]);
                Approve[cnt] = Convert.ToBoolean(lines[6].ToLower() );
                if ( ! Approve[cnt])
                {
                    CleanNext(new string[] { "At least one calibration was rejected by user", "Goodbye" });
                }
            }

            // Write the result into the calibration Excel 
            string xlsfilePath = labelCalibDirectoryPath.Text + @"\" + label1CalibFileName.Text ; //  "TesterCalib.xlsx";
            string sheetName = "ADC" + Adc.address.ToString();

            XLWorkbook workbook;

            if (File.Exists(xlsfilePath))
            {
                // Open the existing file
                try
                {
                    workbook = new XLWorkbook(xlsfilePath);
                }
                catch
                {
                    MessageBox.Show($"Cant open file {xlsfilePath} \nProbably open in Excel. \nClose it and press Ok"); 
                    try
                    {
                        workbook = new XLWorkbook(xlsfilePath);
                    }
                    catch
                    {
                        MessageBox.Show($"Still Cant open file {xlsfilePath} \nCalibration results will be lost");
                        return;
                    }
                }
            }
            else
            {
                // Create a new workbook
                workbook = new XLWorkbook();
            }

            IXLWorksheet worksheet;
            if (workbook.Worksheets.Contains(sheetName))
            {
                worksheet = workbook.Worksheet(sheetName);
            }
            else
            {
                worksheet = workbook.Worksheets.Add(sheetName);
            }
            worksheet.Cell("B1").Value = "Gain";
            worksheet.Cell("C1").Value = "Offset";
            worksheet.Cell("D1").Value = "MaxAbsErr";
            worksheet.Cell("E1").Value = "RmsErr";
            worksheet.Cell("F1").Value = "Date";
            worksheet.Cell("F2").Value = CalibDate ;

            for ( int cnt = 0; cnt < 8; cnt++)
            {
                string colstr = (2 + cnt).ToString();

                worksheet.Cell("A" + colstr).Value = cnt+1;
                worksheet.Cell("B"+ colstr).Value = Gain[cnt];
                worksheet.Cell("C" + colstr).Value = Offset[cnt];
                worksheet.Cell("D" + colstr).Value = MaxAbsErr[cnt];
                worksheet.Cell("E" + colstr).Value = RmsErr[cnt];
            }

            worksheet.Columns().AdjustToContents(); // Adjust column widthes

            // Save tester serial number
            IXLWorksheet worksheetHeader;
            if (workbook.Worksheets.Contains("Tester"))
            {
                worksheetHeader = workbook.Worksheet("Tester");
            }
            else
            {
                worksheetHeader = workbook.Worksheets.Add("Tester");
            }
            worksheetHeader.Cell("A1").Value = "S/N";
            worksheetHeader.Cell("A2").Value = (int)numericUpDownTesterSerial.Value;


            if (File.Exists(xlsfilePath))
            {
                workbook.Save(); 
            }
            else
            {
                // Create a new workbook
                // Open the existing file
                workbook.SaveAs(xlsfilePath);
            }
            CleanNext(new string[] { "Calibration resuls written into calibration file","Goodbye" });
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

        bool CleanNext(string[] msg = null )
        {
            if ( msg == null )
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
                MessageBox.Show( "Entered result must be within 0.5V of target \nPlease try again, then press next Press next" );
            }
            JustANext = (index == 0);
        }

        private void BlockTillNext( )
        {
            int index = WaitHandle.WaitAny(new WaitHandle[] { NextButtonClickedEvent, AbortButtonClickedEvent });
            JustANext = (index == 0) ;
        }

        private void ButtonCalibNext_Click(object sender, EventArgs e)
        {
            _ = NextButtonClickedEvent.Set(); 
        }

        private void Click_AbortCalib(object sender, EventArgs e)
        {
            AbortButtonClickedEvent.Set(); 
        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }

        private void ButtonSetBaud_Click(object sender, EventArgs e)
        {
            int newbaud = Convert.ToInt32(comboBoxNewBaudRate.SelectedItem);
            Adc.SetBaudRate(newbaud);

            ATPTest_textBoxMessage2Humanity.Lines = new string[] { $"Remember to disconnect, set baud rate to  {newbaud}", "and reconnect" };
            //comboBoxBaudRate
        }

        private void NumericUpDownModbusAddress_ValueChanged(object sender, EventArgs e)
        {
            Adc.SetAddress( (byte) numericUpDownModbusAddress.Value); 
        }

        private void ButtonSetAddress_Click(object sender, EventArgs e)
        {
            byte newaddress = (byte)numericUpDownNewAddress.Value;
            if ( Adc.SetDeviceAddress(newaddress) )
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { $"ADC Address successfuly set to  {newaddress}" };
                numericUpDownModbusAddress.Value = newaddress;
            }
        }

        private void ButtonSetRelayAddress_Click(object sender, EventArgs e)
        {
            byte newaddress = (byte)numericUpDownNewRlayAddress.Value;
            if (Relay.SetDeviceAddress(newaddress))
            {
                ATPTest_textBoxMessage2Humanity.Lines = new string[] { $"Relay Address successfuly set to  {newaddress}" };
                numericUpDownRelayAddress.Value = newaddress;
            }
        }

        private void Click_ButtonFindRelayAddress(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show(this, "Please open communications first");
                return;
            }
            MessageBox.Show(this, "Make sure that only one device (Relay) is connected to RS485");
            if (Relay.ReadDeviceAddress(out byte address))
            {
                Relay.SetAddress(address);
                numericUpDownRelayAddress.Value = Relay.address;
                ATPTest_textBoxMessage2Humanity.Text = "Succesful read of relay address";
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "failed to read relay address";
            }

        }

        private void ButtonSetAesterCalibDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder for Calibration results file";

                folderDialog.SelectedPath = labelCalibDirectoryPath.Text;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected folder path
                    labelCalibDirectoryPath.Text = folderDialog.SelectedPath; ;
                }
                else
                {
                    ATPTest_textBoxMessage2Humanity.Lines = new string[] {
                        $"No folder was selected.",
                        $"Recommended: {DefaultCalibPath}"};
                }
            }

        }

        private void ValueChanged_TesterSN(object sender, EventArgs e)
        {
            label1CalibFileName.Text = "TesterCalib_" + numericUpDownTesterSerial.Value.ToString() + @".xlsx";
        }

        private void NumericUpDn_RelayModbusAddressChanged(object sender, EventArgs e)
        {
            Relay.SetAddress((byte) numericUpDownRelayAddress.Value); 
        }

        private void Click_GetRelayVersion(object sender, EventArgs e)
        {
            if (!IsComOpen)
            {
                MessageBox.Show("Open communication first");
                return;
            }
            Relay.SetAddress((byte)numericUpDownRelayAddress.Value);
            bool stat = Relay.ReadDeviceSwVersion();

            if (stat)
            {
                double rev = Relay.SwRevision / 100;
                string junk = "Relay SW version : " + rev.ToString("F2");
                ATPTest_textBoxMessage2Humanity.Text = junk;
            }
            else
            {
                ATPTest_textBoxMessage2Humanity.Text = "Failed to read Relay SW revision";
            }
        }

        private void UpdateSwNumericByCheckbox()
        {
            if ( !UpdateSwBoxes)
            {
                return; 
            }
            CheckBox[] cb = new CheckBox[]  { checkBoxK1 , checkBoxK2, checkBoxK3 , checkBoxK4 , checkBoxK5 , checkBoxK6 , checkBoxK7, checkBoxK8 ,
                checkBoxK9 , checkBoxK10, checkBoxK11 , checkBoxK12 , checkBoxK13 , checkBoxK14 , checkBoxK15, checkBoxK16,
                checkBoxK17 , checkBoxK18, checkBoxK19 , checkBoxK20 , checkBoxK21 , checkBoxK22 , checkBoxK23, checkBoxK24,
                checkBoxK25 , checkBoxK26, checkBoxK27 , checkBoxK28 , checkBoxK29 , checkBoxK30 , checkBoxK31, checkBoxK32
            };  
            int a = 0; 
            for ( int cnt = 0; cnt < cb.Length; cnt++ ) 
            {
                a += cb[cnt].Checked ? (1 << cnt) : 0; 
            }
            textBoxRelayCommand.Text = "0x" + a.ToString("X");
        }

        private void ComboBoxPortsAtp_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxK1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox(); 
        }

        private void checkBoxK2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK4_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK5_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK6_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK7_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK8_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK9_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK10_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK11_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK12_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK13_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK14_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK15_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK16_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK17_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK19_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK18_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK21_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK20_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK22_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK23_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK24_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK25_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK26_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK27_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK28_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK29_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK30_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK31_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void checkBoxK32_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSwNumericByCheckbox();
        }

        private void textBoxRelayCommand_TextChanged(object sender, EventArgs e)
        {
            string str = textBoxRelayCommand.Text.Trim();
            uint decimalValue;
            try
            {
                if (str.StartsWith("0x") || str.StartsWith("0X"))
                {
                    decimalValue = (uint) Convert.ToInt32(str, 16);
                }
                else
                {
                    decimalValue = (uint) Convert.ToInt32(str);
                }
                CheckBox[] cb = new CheckBox[]  { checkBoxK1 , checkBoxK2, checkBoxK3 , checkBoxK4 , checkBoxK5 , checkBoxK6 , checkBoxK7, checkBoxK8 ,
                checkBoxK9 , checkBoxK10, checkBoxK11 , checkBoxK12 , checkBoxK13 , checkBoxK14 , checkBoxK15, checkBoxK16,
                checkBoxK17 , checkBoxK18, checkBoxK19 , checkBoxK20 , checkBoxK21 , checkBoxK22 , checkBoxK23, checkBoxK24,
                checkBoxK25 , checkBoxK26, checkBoxK27 , checkBoxK28 , checkBoxK29 , checkBoxK30 , checkBoxK31, checkBoxK32
                };
                UpdateSwBoxes = false;
                for (int cnt = 0; cnt < cb.Length; cnt++)
                {
                    cb[cnt].Checked = ( ( (1 << cnt) & decimalValue ) != 0 ) ;
                }
                UpdateSwBoxes = true;
            }
            catch
            {
                return;
            }
        }
    }
}
