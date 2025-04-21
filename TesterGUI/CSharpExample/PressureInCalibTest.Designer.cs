namespace PvsGUI
{
    partial class CPressureInCalibTest
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
            this.buttonCalibNext = new System.Windows.Forms.Button();
            this.buttonCalibAbort = new System.Windows.Forms.Button();
            this.numericUpDownResult = new System.Windows.Forms.NumericUpDown();
            this.labelResults = new System.Windows.Forms.Label();
            this.ATPTest_textBoxMessage2Humanity = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dataGridViewAdcResults = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAdcResults)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCalibNext
            // 
            this.buttonCalibNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.buttonCalibNext.Location = new System.Drawing.Point(41, 182);
            this.buttonCalibNext.Name = "buttonCalibNext";
            this.buttonCalibNext.Size = new System.Drawing.Size(121, 42);
            this.buttonCalibNext.TabIndex = 0;
            this.buttonCalibNext.Text = "Next";
            this.buttonCalibNext.UseVisualStyleBackColor = true;
            this.buttonCalibNext.Click += new System.EventHandler(this.ButtonCalibNext_Click);
            // 
            // buttonCalibAbort
            // 
            this.buttonCalibAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.buttonCalibAbort.Location = new System.Drawing.Point(184, 182);
            this.buttonCalibAbort.Name = "buttonCalibAbort";
            this.buttonCalibAbort.Size = new System.Drawing.Size(118, 42);
            this.buttonCalibAbort.TabIndex = 1;
            this.buttonCalibAbort.Text = "Abort";
            this.buttonCalibAbort.UseVisualStyleBackColor = true;
            this.buttonCalibAbort.Click += new System.EventHandler(this.Click_AbortCalib);
            // 
            // numericUpDownResult
            // 
            this.numericUpDownResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.numericUpDownResult.Location = new System.Drawing.Point(41, 75);
            this.numericUpDownResult.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownResult.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDownResult.Name = "numericUpDownResult";
            this.numericUpDownResult.Size = new System.Drawing.Size(261, 34);
            this.numericUpDownResult.TabIndex = 2;
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.labelResults.Location = new System.Drawing.Point(37, 25);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(214, 29);
            this.labelResults.TabIndex = 3;
            this.labelResults.Text = "Actual result (volts)";
            // 
            // ATPTest_textBoxMessage2Humanity
            // 
            this.ATPTest_textBoxMessage2Humanity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.ATPTest_textBoxMessage2Humanity.Location = new System.Drawing.Point(31, 289);
            this.ATPTest_textBoxMessage2Humanity.Multiline = true;
            this.ATPTest_textBoxMessage2Humanity.Name = "ATPTest_textBoxMessage2Humanity";
            this.ATPTest_textBoxMessage2Humanity.Size = new System.Drawing.Size(503, 94);
            this.ATPTest_textBoxMessage2Humanity.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label9.Location = new System.Drawing.Point(38, 247);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(168, 20);
            this.label9.TabIndex = 41;
            this.label9.Text = "Message to humanity";
            // 
            // dataGridViewAdcResults
            // 
            this.dataGridViewAdcResults.AllowUserToAddRows = false;
            this.dataGridViewAdcResults.AllowUserToDeleteRows = false;
            this.dataGridViewAdcResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAdcResults.Location = new System.Drawing.Point(321, 75);
            this.dataGridViewAdcResults.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewAdcResults.Name = "dataGridViewAdcResults";
            this.dataGridViewAdcResults.ReadOnly = true;
            this.dataGridViewAdcResults.RowHeadersWidth = 62;
            this.dataGridViewAdcResults.Size = new System.Drawing.Size(213, 189);
            this.dataGridViewAdcResults.TabIndex = 42;
            // 
            // PressureInCalibTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 395);
            this.Controls.Add(this.dataGridViewAdcResults);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ATPTest_textBoxMessage2Humanity);
            this.Controls.Add(this.labelResults);
            this.Controls.Add(this.numericUpDownResult);
            this.Controls.Add(this.buttonCalibAbort);
            this.Controls.Add(this.buttonCalibNext);
            this.Name = "PressureInCalibTest";
            this.Text = "Pressure ADC Fit";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAdcResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCalibNext;
        private System.Windows.Forms.Button buttonCalibAbort;
        private System.Windows.Forms.NumericUpDown numericUpDownResult;
        private System.Windows.Forms.Label labelResults;
        private System.Windows.Forms.TextBox ATPTest_textBoxMessage2Humanity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dataGridViewAdcResults;
    }
}