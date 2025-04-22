using ClosedXML.Excel;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using ZedGraph;
using static TesterGUI.CRelay32;
using static TesterGUI.XLSGraph;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using MathNet.Numerics.Distributions;

namespace TesterGUI
{
    partial class CAtpMainWindow
    {


        public bool CalibPressureAdcInput(out string msg)
        {
            bool TotalPass = true;
            AdcInputTestDone = false;
            msg = string.Empty;


            // Read the existing calibration 
            if (!Interpreter.GetCalibration() )
            {
                msg = "Could not read previous calibration from EUT";
            }

            // Kill calibration 
            double[] g = new double[Literals.N_PRESSURE];
            double[] c = new double[Literals.N_PRESSURE];
            if (!Interpreter.SetAdcCalibration(g, c, burn: false))
            {
                msg = "Nulling of calibration parameters failed";
            }

            // Will generate results in AutoPressureTest.json
            CPressureInCalibTest form = new CPressureInCalibTest(_automatic: true,_CalibType: 0);
            form.ShowDialog();

            try
            {
                string readJson = File.ReadAllText("AutoPressureTest.json");
                CPressureRslt Rslt = JsonSerializer.Deserialize<CPressureRslt>(readJson);

                if (Rslt.Rstatus)
                {
                    for ( int cnt = 0; cnt < Literals.N_PRESSURE ; cnt++)
                    {
                        g[cnt] = 1 - Rslt.RGain[cnt]; c[cnt] = -Rslt.ROffset[cnt];
                    }

                    if ( !Interpreter.SetAdcCalibration(g, c, burn: true))
                    {
                        msg = "Programming of calibration parameters failed";
                    }
                }
                else
                {
                    msg = "Calibration aborted - no user approval";
                }
            }
            catch
            {
                msg = "Could not read calibration results file" ;
            }
            return TotalPass;
        }



        public bool CalibTPAdcInput(out string msg)
        {
            bool TotalPass = true;
            AdcInputTestDone = false;
            msg = string.Empty;

            // Read the existing calibration 
            if (!Interpreter.GetCalibration())
            {
                msg = "Could not read previous calibration from EUT";
            }

            double[] g = new double[2];
            double[] c = new double[2];
            // Kill calibration 
            if (!Interpreter.SetTPCalibration(g, c, burn: false))
            {
                msg = "Nulling of calibration parameters failed";
            }

            // Will generate results in AutoPressureTest.json
            CPressureInCalibTest form = new CPressureInCalibTest(_automatic: true, _CalibType: 1);
            form.ShowDialog();

            try
            {
                string readJson = File.ReadAllText("AutoTPTest.json");
                CPressureRslt Rslt = JsonSerializer.Deserialize<CPressureRslt>(readJson);

                if (Rslt.Rstatus)
                {
                    for (int cnt = 0; cnt < 2; cnt++)
                    {
                        g[cnt] = 1 - Rslt.RGain[cnt]; c[cnt] = -Rslt.ROffset[cnt];
                    }

                    if (!Interpreter.SetTPCalibration(g, c, burn: true))
                    {
                        msg = "Programming of calibration parameters failed";
                    }
                }
                else
                {
                    msg = "Calibration aborted - no user approval";
                }
            }
            catch
            {
                msg = "Could not read calibration results file";
            }
            return TotalPass;
        }


    }


}