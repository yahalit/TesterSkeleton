using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesterGUI
{

    public struct GetValueResults
    {
        public double d;
        public string s;
        public bool b; 
    }

    public partial class QuestionDlg : Form
    {
        bool AllowClose = false;
        
        public QuestionDlg(string Header , string CheckText , bool IsText , double lowlimit = -1e9 , double highlimit = 1e9 , string [] Instruction = null)
        {
            InitializeComponent();
            labelTestHeader.Text = Header;
            if (IsText)
            {
                numericUpDownRslt.Visible = false;
                textBoxRslt.Visible = true;
            }
            else
            {
                textBoxRslt.Visible = false;
                numericUpDownRslt.Visible = true;
                numericUpDownRslt.Minimum = (decimal)lowlimit;
                numericUpDownRslt.Maximum = (decimal)highlimit;
            }
            if (Instruction != null)
            {
                textBoxInstructions.Lines = Instruction;
            } 
            else
            {
                textBoxInstructions.Visible = false ;
            }

            if (CheckText != null  )
            {
                checkBoxRslt.Text = CheckText;
            }
            else
            {
                checkBoxRslt.Visible = false;
            }

            this.TopMost = true;
        }
        public static Task<GetValueResults> ShowAsynchronously(string Header, string CheckText, bool IsText, double lowlimit = -1e9, double highlimit = 1e9, string[] Instruction = null)
        {
            var tcs = new TaskCompletionSource<GetValueResults>();

            var form = new QuestionDlg(Header, CheckText , IsText, lowlimit, highlimit, Instruction);
            form.FormClosed += (sender, e) =>
            {
                // Complete the task when the form is closed
                GetValueResults rslt = new GetValueResults();
                rslt.b = form.GetResults(out rslt.d, out rslt.s);
                tcs.SetResult(rslt);
            };

            // Show the form (non-blocking)
            form.ShowDialog();

            return tcs.Task; // Return the task to await
        }

        public bool GetResults( out double numeric , out string text)
        {
            numeric = (double)numericUpDownRslt.Value;
            text = textBoxRslt.Visible ? textBoxRslt.Text.Trim() : numericUpDownRslt.Value.ToString();
            return checkBoxRslt.Checked;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if ( !AllowClose )
            {
                e.Cancel = true; // Cancel closing the form
                return; 
            }
            base.OnFormClosing(e);
        }

        private void Click_UserNext(object sender, EventArgs e)
        {
            AllowClose = true;
            this.Close();
        }

    }
}
