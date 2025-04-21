namespace PvsGUI
{
    partial class CRegression
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
            this.components = new System.ComponentModel.Container();
            this.zedGraphControlFit = new ZedGraph.ZedGraphControl();
            this.zedGraphControlError = new ZedGraph.ZedGraphControl();
            this.LabelCoeffs = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonRehect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // zedGraphControlFit
            // 
            this.zedGraphControlFit.Location = new System.Drawing.Point(47, 57);
            this.zedGraphControlFit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.zedGraphControlFit.Name = "zedGraphControlFit";
            this.zedGraphControlFit.ScrollGrace = 0D;
            this.zedGraphControlFit.ScrollMaxX = 0D;
            this.zedGraphControlFit.ScrollMaxY = 0D;
            this.zedGraphControlFit.ScrollMaxY2 = 0D;
            this.zedGraphControlFit.ScrollMinX = 0D;
            this.zedGraphControlFit.ScrollMinY = 0D;
            this.zedGraphControlFit.ScrollMinY2 = 0D;
            this.zedGraphControlFit.Size = new System.Drawing.Size(685, 216);
            this.zedGraphControlFit.TabIndex = 0;
            this.zedGraphControlFit.UseExtendedPrintDialog = true;
            // 
            // zedGraphControlError
            // 
            this.zedGraphControlError.Location = new System.Drawing.Point(47, 301);
            this.zedGraphControlError.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.zedGraphControlError.Name = "zedGraphControlError";
            this.zedGraphControlError.ScrollGrace = 0D;
            this.zedGraphControlError.ScrollMaxX = 0D;
            this.zedGraphControlError.ScrollMaxY = 0D;
            this.zedGraphControlError.ScrollMaxY2 = 0D;
            this.zedGraphControlError.ScrollMinX = 0D;
            this.zedGraphControlError.ScrollMinY = 0D;
            this.zedGraphControlError.ScrollMinY2 = 0D;
            this.zedGraphControlError.Size = new System.Drawing.Size(685, 216);
            this.zedGraphControlError.TabIndex = 1;
            this.zedGraphControlError.UseExtendedPrintDialog = true;
            // 
            // LabelCoeffs
            // 
            this.LabelCoeffs.AutoSize = true;
            this.LabelCoeffs.Location = new System.Drawing.Point(53, 540);
            this.LabelCoeffs.Name = "LabelCoeffs";
            this.LabelCoeffs.Size = new System.Drawing.Size(325, 17);
            this.LabelCoeffs.TabIndex = 3;
            this.LabelCoeffs.Text = "Resulting coefficients: Gain: xxxxxxxx  Offset: yyyyy";
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonNext.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.buttonNext.Location = new System.Drawing.Point(240, 578);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(126, 29);
            this.buttonNext.TabIndex = 4;
            this.buttonNext.Text = "Approve";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.Click_Next);
            // 
            // buttonRehect
            // 
            this.buttonRehect.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonRehect.BackColor = System.Drawing.SystemColors.Info;
            this.buttonRehect.Location = new System.Drawing.Point(424, 578);
            this.buttonRehect.Name = "buttonRehect";
            this.buttonRehect.Size = new System.Drawing.Size(126, 29);
            this.buttonRehect.TabIndex = 5;
            this.buttonRehect.Text = "Reject";
            this.buttonRehect.UseVisualStyleBackColor = false;
            this.buttonRehect.Click += new System.EventHandler(this.ButtonRehect_Click);
            // 
            // CRegression
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 619);
            this.Controls.Add(this.buttonRehect);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.LabelCoeffs);
            this.Controls.Add(this.zedGraphControlError);
            this.Controls.Add(this.zedGraphControlFit);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CRegression";
            this.Text = "Regression";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControlFit;
        private ZedGraph.ZedGraphControl zedGraphControlError;
        private System.Windows.Forms.Label LabelCoeffs;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonRehect;
    }
}