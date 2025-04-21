using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Plot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int ind,len;
            string str = "";
            for(ind= 0; ind < args.Length;ind++)
            {
                str = str + args[ind];
            }
            //MessageBox.Show(str);    
            if (args.Length > 0)
            {
                len = args[0].Length;
                if (len > 0)
                {
                    if (args[0][0] == 34 && args[0][len - 1] == 34)
                    {
                        str = args[0].Substring(0, len - 1);
                    }
                    else
                    {
                        str = args[0];
                    }
                }
                CFigList.OnFileOpenAll(str);
                Application.Run(CFigList.FigList[0]);
                return;
            }


            //int figInd = CFigList.NewFig(0, 1, 1, "Figure 1");
            //double[] xData = new double[2] {0,1};
            //double[] yData = new double[2] {0,1};
            //Color col = Color.FromArgb(0, 0, 200);
            int numOfPoints = 100;

            //int figInd = CFigList.NewFig(0, 12, 1, "Figure 1");
            int figInd = CFigList.NewFig(0, 4, 1, "Figure 1");
            double[] xData = new double[numOfPoints];
            double[] yData0 = new double[numOfPoints];
            double[] yData1 = new double[numOfPoints];
            double[] yData2 = new double[numOfPoints];
            double[] yData3 = new double[numOfPoints];

            for( ind = 0; ind < 100; ind++ )
            {
                xData[ind] = 0.01 * ind;
                yData0[ind] = 1 * Math.Sin(2 * Math.PI * 10.0 * xData[ind]);
                yData1[ind] = 2 * Math.Sin(2 * Math.PI * 10.0 * xData[ind]);
                yData2[ind] = 3 * Math.Sin(2 * Math.PI * 10.0 * xData[ind]);
                yData3[ind] = 4 * Math.Sin(2 * Math.PI * 10.0 * xData[ind]);
            }
            Color col = Color.FromArgb(0, 0, 200);
            CFigList.FigList[figInd].FigAxisList.Axes[0].m_ZoomSync = 0;
            CFigList.FigList[figInd].FigAxisList.AddLine(0, xData, yData0, col, "Sin10Hz1", 4);
            CFigList.FigList[figInd].FigAxisList.Axes[0].HideScale(1,1);
            ind = CFigList.FigList[figInd].FigAxisList.AddLine(1, xData, yData1, col, "Sin10Hz2", 1,2,0);
            CFigList.FigList[figInd].FigAxisList.Axes[1].m_pLines[ind].m_ShowName = 0;
            CFigList.FigList[figInd].FigAxisList.AddLine(2, xData, yData2, col, "Sin10Hz3", 1,1,1);
            CFigList.FigList[figInd].FigAxisList.AddLine(3, xData, yData3, col, "Sin10Hz4", 1);
            CFigList.FigList[figInd].FigAxisList.XLabel(1, "Time [sec]");
            CFigList.FigList[figInd].FigAxisList.YLabel(1, "Sin [val]");

            //for (ind = 0; ind < numOfPoints; ind++)
            //{
            //    yData[ind] = 1 * Math.Sin(2 * Math.PI * 2.0 * xData[ind]);
            //}
            //col = Color.FromArgb(0, 200, 0);
            //CFigList.FigList[figInd].FigAxisList.AddLine(4, xData, yData, col, "Sin5Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.AddLine(5, xData, yData, col, "Sin5Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.AddLine(6, xData, yData, col, "Sin5Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.AddLine(7, xData, yData, col, "Sin5Hz", 1);

            //for (ind = 0; ind < numOfPoints; ind++)
            //{
            //    yData[ind] = 1 * Math.Sin(2 * Math.PI * 3.0 * xData[ind]);
            //}
            //col = Color.FromArgb(200, 0, 0);
            //CFigList.FigList[figInd].FigAxisList.AddLine(8, xData, yData, col, "Sin4Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.AddLine(9, xData, yData, col, "Sin4Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.AddLine(10, xData, yData, col, "Sin4Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.AddLine(11, xData, yData, col, "Sin4Hz", 1);
            //CFigList.FigList[figInd].FigAxisList.XLabel(0, "Time [sec]");
            //CFigList.FigList[figInd].FigAxisList.YLabel(0, "Sin [val]");

            Application.Run(CFigList.FigList[figInd]);            
        }
    }
}
