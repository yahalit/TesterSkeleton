using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.Runtime.InteropServices.ComTypes;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing;
using System.IO.Ports;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using System.Text.Json;
using Plot;
using System.Windows.Interop;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Threading;
using System.Management;
using static TesterGUI.CAtpExcel;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Bibliography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace TesterGUI
{

    // Test information - Location in ATR 
    public struct CTestInfo
    {
        public string TestIdentifier; // Test identifier, like 1.2.3
        public string TestHeader;     // Header for  the test in the ATR 
        public string TestSheet;      //  Sheet in the ATR for test results 
        public string ErrorMessage;   // Error message, in case of test failure 
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Make field read only", Justification = "Tester is used as non-readonly elsewhere")]
    public partial class CAtpMainWindow : Form
    {
        readonly List<CheckBox> TestCheckboxList = new List<CheckBox>();
        CTester Tester = CTester.Instance;
        public CModbusInterpreter MBInterpreter = CModbusInterpreter.Instance; //  new CInterpreter(mutex);
        public CInterpreter Interpreter = CInterpreter.Instance; //  new CInterpreter(mutex);
        public CHostInterpreter  HostInterpreter = CHostInterpreter.Instance; //  new CInterpreter(mutex);
        CGUI_SetRecorderParameters rp = new CGUI_SetRecorderParameters();

        public CAtpExcel AtpExcel = new CAtpExcel();
        readonly string DefaultAtrPath = null;
        readonly string ATRTemplatePath = null;
        bool IsGUIComOpen;
        bool IsHostComOpen;
        bool IsTesterComOpen;
        bool BTEchPasswordEntered    ;
        readonly string TechnicianPassWord;
        readonly string BTEchPasswordClue; 

        readonly string DefaultCalibPath;
        readonly string DefaultCalibFileName;

        readonly string ProjectName; 

        //double NumericUserAnswer;
        GetValueResults UserAnswer   ;
        bool UserApporoved;
        public CRecorder Recorder = CRecorder.Instance; //  new CInterpreter(mutex);
        public string RootFilePath;

        public CAtpMainWindow(string _ProjectName, string _DefaultAtrPath, string _ATRTemplatePath, string _DefaultCalibPath , string _RootFilePath )
        {
            InitializeComponent();
            ProjectName = _ProjectName ;

            //  !! Do NOT remove or chage this Entry Point comment !!
            // [EP4] Decide if a password is required and set password 
            ////////////////////////////////////////////
            BTEchPasswordEntered = true;  // Set false if password is required
            TechnicianPassWord = "Pafuzlachi";
            BTEchPasswordClue = "The terrible beast that is afraid of Omer";


            DefaultAtrPath = _DefaultAtrPath;
            ATRTemplatePath = _ATRTemplatePath;
            labelAtrDirectoryPath.Text= _DefaultAtrPath;
            textBoxATRFileName.Text = ProjectName+ "_ATR_SN_xxx_" + DateTime.Now.ToString("yyyy_MM_dd") + @"_R1.xlsx";
            DefaultCalibPath = _DefaultCalibPath;
            DefaultCalibFileName = DefaultCalibPath + @"\" + @"TesterCalib_1.xlsx";
            labelTesterCalibrationFile.Text = DefaultCalibFileName;
            RootFilePath = _RootFilePath; 

            textBoxMessageToHumanity.Lines = new string[] {"Please copy AtpTemplate_xxx.xlsx to a results file for a new ATP",
                                              "And select your copied (or existing for continuoed ATP) xlsx file as ATP file",
                                               "Select Testbox calibration file, Fill the technician password",
                                               "Select the tests to perform, and press [Start Atp]" };

            //  !! Do NOT remove or chage this Entry Point comment !!
            // [EP1] Create a list of all the tests to perform 
            ////////////////////////////////////////////
            TestCheckboxList.Add(checkPowerOnConsumptionLED);
            TestCheckboxList.Add(checkBoxInstallationAndVersions);
            TestCheckboxList.Add(checkBoxHostCommunicationRs422);
            TestCheckboxList.Add(checkBoxInternalCommunicationFSI);
            TestCheckboxList.Add(checkBoxCollerFans);
            TestCheckboxList.Add(checkBoxTemperatureMeas);
            TestCheckboxList.Add(checkBoxVoltageMeas);
            TestCheckboxList.Add(checkBoxSSRPowerOn);
            TestCheckboxList.Add(checkBoxPanelLEDs);
            TestCheckboxList.Add(checkBoxSerialFlash);
            TestCheckboxList.Add(checkBoxReset);
            TestCheckboxList.Add(checkBoxBootSwitch);
            TestCheckboxList.Add(checkBoxPressureAdcInput);
            TestCheckboxList.Add(checkBoxCFGInputs);
            TestCheckboxList.Add(checkBoxPressureOutJ1);
            TestCheckboxList.Add(checkBoxPressureOutJ7);
            TestCheckboxList.Add(checkBoxSolenoidCurrentReport);
            TestCheckboxList.Add(checkBoxSolenoidStateReport);
            TestCheckboxList.Add(checkBoxSimulatorIDOutputs);
            TestCheckboxList.Add(checkBoxVoltageTestNoLoad);
            TestCheckboxList.Add(checkBoxVoltageAndCurrents);
            TestCheckboxList.Add(checkBoxJ4Plug);
            TestCheckboxList.Add(checkBoxIsolation);

            // Initialize all the test function checks. 
            foreach (CheckBox cb in TestCheckboxList)
            {
                cb.Checked = true; // Default all tests to execute
                cb.Tag = null;
            }


            //  !! Do NOT remove or chage this Entry Point comment !!
            // [EP2] Tag each test checkbox with its test information 
            ////////////////////////////////////////////
            checkPowerOnConsumptionLED.Tag = new CTestIdentifier(TestPowerOnConsumptionLED, "1.0.", Sheet: "Drawer Level tests", Header: "Power On, Power consumption, Power ON LED");
            checkBoxInstallationAndVersions.Tag = new CTestIdentifier(TestInstallationAndVersions, "1.1.", Sheet: "Drawer Level tests", Header : "Installation and Versions");
            checkBoxHostCommunicationRs422.Tag = new CTestIdentifier(TestHostCommunicationRs422, "1.2.", Sheet: "Drawer Level tests", Header: "Host communication");
            checkBoxInternalCommunicationFSI.Tag = new CTestIdentifier(TestInternalCommunicationFSI, "1.3.", Sheet: "Drawer Level tests", Header: "Internal Communication - FSI");
            checkBoxCollerFans.Tag = new CTestIdentifier(TestCollerFans, "1.4.", Sheet: "Drawer Level tests", Header: "Cooler fans");
            checkBoxTemperatureMeas.Tag = new CTestIdentifier(TestTemperatureMeas, "1.5.", Sheet: "Drawer Level tests", Header: "Temperature measurement");
            checkBoxVoltageMeas.Tag = new CTestIdentifier(TestVoltageMeas, "1.6.", Sheet: "Drawer Level tests", Header: "BIT Voltage measurement");
            checkBoxSSRPowerOn.Tag = new CTestIdentifier(TestSSRPowerOn, "1.7.1.", Sheet: "Drawer Level tests", Header: "SSR and Power On");
            checkBoxPanelLEDs.Tag = new CTestIdentifier(TestPanelLEDs, "1.7.3", Sheet: "Drawer Level tests", Header: "Panel LEDs");
            checkBoxSerialFlash.Tag = new CTestIdentifier(TestRandomSerialFlashRW, "2.1.0", Sheet: "Main CPU test", Header: "Serial Flash");
            checkBoxReset.Tag = new CTestIdentifier(TestBoxReset, "2.2.0", Sheet: "Main CPU test", Header: "Reset");
            checkBoxBootSwitch.Tag = new CTestIdentifier(TestBootSwitch, "2.3.0", Sheet: "Main CPU test", Header: "Boot switch");
            checkBoxPressureAdcInput.Tag = new CTestIdentifier(TestPressureAdcInput, "2.4.0.", Sheet: "Panel Control Tests", Header: "ADC input");
            checkBoxCFGInputs.Tag = new CTestIdentifier(TestCFGInputs, "2.4.1.", Sheet: "Panel Control Tests", Header: "CFG input");
            checkBoxPressureOutJ1.Tag = new CTestIdentifier(TestPressureOutJ1, "2.5.", Sheet: "Panel Control Tests", Header: "Pressure Outputs at J1");
            checkBoxPressureOutJ7.Tag = new CTestIdentifier(TestPressureOutJ7, "2.6.", Sheet: "Panel Control Tests", Header: "Pressure Outputs at J7");
            checkBoxSolenoidCurrentReport.Tag = new CTestIdentifier(TestSolenoidCurrentReport, "2.7.", Sheet: "Panel Control Tests", Header: "Solenoid Current Report");
            checkBoxSolenoidStateReport.Tag = new CTestIdentifier(TestSolenoidStateReport, "2.8.", Sheet: "Panel Control Tests", Header: "Solenoid State Report");
            checkBoxSimulatorIDOutputs.Tag = new CTestIdentifier(TestSimulatorIDOutputs, "2.9.", Sheet: "Panel Control Tests", Header: "Simulator ID output");
            checkBoxVoltageTestNoLoad.Tag = new CTestIdentifier(TestVoltageTestNoLoad, "3.1.", Sheet: "Power Unit Tests", Header: "Voltage no load");
            checkBoxVoltageAndCurrents.Tag = new CTestIdentifier(TestVoltageAndCurrents, "3.2.", Sheet: "Power Unit Tests", Header: "Voltage no load");
            checkBoxJ4Plug.Tag = new CTestIdentifier(TestBoxJ4Plug, "3.4.", Sheet: "Power Unit Tests", Header: "J4 Plug");
            checkBoxIsolation.Tag = new CTestIdentifier(TestIsolation, "3.6", Sheet: "Power Unit Tests", Header: "Isolation");

 
            Gadgets.AddToolTip2Control(buttonSelectCalibFile, new string[] { "Select here the tester's calibration file",
                "The file contains also tester identification and date","so that selecting an outdated file or file for another tester will be rejected" }); 

            Gadgets.AddToolTip2Control(buttonSetATRDir, new string[] {"Select a folder in which ATR results are stored","Folder holds both xlsx sources and their published PDFs"});

            Gadgets.AddToolTip2Control(buttonOpenATRForm, new string[] { "A click shall open an Excel file by the selected name" ,
                "- If the file exists it will be opened" , 
                "- Otherwise a new file by that name will be generated from the ATR template"});

            Gadgets.AddToolTip2Control(buttonSelectAtpFile, new string[] { "Click to select an existing Excel file:" ,
                "for the ATR form" ,
                "Front page data must be filled before running ATP ot the form.", "You can use the Open button for that."});


            Gadgets.AddToolTip2Control(ButtonReviseAtpFile, new string[] { "Select a name of an existing xlsx file","Named Whatever_Rxx.xlsx with xx a number",
                 "The file will be copied to Whatever_Rxx+1.xlsx and set as ATR target"});

            Gadgets.AddToolTip2Control(buttonConnectTester, new string[] {"Pressing this button will connect all 3 UART connections:","GUI, Host simulator, and Tester.",
                "Be sure to set all the COM ports before, and that in the main dialog GUI is disconnected.","The lamps at green will verify port connections."});

            Gadgets.AddToolTip2Control(textBoxATRFileName, new string[] {"This is the name of the ATR file.","It is composed as "+ProjectName+"_ATR_SN_xxx_yyyy_mm_dd_Rrr.xlsx.",
                "xxx is the SN, yyyy the year, mm the month, dd the day, and rr the ATR re-test revision.","The ATR will be also published as PDF with the same name","Other than the .pdf extension"});

            //  !! Do NOT remove or chage this Entry Point comment !!
            // [EP3] Read details of available COM ports 
            ////////////////////////////////////////////
            Gadgets.UpdateAvailableComPorts(comboBoxPortsGUIATP);
            Gadgets.UpdateAvailableComPorts(comboBoxPortsHostATP);
            Gadgets.UpdateAvailableComPorts(comboBoxPortsTesterModbusATP);

            RestoreComs();
        }


        /* 
         * ConnectPort: Connect a COM port 
         * Arguments:
         * combo: The combo box in which the COM description string is selected
         * LED  : The LED to green when port is connected
         * baud : Required baud rate 
         * IsModbus: true for modbus connection 
         * 
         * Returns: 
         * true if succesful 
         * bComOpen: Indicator that the port is indeed opened 
         * msg : a string reflecting opening success
         */
        private bool ConnectPort (ComboBox combo, PictureBox LED, ref bool bComOpen , int baud , out string msg ,bool IsModbus  )
        {
            string com = (string)combo.Items[combo.SelectedIndex];

            int colonIndex = com.IndexOf(':');
            if (colonIndex != -1)
                com = com.Substring(0, colonIndex);

            bComOpen = false;
            try
            {
                if (IsModbus)
                {
                    bComOpen = MBInterpreter.ModbusCom.OpenSerialPort(com, baud, out msg);
                }
                else
                {
                    bComOpen = Interpreter.HostCom.OpenSerialPort(com, baud, out msg);
                }
            }
            catch
            {

            }
            if (bComOpen)
            {
                Gadgets.SetLedColor(LED, "Green");
                msg = string.Format("Port opened successfully");
                //MBInterpreter.GetVersion(isbg: true);
            }
            else
            {
                msg = string.Format("Could not open port");
            }
            return bComOpen; 
        }


        /* 
        * DisconnectPort: Disconnect a COM port 
        * Arguments:
        * LED  : The LED to green when port is connected
        * IsModbus: true for modbus connection 
        * 
        * Returns: 
        * bComOpen: Indicator that the port is indeed opened 
        */
        private void DisconnectPort( PictureBox LED, ref bool bComOpen , bool IsModbus)
        {
            try
            {
                if (IsModbus)
                {
                    MBInterpreter.ModbusCom.mySerialPort.Close();
                }
                else
                {
                    Interpreter.HostCom.mySerialPort.Close();
                }
            }
            catch
            {

            }
            bComOpen = false;
            //string msg = string.Format("Closed com port");
            Gadgets.SetLedColor(LED, "Blue");
        }



        /* 
         * Click_CheckAll: Check all the tests to the active state
         */
        private void Click_CheckAll(object sender, EventArgs e)
        {
            foreach (CheckBox cb in TestCheckboxList)
            {
                cb.Checked = true;
            }
        }

        /* 
        * Click_UncheckAll: UnCheck all the tests to the inactive state
        */
        private void Click_UncheckAll(object sender, EventArgs e)
        {
            foreach (CheckBox cb in TestCheckboxList)
            {
                cb.Checked = false;
            }
        }

        /* 
         * Display an instruction; Get a value (numeric or text), and/or check
         * Header: Dialog header
         * CheckText : If a checkbox is wanted , the label to display at the checkbox, otherwise null
         * IsText    : If the user is to fill a text. false if the user is to fill a number
         * lowlimit,highlimit: Limits to numeric inputs; apply only if IsText == false
         * Instruction : Instructions for the tester, displayed at the bottom of the dialog
         */
        public GetValueResults GetUserValue(string Header, string CheckText , bool IsText, double lowlimit = -1e9, double highlimit = 1e9 , string[] Instruction = null)
        {
            UserApporoved = false ; 
            AsyncGetUserValue(Header, CheckText, IsText, lowlimit, highlimit, Instruction);
            while ( !UserApporoved )
            {
                Thread.Sleep(100);
            }
            return UserAnswer;
        }

        // Helper function for GetUserValue()
        public async void AsyncGetUserValue(string Header, string CheckText , bool IsText, double lowlimit = -1e9, double highlimit = 1e9, string[] Instruction = null)
        {
            UserAnswer = await QuestionDlg.ShowAsynchronously(Header, CheckText , IsText, lowlimit, highlimit, Instruction);
            UserApporoved = true;
        }


        /* 
        * Main ATP running loop
        * Goes over all checked test functions and run them 
        */
        private void Click_StartATP(object sender, EventArgs e)
        {
            CTestIdentifier TestId;

            // If password is enabled, require it.
            if ( !BTEchPasswordEntered )
            {
                GetValueResults rslt =
                GetUserValue("Enter technician password", CheckText: null, IsText: true, lowlimit: 0, highlimit: 1, new string[] { "Please enter correct technician password", "otherwise access shall be denied" });

                string TrimmedResult = rslt.s.Trim();

                if (!TechnicianPassWord.Equals(TrimmedResult))
                {
                    textBoxMessageToHumanity.Lines = new string[] {
                    "Testing is only by technician password",
                    "Clue: " + BTEchPasswordClue,  
                    "Sorry" };
                    return;
                }
                BTEchPasswordEntered = true; 
            }

            // Verify legality of ATR file name 
            if (!Gadgets.CheckLegitimateAtrFileName(ProjectName,textBoxATRFileName.Text, out string errmsg, out _))
            {
                MessageBox.Show(this, "Ilegal file name, see details in message lines below");
                textBoxMessageToHumanity.Lines = new string[] {
                        $"Error: {errmsg} : File name fomat must be like "+ProjectName+"_ATR_SN_001_2025_01_14_R1",
                        "Format: "+ProjectName+"_ATR_SN_[Serial number]_[Year]_[Month]_[Day]_R[Revision]",
                        "Serial number: 1 to 1000, Year 2000 to 2200, Month 1 to 12, Day 1 to 31, Revision 0 to 1000",
                        "Where numbers should be placed for each of the square brackets"
                };
                return;
            }

            // Compose the full path for the ATR file and make it common for all test descriptors
            string AtrFileName = labelAtrDirectoryPath.Text + @"\" + textBoxATRFileName.Text;
            foreach (CheckBox cb in TestCheckboxList)
            {
                (cb.Tag as CTestIdentifier).ExcelFileName = AtrFileName;
                (cb.Tag as CTestIdentifier).TempFolder = labelAtrDirectoryPath.Text;
			}

            // Deal the front page of the ATR excel
            // ////////////////////////////////////

            // Verify ATR file is accesible (existing and not open by other means) 
            if (!Gadgets.IsExcelFileAccessible(labelAtrDirectoryPath.Text  + @"\" + textBoxATRFileName.Text , out string msg))
            {
                textBoxMessageToHumanity.Lines = new string[] {
                    "Could not open ATR file",
                     msg };
                return;
            }

            // Parse the front page of the ATR for experiment details 
            if (!AtpExcel.ParseTestsFrontPage(labelAtrDirectoryPath.Text, textBoxATRFileName.Text, out errmsg))
            {
                textBoxMessageToHumanity.Lines = new string[] {
                    "Could not open the ATR file",
                    $"Error: {errmsg} "};
                return;
            }

            AtpExcel.TestForm.CpuSwVersion = (int)Interpreter.Answer_GetUnitData.SimSubverPatch;

            // Open tester calibration file 
            if (!Gadgets.IsExcelFileAccessible(labelTesterCalibrationFile.Text, out msg))
            {
                textBoxMessageToHumanity.Lines = new string[] {
                    "Could not open calibration file",
                     msg };
                return;
            }


            if (!Tester.InitTester(labelTesterCalibrationFile.Text, out msg))
            {
                textBoxMessageToHumanity.Lines = new string[] {
                    "Could not initialize tester",
                    msg };
                return;
            }

            // Go to GUI control mode 
            if (!Interpreter.SetGUIInCharge())
			{
                textBoxMessageToHumanity.Lines = new string[] {
					"Could not set the EUT to GUI mode",
					msg };
				return;
			}

			// Initialie recorder facilities 
			if(!Recorder.PopulateSignalList(RootFilePath + @"..\..\Exe\ProjRecorderSignals.h", RootFilePath + @"..\..\RecordedData", OpenGUI: true ))
            {
                textBoxMessageToHumanity.Lines = new string[] {
                    "Could not set the read list of recorder signals",
                    msg };
                return;
            }


            // Run the tests themselves
            bool TotalPass = true;
            bool TotalPassFromLastFail = true;


            for (int cnt = 0; cnt < TestCheckboxList.Count; cnt++)
            {
                if (!TestCheckboxList[cnt].Checked)
                {
                    continue; 
                }


                TestId = (CTestIdentifier)TestCheckboxList[cnt].Tag;


                // Locate the place for results in the ATR Excel 
                if (!AtpExcel.LocateTestByIdentifier(AtrFileName, TestId, out int RowIndex))
                {
                    textBoxMessageToHumanity.Lines = new string[] {
                        "Could not find the test identifier in the ATR form",
                        $"Test {TestId.TestInfo.TestIdentifier} : {TestId.TestInfo.TestHeader}"};
                    return; 
                }

                TestId.RowIndex = RowIndex;
                TestId.ErrMsg.Clear();

                // The test function itself
                bool SpecificPass = TestId.TestFunc(TestId);

                if (SpecificPass)
                {
                    textBoxMessageToHumanity.Lines = new string[] {
                        $"Testing {TestId.TestInfo.TestIdentifier} : {TestId.TestInfo.TestHeader} completed with success"};
                    textBoxMessageToHumanity.Refresh();
                }
                else
                {
                    string[] lines= new string[1+ TestId.ErrMsg.Count];
                    textBoxMessageToHumanity.Lines = lines;
                    lines[0] = $"Testing {TestId.TestInfo.TestIdentifier} : {TestId.TestInfo.TestHeader} completed with Failure"; 
                    for ( int linecnt = 1; linecnt <= TestId.ErrMsg.Count; linecnt++)
                    {
                        lines[linecnt] = TestId.ErrMsg[linecnt - 1]; 
                    }
                    textBoxMessageToHumanity.Lines = lines;
                    textBoxMessageToHumanity.Refresh();
                }


                TotalPass &= SpecificPass;
                TotalPassFromLastFail &= SpecificPass;
                if (!TotalPassFromLastFail)
                {
                    TotalPassFromLastFail = true; 
                    string FailStr = $"Test Identifier {TestId.TestInfo.TestIdentifier} : {TestId.TestInfo.TestHeader}";
                    if ( checkStopOnFail.Checked)
                    {
                        textBoxMessageToHumanity.Lines = new string[]
                        {
                            "Testing sequence stopped after failure",
                            TestId.ErrMsg[0] ,
                            FailStr,
                            "Because tester instructed to stop after single failure"
                        };
                        return;
                    }
                    else
                    {
                        if (TestId.ErrMsg.Count == 0 )
                        {
                            TestId.ErrMsg.Add("Unspecified error "); 
                        }
                        DialogResult result = MessageBox.Show(
                            "Testing sequence paused after failure \n" + TestId.ErrMsg[0] + "\n" + FailStr,
                            @"Do you wish to proceed?",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        // Check the user's choice
                        if (result != DialogResult.Yes)
                        {
                            textBoxMessageToHumanity.Lines = new string[]
                            {
                                "Testing sequence stopped after failure",
                                TestId.ErrMsg[0] ,
                                FailStr,
                                "Because tester asked to stop after recent failure"
                            };
                            return;
                        }
                    }
                }

            }
            string workbookPath = labelAtrDirectoryPath.Text + @"\" + textBoxATRFileName.Text;
            AtpExcel.ScanAtrResults(workbookPath, out CAtrStatistics stat);
            XLSGraph.PrintExcel(workbookPath);            
            string outcome = TotalPass ? "Success" : "Failure";
            textBoxMessageToHumanity.Lines = new string[] {
                    $"Testing sequence completed with Local {outcome}",
                    $"The total ATP result is {stat.nTests} tests in {stat.nPages} pages",
                    $"Success: {stat.nSuccess} , Failed: {stat.nFailure} , NA: {stat.nNonApplicable} , Undone: {stat.nEmpty}"
            };
        }



        private void Click_ConnectTester(object sender, EventArgs e)
        {
            bool TesterConnected = ConnectPort(comboBoxPortsTesterModbusATP, pictureLEDConnectTestATP, ref IsTesterComOpen, (int)Literals.System.ModbusBaudRate, out string msg1, IsModbus: true );
            bool GUIConnected = ConnectPort(comboBoxPortsGUIATP, pictureLEDConnectGUIATP, ref IsGUIComOpen, (int) Literals.System.GUIBaudRate, out string msg2 , IsModbus: false);


            IsHostComOpen = HostInterpreter.ConnectHostCom( action : true , (string)comboBoxPortsHostATP.SelectedItem , out string msg3);
            Gadgets.SetLedColor(pictureLEDConnectHostATP, IsHostComOpen ? "Green" : "Blue");

            // ConnectPort(comboBoxPortsTesterModbusATP, pictureLEDConnectHostATP, ref IsHostComOpen, (int)Literals.System.HostBaudRate, out msg, IsModbus: false);
            
            textBoxMessageToHumanity.Lines = new string[] { msg1 , msg2 , msg3 };

            if (!TesterConnected   )
            {
                return; 
            }

            if (!Interpreter.GetVersionOffline())
            {
                textBoxMessageToHumanity.Lines = new string[] { "Could not read the installation", "from the EUT" };
            }
            else
            {

            }

            // Get the unit data 
            if (!Interpreter.GetUnitProductionData())
            {
                MessageBox.Show(this, "Could not read the production data");
            }
            else
            {
                LabelMainCpuSW.Text = "0x" + Interpreter.Answer_GetUnitData.SimSubverPatch.ToString("X");
                LabelLLCSw.Text = "0x" + Interpreter.Answer_GetUnitData.LLCSubverPatch.ToString("X");
                LabelSIM_SW.Text = "0x" + Interpreter.Answer_GetUnitData.ValveSubverPatch.ToString("X");
            }

        }

        private void Click_DisconnectAtp(object sender, EventArgs e)
        {
            DisconnectPort( pictureLEDConnectGUIATP, ref IsGUIComOpen, IsModbus: false);
            //DisconnectPort(comboBoxPortsTesterModbusATP, pictureLEDConnectHostATP, ref IsHostComOpen, IsModbus: false);
            DisconnectPort( pictureLEDConnectTestATP, ref IsTesterComOpen, IsModbus: true);
            HostInterpreter.ConnectHostCom(action: false, (string)comboBoxPortsTesterModbusATP.SelectedItem, out _);
            IsHostComOpen = false; 
            Gadgets.SetLedColor(pictureLEDConnectHostATP, "Blue");
        }

        private void Click_SetATRDir(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder for ATR form";

                folderDialog.SelectedPath = labelAtrDirectoryPath.Text; 
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected folder path
                    string selectedFolder = folderDialog.SelectedPath;
                    string TemplatePath = Directory.GetParent(ATRTemplatePath).FullName;
                    if (selectedFolder.Equals(TemplatePath))
                    {
                        textBoxMessageToHumanity.Lines = new string[] {
                        $"The folder {ATRTemplatePath} is reserved for ATR templates",
                        $"You cant use it for ATR forms (Recommended: {DefaultAtrPath}"};
                        return; 
                    }
                    labelAtrDirectoryPath.Text = selectedFolder;
                }
                else
                {
                    textBoxMessageToHumanity.Lines = new string[] {
                        $"No folder was selected.",
                        $"Recommended: {DefaultAtrPath}"};
                }
            }

        }

        private void Click_OpenATRForm(object sender, EventArgs e)
        {
            string targetFilePath =   labelAtrDirectoryPath.Text + @"\" +textBoxATRFileName.Text; // Desired file
            string templateFilePath = ATRTemplatePath ; // Template file
            string errmsg;

            if (! Gadgets.CheckLegitimateAtrFileName(ProjectName,textBoxATRFileName.Text, out errmsg, out _)) 
            {
                MessageBox.Show(this,"Ilegal file name, see details in message lines below"); 
                textBoxMessageToHumanity.Lines = new string[] {
                        $"Error: {errmsg} : File name fomat must be like "+ProjectName+"_ATR_SN_001_2025_01_14_R1",
                        "Format: "+ProjectName+"_ATR_SN_[Serial number]_[Year]_[Month]_[Day]_R[Revision]",
                        "Serial number: 1 to 1000, Year 2000 to 2200, Month 1 to 12, Day 1 to 31, Revision 0 to 1000", 
                        "Where numbers should be placed for each of the square brackets"
                };
                return; 
            }

            try
            {
                if (targetFilePath.EndsWith(@".xlsx") && File.Exists(targetFilePath))
                {
                    if (Gadgets.OpenInExcel(targetFilePath, out errmsg))
                        return;

                    textBoxMessageToHumanity.Lines = new string[] {
                        $"Error: Microsoft Excel is not installed or cannot be accessed",errmsg};
                }
                else
                {
                    textBoxMessageToHumanity.Lines = new string[] {
                        $"Created a copy of the ATR template",
                        $"With the name you selected"};
                    File.Copy(templateFilePath, targetFilePath);
                    if (Gadgets.OpenInExcel(targetFilePath, out errmsg))
                        return;
                    textBoxMessageToHumanity.Lines = new string[] {
                        $"Error: Microsoft Excel is not installed or cannot be accessed",errmsg};
                }
            }
            catch (Exception ex)
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        $"An error occurred:", ex.Message};
            }
        }

        private void ButtonSelectAtpFile_Click(object sender, EventArgs e)
        {
            string filename = null;
            if (Gadgets.SelectExcelFile(labelAtrDirectoryPath.Text, ref filename))
            {
                    if (!Gadgets.CheckLegitimateAtrFileName(ProjectName,filename, out string errmsg, out _))
                    {
                        MessageBox.Show(this, "Ilegal file name, see details in message lines below");
                        textBoxMessageToHumanity.Lines = new string[] {
                            $"Error: {errmsg} : File name fomat must be like "+ProjectName+"_ATR_SN_001_2025_01_14_R1",
                            "Format: "+ProjectName+"_ATR_SN_[Serial number]_[Year]_[Month]_[Day]_R[Revision]",
                            "Serial number: 1 to 1000, Year 2000 to 2200, Month 1 to 12, Day 1 to 31, Revision 0 to 1000",
                            "Where numbers should be placed for each of the square brackets"
                    };
                    return;
                }
                textBoxATRFileName.Text = filename;
            }
            else
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        $"Could not select file",
                        $"Either aborted, or folder selection did not match the ATR folder"};
            }
        }

        private void Click_SelectCalibFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select the tester calibration file",
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                InitialDirectory = DefaultCalibPath, // Optional: Set initial directory
                CheckFileExists = true,    // Ensure file exists before allowing selection
                CheckPathExists = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                labelTesterCalibrationFile.Text = openFileDialog.FileName;
            }

        }

        private void Click_SaveComs(object sender, EventArgs e)
        {
            CComStoreDAta data = new CComStoreDAta
            {
                COM_TESTER = (string)comboBoxPortsTesterModbusATP.SelectedItem,
                COM_HOST = (string)comboBoxPortsHostATP.SelectedItem,
                COM_GUI = (string)comboBoxPortsGUIATP.SelectedItem
            };
            string filePath = "COM_Data.json";
            // Serialize and save to file
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            textBoxMessageToHumanity.Lines = new string[] {
                        $"COM ports identidiers",
                        $"Stored into {filePath} "};

        }

        private void RestoreComs()
        {
            string filePath = "COM_Data.json";
            CComStoreDAta data; 
            if (File.Exists(filePath))
            {
                // Read the JSON file
                string jsonString = File.ReadAllText(filePath);

                // Deserialize into object
                data = JsonSerializer.Deserialize<CComStoreDAta>(jsonString);

                try
                {
                    comboBoxPortsTesterModbusATP.SelectedItem = data.COM_TESTER;
                }
                catch { }
                try
                {
                    comboBoxPortsHostATP.SelectedItem = data.COM_HOST;
                }
                catch { }
                try
                {
                    comboBoxPortsGUIATP.SelectedItem = data.COM_GUI;
                }
                catch { }
            }
        }

        private void ComboBoxPortsTesterModbusATP_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxPortsHostATP_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBoxPortsGUIATP_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void DropDown_TesterPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsTesterModbusATP);
        }

        private void DropDownHostPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsHostATP);
        }

        private void DropDownGUIPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsGUIATP);
        }

        private void Enter_TesterPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsTesterModbusATP);
        }

        private void EnterHostPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsHostATP);
        }

        private void EnterGUIPort(object sender, EventArgs e)
        {
            Gadgets.UpdateAvailableComPorts(comboBoxPortsGUIATP);
        }

        private void Click_ReviseAtpFile(object sender, EventArgs e)
        {
            string filename = null;
            if (Gadgets.SelectExcelFile(labelAtrDirectoryPath.Text, ref filename))
            {
                textBoxATRFileName.Text = filename;
            }
            else
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        $"Could not select file",
                        $"Either aborted, or folder selection did not match the ATR folder"};
            }
            string newFilePath = filename;
            if (Gadgets.IncrementFileVersion(ref newFilePath))
            {
                if (File.Exists(labelAtrDirectoryPath.Text + @"\" + newFilePath))
                {
                    DialogResult result = MessageBox.Show(
                        $"File of name {newFilePath} exists",
                        @"Do you wish to overwrite it?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    // Check the user's choice
                    if (result != DialogResult.Yes)
                    {
                        textBoxMessageToHumanity.Lines = new string[] {
                            $"File selection aborted",
                            $"by user request"};
                        return;
                    }
                }
                File.Copy(labelAtrDirectoryPath.Text+@"\"+filename, labelAtrDirectoryPath.Text + @"\" + newFilePath, overwrite: true);
                textBoxATRFileName.Text = newFilePath;
            }
            else
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        $"Could not revise file",
                        $"For revision generation the file name must end with _Rxxx.xlsx"};
            }
        }

        private void ButtonAnalogInCalib_Click(object sender, EventArgs e)
        {
            if (!IsTesterComOpen)

            {
                MessageBox.Show("Open tester MODBUS communication first");
                return;
            }
            if ( !IsGUIComOpen)
            {
                MessageBox.Show("Open GUI communication first");
                return;
            }

            bool stat = CalibPressureAdcInput(out string errmsg);
            if ( stat )
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        "Pressure ADC calibration complete",
                        "Thanks" };
            }
            else
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        "Could not complete calibration",
                        errmsg };

            }
        }

        private void ButtonCalibEntryVolts_Click(object sender, EventArgs e)
        {
            if (!IsTesterComOpen)

            {
                MessageBox.Show("Open tester MODBUS communication first");
                return;
            }
            if (!IsGUIComOpen)
            {
                MessageBox.Show("Open GUI communication first");
                return;
            }

            bool stat = CalibTPAdcInput(out string errmsg);
            if (stat)
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        "SSR voltage measurement calibration complete",
                        "Thanks" };
            }
            else
            {
                textBoxMessageToHumanity.Lines = new string[] {
                        "Could not complete calibration",
                        errmsg };

            }

        }
    }

    /* 
     * CTestIdentifier: Identifier for a single test
     */
    public class CTestIdentifier
    {
        public Func<CTestIdentifier, bool> TestFunc; // The function that runs the test 
        public CTestInfo TestInfo;  // The test information 
        public List<string> ErrMsg; // List of error messages accumulated in the test 
        public int  RowIndex ; // Location of result in the ATR 
        public string  ExcelFileName = null ; // Name of the ATR file 
        public string TempFolder = null; // Folder to store temporary results

        /* 
         * CTestIdentifier: Consruct Identifier for a single test
         * _TestFunc: Function that performs the test 
         * _TestIdentifier: The group identifier in the ATR form, should be numbers separated by dots, something like 2.3 or 1.4.6.7
         * Sheet: The name of sheet to look for the test in the ATR form 
         * Header: Test group header in the ATR form 
         */
        public CTestIdentifier(Func<CTestIdentifier, bool> _TestFunc, string _TestIdentifier, string Sheet = "Unspecified Sheet", string Header = "Unspecified Test")
        {
            ErrMsg = new List<string>();
            TestFunc = _TestFunc;
            TestInfo = new CTestInfo
            {
                TestIdentifier = _TestIdentifier,
                TestHeader = Header,
                TestSheet = Sheet
            };
        }
    } 

    class CComStoreDAta
    {
        public string COM_TESTER { get; set; }
        public string COM_HOST { get; set; }
        public string COM_GUI { get; set; }
    };
}
