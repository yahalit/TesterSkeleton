using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static Literals;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using System.Security.Policy;
using Kvaser.CanLib;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Plot;
using System.Globalization;

namespace PvsGUI
{
    public partial class CPvsGUI : Form
    {
        public CPvsGUI()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            mutex = Interpreter.GetMutex();
            Interpreter.PopulateErrorList(RootfilePath + @"..\..\Exe\CpuErrorCodes.h");
        }

        public string GuiVer = "1.0";
        public Mutex mutex;//= new Mutex();
        //public bool ServicePeriodicStatusOn = false; // Item definition: Initially no graphic data collection 
        public CInterpreter Interpreter = CInterpreter.Instance; //  new CInterpreter(mutex);
        public CRecorder Recorder = CRecorder.Instance; //  new CInterpreter(mutex);
        public ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        public Stopwatch stopwatch = new Stopwatch();

        public CAtpMainWindow AtpMainWindow;

        //public bool IsComOpen;
        //public bool bGUITakesOver = false;
        public bool GraphicsOpen = false;
        public int NoCommCounter = 0;
        public bool VersionIsKnown = false;
        public bool BlockMainCommunication = false;
        public bool bGUIStateAvailable = false;
        public bool CommGood = false;
        public bool WasCommGood = false;
        public int[] failure = new int[Literals.N_VALVES];

        private bool InstallGraphicDefsAlreadyRunInPeriodicFunction = false;

        public string RootfilePath = @"..\..\";
        public System.Windows.Forms.Timer SimTimer = new System.Windows.Forms.Timer();
        private readonly object _lock = new object();

        CSystemBit SystemBIT = new CSystemBit();
        CAtpUnits AtpUnits;
        private void CPvsGUI_Load(object sender, EventArgs e)
        {

            labelGuiAppVersion.Text = GuiVer;

            textBoxMessage2Humanity.Text = "General comments";
            //InitDataGridView();
            //InitComboBox();
            SimTimer.Tick += new EventHandler(PvsGUIPeriodicFunc);
            SimTimer.Interval = 300;
            SimTimer.Start();

        }
        private void PvsGUIPeriodicFunc(Object myObject,
            EventArgs myEventArgs)
        {
            if (BlockMainCommunication || Interpreter.InOfflineSession)
            {
                return;
            }
            bool lockTaken = false;
            try
            {
                // Try to acquire the lock - assure routine will not preempt itself
                System.Threading.Monitor.TryEnter(_lock, ref lockTaken);
                if (!lockTaken)
                {
                    return; // If another thread has the lock, exit the Tick event
                }
                //PvsGUIPeriodicFuncBody(myObject, myEventArgs);
            }
            finally
            {
                if (lockTaken)
                {
                    // Release the lock if acquired
                    System.Threading.Monitor.Exit(_lock);
                }
            }
        }


 
        private void DiagnosticToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /*
        // A value change has immediated effect only if status is already sent periodically 
        private void PeriodicMsgRateChanged(object sender, EventArgs e)
        {
            int msec;
            if (Interpreter.ServicePeriodicStatusOn)
            {
                msec = (int)numericPeriodicMsec.Value;
                Interpreter.SendSetGUIPeriodicMessage(msec, isbg: true);
            }
        }
        */
        // This routine executes after the form is closed. It should stop the periodic message
        public void Lastwill()
        {
            SimTimer.Stop();
        }

        private void ATPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string DefaultCalibPath = Directory.GetParent(Directory.GetParent(Path.GetFullPath(RootfilePath)).FullName).FullName + @"\TesterCalib";
            AtpUnits = new CAtpUnits(DefaultCalibPath);
            AtpUnits.Show();
        }

        private void TechnicianToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private async void MenuItemATP_Click(object sender, EventArgs e)
        {

            string DefaultATRPath = Directory.GetParent(Directory.GetParent(Path.GetFullPath(RootfilePath)).FullName).FullName + @"\ATR";
            string ATRTemplatePath = Directory.GetParent(Directory.GetParent(Path.GetFullPath(RootfilePath)).FullName).FullName + @"\ATRTemplate\PvsATRTemplate_V001.xlsx";
            string DefaultCalibPath = Directory.GetParent(Directory.GetParent(Path.GetFullPath(RootfilePath)).FullName).FullName + @"\TesterCalib";

            AtpMainWindow = new CAtpMainWindow(DefaultATRPath, ATRTemplatePath, DefaultCalibPath, RootfilePath);
            if (Interpreter.IsComOpen)
            {
                Interpreter.SendSetGUIPeriodicMessage(0xffff, isbg: true, _manualResetEvent);
                stopwatch.Reset();
                stopwatch.Start();
                //HaveVersion = _manualResetEvent.WaitOne(2000);
                bool StoppedPeriodicUnsolicitedMessages = await Task.Run(() =>
                {
                    // Wait 
                    return _manualResetEvent.WaitOne(5000); // (2000);
                });
                stopwatch.Stop();

                if (StoppedPeriodicUnsolicitedMessages == false)
                {
                    MessageBox.Show("Could not stop unsolicited messages");
                    return;
                }

                Interpreter.ServicePeriodicStatusOn = false; // Stop data collection if going to firmware downloading



                BlockMainCommunication = true;
                Interpreter.Flush();

            }
            //TODO: Release line FV.TopMost = true;
            AtpMainWindow.ShowDialog();

            //AtpMainWindow.Show(); 
        }



        private void HighSpeedRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Interpreter.IsComOpen)
            {
                if ( Recorder.PopulateSignalList(RootfilePath + @"..\..\Exe\ProjRecorderSignals.h", RootfilePath + @"..\..\RecordedData", OpenGUI: true))
                {
                    Recorder.OpenSigRecForm();
                }
                else
                {
                    MessageBox.Show(this, "Could not find the recorder signals definition file");
                }
            }
            else
            {
                MessageBox.Show(this, "You must open communication before");
                return;
            }
        }


     }

}
