namespace PvsGUI
{
    partial class CAtpMainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CAtpMainWindow));
            this.buttonSelectAtpFile = new System.Windows.Forms.Button();
            this.textBoxMessageToHumanity = new System.Windows.Forms.TextBox();
            this.buttonStartATP = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxIsolation = new System.Windows.Forms.CheckBox();
            this.checkBoxJ4Plug = new System.Windows.Forms.CheckBox();
            this.checkBoxVoltageAndCurrents = new System.Windows.Forms.CheckBox();
            this.checkBoxVoltageTestNoLoad = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkPowerOnConsumptionLED = new System.Windows.Forms.CheckBox();
            this.checkBoxPanelLEDs = new System.Windows.Forms.CheckBox();
            this.checkBoxInternalCommunicationFSI = new System.Windows.Forms.CheckBox();
            this.checkBoxHostCommunicationRs422 = new System.Windows.Forms.CheckBox();
            this.checkBoxInstallationAndVersions = new System.Windows.Forms.CheckBox();
            this.checkBoxSSRPowerOn = new System.Windows.Forms.CheckBox();
            this.checkBoxVoltageMeas = new System.Windows.Forms.CheckBox();
            this.checkBoxTemperatureMeas = new System.Windows.Forms.CheckBox();
            this.checkBoxCollerFans = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxPressureOutJ7 = new System.Windows.Forms.CheckBox();
            this.checkBoxCFGInputs = new System.Windows.Forms.CheckBox();
            this.checkBoxPressureAdcInput = new System.Windows.Forms.CheckBox();
            this.checkBoxSolenoidStateReport = new System.Windows.Forms.CheckBox();
            this.checkBoxSimulatorIDOutputs = new System.Windows.Forms.CheckBox();
            this.checkBoxSolenoidCurrentReport = new System.Windows.Forms.CheckBox();
            this.checkBoxPressureOutJ1 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxBootSwitch = new System.Windows.Forms.CheckBox();
            this.checkBoxReset = new System.Windows.Forms.CheckBox();
            this.checkBoxSerialFlash = new System.Windows.Forms.CheckBox();
            this.buttonCheckAll = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonSelectCalibFile = new System.Windows.Forms.Button();
            this.labelTesterCalibrationFile = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.LabelSIM_SW = new System.Windows.Forms.Label();
            this.LabelSimSWxxx = new System.Windows.Forms.Label();
            this.LabelLLCSw = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.LabelMainCpuSW = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkStopOnFail = new System.Windows.Forms.CheckBox();
            this.comboBoxPortsGUIATP = new System.Windows.Forms.ComboBox();
            this.comboBoxPortsHostATP = new System.Windows.Forms.ComboBox();
            this.comboBoxPortsTesterModbusATP = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Pafutzlachi = new System.Windows.Forms.Label();
            this.buttonConnectTester = new System.Windows.Forms.Button();
            this.pictureLEDConnectGUIATP = new System.Windows.Forms.PictureBox();
            this.pictureLEDConnectHostATP = new System.Windows.Forms.PictureBox();
            this.pictureLEDConnectTestATP = new System.Windows.Forms.PictureBox();
            this.buttonDisconnectAtp = new System.Windows.Forms.Button();
            this.buttonSetATRDir = new System.Windows.Forms.Button();
            this.labelAtrDirectoryPath = new System.Windows.Forms.Label();
            this.textBoxATRFileName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOpenATRForm = new System.Windows.Forms.Button();
            this.buttonememberPorts = new System.Windows.Forms.Button();
            this.ButtonReviseAtpFile = new System.Windows.Forms.Button();
            this.GroupBoxCalibration = new System.Windows.Forms.GroupBox();
            this.ButtonCalibEntryVolts = new System.Windows.Forms.Button();
            this.ButtonAnalogInCalib = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLEDConnectGUIATP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLEDConnectHostATP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLEDConnectTestATP)).BeginInit();
            this.GroupBoxCalibration.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectAtpFile
            // 
            this.buttonSelectAtpFile.Location = new System.Drawing.Point(25, 91);
            this.buttonSelectAtpFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonSelectAtpFile.Name = "buttonSelectAtpFile";
            this.buttonSelectAtpFile.Size = new System.Drawing.Size(144, 26);
            this.buttonSelectAtpFile.TabIndex = 1;
            this.buttonSelectAtpFile.Text = "Select/Resume";
            this.buttonSelectAtpFile.UseVisualStyleBackColor = true;
            this.buttonSelectAtpFile.Click += new System.EventHandler(this.ButtonSelectAtpFile_Click);
            // 
            // textBoxMessageToHumanity
            // 
            this.textBoxMessageToHumanity.Location = new System.Drawing.Point(39, 487);
            this.textBoxMessageToHumanity.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxMessageToHumanity.Multiline = true;
            this.textBoxMessageToHumanity.Name = "textBoxMessageToHumanity";
            this.textBoxMessageToHumanity.Size = new System.Drawing.Size(553, 75);
            this.textBoxMessageToHumanity.TabIndex = 2;
            // 
            // buttonStartATP
            // 
            this.buttonStartATP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.buttonStartATP.Location = new System.Drawing.Point(32, 340);
            this.buttonStartATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonStartATP.Name = "buttonStartATP";
            this.buttonStartATP.Size = new System.Drawing.Size(120, 26);
            this.buttonStartATP.TabIndex = 6;
            this.buttonStartATP.Text = "Start ATP";
            this.buttonStartATP.UseVisualStyleBackColor = false;
            this.buttonStartATP.Click += new System.EventHandler(this.Click_StartATP);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 473);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Message to humanity";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxIsolation);
            this.groupBox4.Controls.Add(this.checkBoxJ4Plug);
            this.groupBox4.Controls.Add(this.checkBoxVoltageAndCurrents);
            this.groupBox4.Controls.Add(this.checkBoxVoltageTestNoLoad);
            this.groupBox4.Location = new System.Drawing.Point(430, 348);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(161, 128);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Power unit";
            // 
            // checkBoxIsolation
            // 
            this.checkBoxIsolation.AutoSize = true;
            this.checkBoxIsolation.Location = new System.Drawing.Point(19, 78);
            this.checkBoxIsolation.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxIsolation.Name = "checkBoxIsolation";
            this.checkBoxIsolation.Size = new System.Drawing.Size(65, 17);
            this.checkBoxIsolation.TabIndex = 3;
            this.checkBoxIsolation.Text = "Isolation";
            this.checkBoxIsolation.UseVisualStyleBackColor = true;
            // 
            // checkBoxJ4Plug
            // 
            this.checkBoxJ4Plug.AutoSize = true;
            this.checkBoxJ4Plug.Location = new System.Drawing.Point(19, 57);
            this.checkBoxJ4Plug.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxJ4Plug.Name = "checkBoxJ4Plug";
            this.checkBoxJ4Plug.Size = new System.Drawing.Size(61, 17);
            this.checkBoxJ4Plug.TabIndex = 2;
            this.checkBoxJ4Plug.Text = "J4 Plug";
            this.checkBoxJ4Plug.UseVisualStyleBackColor = true;
            // 
            // checkBoxVoltageAndCurrents
            // 
            this.checkBoxVoltageAndCurrents.AutoSize = true;
            this.checkBoxVoltageAndCurrents.Location = new System.Drawing.Point(19, 37);
            this.checkBoxVoltageAndCurrents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxVoltageAndCurrents.Name = "checkBoxVoltageAndCurrents";
            this.checkBoxVoltageAndCurrents.Size = new System.Drawing.Size(115, 17);
            this.checkBoxVoltageAndCurrents.TabIndex = 1;
            this.checkBoxVoltageAndCurrents.Text = "VoltageAndCurrent";
            this.checkBoxVoltageAndCurrents.UseVisualStyleBackColor = true;
            // 
            // checkBoxVoltageTestNoLoad
            // 
            this.checkBoxVoltageTestNoLoad.AutoSize = true;
            this.checkBoxVoltageTestNoLoad.Location = new System.Drawing.Point(19, 18);
            this.checkBoxVoltageTestNoLoad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxVoltageTestNoLoad.Name = "checkBoxVoltageTestNoLoad";
            this.checkBoxVoltageTestNoLoad.Size = new System.Drawing.Size(128, 17);
            this.checkBoxVoltageTestNoLoad.TabIndex = 0;
            this.checkBoxVoltageTestNoLoad.Text = "Voltage Test NO load";
            this.checkBoxVoltageTestNoLoad.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkPowerOnConsumptionLED);
            this.groupBox3.Controls.Add(this.checkBoxPanelLEDs);
            this.groupBox3.Controls.Add(this.checkBoxInternalCommunicationFSI);
            this.groupBox3.Controls.Add(this.checkBoxHostCommunicationRs422);
            this.groupBox3.Controls.Add(this.checkBoxInstallationAndVersions);
            this.groupBox3.Controls.Add(this.checkBoxSSRPowerOn);
            this.groupBox3.Controls.Add(this.checkBoxVoltageMeas);
            this.groupBox3.Controls.Add(this.checkBoxTemperatureMeas);
            this.groupBox3.Controls.Add(this.checkBoxCollerFans);
            this.groupBox3.Location = new System.Drawing.Point(199, 163);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(198, 219);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Drawer level";
            // 
            // checkPowerOnConsumptionLED
            // 
            this.checkPowerOnConsumptionLED.AutoSize = true;
            this.checkPowerOnConsumptionLED.Location = new System.Drawing.Point(19, 19);
            this.checkPowerOnConsumptionLED.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkPowerOnConsumptionLED.Name = "checkPowerOnConsumptionLED";
            this.checkPowerOnConsumptionLED.Size = new System.Drawing.Size(179, 17);
            this.checkPowerOnConsumptionLED.TabIndex = 11;
            this.checkPowerOnConsumptionLED.Text = "PowerOn Consumption and LED";
            this.checkPowerOnConsumptionLED.UseVisualStyleBackColor = true;
            // 
            // checkBoxPanelLEDs
            // 
            this.checkBoxPanelLEDs.AutoSize = true;
            this.checkBoxPanelLEDs.Location = new System.Drawing.Point(19, 197);
            this.checkBoxPanelLEDs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxPanelLEDs.Name = "checkBoxPanelLEDs";
            this.checkBoxPanelLEDs.Size = new System.Drawing.Size(126, 17);
            this.checkBoxPanelLEDs.TabIndex = 10;
            this.checkBoxPanelLEDs.Text = "LEDs and test button";
            this.checkBoxPanelLEDs.UseVisualStyleBackColor = true;
            // 
            // checkBoxInternalCommunicationFSI
            // 
            this.checkBoxInternalCommunicationFSI.AutoSize = true;
            this.checkBoxInternalCommunicationFSI.Location = new System.Drawing.Point(19, 84);
            this.checkBoxInternalCommunicationFSI.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxInternalCommunicationFSI.Name = "checkBoxInternalCommunicationFSI";
            this.checkBoxInternalCommunicationFSI.Size = new System.Drawing.Size(154, 17);
            this.checkBoxInternalCommunicationFSI.TabIndex = 9;
            this.checkBoxInternalCommunicationFSI.Text = "Internal communication FSI";
            this.checkBoxInternalCommunicationFSI.UseVisualStyleBackColor = true;
            // 
            // checkBoxHostCommunicationRs422
            // 
            this.checkBoxHostCommunicationRs422.AutoSize = true;
            this.checkBoxHostCommunicationRs422.Checked = true;
            this.checkBoxHostCommunicationRs422.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHostCommunicationRs422.Location = new System.Drawing.Point(19, 63);
            this.checkBoxHostCommunicationRs422.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxHostCommunicationRs422.Name = "checkBoxHostCommunicationRs422";
            this.checkBoxHostCommunicationRs422.Size = new System.Drawing.Size(158, 17);
            this.checkBoxHostCommunicationRs422.TabIndex = 8;
            this.checkBoxHostCommunicationRs422.Text = "Host communication RS422";
            this.checkBoxHostCommunicationRs422.UseVisualStyleBackColor = true;
            // 
            // checkBoxInstallationAndVersions
            // 
            this.checkBoxInstallationAndVersions.AutoSize = true;
            this.checkBoxInstallationAndVersions.Location = new System.Drawing.Point(19, 41);
            this.checkBoxInstallationAndVersions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxInstallationAndVersions.Name = "checkBoxInstallationAndVersions";
            this.checkBoxInstallationAndVersions.Size = new System.Drawing.Size(139, 17);
            this.checkBoxInstallationAndVersions.TabIndex = 7;
            this.checkBoxInstallationAndVersions.Text = "Installation and versions";
            this.checkBoxInstallationAndVersions.UseVisualStyleBackColor = true;
            // 
            // checkBoxSSRPowerOn
            // 
            this.checkBoxSSRPowerOn.AutoSize = true;
            this.checkBoxSSRPowerOn.Location = new System.Drawing.Point(19, 176);
            this.checkBoxSSRPowerOn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxSSRPowerOn.Name = "checkBoxSSRPowerOn";
            this.checkBoxSSRPowerOn.Size = new System.Drawing.Size(121, 17);
            this.checkBoxSSRPowerOn.TabIndex = 6;
            this.checkBoxSSRPowerOn.Text = "SSR and Power ON";
            this.checkBoxSSRPowerOn.UseVisualStyleBackColor = true;
            // 
            // checkBoxVoltageMeas
            // 
            this.checkBoxVoltageMeas.AutoSize = true;
            this.checkBoxVoltageMeas.Location = new System.Drawing.Point(19, 153);
            this.checkBoxVoltageMeas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxVoltageMeas.Name = "checkBoxVoltageMeas";
            this.checkBoxVoltageMeas.Size = new System.Drawing.Size(148, 17);
            this.checkBoxVoltageMeas.TabIndex = 3;
            this.checkBoxVoltageMeas.Text = "BIT Voltage measurement";
            this.checkBoxVoltageMeas.UseVisualStyleBackColor = true;
            // 
            // checkBoxTemperatureMeas
            // 
            this.checkBoxTemperatureMeas.AutoSize = true;
            this.checkBoxTemperatureMeas.Location = new System.Drawing.Point(19, 131);
            this.checkBoxTemperatureMeas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxTemperatureMeas.Name = "checkBoxTemperatureMeas";
            this.checkBoxTemperatureMeas.Size = new System.Drawing.Size(152, 17);
            this.checkBoxTemperatureMeas.TabIndex = 2;
            this.checkBoxTemperatureMeas.Text = "Temperature measurement";
            this.checkBoxTemperatureMeas.UseVisualStyleBackColor = true;
            // 
            // checkBoxCollerFans
            // 
            this.checkBoxCollerFans.AutoSize = true;
            this.checkBoxCollerFans.Location = new System.Drawing.Point(19, 106);
            this.checkBoxCollerFans.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxCollerFans.Name = "checkBoxCollerFans";
            this.checkBoxCollerFans.Size = new System.Drawing.Size(79, 17);
            this.checkBoxCollerFans.TabIndex = 1;
            this.checkBoxCollerFans.Text = "Cooler fans";
            this.checkBoxCollerFans.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxPressureOutJ7);
            this.groupBox2.Controls.Add(this.checkBoxCFGInputs);
            this.groupBox2.Controls.Add(this.checkBoxPressureAdcInput);
            this.groupBox2.Controls.Add(this.checkBoxSolenoidStateReport);
            this.groupBox2.Controls.Add(this.checkBoxSimulatorIDOutputs);
            this.groupBox2.Controls.Add(this.checkBoxSolenoidCurrentReport);
            this.groupBox2.Controls.Add(this.checkBoxPressureOutJ1);
            this.groupBox2.Location = new System.Drawing.Point(430, 163);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(161, 181);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Panel controls";
            // 
            // checkBoxPressureOutJ7
            // 
            this.checkBoxPressureOutJ7.AutoSize = true;
            this.checkBoxPressureOutJ7.Location = new System.Drawing.Point(19, 72);
            this.checkBoxPressureOutJ7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxPressureOutJ7.Name = "checkBoxPressureOutJ7";
            this.checkBoxPressureOutJ7.Size = new System.Drawing.Size(114, 17);
            this.checkBoxPressureOutJ7.TabIndex = 19;
            this.checkBoxPressureOutJ7.Text = "Pressure output J7";
            this.checkBoxPressureOutJ7.UseVisualStyleBackColor = true;
            // 
            // checkBoxCFGInputs
            // 
            this.checkBoxCFGInputs.AutoSize = true;
            this.checkBoxCFGInputs.Location = new System.Drawing.Point(19, 37);
            this.checkBoxCFGInputs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxCFGInputs.Name = "checkBoxCFGInputs";
            this.checkBoxCFGInputs.Size = new System.Drawing.Size(78, 17);
            this.checkBoxCFGInputs.TabIndex = 6;
            this.checkBoxCFGInputs.Text = "CFG inputs";
            this.checkBoxCFGInputs.UseVisualStyleBackColor = true;
            // 
            // checkBoxPressureAdcInput
            // 
            this.checkBoxPressureAdcInput.AutoSize = true;
            this.checkBoxPressureAdcInput.Location = new System.Drawing.Point(19, 18);
            this.checkBoxPressureAdcInput.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxPressureAdcInput.Name = "checkBoxPressureAdcInput";
            this.checkBoxPressureAdcInput.Size = new System.Drawing.Size(123, 17);
            this.checkBoxPressureAdcInput.TabIndex = 0;
            this.checkBoxPressureAdcInput.Text = "Pressure ADC inputs";
            this.checkBoxPressureAdcInput.UseVisualStyleBackColor = true;
            // 
            // checkBoxSolenoidStateReport
            // 
            this.checkBoxSolenoidStateReport.AutoSize = true;
            this.checkBoxSolenoidStateReport.Location = new System.Drawing.Point(19, 111);
            this.checkBoxSolenoidStateReport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxSolenoidStateReport.Name = "checkBoxSolenoidStateReport";
            this.checkBoxSolenoidStateReport.Size = new System.Drawing.Size(125, 17);
            this.checkBoxSolenoidStateReport.TabIndex = 4;
            this.checkBoxSolenoidStateReport.Text = "Solenoid State report";
            this.checkBoxSolenoidStateReport.UseVisualStyleBackColor = true;
            // 
            // checkBoxSimulatorIDOutputs
            // 
            this.checkBoxSimulatorIDOutputs.AutoSize = true;
            this.checkBoxSimulatorIDOutputs.Location = new System.Drawing.Point(19, 131);
            this.checkBoxSimulatorIDOutputs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxSimulatorIDOutputs.Name = "checkBoxSimulatorIDOutputs";
            this.checkBoxSimulatorIDOutputs.Size = new System.Drawing.Size(121, 17);
            this.checkBoxSimulatorIDOutputs.TabIndex = 3;
            this.checkBoxSimulatorIDOutputs.Text = "Simulator ID outputs";
            this.checkBoxSimulatorIDOutputs.UseVisualStyleBackColor = true;
            // 
            // checkBoxSolenoidCurrentReport
            // 
            this.checkBoxSolenoidCurrentReport.AutoSize = true;
            this.checkBoxSolenoidCurrentReport.Location = new System.Drawing.Point(19, 92);
            this.checkBoxSolenoidCurrentReport.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxSolenoidCurrentReport.Name = "checkBoxSolenoidCurrentReport";
            this.checkBoxSolenoidCurrentReport.Size = new System.Drawing.Size(133, 17);
            this.checkBoxSolenoidCurrentReport.TabIndex = 2;
            this.checkBoxSolenoidCurrentReport.Text = "Solenoid current report";
            this.checkBoxSolenoidCurrentReport.UseVisualStyleBackColor = true;
            // 
            // checkBoxPressureOutJ1
            // 
            this.checkBoxPressureOutJ1.AutoSize = true;
            this.checkBoxPressureOutJ1.Location = new System.Drawing.Point(19, 54);
            this.checkBoxPressureOutJ1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxPressureOutJ1.Name = "checkBoxPressureOutJ1";
            this.checkBoxPressureOutJ1.Size = new System.Drawing.Size(114, 17);
            this.checkBoxPressureOutJ1.TabIndex = 1;
            this.checkBoxPressureOutJ1.Text = "Pressure output J1";
            this.checkBoxPressureOutJ1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxBootSwitch);
            this.groupBox1.Controls.Add(this.checkBoxReset);
            this.groupBox1.Controls.Add(this.checkBoxSerialFlash);
            this.groupBox1.Location = new System.Drawing.Point(199, 381);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(115, 95);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Main CPU";
            // 
            // checkBoxBootSwitch
            // 
            this.checkBoxBootSwitch.AutoSize = true;
            this.checkBoxBootSwitch.Location = new System.Drawing.Point(19, 71);
            this.checkBoxBootSwitch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxBootSwitch.Name = "checkBoxBootSwitch";
            this.checkBoxBootSwitch.Size = new System.Drawing.Size(81, 17);
            this.checkBoxBootSwitch.TabIndex = 3;
            this.checkBoxBootSwitch.Text = "Boot switch";
            this.checkBoxBootSwitch.UseVisualStyleBackColor = true;
            // 
            // checkBoxReset
            // 
            this.checkBoxReset.AutoSize = true;
            this.checkBoxReset.Location = new System.Drawing.Point(19, 50);
            this.checkBoxReset.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxReset.Name = "checkBoxReset";
            this.checkBoxReset.Size = new System.Drawing.Size(54, 17);
            this.checkBoxReset.TabIndex = 2;
            this.checkBoxReset.Text = "Reset";
            this.checkBoxReset.UseVisualStyleBackColor = true;
            // 
            // checkBoxSerialFlash
            // 
            this.checkBoxSerialFlash.AutoSize = true;
            this.checkBoxSerialFlash.Location = new System.Drawing.Point(19, 29);
            this.checkBoxSerialFlash.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxSerialFlash.Name = "checkBoxSerialFlash";
            this.checkBoxSerialFlash.Size = new System.Drawing.Size(74, 17);
            this.checkBoxSerialFlash.TabIndex = 1;
            this.checkBoxSerialFlash.Text = "Seial flash";
            this.checkBoxSerialFlash.UseVisualStyleBackColor = true;
            // 
            // buttonCheckAll
            // 
            this.buttonCheckAll.Location = new System.Drawing.Point(118, 179);
            this.buttonCheckAll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCheckAll.Name = "buttonCheckAll";
            this.buttonCheckAll.Size = new System.Drawing.Size(76, 19);
            this.buttonCheckAll.TabIndex = 15;
            this.buttonCheckAll.Text = "Chack all";
            this.buttonCheckAll.UseVisualStyleBackColor = true;
            this.buttonCheckAll.Click += new System.EventHandler(this.Click_CheckAll);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(116, 204);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 19);
            this.button2.TabIndex = 16;
            this.button2.Text = "Uncheck All";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Click_UncheckAll);
            // 
            // buttonSelectCalibFile
            // 
            this.buttonSelectCalibFile.Location = new System.Drawing.Point(23, 133);
            this.buttonSelectCalibFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonSelectCalibFile.Name = "buttonSelectCalibFile";
            this.buttonSelectCalibFile.Size = new System.Drawing.Size(145, 26);
            this.buttonSelectCalibFile.TabIndex = 17;
            this.buttonSelectCalibFile.Text = "Select Tester Calibration file";
            this.buttonSelectCalibFile.UseVisualStyleBackColor = true;
            this.buttonSelectCalibFile.Click += new System.EventHandler(this.Click_SelectCalibFile);
            // 
            // labelTesterCalibrationFile
            // 
            this.labelTesterCalibrationFile.AutoSize = true;
            this.labelTesterCalibrationFile.Location = new System.Drawing.Point(176, 140);
            this.labelTesterCalibrationFile.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTesterCalibrationFile.Name = "labelTesterCalibrationFile";
            this.labelTesterCalibrationFile.Size = new System.Drawing.Size(198, 13);
            this.labelTesterCalibrationFile.TabIndex = 18;
            this.labelTesterCalibrationFile.Text = "Full file path for ATP calibration Excel file";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.LabelSIM_SW);
            this.groupBox5.Controls.Add(this.LabelSimSWxxx);
            this.groupBox5.Controls.Add(this.LabelLLCSw);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.LabelMainCpuSW);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(609, 163);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Size = new System.Drawing.Size(161, 293);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Installation";
            // 
            // LabelSIM_SW
            // 
            this.LabelSIM_SW.AutoSize = true;
            this.LabelSIM_SW.Location = new System.Drawing.Point(8, 268);
            this.LabelSIM_SW.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSIM_SW.Name = "LabelSIM_SW";
            this.LabelSIM_SW.Size = new System.Drawing.Size(54, 13);
            this.LabelSIM_SW.TabIndex = 26;
            this.LabelSIM_SW.Text = "#SIM SW";
            // 
            // LabelSimSWxxx
            // 
            this.LabelSimSWxxx.AutoSize = true;
            this.LabelSimSWxxx.Location = new System.Drawing.Point(8, 250);
            this.LabelSimSWxxx.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSimSWxxx.Name = "LabelSimSWxxx";
            this.LabelSimSWxxx.Size = new System.Drawing.Size(61, 13);
            this.LabelSimSWxxx.TabIndex = 25;
            this.LabelSimSWxxx.Text = "SIM SW ID";
            // 
            // LabelLLCSw
            // 
            this.LabelLLCSw.AutoSize = true;
            this.LabelLLCSw.Location = new System.Drawing.Point(8, 233);
            this.LabelLLCSw.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelLLCSw.Name = "LabelLLCSw";
            this.LabelLLCSw.Size = new System.Drawing.Size(54, 13);
            this.LabelLLCSw.TabIndex = 24;
            this.LabelLLCSw.Text = "#LLC SW";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 215);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "LLC SW ID";
            // 
            // LabelMainCpuSW
            // 
            this.LabelMainCpuSW.AutoSize = true;
            this.LabelMainCpuSW.Location = new System.Drawing.Point(8, 197);
            this.LabelMainCpuSW.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelMainCpuSW.Name = "LabelMainCpuSW";
            this.LabelMainCpuSW.Size = new System.Drawing.Size(83, 13);
            this.LabelMainCpuSW.TabIndex = 22;
            this.LabelMainCpuSW.Text = "#Main CPU SW";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 180);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Main CPU SW ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Auto update on connect";
            // 
            // checkStopOnFail
            // 
            this.checkStopOnFail.AutoSize = true;
            this.checkStopOnFail.Location = new System.Drawing.Point(32, 320);
            this.checkStopOnFail.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkStopOnFail.Name = "checkStopOnFail";
            this.checkStopOnFail.Size = new System.Drawing.Size(94, 17);
            this.checkStopOnFail.TabIndex = 20;
            this.checkStopOnFail.Text = "Stop on failure";
            this.checkStopOnFail.UseVisualStyleBackColor = true;
            // 
            // comboBoxPortsGUIATP
            // 
            this.comboBoxPortsGUIATP.FormattingEnabled = true;
            this.comboBoxPortsGUIATP.Location = new System.Drawing.Point(531, 122);
            this.comboBoxPortsGUIATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxPortsGUIATP.Name = "comboBoxPortsGUIATP";
            this.comboBoxPortsGUIATP.Size = new System.Drawing.Size(145, 21);
            this.comboBoxPortsGUIATP.TabIndex = 21;
            this.comboBoxPortsGUIATP.DropDown += new System.EventHandler(this.DropDownGUIPort);
            this.comboBoxPortsGUIATP.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPortsGUIATP_SelectedIndexChanged);
            this.comboBoxPortsGUIATP.Enter += new System.EventHandler(this.EnterGUIPort);
            // 
            // comboBoxPortsHostATP
            // 
            this.comboBoxPortsHostATP.FormattingEnabled = true;
            this.comboBoxPortsHostATP.Location = new System.Drawing.Point(531, 76);
            this.comboBoxPortsHostATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxPortsHostATP.Name = "comboBoxPortsHostATP";
            this.comboBoxPortsHostATP.Size = new System.Drawing.Size(145, 21);
            this.comboBoxPortsHostATP.TabIndex = 22;
            this.comboBoxPortsHostATP.DropDown += new System.EventHandler(this.DropDownHostPort);
            this.comboBoxPortsHostATP.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPortsHostATP_SelectedIndexChanged);
            this.comboBoxPortsHostATP.Enter += new System.EventHandler(this.EnterHostPort);
            // 
            // comboBoxPortsTesterModbusATP
            // 
            this.comboBoxPortsTesterModbusATP.FormattingEnabled = true;
            this.comboBoxPortsTesterModbusATP.Location = new System.Drawing.Point(530, 35);
            this.comboBoxPortsTesterModbusATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxPortsTesterModbusATP.Name = "comboBoxPortsTesterModbusATP";
            this.comboBoxPortsTesterModbusATP.Size = new System.Drawing.Size(146, 21);
            this.comboBoxPortsTesterModbusATP.TabIndex = 23;
            this.comboBoxPortsTesterModbusATP.DropDown += new System.EventHandler(this.DropDown_TesterPort);
            this.comboBoxPortsTesterModbusATP.SelectedIndexChanged += new System.EventHandler(this.ComboBoxPortsTesterModbusATP_SelectedIndexChanged);
            this.comboBoxPortsTesterModbusATP.Enter += new System.EventHandler(this.Enter_TesterPort);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(551, 102);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "GUI COM port ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(555, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Host COM port ";
            // 
            // Pafutzlachi
            // 
            this.Pafutzlachi.AutoSize = true;
            this.Pafutzlachi.Location = new System.Drawing.Point(555, 14);
            this.Pafutzlachi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Pafutzlachi.Name = "Pafutzlachi";
            this.Pafutzlachi.Size = new System.Drawing.Size(88, 13);
            this.Pafutzlachi.TabIndex = 26;
            this.Pafutzlachi.Text = "Tester COM port ";
            // 
            // buttonConnectTester
            // 
            this.buttonConnectTester.Location = new System.Drawing.Point(682, 32);
            this.buttonConnectTester.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonConnectTester.Name = "buttonConnectTester";
            this.buttonConnectTester.Size = new System.Drawing.Size(78, 24);
            this.buttonConnectTester.TabIndex = 27;
            this.buttonConnectTester.Text = "Connect";
            this.buttonConnectTester.UseVisualStyleBackColor = true;
            this.buttonConnectTester.Click += new System.EventHandler(this.Click_ConnectTester);
            // 
            // pictureLEDConnectGUIATP
            // 
            this.pictureLEDConnectGUIATP.Image = global::PvsGUI.Properties.Resources.GRAYLED;
            this.pictureLEDConnectGUIATP.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureLEDConnectGUIATP.InitialImage")));
            this.pictureLEDConnectGUIATP.Location = new System.Drawing.Point(530, 98);
            this.pictureLEDConnectGUIATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureLEDConnectGUIATP.Name = "pictureLEDConnectGUIATP";
            this.pictureLEDConnectGUIATP.Size = new System.Drawing.Size(22, 20);
            this.pictureLEDConnectGUIATP.TabIndex = 28;
            this.pictureLEDConnectGUIATP.TabStop = false;
            // 
            // pictureLEDConnectHostATP
            // 
            this.pictureLEDConnectHostATP.Image = global::PvsGUI.Properties.Resources.GRAYLED;
            this.pictureLEDConnectHostATP.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureLEDConnectHostATP.InitialImage")));
            this.pictureLEDConnectHostATP.Location = new System.Drawing.Point(529, 57);
            this.pictureLEDConnectHostATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureLEDConnectHostATP.Name = "pictureLEDConnectHostATP";
            this.pictureLEDConnectHostATP.Size = new System.Drawing.Size(22, 20);
            this.pictureLEDConnectHostATP.TabIndex = 29;
            this.pictureLEDConnectHostATP.TabStop = false;
            // 
            // pictureLEDConnectTestATP
            // 
            this.pictureLEDConnectTestATP.Image = global::PvsGUI.Properties.Resources.GRAYLED;
            this.pictureLEDConnectTestATP.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureLEDConnectTestATP.InitialImage")));
            this.pictureLEDConnectTestATP.Location = new System.Drawing.Point(530, 10);
            this.pictureLEDConnectTestATP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureLEDConnectTestATP.Name = "pictureLEDConnectTestATP";
            this.pictureLEDConnectTestATP.Size = new System.Drawing.Size(22, 20);
            this.pictureLEDConnectTestATP.TabIndex = 30;
            this.pictureLEDConnectTestATP.TabStop = false;
            // 
            // buttonDisconnectAtp
            // 
            this.buttonDisconnectAtp.Location = new System.Drawing.Point(682, 59);
            this.buttonDisconnectAtp.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDisconnectAtp.Name = "buttonDisconnectAtp";
            this.buttonDisconnectAtp.Size = new System.Drawing.Size(78, 24);
            this.buttonDisconnectAtp.TabIndex = 31;
            this.buttonDisconnectAtp.Text = "Disonnect";
            this.buttonDisconnectAtp.UseVisualStyleBackColor = true;
            this.buttonDisconnectAtp.Click += new System.EventHandler(this.Click_DisconnectAtp);
            // 
            // buttonSetATRDir
            // 
            this.buttonSetATRDir.Location = new System.Drawing.Point(24, 9);
            this.buttonSetATRDir.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonSetATRDir.Name = "buttonSetATRDir";
            this.buttonSetATRDir.Size = new System.Drawing.Size(145, 26);
            this.buttonSetATRDir.TabIndex = 32;
            this.buttonSetATRDir.Text = "Select ATR Directory";
            this.buttonSetATRDir.UseVisualStyleBackColor = true;
            this.buttonSetATRDir.Click += new System.EventHandler(this.Click_SetATRDir);
            // 
            // labelAtrDirectoryPath
            // 
            this.labelAtrDirectoryPath.AutoSize = true;
            this.labelAtrDirectoryPath.Location = new System.Drawing.Point(181, 16);
            this.labelAtrDirectoryPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAtrDirectoryPath.Name = "labelAtrDirectoryPath";
            this.labelAtrDirectoryPath.Size = new System.Drawing.Size(144, 13);
            this.labelAtrDirectoryPath.TabIndex = 33;
            this.labelAtrDirectoryPath.Text = "Full file path for ATR Dir Path";
            // 
            // textBoxATRFileName
            // 
            this.textBoxATRFileName.Location = new System.Drawing.Point(172, 51);
            this.textBoxATRFileName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxATRFileName.Name = "textBoxATRFileName";
            this.textBoxATRFileName.Size = new System.Drawing.Size(336, 20);
            this.textBoxATRFileName.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(172, 37);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "ATR file name";
            // 
            // buttonOpenATRForm
            // 
            this.buttonOpenATRForm.Location = new System.Drawing.Point(24, 35);
            this.buttonOpenATRForm.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOpenATRForm.Name = "buttonOpenATRForm";
            this.buttonOpenATRForm.Size = new System.Drawing.Size(144, 26);
            this.buttonOpenATRForm.TabIndex = 36;
            this.buttonOpenATRForm.TabStop = false;
            this.buttonOpenATRForm.Text = "Open/Generate new";
            this.buttonOpenATRForm.UseVisualStyleBackColor = true;
            this.buttonOpenATRForm.Click += new System.EventHandler(this.Click_OpenATRForm);
            // 
            // buttonememberPorts
            // 
            this.buttonememberPorts.Location = new System.Drawing.Point(682, 86);
            this.buttonememberPorts.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonememberPorts.Name = "buttonememberPorts";
            this.buttonememberPorts.Size = new System.Drawing.Size(78, 24);
            this.buttonememberPorts.TabIndex = 37;
            this.buttonememberPorts.Text = "Save COMs";
            this.buttonememberPorts.UseVisualStyleBackColor = true;
            this.buttonememberPorts.Click += new System.EventHandler(this.Click_SaveComs);
            // 
            // ButtonReviseAtpFile
            // 
            this.ButtonReviseAtpFile.Location = new System.Drawing.Point(24, 63);
            this.ButtonReviseAtpFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ButtonReviseAtpFile.Name = "ButtonReviseAtpFile";
            this.ButtonReviseAtpFile.Size = new System.Drawing.Size(144, 26);
            this.ButtonReviseAtpFile.TabIndex = 38;
            this.ButtonReviseAtpFile.Text = "Create revision";
            this.ButtonReviseAtpFile.UseVisualStyleBackColor = true;
            this.ButtonReviseAtpFile.Click += new System.EventHandler(this.Click_ReviseAtpFile);
            // 
            // GroupBoxCalibration
            // 
            this.GroupBoxCalibration.Controls.Add(this.ButtonCalibEntryVolts);
            this.GroupBoxCalibration.Controls.Add(this.ButtonAnalogInCalib);
            this.GroupBoxCalibration.Location = new System.Drawing.Point(784, 163);
            this.GroupBoxCalibration.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBoxCalibration.Name = "GroupBoxCalibration";
            this.GroupBoxCalibration.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBoxCalibration.Size = new System.Drawing.Size(148, 292);
            this.GroupBoxCalibration.TabIndex = 39;
            this.GroupBoxCalibration.TabStop = false;
            this.GroupBoxCalibration.Text = "Calibration";
            // 
            // ButtonCalibEntryVolts
            // 
            this.ButtonCalibEntryVolts.Location = new System.Drawing.Point(8, 67);
            this.ButtonCalibEntryVolts.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ButtonCalibEntryVolts.Name = "ButtonCalibEntryVolts";
            this.ButtonCalibEntryVolts.Size = new System.Drawing.Size(127, 24);
            this.ButtonCalibEntryVolts.TabIndex = 1;
            this.ButtonCalibEntryVolts.Text = "SSR Voltages";
            this.ButtonCalibEntryVolts.UseVisualStyleBackColor = true;
            this.ButtonCalibEntryVolts.Click += new System.EventHandler(this.ButtonCalibEntryVolts_Click);
            // 
            // ButtonAnalogInCalib
            // 
            this.ButtonAnalogInCalib.Location = new System.Drawing.Point(8, 32);
            this.ButtonAnalogInCalib.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ButtonAnalogInCalib.Name = "ButtonAnalogInCalib";
            this.ButtonAnalogInCalib.Size = new System.Drawing.Size(127, 24);
            this.ButtonAnalogInCalib.TabIndex = 0;
            this.ButtonAnalogInCalib.Text = "Analog inputs";
            this.ButtonAnalogInCalib.UseVisualStyleBackColor = true;
            this.ButtonAnalogInCalib.Click += new System.EventHandler(this.ButtonAnalogInCalib_Click);
            // 
            // CAtpMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 567);
            this.Controls.Add(this.GroupBoxCalibration);
            this.Controls.Add(this.ButtonReviseAtpFile);
            this.Controls.Add(this.buttonememberPorts);
            this.Controls.Add(this.buttonOpenATRForm);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxATRFileName);
            this.Controls.Add(this.labelAtrDirectoryPath);
            this.Controls.Add(this.buttonSetATRDir);
            this.Controls.Add(this.buttonDisconnectAtp);
            this.Controls.Add(this.pictureLEDConnectTestATP);
            this.Controls.Add(this.pictureLEDConnectHostATP);
            this.Controls.Add(this.pictureLEDConnectGUIATP);
            this.Controls.Add(this.buttonConnectTester);
            this.Controls.Add(this.Pafutzlachi);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxPortsTesterModbusATP);
            this.Controls.Add(this.comboBoxPortsHostATP);
            this.Controls.Add(this.comboBoxPortsGUIATP);
            this.Controls.Add(this.checkStopOnFail);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.labelTesterCalibrationFile);
            this.Controls.Add(this.buttonSelectCalibFile);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonCheckAll);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonStartATP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMessageToHumanity);
            this.Controls.Add(this.buttonSelectAtpFile);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CAtpMainWindow";
            this.Text = "ATP main window";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLEDConnectGUIATP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLEDConnectHostATP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLEDConnectTestATP)).EndInit();
            this.GroupBoxCalibration.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonSelectAtpFile;
        private System.Windows.Forms.TextBox textBoxMessageToHumanity;
        private System.Windows.Forms.Button buttonStartATP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxIsolation;
        private System.Windows.Forms.CheckBox checkBoxJ4Plug;
        private System.Windows.Forms.CheckBox checkBoxVoltageAndCurrents;
        private System.Windows.Forms.CheckBox checkBoxVoltageTestNoLoad;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxInternalCommunicationFSI;
        private System.Windows.Forms.CheckBox checkBoxHostCommunicationRs422;
        private System.Windows.Forms.CheckBox checkBoxInstallationAndVersions;
        private System.Windows.Forms.CheckBox checkBoxSSRPowerOn;
        private System.Windows.Forms.CheckBox checkBoxVoltageMeas;
        private System.Windows.Forms.CheckBox checkBoxTemperatureMeas;
        private System.Windows.Forms.CheckBox checkBoxCollerFans;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxSolenoidStateReport;
        private System.Windows.Forms.CheckBox checkBoxSimulatorIDOutputs;
        private System.Windows.Forms.CheckBox checkBoxSolenoidCurrentReport;
        private System.Windows.Forms.CheckBox checkBoxPressureOutJ1;
        private System.Windows.Forms.CheckBox checkBoxPressureAdcInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxBootSwitch;
        private System.Windows.Forms.CheckBox checkBoxReset;
        private System.Windows.Forms.CheckBox checkBoxSerialFlash;
        private System.Windows.Forms.Button buttonCheckAll;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonSelectCalibFile;
        private System.Windows.Forms.Label labelTesterCalibrationFile;
        private System.Windows.Forms.CheckBox checkBoxPanelLEDs;
        private System.Windows.Forms.CheckBox checkBoxCFGInputs;
        private System.Windows.Forms.CheckBox checkBoxPressureOutJ7;
        private System.Windows.Forms.CheckBox checkPowerOnConsumptionLED;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkStopOnFail;
        private System.Windows.Forms.ComboBox comboBoxPortsGUIATP;
        private System.Windows.Forms.ComboBox comboBoxPortsHostATP;
        private System.Windows.Forms.ComboBox comboBoxPortsTesterModbusATP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Pafutzlachi;
        private System.Windows.Forms.Button buttonConnectTester;
        private System.Windows.Forms.PictureBox pictureLEDConnectGUIATP;
        private System.Windows.Forms.PictureBox pictureLEDConnectHostATP;
        private System.Windows.Forms.PictureBox pictureLEDConnectTestATP;
        private System.Windows.Forms.Button buttonDisconnectAtp;
        private System.Windows.Forms.Button buttonSetATRDir;
        private System.Windows.Forms.Label labelAtrDirectoryPath;
        private System.Windows.Forms.TextBox textBoxATRFileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOpenATRForm;
        private System.Windows.Forms.Button buttonememberPorts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelSIM_SW;
        private System.Windows.Forms.Label LabelSimSWxxx;
        private System.Windows.Forms.Label LabelLLCSw;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label LabelMainCpuSW;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button ButtonReviseAtpFile;
        private System.Windows.Forms.GroupBox GroupBoxCalibration;
        private System.Windows.Forms.Button ButtonAnalogInCalib;
        private System.Windows.Forms.Button ButtonCalibEntryVolts;
    }
}