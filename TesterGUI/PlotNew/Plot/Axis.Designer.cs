namespace Axis
{
    partial class Axis
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripAxis = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slopeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frequencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fFTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipAxis = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripAxis.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripAxis
            // 
            this.contextMenuStripAxis.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripAxis.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mathToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.zoomToolStripMenuItem,
            this.zoomOutToolStripMenuItem});
            this.contextMenuStripAxis.Name = "contextMenuStripAxis";
            this.contextMenuStripAxis.Size = new System.Drawing.Size(128, 114);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.closeAllToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.addToolStripMenuItem.Text = "New";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.addToolStripMenuItem1,
            this.openAllToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.addToolStripMenuItem1.Text = "Add from file";
            this.addToolStripMenuItem1.Click += new System.EventHandler(this.addToolStripMenuItem1_Click);
            // 
            // openAllToolStripMenuItem
            // 
            this.openAllToolStripMenuItem.Name = "openAllToolStripMenuItem";
            this.openAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.openAllToolStripMenuItem.Text = "Open all";
            this.openAllToolStripMenuItem.Click += new System.EventHandler(this.openAllToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1,
            this.addToolStripMenuItem2});
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(129, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // addToolStripMenuItem2
            // 
            this.addToolStripMenuItem2.Name = "addToolStripMenuItem2";
            this.addToolStripMenuItem2.Size = new System.Drawing.Size(129, 22);
            this.addToolStripMenuItem2.Text = "Add to file";
            this.addToolStripMenuItem2.Click += new System.EventHandler(this.addToolStripMenuItem2_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.saveAllToolStripMenuItem.Text = "Save all";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.closeAllToolStripMenuItem.Text = "Close all";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // mathToolStripMenuItem
            // 
            this.mathToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xDiffToolStripMenuItem,
            this.yDiffToolStripMenuItem,
            this.meanToolStripMenuItem,
            this.slopeToolStripMenuItem,
            this.frequencyToolStripMenuItem,
            this.fFTToolStripMenuItem});
            this.mathToolStripMenuItem.Name = "mathToolStripMenuItem";
            this.mathToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.mathToolStripMenuItem.Text = "Ginput";
            // 
            // xDiffToolStripMenuItem
            // 
            this.xDiffToolStripMenuItem.Name = "xDiffToolStripMenuItem";
            this.xDiffToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.xDiffToolStripMenuItem.Text = "XDiff";
            this.xDiffToolStripMenuItem.Click += new System.EventHandler(this.xDiffToolStripMenuItem_Click);
            // 
            // yDiffToolStripMenuItem
            // 
            this.yDiffToolStripMenuItem.Name = "yDiffToolStripMenuItem";
            this.yDiffToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.yDiffToolStripMenuItem.Text = "YDiff";
            this.yDiffToolStripMenuItem.Click += new System.EventHandler(this.yDiffToolStripMenuItem_Click);
            // 
            // meanToolStripMenuItem
            // 
            this.meanToolStripMenuItem.Name = "meanToolStripMenuItem";
            this.meanToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.meanToolStripMenuItem.Text = "Mean";
            this.meanToolStripMenuItem.Click += new System.EventHandler(this.meanToolStripMenuItem_Click);
            // 
            // slopeToolStripMenuItem
            // 
            this.slopeToolStripMenuItem.Name = "slopeToolStripMenuItem";
            this.slopeToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.slopeToolStripMenuItem.Text = "Slope";
            this.slopeToolStripMenuItem.Click += new System.EventHandler(this.slopeToolStripMenuItem_Click);
            // 
            // frequencyToolStripMenuItem
            // 
            this.frequencyToolStripMenuItem.Name = "frequencyToolStripMenuItem";
            this.frequencyToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.frequencyToolStripMenuItem.Text = "Frequency";
            this.frequencyToolStripMenuItem.Click += new System.EventHandler(this.frequencyToolStripMenuItem_Click);
            // 
            // fFTToolStripMenuItem
            // 
            this.fFTToolStripMenuItem.Name = "fFTToolStripMenuItem";
            this.fFTToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.fFTToolStripMenuItem.Text = "FFT";
            this.fFTToolStripMenuItem.Click += new System.EventHandler(this.fFTToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.zoomToolStripMenuItem.Text = "Zoom";
            this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // Axis
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.ContextMenuStrip = this.contextMenuStripAxis;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Axis";
            this.Size = new System.Drawing.Size(260, 177);
            this.toolTipAxis.SetToolTip(this, "Axis");
            this.SizeChanged += new System.EventHandler(this.Axis_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Axis_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Axis_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Axis_DragOver);
            this.DragLeave += new System.EventHandler(this.Axis_DragLeave);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Axis_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Axis_MouseUp);
            this.contextMenuStripAxis.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripAxis;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xDiffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yDiffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slopeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frequencyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fFTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem2;
        private System.Windows.Forms.ToolTip toolTipAxis;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAllToolStripMenuItem;
    }
}
