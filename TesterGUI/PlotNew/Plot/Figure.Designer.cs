namespace Plot
{
    partial class Figure
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
            this.tableLayoutPanelPlots = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tableLayoutPanelPlots
            // 
            this.tableLayoutPanelPlots.AllowDrop = true;
            this.tableLayoutPanelPlots.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanelPlots.ColumnCount = 1;
            this.tableLayoutPanelPlots.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPlots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPlots.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelPlots.Name = "tableLayoutPanelPlots";
            this.tableLayoutPanelPlots.RowCount = 1;
            this.tableLayoutPanelPlots.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPlots.Size = new System.Drawing.Size(897, 578);
            this.tableLayoutPanelPlots.TabIndex = 0;
            // 
            // Figure
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 578);
            this.Controls.Add(this.tableLayoutPanelPlots);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Figure";
            this.Text = "Figure";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPlots;
    }
}

