using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plot
{
    public partial class Prop : Form
    {
        Axis.Axis CurAx;
        public Prop(Axis.Axis ax)
        {
            int ind;
            CurAx = ax;
            InitializeComponent();
            DataGridViewTextBoxColumn tbn = new DataGridViewTextBoxColumn();
            tbn.HeaderText = "Name";
            tbn.Name = "Name";
            dataGridViewProp.Columns.Add(tbn);

            DataGridViewTextBoxColumn tbc = new DataGridViewTextBoxColumn();
            tbc.HeaderText = "Color";
            tbc.Name = "Color";
            dataGridViewProp.Columns.Add(tbc);
            tbc.ReadOnly = true;

            DataGridViewTextBoxColumn tbu = new DataGridViewTextBoxColumn();
            tbu.HeaderText = "Gain";
            tbu.Name = "Gain";
            dataGridViewProp.Columns.Add(tbu);

            DataGridViewTextBoxColumn tbo = new DataGridViewTextBoxColumn();
            tbo.HeaderText = "Offset";
            tbo.Name = "Offset";
            dataGridViewProp.Columns.Add(tbo);

            DataGridViewCheckBoxColumn tbm = new DataGridViewCheckBoxColumn();
            tbm.HeaderText = "Marker";
            tbm.Name = "Marker";
            dataGridViewProp.Columns.Add(tbm);

            for (ind = 0; ind < 5; ind++)
            {
                dataGridViewProp.Columns[ind].Width = dataGridViewProp.Size.Width / 4;
            }

            dataGridViewProp.Rows.Clear();
            for (ind = 0; ind < ax.m_pLines.Count; ind++)
            {
                dataGridViewProp.Rows.Add(ax.m_pLines[ind].m_Name);
                dataGridViewProp.Rows[ind].Cells[1].Style.BackColor = ax.m_pLines[ind].m_Color;
                dataGridViewProp.Rows[ind].Cells[1].Style.SelectionBackColor = ax.m_pLines[ind].m_Color;
                dataGridViewProp.Rows[ind].Cells[2].Value = ax.m_pLines[ind].m_YGain;
                dataGridViewProp.Rows[ind].Cells[3].Value = ax.m_pLines[ind].m_YOffset;
                dataGridViewProp.Rows[ind].Cells[4].Value = (ax.m_pLines[ind].m_PointSize>=4);
            }
        }

        private void dataGridViewProp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 )
            {
                return;
            }
            if (e.ColumnIndex == 1)
            {
                ColorDialog dlg = new ColorDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewProp.Rows[e.RowIndex].Cells[1].Style.BackColor = dlg.Color;
                    dataGridViewProp.Rows[e.RowIndex].Cells[1].Style.SelectionBackColor = dlg.Color;
                    CurAx.m_pLines[e.RowIndex].m_Color = dlg.Color;
                    CurAx.Invalidate();
                }
            }
        }

        private void dataGridViewProp_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int col = e.ColumnIndex;
            int row = e.RowIndex;
            dataGridViewProp.Rows[row].Cells[col].Style.BackColor = Color.FromArgb(255, 200, 100);
        }

        private void dataGridViewProp_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double val;
            int col = e.ColumnIndex;
            int row = e.RowIndex;
            dataGridViewProp.Rows[row].Cells[col].Style.BackColor = Color.FromArgb(255, 255, 255);
            if (dataGridViewProp.Rows[row].Cells[col].Value == null)
            {
                return;
            }
            string str = dataGridViewProp.Rows[row].Cells[col].Value.ToString();
            if ( col == 2 || col == 3 )
            {
                try
                {
                    val = Convert.ToDouble(str);
                    if (col == 2)
                    {
                        CurAx.m_pLines[row].m_YGain = val;
                    }
                    else 
                    {
                        CurAx.m_pLines[row].m_YOffset = val;
                    }
                }
                catch
                {
                    if (col == 2)
                    {
                        dataGridViewProp.Rows[row].Cells[col].Value=CurAx.m_pLines[row].m_YGain ;
                    }
                    else
                    {
                        dataGridViewProp.Rows[row].Cells[col].Value=CurAx.m_pLines[row].m_YOffset;
                    }
                    return;
                }
            }
            else if (col == 0)
            {
                CurAx.m_pLines[row].m_Name = dataGridViewProp.Rows[row].Cells[col].Value.ToString();
            }
            CurAx.Invalidate();
        }

        private void dataGridViewProp_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                int val;
                DataGridViewCheckBoxCell oCell;
                oCell = dataGridViewProp.Rows[e.RowIndex].Cells[4] as DataGridViewCheckBoxCell;
                try
                {
                    val = Convert.ToInt32(oCell.Value);
                }
                catch
                {
                    val = 0;
                }
                oCell.Value = (val==0);
                dataGridViewProp.RefreshEdit();
                dataGridViewProp.Refresh();
                //dataGridViewProp.Rows[e.RowIndex].Cells[4].Value = (val == 0);
                //dataGridViewProp.Invalidate();
                if (val == 0)
                {
                    CurAx.m_pLines[e.RowIndex].m_PointSize = 4;
                }
                else
                {
                    CurAx.m_pLines[e.RowIndex].m_PointSize = 1;
                }
                CurAx.Invalidate();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ind;
            for( ind = dataGridViewProp.SelectedRows.Count-1; ind >= 0;ind--)
            {
                CurAx.DeleteLine(dataGridViewProp.SelectedRows[ind].Index);
                dataGridViewProp.Rows.Remove(dataGridViewProp.SelectedRows[ind]);
            }
            CurAx.Invalidate();
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurAx.DeleteAllLine();
            CurAx.Invalidate();
        }

    }
}
