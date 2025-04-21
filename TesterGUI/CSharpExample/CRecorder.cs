using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace PvsGUI
{
    public class CRecorder
    {
        string ResultsStorageFolder;
        public bool SigRecFormOpened = false; 
        public SigRec sigrec; 
        public List<Tuple<int, int, string, string, string>> data = new List<Tuple<int, int, string, string, string>>() ;
        public CInterpreter Interpreter = CInterpreter.Instance; //  new CInterpreter(mutex);

        private static readonly CRecorder _instance = new CRecorder();
        public static CRecorder Instance
        {
            get { return _instance; }
        }

        // Private static instance of the class
        // Private constructor to prevent instantiation from outside the class
        private CRecorder( )
        {
            // Console.WriteLine("Singleton instance created");
        }


        public bool PopulateSignalList(string DefFilePath, string resultpath , bool OpenGUI)
        {
            if (!File.Exists(DefFilePath))
            {
                return false; //  throw new FileNotFoundException($"The file {filePath} does not exist.");
            }
            if (!Directory.Exists(resultpath))
            {
                return false; //  throw new FileNotFoundException($"The file {filePath} does not exist.");
            }


            ResultsStorageFolder = resultpath; 

            //string pattern = @".*?\/\/.*?:\s*(\S+)\s*(?:\[(\S+)\])?";
            //string pattern = @"\{\s*(\d).*\/\/\s*:\s*(\S+)?\s*(?:\[(\S+)\])?\s*(?:\{([\S\s]+)\})?"; ;
            string pattern = @"\/\/\s*:\s*(\S+)\s*(?:\[(\S+)\])\s*(?:\{([\S\s]+)\})";
            string pattern2 = @"\{\s*(\d+)";

            int index = 0;
            data.Clear(); 
            foreach (var line in File.ReadLines(DefFilePath))
            {
                Match match = Regex.Match(line, pattern);
                Match matchn = Regex.Match(line, pattern2);
                if ( !(match.Groups[1].Success & match.Groups[2].Success & match.Groups[3].Success & matchn.Groups[1].Success) )
                {
                    if (line.Contains('{'))
                    {
                        MessageBox.Show("Could not interpret a recorder var line \n" + line);
                        return false; 
                    }
                    continue; 
                }
                string name1 = match.Groups[1].Value; // Captured name1
                string name2 = match.Groups[2].Value;
                string name3 = match.Groups[3].Value;
                int.TryParse(matchn.Groups[1].Value, out int flags); 

                data.Add(new Tuple<int, int, string, string, string>(index, flags, name1,name2,name3)); 
                index += 1; 
            }

            sigrec = new SigRec(data, ResultsStorageFolder);

            bool stat = sigrec.GetRecorderCapacities(); 
            if ( stat & OpenGUI)
            {
                stat = sigrec.DisplayRecorderCapacities();
            }
            if ( !stat)
            {
                MessageBox.Show("Recorder cant read from system \nIts volume capabilities");
                return false; 
            }

            if (data.Count != sigrec.GetLengthOfSigList() )
            {
                MessageBox.Show("Recorder descriptor file length \nDoes not match FW in CPU");
                return false;
            }
            return true;
        }

        public void OpenSigRecForm()
        {
            sigrec.FormClosed += (sender, e) =>
            {
                SigRecFormOpened = false;
            };
            sigrec.Show();
            sigrec.InitRecList();

            SigRecFormOpened = true;
        }

        public void CloseSigRecForm( )
        {
            if (sigrec != null && !sigrec.IsDisposed)
            {
                sigrec.Close();
                SigRecFormOpened = false;
            }

        }


        public bool WaitRecorderReady(double dt)
        {

            DateTime startTime = DateTime.Now;
            TimeSpan timeout = TimeSpan.FromSeconds(dt);
            while (DateTime.Now - startTime < timeout)
            {
                if (!Interpreter.GetSimulatorState())
                {
                    return false;
                }
                if (Interpreter.SystemBIT.RecorderReady )
                {
                    return true; 
                }
            }
            return false; 
        }


        public void SetRecorderState( bool RecorderReady, bool RecorderWaitTrigger)
        {
            if (SigRecFormOpened )
            {
                try
                {
                    sigrec.CheckRecorderState(RecorderReady,RecorderWaitTrigger );
                }
                catch { }; 
            }
        }

    }
}
