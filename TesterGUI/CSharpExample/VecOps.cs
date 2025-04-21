using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Management;
using System.Drawing;
using MathNet.Numerics;
using ZedGraph;
using DocumentFormat.OpenXml.Drawing.Charts;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.ComTypes;
using Plot;
using DocumentFormat.OpenXml.Bibliography;


namespace PvsGUI
{

    /*
    * Routines: 
    * class VecOps: 
    * Any:  Find if a bool array contains any true
    * MaxAbs:  Find the maximum absolute value in a vector
    * Rms:  Find RMS of values in a vector
    * LinearRegression: Fit a line coefficients[0] + coefficients[1] * xdata approx ydata
    * CheckLinearRegression: Check how well a linear relation fits xdata to ydata. 
    * CheckUnityRegression: Check how well xdata matches ydata. 
    * CheckConstantFit: Check how well xdata * level matches ydata. 
    * MeanOfRecords: For a jagged array of measurement sets, find the mean measurements set
    * IsDigitalLow :  Does an analog voltage represent a Digital 0 (below 0.25 of logic supply)
    * IsDigitalHigh: Does an analog voltage represent a Digital 1 (above 0.6 of logic supply)
    * class Gadgets
    * SetLedColor: Set the color for an indication LED to a color that may be "GREEN", "RED", "BLUE" or "GRAY"
    * CheckLegitimateAtrFileName: Test that an ATR Excel file name is legitimate, and parse its attributes 
    * IsExcelFileAccessible: Verify if a an excel file of a given name may be opened
    * SelectExcelFile: Select an Excel file from a given folder by an Open File dialog
    * UpdateAvailableComPorts: Analyze into a conbo box all the COM ports connected to a PC and analyze them 
    * CalculateDropdownWidth: Calculate the width of a combo box required to display all its strings
    * IncrementFileVersion: For a given ATR file name, increment the version field
    * AddToolTip2Control: Add a tool tip explanation to a windows control
    * CreateZedGraphChart:  Draw a Graph and save it as PNG
    * InsertImageIntoExcel:  Put a PNG image into an Excel file 
    * PrintExcel: Print the Excel ATR file as PDF 
    */

    /* 
     * 
     */
    public struct AtrAttrib
    {
        public int serialNumber; // EUT serial number
        public int year ; // Test year 
        public int month ; // Test month 
        public int day; // Test day 
        public int revision ; // Atr file revision 
    }


    public struct CComId
    {
        public string PID; // Product ID 
        public string VID; // Vendor ID 
        public string SN;  // Serial number
        public string Name; // COM1, COM3, COM7, etc. 
        public string DeviceID; // A unique string to identify a specidic device 
        public string Description; // Something like "FTDI USB Serial Port" or "Silicon Labs CP210x USB to UART Bridge (COM3)" 
    }


    public static class VecOps
    {
        /* 
         * Any:  Find if a bool array contains any true
         */
        public static bool Any(bool[] vec)
        {
            for (int cnt = 0; cnt < vec.Length; cnt++)
            {
                if (vec[cnt]) return true;
            }
            return false;
        }
        /* 
        * MaxAbs:  Find the maximum absolute value in a vector
        */
        public static double MaxAbs(double[] vec)
        {
            double Av = 0 ; 
            for (int cnt = 0; cnt < vec.Length; cnt++)
            {
                Av = Math.Max( Av , Math.Abs(vec[cnt])); 
            }
            return Av;
        }

        /* 
        * Rms:  Find RMS of values in a vector
        */
        public static double Rms(double[] vec)
        {
            double Av = 0;
            if ( vec.Length > 0) 
            {
                for (int cnt = 0; cnt < vec.Length; cnt++)
                {
                    Av = vec[cnt] * vec[cnt];
                }
                Av = Math.Sqrt(Av/ vec.Length); 
            }
            return Av;
        }

        /* 
         * LinearRegression: Fit a line coefficients[0] + coefficients[1] * xdata approx ydata
         * Returns: true if ok, false if there is not enough data to draw a line 
         */
        public static bool LinearRegression(double[] xdata, double[] ydata, out double[] coefficients)
        {
            coefficients = new double[2];

            if (xdata.Length < 2 || (xdata.Length != ydata.Length))
            { return false; }

            bool dataok = false;
            for (int cnt = 1; cnt < xdata.Length; cnt++)
            {
                if (Math.Abs(xdata[cnt] - xdata[0]) > 1e-9)
                {
                    dataok = true;
                    break;
                }
            }
            if (!dataok) { return false; }

            coefficients = Fit.Polynomial(xdata, ydata, 1);
            return true; 
        }


        /* 
         * CheckLinearRegression: Check how well a linear relation fits xdata to ydata. 
         * Let e =  ydata - (coefficients[0] + coefficients[1] * xdata)
         * Returns:  maxerr = max( abs(e)) ,  rmserr = rms(e) 
         */
        public static void CheckLinearRegression(double[] xdata, double[] ydata, double[] coefficients, out double maxerr, out double rmserr)
        {
            int nData = ydata.Length;
            maxerr = 0;
            rmserr = 0;
            double[] Fitted = new double[nData];
            double[] FitError = new double[nData];
            for (int cnt = 0; cnt < nData; cnt++)
            {
                Fitted[cnt] = coefficients[0] + coefficients[1] * xdata[cnt];
                FitError[cnt] = ydata[cnt] - Fitted[cnt];
                maxerr = Math.Max(maxerr, Math.Abs(FitError[cnt]));
                rmserr += FitError[cnt] * FitError[cnt];
            }
            rmserr = Math.Sqrt(rmserr / nData);
        }


        /* 
         * CheckUnityRegression: Check how well xdata matches ydata. 
         * Let e =  ydata - xdata
         * Returns:  maxerr = max( abs(e)) ,  rmserr = rms(e) 
         */
        public static void CheckUnityRegression(double[] xdata, double[] ydata, out double maxerr, out double rmserr)
        {
            double[] coefficients = new double[] { 0,1};
            CheckLinearRegression(xdata, ydata, coefficients, out maxerr, out rmserr);
        }

        /* 
         * CheckConstantFit: Check how well xdata * level matches ydata. 
         * Let e =  ydata - level * xdata
         * Returns:  maxerr = max( abs(e)) ,  rmserr = rms(e) 
         */
        public static void CheckConstantFit(double[] xdata, double[] ydata, double level , out double maxerr, out double rmserr)
        {
            double[] coefficients = new double[] { level , 0 };
            CheckLinearRegression(xdata, ydata, coefficients, out maxerr, out rmserr);
        }

        /* 
         * MeanOfRecords: For a jagged array of measurement sets, find the mean measurements set
         * arguments: recs: a set of double [] 
         * returns: mean: Their average, mean[k] = average( recs[0][k],recs[1][k],...) 
         */

        public static void MeanOfRecords(double[][] recs, out double[] mean)
        {
            mean = new double[recs.Length];
            for (int cnt = 0; cnt < recs.Length; cnt++)
            {
                if (recs[cnt].Length > 0)
                {
                    double Av = 0;
                    for (int i = 0; i < recs[cnt].Length; i++)
                    {
                        Av += recs[cnt][i];
                    }
                    mean[cnt] = Av / recs[cnt].Length;
                }
            }
        }


        /* 
         * IsDigitalLow: Does an analog voltage represent a Digital 0 (below 0.25 of logic supply)
         * Arguments: 
         * a: Tested value 
         * reference: logic supply voltage
         * Returns: decision 
         */
        public static bool IsDigitalLow(double a, double reference = 3.3)
        {
            return ( (a > -0.2) & (a < reference * 0.25) ); 
        }

        /* 
         * IsDigitalHigh: Does an analog voltage represent a Digital 1 (above 0.6 of logic supply)
         * Arguments: 
         * a: Tested value 
         * reference: logic supply voltage
         * Returns: decision 
         */
        public static bool IsDigitalHigh(double a, double reference = 3.3)
        {
            return ((a > reference * 0.6)  & (a < reference * 1.2) );
        }

    }


    public static class Gadgets
    {
        /* 
         * SetLedColor: Set the color for an indication LED to a color that may be "GREEN", "RED", "BLUE" or "GRAY"
         */
        public static void SetLedColor(PictureBox pictureBoxInControl, string color)
        {
            // Dispose of the old image to free up resources (if needed)
            pictureBoxInControl.Image?.Dispose();

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

        /* 
         * OpenInExcel: Open an Excel file by launching an Excel application  
         * filePath: Path of file to open 
         * Returns: 
         * errmsg, "Ok", or error message id file could not be opened 
         */
        public static bool OpenInExcel(string filePath , out string errmsg)
        {
            errmsg = "Ok"; 
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = $"\"{filePath}\" /e", // Pass the file path as an argument to Excel
                    UseShellExecute = true // Ensures the file opens with the default application (Excel)
                });

                return true; 
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false; 
            }
        }


        /* 
         * CheckLegitimateAtrFileName: Test that an ATR Excel file name is legitimate, and parse its attributes 
         */
        static public bool CheckLegitimateAtrFileName(string input, out string errmsg , out AtrAttrib attr)
        {
            bool RetVal = true ; 
            string pattern = @"^PVS_ATR_SN_(\d+)_(\d{4})_(\d{2})_(\d{2})_R(\d+)$";
            attr = new AtrAttrib();
            if ( !input.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) )
            {
                errmsg = "File exstention must be .xlsx";
                return false; 
            }
            input = input.Substring(0, input.Length - 5);

            Match match = Regex.Match(input, pattern);
            errmsg = ""; 
            if (match.Success)
            {
                try
                {
                    attr.serialNumber = int.Parse(match.Groups[1].Value);
                    attr.year = int.Parse(match.Groups[2].Value);
                    attr.month = int.Parse(match.Groups[3].Value);
                    attr.day = int.Parse(match.Groups[4].Value);
                    attr.revision = int.Parse(match.Groups[5].Value);
                    if (attr.serialNumber < 1 || attr.serialNumber > 100000)
                    {
                        errmsg = "SerialNumber out of range";
                        RetVal = false;
                    }
                    if (attr.year < 2000 || attr.year > 2200)
                    {
                        errmsg = "Year out of range";
                        RetVal = false;
                    }
                    if (attr.month < 1 || attr.month > 12)
                    {
                        errmsg = "Month out of range";
                        RetVal = false;
                    }
                    if (attr.day < 1 || attr.day > 31)
                    {
                        errmsg = "Day of month out of range";
                        RetVal = false;
                    }
                    if (attr.revision < 0 || attr.revision > 1000)
                    {
                        errmsg = "Revision out of range";
                        RetVal = false;
                    }
                }
                catch
                {
                    errmsg = "Failed to parse numeric fields";
                    RetVal = false;
                }
            }
            else
            {
                errmsg = "Bad format";
                RetVal = false;
            }
            return RetVal;
        }

        /* 
         * IsExcelFileAccessible: Verify if an Excel file of a given name may be opened
         * filePath: Path of file to open 
         * Returns: 
         * return value: true if file may be opened, false otherwise
         * msg: Error message if false return
         */
        static public bool IsExcelFileAccessible(string filePath, out string msg )
        {
            // Check if the file exists
            msg = "Ok"; 
            if (!File.Exists(filePath))
            {
                msg = "File does not exist.";
                return false;
            }

            // Check if the file has a valid Excel extension
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".xlsx" )
            {
                msg = "File is not an Excel file.";
                return false;
            }

            // Attempt to open the file to check accessibility
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // Successfully opened the file
                }
                return true;
            }
            catch (IOException ex)
            {
                msg = $"File is locked or inaccessible: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                msg = $"Unexpected error: {ex.Message}";
                return false;
            }
        }


        /* 
         * SelectExcelFile: Select an Excel file from a given folder by an Open File dialog
         * Arguments: 
         * folderPath: The folder where to look for the file 
         * Returns
         * return value: true if succesful
         * filename    : if true, the name of selected file 
         */
        public static bool SelectExcelFile(string folderPath , ref string filename)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = folderPath;
                openFileDialog.Filter = "Excel Files(*.xlsx) | *.xlsx";
                openFileDialog.Title = "Select an Excel File";
                openFileDialog.CheckFileExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filename = Path.GetFileName(openFileDialog.FileName); // File name with extension
                    string directoryPath = Path.GetDirectoryName(openFileDialog.FileName); // Directory path
                    if ( !folderPath.Equals(directoryPath) )
                    {
                        return false;
                    }
                    return true; 
                }
            }

            return false; // Return null if no file was selected
        }




        /* 
         * Analyze all the COM ports connected to a PC and analyze them 
         * Parameters: 
         * mustBeUsb: true: search only USB peripherals 
         * 
         */
        public static bool FindCOMParameters ( out List<CComId> ComId, out string msg , bool mustBeUsb = true )
        {
            ComId = new List<CComId>();
            msg = "Ok"; 
            try
            {
                // Query WMI for all COM ports
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");
                foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    // Display basic COM port information
                    CComId Id = new CComId
                    {
                        Name = ExtractFromParentheses((string)queryObj["Name"]),
                        DeviceID = (string)queryObj["DeviceID"],
                        Description = (string)queryObj["Description"]
                    };

                    if (mustBeUsb && !Id.Description.Contains("USB") )
                    {
                        continue;
                    }

                    // Retrieve PNPDeviceID which contains VID, PID, and serial number

                    if (!string.IsNullOrEmpty(Id.DeviceID))
                    {
                        // Parse VID, PID, and Serial Number
                        Id.VID = ExtractProperty(Id.DeviceID, "VID_");
                        Id.PID = ExtractProperty(Id.DeviceID, "PID_");
                        Id.SN  = ExtractSerialNumber(Id.DeviceID);
                    }
                    else
                    {
                        Id.VID = "Unknown";
                        Id.PID = "Unknown";
                        Id.SN = "Unknown"; 
                    }

                    ComId.Add(Id);
                }
                return true; 
            }
            catch (Exception ex)
            {
                msg = $"An error occurred: {ex.Message}";
                return false; 
            }

        }



        // Helper method for FindCOMParameters() to extract VID or PID of a COM port
        static string ExtractProperty(string input, string propertyName)
        {
            int startIndex = input.IndexOf(propertyName);
            if (startIndex >= 0)
            {
                startIndex += propertyName.Length;
                int endIndex = input.IndexOf('&', startIndex);
                if (endIndex > startIndex)
                    return input.Substring(startIndex, endIndex - startIndex);
                endIndex = input.IndexOf('+', startIndex);
                if (endIndex > startIndex)
                    return input.Substring(startIndex, endIndex - startIndex);
            }
            return "N/A";
        }

        // Helper method for FindCOMParameters() to extract VID or PID of a COM port
        static string ExtractFromParentheses(string input)
        {
            int startIndex = input.LastIndexOf('(');
            int EndIndex = input.LastIndexOf(')');
            if (startIndex >= 0 && EndIndex > startIndex+1)
            {
                return input.Substring(startIndex+1, EndIndex - startIndex-1);
            }
            return "N/A";
        }

        // Helper method for FindCOMParameters()  to extract Serial Number of a COM port 
        static string ExtractSerialNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "NA";

            int startIndex = input.LastIndexOf('\\');

            // Find the start of "+Something"
            int plusIndex = input.LastIndexOf('+');
            if (plusIndex == -1)
            {
                if (startIndex >= 0 && startIndex < input.Length - 1)
                    return input.Substring(startIndex + 1);
                return "NA";
            }

            // Find the start of "\\Unimportant"
            int backslashIndex = input.IndexOf('\\', plusIndex);
            if (backslashIndex == -1)
            {
                if (startIndex >= 0 && startIndex < input.Length - 1)
                    return input.Substring(startIndex + 1);
                return "NA";
            }

            // Extract the "Something" substring
            return input.Substring(plusIndex + 1, backslashIndex - (plusIndex + 1));
        }

        /* 
        * UpdateAvailableComPorts: Analyze into a conbo box all the COM ports connected to a PC and analyze them 
        * Parameters: 
        * combo: the combo box to update 
        * 
        */
        static public void UpdateAvailableComPorts(ComboBox combo)
        {
            //int selindex = Math.Max(combo.SelectedIndex, 0);
            List<string> ports = new List<string>();
            if (Gadgets.FindCOMParameters(out List<CComId> ComId, msg: out string _))
            {
                for (int cnt = 0; cnt < ComId.Count; cnt++)
                {
                    ports.Add(ComId[cnt].Name + ":" + ComId[cnt].SN );
                }
            }
            if (ports.Count == 0 ) 
            {
                ports.Add("None");
            }
            // Test if anything in the combo changed. If so reset the selection
            int selindex = 0 ;
            if (combo.Items.Count > 0)
            {
                selindex = Math.Max(ports.IndexOf(combo.SelectedItem.ToString()), 0);
            } 
            combo.Items.Clear();
            foreach (string port in ports)
            {
                combo.Items.Add(port);
            }

            combo.DropDownWidth = CalculateDropdownWidth(combo);
            combo.SelectedIndex = Math.Min(selindex, combo.Items.Count - 1);
        }

        /* 
         * CalculateDropdownWidth: Calculate the width of a combo box required to display all its strings
         * Parameter
         * The combo with strings included 
         * Return: pixel width 
         */
        static public int CalculateDropdownWidth(ComboBox comboBox)
        {
            int maxWidth = comboBox.Width;
            using (Graphics g = comboBox.CreateGraphics())
            {
                foreach (var item in comboBox.Items)
                {
                    int itemWidth = (int)g.MeasureString(item.ToString(), comboBox.Font).Width;
                    if (itemWidth > maxWidth)
                    {
                        maxWidth = itemWidth;
                    }
                }
            }
            return maxWidth;
        }

        /* 
         * IncrementFileVersion: For a given ATR file name, increment the version field
         * returns: 
         * true if sucessful, false if failed to analyze for version 
         */
        public static bool IncrementFileVersion(ref string fileName)
        {
            bool RetVal = true;
            Regex regex = new Regex(@"_R(\d+)\.xlsx$", RegexOptions.IgnoreCase);
            try
            {
                string result = regex.Replace(fileName, match =>
                {
                    // Extract the number and increment it
                    int number = int.Parse(match.Groups[1].Value);
                    int incrementedNumber = number + 1;

                    // Replace with incremented number
                    return $"_R{incrementedNumber}.xlsx";
                });
                if(result.Equals(fileName))
                {
                    RetVal = false;
                }
                fileName = result;
            }
            catch
            {
                RetVal = false;
            }
            return RetVal;
        }

        /* 
         *AddToolTip2Control: Add a tool tip explanation to a windows control
         *Arguments: 
         * control: The control to enhance with tool tip help 
         * lines  : The lines of help 
         */
        public static bool AddToolTip2Control(System.Windows.Forms.Control control, string[] lines )
        {
            bool RetVal = true; 
            try
            {
                System.Windows.Forms.ToolTip toolTipSelect = new System.Windows.Forms.ToolTip
                {
                    AutoPopDelay = 10000,     // Tooltip visible for 10 seconds
                    InitialDelay = 500,      // Delay before appearing
                    ReshowDelay = 500,       // Delay when switching controls
                    ShowAlways = true        // Show tooltip even if the form is inactive
                };
                string caption = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    caption += lines[i]; 
                    if ( i < lines.Length - 1)
                    {
                        caption +=  " \n ";
                    }
                } 
                toolTipSelect.SetToolTip(control,caption);

            }
            catch 
            { 
                RetVal = false; 
            }
            return RetVal; 
        }
    }

static class XLSGraph
    {
        public struct GrAttributes
        {
            public string Title;  // Graph title
            public string XLabel; // a axis label 
            public string YLabel; // y axis label 
            public string[] CurveNames; // A legend name for each curve 
            public System.Drawing.Color [] Colors; // A color for each curve 
            public int [] Selector; // Selects which of the available curves to draw, Selector[n]=k , k < number of available curves, will select the k'th curve to draw
        }

        /* 
         * CreateZedGraphChart:  Draw a Graph and save it as PNG
         * xValues: Vector to serve as x values 
         * yValues: Array of vectors to serve as y values
         * atr    : Graph attributes, see struct definition 
         * imagePath: File name to store the image 
         */
        public static void CreateZedGraphChart(double [] xValues, double [][] yValues, GrAttributes atr , string imagePath)
        {
            GraphPane pane = new GraphPane
            {
                Title = { Text = atr.Title },
                XAxis = { Title = { Text = atr.XLabel } },
                YAxis = { Title = { Text = atr.YLabel } }
            };


            // Add a curve to the graph
            for ( int cnt = 0; cnt < atr.Selector.Length; cnt++)
            {
                pane.AddCurve(atr.CurveNames[cnt], xValues , yValues[atr.Selector[cnt]], atr.Colors[cnt], SymbolType.None);

            }

            // Render the graph to a bitmap
            Bitmap bitmap = new Bitmap(600, 400);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                pane.AxisChange();
                pane.Draw(g);
            }

            // Save the chart as a PNG image
            bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
        }


        /* 
         * InsertImageIntoExcel:  Put a PNG image into an Excel file 
         * imagePath: The image
         * BaseCell: Top left corner
         * sheet    :Sheet name to insert in  
         */
        public static void InsertImageIntoExcel(string imagePath, string BaseCell , string xlsfilePath, string sheet )
        {
            // Insert the image at cell B2
            XLWorkbook workbook = new XLWorkbook(xlsfilePath);
            IXLWorksheet worksheet = workbook.Worksheet(sheet);
            worksheet.AddPicture(imagePath)
                                    .MoveTo(worksheet.Cell(BaseCell))
                                    .Scale(1.0); // Scale the image if necessary
//                                    .WithPlacement(XLPicturePlacement.FreeFloating)
            workbook.Save(); 

        }

        /* 
         * PrintExcel: Print the Excel ATR file as PDF 
         * workbookPath: The filename of the excel workbook 
         */
        public static bool PrintExcel(string workbookPath)
        {
            // Initialize Excel application
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = null;
            bool RetVal = true;
            try
            {
                // Open the Excel workbook
                excelWorkbook = excelApp.Workbooks.Open(workbookPath);

                // Define the output PDF path

                string pdfPath = Path.ChangeExtension(workbookPath, ".pdf");

                if (System.IO.File.Exists(pdfPath))
                {
                    System.IO.File.Delete(pdfPath);  // Delete existing PDF
                }

                // Export the workbook to PDF
                excelWorkbook.ExportAsFixedFormat(
                    Excel.XlFixedFormatType.xlTypePDF,
                    pdfPath,
                    Excel.XlFixedFormatQuality.xlQualityStandard,
                    IncludeDocProperties: true,
                    IgnorePrintAreas: false,
                    OpenAfterPublish: false
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                RetVal = false;
            }
            finally
            {
                // Close the workbook and quit the Excel application
                if (excelWorkbook != null)
                {
                    excelWorkbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                }
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
            return RetVal;
        }
    }

}
