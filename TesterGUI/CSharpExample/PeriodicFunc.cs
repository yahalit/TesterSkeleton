using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvsGUI
{
    public partial class CPvsGUIold
    {

        private readonly object _lock = new object();
        int NoCommCounter = 0;
        bool BitInfoInit = false;
        bool DeviceConfigInfoInit = false;

        private void PeriodicFunc(Object myObject,
            EventArgs myEventArgs)
        {
            bool lockTaken = false;
            try
            {
                // Try to acquire the lock
                System.Threading.Monitor.TryEnter(_lock, ref lockTaken);
                if (!lockTaken)
                {
                    return; // If another thread has the lock, exit the Tick event
                }
                PeriodicFuncBody(myObject, myEventArgs);
            }
            finally
            {
                if (lockTaken)
                {
                    // Release the lock if acquired
                    System.Threading.Monitor.Exit(_lock);
                }
            }
        }

        string SimulationStatusString(uint stat)
        {
            switch (stat)
            {
                case 0:
                    return "Disabled"; 
                case 1:
                    return "Normal";
                case 2:
                    return "FullyOpen";
                case 3:
                    return "FullyClosed";
                case 4:
                    return "TimeDelay";
                case 5:
                    return "SlowReaction";
                default:
                    return "Unknown";
            }

        }

        private void PeriodicFuncBody(Object myObject, EventArgs myEventArgs)
        {
            bool CommGood = false;

            mutex.WaitOne();
            if (IsComOpen)
            {

                Interpreter.HostCom.InterceptCOMMsg();
                Interpreter.HostCom.TransmitComMsg();

                // See if there is any periodic update , not destroying any accepted message
                Interpreter.UpdatePeriodicStatus();
                if (Interpreter.PeriodicUpdateAvailable)
                {
                    CommGood = true;
                    labelPressure1.Text = Interpreter.PressureRead[0].ToString();
                    labelPressure2.Text = Interpreter.PressureRead[1].ToString();
                    labelPressure3.Text = Interpreter.PressureRead[2].ToString();
                    labelPressure4.Text = Interpreter.PressureRead[3].ToString();

                    if (Interpreter.RemainTimeRead > 1.0e15)
                        labelTimeRemain.Text = "Infinity";
                    else
                        labelTimeRemain.Text = (Interpreter.RemainTimeRead * 1e-6) .ToString("F3");
                    labelTimeNow.Text = (Interpreter.HostTimeRead * 1e-6).ToString("F3");
                    //DecodePeriodicSimState(HostCom.Messages[c1].Buf, out HostTimeRead, out , ValveSimStateRead);
                    for (int cnt = 0; cnt < GridNames.Length; cnt++)
                    {
                        if (Interpreter.BitDetail[cnt].installed)
                        {
                            dataGridView1.Rows[cnt].Cells[0].Value = Interpreter.SimFaultModeRead[cnt];
                            dataGridView1.Rows[cnt].Cells[1].Value = Interpreter.CurRead[cnt];
                            dataGridView1.Rows[cnt].Cells[2].Value = Interpreter.PosRead[cnt];
                            if (Interpreter.FaultCodeRead[cnt] == 0xf000 )
                            {
                                dataGridView1.Rows[cnt].Cells[3].Value = "NO FSI";
                            }
                            else
                            {
                                dataGridView1.Rows[cnt].Cells[3].Value = "0x"+Interpreter.FaultCodeRead[cnt].ToString("X");
                            }
                            dataGridView1.Rows[cnt].Cells[4].Value = SimulationStatusString(Interpreter.ValveSimStateRead[cnt]); 
                        }
                        else
                        {
                            for (int ind = 0; ind < 4; ind++)
                            {
                                dataGridView1.Rows[cnt].Cells[ind].Value = "NA";
                            }
                        }
                        dataGridView1.Rows[cnt].Selected = false;
                    }

                    Interpreter.PeriodicUpdateAvailable = false;
                }
                SetLedColor(pictureLedInControl, Interpreter.GlobalErrorDetail.ControlledByGUI ? "Gray" : "Blue");

                if (Interpreter.ServiceTraps())
                {
                    CommGood = true;
                }
                if (ServicePeriodicStatusOn)
                {// Service untrapped periodic status 
                    Interpreter.ServicePeriodicStatus();
                }

                buttonStartPeriodicMsg.Enabled = true;
                if (Interpreter.VerMsgNew != 0)
                {
                    Interpreter.VerMsgNew = 0;
                    labelHostSW.Text = PrintSwVersion(Interpreter.VersionMsg.SwVersion);
                    buttonViewVer.Enabled = true;
                }
                if (Interpreter.BitMsgNew != 0)
                {
                    Interpreter.BitMsgNew = 0;

                    if (BitInfoInit == false)
                    {
                        InitializeState();
                        BitInfoInit = true;
                    }

                    SetLedColor(pictureLedInControl, Interpreter.GlobalErrorDetail.ControlledByGUI ? "Gray" : "Blue");

                    SetLedColor(pictureBoxOneOrMoreValvesFail, Interpreter.GlobalErrorDetail.OneOrMoreValvesFail ? "Red" : "Green");
                    SetLedColor(pictureBoxControlCircuitFail, Interpreter.GlobalErrorDetail.ControlCircuitFail ? "Red" : "Green");
                    SetLedColor(pictureBoxUnpoweredPowerSection, Interpreter.GlobalErrorDetail.UnpoweredPowerSection ? "Red" : "Green");
                    SetLedColor(pictureBoxDatabaseFail, Interpreter.GlobalErrorDetail.DatabaseFail ? "Red" : "Green");
                    SetLedColor(pictureBoxNoCalibration, Interpreter.GlobalErrorDetail.NoCalibration ? "Red" : "Green");
                    SetLedColor(pictureBoxGrossOverload, Interpreter.GlobalErrorDetail.GrossOverload ? "Red" : "Green");
                    SetLedColor(pictureBoxPowerCompartmentOverTemp, Interpreter.GlobalErrorDetail.PowerCompartmentOverTemp ? "Red" : "Green");
                    SetLedColor(pictureBoxAtLeastOneValveNotInstalled, Interpreter.GlobalErrorDetail.AtLeastOneValveNotInstalled ? "Red" : "Green");
                    SetLedColor(pictureBoxNotReadyOrPreparing, Interpreter.GlobalErrorDetail.ControlledByGUI ? "Red" : "Green");
                    SetLedColor(pictureBoxNoModelMapping, Interpreter.GlobalErrorDetail.NoModelMapping ? "Red" : "Green");
                    SetLedColor(pictureBoxSimulationMode, Interpreter.GlobalErrorDetail.SimulationMode ? "Blue":"Gray");
                    SetLedColor(pictureBoxProgrammingMode, Interpreter.GlobalErrorDetail.ProgrammingMode? "Blue":"Gray");

                    SetLedColor(pictureBoxCfg1, (Interpreter.BitInfo.CfgIn & 1) != 0 ? "Blue" : "Gray");
                    SetLedColor(pictureBoxCfg2, (Interpreter.BitInfo.CfgIn & 2 ) != 0 ? "Blue" : "Gray");
                    SetLedColor(pictureBoxParity, (Interpreter.BitInfo.CfgIn & 4) != 0 ? "Blue" : "Gray");

                    ServicePeriodicStatusOn = !Interpreter.GlobalErrorDetail.NoPeriodicMessage;
                }
                if (Interpreter.DeviceConfigMsgNew != 0)
                {
                    Interpreter.DeviceConfigMsgNew = 0;

                    if (DeviceConfigInfoInit == false)
                    {
                        if ( Interpreter.DeviceExecStatus.SetNumber > 3   )
                        {
                            numericUpDownParamSet.Value = 0; 
                        }
                        else
                        {
                            numericUpDownParamSet.Value = Interpreter.DeviceExecStatus.SetNumber;
                        }
                        DeviceConfigInfoInit = true;
                    }
                }
                    if (Interpreter.PeriodicStatusMsgNew != 0)
                {// Already info updated so just turn on the flag
                    Interpreter.PeriodicStatusMsgNew = 0;
                }
                Interpreter.GetBIT(isbg: false);
                if (ServicePeriodicStatusOn)
                {
                    buttonStartPeriodicMsg.Text = "Stop";
                }
                else
                {
                    buttonStartPeriodicMsg.Text = "Start";
                }

                // Indicate absence of communication 
                NoCommCounter += 1;
                if (CommGood)
                {
                    NoCommCounter = 0;
                }
                if (NoCommCounter > 3)
                {
                    NoCommCounter = 3;
                    SetLedColor(pictureLEDConnect, "Red");
                }
                else
                {
                    SetLedColor(pictureLEDConnect, "Blue");
                }
            } // End of "Have communication" section 
            else
            {
                buttonStartPeriodicMsg.Enabled = false;
                Interpreter.Flush();
                Interpreter.HostCom.Messages.Clear();
                Interpreter.traps.Clear();
                ServicePeriodicStatusOn = false;
                buttonStartPeriodicMsg.Text = "Start";
                SetLedColor(pictureLedInControl, "Gray");
                SetLedColor(pictureBoxOneOrMoreValvesFail, "Gray");
                SetLedColor(pictureBoxControlCircuitFail, "Gray");
                SetLedColor(pictureBoxUnpoweredPowerSection, "Gray");
                SetLedColor(pictureBoxDatabaseFail, "Gray");
                SetLedColor(pictureBoxNoCalibration, "Gray");
                SetLedColor(pictureBoxGrossOverload, "Gray");
                SetLedColor(pictureBoxPowerCompartmentOverTemp, "Gray");
                SetLedColor(pictureBoxAtLeastOneValveNotInstalled, "Gray");
                SetLedColor(pictureBoxNotReadyOrPreparing, "Gray");
                SetLedColor(pictureBoxNoModelMapping, "Gray");
                SetLedColor(pictureBoxSimulationMode, "Gray");
                SetLedColor(pictureBoxProgrammingMode, "Gray");

                SetLedColor(pictureLEDConnect, "Gray");
                SetLedColor(pictureLedInControl, "Gray");

                Interpreter.PeriodicMsgCntr = 0;
                Interpreter.SolicitedMsgCntr = 0;
                NoCommCounter = 0;
            }// End of "No communication" section
            labelSolicitedMsgCntr.Text = Interpreter.SolicitedMsgCntr.ToString();
            labelPeriodicMessage.Text = Interpreter.PeriodicMsgCntr.ToString();

            mutex.ReleaseMutex();
        }


        private void InitializeState()
        {
            for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
            {
                ValveLabels[cnt].ForeColor = Interpreter.GetLabelColor(Interpreter.BitDetail[cnt]);
            }
            int num = Math.Min(Interpreter.BitInfo.IdOut,(byte)15);
            numericUpIdOut.Value = num; 

        }

    }
}