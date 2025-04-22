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
using Kvaser.CanLib;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

//System.Runtime.InteropServices

namespace TesterGUI
{
    public class KvaserCom
    {

        static void ErrorDump(string id, Canlib.canStatus stat, bool quit)
        {
            string buf = "";
            if (stat != Canlib.canStatus.canOK)
            {
                Canlib.canGetErrorText(stat, out buf);
                Console.WriteLine("{0}: failed, stat={1} ({2})", id, (int)stat, buf);
                Thread.Sleep(5000);
                if (quit) Environment.Exit(1);
            }
        }  // end of ErrorDump()
        static void CloseChannel(int chHndl)
        {
            Canlib.canStatus status;

            // take the channel offline
            status = Canlib.canBusOff(chHndl);
            ErrorDump("canBusOff", status, false);

            // free the channel handle
            status = Canlib.canClose(chHndl);
            ErrorDump("canClose", status, false);
        }  // end of CloseChannel()

        static void InitChannel(int chNum, ref int chHndl)
        {
            Canlib.canStatus status;

            // Because we are working on timing we want the channel exclusive
            chHndl = Canlib.canOpenChannel(chNum, Canlib.canOPEN_ACCEPT_VIRTUAL);
            if (chHndl < 0)
                ErrorDump("canOpenChannel", (Canlib.canStatus)(chHndl), true);

            // decided to use the default 250 kBits/sec
            status = Canlib.canSetBusParams(chHndl, Canlib.canBITRATE_500K, 0, 0, 0, 0);
            ErrorDump("canSetBusParams", status, true);
        }  // end of InitChannel()

        int KchannelCount;
        object buffer;
        object Kserial;
        private void Kvaser_Click(object sender, EventArgs e)
        {
            Canlib.canInitializeLibrary();
            int V = Canlib.canGetVersionEx(Canlib.canVERSION_CANLIB32_PRODVER32);
            Canlib.canGetNumberOfChannels(out KchannelCount);
            for (int Kcnt = 0; Kcnt < KchannelCount; Kcnt++)
            {
                Canlib.canGetChannelData(Kcnt, Canlib.canCHANNELDATA_DEVDESCR_ASCII, out buffer);
                Canlib.canGetChannelData(Kcnt, Canlib.canCHANNELDATA_CARD_SERIAL_NO, out Kserial);
            }
            //int rxChannel = 0;
            //int handle = 0 ; 
            //InitChannel(rxChannel, ref handle);
        }
    }

    public class MathLib
    {
        private void MathLib_Click(object sender, EventArgs e)
        {
            var matrixA = Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 2},
                {3, 4}
            });
            var inverse = matrixA.Inverse(); // Inverse 
            double[,] array = inverse.ToArray();

            double[] xData = { 1, 2, 3, 4, 5 };
            double[] yData = { 1.2, 1.9, 3.0, 4.1, 5.1 };

            // Fit a polynomial of degree 2
            int degree = 2;
            double[] coefficients = Fit.Polynomial(xData, yData, degree);

        }
    }

}
