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
using System.Threading;


namespace PvsGUI
{
    public struct FrameDescriptor
        {
        public double xSpan;
        public double Ymin;
        public double Ymax;
        public bool IsYAuto; 
        public FrameDescriptor(double _xSpan, double _Ymin , double _Ymax , bool _IsYAuto )
        {
            xSpan = _xSpan;
            Ymin = _Ymin;
            Ymax = _Ymax;
            IsYAuto = _IsYAuto;
        }
    }

    public partial class Graphs : Form
    {
        static int MaxDataLength = 50000; // Maximum data length for vector display 
        static int N_VALVES = 14; 
        bool [] GraphActive;

        // Data vectors for display. The data is public, so it may be written in directly  
        //public double[][] Graph1Data = new double[N_VALVES][];
        //public double[][] Graph2Data = new double[N_VALVES][];
        //public double[][] Graph3Data = new double[N_VALVES][];
        LineItem[,] LineItems = new LineItem[3, N_VALVES];
        public double[] TimeData = new double[MaxDataLength];
        public string[] ValveLegend = new string[N_VALVES];
        public string[] PressureLegend = new string[N_VALVES];
        Color [] SignalColors;
        public Mutex mutex;
        public int MinIndex;
        public int SampleLength;

        public void UpdateGraphicDisplay( int FrameSelect , bool IsPressure , bool [] IsShown , List<double> [] srcList , List<double> Time ,
            FrameDescriptor Frame)
        {
            ZedGraphControl z;

            if (Time.Count == 0 )
            {
                return; // Nothing to do 
            }
            //double[] DataArray;
            string  legend ; 
            switch (FrameSelect)
            {
                case 0: 
                    z = zedGraphControl1;
//                    DataArray = Graph1Data;
                    break;
                case 1:
                    z = zedGraphControl2;
//                    DataArray = Graph2Data;
                    break;
                default:
                    FrameSelect = 2; 
                    z = zedGraphControl3;
//                    DataArray = Graph3Data;
                    break;
            }
            int nGraphsMax; 
            if (IsPressure )
            {
                nGraphsMax = 4;
            }
            else
            {
                nGraphsMax = N_VALVES;
            }
            // Mutex is necessary to avoid treating a list while its members are manipulated elsewhere
            mutex.WaitOne();
            MinIndex = Math.Min(FindLeastIndex(Time, Frame.xSpan),Literals.MaxListLength) ;
            SampleLength = Time.Count - MinIndex ;
            double[] DataArray = new double[SampleLength]; 
            Time.CopyTo(MinIndex, TimeData, 0, SampleLength);
            mutex.ReleaseMutex();
            // Clear existing data
            z.GraphPane.CurveList.Clear();
            z.GraphPane.GraphObjList.Clear();
            for ( int cnt = 0; cnt < nGraphsMax; cnt++ )
            {
                if ( ! IsShown[cnt] )
                {
                    continue; 
                }
                if (IsPressure)
                {
                    legend = PressureLegend[cnt] ;
                }
                else
                {
                    legend = ValveLegend[cnt];
                }
                mutex.WaitOne();

                srcList[cnt].CopyTo(MinIndex, DataArray, 0, SampleLength);
                mutex.ReleaseMutex();
                LineItems[FrameSelect, cnt] = z.GraphPane.AddCurve(legend , TimeData, DataArray, SignalColors[cnt]);
                // Hide symbols on the data line
                LineItems[FrameSelect, cnt].Symbol.IsVisible = false;
            }

            if (Frame.IsYAuto)
            {
                z.GraphPane.YAxis.Scale.MinAuto = true;
                z.GraphPane.YAxis.Scale.MaxAuto = true;
            }
            else
            {
                z.GraphPane.YAxis.Scale.Min = Frame.Ymin;
                z.GraphPane.YAxis.Scale.Max = Frame.Ymax;
            }

            // Finally redrawf
            z.AxisChange();
            z.Invalidate();
        }


        public Graphs( bool [] GraphActive_in , string [] Titles , Color[] ColorsIn, Mutex _mutex )
        {
            InitializeComponent();
            SignalColors = ColorsIn;
            this.SizeChanged += EvtSetsize;
            GraphActive = GraphActive_in; 
            setsize(GraphActive);
            zedGraphControl1.GraphPane.XAxis.Title.Text = "Time [sec]";
            zedGraphControl2.GraphPane.XAxis.Title.Text = "Time [sec]";
            zedGraphControl3.GraphPane.XAxis.Title.Text = "Time [sec]";
            mutex =  _mutex;

            // Create legends for valves and pressures 
            for ( int cnt = 0; cnt < 14; cnt++ )
            {
                ValveLegend[cnt] = $"V {cnt}";
 //               Graph1Data[cnt] = new double[MaxDataLength];
 //               Graph2Data[cnt] = new double[MaxDataLength];
 //               Graph3Data[cnt] = new double[MaxDataLength];
            }
            for (int cnt = 0; cnt < 4; cnt++)
            {
                PressureLegend[cnt] = $"P {cnt}";
            }

            SetTitles(Titles);

/*
            PointPairList baba = new PointPairList();

            float[] xx = { 1,2,3,4,5};
            float[] yy = { 1,2,1,3,1};

            for ( int cnt = 0; cnt < 5; cnt++)
            {
                baba.Add(xx[cnt], yy[cnt]); 
            }
            LineItem line = zedGraphControl1.GraphPane.AddCurve("yup", baba, Color.Red, SymbolType.Square);
            //myPane.YAxis.Scale.Min = 0;  // Minimum value for Y-axis
            //myPane.YAxis.Scale.Max = 100; // Maximum value for Y-axis
            zedGraphControl1.AxisChange();
*/
        }

        private void EvtSetsize(object sender, EventArgs e)
        {
            setsize( GraphActive);
        }
        public void setsize(bool[] GraphActive)
        {
            int w = this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;

            bool [] act = new bool[3] { GraphActive [0], GraphActive [1] , GraphActive [2]}; 
            int nGr = (act[0] ? 1 : 0) + (act[1] ? 1 : 0) + (act[2] ? 1 : 0);
            if ( nGr == 0 )
            {
                zedGraphControl1.Hide();
                zedGraphControl2.Hide();
                zedGraphControl3.Hide();
                return;
            }
            int h3 = h / nGr ;

            int top = h3 / 20;
            if (act[0])
            {
                zedGraphControl1.Location = new Point(w / 20, top);
                zedGraphControl1.Size = new Size(w * 9 / 10, h3 * 9 / 10);
                top = top + h3;
                zedGraphControl1.Show();
            }
            else
            {
                zedGraphControl1.Hide(); 
            }
            if (act[1])
            {
                zedGraphControl2.Location = new Point(w / 20, top);
                zedGraphControl2.Size = new Size(w * 9 / 10, h3 * 9 / 10);
                top = top + h3;
                zedGraphControl2.Show();
            }
            else
            {
                zedGraphControl2.Hide();
            }
            if ( act[2])
            {
                zedGraphControl3.Location = new Point(w / 20, top);
                zedGraphControl3.Size = new Size(w * 9 / 10, h3 * 9 / 10);
                zedGraphControl3.Show();
            }
            else
            {
                zedGraphControl3.Hide();
            }
        }

        public string GetUnits( string tit )
        {
            if (tit.Contains("Curr"))
            {
                return "Amp";
            }
            if (tit.Contains("Volt"))
            {
                return "Volt";
            }
            if (tit.Contains("Pos"))
            {
                return "Meter";
            }
            if (tit.Contains("Press"))
            {
                return "Bar";
            }
            return "Unspecified";
        }

        //zedGraphControl1.GraphPane.Title.Text = "Ananat";
        //zedGraphControl1.GraphPane.XAxis.Title.Text = "bubu";
        //zedGraphControl1.GraphPane.YAxis.Title.Text = "rrr";
        public void SetTitles(string [] tit)
        {
            zedGraphControl1.GraphPane.Title.Text = tit[0] ;
            zedGraphControl2.GraphPane.Title.Text = tit[1];
            zedGraphControl3.GraphPane.Title.Text = tit[2];


            zedGraphControl1.GraphPane.YAxis.Title.Text = GetUnits(tit[0]);
            zedGraphControl2.GraphPane.YAxis.Title.Text = GetUnits(tit[1]);
            zedGraphControl3.GraphPane.YAxis.Title.Text = GetUnits(tit[2]);

        }

        int FindLeastIndex(List<double> list , double span)
        {
            int left = 0;
            int right = list.Count - 1;
            double target = list[right] - span;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (list[mid] >= target)
                {
                    right = mid - 1; // Move left to find the lowest index
                }
                else
                {
                    left = mid + 1; // Move right to find a valid item
                }
            }

            // Check if `left` is valid and points to a value >= target
            return left;
        }


    }
}