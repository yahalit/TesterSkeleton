using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plot;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace Axis
{
	public partial class Axis : UserControl
	{
		public int AxIndex;
		public AxisList.AxisList ParentAxList;
		public Axis(AxisList.AxisList pAxList,int ind)
		{
			AxIndex = ind;
			ParentAxList = pAxList;
			InitializeComponent();
			m_PlotUpdate = 0;
			m_XGridStepsNum = 5;
			m_YGridStepsNum = 5;
			m_LogScaleX = 0;
			m_ExpMinSize = 4;
			m_ZoomOn = 1;
			zoomToolStripMenuItem.Checked = true;
			m_ZoomMode = 0;
			m_ZoomSync = 1;
			m_MoveMode = 0;
			m_AutoGrid = 1;
			m_xGrSize = 0;
			m_yGrSize = 0;
			m_xGrMin = 0;
			m_yGrMin = 0;
			m_xGridOn = 1;
			m_yGridOn = 1;
			m_HideXScale = 0;
			m_HideYScale = 0;
			m_HitPointTolPer = 0.01;
			m_CursorCross = 0;
			m_FindFreq = 0;
			m_FindSlope = 0;
			m_FindDiffX = 0;
			m_FindDiffY = 0;
			Size textSize = new Size();
			GetLabelSize("ABC0", ref textSize);
			m_XGraySpace = (int)(textSize.Height * 2);
			m_YGraySpace = (int)(textSize.Height * 2);
			//m_XGraySpace = 0;
			//m_YGraySpace = 0;
		}
		const int MAX_POINTS_NUM = 1000000;
		static int[] PointsToDrawX = new int[MAX_POINTS_NUM];
		static int[] PointsToDrawY = new int[MAX_POINTS_NUM];
		static uint[] NumPointsToDraw = new uint[MAX_POINTS_NUM];

		int m_XGridStepsNum;
		int m_YGridStepsNum;
		int m_ExpMinSize;

		double[] m_xGrid = new double[50];
		double[] m_yGrid = new double[50];
		int m_ZoomOn;

		int m_LogScaleX;

		public List<CLine> m_pLines = new List<CLine>();

		int m_FindMean;
		int m_FindSlope;
		int m_FindFreq;
		int m_FindDiffX;
		int m_FindDiffY;
		int m_CursorCross;

		string m_xLabel;
		string m_yLabel;

		public void SetLogScaleX(int on)
		{
			m_LogScaleX = on;
		}

		public void XLabel(string str)
		{
			m_xLabel = str;
		}
		public void YLabel(string str)
		{
			m_yLabel = str;
		}

		int m_PlotPosX1; //Left plot boundary [Pixels]
		int m_PlotPosY1; //Top plot boundary [Pixels]
		int m_PlotPosX2; //Right plot boundary [Pixels]
		int m_PlotPosY2; //Bottom plot boundary [Pixels]

		int m_XGraySpace;//x cordinate of axis inside control
		int m_YGraySpace;//y cordinate of axis inside control

		int m_PlotXSize; //Plot size [Pixels]
		int m_PlotYSize; //Plot size [Pixels]

		public double m_xGrSize; //Plot current size [user units]
		double m_yGrSize; //Plot current size [user units]

		double m_HitPointTolPer; //Hit point tolerance in percent from m_xGrSize and m_yGrSize

		public double m_StartTime;
		public double m_xGrMin; //Left plot boundary [user units]
		double m_yGrMin; //Top plot boundary [user units]
		double m_xGrMax; //Right plot boundary [user units]
		double m_yGrMax; //Bottom plot boundary [user units]

		int m_xGridOn;
		int m_yGridOn;

		int m_HideXScale;
		int m_HideYScale;
		public int m_MoveMode;
		public int m_MoveModeStartX;
		public int m_MoveModeStartY;
		double m_MoveModeMinX;
		double m_MoveModeMinY;
		public int m_ZoomMode;
		public int m_ZoomSync;
		public int m_AutoGrid;
		int m_ZoomStartX;
		int m_ZoomStartY;
		int m_ZoomX;
		int m_ZoomY;
		int m_ZoomXOld;
		int m_ZoomYOld;

		int m_PlotUpdate;

		public void HideScale(int hideXScale,int hideYScale)
        {
			m_HideXScale = hideXScale;
			m_HideYScale = hideYScale;
			Axis_SizeChanged(null, null);
		}

		int FindAxisCross(int x1, int y1, int x, int y, int[] x2, int[] y2)
		{
			double k, c;
			double[] xS = new double[4];
			double[] yS = new double[4];
			double[] xn = new double[2];
			double[] yn = new double[2];
			int ind, res, num, n;

			xn[0] = x;//- 0.5*(m_PlotPosX1+m_PlotPosX2) ;
			xn[1] = x1;//- 0.5*(m_PlotPosX1+m_PlotPosX2) ;
			yn[0] = y;//- 0.5*(m_PlotPosY1+m_PlotPosY2) ;
			yn[1] = y1;//- 0.5*(m_PlotPosY1+m_PlotPosY2) ;

			n = 0;
			if (Math.Abs(xn[0] - xn[1]) < 1.0e-6)
			{
				if ((double)(m_PlotPosX1 - x) * (m_PlotPosX2 - x) <= 0)
				{
					if ((double)(m_PlotPosY1 - y) * (m_PlotPosY1 - y1) <= 0)
					{
						x2[n] = x;
						y2[n] = m_PlotPosY1;
						n++;
					}
					if ((double)(m_PlotPosY2 - y) * (m_PlotPosY2 - y1) <= 0)
					{
						x2[n] = x;
						y2[n] = m_PlotPosY2;
						n++;
					}
				}
				return n;
			}
			if (Math.Abs(yn[0] - yn[1]) < 1.0e-6)
			{
				if ((double)(m_PlotPosY1 - y) * (m_PlotPosY2 - y) <= 0)
				{
					if ((double)(m_PlotPosX1 - x) * (m_PlotPosX1 - x1) <= 0)
					{
						x2[n] = m_PlotPosX1;
						y2[n] = y;
						n++;
					}
					if ((double)(m_PlotPosX2 - x) * (m_PlotPosX2 - x1) <= 0)
					{
						x2[n] = m_PlotPosX2;
						y2[n] = y;
						n++;
					}
				}
				return n;
			}

			k = ((double)(yn[0] - yn[1])) / (xn[0] - xn[1]);
			c = y - k * x;

			yS[n] = m_PlotPosY1;
			xS[n] = (m_PlotPosY1 - c) / k;
			if ((m_PlotPosX1 - xS[n]) * (m_PlotPosX2 - xS[n]) <= 0)
			{
				n++;
			}

			yS[n] = m_PlotPosY2;
			xS[n] = (m_PlotPosY2 - c) / k;
			if ((m_PlotPosX1 - xS[n]) * (m_PlotPosX2 - xS[n]) <= 0)
			{
				n++;
			}

			xS[n] = m_PlotPosX1;
			yS[n] = k * m_PlotPosX1 + c;
			if ((m_PlotPosY1 - yS[n]) * (m_PlotPosY2 - yS[n]) <= 0)
			{
				n++;
			}

			xS[n] = m_PlotPosX2;
			yS[n] = k * m_PlotPosX2 + c;
			if ((m_PlotPosY1 - yS[n]) * (m_PlotPosY2 - yS[n]) <= 0)
			{
				n++;
			}

			num = 0;
			for (ind = 0; ind < n; ind++)
			{
				res = IsInSegment(xn, yn, xS[ind], yS[ind]);
				if (res == 1)
				{
					x2[num] = round(xS[ind]);
					y2[num] = round(yS[ind]);
					num++;
				}
			}
			return num;
		}
		int IsInSegment(double[] xn, double[] yn, double xS, double yS)
		{
			if ((xn[0] - xS) * (xn[1] - xS) <= 0)
			{
				if ((yn[0] - yS) * (yn[1] - yS) <= 0)
				{
					return 1;
				}
			}
			return 0;
		}

		public void SetXsize(double xMin, double xSize)
		{ 
			m_xGrMin=xMin;
			m_xGrSize=xSize;
			m_AutoGrid = 0;
			Invalidate();
		}

        public int AddPoints(int lineNum, double[] xData, double[] yData, 
			int constSizeX, int constSizeY)
        {
			string str;
			int ind, len, cnt, oldLen, res, memSize, d=0,dataLen;
			double xFac, yFac, x, y, xmax, ymax, xmin, ymin;

			if (lineNum < 0 || lineNum >= m_pLines.Count )
			{
				str = String.Format( "Line %d does not exists", lineNum);
				//MessageBox( 0 , CString(str) , CString("Message") ,MB_ICONSTOP) ;
				return -1;
			}
			oldLen = m_pLines[lineNum].m_pXData.Count;
			dataLen = xData.Length;
			len = oldLen + dataLen;
			//while (m_PlotDraw == 1) ;
			m_PlotUpdate = 1;
			memSize = len;
			cnt = 0;
			for (ind = oldLen; ind < len; ind++)
			{
				m_pLines[lineNum].m_pXData.Add(xData[cnt]);
				m_pLines[lineNum].m_pYData.Add(yData[cnt]);
				cnt++;
			}
			m_PlotUpdate = 0;
			if (m_xGrSize == 0)
			{
				m_xGrSize = 1;
			}
			if (m_yGrSize == 0)
			{
				m_yGrSize = 1;
			}
			xFac = (m_PlotXSize) / m_xGrSize;
			yFac = (m_PlotYSize) / m_yGrSize;
			xmax = round(m_PlotPosX1 + (xData[0] - m_xGrMin) * xFac);
			ymax = round(m_PlotPosY2 - (yData[0] - m_yGrMin) * yFac);
			xmin = round(m_PlotPosX1 + (xData[0] - m_xGrMin) * xFac);
			ymin = round(m_PlotPosY2 - (yData[0] - m_yGrMin) * yFac);
			cnt = oldLen - 1;
			if (cnt < 0)
			{
				cnt = 0;
			}
			for (ind = cnt; ind < len; ind++)
			{
				x = round(m_PlotPosX1 + (m_pLines[lineNum].XFunc(m_pLines[lineNum].m_pXData[ind])
					- m_xGrMin) * xFac);
				y = round(m_PlotPosY2 - (m_pLines[lineNum].YFunc(m_pLines[lineNum].m_pYData[ind])
					- m_yGrMin) * yFac);
				if (x > xmax)
				{
					xmax = x;
				}
				if (x < xmin)
				{
					xmin = x;
				}
				if (y > ymax)
				{
					ymax = y;
				}
				if (y < ymin)
				{
					ymin = y;
				}
			}

			int top = (int)Math.Round(ymin - 1);
			int bottom = (int)Math.Round(ymax + 1);
			int left = (int)Math.Round(xmin - 1);
			int right = (int)Math.Round(xmax + 1);
			Rectangle rect = new Rectangle(left,top,right-left,bottom-top);

			m_AutoGrid = 0;

			res = IsOutside(rect, ref d);
			if (res == 0)
			{
				Invalidate(rect,true);
				return 1;
			}
			else if (res == 1)
			{
				x = m_xGrSize * 1;
				y = d / xFac * 2;
				if (y > x)
				{
					x = y;
				}
				m_xGrMin = m_xGrMin - x;
				if (constSizeX == 0)
					m_xGrSize = m_xGrSize + x;
			}
			else if (res == 2)
			{
				x = m_xGrSize * 1;
				y = d / xFac * 2;
				if (y > x)
				{
					x = y;
				}
				if (constSizeX == 0)
					m_xGrSize = m_xGrSize + x;
				else if(constSizeX == 2)
					m_xGrMin = m_xGrMin + y;
				else
					m_xGrMin = m_xGrMin + x;
			}
			else if (res == 3)
			{
				x = m_yGrSize * 0.1;
				y = d / yFac * 1.1;
				if (y > x)
				{
					x = y;
				}
				if (constSizeY == 0)
					m_yGrSize = m_yGrSize + x;
				else
					m_yGrMin = m_yGrMin + x;
			}
			else if (res == 4)
			{
				x = m_yGrSize * 0.1;
				y = d / yFac * 1.1;
				if (y > x)
				{
					x = y;
				}
				m_yGrMin = m_yGrMin - x;
				if (constSizeY == 0)
					m_yGrSize = m_yGrSize + x;
			}
			top = m_PlotPosY1;
			bottom = m_PlotPosY2 + 20;
			left = m_PlotPosX1 - 40;
			right = m_PlotPosX2;
			//if (m_pLines[lineNum].m_PointSize > 1)
			{
				rect = new Rectangle(left, top, right - left, bottom - top);
				Invalidate(rect, true);
			}
			return 2;
		}

		int IsOutside(Rectangle rect, ref int d)
		{
			if (m_PlotPosX1 > rect.Left)
			{
				d = m_PlotPosX1 - rect.Left;
				return 1;
			}
			if (m_PlotPosX2 < rect.Right)
			{
				d = rect.Right - m_PlotPosX2;
				return 2;
			}
			if (m_PlotPosY1 > rect.Top)
			{
				d = m_PlotPosY1 - rect.Top;
				return 3;
			}
			if (m_PlotPosY2 < rect.Bottom)
			{
				d = rect.Bottom - m_PlotPosY2;
				return 4;
			}
			return 0;
		}

		public int AddLine(double[] xData, double[] yData,
            Color color, string name,int pointSize=4,float lineWidth=1,int dush=0)
		{
			int ind;
			CLine line = new CLine();
			for (ind = 0; ind < xData.Length; ind++)
			{
				line.m_pXData.Add(xData[ind]);
				line.m_pYData.Add(yData[ind]);
			}
			line.m_Name = name;
			line.m_Color = color;
			line.m_PointSize = pointSize;
			line.m_LineWidth = lineWidth;
			line.m_Dush = dush;
			m_pLines.Add(line);
			return m_pLines.Count-1;
		}

		public void SetLineToolTip(int ind,List<string> toolTipStrs)
        {
			int k;
			if( ind >= m_pLines.Count )
            {
				return;
            }
			if( toolTipStrs == null )
            {
				m_pLines[ind].m_ShowTooltip = 0;
				return;
            }
			for (k = 0; k < toolTipStrs.Count; k++)
			{
				m_pLines[ind].m_ToolTip.Add(toolTipStrs[k]);
			}
		}

		public int DeleteLine(int ind)
		{
			m_pLines.RemoveAt(ind);
			return 0;
		}
		public void DeleteAllLine()
		{
			m_pLines.Clear();
		}

		void CalcGrid(double xMin, double xMax, ref int gridStepsNum, double[] myGrid)
		{
			double dx, n;
			long ind;
			long startG, endG, d;
			dx = xMax - xMin;

			if (dx == 0)
			{
				dx = 1;
				xMin = xMin - 0.5;
				xMax = xMin + 0.5;
			}
			n = 1.0;
			while (dx >= 10.0)
			{
				n = n * 0.1;
				dx = dx * 0.1;
			}
			while (dx < 1.0)
			{
				n = n * 10.0;
				dx = dx * 10.0;
			}
			startG = (long)Math.Ceiling(xMin * n);
			endG = (long)Math.Floor(xMax * n);
			d = endG - startG;
			if (d < 2)
			{
				n = n * 4;
				startG = (long)Math.Ceiling(xMin * n);
				endG = (long)Math.Floor(xMax * n);
				d = endG - startG;
			}
			gridStepsNum = (int)(d + 1);
			for (ind = 0; ind < gridStepsNum; ind++)
			{
				myGrid[ind] = startG / n + ind * 1 / n;
			}
		}

		int round(double x)
		{
			int signFlag, ret;
			signFlag = 1;
			if (x < 0)
			{
				x = -x;
				signFlag = -1;
			}
			ret = signFlag * (int)(x + 0.5);
			return ret;
		}
		int Ceil(double x)
		{
			double frac;
			int signFlag, ret, add, whole;
			signFlag = 1;
			if (x < 0)
			{
				x = -x;
				signFlag = -1;
			}
			whole = (int)x;
			frac = x - whole;
			add = 0;
			if (frac > 1e-6)
			{
				add = 1;
			}
			ret = signFlag * (whole + add);
			return ret;
		}
		string FormatNum(double num, double pExp)
		{
			double num1;
			string str;
			num1 = num * Math.Pow(10.0, -pExp);
			str = String.Format("{0}", num1);
			return str;
		}

		void MaxMin(int x1, int x2, int x3,ref int maxX,ref int midX,ref int minX)
		{
			int k, ind, temp;
			int[] x = new int[3];
			x[0] = x1;
			x[1] = x2;
			x[2] = x3;
			for (ind = 0; ind < 2; ind++)
			{
				for (k = 0; k < 2; k++)
				{
					if (x[k] > x[k + 1])
					{
						temp = x[k];
						x[k] = x[k + 1];
						x[k + 1] = temp;
					}
				}
			}
			maxX = x[2];
			midX = x[1];
			minX = x[0];
		}

		public class CLine
		{
			public string m_Name;
			public Color m_Color;
			public uint m_Fill;
			public int m_ShowTooltip;
			public int m_PointSize;
			public float m_LineWidth;
			public int m_Dush;
			public int m_NoLine;
			public int m_ShowName;
			public double m_XGain;
			public double m_XOffset;
			public double m_YGain;
			public double m_YOffset;
			public List<double> m_pXData = new List<double>();
			public List<double> m_pYData = new List<double>();
			public List<string> m_ToolTip = new List<string>();
			public CLine()
            {
				m_PointSize = 1;
				m_LineWidth = 1;
				m_Dush = 0;
				m_XGain =1.0;
				m_XOffset=0;
				m_YGain=1.0;
				m_YOffset=0;
				m_Fill = 0;
				m_NoLine = 0;
				m_Name = "";
				m_ShowName = 1;
				m_ShowTooltip = 1;
			}
			public double XFunc(double x)
            {
				return m_XGain * (x + m_XOffset);
			}
			public double YFunc(double y)
            {
				return m_YGain * (y + m_YOffset);
			}
		};

		void GetLabelSize(string drawString, ref Size textSize)
		{
			System.Drawing.Font drawFont = new System.Drawing.Font("Calibri", 10);
			textSize = TextRenderer.MeasureText(drawString, drawFont);
		}
		void DrawLabel(Graphics gr,int x, int y, string drawString,Color col,float vert)
		{
			Font drawFont = new System.Drawing.Font("Calibri", 10);
			SolidBrush drawBrush = new System.Drawing.SolidBrush(col);
			StringFormat drawFormat = new System.Drawing.StringFormat();
			if (vert != 0)
			{
				//drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
				gr.TranslateTransform(x, y);
				gr.RotateTransform(vert);
				gr.DrawString(drawString, drawFont, drawBrush, 0, 0, drawFormat);
			}
			else
			{
				gr.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
			}

			gr.ResetTransform();
			drawFont.Dispose();
			drawBrush.Dispose();
		}

		private void Axis_Paint(object sender, PaintEventArgs e)
        {			
			double xDiff, yDiff;
			string str, str1;
			int ind, k,m, x, y, x1, y1, xExp, yExp,
				num, xLegendPos, segCnt, totPointsCnt;
			int[] x2 = new int[4];
			int[] y2 = new int[4];
			double xmax, xmin, ymax, ymin, xFac, yFac, xStep, yStep, xSize, ySize;

			if (m_PlotUpdate == 1)
			{
				return;
			}

			if (m_pLines.Count < 1)
			{
				return;
			}

			if( m_pLines[0].m_pXData.Count < 1 )
            {
				return;
            }

			if (m_AutoGrid == 1 )
			{
				//Calculate automatic scale and offset
				xmin = m_pLines[0].XFunc(m_pLines[0].m_pXData[0]);
				xmax = m_pLines[0].XFunc(m_pLines[0].m_pXData[0]);
				ymin = m_pLines[0].YFunc(m_pLines[0].m_pYData[0]);
				ymax = m_pLines[0].YFunc(m_pLines[0].m_pYData[0]);
				for (ind = 0; ind < m_pLines.Count; ind++)
				{
					for (k = 0; k < m_pLines[ind].m_pXData.Count; k++)
					{
						if (m_pLines[ind].XFunc(m_pLines[ind].m_pXData[k]) < xmin)
						{
							xmin = m_pLines[ind].XFunc(m_pLines[ind].m_pXData[k]);
						}
						if (m_pLines[ind].XFunc(m_pLines[ind].m_pXData[k]) > xmax)
						{
							xmax = m_pLines[ind].XFunc(m_pLines[ind].m_pXData[k]);
						}
						if (m_pLines[ind].YFunc(m_pLines[ind].m_pYData[k]) < ymin)
						{
							ymin = m_pLines[ind].YFunc(m_pLines[ind].m_pYData[k]);
						}
						if (m_pLines[ind].YFunc(m_pLines[ind].m_pYData[k]) > ymax)
						{
							ymax = m_pLines[ind].YFunc(m_pLines[ind].m_pYData[k]);
						}
					}
				}
				xDiff = xmax - xmin;
				yDiff = ymax - ymin;
				if (xDiff == 0)
				{
					xDiff = 1;
					xmin = xmin - 0.5;
				}
				if (yDiff == 0)
				{
					yDiff = 1;
					ymin = ymin - 0.5;
				}
				m_xGrMin = xmin - xDiff * 0.03;
				m_yGrMin = ymin - yDiff * 0.03;
				m_xGrSize = xDiff * 1.06;
				m_yGrSize = yDiff * 1.06;
			}
			else
			{
				xDiff = m_xGrSize;
				yDiff = m_yGrSize;
				xmin = m_xGrMin;
				ymin = m_yGrMin;
			}

			if (m_xGrSize < 0)
			{
				m_xGrSize = 1.0;
			}
			if (m_yGrSize < 0)
			{
				m_yGrSize = 1.0;
			}
			CalcGrid(m_xGrMin, m_xGrMin + m_xGrSize, ref m_XGridStepsNum, m_xGrid);
			CalcGrid(m_yGrMin, m_yGrMin + m_yGrSize, ref m_YGridStepsNum, m_yGrid);

			m_xGrMax = m_xGrMin + m_xGrSize;
			m_yGrMax = m_yGrMin + m_yGrSize;

			if (m_xGrSize == 0)
			{
				m_xGrSize = 1;
			}
			if (m_yGrSize == 0)
			{
				m_yGrSize = 1;
			}

			xmin = 0;
			ymin = 0;
			xmax = 0;
			ymax = 0;
			xExp = 0;
			yExp = 0;
			xSize = 0;
			ySize = 0;
			if (m_xGrMin != 0)
				xmin = Math.Log10(Math.Abs(m_xGrMin));
			if (m_xGrMax != 0)
				xmax = Math.Log10(Math.Abs(m_xGrMax));
			if (m_xGrSize != 0)
				xSize = Math.Log10(Math.Abs(m_xGrSize));
			if (m_yGrMin != 0)
				ymin = Math.Log10(Math.Abs(m_yGrMin));
			if (m_yGrMax != 0)
				ymax = Math.Log10(Math.Abs(m_yGrMax));
			if (m_yGrSize != 0)
				ySize = Math.Log10(Math.Abs(m_yGrSize));
			if (Math.Abs(xSize) > m_ExpMinSize)
			{
				xExp = round(xSize);
			}
			if (Math.Abs(xmin) > m_ExpMinSize && Math.Abs(xmax) > m_ExpMinSize)
			{
				xExp = round(xmax);
			}
			if (Math.Abs(ySize) > m_ExpMinSize)
			{
				yExp = round(ySize);
			}
			if (Math.Abs(ymin) > m_ExpMinSize && Math.Abs(ymax) > m_ExpMinSize)
			{
				yExp = round(ymax);
			}

			xFac = (m_PlotXSize) / m_xGrSize;
			yFac = (m_PlotYSize) / m_yGrSize;

			//Legend
			Size textSize = new Size();
			xLegendPos = m_PlotPosX1 ;
			for (ind = 0; ind < m_pLines.Count; ind++)
			{
				if (m_pLines[ind].m_ShowName == 1)
				{
					GetLabelSize(m_pLines[ind].m_Name, ref textSize);
					DrawLabel(e.Graphics, xLegendPos, m_PlotPosY1 - textSize.Height,
						m_pLines[ind].m_Name, m_pLines[ind].m_Color, 0);
					xLegendPos = xLegendPos + textSize.Width;
				}
			}

			//Draw lines 
			Pen pen;
			SolidBrush brush;
			Graphics formGraphics;
			formGraphics = this.CreateGraphics();
            for (ind = 0; ind < m_pLines.Count; ind++)
            {
				brush = new SolidBrush(m_pLines[ind].m_Color);
				pen = new Pen(m_pLines[ind].m_Color, m_pLines[ind].m_LineWidth);
				pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)m_pLines[ind].m_Dush;

				x1 = round(m_PlotPosX1 + (m_pLines[ind].XFunc(m_pLines[ind].m_pXData[0]) - m_xGrMin) * xFac);
                y1 = round(m_PlotPosY2 - (m_pLines[ind].YFunc(m_pLines[ind].m_pYData[0]) - m_yGrMin) * yFac);
				x = x1;
				y = y1;
                segCnt = 0;
                totPointsCnt = 0;
                NumPointsToDraw[segCnt] = 0;
                if (x1 >= m_PlotPosX1 && x1 <= m_PlotPosX2 &&
                    y1 >= m_PlotPosY1 && y1 <= m_PlotPosY2)
                {
                    NumPointsToDraw[segCnt] = 1;
                    PointsToDrawX[totPointsCnt] = x1;
                    PointsToDrawY[totPointsCnt] = y1;
                    totPointsCnt++;
                    if (m_pLines[ind].m_PointSize > 1)
                    {
						formGraphics.FillEllipse(brush, x1 - m_pLines[ind].m_PointSize / 2, 
							y1 - m_pLines[ind].m_PointSize / 2, 
							m_pLines[ind].m_PointSize, m_pLines[ind].m_PointSize);
                    }
                }
                for (k = 1; k < m_pLines[ind].m_pXData.Count; k++)
                {
                    x = round(m_PlotPosX1 + (m_pLines[ind].XFunc(m_pLines[ind].m_pXData[k]) - m_xGrMin) * xFac);
                    y = round(m_PlotPosY2 - (m_pLines[ind].YFunc(m_pLines[ind].m_pYData[k]) - m_yGrMin) * yFac);
                    if (x1 < m_PlotPosX1 || x1 > m_PlotPosX2 ||
                         y1 < m_PlotPosY1 || y1 > m_PlotPosY2)
                    {//Prev point is out
                        num = FindAxisCross(x1, y1, x, y, x2, y2);
                        if (num == 1)
                        {
                            PointsToDrawX[totPointsCnt] = x2[0];
                            PointsToDrawY[totPointsCnt] = y2[0];
                            totPointsCnt++;
                            PointsToDrawX[totPointsCnt] = x;
                            PointsToDrawY[totPointsCnt] = y;
                            totPointsCnt++;
                            NumPointsToDraw[segCnt] = 2;
                            if (m_pLines[ind].m_PointSize > 1)
                            {
								formGraphics.FillEllipse(brush, x - m_pLines[ind].m_PointSize / 2,
									y - m_pLines[ind].m_PointSize / 2,
									m_pLines[ind].m_PointSize, m_pLines[ind].m_PointSize);
							}
						}
                        if (num == 2)
                        {
                            PointsToDrawX[totPointsCnt] = x2[0];
                            PointsToDrawY[totPointsCnt] = y2[0];
                            totPointsCnt++;
                            PointsToDrawX[totPointsCnt] = x2[1];
                            PointsToDrawY[totPointsCnt] = y2[1];
                            totPointsCnt++;
                            NumPointsToDraw[segCnt] = 2;
                            segCnt++;
                        }
                    }
                    else
                    {//Prev point is in
                        if (x < m_PlotPosX1 || x > m_PlotPosX2 ||
                             y < m_PlotPosY1 || y > m_PlotPosY2)
                        {//Current point is out
                            num = FindAxisCross(x1, y1, x, y, x2, y2);
                            if (num == 1)
                            {
                                PointsToDrawX[totPointsCnt] = x2[0];
                                PointsToDrawY[totPointsCnt] = y2[0];
                                totPointsCnt++;
                                NumPointsToDraw[segCnt]++;
                                segCnt++;
                                //dc->MoveTo( x1 , y1 ) ;
                                //dc->LineTo( x2[0] , y2[0] ) ;
                            }
                            if (num == 2)
                            {
                            }
                        }
                        else
                        {//Current point is in
                            NumPointsToDraw[segCnt]++;
                            //if (totPointsCnt >= PointsToDrawX.Length)
                            //{
                                PointsToDrawX[totPointsCnt] = x;
                                PointsToDrawY[totPointsCnt] = y;
                                totPointsCnt++;
                                //dc->LineTo( x , y ) ;
                                if (m_pLines[ind].m_PointSize > 1)
                                {
                                    formGraphics.FillEllipse(brush, x - m_pLines[ind].m_PointSize / 2,
                                        y - m_pLines[ind].m_PointSize / 2,
                                        m_pLines[ind].m_PointSize, m_pLines[ind].m_PointSize);
                                }
                            //}
                        }
                    }
                    x1 = x;
                    y1 = y;
                }
                if (x > m_PlotPosX1 && x < m_PlotPosX2 &&
                    y > m_PlotPosY1 && y < m_PlotPosY2)
                {
                    segCnt++;
                }

				if (m_pLines[ind].m_Fill == 1)
                {
                    //dc->PolyPolygon(PointsToDraw, (int*)NumPointsToDraw, segCnt);
                }
                else
                {
					if (m_pLines[ind].m_NoLine == 0)
					{
						int pointCnt = 0;
						for (k = 0; k < segCnt; k++)
						{
							Point[] poly;
							if (NumPointsToDraw[k] == 1)
							{
								poly = new Point[2];
							}
                            else
                            {
								poly = new Point[NumPointsToDraw[k]];
                            }
							for(m=0;m< NumPointsToDraw[k];m++ )
                            {
                                if (pointCnt >= PointsToDrawX.Length)
                                {
                                    break;
                                }
                                poly[m].X = PointsToDrawX[pointCnt];
								poly[m].Y = PointsToDrawY[pointCnt];
								pointCnt++;
							}
							if(NumPointsToDraw[k] == 1)
                            {
								poly[1].X = PointsToDrawX[0];
								poly[1].Y = PointsToDrawY[0];
							}
							e.Graphics.DrawLines(pen,poly);
						}
					}
				}
				brush.Dispose();
				pen.Dispose();
			}
			formGraphics.Dispose();

			xStep = (double)m_PlotXSize / m_XGridStepsNum;
			yStep = (double)m_PlotYSize / m_YGridStepsNum;

			//Draw axis with grid
			Color col = Color.FromArgb(100, 100, 100);
			pen = new Pen(col);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            if (m_xGridOn == 1)
            {
                for (ind = 0; ind < m_XGridStepsNum; ind++)
                {
                    x = round(m_PlotPosX1 + (m_xGrid[ind] - m_xGrMin) * xFac);
					if( x < 0 )
                    {
						x = 0;
                    }
					e.Graphics.DrawLine(pen, x, m_PlotPosY1, x, m_PlotPosY2);
					if (ind == 0 || ind == m_XGridStepsNum - 1)
					{
						str = FormatNum(m_xGrid[ind], xExp);
						if (m_LogScaleX == 0)
						{
							GetLabelSize(str, ref textSize);
							if (m_HideXScale == 0)
								DrawLabel(e.Graphics, x - textSize.Width / 2, m_PlotPosY2,
									str, col, 0);
						}
						else
						{
							xmax = Math.Pow(10.0, -xExp);
							xmax = m_xGrid[ind] * xmax;
							xmax = Math.Pow(10.0, xmax);
							str = String.Format("{0}", xmax);
							GetLabelSize(str, ref textSize);
							if (m_HideXScale == 0)
								DrawLabel(e.Graphics, x - textSize.Width / 2, m_PlotPosY2,
									str, col, 0);
						}
					}
                }
            }

			if (m_yGridOn == 1)
			{
				for (ind = 0; ind < m_YGridStepsNum; ind++)
				{
					y = round(m_PlotPosY2 - (m_yGrid[ind] - m_yGrMin) * yFac);
					if( y < 0  )
                    {
						y = 0;
                    }
					e.Graphics.DrawLine(pen, m_PlotPosX1, y, m_PlotPosX2, y);
					if (ind == 0 || ind == m_YGridStepsNum - 1)
					{
						str = FormatNum(m_yGrid[ind], yExp);
						GetLabelSize(str, ref textSize);
						if (m_HideYScale == 0)
							DrawLabel(e.Graphics, m_PlotPosX1 - textSize.Height,
								y + textSize.Width / 2,
								str, col, -90);
					}
                }

                //Y label
                str1 = "";
                if (yExp != 0)
                {
                    str1 = String.Format("*1e{0}", yExp);
                }
                str = m_yLabel + str1;
				GetLabelSize(str, ref textSize);
				if (m_HideYScale == 0)
				{
					DrawLabel(e.Graphics, m_PlotPosX1 - textSize.Height * 2,
					m_PlotPosY1 + m_PlotYSize / 2 + textSize.Width / 2,
					str, col, -90);
				}
			}

			if (m_xGridOn == 1)
			{
				//X label
				str1 = "";
				if (xExp != 0)
				{
					str1 = String.Format("*1e{0}", xExp);
				}
				str = m_xLabel + str1;
				GetLabelSize(str, ref textSize);
				if (m_HideXScale == 0)
				{
					DrawLabel(e.Graphics, m_PlotPosX1 + m_PlotXSize / 2 - textSize.Width / 2,
					m_PlotPosY2 + textSize.Height,
					str, col, 0);
				}
			}

            //Handle zomming rectangle
            if (m_ZoomMode == 1)
            {
				Point[] zoomRect = new Point[5];
				zoomRect[0].X = m_ZoomStartX;
				zoomRect[0].Y = m_ZoomStartY;

				zoomRect[1].X = m_ZoomStartX;
				zoomRect[1].Y = m_ZoomY;

				zoomRect[2].X = m_ZoomX;
				zoomRect[2].Y = m_ZoomY;

				zoomRect[3].X = m_ZoomX;
				zoomRect[3].Y = m_ZoomStartY;

				zoomRect[4].X = m_ZoomStartX;
				zoomRect[4].Y = m_ZoomStartY;

				e.Graphics.DrawLines(pen, zoomRect);
            }
			pen.Dispose();
        }

        private void Axis_SizeChanged(object sender, EventArgs e)
        {
			Size textSize = new Size();
			GetLabelSize("ABC0", ref textSize);
			if (m_HideXScale == 1)
			{
				m_XGraySpace = 0;
			}
			else
			{
				m_XGraySpace = (int)(textSize.Height * 2);
			}
			if (m_HideYScale == 1)
			{
				m_YGraySpace = 0;
			}
			else
			{
				m_YGraySpace = (int)(textSize.Height * 2);
			}
			m_PlotPosX1 = m_XGraySpace;
			m_PlotPosY1 = m_YGraySpace;
			m_PlotPosX2 = Size.Width - m_XGraySpace;
			m_PlotPosY2 = Size.Height - m_YGraySpace;
			m_PlotXSize = m_PlotPosX2 - m_PlotPosX1;
			m_PlotYSize = m_PlotPosY2 - m_PlotPosY1;
			Invalidate();
		}

		double[] x = new double[2];
		double[] y = new double[2];
        private void Axis_MouseDown(object sender, MouseEventArgs e)
        {
			CInput inputDlg = new CInput() ;
			string str;
			double[] val = new double[3];
			int isNumOk=0;
			if (e.Button == MouseButtons.Right)
			{
				if (m_ZoomOn == 0)
					return;
				//if (e.X < m_PlotPosX1 || e.X > m_PlotPosX2)
				//	return;
				//if (e.Y < m_PlotPosY1 || e.Y > m_PlotPosY2)
				//	return;
			}
			else
			{
				if(e.X < m_PlotPosX1 || e.X > m_PlotPosX2 )
					return ;
				if(e.X < m_PlotPosY1 || e.Y > m_PlotPosY2 )
					return ;
				if( m_FindFreq > 0 )
				{
					x[m_FindFreq-1] = m_xGrMin + m_xGrSize *
						(e.X - m_PlotPosX1)/m_PlotXSize ;
					m_FindFreq++;
					if( m_FindFreq >= 3 )
					{
						m_FindFreq = 0 ;
						m_CursorCross = 0 ;
						inputDlg.SetLabel("Enter number of periods") ;
						inputDlg.SetText("2") ;
						if( inputDlg.ShowDialog() == DialogResult.OK )
						{
							inputDlg.GetText(ref val,ref isNumOk);
							if (isNumOk == 1)
							{
								str = String.Format("Frequency is {0}", val[0] / Math.Abs(x[1] - x[0]));
								MessageBox.Show(str);
							}
						}
					}
					return ;
				}
				else if( m_FindSlope > 0 )
				{
					x[m_FindSlope-1] = m_xGrMin + m_xGrSize *
						(e.X - m_PlotPosX1)/m_PlotXSize ;
					y[m_FindSlope-1] = m_yGrMin + m_yGrSize *
						(-e.Y + m_PlotPosY2)/m_PlotYSize ;
					m_FindSlope++;
					if( m_FindSlope >= 3 )
					{
						m_FindSlope = 0 ;
						m_CursorCross = 0 ;
						str = String.Format("Slope is {0}",(y[1]-y[0])/(x[1]-x[0]));
						MessageBox.Show(str);
					}
					return;
				}
				else if( m_FindMean > 0 )
				{
					x[m_FindMean-1] = m_xGrMin + m_xGrSize *
						(e.X - m_PlotPosX1)/m_PlotXSize ;
					y[m_FindMean-1] = m_yGrMin + m_yGrSize *
						(-e.Y + m_PlotPosY2)/m_PlotYSize ;
					m_FindMean++;
					if( m_FindMean >= 3 )
					{
						m_FindMean = 0 ;
						m_CursorCross = 0 ;
						str = String.Format("Mean is {0},{1}",0.5*(x[1]+x[0]),0.5*(y[1]+y[0]));
						MessageBox.Show(str);
					}
					return ;
				}
				else if( m_FindDiffX > 0 )
				{
					x[m_FindDiffX-1] = m_xGrMin + m_xGrSize *
						(e.X - m_PlotPosX1)/m_PlotXSize ;
					y[m_FindDiffX-1] = m_yGrMin + m_yGrSize *
						(-e.Y + m_PlotPosY2)/m_PlotYSize ;
					m_FindDiffX++;
					if( m_FindDiffX >= 3 )
					{
						m_FindDiffX = 0 ;
						m_CursorCross = 0 ;
						str = String.Format("Diff X is {0}",x[1]-x[0]);
						MessageBox.Show(str);
					}
					return ;
				}
				else if( m_FindDiffY > 0 )
				{
					x[m_FindDiffY-1] = m_xGrMin + m_xGrSize *
						(e.X - m_PlotPosX1)/m_PlotXSize ;
					y[m_FindDiffY-1] = m_yGrMin + m_yGrSize *
						(-e.Y + m_PlotPosY2)/m_PlotYSize ;
					m_FindDiffY++;
					if( m_FindDiffY >= 3 )
					{
						m_FindDiffY = 0 ;
						m_CursorCross = 0 ;
						str = String.Format("Diff Y is {0}",y[1]-y[0]);
						MessageBox.Show(str);
					}
					return ;
				}

				if (m_ZoomOn == 0)
				{
					m_MoveMode = 1;
					m_MoveModeStartX = e.X;
					m_MoveModeStartY = e.Y;
					m_MoveModeMinX = m_xGrMin;
					m_MoveModeMinY = m_yGrMin;
					m_StartTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
					return;
				}
				else
				{
					m_ZoomMode = 1;
					m_ZoomStartX = e.X;
					m_ZoomStartY = e.Y;
					m_ZoomXOld = m_ZoomStartX;
					m_ZoomYOld = m_ZoomStartY;
				}
			}
		}

		public double MouseMoveX;
		public double MouseMoveY;
		public int MouseMoveXyUpdate;
		public int MouseMoveOnPoint;
		public int MouseMoveOnLine;

		private void Axis_MouseMove(object sender, MouseEventArgs e)
        {
			double dt;
			Rectangle rect1 = new Rectangle();
			Rectangle rect2 = new Rectangle();
			int maxX=0, midX=0, minX=0, maxY=0, midY=0, minY=0,
				delta, ind, k, lineInd, pointInd;
			double dist, dist1, xDist, yDist, a;
			if (m_CursorCross == 1)
			{
				Cursor = Cursors.Cross;
			}
            else
            {
				Cursor = Cursors.Default;
            }
			MouseMoveX = 0;
			MouseMoveY = 0;
			MouseMoveXyUpdate = 0;
			MouseMoveOnPoint = -1;
			MouseMoveOnLine = -1;
			pointInd = -1;
			lineInd = -1;
			if (e.X > m_PlotPosX1 && e.X < m_PlotPosX2)
			{
				if (e.Y > m_PlotPosY1 && e.Y < m_PlotPosY2)
				{
					if( m_MoveMode == 1 )
                    {
						m_AutoGrid = 0;
						m_xGrMin = m_MoveModeMinX - (e.X - m_MoveModeStartX)*m_xGrSize/m_PlotXSize;
						m_yGrMin = m_MoveModeMinY + (e.Y - m_MoveModeStartY)*m_yGrSize/m_PlotYSize;
						dt = DateTime.Now.TimeOfDay.TotalMilliseconds - m_StartTime;
						if ( dt > 200.0 ) 
						{
							m_StartTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
							Invalidate();
						}
						return;
					}
					if (m_PlotYSize != 0 && m_PlotXSize != 0)
					{
						MouseMoveXyUpdate = 1;
						MouseMoveX = m_xGrMin + m_xGrSize * (e.X - m_PlotPosX1) / m_PlotXSize;
						MouseMoveY = m_yGrMin + m_yGrSize * (-e.Y + m_PlotPosY2) / m_PlotYSize;
						if (MouseMoveOnPoint == -1 && MouseMoveOnLine == -1)
						{
							dist = 1e9;
							for (ind = 0; ind < m_pLines.Count; ind++)
							{
								for (k = 0; k < m_pLines[ind].m_pXData.Count; k++)
								{
									xDist = (m_pLines[ind].XFunc(m_pLines[ind].m_pXData[k]) - MouseMoveX) * m_PlotXSize / m_xGrSize;
									xDist = xDist * xDist;
									yDist = (m_pLines[ind].YFunc(m_pLines[ind].m_pYData[k]) - MouseMoveY) * m_PlotYSize / m_yGrSize;
									yDist = yDist * yDist;
									dist1 = Math.Sqrt(xDist + yDist);
									a = Math.Sqrt(m_PlotXSize * m_PlotXSize + m_PlotYSize * m_PlotYSize);
									if (dist1 < a * m_HitPointTolPer)
									{
										if (dist1 < dist)
										{
											dist = dist1;
											pointInd = k;
											lineInd = ind;
										}
									}
								}
							}
							if (pointInd >= 0)
							{
								MouseMoveOnPoint = pointInd;
								MouseMoveOnLine = lineInd;
								if (m_pLines[lineInd].m_ShowTooltip == 1)
								{	
									if (pointInd >= m_pLines[lineInd].m_ToolTip.Count)
									{ 
										string str = String.Format("{0},{1}", m_pLines[lineInd].m_pXData[pointInd],
										m_pLines[lineInd].m_pYData[pointInd]);
										toolTipAxis.SetToolTip(this, str);
									}
									else
									{
										toolTipAxis.SetToolTip(this, m_pLines[lineInd].m_ToolTip[pointInd]);
									}
								}
							}
							else
							{
								toolTipAxis.SetToolTip(this, "");
							}
						}
					}
				}
			}

			if (m_ZoomMode != 1)
			{
				return;
			}

			m_ZoomX = e.X;
			m_ZoomY = e.Y;
			delta = 1;

			MaxMin(m_ZoomStartX, m_ZoomXOld, m_ZoomX, ref maxX, ref midX, ref minX);
			MaxMin(m_ZoomStartY, m_ZoomYOld, m_ZoomY, ref maxY, ref midY, ref minY);

			rect1.X = minX;
			rect1.Width = maxX + delta - minX;
			if (m_ZoomStartY < m_ZoomY)
			{
				rect1.Y = midY;
				rect1.Height = maxY + delta - midY;
			}
			else
			{
				rect1.Y = minY;
				rect1.Height = midY + delta - minY;
			}

			rect2.Y = minY;
			rect2.Height = maxY + delta - minY;
			if (m_ZoomStartX < m_ZoomX)
			{
				rect2.X = midX;
				rect2.Width = maxX + delta - midX;
			}
			else
			{
				rect2.X = minX;
				rect2.Width = midX + delta - minX;
			}

			m_ZoomXOld = m_ZoomX;
			m_ZoomYOld = m_ZoomY;

			Invalidate(rect1, true);
			Invalidate(rect2, true);
		}

        private void Axis_MouseUp(object sender, MouseEventArgs e)
        {
			if (e.Button == MouseButtons.Right)
			{
			}
			else
			{
				m_MoveMode = 0;
				if (m_MoveMode == 1)
				{
					Invalidate();
					return;
				}
				if (m_ZoomOn == 0)
					return;
				if (m_ZoomMode == 0)
					return;
				if (m_ZoomStartX - e.X == 0)
				{
					m_ZoomMode = 0;
					return;
				}
				if (m_ZoomStartY - e.Y == 0)
				{
					m_ZoomMode = 0;
					return;
				}
				OnLBUp(e.X, e.Y);
			}
		}

		void OnLBUp(int x, int y)
		{
			double dx, dy, xm, ym;
			//Rectangle rect = new Rectangle();
			m_AutoGrid = 0;
			m_ZoomX = x;
			m_ZoomY = y;
			if (m_ZoomStartY < m_ZoomY)
			{
				dy = m_ZoomY - m_ZoomStartY;
				ym = m_PlotPosY2 - m_ZoomY;
			}
			else
			{
				dy = m_ZoomStartY - m_ZoomY;
				ym = m_PlotPosY2 - m_ZoomStartY;
			}
			if (m_ZoomStartX < m_ZoomX)
			{
				dx = m_ZoomX - m_ZoomStartX;
				xm = m_ZoomStartX - m_PlotPosX1;
			}
			else
			{
				dx = m_ZoomStartX - m_ZoomX;
				xm = m_ZoomX - m_PlotPosX1;
			}

			m_xGrMin = m_xGrMin + (m_xGrSize * xm) / m_PlotXSize;
			m_yGrMin = m_yGrMin + (m_yGrSize * ym) / m_PlotYSize;
			m_xGrSize = (m_xGrSize * dx) / m_PlotXSize;
			m_yGrSize = (m_yGrSize * dy) / m_PlotYSize;

			//rect.top = m_PlotPosY1;
			//rect.bottom = m_PlotPosY2;
			//rect.left = m_PlotPosX1;
			//rect.right = m_PlotPosX2;
			//RedrawWindow(m_Parent, 0, 0, RDW_ERASE | RDW_INVALIDATE);
			ParentAxList.MouseUp();
			m_ZoomMode = 0;
			Invalidate();
		}

		public void ZoomOut()
        {
			//CRect rect;
			//if (m_ZoomOn == 0)
			//	return;
			//if (e.X < m_PlotPosX1 || e.X > m_PlotPosX2)
			//	return;
			//if (e.Y < m_PlotPosY1 || e.Y > m_PlotPosY2)
			//	return;
			m_AutoGrid = 1;
			m_ZoomMode = 0;
			m_MoveMode = 0;
			//rect.top = m_PlotPosY1;
			//rect.bottom = m_PlotPosY2;
			//rect.left = m_PlotPosX1;
			//rect.right = m_PlotPosX2;
			Invalidate();
		}

		private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
			ZoomOut();
			if (Parent == null)
			{
				return;
			}
			ParentAxList.ZoomOut();
		}
        private void xDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
			m_CursorCross = 1;
			m_FindFreq = 0;
			m_FindSlope = 0;
			m_FindDiffX = 1;
			m_FindDiffY = 0;
			m_FindMean = 0;
		}
        private void yDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
			m_CursorCross = 1;
			m_FindFreq = 0;
			m_FindSlope = 0;
			m_FindDiffX = 0;
			m_FindDiffY = 1;
			m_FindMean = 0;
		}
        private void slopeToolStripMenuItem_Click(object sender, EventArgs e)
        {
			m_CursorCross = 1;
			m_FindFreq = 0;
			m_FindSlope = 1;
			m_FindDiffX = 0;
			m_FindDiffY = 0;
			m_FindMean = 0;
		}
		private void frequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
			m_CursorCross = 1;
			m_FindFreq = 1;
			m_FindSlope = 0;
			m_FindDiffX = 0;
			m_FindDiffY = 0;
			m_FindMean = 0;
		}
		private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
			m_CursorCross = 1;
			m_FindFreq = 0;
			m_FindSlope = 0;
			m_FindDiffX = 0;
			m_FindDiffY = 0;
			m_FindMean = 1;
		}			
		private void fFTToolStripMenuItem_Click(object sender, EventArgs e)
        {
			int ind, k, n, lenH;
			double[] fft;
			double[] freq;
			double ts,coefR,coefI;
			string str;
			int figInd = CFigList.NewFig(33, 1, 1, "FFT");
			for (ind = 0; ind < m_pLines.Count; ind++)
			{
				if (m_pLines[ind].m_pXData.Count < 10)
				{
					continue;
				}
				lenH = (int)Math.Floor((m_pLines[ind].m_pXData.Count - 1.0) * 0.5);
				fft = new double[lenH];
				freq = new double[lenH];
				ts = m_pLines[ind].XFunc(m_pLines[ind].m_pXData[1]) -
					m_pLines[ind].XFunc(m_pLines[ind].m_pXData[0]);
				for (k = 1; k < lenH; k++)
				{
					coefR = 0;
					coefI = 0;
					for (n = 0; n < m_pLines[ind].m_pXData.Count; n++)
					{
						coefR += m_pLines[ind].YFunc(m_pLines[ind].m_pYData[n]) *
							Math.Cos(-2.0 * Math.PI * k * n / m_pLines[ind].m_pXData.Count);
						coefI += m_pLines[ind].YFunc(m_pLines[ind].m_pYData[n]) *
							Math.Sin(-2.0 * Math.PI * k * n / m_pLines[ind].m_pXData.Count);
					}
					fft[k - 1] = 2.0 * Math.Sqrt(coefR * coefR + coefI * coefI) / m_pLines[ind].m_pXData.Count;
					freq[k - 1] = (k) / (m_pLines[ind].m_pXData.Count * ts);
				}
				str = String.Format("fft({0})", m_pLines[ind].m_Name);
				CFigList.FigList[figInd].FigAxisList.AddLine(0, freq, fft,m_pLines[ind].m_Color,str);
			}
			CFigList.FigList[figInd].Show();
		}

		public void OnFileOpen(string fName, int add) 
		{
			int  varNum;
			int res, ind , cnt = 0 ;
			string name1 ="", name2 = "";
			string str;
			double[] pDataX = new double[1];
			double[] pDataY = new double[1];
			double d;
			string[] words;
			if ( add == 0 )
            {
				DeleteAllLine();
			}
			try
			{
				if (fName.Contains(".maf") || fName.Contains(".mat"))
				{
					BinaryReader reader = new BinaryReader(File.Open(fName, FileMode.Open));
					ind = 0;
					while( true )
					{
						res = CFigList.GetNextMat(reader,ref pDataX,ref cnt,ref name1);
						if (res != 1)
						{
							break;
						}
						if (name1 == "NumOfFigs")
						{
							continue;
						}
						if (name1 == "FigsPlots")
						{
							continue;
						}
						res = CFigList.GetNextMat(reader, ref pDataY, ref cnt, ref name2);
						if (res != 1)
						{
							break;
						}
						AddLine(pDataX, pDataY, CFigList.ColorArr[ind & 7], name2);
						ind++;
					}
					reader.Close();
				}
				else
				{
					StreamReader sw = new StreamReader(fName);
					varNum = 0;
                    while ( true )
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
								d= Double.Parse(words[ind]);
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
							if(pDataX.Length == pDataY.Length )
							AddLine(pDataX, pDataY, CFigList.ColorArr[(varNum>>1) & 7], name1);
						}
					}
					sw.Close();
                }
				ZoomOut();
			}
            catch
            {
            }
		}

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			OpenFileDialog fod = new OpenFileDialog();
			fod.InitialDirectory = CFigList.CurrentPath;
			fod.Title = "Open file";
			fod.Filter = "maf|*.maf|csv|*.csv";
			if (fod.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			CFigList.CurrentPath = fod.FileName.Remove(fod.FileName.Length - fod.SafeFileName.Length);
			OnFileOpen(fod.FileName, 0);
		}

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			OpenFileDialog fod = new OpenFileDialog();
			fod.InitialDirectory = CFigList.CurrentPath;
			fod.Title = "Open file";
			fod.Filter = "maf|*.maf|csv|*.csv";
			if (fod.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			CFigList.CurrentPath = fod.FileName.Remove(fod.FileName.Length - fod.SafeFileName.Length);
			OnFileOpen(fod.FileName, 1);
		}

		void OnFileSaveMat(string fName, int add )
		{
			int ind;
			BinaryWriter sw;
			if ( add == 0 )
			{
				sw = new BinaryWriter(File.Open(fName, FileMode.Create));
			}
            else
            {
				sw = new BinaryWriter(File.Open(fName, FileMode.Append));
			}
			for (ind = 0; ind < m_pLines.Count; ind++)
			{
				CFigList.SaveMatDArray(sw, m_pLines[ind].m_Name+"X", m_pLines[ind].m_pXData);
				CFigList.SaveMatDArray(sw, m_pLines[ind].m_Name, m_pLines[ind].m_pYData);
			}
			sw.Close();
		}

		void OnFileSaveCsv(string fName, int add ) 
		{
			int ind,k;
			string str;
			StreamWriter sw;
			if (add == 0)
			{
				sw = new StreamWriter(new FileStream(fName, FileMode.Create));
			}
			else
			{
				sw = new StreamWriter(new FileStream(fName, FileMode.Append)) ;
			}
			for (ind = 0; ind < m_pLines.Count; ind++)
			{
				str = m_pLines[ind].m_Name + "X";
				for (k = 0; k < m_pLines[ind].m_pXData.Count; k++)
				{
					str = str + "," + m_pLines[ind].m_pXData[k];
				}
				sw.WriteLine(str);
				str = m_pLines[ind].m_Name;
				for (k = 0; k < m_pLines[ind].m_pXData.Count; k++)
				{
					str = str + "," + m_pLines[ind].m_pYData[k];
				}
				sw.WriteLine(str);
			}
			sw.Close();
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			SaveFileDialog fod = new SaveFileDialog();
			fod.InitialDirectory = CFigList.CurrentPath;
			fod.Title = "Save file";
			fod.Filter = "maf|*.maf|csv|*.csv|all|*.*";
			if (fod.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			CFigList.CurrentPath = Path.GetDirectoryName(fod.FileName);
			string extStr = Path.GetExtension(fod.FileName);
			if (extStr == ".maf" || extStr == ".mat" || extStr == "" )
			{
				string fName= fod.FileName;
				if ( extStr == "" )
                {
					fName = fName + ".maf";
				}
				OnFileSaveMat(fName, 0);
			}
            else 
			{
				OnFileSaveCsv(fod.FileName, 0);
			}
		}

		private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int ind,k,cnt, figCnt, numOfFigs, NUM_OF_PLOTS,m;
			SaveFileDialog fod = new SaveFileDialog();
			fod.InitialDirectory = CFigList.CurrentPath;
			fod.Title = "Save file";
			fod.Filter = "maf|*.maf";
			if ( fod.ShowDialog() != System.Windows.Forms.DialogResult.OK )
			{
				return;
			}
			CFigList.CurrentPath = Path.GetDirectoryName(fod.FileName);
			string extStr = Path.GetExtension(fod.FileName);
			string fName = fod.FileName;
			if ( extStr != ".maf" )
			{
				fName = fName + ".maf";				
			}

			List<double> temp = new List<double>();
			BinaryWriter sw;
			sw = new BinaryWriter(File.Open(fName, FileMode.Create));
			numOfFigs = Convert.ToInt32(CFigList.FigList.Count);
			temp.Add(numOfFigs) ;
			CFigList.SaveMatDArray(sw, "NumOfFigs", temp);
			temp.Clear();
			NUM_OF_PLOTS = 100;
			for ( ind =  0; ind < (NUM_OF_PLOTS + 4) * numOfFigs; ind++ )
				temp.Add(0);
			figCnt = 0; 
			for (cnt = 0; cnt < CFigList.FigList.Count; cnt++)
			{
				//FigsPlots variable format: figure number, sub plot dimentions, number of signals, plot number of signal ....
				temp[numOfFigs * 0 + figCnt] = (double)cnt;
				temp[numOfFigs * 1 + figCnt] = CFigList.FigList[cnt].FigAxisList.NumOfRows;
				temp[numOfFigs * 2 + figCnt] = CFigList.FigList[cnt].FigAxisList.NumOfCols;
				m = 0;
				for (ind = 0; ind < CFigList.FigList[cnt].FigAxisList.Axes.Count; ind++)
				{
					for (k = 0; k < CFigList.FigList[cnt].FigAxisList.Axes[ind].m_pLines.Count; k++)//Figs.Figures[cnt]->m_Plot[ind].m_LinesNumberMax
					{
						if (CFigList.FigList[cnt].FigAxisList.Axes[ind].m_pLines[k].m_pXData.Count == 0)//Figs.Figures[cnt]->m_Plot[ind].m_pLines[k].m_pXData
						{
							continue;
						}
						temp[numOfFigs * (4 + m) + figCnt] = ind;
						m++;
					}
				}
				temp[numOfFigs * 3 + figCnt] = m;
				figCnt++;			
			}
			CFigList.SaveMatDArray(sw, "FigsPlots", temp);
			sw.Close();
			for (ind = 0; ind < CFigList.FigList.Count; ind++)
			{
				for (k = 0; k < CFigList.FigList[ind].FigAxisList.Axes.Count; k++)
				{
					CFigList.FigList[ind].FigAxisList.Axes[k].OnFileSaveMat(fName,1);
				}
			}
		}

		private void addToolStripMenuItem2_Click(object sender, EventArgs e)
        {
			SaveFileDialog fod = new SaveFileDialog();
			fod.InitialDirectory = CFigList.CurrentPath;
			fod.Title = "Save file";
			fod.Filter = "maf|*.maf|csv|*.csv|all|*.*";
			if (fod.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			CFigList.CurrentPath = Path.GetDirectoryName(fod.FileName);
			string extStr = Path.GetExtension(fod.FileName);
			if (extStr == ".maf" || extStr == ".mat")
			{
				OnFileSaveMat(fod.FileName, 1);
			}
			else
			{
				OnFileSaveCsv(fod.FileName, 1);
			}
		}

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
			int isNumOk=0;
			double[] val = new double[3];
			CInput inputDlg = new CInput();
			inputDlg.SetLabel("Enter subplot dimentions (m,n)");
			inputDlg.SetText("1,1");
			if (inputDlg.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			inputDlg.GetText(ref val, ref isNumOk);
			if (isNumOk < 2 )
			{
				MessageBox.Show("Enter subplot dimentions (m,n)");
				return;
			}
			int figInd = CFigList.NewFig(33,(int)val[0], (int)val[1], "Figure");
			CFigList.FigList[figInd].Show();
		}

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
			CFigList.CloseAll();
		}

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (zoomToolStripMenuItem.Checked == true)
			{
				m_ZoomOn = 0;
				zoomToolStripMenuItem.Checked = false;
			}
			else
			{
				m_ZoomOn = 1;
				zoomToolStripMenuItem.Checked = true;
			}
		}

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Prop propWin = new Prop(this);
			propWin.ShowDialog();
		}

        private void openAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
			OpenFileDialog fod = new OpenFileDialog();
			fod.InitialDirectory = CFigList.CurrentPath;
			fod.Title = "Open file";
			fod.Filter = "maf|*.maf|csv|*.csv";
			if (fod.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			CFigList.CurrentPath = fod.FileName.Remove(fod.FileName.Length - fod.SafeFileName.Length);
			OnFileOpenAll(fod.FileName);
		}

		public void OnFileOpenAll(string fName)
		{
			int varNum, numOfFigs=0,m, firstVar=0,k;
			int res, ind, cnt = 0,figInd,lineInd,fInd;
			int MAX_NUM_FIGS=100, NUM_OF_PLOTS=100, wasFigsPlots=0;
			string name1 = "", name2 = "";
			string str;
			double[] pDataX = new double[1];
			double[] pDataY = new double[1];
			int[,] figsPlots = new int[100,105];
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
						figsPlots[m,0] = ParentAxList.AxFigNum;
						figsPlots[m,1] = 1;
						figsPlots[m,2] = 1;
						figsPlots[m,3] = NUM_OF_PLOTS;
						for (cnt = 0; cnt < NUM_OF_PLOTS; cnt++)
						{
							figsPlots[m,4 + cnt] = 0;
						}
					}
					numOfFigs = 1; 
					while (true)
					{
						res = CFigList.GetNextMat(reader, ref pDataX, ref cnt, ref name1);
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
								res = CFigList.GetNextMat(reader, ref pDataX, ref cnt, ref name1);
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
										CFigList.CloseFig(figsPlots[m,0]);
										fInd = CFigList.NewFig(figsPlots[m, 0], figsPlots[m, 1],
											figsPlots[m, 2],"Figure");
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
						res = CFigList.GetNextMat(reader, ref pDataY, ref cnt, ref name2);
						if (res != 1)
						{
							break;
						}
						if(wasFigsPlots == 1)
						{
							fInd = CFigList.FindFigNum(figsPlots[figInd,0]);
							k = figsPlots[figInd, 4+ lineInd];
							CFigList.FigList[fInd].FigAxisList.Axes[k].AddLine(pDataX, pDataY, CFigList.ColorArr[lineInd & 7], name2);
							//SetCurrFigure(figsPlots[figInd][0]);
							//SubPlot(figsPlots[figInd][1], figsPlots[figInd][2]);
							//Plot(dPtr1, dPtr2, cnt, col[lineInd % 8], name2, figsPlots[figInd][4 + lineInd]);
							//HoldOn(1);
							lineInd++;
							if (lineInd == figsPlots[figInd,3])//numOfLines
							{
								lineInd = 0;
								figInd++;
							}
						}
						else
                        {
							AddLine(pDataX, pDataY, CFigList.ColorArr[ind & 7], name2);
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
								AddLine(pDataX, pDataY, CFigList.ColorArr[(varNum >> 1) & 7], name1);
						}
					}
					sw.Close();
				}
				ZoomOut();
			}
			catch
			{
			}
		}

		protected bool GetFilename(out string filename, DragEventArgs e)
		{
			bool ret = false;
			filename = String.Empty;

			if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
			{
				Array data = ((IDataObject)e.Data).GetData("FileName") as Array;
				if (data != null)
				{
					if ((data.Length == 1) && (data.GetValue(0) is String))
					{
						filename = ((string[])data)[0];
						string ext = Path.GetExtension(filename).ToLower();
						if ((ext == ".maf") || (ext == ".mat") || (ext == ".csv"))
						{
							ret = true;
						}
					}
				}
			}
			return ret;
		}

		private void Axis_DragDrop(object sender, DragEventArgs e)
        {
			string filename;
			bool validData = GetFilename(out filename, e);
			OnFileOpen(filename, 1);
		}

		private void Axis_DragEnter(object sender, DragEventArgs e)
        {
			e.Effect = DragDropEffects.Copy;
		}

        private void Axis_DragLeave(object sender, EventArgs e)
        {
        }

        private void Axis_DragOver(object sender, DragEventArgs e)
        {
		}
	}
}

namespace AxisList  
{
	public class AxisList
	{
		public int AxFigNum;
		public int NumOfRows;
		public int NumOfCols;
		public List<Axis.Axis> Axes = new List<Axis.Axis>();
		public AxisList(int figNum)
		{
			AxFigNum = figNum;
			NumOfRows = 0;
			NumOfCols = 0;
			Axes.Clear();
		}

		public void InitTableLayoutPanel(TableLayoutPanel tableLayoutPan, int rows, int cols)
		{
			float per;
			int ind, m, n;
			Axes = new List<Axis.Axis>();
			tableLayoutPan.ColumnStyles.Clear();
			tableLayoutPan.ColumnCount = cols;
			per = 100.0f / cols;
			for (ind = 0; ind < tableLayoutPan.ColumnCount; ind++)
			{
				tableLayoutPan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(
					System.Windows.Forms.SizeType.Percent, per));
			}
			tableLayoutPan.RowStyles.Clear();
			tableLayoutPan.RowCount = rows;
			per = 100.0f / rows;
			for (ind = 0; ind < tableLayoutPan.RowCount; ind++)
			{
				tableLayoutPan.RowStyles.Add(new System.Windows.Forms.RowStyle(
					System.Windows.Forms.SizeType.Percent, per));
			}
			NumOfRows = rows;
			NumOfCols = cols;
			for (n = 0; n < cols; n++)
			{
				for (m = 0; m < rows; m++)
				{
					Axes.Add(new Axis.Axis(this,m));
					ind = rows * n + m;
					tableLayoutPan.Controls.Add(Axes[ind], n, m);
					Axes[ind].Dock = DockStyle.Fill;
				}
			}
		}

		public void ZoomOut()
		{
			int k;
			for (k = 0; k < Axes.Count; k++)
			{
				Axes[k].ZoomOut();
			}
		}

		public void MouseUp()
		{
			if (Axes.Count < 2 )
			{
				return;
			}
			int k, res = 0, ind = 0;
			for (k = 0; k < Axes.Count; k++)
			{
				if (Axes[k].m_ZoomMode == 1 && Axes[k].m_ZoomSync == 1 )
				{
					res = 1;
					ind = k;
					break;
				}
			}
			if (res == 1)
			{
				for (k = 0; k < Axes.Count; k++)
				{
					if (ind != k)
					{
						if (Axes[k].m_ZoomSync == 1)
						{
							Axes[k].m_xGrMin = Axes[ind].m_xGrMin;
							Axes[k].m_xGrSize = Axes[ind].m_xGrSize;
							Axes[k].m_AutoGrid = 0;
							Axes[k].Invalidate();
						}
					}
				}
			}
		}

		public int DeleteAllLine(int axisInd)
		{
			if (axisInd >= Axes.Count)
			{
				return -1;
			}
			Axes[axisInd].DeleteAllLine();
			return 0;
		}

		public int AddLine(int axisInd, double[] xData, double[] yData,
			Color col, string name, int pointSize = 4,float lineWidth=1,int dush=0)
		{
			int lineInd;
			if (axisInd >= Axes.Count)
			{
				return -1;
			}
			lineInd = Axes[axisInd].AddLine(xData, yData, col, name, pointSize, lineWidth,dush);
			return lineInd;
		}
		public int AddPoints(int axisInd, int lineNum, double[] xData, double[] yData,
			int constSizeX, int constSizeY)
		{
			int res;
			if (axisInd >= Axes.Count)
			{
				return -1;
			}
			res = Axes[axisInd].AddPoints(lineNum, xData, yData, constSizeX, constSizeY);
			return res;
		}
		public int AddPoints(int axisInd, string lineName, double[] xData, double[] yData,
			int constSizeX, int constSizeY)
		{
			int ind, res;
			if (axisInd >= Axes.Count)
			{
				return -1;
			}
			for (ind = 0; ind < Axes[axisInd].m_pLines.Count; ind++)
			{
				if (lineName == Axes[axisInd].m_pLines[ind].m_Name)
				{
					break;
				}
			}
			if (ind >= Axes[axisInd].m_pLines.Count)
			{
				return -1;
			}
			res = Axes[axisInd].AddPoints(ind, xData, yData, constSizeX, constSizeY);
			return res;
		}

		public int SetXsize(int axisInd, double xMin, double xSize)
		{
			if (axisInd >= Axes.Count)
			{
				return -1;
			}
			Axes[axisInd].SetXsize(xMin,xSize);
			return 1;
		}

		public int SetLineToolTip(int axisInd, int lineInd, List<string> toolTipStrs)
		{
			if (axisInd >= Axes.Count)
			{
				return -1;
			}
			Axes[axisInd].SetLineToolTip(lineInd, toolTipStrs);
			return 0;
		}
		
		public void XLabel(int axInd, string str)
		{
			if (axInd >= Axes.Count)
			{
				return;
			}
			Axes[axInd].XLabel(str);
		}
		public void SetLogScaleX(int axInd, int logScaleX)
		{
			if (axInd >= Axes.Count)
			{
				return;
			}
			Axes[axInd].SetLogScaleX(logScaleX);
		}

		public void InvalidateAxis(int axInd)
		{
			if (axInd >= Axes.Count)
			{
				return;
			}
			Axes[axInd].Invalidate();
		}

		public void YLabel(int axInd, string str)
		{
			if (axInd >= Axes.Count)
			{
				return;
			}
			Axes[axInd].YLabel(str);
		}
	}
}