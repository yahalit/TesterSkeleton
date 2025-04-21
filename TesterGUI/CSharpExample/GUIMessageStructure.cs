using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using DocumentFormat.OpenXml.Drawing;
using MathNet.Numerics.Random;
using System.Windows.Forms;
using MathNet.Numerics;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PvsGUI
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CGUI_SetInstallation
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort Valid;
        public CStiffaOf7US Installation; 
        public ushort cs;
        public void Fill(bool[] values, uint cntr, out byte[] buf, bool _Valid = true )
        {
            Valid = (ushort)(_Valid ? 1 : 0 ) ;
            Installation.CopyFromBool(values) ;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGUI_SetInstallation) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetInstallation, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGUI_SetInstallation)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGUI_SetInstallation*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSetSingleModelParamsSet
    {
        public ushort preamble;
        public CMsgHeader Header;
        public CStiffaOf25Floats Parameters;
        public ushort Model;
        public ushort Valid;
        public ushort cs;

        public void Fill(float[] parameters, int model, int valid, uint cntr, out byte[] buf)
        {

            Parameters.CopyFromFloatVec(parameters);
            Model = (ushort)model;
            Valid = (ushort)valid;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetSingleModelParamsSet) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetSingleModelParamsSet, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetSingleModelParamsSet)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetSingleModelParamsSet*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }
    public unsafe struct CSetActiveMapping
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort ActiveMapping; // 0 / 1/ 2/ 3
        public ushort cs;

        public void Fill(int activeMapping, uint cntr, out byte[] buf)
        {

            ActiveMapping = (ushort)activeMapping; 
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetActiveMapping) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetActiveMappingToSerialFlash, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetActiveMapping)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetActiveMapping*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    
    public unsafe struct CGetSingleModelParamsSet
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort Model; //number netween 0 and 7.
        public ushort cs;

        public void Fill(int model, uint cntr, out byte[] buf)
        {
            Model = (ushort)model;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGetSingleModelParamsSet) / 2),
                (ushort)Literals.GUIOpCodes.GUI_GetSingleModelParamsSet, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGetSingleModelParamsSet)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGetSingleModelParamsSet*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }
    public unsafe struct CGetMapping
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort ParSet; //PerSet is the mapping number chosen (0 / 1 / 2 / 3 equvalent to A/ B/ C/ D).
        public ushort cs;

        public void Fill(int parSet, uint cntr, out byte[] buf)
        {
            ParSet = (ushort)parSet;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGetMapping) / 2),
                (ushort)Literals.GUIOpCodes.GUI_GetModel2ValveMapping, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGetMapping)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGetMapping*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }
    public unsafe struct CSetMapping
    {
        public ushort preamble;
        public CMsgHeader Header;
        public CStiffaOf14US Map; //indices 0-11: for normal valves. values can be 0-3. indices 12-13: for special valves. values can be 4-7.
        public ushort Valid; // Nonzero if valid else 0
        public ushort ParSet; //PerSet is the mapping number chosen (0 / 1 / 2 / 3 equvalent to A/ B/ C/ D).
        public ushort cs;

        //GUI_SetModel2ValveMapping = 0xB006

        public void Fill(int [] map, int valid, int parSet, uint cntr, out byte[] buf)
        {

            Map.CopyFromInt(map); //I need to copu from int to CStiffaOf14US
            Valid = (ushort)valid;
            ParSet = (ushort)parSet; 

            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetMapping) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetModel2ValveMapping, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetMapping)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetMapping*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetID
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort TestOrSetup;
        public ushort DacVal1;
        public ushort DacVal2;
        public ushort DacVal3;
        public ushort DacVal4;
        public ushort bUseTestValues;
        public ushort cs;
        public void Fill(bool[] values, uint cntr, out byte[] buf, bool ManualMode = true, bool TestMode = true)
        {
            if (TestMode) TestOrSetup = (ushort)1; //Test
            else TestOrSetup = (ushort)2; //Setup

            bUseTestValues = (ushort)(ManualMode ? 1 : 0);
            DacVal1 = (ushort)(values[0] ? 1 : 0);
            DacVal2 = (ushort)(values[1] ? 1 : 0);
            DacVal3 = (ushort)(values[2] ? 1 : 0);
            DacVal4 = (ushort)(values[3] ? 1 : 0);
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetID) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetSimID, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetID)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetID*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct GUIAnswer_GetUnitData
    {
        public CMsgHeader Header;
        public uint ValveSubverPatch;
        public uint LLCSubverPatch;
        public uint SimSubverPatch;
        public CStiffaOf7US LLCSerialNumbers;
        public ushort IsValidIdentity;
        public ushort SerialNumber;
        public ushort HardwareRevision;
        public uint ProductionDate;
        public uint RevisionDate;
        public uint HardwareType;
        public ushort cs;
        public unsafe void Fill(ushort[] uPtr, out int HwVer , out int HwSubVer , out int _SerialNumber , out DateTime ProdDate)
        {
            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(GUIAnswer_GetUnitData*)ptr;
            }
            HwVer = (HardwareRevision >> 8) & 0xff;
            HwSubVer = HardwareRevision & 0xff;
            _SerialNumber = SerialNumber;
            int year = (((int)ProductionDate >> 16) & 0xff) + 2000;
            int month = ((int)ProductionDate >> 8) & 0xff;
            int day = (int)ProductionDate & 0xff ; 
            if ( year < 2024 || year > 2100 || month < 1 || month > 12 || day < 1 || day > 31 )
            {
                MessageBox.Show("Read EUT identity: \n EUT has bad date data");
                ProdDate = DateTime.Now ; 
            }
            else
            {
                ProdDate = new DateTime(year, month, day);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct GUI_SetUnitProductionData
    {
        public ushort preamble;
        public CMsgHeader Header;
        uint PassWord;
        uint HardwareRevision;
        uint ProductionDate;
        uint RevisionDate;
        uint HardwareType;
        uint SerialNumber;
        uint ProductionBatchCode; 
        public ushort cs;

        public void Fill(int HwVer, int HwSubver , int _SerialNumber , DateTime ProdTime, uint cntr, out byte [] buf )
        {
            PassWord = 0x12345678;
            HardwareRevision = (uint) ( (HwSubver & 0xff) + (HwVer & 0xff) * 256) ;
            ProductionDate   = (uint)(( ProdTime.Year - 2000) * 65536 + ProdTime.Month * 256 + ProdTime.Day ) ;
            RevisionDate = ProductionDate;
            SerialNumber = (uint)_SerialNumber;
            HardwareType = 0xbbbb;
            ProductionBatchCode = 0; 
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(GUI_SetUnitProductionData) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetUnitProductionData, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(GUI_SetUnitProductionData)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(GUI_SetUnitProductionData*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetAtpConversion
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort ManualMode;
        public CStiffaOf14US ConvertCmds;
        public CStiffaOf14US VoltageCmds;
        public ushort Spare; 
        public ushort cs;
        public void Fill(int[] values, bool [] ConvertOn , uint cntr, out byte[] buf, bool _ManualMode = true)
        {
            ManualMode = (ushort)(_ManualMode ? 1 : 0 ) ;
            ConvertCmds.CopyFromBool(ConvertOn);
            VoltageCmds.CopyFromInt(values); 
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetAtpConversion) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetAtpConversion, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetAtpConversion)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetAtpConversion*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetSingleParam
    {
        public ushort preamble;
        public CMsgHeader Header;
        public float Par0;
        public float Par1;
        public float Par2;
        public float Par3;
        public float Par4;
        public float Par5;
        public float Par6;
        public float Par7;
        public float Par8;
        public float Par9;
        public float Par10;
        public float Par11;
        public float Par12;
        public float Par13;
        public float Par14;
        public float Par15;
        public float Par16;
        public float Par17;
        public float Par18;
        public float Par19;
        public float Par20;
        public float Par21;
        public float Par22;
        public float Par23;
        public float Par24;
        public ushort ParamSet;
        public ushort Valid;
        public ushort cs;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOf4
    {
        public  float Par0;
        public float Par1;
        public float Par2;
        public float Par3;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOf14
    {
        public float Par0;
        public float Par1;
        public float Par2;
        public float Par3;
        public float Par4;
        public float Par5;
        public float Par6;
        public float Par7;
        public float Par8;
        public float Par9;
        public float Par10;
        public float Par11;
        public float Par12;
        public float Par13;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOfInt8
    {
        public int Par0;
        public int Par1;
        public int Par2;
        public int Par3;
        public int Par4;
        public int Par5;
        public int Par6;
        public int Par7;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOfUInt7
    {
        public int Par0;
        public int Par1;
        public int Par2;
        public int Par3;
        public int Par4;
        public int Par5;
        public int Par6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOf8UInt
    {
        public int Par0;
        public int Par1;
        public int Par2;
        public int Par3;
        public int Par4;
        public int Par5;
        public int Par6;
        public int par7; 

        public void CopyFromBuf( byte [] buf , int offset )
        {
            fixed (byte* ptr = &buf[offset])
            {
                this = *((CStiffaOf8UInt*)ptr);
            }
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOf4US
    {
        public ushort Par0;
        public ushort Par1;
        public ushort Par2;
        public ushort Par3;
        public void AddTo(List<double>[] L, double fac = 1)
        {
            L[0].Add((short)Par0 * fac);
            L[1].Add((short)Par1 * fac);
            L[2].Add((short)Par2 * fac);
            L[3].Add((short)Par3 * fac);
        }
        public void CopyToDouble(double[] L, double fac = 1)
        {
            L[0] = ((short)Par0 * fac);
            L[1] = ((short)Par1 * fac);
            L[2] = ((short)Par2 * fac);
            L[3] = ((short)Par3 * fac);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CStiffaOf7US
    {
        public ushort Par0;
        public ushort Par1;
        public ushort Par2;
        public ushort Par3;
        public ushort Par4;
        public ushort Par5;
        public ushort Par6;
        public void CopyToBool(bool[] L)
        {
            L[0] = (Par0 != 0 ) ;
            L[1] = (Par1 != 0);
            L[2] = (Par2 != 0);
            L[3] = (Par3 != 0);
            L[4] = (Par4 != 0);
            L[5] = (Par5 != 0);
            L[6] = (Par6 != 0);
        }

        public void CopyFromBool( bool[] L )
        {
            Par0 = (ushort)(L[0] ? 1 : 0);
            Par1 = (ushort)(L[1] ? 1 : 0);
            Par2 = (ushort)(L[2] ? 1 : 0);
            Par3 = (ushort)(L[3] ? 1 : 0);
            Par4 = (ushort)(L[4] ? 1 : 0);
            Par5 = (ushort)(L[5] ? 1 : 0);
            Par6 = (ushort)(L[6] ? 1 : 0);
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CStiffaOf25Floats
    {
        public float Par0;
        public float Par1;
        public float Par2;
        public float Par3;
        public float Par4;
        public float Par5;
        public float Par6;
        public float Par7;
        public float Par8;
        public float Par9;
        public float Par10;
        public float Par11;
        public float Par12;
        public float Par13;
        public float Par14;
        public float Par15;
        public float Par16;
        public float Par17;
        public float Par18;
        public float Par19;
        public float Par20;
        public float Par21;
        public float Par22;
        public float Par23;
        public float Par24;

        public void CopyFromFloatVec(float[] vec)
        {
            Par0 = vec[0];
            Par1 = vec[1];
            Par2 = vec[2];
            Par3 = vec[3];
            Par4 = vec[4];
            Par5 = vec[5];
            Par6 = vec[6];
            Par7 = vec[7];
            Par8 = vec[8];
            Par9 = vec[9];
            Par10 = vec[10];
            Par11 = vec[11];
            Par12 = vec[12];
            Par13 = vec[13];
            Par14 = vec[14];
            Par15 = vec[15];
            Par16 = vec[16];
            Par17 = vec[17];
            Par18 = vec[18];
            Par19 = vec[19];
            Par20 = vec[20];
            Par21 = vec[21];
            Par22 = vec[22];
            Par23 = vec[23];
            Par24 = vec[24];
        }
        public void CopyToVec(float[] vec)
        {
            vec[0] = Par0;
            vec[1] = Par1;
            vec[2] = Par2;
            vec[3] = Par3;
            vec[4] = Par4;
            vec[5] = Par5;
            vec[6] = Par6;
            vec[7] = Par7;
            vec[8] = Par8;
            vec[9] = Par9;
            vec[10] = Par10;
            vec[11] = Par11;
            vec[12] = Par12;
            vec[13] = Par13;
            vec[14] = Par14;
            vec[15] = Par15;
            vec[16] = Par16;
            vec[17] = Par17;
            vec[18] = Par18;
            vec[19] = Par19;
            vec[20] = Par20;
            vec[21] = Par21;
            vec[22] = Par22;
            vec[23] = Par23;
            vec[24] = Par24;
        }
    }
    public unsafe struct CStiffaOf14US
    {
        public ushort Par0;
        public ushort Par1;
        public ushort Par2;
        public ushort Par3;
        public ushort Par4;
        public ushort Par5;
        public ushort Par6;
        public ushort Par7;
        public ushort Par8;
        public ushort Par9;
        public ushort Par10;
        public ushort Par11;
        public ushort Par12;
        public ushort Par13;
        public void AddTo(List<double>[] L, double fac = 1)
        {
            L[0].Add((short)Par0 * fac);
            L[1].Add((short)Par1 * fac);
            L[2].Add((short)Par2 * fac);
            L[3].Add((short)Par3 * fac);
            L[4].Add((short)Par4 * fac);
            L[5].Add((short)Par5 * fac);
            L[6].Add((short)Par6 * fac);
            L[7].Add((short)Par7 * fac);
            L[8].Add((short)Par8 * fac);
            L[9].Add((short)Par9 * fac);
            L[10].Add((short)Par10 * fac);
            L[11].Add((short)Par11 * fac);
            L[12].Add((short)Par12 * fac);
            L[13].Add((short)Par13 * fac);

            if ( L[0].Count > Literals.MaxListLength)
            {
                for ( int cnt = 0; cnt < Literals.N_VALVES; cnt++ )
                {
                    L[cnt].RemoveAt(0); 
                }
            }
        }
        public void CopyToDouble(double[] L, double fac = 1)
        {
            L[0]=((short)Par0 * fac);
            L[1]=((short)Par1 * fac);
            L[2]=((short)Par2 * fac);
            L[3]=((short)Par3 * fac);
            L[4]=((short)Par4 * fac);
            L[5]=((short)Par5 * fac);
            L[6]=((short)Par6 * fac);
            L[7]=((short)Par7 * fac);
            L[8]=((short)Par8 * fac);
            L[9]=((short)Par9 * fac);
            L[10]=((short)Par10 * fac);
            L[11]=((short)Par11 * fac);
            L[12]=((short)Par12 * fac);
            L[13]=((short)Par13 * fac);

        }

        public void CopyFromBool( bool[] vec )
        {
            int[] ivec = new int[vec.Length];
            for (int i = 0; i < ivec.Length; i++) ivec[i] = vec[i] ? 1 : 0;
            CopyFromInt(ivec); 
        }

        public void CopyFromInt(int[] vec)
        {
            Par0 = (ushort)vec[0];
            Par1 = (ushort)vec[1];
            Par2 = (ushort)vec[2];
            Par3 = (ushort)vec[3];
            Par4 = (ushort)vec[4];
            Par5 = (ushort)vec[5];
            Par6 = (ushort)vec[6];
            Par7 = (ushort)vec[7];
            Par8 = (ushort)vec[8];
            Par9 = (ushort)vec[9];
            Par10 = (ushort)vec[10];
            Par11 = (ushort)vec[11];
            Par12 = (ushort)vec[12];
            Par13 = (ushort)vec[13];
        }

        public void CopyToUint(uint[] L)
        {
            L[0] = (Par0 );
            L[1] = (Par1);
            L[2] = (Par2 );
            L[3] = (Par3 );
            L[4] = (Par4 );
            L[5] = (Par5);
            L[6] = (Par6 );
            L[7] = (Par7 );
            L[8] = (Par8 );
            L[9] = (Par9 );
            L[10] = (Par10);
            L[11] = (Par11 );
            L[12] = (Par12 );
            L[13] = (Par13 );
        }

        public void AddTo(List<int>[] L )
        {
            L[0].Add(Par0 );
            L[1].Add(Par1 );
            L[2].Add(Par2 );
            L[3].Add(Par3 );
            L[4].Add(Par4 );
            L[5].Add(Par5 );
            L[6].Add(Par6 );
            L[7].Add(Par7 );
            L[8].Add(Par8 );
            L[9].Add(Par9 );
            L[10].Add(Par10 );
            L[11].Add(Par11 );
            L[12].Add(Par12 );
            L[13].Add(Par13 );
            if (L[0].Count > Literals.MaxListLength)
            {
                for (int cnt = 0; cnt < Literals.N_VALVES; cnt++)
                {
                    L[cnt].RemoveAt(0);
                }
            }
        }

    } // End of CStiffaOf14US

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CGetSetCalibParam
    {
        public ushort preamble; 
        public CMsgHeader Header;
        public uint Password0x12345678; 
        public CStiffaOf4 AdcPressureOffset;
        public CStiffaOf4 AdcPressureGain;
        public CStiffaOf4 DACPressureOffsetJ1;
        public CStiffaOf4 DACPressureOffsetJ7;
        public CStiffaOf4 DACPressureGainJ1;
        public CStiffaOf4 DACPressureGainJ7;
        public CStiffaOf14 DACSolenoidCurrentOffset;
        public CStiffaOf14 DACSolenoidCurrentGain;
        public float Tp1Gain;
        public float Tp1Offset;
        public float Tp2Gain;
        public float Tp2Offset;
        public CStiffaOf4 SpareFloat;
        public CStiffaOfInt8 SpareInt;
        public uint CalibCs;
        public ushort BurnCmdReadZero ; // On Set command, 1 means "Burn to flash". On Get, this will be zero 
        public ushort Valid;
        public ushort cs;

        public unsafe void Fill(ushort[] uPtr )
        {
            ushort [] MuPtr = new ushort [uPtr.Length+1]; ;
            MuPtr[0] = 0xa5a5;
            Array.Copy(uPtr, 0, MuPtr, 1, uPtr.Length);

            fixed (ushort* ptr = &MuPtr[0]) 
            {
                this = *(CGetSetCalibParam*)ptr;
            }
        }

        public void FillAdcCalib(double [] gain , double [] offset , bool burn , uint cntr, out byte[] buf)
        {
            Password0x12345678 = 0x12345678;
            Tp1Offset = (float) offset[0];
            Tp2Offset = (float)offset[1];

            Tp1Gain = (float)gain[0];
            Tp2Gain = (float)gain[1];
            BurnCmdReadZero = (ushort) (burn ? 1 : 0);
            Valid = (ushort)1; 
            Copy2Buf(out buf, cntr);
        }

        public void FillTPCalib(double[] gain, double[] offset, bool burn, uint cntr, out byte[] buf)
        {
            Password0x12345678 = 0x12345678;
            AdcPressureOffset.Par0 = (float)offset[0];
            AdcPressureOffset.Par1 = (float)offset[1];

            AdcPressureGain.Par0 = (float)gain[0];
            AdcPressureGain.Par1 = (float)gain[1];

            BurnCmdReadZero = (ushort)(burn ? 1 : 0);
            Valid = (ushort)1;
            Copy2Buf(out buf, cntr);
        }

        public unsafe void CalcCalibCs()
        {
            fixed (uint* ptr = &Password0x12345678)
            {
                uint* ptrStart = ptr ;
                fixed ( uint* ptrEnd = &CalibCs)
                {
                    uint sum = 0;
                    while (ptrStart < ptrEnd)
                    {
                        sum += *ptrStart++;
                    }
                    *ptrStart = ~sum + 1; 
                }
            }
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGetSetCalibParam) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetCalibration, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGetSetCalibParam)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGetSetCalibParam*)ptr = this;
            }
            CalcCalibCs(); 
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }


    }




    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DataRequest
    {
        public ushort Preamble;
        public CMsgHeader Header;
        public ushort cs;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]


    public unsafe struct CSetPressure
    {
        public ushort preamble;
        public CMsgHeader Header;
        public CStiffaOf4US TestValuesJ1;
        public CStiffaOf4US TestValuesJ7;
        public ushort bUseTestValues;
        public ushort cs;

        public void Fill(double PressureVolts, int index ,  uint cntr, out byte[] buf, bool UseTestValues = true)
        {
            ushort PressureBits = (ushort) ( Math.Min(Math.Max(PressureVolts,0), 9.99 ) * 6553.6 ) ;
            ushort uzero = (ushort)0;
            TestValuesJ1.Par0 = (index == 0) ? PressureBits : uzero;
            TestValuesJ1.Par1 = (index == 1) ? PressureBits : uzero;
            TestValuesJ1.Par2 = (index == 2) ? PressureBits : uzero;
            TestValuesJ1.Par3 = (index == 3) ? PressureBits : uzero;
            TestValuesJ7.Par0 = (index == 0) ? PressureBits : uzero;
            TestValuesJ7.Par1 = (index == 1) ? PressureBits : uzero;
            TestValuesJ7.Par2 = (index == 2) ? PressureBits : uzero;
            TestValuesJ7.Par3 = (index == 3) ? PressureBits : uzero;
            bUseTestValues = (ushort)(UseTestValues ?1:0);
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetPressure) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetPressureTestValue, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetPressure)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetPressure*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSetLedValues
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort bUseTestValues;
        public CStiffaOf14US Group1;
        public CStiffaOf14US Group2;
        public CStiffaOf14US Group3;
        public CStiffaOf14US Group4;
        public CStiffaOf14US Group5;
        public ushort cs;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSetFanSSRValues
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort bUserSSRValue;
        public ushort TestSSRValue;
        public ushort bUserFanValue;
        public ushort TestFanValue0;
        public ushort TestFanValue1;
        public ushort cs;

		public void Fill(bool SetSSR, bool SSRValue, bool SetFan, bool Fan1Value, bool Fan2Value, uint cntr , out byte[] buf )
		{
			preamble = 0xa5a5;
			Header.Fill((ushort)(sizeof(CSetFanSSRValues) / 2), (ushort)Literals.GUIOpCodes.GUI_SetSSRAndFanTestValue, cntr, CompensateHeader: false);

			Header.opcode = (ushort)Literals.GUIOpCodes.GUI_SetSSRAndFanTestValue;
			bUserSSRValue = SetSSR ? (ushort)1 : (ushort)0;
			TestSSRValue = SSRValue ? (ushort)1 : (ushort)0;
			bUserFanValue = SetFan ? (ushort)1 : (ushort)0;
			TestFanValue0 = Fan1Value ? (ushort)1 : (ushort)0;
			TestFanValue1 = Fan2Value ? (ushort)1 : (ushort)0;
            Copy2Buf(out buf, cntr); 
		}


		public void Copy2Buf(out byte[] buf, uint cntr)
		{
			Header.Time = 0;
			Header.MessageNum = cntr;
			byte[] mbuf = new byte[Header.len];
			fixed (byte* ptr = mbuf)
			{
				*(CSetFanSSRValues*)ptr = this;
				ushort* uPtr = (ushort*)ptr;
				ushort cs = 0; // Thats ~0xa5a5 
				for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
				{
					cs = (ushort)(cs - *uPtr++);
				}
				*uPtr = cs;
			}
			buf = mbuf;
		}



	}


	[StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSetValveCurrentTestValue
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort bUserValveCurrents;
        public ushort DacManualMode; // 0: Do not apply calibration 1: apply 
        public CStiffaOf14US TestValues;
        public ushort cs;
        public void Fill(double OutVolts, int index, uint cntr, out byte[] buf , bool ManualMode = true , bool ApplyCalib = true )
        {
            int [] values = new int[Literals.N_VALVES];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 0;
            }
            values[index] = (int) Math.Max(Math.Min(OutVolts * 65536.0 / 5.0 , 65535), 0);
            TestValues.CopyFromInt(values);
            bUserValveCurrents = (ushort) (ManualMode ? 1 : 0) ;
            DacManualMode = (ushort)(ApplyCalib ? 1 : 0) ;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetValveCurrentTestValue) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetValveCurrentTestValue, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetValveCurrentTestValue)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetValveCurrentTestValue*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSetDiscreteTestValues
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort bUserDiscreteValveIndications;
        public CStiffaOf14US UserDiscreteValveIndicationValues;
        public ushort cs;
        public void Fill( bool[] _values, uint cntr, out byte[] buf, bool DiscreteManualMode = true)
        {
            int[] values = new int[Literals.N_VALVES];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = _values[i] ? 1 : 0;
            }
            UserDiscreteValveIndicationValues.CopyFromInt(values);
            bUserDiscreteValveIndications = (ushort)(DiscreteManualMode ? 1 : 0);
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetDiscreteTestValues) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetValveDiscreteTestValue,cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetDiscreteTestValues)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetDiscreteTestValues*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CProtectedCmd
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort codeL;
        public ushort codeH;
        public ushort cs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CBroadcastSdo
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort Index;
        public ushort SubIndex;
        public int   Payload;
        public ushort TxMode;
        public ushort MaxRetryCnt;
        public ushort TimeOutMsec;
        public ushort SlaveBroadcastId;
        public ushort WaitResponse; 
        public ushort cs;
        public void Fill(int IndexIn, int SubIndexIn , int PayloadIn , bool TxModeIn = true , int MaxRetryCntIn = 5 , int TimeoutIn = 20 , bool bSim = true, bool bWaitResponse = true )
        {
            preamble = 0xa5a5;
            Header.opcode = (ushort)Literals.GUIOpCodes.GUI_BroadcastSdo;
            Header.Time = 0;
            Header.MessageNum = 0;
            Header.len = (ushort)sizeof(CBroadcastSdo);
            Index =(ushort)  IndexIn;
            SubIndex = (ushort)SubIndexIn;
            MaxRetryCnt = (ushort)MaxRetryCntIn;
            TimeOutMsec = (ushort)TimeoutIn;
            if (TxModeIn )
            {
                TxMode = 1;
                Payload = PayloadIn;
            }
            else
            {
                TxMode = 0;
                Payload = 0;
            }
            if ( bSim )
            {
                SlaveBroadcastId = (ushort) Literals.E_CanId.CAN_ID_SIM_SDO_BROADCAST ;
            }
            else
            {
                SlaveBroadcastId = (ushort)Literals.E_CanId.CAN_ID_LLC_SDO_BROADCAST;
            }
            WaitResponse = bWaitResponse ? (ushort)1 : (ushort)0; 
            cs = 0;
        }
        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            Header.Time = 0;
            Header.MessageNum = cntr;
            byte[] mbuf = new byte[Header.len];
            fixed (byte* ptr = mbuf)
            {
                *(CBroadcastSdo*)ptr = this;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CGetAtpSignals
    {
        public CMsgHeader Header;
        public uint FSIInterruptCtr; // 0..1
        public CStiffaOfUInt7 FsiComCnt_A;
        public CStiffaOfUInt7 FsiComCnt_B;
        public CStiffaOfUInt7 FsiAxCnt; // .. 43
        public ushort Adc800R_1; //44
        public ushort Adc800R_2;
        public ushort Adc800R_3;
        public ushort Adc800R_4;
        public ushort Adc800R_1Raw; // 48 
        public ushort Adc800R_2Raw;
        public ushort Adc800R_3Raw;
        public ushort Adc800R_4Raw;
        public ushort InputVolTP1Caps;
        public ushort InputVolTP2Entry;
        public ushort junk54;
        public ushort junk55;
        public ushort junk56;
        public ushort junk57;
        public ushort junk58;
        public ushort junk59;
        public ushort junk60;
        public ushort junk61;
        public ushort junk62;
        public ushort junk63;
        public ushort cs;
        public void GetFsiCounters( out int[] fcnt)
        {
            fcnt = new int[] { FsiComCnt_A.Par0 , FsiComCnt_A.Par1 , FsiComCnt_A.Par2 , FsiComCnt_A.Par3 , FsiComCnt_A.Par4 , FsiComCnt_A.Par5 , FsiComCnt_A.Par6 ,
            FsiComCnt_B.Par0 , FsiComCnt_B.Par1 , FsiComCnt_B.Par2 , FsiComCnt_B.Par3 , FsiComCnt_B.Par4 , FsiComCnt_B.Par5 , FsiComCnt_B.Par6 }; 
        }

        public void GetAdcVoltage( out double[] volt)
        {
            volt = new double[4]; 
            volt[0] = Adc800R_1 / 512.0 ;
            volt[1] = Adc800R_2 / 512.0;
            volt[2] = Adc800R_3 / 512.0;
            volt[3] = Adc800R_4 / 512.0;
        }

        public void GetTpVolts(out double[] volt)
        {
            volt = new double[2];
            volt[0] = InputVolTP1Caps / 100.0 ;
            volt[1] = InputVolTP2Entry / 100.0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CGetSdoBroadcast
    {
        public CMsgHeader Header;
        public ushort SdoState;
        public int ResponseSim1;
        public int ResponseSim2;
        public int ResponseSim3;
        public int ResponseSim4;
        public int ResponseSim5;
        public int ResponseSim6;
        public int ResponseSim7;
        public int ResponseSim8;
        public int ResponseSim9;
        public int ResponseSim10;
        public int ResponseSim11;
        public int ResponseSim12;
        public int ResponseSim13;
        public int ResponseSim14;
        public int ResponseLLC1;
        public int ResponseLLC2;
        public int ResponseLLC3;
        public int ResponseLLC4;
        public int ResponseLLC5;
        public int ResponseLLC6;
        public int ResponseLLC7;
        public ushort cs;

        public bool GetInt( out int[] value)
        {
            value = new int[21]; 
            if (SdoState != 3)
            {
                return false; 
            }
            value[0] = ResponseSim1;
            value[1] = ResponseSim2;
            value[2] = ResponseSim3;
            value[3] = ResponseSim4;
            value[4] = ResponseSim5;
            value[5] = ResponseSim6;
            value[6] = ResponseSim7;
            value[7] = ResponseSim8;
            value[8] = ResponseSim8;
            value[9] = ResponseSim10;
            value[10] = ResponseSim11;
            value[11] = ResponseSim12;
            value[12] = ResponseSim13;
            value[13] = ResponseSim14;
            value[14] = ResponseLLC1;
            value[15] = ResponseLLC2;
            value[16] = ResponseLLC3;
            value[17] = ResponseLLC4;
            value[18] = ResponseLLC5;
            value[19] = ResponseLLC6;
            value[20] = ResponseLLC7;
            return true;
        }
        public bool GetFloat(out float[] value)
        {
            value = new float[21];
            float fTemp; 
            if (  !GetInt(out int[] ival) )
            {
                return false; 
            }
            for ( int cnt = 0; cnt < 21; cnt++)
            {
                byte[] bytes = BitConverter.GetBytes(ival[cnt]);
                fTemp = BitConverter.ToSingle(bytes, 0);
                if (float.IsNaN(fTemp) || float.IsInfinity(fTemp))
                {
                    return false;
                }
                value[cnt] -= fTemp; 
            }
            return true; 
        }
    }

    public struct CExtraBit
    {
        public bool InAtpMode; 
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSingleParametersSet
    {
        public CMsgHeader Header;
        public CStiffaOf25Floats parameters;
        public ushort ParSet;
        public ushort valid;
        public ushort cs;

        public unsafe void Fill(ushort[] uPtr) 
        {

            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(CSingleParametersSet*)ptr;
            }
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CValveMapping
    {
        public CMsgHeader Header;
        public CStiffaOf14US map;
        public ushort ParSet;
        public ushort valid;
        public ushort cs;

        public unsafe void Fill(ushort[] uPtr) 
        {

            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(CValveMapping*)ptr;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CActiveMapping
    {
        public CMsgHeader Header;
        public ushort ActiveMapping;
        public ushort cs;

        public unsafe void Fill(ushort[] uPtr, out int mapping) //SimulatorId.Fill(buf, out IDvalues);
        {

            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(CActiveMapping*)ptr;
            }

            mapping = (int)ActiveMapping;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSimulatorId
    {
        public CMsgHeader Header;
        public ushort ID1;
        public ushort ID2;
        public ushort ID3;
        public ushort ID4;
        public ushort ID1Test;
        public ushort ID2Test;
        public ushort ID3Test;
        public ushort ID4Test;
        public ushort IdTestValuesUsed;
        public ushort CfgId_us;
        public ushort cs;

        //public void GetAdcVoltage(out double[] volt)
        /*
        public void GetIDValues(out bool[] values)
        {
            values = new bool[4];
            values[0] = ID1 == 1 ? true: false;
            values[1] = ID2 == 1 ? true : false;
            values[2] = ID3 == 1 ? true : false;
            values[3] = ID4 == 1 ? true : false;
        }
        */
        public unsafe void Fill(ushort[] uPtr, out bool [] values) //SimulatorId.Fill(buf, out IDvalues);
        {

            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(CSimulatorId*)ptr;
            }

            //fill ID values
            values = new bool[4];
            values[0] = ID1 == 1 ;
            values[1] = ID2 == 1 ;
            values[2] = ID3 == 1 ;
            values[3] = ID4 == 1 ;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CSimulatorState
    {
        public CMsgHeader Header;
        public ushort BitStat ;
        public ushort Azero;
        public ushort CFG1_Volts;
        public ushort CFG2_Volts;
        public ushort Parity_Volts;
        public ushort LedIntensity0 ;
        public ushort LedIntensity1;
        public ushort LedIntensity2;
        public ushort LedIntensity3;
        public ushort LedIntensity4;
        public ushort Temperatures;
        public ushort TP1_Volts;
        public ushort TP2_Volts;
        public uint BIT;
        public CStiffaOf4US OutPressureVoltsJ1;
        public CStiffaOf4US OutPressureVoltsJ7;
        public ushort V5_Volts;
        public ushort V12_Volts;
        public CStiffaOf4US InAdc800R;
        public ushort BBit2;
        public ushort cs;

        public unsafe void Fill(ushort [] uPtr, out CSystemBit _BIT)
        {
            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(CSimulatorState*)ptr;
            }
            _BIT = new CSystemBit(); 
            _BIT.Fill(BIT );
        }

        public CSystemBit GetCBit( )
        {
            CSystemBit b = new CSystemBit();
            b.Fill(BIT);
            return b; 
        }

        public CExtraBit GetBit2( ) 
        {
            CExtraBit b = new CExtraBit()
            {
                InAtpMode = ((BBit2 & 1) != 0)
            };
            return b; 
        } 

        public void GetTemperatures(out double LoadTemperature , out double ElectronicsTemperature)
        {
            LoadTemperature  = (double)(((int)Temperatures & 0xff ) -60 ) ;
            ElectronicsTemperature = (double)((((int)Temperatures>>8) & 0xff) - 60);
        }
        public void GetBitVoltages(out double Bit5V, out double Bit12V)
        {
            Bit5V = V5_Volts / 6.553600000000000e+03;
            Bit12V = V12_Volts / 3.276800000000000e+03; 
        }

        // Read the configuration voltages as digital . 
        // value: false or true, digital 
        // ok   : false if voltage does not fall to < VIL or > VIH
        // vref : Voltage level of the digital input
        public void GetCfg(out bool [] value, out bool[] ok , double vref )
        {
            value = new bool[3];
            ok = new bool[3];

            double [] v = new double [] { (CFG1_Volts * 0.001) ,(CFG2_Volts * 0.001) , (Parity_Volts * 0.001) } ;
            
            for ( int cnt = 0; cnt < 3; cnt++ )
            {
                if (VecOps.IsDigitalHigh(v[cnt],vref))
                {
                    value[cnt] = true;
                    ok[cnt] = true;
                }
                else
                {
                    if ( VecOps.IsDigitalLow(v[cnt],vref))
                    {
                        ok[cnt] = true;
                    }
                } 
            }
        }

    }


    public struct CAxisCond
    {
        public bool Installed;
        public bool IsBoot ;
        public bool Responding;
        public bool Calibrated;
        public bool VersionMatch; 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CVersionMsg
    {
        public CMsgHeader Header;
        public CHWversion MainCpuVersion;
        public CHWversion SimHwVer0;
        public CHWversion SimHwVer1;
        public CHWversion SimHwVer2;
        public CHWversion SimHwVer3;
        public CHWversion SimHwVer4;
        public CHWversion SimHwVer5;
        public CHWversion SimHwVer6;
        public CHWversion SimHwVer7;
        public CHWversion SimHwVer8;
        public CHWversion SimHwVer9;
        public CHWversion SimHwVer10;
        public CHWversion SimHwVer11;
        public CHWversion SimHwVer12;
        public CHWversion SimHwVer13;
        public CSWversion SwVersion;
        public ulong SlaveStat;
        public ushort cs;
        public CAxisCond GetAxisCond(int ind)
        {
            CAxisCond str = new CAxisCond() ;
            int ax = (int) ( (SlaveStat >> (ind * 3)) & 7);
            if (ax > 0)
            {
                str.Installed = true; 
                if ( ax > 1)
                {
                    str.Responding = true;
                    if (ax == 2)
                    {
                        str.IsBoot = true;
                    }
                    else
                    {
                        ax -=  3;
                        if ( (ax & 1) != 0 )
                        {
                            str.Calibrated = true;
                        }
                        if ((ax & 2) != 0)
                        {
                            str.VersionMatch = true;
                        }
                    }
                }
            } 
            return str; 
        }
    };


    //GUI_SetGUIPeriodicMessage
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetGUIPeriodicMessage
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort PeriodMsec ;
        public ushort PassKey    ;
        public ushort cs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CGUIPeriodicMessage
    {
        public CMsgHeader Header;
        public unsafe CStiffaOf14US InCurrent;
        public unsafe CStiffaOf14US InVoltage;
        public unsafe CStiffaOf14US Position;
        public unsafe CStiffaOf14US OutVolts;
        public unsafe CStiffaOf14US SlaveStatus;
        public unsafe CStiffaOf4US Pressure;
        public unsafe uint Bit;
        public unsafe uint TimeRemain;
        public unsafe ushort OpenedStatus; //bitfield of length 16. lower bit is open status of valve # 0. two upper bits have no meaning. 
        public unsafe ushort ClosedStatus;
        public unsafe byte LoadTemperature;
        public unsafe byte ElectTemperature;
        public unsafe ushort Spare4;
        public unsafe ushort Spare3;
        public unsafe ushort Spare2;
        public unsafe ushort Spare1;
        public unsafe ushort Spare0;
        public ushort cs;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CDownload2Ram
    {
        public ushort preamble;
        public CMsgHeader Header;
        uint RamAddress;
        public unsafe CStiffaOf8UInt Block0;
        public unsafe CStiffaOf8UInt Block1;
        public unsafe CStiffaOf8UInt Block2;
        public unsafe CStiffaOf8UInt Block3;
        public unsafe CStiffaOf8UInt Block4;
        public unsafe CStiffaOf8UInt Block5;
        public unsafe CStiffaOf8UInt Block6;
        public unsafe CStiffaOf8UInt Block7;
        public ushort cs;

        public unsafe void Fill(uint _ramAddress, byte[] buf, int offset)
        {
            preamble = 0xa5a5;
            RamAddress = _ramAddress;
            Block0.CopyFromBuf(buf, offset);
            Block1.CopyFromBuf(buf, offset + 32);
            Block2.CopyFromBuf(buf, offset + 64);
            Block3.CopyFromBuf(buf, offset + 96);
            Block4.CopyFromBuf(buf, offset + 128);
            Block5.CopyFromBuf(buf, offset + 160);
            Block6.CopyFromBuf(buf, offset + 192);
            Block7.CopyFromBuf(buf, offset + 224);
            // Checksum
            ushort csSum = 0;
            fixed (ushort* usPtr = &preamble)
            {
                for (int cnt = 0; cnt < (sizeof(CDownload2Ram) / 2 - 1); cnt++)
                {
                    csSum -= usPtr[cnt];
                }
            }
            cs = csSum;
        }

        public unsafe void Copy2Buf(out byte[] buf, uint cntr)
        {
            Header.Fill((ushort)(sizeof(CDownload2Ram) / 2), (ushort)Literals.GUIOpCodes.GUILoadDataIntoRAM, cntr, CompensateHeader:false);
            byte[] mbuf = new byte[sizeof(CDownload2Ram)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CDownload2Ram*)ptr = this;
            }
            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CKillAndReboot
    {
        public ushort preamble;
        public CMsgHeader Header;
        public uint pass;
        public ushort cs;
    }
    
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CPrepDownLoad
    {
        public ushort preamble;
        public CMsgHeader Header;
        public uint FirstAddress;
        public uint LastAddress;
        public uint SwCheckSum;
        public ushort cs;
        public void Fill( uint _FirstAddress , uint _LastAddress, uint _SwCheckSum , int OpCodeIn = (int) Literals.GUIOpCodes.GUI_BootPrepFwLoad  )
        {
            preamble = 0xa5a5;
            Header.opcode = (ushort)OpCodeIn;
            Header.Time = 0;
            Header.MessageNum = 0; 
            Header.len = (ushort) sizeof(CPrepDownLoad); 
            FirstAddress = _FirstAddress;
            LastAddress = _LastAddress;
            SwCheckSum = _SwCheckSum;
            cs = 0; 
        }
        public void Copy2Buf(out byte[] buf  , uint cntr)
        {
            Header.Time = 0;
            Header.MessageNum = cntr ;
            byte[] mbuf = new byte[Header.len];
            fixed (byte* ptr = mbuf)
            {
                *(CPrepDownLoad*)ptr = this ;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CPrepDownLoadDsp
    {
        public ushort preamble;
        public CMsgHeader Header;
        public uint FirstAddress;
        public uint LastAddress;
        public uint SwCheckSum;
        public ushort cs;
        public void Fill(uint _FirstAddress, uint _LastAddress, uint _SwCheckSum, 
            int OpCodeIn = (int)Literals.GUIOpCodes.GUI_BootPrepFwLoadDsp )
        {
            preamble = 0xa5a5;
            Header.opcode = (ushort)OpCodeIn;
            Header.Time = 0;
            Header.MessageNum = 0;
            Header.len = (ushort)sizeof(CPrepDownLoadDsp);
            FirstAddress = _FirstAddress;
            LastAddress = _LastAddress;
            SwCheckSum = _SwCheckSum;
            cs = 0;
        }
        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            Header.Time = 0;
            Header.MessageNum = cntr;
            byte[] mbuf = new byte[Header.len];
            fixed (byte* ptr = mbuf)
            {
                *(CPrepDownLoadDsp*)ptr = this;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetFwBurn
    {
        public ushort preamble;
        public CMsgHeader Header;
        public uint FirstAddress;
        public uint LastAddress;
        public uint SwCheckSum;
        public ushort cs;
        public void Fill(uint _FirstAddress, uint _LastAddress, uint _SwCheckSum)
        {
            preamble = 0xa5a5;
            Header.opcode = (ushort)Literals.GUIOpCodes.GUI_SetFwBurn;
            Header.Time = 0;
            Header.MessageNum = 0;
            Header.len = (ushort)sizeof(CSetFwBurn);
            FirstAddress = _FirstAddress;
            LastAddress = _LastAddress;
            SwCheckSum = _SwCheckSum;
            cs = 0;
        }
        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            Header.Time = 0;
            Header.MessageNum = cntr;
            byte[] mbuf = new byte[Header.len];
            fixed (byte* ptr = mbuf)
            {
                *(CSetFwBurn*)ptr = this;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetFwBurnDsp
    {
        public ushort preamble;
        public CMsgHeader Header;
        public uint FirstAddress;
        public uint LastAddress;
        public uint SwCheckSum;
        public uint DownloadMask;
        public ushort bIsSim;
        public ushort TimeOutMsec; 
        public ushort cs;
        public void Fill(uint _FirstAddress, uint _LastAddress, uint _SwCheckSum, 
            uint _DownloadMask , bool _bIsSim , int _TimeOutMsec)
        {
            preamble = 0xa5a5;
            Header.opcode = (ushort)Literals.GUIOpCodes.GUI_SetFwBurnDsp ;
            Header.Time = 0;
            Header.MessageNum = 0;
            Header.len = (ushort)sizeof(CSetFwBurnDsp);
            FirstAddress = _FirstAddress;
            LastAddress = _LastAddress;
            SwCheckSum = _SwCheckSum;
            DownloadMask = _DownloadMask;
            bIsSim = (ushort)(_bIsSim ? 1 : 0);
            TimeOutMsec = (ushort)_TimeOutMsec; 
            cs = 0;
        }
        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            Header.Time = 0;
            Header.MessageNum = cntr;
            byte[] mbuf = new byte[Header.len];
            fixed (byte* ptr = mbuf)
            {
                *(CSetFwBurnDsp*)ptr = this;
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
            buf = mbuf;
        }
    }


    public struct CSystemBit
    {
        public bool AtLeastOneValveFailed; 
        public bool ReservedForOwnCpuFail;
        public bool PeripheralCardFail;
        public bool DataBaseFailure;
        public bool NoCalib;
        public bool GrossOverload;
        public bool bLoadTemperatureException;
        public bool SwLoadFailure;
        public bool bNotFullyInstalled;
        public bool InDataBaseProgramming;
        public bool ElectroincsTemperatureException;
        public bool SlaveCommunicationTimeOut;
        public bool VoltagesReady;
        public bool HostMissing;
        public bool LLCCommunicationTimeOut;
        public bool SSROn;
        public bool FanCoolingOnState1;
        public bool FanCoolingOnState2;
        public bool GUIInCharge;
        public bool AxesProgrammed;
        public bool AxesMapped;
        public bool Operational;
        public bool GUITxPeriodic;
        public bool CfgId1; //can be canceled in the future if need more bits for this message, because this information is also sent in the simulator perioic message.
        public bool CfgId2; //can be canceled in the future if need more bits for this message, because this information is also sent in the simulator perioic message.
        public bool CfgParity; //can be canceled in the future if need more bits for this message, because this information is also sent in the simulator perioic message.
        public int ActiveMapping; //two bits.
        public bool RecorderReady;
        public bool RecorderWaitTrigger;

        //public bool ActiveMapping_bit1; //active mapping is sent as an int unsigned bits 26..27
        //public bool ActiveMapping_bit2; // rest are reserved.


        public void Fill(uint b)
        {
            AtLeastOneValveFailed = ((b & 1) != 0)    ;
            ReservedForOwnCpuFail = (((b>>1) & 1) != 0)  ;
            PeripheralCardFail = (((b >> 2) & 1) != 0) ;
            DataBaseFailure = (((b >> 3) & 1) != 0) ;
            NoCalib = (((b >> 4) & 1) != 0)  ;
            GrossOverload = (((b >> 5) & 1) != 0)  ;
            bLoadTemperatureException = (((b >> 6) & 1) != 0) ;
            SwLoadFailure = (((b >> 7) & 1) != 0)  ;
            bNotFullyInstalled = (((b >> 8) & 1) != 0) ;
            InDataBaseProgramming = (((b >> 9) & 1) != 0) ;
            ElectroincsTemperatureException = (((b >> 10) & 1) != 0) ;
            SlaveCommunicationTimeOut = (((b >> 11) & 1) != 0)  ;
            VoltagesReady = (((b >> 12) & 1) != 0)  ;
            HostMissing = (((b >> 13) & 1) != 0)  ;
            LLCCommunicationTimeOut = (((b >> 14) & 1) != 0) ;
            SSROn = (((b >> 15) & 1) != 0)  ;
            FanCoolingOnState1 = (((b >> 16) & 1) != 0)  ;
            FanCoolingOnState2 = (((b >> 17) & 1) != 0)  ;
            GUIInCharge = (((b >> 18) & 1) != 0) ;
            AxesProgrammed = (((b >> 19) & 1) != 0)  ;
            AxesMapped = (((b >> 20) & 1) != 0)  ;
            Operational = (((b >> 21) & 1) != 0)  ;
            GUITxPeriodic = (((b >> 22) & 1) != 0);
            CfgId1 = (((b >> 23) & 1) != 0);
            CfgId2 = (((b >> 24) & 1) != 0);
            CfgParity = (((b >> 25) & 1) != 0);
            ActiveMapping = (int)((b >> 26) & 3);
            RecorderReady = (((b >> 28) & 1) != 0 ) ;
            RecorderWaitTrigger = (((b >> 29) & 1)!=0);
        }
    }



    public struct CSlaveBit
    {
        public bool ConverterOn;
        public bool ConverterReady;
        public bool ConverterFault;
        public bool OverTemperature;
        public bool CurrentLimit;
        public bool NoCalibration;
        public int ConverterMode;
        public int SystemMode;
        public bool CommunicationTimeout;
        public int SimulatingFault;
        public bool installed;
        public bool FsiComTimeOut; 


        public void Fill(uint b)
        {
            if (b == 0xffff)
            {
                this.Fill(0);
                installed = false;
                return; 
            }
            if ( b == 0x1c00)
            {
                this.Fill(0);
                FsiComTimeOut = true; 
                return; 
            }

            FsiComTimeOut = false; 
            ConverterOn = ((b & 1) != 0)   ;
            ConverterReady = (((b >> 1) & 1) != 0);
            ConverterFault = (((b >> 2) & 1) != 0)  ;
            OverTemperature = (((b >> 3) & 1) != 0) ;
            CurrentLimit = (((b >> 4) & 1) != 0) ;
            NoCalibration = (((b >> 5) & 1) != 0) ;
            ConverterMode = (int)((b >> 6) & 7);
            SystemMode = (int)((b >> 9) & 7);
            CommunicationTimeout = (((b >> 12) & 1) != 0) ;
            SimulatingFault = (int)((b >> 13) & 7);
            installed = true;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CGUI_SetDebugPeriodicReport
    {
        public ushort preamble;
        public CMsgHeader Header;
        public float AmplitudeCurrent;
        public float AmplitudePos;
        public float AmplitudePressure;
        public float AmplitudeVolts;
        public float Frequency;
        public ushort On;
        public ushort Spare;
        public ushort cs;

        public void Fill(bool _On, double _AmplitudeCurrent, double _AmplitudePos, double _AmplitudePressure, double _AmplitudeVolts,
            double _Frequency, uint cntr, out byte[] buf)
        {
            AmplitudeCurrent = (float)_AmplitudeCurrent;
            AmplitudePos = (float)_AmplitudePos;
            AmplitudePressure = (float)_AmplitudePressure;
            AmplitudeVolts = (float)_AmplitudeVolts;
            Frequency = (float)_Frequency;
            On =  _On ? (ushort)1 : (ushort)0 ;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGUI_SetDebugPeriodicReport) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetDebugPeriodicReport, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGUI_SetDebugPeriodicReport)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGUI_SetDebugPeriodicReport*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }



    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CGUI_SetRecorderParameters
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort nVars2Record;
        public ushort RecorderGap;
        public ushort TriggerType;
        public ushort TimeBasis;
        public uint   RecLength ;
        public uint PreTriggerCnt ;
        public float Threshold;
        public ushort SigIndex0;
        public ushort SigIndex1;
        public ushort SigIndex2;
        public ushort SigIndex3;
        public ushort SigIndex4;
        public ushort SigIndex5;
        public ushort SigIndex6;
        public ushort SigIndex7;
        public ushort SigIndex8;
        public ushort SigIndex9;
        public ushort SigIndex10;
        public ushort SigIndex11;
        public ushort SigIndex12;
        public ushort SigIndex13;
        public ushort SigIndex14;
        public ushort SigIndex15;
        public ushort cs;
        public void Fill( out byte[] buf , uint cntr,  int _nVars2Record = 0 , int[] _SigIndex = null , int _RecLength = 1000 , 
            int _RecorderGap = 1 , int _TimeBasis = 0 , int _TriggerType = 0 , int _PreTriggerCnt = 0 , double _Threshold = 0  ) 
        {
            nVars2Record = (ushort) _nVars2Record;
            RecorderGap = (ushort)_RecorderGap;
            TriggerType = (ushort)_TriggerType;
            TimeBasis = (ushort)_TimeBasis;
            RecLength = (uint)_RecLength;
            PreTriggerCnt = (uint)_PreTriggerCnt;
            Threshold = (float)_Threshold;
            
            if (_SigIndex != null )
            {
                int[] sigarr = new int[Literals.N_MAX_SIGS];
                Array.Copy(_SigIndex, sigarr, Math.Min(Literals.N_MAX_SIGS,_SigIndex.Length));
                SigIndex0 = (ushort)sigarr[0];
                SigIndex1 = (ushort)sigarr[1];
                SigIndex2 = (ushort)sigarr[2];
                SigIndex3 = (ushort)sigarr[3];
                SigIndex4 = (ushort)sigarr[4];
                SigIndex5 = (ushort)sigarr[5];
                SigIndex6 = (ushort)sigarr[6];
                SigIndex7 = (ushort)sigarr[7];
                SigIndex8 = (ushort)sigarr[8];
                SigIndex9 = (ushort)sigarr[9];
                SigIndex10 = (ushort)sigarr[10];
                SigIndex11 = (ushort)sigarr[11];
                SigIndex12 = (ushort)sigarr[12];
                SigIndex13 = (ushort)sigarr[13];
                SigIndex12 = (ushort)sigarr[14];
                SigIndex13 = (ushort)sigarr[15];
            }

            Copy2Buf(out  buf, cntr ); 
        }
        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGUI_SetRecorderParameters) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetRecorderParameters, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGUI_SetRecorderParameters)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGUI_SetRecorderParameters*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CGUI_GetRecordedSignal
    {
        public ushort preamble;
        public CMsgHeader Header;
        public ushort SigIndex;
        public uint StartIndex;
        public uint StopIndex;
        public ushort cs;
        
        public void Fill( int _SigIndex, int _StartIndex, int _StopIndex, uint cntr, out byte[] buf)
        {            
            SigIndex = (ushort)_SigIndex;
            StartIndex = (uint)_StartIndex;
            StopIndex = (uint)_StopIndex;
            Copy2Buf(out  buf, cntr ); 
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CGUI_GetRecordedSignal) / 2), 
                (ushort)Literals.GUIOpCodes.GUI_GetRecordedSignal, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CGUI_GetRecordedSignal)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CGUI_GetRecordedSignal*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct GUIAnswer_RequestValveStatusReport
    {
        public ushort preamble;
        public CMsgHeader Header;
        CStiffaOf14US RawTemperature;
        CStiffaOf14US SlaveStatus ;
        CStiffaOf14US Spare;
        public ushort cs;
        public unsafe void Fill(ushort[] uPtr, out List<CSlaveBit> SlaveBit , out double[] SlaveTemperature  )
        {
            fixed (ushort* ptr = &uPtr[0])
            {
                this = *(GUIAnswer_RequestValveStatusReport*)ptr;
            }
            SlaveBit = new List<CSlaveBit>();
            SlaveTemperature = new double[Literals.N_VALVES];
            uint[] stat = new uint[Literals.N_VALVES];
            SlaveStatus.CopyToUint(stat);
            uint[] tmp = new uint[Literals.N_VALVES];
            RawTemperature.CopyToUint(tmp);
            for ( int cnt = 0; cnt < Literals.N_VALVES; cnt++ )
            {
                CSlaveBit sl = new CSlaveBit();
                sl.Fill(stat[cnt]); 
                SlaveBit.Add( sl );
                SlaveTemperature[cnt] = tmp[cnt] * 0.1; 
            }
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CAnswer_SetRecorderParameters
    {
        public CMsgHeader Header;
        public ushort gap; //0
        public uint RecLength;
        public ushort nSignals ;//3
        public ushort TriggerType;
        public uint   PreTriggerCnt;
        public ushort TimeBasis; //7
        public ushort MaxSignals2Record;
        public uint   BufLength;
        public ushort SlowRecorderMultiplier; //11
        public float FastSamplingTime;//12
        public float TriggerLevel ;//14
        public ushort RecorderStatus; //16
        public ushort LengthOfSigList ;//17 
        public ushort SigIndex1; //18
        public ushort SigFlags1;
        public ushort SigIndex2;
        public ushort SigFlags2;
        public ushort SigIndex3;
        public ushort SigFlags3;
        public ushort SigIndex4;
        public ushort SigFlags4;
        public ushort SigIndex5;
        public ushort SigFlags5;
        public ushort SigIndex6;
        public ushort SigFlags6;
        public ushort SigIndex7;
        public ushort SigFlags7;
        public ushort SigIndex8;
        public ushort SigFlags8;
        public ushort SigIndex9;
        public ushort SigFlags9;
        public ushort SigIndex10;
        public ushort SigFlags10;
        public ushort SigIndex11;
        public ushort SigFlags11;
        public ushort SigIndex12;
        public ushort SigFlags12;
        public ushort SigIndex13;
        public ushort SigFlags13;
        public ushort SigIndex14;
        public ushort SigFlags14;
        public ushort SigIndex15;
        public ushort SigFlags15;
        public ushort SigIndex16;
        public ushort SigFlags16;

        public ushort cs;

        public bool GetSigAttributes( out int [] SigIndex, out int [] flags)
        {
            if (nSignals == 0 || nSignals > 16)
            {
                SigIndex = new int[0] ;
                flags = new int[0];
                return false; 
            }
            int[] _SigIndex = new int[] { SigIndex1 , SigIndex2 , SigIndex3 , SigIndex4, SigIndex5 , SigIndex6 , SigIndex7 , SigIndex8,
            SigIndex9 , SigIndex10 , SigIndex11, SigIndex12 , SigIndex13 , SigIndex14 , SigIndex15 , SigIndex16 }; 
            int [] _flags = new int[] { SigFlags1, SigFlags2, SigFlags3, SigFlags4, SigFlags5, SigFlags6, SigFlags7, SigFlags8,
            SigFlags9, SigFlags10 , SigFlags11 , SigFlags12 , SigFlags13 , SigFlags14, SigFlags15, SigFlags16} ;

            for ( int cnt = 0; cnt < nSignals; cnt++ )
            {
                if (_SigIndex[cnt] <  0 || _SigIndex[cnt] >= MaxSignals2Record)
                {
                    SigIndex = new int[0];
                    flags = new int[0];
                    return false;
                }
            }
            SigIndex = new int[nSignals];
            flags = new int[nSignals];
            Array.Copy(_SigIndex, SigIndex, nSignals);
            Array.Copy(_flags, flags, nSignals);
            return true; 
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CSetFwTestSerialFlash
    {
        public ushort preamble;
        public CMsgHeader Header;
        public int RandomSeed; 
        public ushort cs;
        public void Fill(int _RandomSeed, uint cntr, out byte[] buf)
        {
            RandomSeed = _RandomSeed;
            Copy2Buf(out buf, cntr);
        }

        public void Copy2Buf(out byte[] buf, uint cntr)
        {
            preamble = 0xa5a5;
            Header.Fill((ushort)(sizeof(CSetFwTestSerialFlash) / 2),
                (ushort)Literals.GUIOpCodes.GUI_SetFwTestSerialFlash, cntr, CompensateHeader: false);
            byte[] mbuf = new byte[sizeof(CSetFwTestSerialFlash)];
            fixed (byte* ptr = &mbuf[0])
            {
                *(CSetFwTestSerialFlash*)ptr = this;
            }
            MsgService.CalcCs(mbuf);

            buf = mbuf;
        }

    }

    public struct CSerFlashState
    {
        public bool CalibValid;
        public bool ModelsValid; 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public unsafe struct CAnswer_AxisVersion
    {
        public CMsgHeader Header;
        public uint AxisSwVer1;
        public uint AxisSwVer2;
        public uint AxisSwVer3;
        public uint AxisSwVer4;
        public uint AxisSwVer5;
        public uint AxisSwVer6;
        public uint AxisSwVer7;
        public uint AxisSwVer8;
        public uint AxisSwVer9;
        public uint AxisSwVer10;
        public uint AxisSwVer11;
        public uint AxisSwVer12;
        public uint AxisSwVer13;
        public uint AxisSwVer14;
        public uint LLCSwVer1;
        public uint LLCSwVer2;
        public uint LLCSwVer3;
        public uint LLCSwVer4;
        public uint LLCSwVer5;
        public uint LLCSwVer6;
        public uint LLCSwVer7;
        public uint MainSwExpected;
        public uint LLCSwExpected;
        public uint ValveSwExpected;
        public ushort CardSerial1;
        public ushort CardSerial2;
        public ushort CardSerial3;
        public ushort CardSerial4;
        public ushort CardSerial5;
        public ushort CardSerial6;
        public ushort CardSerial7;
        public ushort SerFlashState; 
        public ushort cs;

        public void GetAxisVer(out uint[] AxisSwVer , out uint[] LLCSwVer , out int[] CardSn , out CSerFlashState ser)
        {
            AxisSwVer = new uint[] { AxisSwVer1 , AxisSwVer2 , AxisSwVer3 , AxisSwVer4,AxisSwVer5 , AxisSwVer6 , AxisSwVer7 , AxisSwVer8,
            AxisSwVer9 ,AxisSwVer10 , AxisSwVer11, AxisSwVer12 , AxisSwVer13 , AxisSwVer14 };
            LLCSwVer = new uint[] { LLCSwVer1 , LLCSwVer2 , LLCSwVer3 , LLCSwVer4 , LLCSwVer5 , LLCSwVer6 , LLCSwVer7};
            CardSn = new int[] { CardSerial1  , CardSerial2 , CardSerial3 , CardSerial4 , CardSerial5 , CardSerial6 , CardSerial7};

            ser = new CSerFlashState
            {
                CalibValid = ((SerFlashState & 1) != 0),
                ModelsValid = ((SerFlashState & 2) != 0)
            };
        }
    }


    public unsafe class MsgService
    {
        static public void CalcCs(byte[] mbuf)
        {
            fixed (byte* ptr = mbuf)
            {
                ushort* uPtr = (ushort*)ptr;
                ushort cs = 0; // Thats ~0xa5a5 
                for (int cnt = 0; cnt < (mbuf.Length / 2 - 1); cnt++)
                {
                    cs = (ushort)(cs - *uPtr++);
                }
                *uPtr = cs;
            }
        }
    }



} // End of namespace




