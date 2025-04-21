using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using OfficeOpenXml;  // EPPlus namespace
using System.IO;
using System.Threading;
using MatFileHandler;
using ClosedXML.Excel;

//using MatFileHandler.matDataTypes;


namespace PvsGUI
{
    public class CDownFWEventArgs : EventArgs
    {
        // 1: Kill and go to boot 
        // 2: PrepFwLoad
        // 3: Firm Download 
        public int OpCode;  

        public CDownFWEventArgs(int opcode)
        {
            OpCode = opcode;
        }
    }

    public partial class CPvsGUIold : Form
    {
        static Mutex mutex = new Mutex();
        bool ServicePeriodicStatusOn = false ;
        bool GraphicsOpen = false;
        GraphicDefs GraphicForm ;
        VersionDisplay VerDisplay;
        bool VersionDisplayOn; 
        string RootfilePath = @"..\..\";

        Label[] ValveLabels;
        // Method to replace the image (Bitmap) in the specified PictureBox
        public void SetLedColor(PictureBox pictureBoxInControl, string color)
        {
            // Dispose of the old image to free up resources (if needed)
            if (pictureBoxInControl.Image != null)
            {
                pictureBoxInControl.Image.Dispose();
            }

            switch (color)
            {
                case "Red":
                    pictureBoxInControl.Image = Properties.Resources.REDLED;
                    break;
                case "Blue":
                    pictureBoxInControl.Image = Properties.Resources.BLUELED;
                    break;
                case "Green":
                    pictureBoxInControl.Image = Properties.Resources.GREENLED;
                    break;
                default:
                    pictureBoxInControl.Image = Properties.Resources.GRAYLED;
                    break; 
            }
        }
        public CInterpreter Interpreter = new CInterpreter(mutex);
        bool IsComOpen; 

        System.Windows.Forms.Timer SimTimer = new System.Windows.Forms.Timer();
        static public AxisList.AxisList PlotExamplePos = new AxisList.AxisList(1 );
        static public AxisList.AxisList PlotExampleCur = new AxisList.AxisList(1 );

        public CPvsGUIold()
        {
            InitializeComponent();
        }

        string PrintSwVersion(CSWversion str)
        {
            string[] mon = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string strmon = "Bad";
            if ( str.month >= 1 && str.month <= 12)
            {
                strmon = mon[str.month-1];
            }
            string ostr = String.Format("{0}:{1}:{2},{3}:{4}:{5}",str.ver,str.subver,str.patch,str.year,strmon,str.day) ;
            return ostr;
        }


        //public int[] FaultSimMode = new int[14] ;
        private void Form1_Load(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory; 
            string filePath = RootfilePath +  "PvsGUISetupFile.xlsx";
            //FileInfo fileInfo = new FileInfo(filePath);

            // Load .mat file
            // Loading data
/*
            // https://github.com/mahalex/MatFileHandler?tab=readme-ov-file
            IMatFile matFile;
            using (var fileStream = new System.IO.FileStream(@"..\..\example.mat", System.IO.FileMode.Open))
            {
                var reader = new MatFileReader(fileStream);
                matFile = reader.Read();
            }
*/
            // Load the Excel file
            buttonConnect.Click += new EventHandler(buttonConnect_Click);
            buttonViewVer.Enabled = false;

            ValveLabels = new Label[] {  labelV1 ,  labelV2 ,  labelV3 ,  labelV4 ,  labelV5 , labelV6 ,  labelV7 ,
             labelV8 ,  labelV9 ,  labelV10 ,  labelV11 ,  labelV12 ,  labelV13 ,  labelV14  };

            labelTimeNow.Text = DateTime.Now.ToString();
            labelTimeRemain.Text = 0.ToString();
            numericUpDownStartTimeAhead.Value = 1.0m;
            numericUpDownLifeTime.Value = 100.0m;



            textBoxMessage2Humanity.Text = "Run\r\n You fools\r\n (Gandalf)"; 
            InitDataGridView();
            InitComboBox();
            SetLedColor(pictureLEDConnect, "Blue");
            SetLedColor(pictureLedInControl, "Blue");
            SimTimer.Tick += new EventHandler(PeriodicFunc);
            SimTimer.Interval = 300;
            SimTimer.Start();
            PlotStam();

        }

        static public string [] GridNames = {
            "V1",
            "V2",
            "V3",
            "V4",
            "V5",
            "V6",
            "V7",
            "V8",
            "V9",
            "V10",
            "V11",
            "V12",
            "V13",
            "V14",
        };


        static public string[] FailModeNames = {
             "Disabled",
             "Normal",
             "Stuck open",
             "Stuck closed",
             "TimeDelay",
             "SlowReaction"
        };

        public void InitDataGridView()
        {
            int ind;
            DataGridViewTextBoxColumn tbm = new DataGridViewTextBoxColumn();
            tbm.HeaderText = "Mode";
            tbm.Name = "Mode";
            dataGridView1.Columns.Add(tbm);
            DataGridViewTextBoxColumn tbc = new DataGridViewTextBoxColumn();
            tbc.HeaderText = "Current";
            tbc.Name = "Current";
            dataGridView1.Columns.Add(tbc);
            DataGridViewTextBoxColumn tbp = new DataGridViewTextBoxColumn();
            tbp.HeaderText = "Relative Pos";
            tbp.Name = "Relative Pos";
            dataGridView1.Columns.Add(tbp);
            DataGridViewTextBoxColumn tbf = new DataGridViewTextBoxColumn();
            tbf.HeaderText = "Fault";
            tbf.Name = "Fault";
            dataGridView1.Columns.Add(tbf);
            DataGridViewTextBoxColumn tbsm = new DataGridViewTextBoxColumn();
            tbsm.HeaderText = "SimMode";
            tbsm.Name = "SimMode";
            dataGridView1.Columns.Add(tbsm);
            for (ind = 0; ind < 5 ; ind++)
            {
                dataGridView1.Columns[ind].Width = dataGridView1.Size.Width  / 6;
            }
            for (ind = 0; ind < GridNames.Length; ind++)
            {
                dataGridView1.Rows.Add(GridNames[ind]);
                dataGridView1.RowHeadersVisible = true;
                dataGridView1.Rows[ind].HeaderCell.Value = GridNames[ind];
                for ( int cnt = 0; cnt < 5; cnt++ )
                {
                    dataGridView1.Rows[ind].Cells[cnt].ReadOnly = true;
                    dataGridView1.Rows[ind].Cells[cnt].Value = 0;
                    dataGridView1.Rows[ind].Selected = false;
                }
            }
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            //dataGridView1.Rows[ind-1].Selected = true;
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if( dataGridView1.SelectedRows.Count <= 0 )
            {
                return;
            }
        }
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string msg;
            string com = (string) comboBoxComPorts.Items[comboBoxComPorts.SelectedIndex];
            mutex.WaitOne();


            if (IsComOpen == false)
            {
                IsComOpen = Interpreter.HostCom.OpenSerialPort(com , out msg) ;
                if (IsComOpen)
                {
                    SetLedColor(pictureLEDConnect, "Green");
                    buttonConnect.Text = "Disconnect";
                    msg = string.Format("Port opened successfully");
                    Interpreter.GetVersion(isbg: true); 
                }
                else
                {
                    msg = string.Format("Could not open port");
                }
            }
            else
            {
                IsComOpen = false;
                Interpreter.HostCom.mySerialPort.Close(); 
                SetLedColor(pictureLEDConnect, "Blue");
                buttonConnect.Text = "Connect";
                msg = string.Format("Closed com port");
            }
            textBoxMessage2Humanity.Text = msg;
            mutex.ReleaseMutex();
        }
        public void InitComboBox()
        {
            string[] ports;
            Interpreter.HostCom.GetAvailablePorts(out ports);
            if (ports.Length == 0 )
            {
                string[] NoPort = { "None"};
                ports = NoPort;
            }
            foreach (string port in ports)
            {
                comboBoxComPorts.Items.Add(port);
            }
           comboBoxComPorts.SelectedIndex = 0;
        }


        public void  PlotStam()
        {
            int ind;
            Color col = Color.FromArgb(0, 0, 200);
            int arrLen = 100;
            double[] xData = new double[arrLen];
            double[] yData1 = new double[arrLen];
            double[] yData2 = new double[arrLen];
            double totalTime = 1.0;
            double freq = 1;
            double offset = 1;
            for (ind = 0; ind < xData.Length; ind++)
            {
                xData[ind] = totalTime / arrLen * ind;
                yData1[ind] = Math.Sin(2 * Math.PI * freq * xData[ind]) + offset;
                yData2[ind] = Math.Cos(2 * Math.PI * freq * xData[ind]) + offset;
            }
            PlotExamplePos.DeleteAllLine(0);
            PlotExamplePos.AddLine(axisInd: 0, xData: xData, yData: yData1, col: col, name: "kukuk");
            PlotExamplePos.InvalidateAxis(0);

            PlotExampleCur.DeleteAllLine(0);
            PlotExampleCur.AddLine(axisInd: 0, xData: xData, yData: yData1, col: System.Drawing.Color.Red , name: "muku");
            PlotExampleCur.AddLine(axisInd: 0, xData: xData, yData: yData2, col: System.Drawing.Color.Green , name: "chuku");
            PlotExampleCur.InvalidateAxis(0);


        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click_1(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void StartPeriodicMessage_Click(object sender, EventArgs e)
        {
            // One message 
            if (ServicePeriodicStatusOn )
            {
                Interpreter.GetPeriodicStatus(isbg: true, value: (ushort)0);
                ServicePeriodicStatusOn = false ;
            }
            else
            {
                ushort value = (ushort)numericPeriodicMsec.Value;
                if (value > 1 && value < 5)
                    value = 5;
                ServicePeriodicStatusOn = true ;
                Interpreter.GetPeriodicStatus(isbg: true, value: value );
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label43_Click(object sender, EventArgs e)
        {

        }

        private void label43_Click_1(object sender, EventArgs e)
        {

        }

        private void buttonViewVer_Click(object sender, EventArgs e)
        {
            if (VersionDisplayOn )
            {
                return;
            }
            VersionDisplay VerDisplay = new VersionDisplay(Interpreter.VersionMsg , RootfilePath) ;
            VerDisplay.RequestInterpreterEvent += VersionDisplayRequestInterpreterHandler ;
            VerDisplay.FormClosed += (_sender, _e) => { VersionDisplayOn = false; };
            VerDisplay.Show();
            VersionDisplayOn = true; 
        }

        // Event handler method
        private void VersionDisplayRequestInterpreterHandler(object sender, EventArgs e)
        {
            if (!VersionDisplayOn)
            {
                return;
            }
            MessageBox.Show("Custom event triggered from the form!");
            VerDisplay.SignalCompletion();
        }

        private void BuildExecCommand(bool sim = false )
        {
            uint[] options = new uint[14];
            options[0] = (uint)comboValve1.SelectedIndex;
            options[1] = (uint)comboValve2.SelectedIndex;
            options[2] = (uint)comboValve3.SelectedIndex;
            options[3] = (uint)comboValve4.SelectedIndex;
            options[4] = (uint)comboValve5.SelectedIndex;
            options[5] = (uint)comboValve6.SelectedIndex;
            options[6] = (uint)comboValve7.SelectedIndex;
            options[7] = (uint)comboValve8.SelectedIndex;
            options[8] = (uint)comboValve9.SelectedIndex;
            options[9] = (uint)comboValve10.SelectedIndex;
            options[10] = (uint)comboValve11.SelectedIndex;
            options[11] = (uint)comboValve12.SelectedIndex;
            options[12] = (uint)comboValve13.SelectedIndex;
            options[13] = (uint)comboValve14.SelectedIndex;
            ulong timenow = Interpreter.GetTimeUsec();
            ulong startTime = (ulong)(numericUpDownStartTimeAhead.Value * 1000000) + timenow;
            ulong LifeTime = (ulong)(numericUpDownLifeTime.Value * 1000000);

            Interpreter.FillExecConfig(options, startTime, LifeTime, sim );
            Interpreter.SendExec(isbg: true);
        }

        private void buttonExec_Click(object sender, EventArgs e)
        {
            // Prepare an exec message
            BuildExecCommand(sim : false); 
        }

        private void comboBoxComPorts_Enter(object sender, EventArgs e)
        {
            string[] ports;
            Interpreter.HostCom.GetAvailablePorts(out ports);
            if (ports.Length == 0)
            {
                string[] NoPort = { "None" };
                ports = NoPort;
            }
            comboBoxComPorts.Items.Clear();
            foreach (string port in ports)
            {
                comboBoxComPorts.Items.Add(port);
            }

        }

        private void comboBoxComPorts_DropDown(object sender, EventArgs e)
        {
            comboBoxComPorts_Enter(sender, e);
        }

       // handler for sending the selected modle set
        private void button1_Click(object sender, EventArgs e)
        {
            ushort ParamSet = (ushort) numericUpDownParamSet.Value;
            Interpreter.SetParametersSet(isbg: true, ParamSet);
       }

        private void label8_Click(object sender, EventArgs e)
        {

        }

 
        private void buttonSimulate_Click(object sender, EventArgs e)
        {
            BuildExecCommand(sim : true);

        }

        private void labelOneOrMoreValvesFail_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void SetIdOut_Click(object sender, EventArgs e)
        {
            ushort IdoutValue  = (ushort)numericUpDownParamSet.Value;
            Interpreter.SetIdOut(isbg: true, Math.Min(IdoutValue,(ushort)15) );
        }

        private void buttonGraphicsForm_Click(object sender, EventArgs e)
        {
            if (GraphicsOpen)
                return;

            bool[] installed = new bool[14]; 
            for ( int cnt = 0; cnt < 14; cnt++ )
            {
                installed[cnt] = Interpreter.BitDetail[cnt].installed; 
            }
            
            try
            {
                GraphicForm = new GraphicDefs(installed, RootfilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant open the defintions, \nmaybe GraphSetup.xlsx is open by Excel!,\n"+ ex.Message);
                return;
            }
            GraphicForm.FormClosed += (s, args) =>
            {
                // This code runs when myForm is closed
                GraphicsOpen = false ;
                buttonGraphicsForm.Enabled = true;
            };
            GraphicForm.Show();
            GraphicsOpen = true;
            buttonGraphicsForm.Enabled = false; 
        }

        private void DownMainFW_Click(object sender, EventArgs e)
        {

        }
    }
}
