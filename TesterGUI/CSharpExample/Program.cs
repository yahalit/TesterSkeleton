using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PvsGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CPvsGUI MyPvsGUI = new CPvsGUI() ;
            Application.Run(MyPvsGUI);
            MyPvsGUI.Lastwill(); 
        }
    }
}
