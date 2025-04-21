using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using DocumentFormat.OpenXml.Math;
using System.IO;

namespace PvsGUI
{
    public partial class CRegression : Form
    {
        double[] coefficients = new double[2];
        double maxerr = 0;
        double rmserr = 0;
        bool UserApprove = false;
        string title;
        int GraphNum; 

        public CRegression( double[] xdata , double[] ydata , int _GraphNum , string titlein = null )
        {
            InitializeComponent();
            GraphNum = _GraphNum; 
            this.FormClosing += PublishResults; 

            if (titlein == null)
                title = "data";
            else
                title = titlein; 

            this.SizeChanged += EvtSetsize;

            // Get the graph pane
            GraphPane graphPane = zedGraphControlFit.GraphPane;

            // Set the title and axis labels
            graphPane.Title.Text = "Fit result for " + title ;
            graphPane.XAxis.Title.Text = "Actual voltages";
            graphPane.YAxis.Title.Text = "Results";

            var curveideal = graphPane.AddCurve("Ideal", new PointPairList(ydata, ydata), System.Drawing.Color.Green);
            curveideal.Line.IsVisible = true;
            curveideal.Symbol.IsVisible = false;

            var curveuncorrectd = graphPane.AddCurve("Uncorrected", new PointPairList(xdata, ydata), System.Drawing.Color.Blue);
            curveuncorrectd.Line.IsVisible = false;
            curveuncorrectd.Symbol.IsVisible = true;
            curveuncorrectd.Symbol.Type = SymbolType.Circle;

            graphPane.XAxis.MajorGrid.IsVisible = true;
            graphPane.YAxis.MajorGrid.IsVisible = true;

            // Test if all the xdata are equal. If so, space them to prevent NAN
            double dx = 0;
            double dy = 0;
            for ( int cnt = 1; cnt < xdata.Length ; cnt++ )
            {
                dx = Math.Max(dx , Math.Abs(xdata[cnt] - xdata[0]) );
                dy = Math.Max(dy, Math.Abs(ydata[cnt] - ydata[0]));
            }
            if (( dx * 1e-6  > dy) || (dx * 1e6 < dy) || dx ==0 || dy == 0 ) 
            {
                for (int cnt = 1; cnt < xdata.Length; cnt++) xdata[cnt] = ydata[cnt] * 1e-6;
            }

            coefficients = Fit.Polynomial(xdata, ydata, 1);
            int nData = ydata.Length;
            double[] Fitted = new double[nData];
            double[] FitError = new double[nData];
            for ( int cnt = 0; cnt < nData; cnt++)
            {
                Fitted[cnt] = coefficients[0] + coefficients[1] * xdata[cnt];
                FitError[cnt] = ydata[cnt] - Fitted[cnt];
                maxerr = Math.Max(maxerr, Math.Abs(FitError[cnt] ) );
                rmserr += FitError[cnt] * FitError[cnt];  
            }
            rmserr = Math.Sqrt(rmserr / nData);

            LabelCoeffs.Text = "Gain: " + coefficients[1].ToString("F5") + " Offset : " + coefficients[0].ToString("F5") +
                " MaxAbsErr : " + maxerr.ToString("F5") + "RMS Err : " + rmserr.ToString("F5"); 

            var curvefitted = graphPane.AddCurve("Fitted", new PointPairList(Fitted, ydata), System.Drawing.Color.Blue);
            curvefitted.Line.IsVisible = false;
            curvefitted.Symbol.IsVisible = true;
            curvefitted.Symbol.Type = SymbolType.XCross;
            // Refresh the graph to show the updates
            zedGraphControlFit.AxisChange();
            zedGraphControlFit.Invalidate();

            // Set the error graph 
            // Get the graph pane
            GraphPane graphPane2 = zedGraphControlError.GraphPane;

            // Set the title and axis labels
            graphPane2.Title.Text = "Fitting error result for " + title;
            graphPane2.XAxis.Title.Text = "Actual voltages";
            graphPane2.YAxis.Title.Text = "Error (Volts)";

            var curveError = graphPane2.AddCurve("Error", new PointPairList(xdata, FitError), System.Drawing.Color.Blue);
            curveError.Line.IsVisible = true;
            curveuncorrectd.Symbol.IsVisible = true;
            curveError.Symbol.Type = SymbolType.Plus;

            zedGraphControlError.AxisChange();
            zedGraphControlError.Invalidate();
        }
        private void EvtSetsize(object sender, EventArgs e)
        {
            Setsize();
        }
        public void Setsize()
        {
            int w = this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;

            double  nGr = 2.1;
            int h3 = (int)(h / nGr );

            int top = h3 / 20;
            zedGraphControlFit.Location = new Point(w / 20, top);
            zedGraphControlFit.Size = new Size(w * 9 / 10, h3 * 9 / 10);
            zedGraphControlFit.Show();

            top += h3;
            zedGraphControlError.Location = new Point(w / 20, top);
            zedGraphControlError.Size = new Size(w * 9 / 10, h3 * 9 / 10);
            zedGraphControlError.Show();
        }


        private void PublishResults(object sender, FormClosingEventArgs e)
        {
            if (UserApprove )
            {
                DateTime now = DateTime.Now;
                string[] Lines = new string[] {"Date Gain, offset, max err, rms err , approve for: "+title ,
                now.ToString("yyyy-MM-ddTHH:mm"),
                coefficients[1].ToString(), coefficients[0].ToString() ,
                maxerr.ToString() , rmserr.ToString()  , UserApprove.ToString() };
                string filePath = "RegressionOut_" + GraphNum.ToString() + ".txt";
                File.WriteAllLines(filePath, Lines);
            }
        }
        //LabelCoeffs.Text = "Gain: " + coefficients[1].ToString("F5") + " Offset : " + coefficients[0].ToString("F5") +
        //        " MaxAbsErr : " + maxerr.ToString("F5") + "RMS Err : " + rmserr.ToString("F5"); 


        private void Click_Next(object sender, EventArgs e)
        {
            UserApprove = true; 
            this.Close(); 
        }

        private void ButtonRehect_Click(object sender, EventArgs e)
        {
            UserApprove = false;
            this.Close();

        }
    }
}




