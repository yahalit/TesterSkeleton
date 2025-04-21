
namespace PvsGUI
{
    partial class CPvsGUI
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.DiagnosticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highSpeedRecorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.technicianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aTPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemATP = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxMessage2Humanity = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelGuiAppVersion = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DiagnosticsToolStripMenuItem,
            this.technicianToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(522, 24);
            this.menuStrip1.TabIndex = 58;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // DiagnosticsToolStripMenuItem
            // 
            this.DiagnosticsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highSpeedRecorderToolStripMenuItem});
            this.DiagnosticsToolStripMenuItem.Name = "DiagnosticsToolStripMenuItem";
            this.DiagnosticsToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.DiagnosticsToolStripMenuItem.Text = "Diagnostic";
            this.DiagnosticsToolStripMenuItem.Click += new System.EventHandler(this.DiagnosticToolStripMenuItem_Click);
            // 
            // highSpeedRecorderToolStripMenuItem
            // 
            this.highSpeedRecorderToolStripMenuItem.Name = "highSpeedRecorderToolStripMenuItem";
            this.highSpeedRecorderToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.highSpeedRecorderToolStripMenuItem.Text = "High speed Recorder";
            this.highSpeedRecorderToolStripMenuItem.Click += new System.EventHandler(this.HighSpeedRecorderToolStripMenuItem_Click);
            // 
            // technicianToolStripMenuItem
            // 
            this.technicianToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aTPToolStripMenuItem,
            this.MenuItemATP});
            this.technicianToolStripMenuItem.Name = "technicianToolStripMenuItem";
            this.technicianToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.technicianToolStripMenuItem.Text = "Technician";
            this.technicianToolStripMenuItem.Click += new System.EventHandler(this.TechnicianToolStripMenuItem_Click);
            // 
            // aTPToolStripMenuItem
            // 
            this.aTPToolStripMenuItem.Name = "aTPToolStripMenuItem";
            this.aTPToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aTPToolStripMenuItem.Text = "ATP Maintenance";
            this.aTPToolStripMenuItem.Click += new System.EventHandler(this.ATPToolStripMenuItem_Click);
            // 
            // MenuItemATP
            // 
            this.MenuItemATP.Name = "MenuItemATP";
            this.MenuItemATP.Size = new System.Drawing.Size(180, 22);
            this.MenuItemATP.Text = "ATP";
            this.MenuItemATP.Click += new System.EventHandler(this.MenuItemATP_Click);
            // 
            // textBoxMessage2Humanity
            // 
            this.textBoxMessage2Humanity.Location = new System.Drawing.Point(9, 611);
            this.textBoxMessage2Humanity.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMessage2Humanity.Multiline = true;
            this.textBoxMessage2Humanity.Name = "textBoxMessage2Humanity";
            this.textBoxMessage2Humanity.Size = new System.Drawing.Size(519, 45);
            this.textBoxMessage2Humanity.TabIndex = 64;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(590, 613);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 66;
            this.label3.Text = "GUI application version:";
            // 
            // labelGuiAppVersion
            // 
            this.labelGuiAppVersion.AutoSize = true;
            this.labelGuiAppVersion.Location = new System.Drawing.Point(715, 613);
            this.labelGuiAppVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelGuiAppVersion.Name = "labelGuiAppVersion";
            this.labelGuiAppVersion.Size = new System.Drawing.Size(22, 13);
            this.labelGuiAppVersion.TabIndex = 67;
            this.labelGuiAppVersion.Text = "1.0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(590, 642);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 70;
            this.label4.Text = "Date:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(715, 642);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "09.04.2025";
            // 
            // CPvsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 119);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelGuiAppVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxMessage2Humanity);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CPvsGUI";
            this.Text = "PvsGUI";
            this.Load += new System.EventHandler(this.CPvsGUI_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem DiagnosticsToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxMessage2Humanity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelGuiAppVersion;
        private System.Windows.Forms.ToolStripMenuItem technicianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aTPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemATP;
        private System.Windows.Forms.ToolStripMenuItem highSpeedRecorderToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}