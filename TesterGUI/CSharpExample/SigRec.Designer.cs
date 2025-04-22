
namespace TesterGUI
{
    partial class SigRec
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
            this.dataGridViewSigRec = new System.Windows.Forms.DataGridView();
            this.dataGridViewSelected = new System.Windows.Forms.DataGridView();
            this.numericUpDownMaxPoints = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTimeSpan = new System.Windows.Forms.NumericUpDown();
            this.labelMaxDesiredPts = new System.Windows.Forms.Label();
            this.labelMaxAvailPoints = new System.Windows.Forms.Label();
            this.TimeSpan = new System.Windows.Forms.Label();
            this.numericUpDownTriggerLevel = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownTriggerPercent = new System.Windows.Forms.NumericUpDown();
            this.comboBoxTriggerMethod = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.checkBoxRecorderReady = new System.Windows.Forms.CheckBox();
            this.buttonBringRecorderData = new System.Windows.Forms.Button();
            this.labelFilePath = new System.Windows.Forms.Label();
            this.buttonSelectRecorderRsltFolder = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRecFileName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxRecorderArmed = new System.Windows.Forms.CheckBox();
            this.progressBarUpload = new System.Windows.Forms.ProgressBar();
            this.checkBoxClearAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSigRec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSelected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeSpan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTriggerLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTriggerPercent)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewSigRec
            // 
            this.dataGridViewSigRec.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSigRec.Location = new System.Drawing.Point(50, 34);
            this.dataGridViewSigRec.Name = "dataGridViewSigRec";
            this.dataGridViewSigRec.RowHeadersWidth = 51;
            this.dataGridViewSigRec.RowTemplate.Height = 24;
            this.dataGridViewSigRec.Size = new System.Drawing.Size(682, 488);
            this.dataGridViewSigRec.TabIndex = 0;
            this.dataGridViewSigRec.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SigCellClick);
            // 
            // dataGridViewSelected
            // 
            this.dataGridViewSelected.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSelected.Location = new System.Drawing.Point(757, 34);
            this.dataGridViewSelected.Name = "dataGridViewSelected";
            this.dataGridViewSelected.RowHeadersWidth = 51;
            this.dataGridViewSelected.RowTemplate.Height = 24;
            this.dataGridViewSelected.Size = new System.Drawing.Size(325, 488);
            this.dataGridViewSelected.TabIndex = 1;
            this.dataGridViewSelected.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ListCellClick);
            // 
            // numericUpDownMaxPoints
            // 
            this.numericUpDownMaxPoints.Location = new System.Drawing.Point(1126, 81);
            this.numericUpDownMaxPoints.Name = "numericUpDownMaxPoints";
            this.numericUpDownMaxPoints.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownMaxPoints.TabIndex = 2;
            // 
            // numericUpDownTimeSpan
            // 
            this.numericUpDownTimeSpan.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numericUpDownTimeSpan.Location = new System.Drawing.Point(1126, 142);
            this.numericUpDownTimeSpan.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTimeSpan.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numericUpDownTimeSpan.Name = "numericUpDownTimeSpan";
            this.numericUpDownTimeSpan.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownTimeSpan.TabIndex = 3;
            this.numericUpDownTimeSpan.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // labelMaxDesiredPts
            // 
            this.labelMaxDesiredPts.AutoSize = true;
            this.labelMaxDesiredPts.Location = new System.Drawing.Point(1123, 61);
            this.labelMaxDesiredPts.Name = "labelMaxDesiredPts";
            this.labelMaxDesiredPts.Size = new System.Drawing.Size(130, 17);
            this.labelMaxDesiredPts.TabIndex = 4;
            this.labelMaxDesiredPts.Text = "Max. desired points";
            // 
            // labelMaxAvailPoints
            // 
            this.labelMaxAvailPoints.AutoSize = true;
            this.labelMaxAvailPoints.Location = new System.Drawing.Point(1102, 34);
            this.labelMaxAvailPoints.Name = "labelMaxAvailPoints";
            this.labelMaxAvailPoints.Size = new System.Drawing.Size(168, 17);
            this.labelMaxAvailPoints.TabIndex = 5;
            this.labelMaxAvailPoints.Text = "Maximum available points";
            // 
            // TimeSpan
            // 
            this.TimeSpan.AutoSize = true;
            this.TimeSpan.Location = new System.Drawing.Point(1123, 122);
            this.TimeSpan.Name = "TimeSpan";
            this.TimeSpan.Size = new System.Drawing.Size(74, 17);
            this.TimeSpan.TabIndex = 6;
            this.TimeSpan.Text = "Time span";
            // 
            // numericUpDownTriggerLevel
            // 
            this.numericUpDownTriggerLevel.Location = new System.Drawing.Point(1126, 208);
            this.numericUpDownTriggerLevel.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownTriggerLevel.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numericUpDownTriggerLevel.Name = "numericUpDownTriggerLevel";
            this.numericUpDownTriggerLevel.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownTriggerLevel.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1123, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Trigger level";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1123, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Trigger percents";
            // 
            // numericUpDownTriggerPercent
            // 
            this.numericUpDownTriggerPercent.Location = new System.Drawing.Point(1126, 277);
            this.numericUpDownTriggerPercent.Name = "numericUpDownTriggerPercent";
            this.numericUpDownTriggerPercent.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownTriggerPercent.TabIndex = 10;
            // 
            // comboBoxTriggerMethod
            // 
            this.comboBoxTriggerMethod.FormattingEnabled = true;
            this.comboBoxTriggerMethod.Items.AddRange(new object[] {
            "Immediate",
            "Up",
            "Down",
            "Equal"});
            this.comboBoxTriggerMethod.Location = new System.Drawing.Point(1125, 332);
            this.comboBoxTriggerMethod.Name = "comboBoxTriggerMethod";
            this.comboBoxTriggerMethod.Size = new System.Drawing.Size(121, 24);
            this.comboBoxTriggerMethod.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1123, 312);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Trigger method";
            // 
            // buttonRecord
            // 
            this.buttonRecord.Location = new System.Drawing.Point(1126, 381);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(119, 36);
            this.buttonRecord.TabIndex = 13;
            this.buttonRecord.Text = "Record";
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.Click_DoRecord);
            // 
            // checkBoxRecorderReady
            // 
            this.checkBoxRecorderReady.AutoSize = true;
            this.checkBoxRecorderReady.Location = new System.Drawing.Point(1119, 450);
            this.checkBoxRecorderReady.Name = "checkBoxRecorderReady";
            this.checkBoxRecorderReady.Size = new System.Drawing.Size(134, 21);
            this.checkBoxRecorderReady.TabIndex = 14;
            this.checkBoxRecorderReady.Text = "Recorder Ready";
            this.checkBoxRecorderReady.UseVisualStyleBackColor = true;
            // 
            // buttonBringRecorderData
            // 
            this.buttonBringRecorderData.Location = new System.Drawing.Point(1126, 486);
            this.buttonBringRecorderData.Name = "buttonBringRecorderData";
            this.buttonBringRecorderData.Size = new System.Drawing.Size(119, 36);
            this.buttonBringRecorderData.TabIndex = 15;
            this.buttonBringRecorderData.Text = "Bring data";
            this.buttonBringRecorderData.UseVisualStyleBackColor = true;
            this.buttonBringRecorderData.Click += new System.EventHandler(this.Click_BringRecorderData);
            // 
            // labelFilePath
            // 
            this.labelFilePath.AutoSize = true;
            this.labelFilePath.Location = new System.Drawing.Point(180, 566);
            this.labelFilePath.Name = "labelFilePath";
            this.labelFilePath.Size = new System.Drawing.Size(406, 17);
            this.labelFilePath.TabIndex = 16;
            this.labelFilePath.Text = "ThisLabelIsTheFullFilePathForRecordedVariables, Oh how nice";
            // 
            // buttonSelectRecorderRsltFolder
            // 
            this.buttonSelectRecorderRsltFolder.Location = new System.Drawing.Point(50, 547);
            this.buttonSelectRecorderRsltFolder.Name = "buttonSelectRecorderRsltFolder";
            this.buttonSelectRecorderRsltFolder.Size = new System.Drawing.Size(119, 36);
            this.buttonSelectRecorderRsltFolder.TabIndex = 17;
            this.buttonSelectRecorderRsltFolder.Text = "Select folder";
            this.buttonSelectRecorderRsltFolder.UseVisualStyleBackColor = true;
            this.buttonSelectRecorderRsltFolder.Click += new System.EventHandler(this.ButtonSelectRecorderRsltFolder_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 547);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Results folder";
            // 
            // textBoxRecFileName
            // 
            this.textBoxRecFileName.Location = new System.Drawing.Point(888, 561);
            this.textBoxRecFileName.Name = "textBoxRecFileName";
            this.textBoxRecFileName.Size = new System.Drawing.Size(365, 22);
            this.textBoxRecFileName.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(888, 541);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(371, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "Result file name (no extension here, will be .xlsx and .mat)";
            // 
            // checkBoxRecorderArmed
            // 
            this.checkBoxRecorderArmed.AutoSize = true;
            this.checkBoxRecorderArmed.Location = new System.Drawing.Point(1119, 423);
            this.checkBoxRecorderArmed.Name = "checkBoxRecorderArmed";
            this.checkBoxRecorderArmed.Size = new System.Drawing.Size(134, 21);
            this.checkBoxRecorderArmed.TabIndex = 21;
            this.checkBoxRecorderArmed.Text = "Recorder Armed";
            this.checkBoxRecorderArmed.UseVisualStyleBackColor = true;
            // 
            // progressBarUpload
            // 
            this.progressBarUpload.Location = new System.Drawing.Point(886, 589);
            this.progressBarUpload.Name = "progressBarUpload";
            this.progressBarUpload.Size = new System.Drawing.Size(367, 21);
            this.progressBarUpload.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarUpload.TabIndex = 22;
            // 
            // checkBoxClearAll
            // 
            this.checkBoxClearAll.AutoSize = true;
            this.checkBoxClearAll.Location = new System.Drawing.Point(52, 7);
            this.checkBoxClearAll.Name = "checkBoxClearAll";
            this.checkBoxClearAll.Size = new System.Drawing.Size(82, 21);
            this.checkBoxClearAll.TabIndex = 23;
            this.checkBoxClearAll.Text = "Clear All";
            this.checkBoxClearAll.UseVisualStyleBackColor = true;
            this.checkBoxClearAll.CheckedChanged += new System.EventHandler(this.Click_checkBoxClearAll);
            // 
            // SigRec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1299, 612);
            this.Controls.Add(this.checkBoxClearAll);
            this.Controls.Add(this.progressBarUpload);
            this.Controls.Add(this.checkBoxRecorderArmed);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxRecFileName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSelectRecorderRsltFolder);
            this.Controls.Add(this.labelFilePath);
            this.Controls.Add(this.buttonBringRecorderData);
            this.Controls.Add(this.checkBoxRecorderReady);
            this.Controls.Add(this.buttonRecord);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxTriggerMethod);
            this.Controls.Add(this.numericUpDownTriggerPercent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownTriggerLevel);
            this.Controls.Add(this.TimeSpan);
            this.Controls.Add(this.labelMaxAvailPoints);
            this.Controls.Add(this.labelMaxDesiredPts);
            this.Controls.Add(this.numericUpDownTimeSpan);
            this.Controls.Add(this.numericUpDownMaxPoints);
            this.Controls.Add(this.dataGridViewSelected);
            this.Controls.Add(this.dataGridViewSigRec);
            this.Name = "SigRec";
            this.Text = "SigRec";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSigRec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSelected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeSpan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTriggerLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTriggerPercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewSigRec;
        private System.Windows.Forms.DataGridView dataGridViewSelected;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxPoints;
        private System.Windows.Forms.NumericUpDown numericUpDownTimeSpan;
        private System.Windows.Forms.Label labelMaxDesiredPts;
        private System.Windows.Forms.Label labelMaxAvailPoints;
        private System.Windows.Forms.Label TimeSpan;
        private System.Windows.Forms.NumericUpDown numericUpDownTriggerLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownTriggerPercent;
        private System.Windows.Forms.ComboBox comboBoxTriggerMethod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.CheckBox checkBoxRecorderReady;
        private System.Windows.Forms.Button buttonBringRecorderData;
        private System.Windows.Forms.Label labelFilePath;
        private System.Windows.Forms.Button buttonSelectRecorderRsltFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxRecFileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxRecorderArmed;
        private System.Windows.Forms.ProgressBar progressBarUpload;
        private System.Windows.Forms.CheckBox checkBoxClearAll;
    }
}