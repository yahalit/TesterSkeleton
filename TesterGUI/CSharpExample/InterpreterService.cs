using System.IO.Ports;
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
using System.Globalization;

namespace PvsGUI
{
    public class CMsgTrap
    {
        public Func<ushort[], int> handler;
        public ushort OpCode;
        public uint MsgCntr;
        public DateTime IssueTime;
        public DateTime tOut;
        public int status;
        public CMessage msg;
        public CMsgTrap(ushort opcode_in, Func<ushort[], int> handler_in, uint cntr ,double tOutSec = 10000.0)  // Vandal  0.1  )
        { 
            handler = handler_in;
            OpCode = opcode_in;
            MsgCntr = cntr;
            IssueTime = DateTime.Now ;
            tOut = IssueTime.AddSeconds(tOutSec);
            status = 0; // Unserviced 
        }
    }
    public partial class CInterpreter
    {


        public void Flush()
        {
            VerMsgNew = false;
            PeriodicUpdateAvailable = false; 
            HostCom.FlushCommunication(); 
        }

        public DateTime UlongToDateTime(ulong microsecondsSinceMidnight)
        {
            DateTime todayMidnight = DateTime.Today; // Midnight of the current date
            // Convert microseconds to ticks (1 microsecond = 10 ticks)
            long ticks = (long)(microsecondsSinceMidnight * 10);
            // Create a TimeSpan from the ticks
            TimeSpan timeSinceMidnight = TimeSpan.FromTicks(ticks);
            // Add the TimeSpan to today's midnight to get the final DateTime
            DateTime result = todayMidnight.Add(timeSinceMidnight);
            return result;
        }
        public ulong GetTimeUsec()
        {
            DateTime currentTime = DateTime.Now;
            ulong Time = (ulong)(currentTime.Millisecond * 1000.0 + currentTime.Second * 1000000.0
                + currentTime.Minute * 60000000.0 + currentTime.Hour * 3.6000e+09);
            return Time;
        }

        public string ExceptionText(int expnum, bool AddDetail = true )
        {
            int errnum =  ExpNum.IndexOf(expnum);
            if (errnum < 0)
            {
                return $"Undefined Exception {expnum}";
            }
            string retval = ExpText[errnum]; 
            if ( AddDetail)
            {
                retval = retval + "\n" + ExpDetail[errnum];
            }
            return retval;
        }

        public bool ParseExceptionStatements(string filePath, out List<string> exptext, out List<string> expdetail, out List<int> expnum)
        {
            exptext = new List<string>();
            expnum = new List<int>();
            expdetail = new List<string>();


            if (!File.Exists(filePath))
            {
                return false; //  throw new FileNotFoundException($"The file {filePath} does not exist.");
            }

            foreach (var line in File.ReadLines(filePath))
            {
                string [] words = Regex.Split(line, @"\s+");  // Regex.Match(line, definePattern);
                string[] parts = line.Split(new[] { "//" }, 2, StringSplitOptions.None);
                if ((words.Length >= 3) && ( words[0] == "#define" ))
                {
                    string identifier = words[1];
                    string hexString = words[2];
                    int number; 
                    if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        hexString = hexString.Substring(2);
                        number = int.Parse(hexString, NumberStyles.HexNumber);
                    }
                    else
                    {
                        number = int.Parse(hexString);
                        if (number < 0)
                            number += 65536; 

                    }
                    // Parse the hexadecimal string

                    exptext.Add(identifier);
                    expnum.Add(number);
                    if ( parts.Length > 1)
                    {
                        expdetail.Add(parts[1]);
                    }
                    else
                    {
                        expdetail.Add("Details not given");
                    }
                }
            }

            return true;
        }
    }

}