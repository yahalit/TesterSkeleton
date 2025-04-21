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
    public partial class CInput : Form
    {
        public CInput()
        {
            InitializeComponent();
        }
        public void SetLabel(string str)
        {
            labelInput.Text = str;
        }
        public void SetText(string str)
        {
            textBoxInput.Text = str;
        }
        public string GetText(ref double[] val,ref int NumIsOk)
        {
            int ind;
            string[] words;
            try
            {
                words  = textBoxInput.Text.Split(',');
                for( ind = 0; ind < words.Length; ind++  )
                    val[ind] = Convert.ToDouble(words[ind]);
                NumIsOk = words.Length;
            }
            catch 
            {
                NumIsOk = 0;
            }
            return textBoxInput.Text ;
        }
    }
}
