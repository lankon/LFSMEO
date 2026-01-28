using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_OTO
{
    public class OTO : ISpectrometer
    {
        #region parameter define
        SpectrumData SD_Live;
        enum ERROR_CODE
        {
            STATUS_OK,
            ERROR_NONE_FRAME_SIZE,
            ERROR_GET_WAVELENGTH_FAIL,
        }
        public struct SpectrumData
        {
            public IntPtr DeviceHandle;
            public IntPtr ColorIntPtr;
            public ushort framesize;
            public uint Avg;
            public int Boxcar;

            public uint integration_time;
            public string SerialNumber;
            public float[] Lumda;
            public float[] Intensity_raw;
            public float[] Intensity;

            public float[] Reference_Intensity;
            public float[] Dark_Intensity;

            //Process
            public float[] E_BG_LevelBuffer;
            public float[] O_BG_LevelBuffer;

            //Color
            public ColorInfo Colorinfo;
            public int observer;
            public int illuminant;
        }
        public struct ColorInfo
        {
            public UInt32 Observer;         //0=2 degree , 1=10 degree
            public UInt32 Illuminant;       // 0=A, 1=B, 2=C, ........19=F12
            public UInt32 ColorType;        //0=relative emission, 1=relative reflection, 2=absolute emission
            public double Ie;               //Summa of intensity;
            public double X;
            public double Y;
            public double Z;

            public double x;
            public double y;
            public double z;

            public double CRI_Ra;
            public double CRI_R0;
            public double CRI_R1;
            public double CRI_R2;
            public double CRI_R3;
            public double CRI_R4;
            public double CRI_R5;
            public double CRI_R6;
            public double CRI_R7;
            public double CRI_R8;
            public double CRI_R9;
            public double CRI_R10;
            public double CRI_R11;
            public double CRI_R12;
            public double CRI_R13;
            public double CRI_R14;
            public double CRI_DC;

            public double CCT;
            public double UVW_U;
            public double UVW_V;
            public double UVW_W;

            public double uvw_u;
            public double uvw_v;
            public double uvw_w;

            public double Luv_L;
            public double Luv_u;
            public double Luv_v;

            public double uv_hue_angle;
            public double uv_saturation;
            public double DominantWavelength;
            public double Purity;
            public double CIE_whiteness;
            public double CIE_Tint;

            public double Hunter_L;
            public double Hunter_a;
            public double Hunter_b;

            public double CIE_L;
            public double CIE_a;
            public double CIE_b;

            public double CIELAB_hue_angle;
            public double CIELAP_chroma;
            public double CIE1960u;
            public double CIE1960v;
            public double CIE1960w;
            public double CIE1960U;
            public double CIE1960V;
            public double CIE1960W;

            public double CIE1976u;
            public double CIE1976v;
            public double CIE1976w;
            public double CIE1976U;
            public double CIE1976V;
            public double CIE1976W;

            public double Ref_x;
            public double Ref_y;
            public double Ref_z;
        }
        #endregion

        #region private function
        private void AllocateBuffer(ref SpectrumData SD, int Framesize)
        {
            //init parameter
            SD.Avg = 1;
            SD.Boxcar = 0;
            SD.integration_time = 6;
            SD.observer = 0;
            SD.illuminant = 5;
            SD.SerialNumber = "";
            SD.Intensity_raw = new float[Framesize];
            SD.Intensity = new float[Framesize];
            SD.Lumda = new float[Framesize];
            SD.Dark_Intensity = new float[Framesize];
            SD.E_BG_LevelBuffer = new float[Framesize];
            SD.Reference_Intensity = new float[Framesize];
            SD.O_BG_LevelBuffer = new float[Framesize];
        }
        #endregion

        public int Open()
        {
            try
            {
                uint status;
                uint device_num = 0;
                uint i = 0;
                //Get compatiable VID and PID
                UInt32 buffersize = 0;
                UInt32[] VIDPID = new UInt32[2];
                unsafe
                {
                    Link_UAI.Link_UAI.UAI_SpectrometerGetDeviceList(ref buffersize, null);
                    VIDPID = new UInt32[buffersize * 2]; //VIDPID buffersize should be 2 X VIDPID counts.
                    fixed (UInt32* temp_VIDPID = VIDPID)
                    {
                        Link_UAI.Link_UAI.UAI_SpectrometerGetDeviceList(ref buffersize, temp_VIDPID);
                    }
                }

                for (int j = 0; j < buffersize * 2; j = j + 2)
                {
                    device_num = 0;
                    status = Link_UAI.Link_UAI.UAI_SpectrometerGetDeviceAmount(VIDPID[j], VIDPID[j + 1], ref device_num);

                    for (i = 0; i < device_num; i++)
                    {
                        status = Link_UAI.Link_UAI.UAI_SpectrometerOpen(i, ref SD_Live.DeviceHandle, VIDPID[j], VIDPID[j + 1]);
                        
                        if (status == 0)
                        {
                            //Open device successfully then get frame size
                            Link_UAI.Link_UAI.UAI_SpectromoduleGetFrameSize(SD_Live.DeviceHandle, ref SD_Live.framesize);
                            if (SD_Live.framesize == 0)
                                return (int)ERROR_CODE.ERROR_NONE_FRAME_SIZE;

                            //allocate wavelength and raw data buffer
                            AllocateBuffer(ref SD_Live, SD_Live.framesize);

                            //Get serial
                            byte[] temp_SN = new byte[16];
                            Link_UAI.Link_UAI.UAI_SpectrometerGetSerialNumber(SD_Live.DeviceHandle, temp_SN);
                            SD_Live.SerialNumber = System.Text.Encoding.Default.GetString(temp_SN).Replace(@"\s", "");
                            SD_Live.SerialNumber = SD_Live.SerialNumber.Substring(0, Link_UAI.Link_UAI.LastIndexOfnumber(temp_SN));

                            //UAI_FirmwareGetBuildNumber
                            byte[] temp_FWVersion = new byte[8];
                            Link_UAI.Link_UAI.UAI_FirmwareGetVersion(SD_Live.DeviceHandle, temp_FWVersion);
                            string FWVersion = Convert.ToChar(temp_FWVersion[0]).ToString() + temp_FWVersion[1].ToString().PadLeft(3, '0') + "." + temp_FWVersion[3].ToString();

                            //UAI_SpectrometerGetActivatingDate
                            byte[] temp_Date = new byte[16];
                            Link_UAI.Link_UAI.UAI_SpectrometerGetActivatingDate(SD_Live.DeviceHandle, temp_Date);
                            string ActivatingDate = System.Text.Encoding.Default.GetString(temp_Date).Replace(@"\s", "");

                            //UAI_SpectrometerGetManufacturingDate
                            byte[] temp_ManufacturingDate = new byte[16];
                            Link_UAI.Link_UAI.UAI_SpectrometerGetActivatingDate(SD_Live.DeviceHandle, temp_ManufacturingDate);
                            string ManufacturingDate = System.Text.Encoding.Default.GetString(temp_ManufacturingDate).Replace(@"\s", "");

                            //UAI_SpectrometerGetRomSerialNumber
                            byte[] temp_ROMsn = new byte[16];
                            Link_UAI.Link_UAI.UAI_SpectrometerGetRomSerialNumber(SD_Live.DeviceHandle, temp_ROMsn);
                            string ROMsn = System.Text.Encoding.Default.GetString(temp_ROMsn).Replace(@"\s", "");

                            //UAI_SpectromoduleGetMaximumIntegrationTime
                            UInt16 MaxIntegrationTime = 0;
                            Link_UAI.Link_UAI.UAI_SpectromoduleGetMaximumIntegrationTime(SD_Live.DeviceHandle, ref MaxIntegrationTime);

                            //UAI_SpectromoduleGetMinimumIntegrationTime
                            UInt16 MinIntegrationTime = 0;
                            Link_UAI.Link_UAI.UAI_SpectromoduleGetMinimumIntegrationTime(SD_Live.DeviceHandle, ref MinIntegrationTime);

                            //Get wavelength table
                            status = Link_UAI.Link_UAI.UAI_SpectrometerWavelengthAcquire(SD_Live.DeviceHandle, SD_Live.Lumda);
                            
                            if (status != (int)ERROR_CODE.STATUS_OK)
                                return (int)ERROR_CODE.ERROR_GET_WAVELENGTH_FAIL;

                            return (int)ERROR_CODE.STATUS_OK;
                        }
                    }
                }

                if (device_num == 0)
                    return -1;
                else
                    return (int)ERROR_CODE.STATUS_OK;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
