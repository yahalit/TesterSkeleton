using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.Runtime.InteropServices.ComTypes;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Plot;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Presentation;
using System.Management;
using DocumentFormat.OpenXml.Drawing.Diagrams;

/*
*SetNAInAtr: Set a result in the ATR is Not Applicable (unit not installes)
*/ 


namespace PvsGUI
{
    // Summarize the ATR statistics
    public struct CAtrStatistics
    {
        public int nPages; // Number of test pages
        public int nTests; // Number of total test items
        public int nSuccess; // Number of PASS tests 
        public int nFailure; // Number of failed tests
        public int nNonApplicable; // Number of inapplicable tests
        public int nEmpty; // Number of unperformed tests
    }

    //The formal details in the ATR front page
    public struct CTestForm
    {
        public string SerialNumber; // EUT serial number 
        public string TesterName; // Name of tester
        public string TestLocation; // Test location 
        public double RoomTemperature; // Room temperature at test time 
        public int CpuSwVersion; // SW version of the EUT 
    }

    // User-driven tolerance boud specifier
    public struct AtrBoundSpecifier
    {
        public double LowValue; // Lower acceptable value
        public double HighValue; // Higher acceptable value 
        public bool valid;       //true if valid 
        public AtrBoundSpecifier( double l , double h , bool v = true)
        {
            LowValue = l;
            HighValue = h;
            valid = v;
        }
    }

    public partial class CAtpExcel 
    {

        string AtpDir;
        public CTestForm TestForm = new CTestForm();
        public CInterpreter Interpreter = CInterpreter.Instance; //  new CInterpreter(mutex);
        DateTime TestDate = DateTime.Now; 


        bool LocateItemInColumn(string item, IXLWorksheet ws, char RsltcolumnIndex, out string value, out string errmsg, out int rowIndex, bool MayBeNumber = false, bool ValueExpected = true )
        {
            value = "";
            errmsg = "Ok";
            char LookColumn = (char) ( RsltcolumnIndex - 1) ;
            var column = ws.Column("" +  LookColumn);
            rowIndex = -1;
            foreach (var cell in column.CellsUsed())
            {
                string nextstr = cell.GetString().Trim();
                if (nextstr.StartsWith(item, StringComparison.OrdinalIgnoreCase))
                {
                    rowIndex = cell.Address.RowNumber; // Get the row number of the matching cell
                    
                    var rsltcell = ws.Cell(RsltcolumnIndex + rowIndex.ToString());

                    if (ValueExpected )
                    {
                        if (rsltcell.DataType != XLDataType.Text && !(MayBeNumber & (rsltcell.DataType == XLDataType.Number)))
                        {
                            errmsg = $"The value of {item} is missing in worksheet {ws.Name} column {RsltcolumnIndex} ";
                            return false;
                        }
                        value = rsltcell.GetString().Trim();
                    }
                    else
                    {
                        value = ""; 
                    }
                    return true ;
                }

            }
            errmsg = $"The header of {item} is missing in worksheet {ws.Name} column {LookColumn} ";
            return false;
        }
        public bool LocateNumberInColumn(string item, IXLWorksheet ws, char RsltcolumnIndex, out double value, out string errmsg, double lower, double upper)
        {
            value = 0;
            if (!LocateItemInColumn(item, ws, RsltcolumnIndex, out string svalue, out errmsg, out _, MayBeNumber: true))
            {
                return false;
            }
            // Try a hex number
            if (svalue.StartsWith("0x") || svalue.StartsWith("0X")) 
            {
                value = (double)Convert.ToInt32(svalue, 16);
                return true; 
            }
            if (!double.TryParse(svalue, out value) || double.IsNaN(value) || double.IsInfinity(value) )
            {
                errmsg = $"The value of {item} is not a number in worksheet {ws.Name} column {RsltcolumnIndex} ";
                return false;
            }
            if ( value < lower || value > upper)
            {
                errmsg = $"The value of {item} is {value} : out of the spec range [{lower},{upper}] in worksheet {ws.Name} column {RsltcolumnIndex} ";
                return false;
            }
            return true;
        }

        public bool LocateNumberInColumn(string item, IXLWorksheet ws, char RsltcolumnIndex, out int value, out string errmsg, double lower, double upper)
        {
            bool retval = LocateNumberInColumn(item, ws, RsltcolumnIndex, out double dvalue, out errmsg,lower,upper);
            value = (int)dvalue;
            return retval; 
        }


        // Locate in a given sheet the test number to start the test sequence
        public bool LocateTestByIdentifier(string xlsfilePath, CTestIdentifier TestId , out int rowIndex )
        {
            rowIndex = -1;
            XLWorkbook workbook;
            IXLWorksheet ws;
            try
            {
                workbook = new XLWorkbook(xlsfilePath);
                ws = workbook.Worksheet(TestId.TestInfo.TestSheet);
            }
            catch
            {
                return false; 
            }

            var column = ws.Column("A");
            bool HeaderFound = false; 
            foreach (var cell in column.CellsUsed())
            {
                string str = cell.GetString().Trim(); // Remove leading and trailing whites
                if (HeaderFound)
                { // Go till the next header or till the end of cells
                    if (str.StartsWith("Header", StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Went till end and did not find it 
                    }
                    if (str.StartsWith(TestId.TestInfo.TestIdentifier, StringComparison.OrdinalIgnoreCase))
                    {
                        rowIndex = cell.Address.RowNumber;
                        return true;
                    }
                } 
                else
                {   // Find the header to start with 
                    if (str.Equals("Header", StringComparison.OrdinalIgnoreCase))
                    {
                        string str1 = ws.Cell("B" + cell.Address.RowNumber.ToString()).GetString();
                        str1 = Regex.Replace(str1, @"\s+", "");
                        string str2 = Regex.Replace(TestId.TestInfo.TestHeader, @"\s+", "");

                        // Compare the header to the desired strign after removing all the white spaces 

                        if (str1.Equals(str2, StringComparison.OrdinalIgnoreCase))
                        {
                            HeaderFound = true;
                        }
                    }

                }
            }
            return false;  // Went till end and did not find it 
        }



        // Write test result in the ATR form and evaluate Pass if necessary. 
        // Arguments: 
        // xlsfilePath : Path to open XLSX file 
        // SpecificTestID: The identifier to the specific test (not the group ID!)
        // TestId        : The gross test identifier 
        // dActValue     : Numeric actual value, applicable if sActValue is null, which will give pass/fail by comparison to low/high limits 
        // sActValue     : String actual value , taken if not-null. Pass/Fail is taken from the Pass argument 
        // Pass          : Test result : true for pass (green cell), false (red cell) for fail
        // ErrMsg        : Error message list, appended on error
        // SpecificErrorMsg : Error message to append into ErrMsg if error, ignored if null 
        // applicable    : If false, values and limits are ignored, the result is NA in gray cell
        // 
        public bool SetResultInAtr(string SpecificTestID, CTestIdentifier TestId, double dActValue, string sActValue, ref bool Pass, ref List<string> ErrMsg, string SpecificErrorMsg = null, bool applicable = true)
        {
            return SetResultInAtrDirect(SpecificTestID, TestId, dActValue, sActValue,ref Pass, ref ErrMsg, SpecificErrorMsg, applicable, new AtrBoundSpecifier ( 0,0,false)  ); 
        }

        /* 
         *  An overloaded method with dynamic Pass values instead of reading them from the Excel 
         */
        public bool SetResultInAtr(string SpecificTestID, CTestIdentifier TestId, double dActValue, AtrBoundSpecifier Bounds, ref bool Pass, ref List<string> ErrMsg, string SpecificErrorMsg = null, bool applicable = true)
        {
            return SetResultInAtrDirect(SpecificTestID, TestId, dActValue, null, ref Pass, ref ErrMsg, SpecificErrorMsg, applicable, Bounds);
        }

        /* 
         *  SetResultInAtrDirect: helper for SetResultInAtr
         */
        bool SetResultInAtrDirect( string SpecificTestID , CTestIdentifier TestId, double dActValue , string sActValue , ref bool Pass , ref List<string> ErrMsg , string SpecificErrorMsg  ,  bool applicable , AtrBoundSpecifier Bounds )
        {
            string xlsfilePath = TestId.ExcelFileName; 
            XLWorkbook workbook = new XLWorkbook(xlsfilePath);
            IXLWorksheet ws = workbook.Worksheet(TestId.TestInfo.TestSheet);

            for ( int cnt = 0; cnt < 100; cnt++ )
            {
                int rowIndex = TestId.RowIndex + cnt ;
                string str = ws.Cell(rowIndex, (int)Literals.ATRColumn.Header).GetString().Trim(); // Remove leading and trailing whites
                if (str.Equals("Header", StringComparison.OrdinalIgnoreCase))
                {
                    ErrMsg.Add($"Place to put the result for {SpecificTestID} not found in Excel ATR");
                    return false; // Went till end and did not find it 
                }
                if (str.Equals(SpecificTestID))
                {
            
                    if (sActValue == null)
                    { // Use the numeric value 
                        ws.Cell(rowIndex, (int)Literals.ATRColumn.ActualValue).Value = dActValue;
                        try
                        {
                            double LowLimit; 
                            double HighLimit ;
                            if ( Bounds.valid ) 
                            {
                                LowLimit = Bounds.LowValue; 
                                HighLimit = Bounds.HighValue;
                                ws.Cell(rowIndex, (int)Literals.ATRColumn.LowValue).Value = LowLimit;
                                ws.Cell(rowIndex, (int)Literals.ATRColumn.HighValue).Value = HighLimit;
                            }
                            else
                            {
                                LowLimit = ws.Cell(rowIndex, (int)Literals.ATRColumn.LowValue).Value.GetNumber();
                                HighLimit = ws.Cell(rowIndex, (int)Literals.ATRColumn.HighValue).Value.GetNumber();
                            }
                            Pass = (dActValue >= LowLimit) && (dActValue <= HighLimit);
                        }
                        catch
                        {
                            ErrMsg.Add($"Could not read acceptance limits for test {SpecificTestID} from ATR form");
                            return false; // Went till end and did not find it 
                        }
                    }
                    else
                    {
                        ws.Cell(rowIndex, (int)Literals.ATRColumn.ActualValue).Value = sActValue;
                    }
                    bool RetVal = true ; 
                    if (applicable)
                    {
                        if (Pass)
                        {
                            ws.Cell(rowIndex, (int)Literals.ATRColumn.Pass).Style.Fill.BackgroundColor = XLColor.Green;
                            ws.Cell(rowIndex, (int)Literals.ATRColumn.Pass).Value = "Pass";
                        }
                        else
                        {
                            ws.Cell(rowIndex, (int)Literals.ATRColumn.Pass).Style.Fill.BackgroundColor = XLColor.Red;
                            ws.Cell(rowIndex, (int)Literals.ATRColumn.Pass).Value = "Fail";
                            if (SpecificErrorMsg != null )
                            {
                                ErrMsg.Add(SpecificErrorMsg);
                            }
                            RetVal = false;
                        }
                        ws.Cell(rowIndex, (int)Literals.ATRColumn.Date).Value = TestDate;
                    }
                    else
                    {
                        ws.Cell(rowIndex, (int)Literals.ATRColumn.Pass).Style.Fill.BackgroundColor = XLColor.Gray;
                        ws.Cell(rowIndex, (int)Literals.ATRColumn.Pass).Value = "NA";
                    }
                    workbook.Save();
                    return RetVal;
                }
            }
            ErrMsg.Add("Place to put this result not found in Excel ATR");
            return false;
        }


        // 
        /* 
         * SetNAInAtr: Set a result in the ATR is Not Applicable (unit not installes)
         * Arguments: SpecificTestID - the ID of the specific test to fill 
         * TestId: Test identifier 
         */
        public void SetNAInAtr(string SpecificTestID, CTestIdentifier TestId)
        {
            bool Pass = true;
            List<string> ErrMsg = new List<string>(); 
            SetResultInAtr(SpecificTestID, TestId, 0, "Not applicable",ref Pass,ref  ErrMsg , null , applicable : false);
        } 



        bool IsOnlyDigitsAndDots(string _input)
        {
            if (_input == null) return false;
            string input = _input.Trim();
            if(input.Length == 0 ) return false;
            return Regex.IsMatch(input, @"^[\d.]+$");
        }

        /* 
         * After the ATR is done, go over the results and calculate statistics: success/fail/NA/Undone
         */
        public bool ScanAtrResults(string filename, out CAtrStatistics stat)
        {
            stat = new CAtrStatistics();

            List<string> workSheetNames = new List<string>();
            using (var workbook = new XLWorkbook(filename))
            {
                foreach (IXLWorksheet ws in workbook.Worksheets)
                {
                    if (ws.Name == "Test Form")
                    {
                        continue;
                    }
                    // Scan results for all the other forms
                    string h1 = ws.Cell("A1").Value.ToString().Trim();
                    string h2 = ws.Cell("H1").Value.ToString().Trim();
                    if (!(h1.Equals("Identifier", StringComparison.OrdinalIgnoreCase) && h2.Equals("Pass", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                    stat.nPages += 1;
                    var column = ws.Column("A");
                    foreach (var cell in column.CellsUsed())
                    {
                        if (IsOnlyDigitsAndDots(cell.Value.ToString()))
                        {
                            stat.nTests += 1;
                            int rowIndex = cell.Address.RowNumber;
                            string pasfail = ws.Cell("H" + cell.Address.RowNumber.ToString()).Value.ToString().Trim();
                            if (pasfail.Equals("Pass", StringComparison.OrdinalIgnoreCase) || pasfail.Equals("True", StringComparison.OrdinalIgnoreCase))
                            {
                                stat.nSuccess++;
                            }
                            else
                            {
                                if (pasfail.Equals("Fail", StringComparison.OrdinalIgnoreCase) || pasfail.Equals("False", StringComparison.OrdinalIgnoreCase))
                                {
                                    stat.nFailure++;
                                }
                                else
                                {
                                    if (pasfail.Equals("NA", StringComparison.OrdinalIgnoreCase))
                                    {
                                        stat.nNonApplicable++;
                                    }
                                    else
                                    {
                                        stat.nEmpty++;
                                    }
                                }
                            }
                        }
                    }
                }
                // Write total statistics in ATR form
                try
                {
                    int rowIndex; 
                    IXLWorksheet TestFormSheet = workbook.Worksheet("Test Form");
                    LocateItemInColumn("Passed tests", TestFormSheet, 'C', out _, out _, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = stat.nSuccess;

                    LocateItemInColumn("Failed tests", TestFormSheet, 'C', out _, out _, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = stat.nFailure;

                    LocateItemInColumn("Non applicable tests", TestFormSheet, 'C', out _, out _, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = stat.nNonApplicable;

                    LocateItemInColumn("Undone tests", TestFormSheet, 'C', out _, out _, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = stat.nEmpty;

                    LocateItemInColumn("Total tests results", TestFormSheet, 'C', out _, out _, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    string rslt = "";
                    if ( stat.nEmpty > 0 )
                    {
                        rslt += "Incomplete : ";
                    }
                    if (stat.nFailure > 0 )
                    {
                        rslt += "Failed";
                        TestFormSheet.Cell("C" + rowIndex.ToString()).Style.Fill.BackgroundColor = XLColor.Red;
                    }
                    else
                    {
                        if (stat.nSuccess > 0)
                        {
                            rslt += "Success";
                            if (stat.nEmpty > 0 )
                            {
                                TestFormSheet.Cell("C" + rowIndex.ToString()).Style.Fill.BackgroundColor = XLColor.Yellow;
                            }
                            else
                            {
                                TestFormSheet.Cell("C" + rowIndex.ToString()).Style.Fill.BackgroundColor = XLColor.Green;
                            }
                        }
                    }
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = rslt;
                    workbook.Save(); 
                }
                catch
                { }
            }

            return true; 
        }

        // Parese the front page data of the ATR, write date, SW versions, and configuration
        public bool ParseTestsFrontPage(string _AtpDir, string fname, out string errmsg)
        {
            errmsg = "Ok";
            AtpDir = _AtpDir;
            string filename = AtpDir + @"\" + fname;
            List<string> workSheetNames = new List<string>();

            bool TestFormExists = false;
            try
            {
                using (var workbook = new XLWorkbook(filename))
                {
                    foreach (IXLWorksheet ws in workbook.Worksheets)
                    {
                        workSheetNames.Add(ws.Name);
                        if (ws.Name == "Test Form")
                        {
                            TestFormExists = true;
                            break;
                        }
                    }
                    if (!TestFormExists)
                    {
                        errmsg = "Could not find the \"TestForm\" worksheet";
                        return false;
                    }

                    IXLWorksheet TestFormSheet = workbook.Worksheet("Test Form");

                    if (!LocateItemInColumn(@"S/N", TestFormSheet, 'C', out TestForm.SerialNumber, out errmsg, rowIndex: out _, MayBeNumber: true, ValueExpected : false))
                    {
                        return false;
                    }
                    if (!LocateItemInColumn("Tester", TestFormSheet, 'C', out TestForm.TesterName, out errmsg, rowIndex: out _, MayBeNumber: true))
                    {
                        return false;
                    }
                    if (!LocateItemInColumn("test Location", TestFormSheet, 'C', out TestForm.TestLocation, out errmsg, rowIndex: out _, MayBeNumber: true))
                    {
                        return false;
                    }
                    LocateItemInColumn("Date", TestFormSheet, 'C', out _, out errmsg, out int rowIndex, MayBeNumber: false, ValueExpected: false);
                    if (rowIndex < 0)
                    {
                        return false;
                    }
                    TestDate = DateTime.Now;
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = TestDate ; // Put the date of today 

                    /*
                    if (!LocateNumberInColumn("Room temperature", TestFormSheet, 'C', out TestForm.RoomTemperature, out errmsg, 0, 40))
                    {
                        return false;
                    }
                    if (!LocateNumberInColumn("Expected CPU SW version code", TestFormSheet, 'C', out TestForm.CpuSwVersion, out errmsg, 0, (double)0x7fffffff))
                    {
                        return false;
                    }

                    if (!LocateNumberInColumn("Expected LLC SW version code", TestFormSheet, 'C', out TestForm.LLCSwVersion, out errmsg, 0, (double)0x7fffffff))
                    {
                        return false;
                    }

                    if (!LocateNumberInColumn("Expected Simulator SW version code", TestFormSheet, 'C', out TestForm.SimSwVersion, out errmsg, 0, (double)0x7fffffff))
                    {
                        return false;
                    }
                    */
                    // Neta: Here is some non-generic code. Please replace it with generics

                    LocateItemInColumn("Expected CPU SW version code", TestFormSheet, 'C', out _, out errmsg, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    if (rowIndex < 0)
                    {
                        return false;
                    }
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = Interpreter.Answer_GetUnitData.SimSubverPatch.ToString("X"); // Put the date of today 

                    LocateItemInColumn("Expected LLC SW version code", TestFormSheet, 'C', out _, out errmsg, out rowIndex, MayBeNumber: false, ValueExpected: false);
                    if (rowIndex < 0)
                    {
                        return false;
                    }
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = Interpreter.Answer_GetUnitData.LLCSubverPatch.ToString("X"); // Put the date of today 


                    LocateItemInColumn("Expected Simulator SW version code", TestFormSheet, 'C', out _, out errmsg, out rowIndex, MayBeNumber: false);
                    if (rowIndex < 0)
                    {
                        return false;
                    }
                    TestFormSheet.Cell("C" + rowIndex.ToString()).Value = Interpreter.Answer_GetUnitData.ValveSubverPatch.ToString("X"); // Put the date of today 



                    workbook.Save();
                } // End using 

            }
            catch (Exception ex) 
            {
                errmsg = $"Could not open file: { ex.Message}"; 
                return false;
            }
            return true;
        }



    }
} 