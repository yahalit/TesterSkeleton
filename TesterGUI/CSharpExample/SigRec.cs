using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
//using MatFileHandler;
using ClosedXML.Excel;
using csmatio.io;
using csmatio.types;



namespace PvsGUI
{
    public partial class SigRec : Form
    {
        List<Tuple<int, int, string, string, string>> data;
        DataTable dataTable = new DataTable();
        DataTable selectTable = new DataTable();
        List<bool> selected = new List<bool>();
        public double[][] RecordedSignals = new double[Literals.N_MAX_SIGS][];
        public double[] RecordedTime; 
        CAnswer_SetRecorderParameters MasterPars = new CAnswer_SetRecorderParameters() ;
        //CAnswer_SetRecorderParameters MasterPars = new CAnswer_SetRecorderParameters();
        CGUI_SetRecorderParameters RecorderRequest = new CGUI_SetRecorderParameters();
        //int RecordLength;  The length of one record to bring
        readonly int MessagePayloadCount = 64 ; // The amount of payload items that can be asked in single transaction in a recorder upload sequence

        string ResultStoragePath;
        public CInterpreter Interpreter = CInterpreter.Instance; //  new CInterpreter(mutex);
        public Mutex mutex;
        readonly Func<ushort[], int>[] GetRecordData = new Func<ushort[], int>[13];
        CRecorderPars RecorderPars = new CRecorderPars() ;

        public SigRec(List<Tuple<int, int, string, string, string>> data_in , string ResultStoragePath_in)
        {
            data = data_in; 
            InitializeComponent();
            LoadSigData();
            ResultStoragePath = ResultStoragePath_in;
            labelFilePath.Text = Path.GetFullPath (ResultStoragePath) ;
            mutex = Interpreter.GetMutex();
            for ( int cnt = 0; cnt < 13; cnt++ )
            {
                GetRecordData[cnt] = ReadNullVec;
            }
            GetRecordData[0] = ReadLongVec;
            GetRecordData[2] = ReadFloatVec;
            GetRecordData[4] = ReadULongVec;
            GetRecordData[8] = ReadShortVec;
            GetRecordData[12] = ReadUShortVec;

            MasterPars.FastSamplingTime = 125e-6f; // Start with something that makes sense
        }

        public int GetLengthOfSigList()
        {
            return (int)MasterPars.LengthOfSigList; 
        }

        // Load the signals data into the graphics display table 
        void LoadSigData()
        {
            dataTable.Clear();
            dataTable.Columns.Add("Index", typeof(int));
            dataTable.Columns.Add("Signal", typeof(string));
            dataTable.Columns.Add("Group", typeof(string));
            dataTable.Columns.Add("Help", typeof(string));

            selectTable.Clear();
            selectTable.Columns.Add("Index", typeof(int));
            selectTable.Columns.Add("Signal", typeof(string));

            int cnt = 0; 
            foreach (var item in data)
            {
                cnt += 1; 
                DataRow row = dataTable.NewRow();
                row["Index"] = item.Item1;
                row["Signal"] = item.Item3;
                row["Group"] = item.Item4;
                row["Help"] = item.Item5;
                selected.Add(false);
                dataTable.Rows.Add(row);
            }

            dataGridViewSigRec.DataSource = dataTable;
            dataGridViewSigRec.Refresh();
            dataGridViewSigRec.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;

            dataGridViewSelected.DataSource = selectTable;
            dataGridViewSelected.Refresh();

            textBoxRecFileName.Text = "RecSave";
        }


        public bool RecorderImmediate(string[] SignalNames, CRecorderPars Pars, out double [] t , out double[][] rslt)
        {
            rslt = new double[SignalNames.Length][];
            t = new double[1]; 
            CRecorderPars LocalPars = Pars;

            LocalPars.TriggerMethod = 0; // Assure immediate
            LocalPars.TriggerPercent = 1;

            if ( !RecorderInitiateOffline(SignalNames, LocalPars)) 
                { return false; }
            // Wait the amount of time 
            Thread.Sleep((int)(LocalPars.TimeSpan * 1000 ) );

            // Bring the record 
            if (!LocateSignalsByNames(SignalNames, out int[] SignalIndices, out int[] SignalFlags))
            {
                return false;
            }
            return BringRecordsVector2Array(SignalIndices, SignalFlags, Pars, out rslt, out  t); 
        }

        public bool RecorderInitiateOffline(string[] SignalNames, CRecorderPars Pars) 
        {
            if ( !LocateSignalsByNames(SignalNames, out int[] signals, out int[] Flags))
            {
                return false; 
            }

            int nSignals = signals.Length;

            if (nSignals == 0)
            {
                MessageBox.Show(this, "No signals to record");
                return false;
            }

            int maxpoints = (int)((double)MasterPars.BufLength / nSignals);

            maxpoints = (int)Math.Min(Math.Max(3, Pars.MaxPoints ), maxpoints);
            double TimeSpan = Pars.TimeSpan ;

            int gap = (int)Math.Ceiling(TimeSpan / (maxpoints * MasterPars.FastSamplingTime));
            int TriggerType = Pars.TriggerMethod ;
            int PreTriggerCnt = (int)(maxpoints * 0.01 * Pars.TriggerPercent);

            maxpoints = (int)(TimeSpan / (MasterPars.FastSamplingTime * gap));

            RecorderRequest.Fill(out byte[] buf, Interpreter.AllocateMessageCounter(), nSignals, signals, maxpoints,
                 _RecorderGap: gap, _TimeBasis: 0, _TriggerType: TriggerType, _PreTriggerCnt: PreTriggerCnt,
                 _Threshold: (double)numericUpDownTriggerLevel.Value);

            if (Interpreter.OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerSetRecorderParameters, DecodeRecordData) == false)
            {
                MessageBox.Show(this, "Failed to program the recorder");
            }
            return true;
        }


        public bool BringRecordsVector2Array(int [] SignalIndices , int [] SignalFlags ,  CRecorderPars Pars , out double[][] rslt , out double[] RecordedTime)
        {
            rslt = new double[SignalIndices.Length][];
            RecordedTime = new double[1]; 
            CGUI_SetRecorderParameters RecorderState = new CGUI_SetRecorderParameters();

            RecorderState.Fill(out byte[] buf, Interpreter.AllocateMessageCounter(), 0);

            if (Interpreter.OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerSetRecorderParameters, DecodeRecordData) == false)
            {
                MessageBox.Show(this, "Failed to fetch data from the recorder");
                return false;
            }

            TotalBlocks = (int)Math.Ceiling(RecorderState.RecLength / 64.0) * SignalIndices.Length;
            if (TotalBlocks < 1)
            {
                _ = MessageBox.Show(this, "Nothing to bring");
                return false;
            }

            bool stat = UploadAllRecordedSignals((int)RecorderState.RecLength, SignalIndices, SignalFlags, null);
            if (!stat)
            {
                MessageBox.Show(this, "Could not upload record");
                return false;
            }


            // Save the results in Matlab and in Excel 
            RecordedTime = new double[MasterPars.RecLength]; 
            for ( int i = 0; i < SignalIndices.Length; i++)
            {
                rslt[i] = new double[MasterPars.RecLength]; 
            }

            for (int cnt = 0; cnt < MasterPars.RecLength; cnt++)
            {
                RecordedTime[cnt] = MasterPars.FastSamplingTime * MasterPars.gap * cnt;
                for (int i = 0; i < SignalIndices.Length; i++)
                {
                    rslt[i][cnt] = RecordedSignals[i][cnt]; 
                }
            }
            return true;
        }


        /* 
         * Locate the signal index given its string name
         */
        public bool LocateSignalsByNames(string [] mysig, out int[] Indices , out int[] Flags)
        {
            Indices = new int[mysig.Length];
            Flags   = new int[mysig.Length];
            for ( int cnt = 0; cnt < mysig.Length; cnt++)
            {
                Indices[cnt] = LocateSignalByName(mysig[cnt], out Flags[cnt]);
                if (Indices[cnt] < 0)
                {
                    return false; 
                }
            }
            return true; 
        }


        /* 
         * Locate the signal index given its string name
         */
        public int LocateSignalByName(string mysig, out int Flags)
        {
            string trimsig = mysig.Trim();
            Flags = 0;
            foreach (var item in data)
            {
                string signal = item.Item3;
                if (signal.Trim() == trimsig)
                {
                    Flags = (int)item.Item2;
                    return (int)item.Item1;
                }
            }
            return -1; // Failed 
        }



        public unsafe int DecodeRecordData(ushort [] buf)
        {
            CAnswer_SetRecorderParameters MasterParsRaw = new CAnswer_SetRecorderParameters();
            fixed (ushort* ptr = buf)
            {
                CAnswer_SetRecorderParameters* sPtr = (CAnswer_SetRecorderParameters*)ptr;
                MasterParsRaw = *sPtr;
                // Test for sanity 
                if (  float.IsNaN(MasterParsRaw.FastSamplingTime) || float.IsNaN(MasterParsRaw.TriggerLevel) || MasterParsRaw.FastSamplingTime < 10e-6 || MasterParsRaw.FastSamplingTime > 1e-2)
                {
                    return -1; 
                }
                if (MasterParsRaw.SlowRecorderMultiplier < 1 )
                {
                    return -1;
                }
                MasterPars = *sPtr;
            }
            return 0; 
        }

        public bool GetRecorderCapacities()
        {
            CGUI_SetRecorderParameters rp = new CGUI_SetRecorderParameters();
            rp.Fill(out byte[] buf, Interpreter.AllocateMessageCounter());
            if (Interpreter.OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerSetRecorderParameters, DecodeRecordData) == false)
            {
                return false;
            }
            RecorderPars.TimeSpan = Math.Max(MasterPars.FastSamplingTime * MasterPars.gap * MasterPars.RecLength, 5.0e-4);
            RecorderPars.TriggerPercent = (100.0 * Math.Min(MasterPars.PreTriggerCnt, MasterPars.RecLength) / Math.Max(1, MasterPars.RecLength));
            return true; 
        } 

        public bool DisplayRecorderCapacities()
        {  
            labelMaxAvailPoints.Text = @"Total Memory Pts. : " + MasterPars.BufLength.ToString();
            numericUpDownTimeSpan.Value = (decimal)RecorderPars.TimeSpan;
            numericUpDownTriggerLevel.Value = (decimal) MasterPars.TriggerLevel;

            numericUpDownMaxPoints.Maximum = MasterPars.BufLength;
            numericUpDownMaxPoints.Value = Math.Min( 10000, MasterPars.BufLength) ;

            numericUpDownTriggerPercent.Value = (decimal)RecorderPars.TriggerPercent; 

            comboBoxTriggerMethod.SelectedIndex = (int) Math.Min(Math.Max( (int)MasterPars.TriggerType,0) , 3 ) ;

            // Select the already selected
            for (int cnt = 0; cnt <16 ; cnt++)
            {
                selected[cnt] = false ;
            }

            return true; 
        }

        public void InitRecList ()
        {
            if (MasterPars.GetSigAttributes(out int[] SigIndex, out int[] flags))
            {
                for (int cnt = 0; cnt < MasterPars.nSignals; cnt++)
                {
                    selected[SigIndex[cnt]] = true;

                }
                RefreshRecList();
                ColorSelectedSigRows();
            }
        }


        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Get the column index of the clicked header
            int columnIndex = e.ColumnIndex;
            DataView view;
            view = dataTable.DefaultView;
            // Get the column name
            string columnName = dataGridViewSigRec.Columns[columnIndex].Name;
            switch (columnIndex)
            {
                default:
                    break;
                case 0: // Index
                    view.Sort = columnName + " ASC";
                    dataGridViewSigRec.DataSource = view.ToTable();
                    break; 
                case 1: // Signal name 
                    view.Sort = columnName + " ASC";
                    break;
                case 2: // Group name 
                    view.Sort = columnName + " ASC, Signal ASC";
                    break;
            }
            dataGridViewSigRec.DataSource = view.ToTable();
            dataGridViewSigRec.Refresh(); 
            ColorSelectedSigRows();



        }

        void RefreshRecList()
        {
            selectTable.Clear();
            for ( int cnt = 0; cnt < selected.Count; cnt++)
            {
                if ( selected[cnt] )
                {
                    DataRow row = selectTable.NewRow();
                    row["Index"] = dataTable.Rows[cnt].ItemArray[0] ;
                    row["Signal"] = dataTable.Rows[cnt].ItemArray[1];
                    selectTable.Rows.Add(row);
                    if (selectTable.Rows.Count >= 16  )
                    {
                        break; 
                    }
                }
            }
            dataGridViewSelected.Refresh();
        }

        private void SigCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int rowIndex; // Row index of the clicked cell
                try
                {
                    rowIndex = (int)dataGridViewSigRec.Rows[e.RowIndex].Cells[0].Value;
                    selected[rowIndex] = !selected[rowIndex];

                    RefreshRecList();
                }
                catch
                {

                }
            }
            ColorSelectedSigRows(); 
        }

        void ColorSelectedSigRows()
        {
            System.Drawing.Color color;

            for (int cnt = 0; cnt < selected.Count; cnt++ )
            {
                // Set the background color for the entire row
                int rowIndex ; // Row index of the clicked cell
                rowIndex = (int)dataGridViewSigRec.Rows[cnt].Cells[0].Value;
                color = !selected[rowIndex] ? System.Drawing.Color.White : System.Drawing.Color.LightBlue;
                foreach (DataGridViewCell cell in dataGridViewSigRec.Rows[cnt].Cells)
                {
                    cell.Style.BackColor = color; // Change to desired color
                }
            }

        }

        private void ListCellClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        public void CheckRecorderState(bool RecorderReady, bool RecorderWaitTrigger)
        {
            checkBoxRecorderReady.Checked = RecorderReady;
            checkBoxRecorderArmed.Checked = !RecorderReady & RecorderWaitTrigger; 
        }


        private void ButtonSelectRecorderRsltFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select result storage folder"; // Optional: Set a description
                folderDialog.ShowNewFolderButton = true;     // Allow creating new folders
                folderDialog.SelectedPath = Path.GetFullPath(ResultStoragePath);
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                // Show the dialog
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    ResultStoragePath = folderDialog.SelectedPath; // Retrieve the folder path
                    labelFilePath.Text = ResultStoragePath;
                }
            }
        }

        CGUI_GetRecordedSignal GUI_GetRecordedSignal = new CGUI_GetRecordedSignal();


        double [] TempRecorderVec  ;
        int FillIndex  = 0;
        int TotalLength = 0; 


        unsafe public int ReadNullVec(ushort[] u)
        {
            return -1; 
        }
        unsafe public int ReadLongVec(ushort[] u)
        {
            int ReadLen = Math.Min(u[6] / 4 - 5, TotalLength- FillIndex);
            fixed (ushort* uPtr = &u[8])
            {
                int* lPtr = (int*)uPtr;
                for (int cnt = 0; cnt < ReadLen; cnt++)
                {
                    TempRecorderVec[FillIndex++] = *lPtr++;
                }
            }
            return 0;
        }

        //float[] FloatVec = new float[256];

        unsafe public int ReadFloatVec(ushort[] u)
        {
            int ReadLen = Math.Min(u[6]/4-5, TotalLength - FillIndex);

            fixed (ushort*uPtr = &u[8])
            {
                float* fPtr = (float*)uPtr;  
                for (int cnt = 0; cnt < ReadLen; cnt++)
                {
                    TempRecorderVec[FillIndex++] = *fPtr++ ;
                }
            }
            return 0;
        }

        unsafe public int ReadULongVec(ushort[] u)
        {
            int ReadLen = Math.Min(u[6] / 4 - 5, TotalLength - FillIndex);

            fixed (ushort* uPtr = &u[8])
            {
                uint* lPtr = (uint*)uPtr;
                for (int cnt = 0; cnt < ReadLen; cnt++)
                {
                    TempRecorderVec[FillIndex++] = *lPtr++;
                }
            }
            return 0;
        }

        unsafe public int ReadShortVec(ushort[] u)
        {
            int ReadLen = Math.Min(u[6] / 4 - 5, TotalLength - FillIndex);

            fixed (ushort* uPtr = &u[8])
            {
                for (int cnt = 0; cnt < ReadLen; cnt++)
                {
                    TempRecorderVec[FillIndex++] =  uPtr[cnt*2];
                }
            }
            return 0;
        }

        unsafe public int ReadUShortVec(ushort[] u)
        {
            int ReadLen = Math.Min(u[6] / 4 - 5, TotalLength - FillIndex);

            fixed (ushort* uPtr = &u[8])
            {
                short* sPtr = (short*)uPtr; 
                for (int cnt = 0; cnt < ReadLen; cnt++)
                {
                    TempRecorderVec[FillIndex++] = sPtr[cnt * 2];
                }
            }
            return 0;
        }

        private bool UploadAllRecordedSignals (int length, int[] SignalIndices, int [] SignalFlags , ProgressBar bar   )
		{ 
            for (int SigIndex = 0; SigIndex < SignalIndices.Length; SigIndex ++)
            {
                //int index =  SignalIndices[SigIndex] ;
                if ( ! UploadRecordedSignal(SigIndex, SignalFlags[SigIndex], length, bar) )
                {
                    return false;
                }
            }
            return true; 
        }


        private bool UploadRecordedSignal( int SigIndex   , int Flag , int length , ProgressBar bar   )
        {
            int AcceptedLength = 0 ;
            if (Flag > 12 )
            {
                return false; 
            }

            TempRecorderVec = new double[length];
            TotalLength = length;
            FillIndex = 0;
            while (AcceptedLength < length)
            {
                int NextLength = Math.Min(AcceptedLength + MessagePayloadCount , length) - AcceptedLength ;
                GUI_GetRecordedSignal.Fill(SigIndex, AcceptedLength, AcceptedLength + NextLength - 1, Interpreter.AllocateMessageCounter(), out byte [] buf);
                Interpreter.OfflineTransaction(100, out int ErrorCode,  buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerGetRecordedSignal,GetRecordData[Flag]);
                AcceptedLength = NextLength + AcceptedLength;
                if (ErrorCode != 0 )
                {
                    return false;
                }
                // Copy results to the vector
                
                UploadedBlocks = Math.Min( UploadedBlocks +  1, TotalBlocks) ;
                if ( bar != null) bar.Value = (int)(100.0 * UploadedBlocks / TotalBlocks);
				//progressBarUpload.Value = (int) (100.0 * UploadedBlocks / TotalBlocks);
            }
            RecordedSignals[SigIndex] = new double[length];
            Array.Copy(TempRecorderVec, RecordedSignals[SigIndex], length);
            return true; 
        }


        void GetGUISignalsToRecord(out int[] signals)
        {
            // Issue recorder 
            //CGUI_SetRecorderParameters RecorderRequest = new CGUI_SetRecorderParameters();
            int nSignals = selectTable.Rows.Count;
            signals = new int[nSignals];
            for (int cnt = 0; cnt < nSignals; cnt++)
            {
                signals[cnt] = Convert.ToInt32(selectTable.Rows[cnt]["Index"]);
            }
        }

        private void Click_DoRecord(object sender, EventArgs e)
        {
            GetGUISignalsToRecord(out int[] signals);
            InitiateRecorder(signals); 
        }

        public void InitiateRecorder(int [] signals)
        {
            int nSignals = signals.Length; 

            if (nSignals == 0)
            {
                MessageBox.Show(this, "No signals to record");
                return;
            }

            int maxpoints = (int)((double)MasterPars.BufLength / nSignals);

            maxpoints = (int) Math.Min( Math.Max(3, numericUpDownMaxPoints.Value) , maxpoints ) ;
            double TimeSpan = (double)numericUpDownTimeSpan.Value;

            int gap = (int) Math.Ceiling( TimeSpan / (maxpoints * MasterPars.FastSamplingTime) );
            int TriggerType = comboBoxTriggerMethod.SelectedIndex;
            int PreTriggerCnt = (int)(maxpoints * 0.01 * (double)numericUpDownTriggerPercent.Value ); 

            maxpoints = (int)(TimeSpan / ( MasterPars.FastSamplingTime * gap ) );

            RecorderRequest.Fill(out byte[] buf, Interpreter.AllocateMessageCounter(), nSignals, signals, maxpoints,
                 _RecorderGap: gap, _TimeBasis: 0, _TriggerType: TriggerType, _PreTriggerCnt: PreTriggerCnt,
                 _Threshold: (double)numericUpDownTriggerLevel.Value);

            if (Interpreter.OfflineTransaction(100, out int  ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerSetRecorderParameters, DecodeRecordData) == false)
            {
                MessageBox.Show( this ,"Failed to program the recorder") ;
            }

        }

        int TotalBlocks = 1;
        int UploadedBlocks = 1;


        private void Click_BringRecorderData(object sender, EventArgs e)
        {
            progressBarUpload.Value = 0;
            UploadedBlocks = 0;
            if (!checkBoxRecorderReady.Checked)
            {
                MessageBox.Show(this, "Recorder is not ready \ncant bring contents");
                return;
            }
			
            // File name for data storage 
            string basename = labelFilePath.Text + @"\" + textBoxRecFileName.Text;

            // Get the signal indices and names 
            int[] SignalIndices = new int[selectTable.Rows.Count];
			string[] SignalNames = new string[selectTable.Rows.Count];
            int[] SignalFlags = new int[selectTable.Rows.Count];
			for (int c1 = 0; c1 < selectTable.Rows.Count; c1++)
			{
				SignalIndices[c1] = Convert.ToInt32(selectTable.Rows[c1].ItemArray[0]);
				SignalNames[c1] = selectTable.Rows[c1].ItemArray[1].ToString();
				SignalFlags[c1] = Convert.ToInt32(selectTable.Rows[c1].ItemArray[2]);
			}


			BringRecorderData(basename, SignalIndices , SignalNames , SignalFlags , progressBarUpload); 

		} 

        public bool BringRecorderData(string basename, int[] SignalIndices , string [] SignalNames , int [] SignalFlags, ProgressBar bar = null)
        { 
            CGUI_SetRecorderParameters RecorderState = new CGUI_SetRecorderParameters();
            
            RecorderState.Fill(out byte [] buf, Interpreter.AllocateMessageCounter(), 0);

            if (Interpreter.OfflineTransaction(100, out int ErrorCode, buf, (int)Literals.Reply2GUIOpCodes.GUIOpCode_AnswerSetRecorderParameters, DecodeRecordData) == false)
            {
                MessageBox.Show(this, "Failed to fetch data from the recorder");
                return false; 
            }

            TotalBlocks = (int)Math.Ceiling(MasterPars.RecLength / 64.0) * MasterPars.nSignals;
            if (TotalBlocks < 1)
            {
                _ = MessageBox.Show(this, "Nothing to bring");
                return false;
            }

            bool stat = UploadAllRecordedSignals((int)MasterPars.RecLength,SignalIndices, SignalFlags ,bar);
            if ( !stat )
            {
                MessageBox.Show(this,"Could not upload record");
                return false; 
            }


            // Save the results in Matlab and in Excel 
            double[][] t = new double[1][];

            t[0] = new double[MasterPars.RecLength];
			RecordedTime = new double[MasterPars.RecLength]; 

			for ( int cnt = 0; cnt < MasterPars.RecLength; cnt++)
            {
				RecordedTime[cnt] =  MasterPars.FastSamplingTime * MasterPars.gap * cnt;
                t[0][cnt] = RecordedTime[cnt];
			}

            // Save both as Matlab and as Excel 
            string matfilename = basename + @".mat";
            string xlsfilename = basename + @".xlsx";
            //CSMatIO 
            MLDouble ml_t = new MLDouble("t", t);
            List<MLArray> mlList = new List<MLArray>();
            mlList.Add(ml_t);

            double[][][] TempStore = new double[SignalIndices.Length][][]; 
            for ( int cnt = 0; cnt < SignalIndices.Length; cnt++)
            {
                TempStore[cnt] = new double[1][];
                TempStore[cnt][0] = new double[MasterPars.RecLength];
                for ( int c1 = 0; c1 < MasterPars.RecLength; c1++)
                {
                    TempStore[cnt][0][c1] = RecordedSignals[cnt][c1];// Fill the vector that fits the row with signal 
                }
                int index = Convert.ToInt32(SignalIndices[cnt] ); // Signal index 
                string SigName = data[index].Item3;
                mlList.Add(new MLDouble(SigName, TempStore[cnt]));
            }

            MatFileWriter mfw = new MatFileWriter(matfilename, mlList, false);

            // Write into the Excel 
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet named "Results"
                var worksheet = workbook.Worksheets.Add("Results");

                // Define headers

                worksheet.Cell(1, 1).Value = "Time";
                for (int c1 = 0; c1 < SignalIndices.Length; c1++)
                {
                    int index = Convert.ToInt32(SignalIndices[c1]);
                    string next = SignalNames[c1]  ; 
                    worksheet.Cell(1, c1 + 2).Value = next;
                }


                // Populate the worksheet with data
                for (int row = 0; row < MasterPars.RecLength; row++)
                {
                    worksheet.Cell(row + 2, 1).Value = t[0][row];
                    for (int c1 = 0; c1 < SignalIndices.Length; c1++)
                    {
                        worksheet.Cell(row + 2, c1+2).Value = RecordedSignals[c1][row];
                    }
                }

                // Format the table
                worksheet.Columns().AdjustToContents(); // Adjust column widths

                // Save the workbook to the file path
                workbook.SaveAs(xlsfilename);
            }

            //MessageBox.Show(this, "Matlab and Excel files \nSuccessfuly saved");
            return true; 
        }

        private void Click_checkBoxClearAll(object sender, EventArgs e)
        {
            for ( int cnt = 0; cnt < selected.Count ; cnt++)
            {
                selected[cnt] = false;
            }
            RefreshRecList();
            ColorSelectedSigRows();
            checkBoxClearAll.Checked = false;
        }
    }

    public struct CRecorderPars
    {
        public double TimeSpan;

        public double TriggerPercent;

        public int MaxPoints;
        public int TriggerMethod; 
    }
}
