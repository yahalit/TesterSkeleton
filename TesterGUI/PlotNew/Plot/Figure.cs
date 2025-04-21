using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Axis;
using System.IO;

namespace Plot
{
    public partial class Figure : Form
    {
        public int FigNum;
        public AxisList.AxisList FigAxisList ;

		System.Windows.Forms.Timer FigureTimer = new System.Windows.Forms.Timer();
        public double StartTime;
        private void PeriodicFunc(Object myObject,
            EventArgs myEventArgs)
        {
            double[] xData = new double[1];
            double[] yData = new double[1];
            double timenow = DateTime.Now.Ticks;
            xData[0] = (timenow - StartTime)/ TimeSpan.TicksPerSecond+1;
            yData[0] = Math.Sin(2*0.1*Math.PI* xData[0]);
            FigAxisList.AddPoints(0,0,xData,yData,2,0);
        }
        
        public Figure(int figNum,int rows, int cols,string name)
        {
            FigNum = figNum;
            FigAxisList = new AxisList.AxisList(figNum);
            InitializeComponent();
            Text = name;
            FigAxisList.InitTableLayoutPanel(tableLayoutPanelPlots, rows, cols);
			CFigList.ColorArr[0] = Color.FromArgb(0, 0, 200);
			CFigList.ColorArr[1] = Color.FromArgb(200, 0, 0);
			CFigList.ColorArr[2] = Color.FromArgb(0, 200, 200);
			CFigList.ColorArr[3] = Color.FromArgb(200, 0, 200);
			CFigList.ColorArr[4] = Color.FromArgb(200, 200, 0);
			CFigList.ColorArr[5] = Color.FromArgb(0, 0, 0);
			CFigList.ColorArr[6] = Color.FromArgb(200, 100, 0);
			CFigList.ColorArr[7] = Color.FromArgb(100, 0, 0);
			//StartTime = DateTime.Now.Ticks ;
			//FigureTimer.Tick += new EventHandler(PeriodicFunc);
			//FigureTimer.Interval = 500;
			//FigureTimer.Start();
		}
    }

    public static class CFigList
    {
        public static List<Figure> FigList = new List<Figure>();
		public static string CurrentPath;
		public static Color[] ColorArr = new Color[8];

		public static int FindFigNum(int figNum)
        {
            int ind;
            for (ind = 0; ind < FigList.Count; ind++)
            {
                if (figNum == FigList[ind].FigNum)
                {
                    return ind;
                }
            }
            return -1;
        }

        public static int OpenFig(int figNum, int rows, int cols, string name)
        {
            int res;
            res = FindFigNum(figNum);
            if (res >= 0)
            {
                FigList[res].Close();
                FigList.RemoveAt(res);
            }
            Figure fig = new Figure(figNum, rows, cols, name);
            CFigList.FigList.Add(fig);
            return CFigList.FigList.Count - 1;
        }
        public static int CloseFig(int figNum)
        {
            int res;
            res = FindFigNum(figNum);
            if (res >= 0)
            {
                FigList[res].Close();
                FigList.RemoveAt(res);
                return 1;
            }
            return 0;
        }

        public static int NewFig(int figNum, int rows, int cols, string name)
        {
            Figure fig = new Figure(figNum, rows, cols, name);
            CFigList.FigList.Add(fig);
            return CFigList.FigList.Count - 1;
        }

        public static void CloseAll()
        {
            int ind;
            for (ind = 0; ind < FigList.Count; ind++)
            {
                FigList[ind].Close();
            }
            FigList.Clear();
        }

		public static int SaveMatDArray(BinaryWriter sw, string vecName, List<double> datain)
		{
			int type = 0;   // type 
			int mrows = datain.Count;  // row dimension 
			int ncols = 1;  // column dimension
			int imagf = 0;  // flag indicating imag part 
			int namlen = vecName.Length + 1; // name length (including NULL)
			sw.Write(type);
			sw.Write(mrows);
			sw.Write(ncols);
			sw.Write(imagf);
			sw.Write(namlen);
			byte[] bytes = Encoding.ASCII.GetBytes(vecName);
			sw.Write(bytes);
			byte zByte = 0;
			sw.Write(zByte);
			for (int ind = 0; ind < mrows; ind++)
				sw.Write(datain[ind]);
			return 1;
		} 

		public static int GetNextMat(BinaryReader sw, ref double[] pData, ref int mn, ref string name)
		{
			int type;   // type 
			int mrows;  // row dimension 
			int ncols;  // column dimension
			int imagf;  // flag indicating imag part 
			int namlen; // name length (including NULL)
			byte[] bName;

			try
			{
				int ind;
				// Read the matrix title structure
				type = sw.ReadInt32();
				mrows = sw.ReadInt32();
				ncols = sw.ReadInt32();
				imagf = sw.ReadInt32();
				namlen = sw.ReadInt32();
				bName = sw.ReadBytes(namlen);
				name = Encoding.Default.GetString(bName, 0, namlen - 1);
				mn = mrows * ncols;
				// Test type of matrix 
				if (type == 0) //CMatrix::TypeNumeric
				{
					pData = new double[mn];
					for (ind = 0; ind < mn; ind++)
					{
						pData[ind] = sw.ReadDouble();
					}
					return 1;
				}
			}
			catch
			{
			}
			return 0;
		}

		public static void OnFileOpenAll(string fName)
		{
			int varNum, numOfFigs = 0, m, firstVar = 0, k,newFigHand=-1;
			int res, ind, cnt = 0, figInd, lineInd, fInd;
			int MAX_NUM_FIGS = 100, NUM_OF_PLOTS = 100, wasFigsPlots = 0;
			string name1 = "", name2 = "";
			string str;
			double[] pDataX = new double[1];
			double[] pDataY = new double[1];
			int[,] figsPlots = new int[100, 105];
			double d;
			string[] words;
			try
			{
				if (fName.Contains(".maf") || fName.Contains(".mat"))
				{
					BinaryReader reader = new BinaryReader(File.Open(fName, FileMode.Open));
					ind = 0;
					firstVar = 0;
					figInd = 0;
					lineInd = 0;
					for (m = 0; m < MAX_NUM_FIGS; m++)
					{
						figsPlots[m, 0] = 0;
						figsPlots[m, 1] = 1;
						figsPlots[m, 2] = 1;
						figsPlots[m, 3] = NUM_OF_PLOTS;
						for (cnt = 0; cnt < NUM_OF_PLOTS; cnt++)
						{
							figsPlots[m, 4 + cnt] = 0;
						}
					}
					numOfFigs = 1;
					while (true)
					{
						res = GetNextMat(reader, ref pDataX, ref cnt, ref name1);
						if (res != 1)
						{
							break;
						}
						if (firstVar == 0)
						{
							firstVar = 1;
							if (name1 == "NumOfFigs")
							{
								numOfFigs = Convert.ToInt32(pDataX[0]);
								if (numOfFigs < 1)
								{
									break;
								}
								res = GetNextMat(reader, ref pDataX, ref cnt, ref name1);
								if (res != 1)
								{
									break;
								}
								if (name1 == "FigsPlots")
								{
									wasFigsPlots = 1;
									for (m = 0; m < cnt; m++)
									{
										figsPlots[m % numOfFigs, m / numOfFigs] = (int)pDataX[m];
									}
									for (m = 0; m < numOfFigs; m++)
									{
										CFigList.CloseFig(figsPlots[m, 0]);
										fInd = CFigList.NewFig(figsPlots[m, 0], figsPlots[m, 1],
											figsPlots[m, 2], "Figure");
										CFigList.FigList[fInd].Show();
									}
									continue;
								}
							}
							else
							{
								reader.BaseStream.Position = 0;
								continue;
							}
						}
						res = GetNextMat(reader, ref pDataY, ref cnt, ref name2);
						if (res != 1)
						{
							break;
						}
						if (wasFigsPlots == 1)
						{
							fInd = CFigList.FindFigNum(figsPlots[figInd, 0]);
							k = figsPlots[figInd, 4 + lineInd];
							CFigList.FigList[fInd].FigAxisList.Axes[k].AddLine(pDataX, pDataY, ColorArr[lineInd & 7], name2);
							//SetCurrFigure(figsPlots[figInd][0]);
							//SubPlot(figsPlots[figInd][1], figsPlots[figInd][2]);
							//Plot(dPtr1, dPtr2, cnt, col[lineInd % 8], name2, figsPlots[figInd][4 + lineInd]);
							//HoldOn(1);
							lineInd++;
							if (lineInd == figsPlots[figInd, 3])//numOfLines
							{
								lineInd = 0;
								figInd++;
							}
						}
						else
						{
							if (newFigHand == -1)
							{
								newFigHand = CFigList.NewFig(0, 0, 0, "Figure");
							}
							CFigList.FigList[newFigHand].FigAxisList.Axes[0].AddLine(pDataX, pDataY, ColorArr[ind & 7], name2);
						}
						ind++;
					}
					reader.Close();
				}
				else
				{
					StreamReader sw = new StreamReader(fName);
					varNum = 0;
					while (true)
					{
						str = sw.ReadLine();
						if (str == null)
						{
							//end if file
							break;
						}
						words = str.Split(',');
						if (words.Length < 2)
						{
							continue;
						}
						name1 = words[0];
						pDataX = pDataY;
						pDataY = new double[words.Length - 1];
						for (ind = 1; ind < words.Length; ind++)
						{
							try
							{
								d = Double.Parse(words[ind]);
								pDataY[ind - 1] = d;
							}
							catch
							{
								continue;
							}
						}
						varNum++;
						if (varNum % 2 == 0)
						{
							if (pDataX.Length == pDataY.Length)
							{
								if (newFigHand == -1)
								{
									newFigHand = CFigList.NewFig(0, 0, 0, "Figure");
								}
								CFigList.FigList[newFigHand].FigAxisList.Axes[0].AddLine(pDataX, pDataY, ColorArr[(varNum >> 1) & 7], name1);
							}
						}
					}
					sw.Close();
				}
			}
			catch
			{
			}
		}
	}



}
