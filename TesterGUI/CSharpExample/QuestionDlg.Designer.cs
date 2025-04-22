namespace TesterGUI
{
    partial class QuestionDlg
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
            this.labelTestHeader = new System.Windows.Forms.Label();
            this.textBoxRslt = new System.Windows.Forms.TextBox();
            this.numericUpDownRslt = new System.Windows.Forms.NumericUpDown();
            this.buttonEnterRslt = new System.Windows.Forms.Button();
            this.textBoxInstructions = new System.Windows.Forms.TextBox();
            this.checkBoxRslt = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRslt)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTestHeader
            // 
            this.labelTestHeader.AutoSize = true;
            this.labelTestHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTestHeader.Location = new System.Drawing.Point(66, 25);
            this.labelTestHeader.Name = "labelTestHeader";
            this.labelTestHeader.Size = new System.Drawing.Size(326, 32);
            this.labelTestHeader.TabIndex = 0;
            this.labelTestHeader.Text = "Test header bla bla bla";
            // 
            // textBoxRslt
            // 
            this.textBoxRslt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRslt.Location = new System.Drawing.Point(72, 73);
            this.textBoxRslt.Name = "textBoxRslt";
            this.textBoxRslt.Size = new System.Drawing.Size(386, 35);
            this.textBoxRslt.TabIndex = 1;
            // 
            // numericUpDownRslt
            // 
            this.numericUpDownRslt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownRslt.Location = new System.Drawing.Point(72, 90);
            this.numericUpDownRslt.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRslt.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numericUpDownRslt.Name = "numericUpDownRslt";
            this.numericUpDownRslt.Size = new System.Drawing.Size(440, 35);
            this.numericUpDownRslt.TabIndex = 2;
            // 
            // buttonEnterRslt
            // 
            this.buttonEnterRslt.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnterRslt.Location = new System.Drawing.Point(138, 181);
            this.buttonEnterRslt.Name = "buttonEnterRslt";
            this.buttonEnterRslt.Size = new System.Drawing.Size(201, 50);
            this.buttonEnterRslt.TabIndex = 3;
            this.buttonEnterRslt.Text = "Approved";
            this.buttonEnterRslt.UseVisualStyleBackColor = true;
            this.buttonEnterRslt.Click += new System.EventHandler(this.Click_UserNext);
            // 
            // textBoxInstructions
            // 
            this.textBoxInstructions.Location = new System.Drawing.Point(72, 237);
            this.textBoxInstructions.Multiline = true;
            this.textBoxInstructions.Name = "textBoxInstructions";
            this.textBoxInstructions.Size = new System.Drawing.Size(492, 106);
            this.textBoxInstructions.TabIndex = 4;
            // 
            // checkBoxRslt
            // 
            this.checkBoxRslt.AutoSize = true;
            this.checkBoxRslt.Location = new System.Drawing.Point(80, 145);
            this.checkBoxRslt.Name = "checkBoxRslt";
            this.checkBoxRslt.Size = new System.Drawing.Size(113, 24);
            this.checkBoxRslt.TabIndex = 5;
            this.checkBoxRslt.Text = "checkBox1";
            this.checkBoxRslt.UseVisualStyleBackColor = true;
            // 
            // QuestionDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 349);
            this.Controls.Add(this.checkBoxRslt);
            this.Controls.Add(this.textBoxInstructions);
            this.Controls.Add(this.buttonEnterRslt);
            this.Controls.Add(this.numericUpDownRslt);
            this.Controls.Add(this.textBoxRslt);
            this.Controls.Add(this.labelTestHeader);
            this.Name = "QuestionDlg";
            this.Text = "QuestionDlg";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRslt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTestHeader;
        private System.Windows.Forms.TextBox textBoxRslt;
        private System.Windows.Forms.NumericUpDown numericUpDownRslt;
        private System.Windows.Forms.Button buttonEnterRslt;
        private System.Windows.Forms.TextBox textBoxInstructions;
        private System.Windows.Forms.CheckBox checkBoxRslt;
    }
}