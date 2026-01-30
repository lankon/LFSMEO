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
            STATUS_OK = 0,
            ERROR_GET_DEVICE_FAIL = -1,
            ERROR_OPEN_DEVICE_FAIL = -2,
            ERROR_NONE_FRAME_SIZE = -3,
            ERROR_GET_WAVELENGTH_FAIL = -4,
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
        private UInt32[] GetAvailableVidPidList()
        {
            uint bufferSize = 0;
            UInt32[] vidPid = new UInt32[bufferSize * 2];

            unsafe
            {
                Link_UAI.Link_UAI.UAI_SpectrometerGetDeviceList(ref bufferSize, null);
                if (bufferSize == 0) return null;

                fixed (UInt32* p = vidPid)
                {
                    Link_UAI.Link_UAI.UAI_SpectrometerGetDeviceList(ref bufferSize, p);
                }
            }
            return vidPid;
        }
        private int InitializeDeviceSettings()
        {
            // 取得 FrameSize 並分配記憶體
            Link_UAI.Link_UAI.UAI_SpectromoduleGetFrameSize(SD_Live.DeviceHandle, ref SD_Live.framesize);
            
            if (SD_Live.framesize == 0) 
                return (int)ERROR_CODE.ERROR_NONE_FRAME_SIZE;

            AllocateBuffer(ref SD_Live, SD_Live.framesize);

            // 統一讀取設備資訊 (SN, FW, Date...)
            SD_Live.SerialNumber = GetSDKString(Link_UAI.Link_UAI.UAI_SpectrometerGetSerialNumber, 16);

            // 取得波長表
            var status = Link_UAI.Link_UAI.UAI_SpectrometerWavelengthAcquire(SD_Live.DeviceHandle, SD_Live.Lumda);
            return (status == (int)ERROR_CODE.STATUS_OK) ? (int)ERROR_CODE.STATUS_OK : (int)ERROR_CODE.ERROR_GET_WAVELENGTH_FAIL;
        }
        private string GetSDKString(Func<IntPtr, byte[], uint> sdkMethod, int length)
        {
            byte[] buffer = new byte[length];
            sdkMethod(SD_Live.DeviceHandle, buffer);
            // 移除空白字元與 Null 結束符
            return System.Text.Encoding.Default.GetString(buffer).Trim('\0', ' ', '\r', '\n');
        }
        #endregion
        public ESpectrometerType GetSpectrometerType()
        {
            return ESpectrometerType.OTO;
        }
        public int Open()
        {
            try
            {
                // 1. 取得設備 VID/PID 列表
                var vidPidList = GetAvailableVidPidList();
                
                if (vidPidList == null) 
                    return (int)ERROR_CODE.ERROR_OPEN_DEVICE_FAIL;

                // 2. 搜尋並嘗試開啟設備
                for (int j = 0; j < vidPidList.Length; j += 2)
                {
                    uint vid = vidPidList[j];
                    uint pid = vidPidList[j + 1];
                    uint deviceCount = 0;

                    if (Link_UAI.Link_UAI.UAI_SpectrometerGetDeviceAmount(vid, pid, ref deviceCount) != 0 || deviceCount == 0)
                        continue;

                    for (uint i = 0; i < deviceCount; i++)
                    {
                        if (Link_UAI.Link_UAI.UAI_SpectrometerOpen(i, ref SD_Live.DeviceHandle, vid, pid) == 0)
                        {
                            // 3. 成功開啟後進行參數初始化
                            return InitializeDeviceSettings();
                        }
                    }
                }

                return (int)ERROR_CODE.ERROR_OPEN_DEVICE_FAIL;
            }
            catch
            { 
                return (int)ERROR_CODE.ERROR_OPEN_DEVICE_FAIL;
            }
        }
        public float[] GetSpectrumOneShot(uint integral_time, uint avg_time = 1)
        {
            //每次呼叫皆進行一次清除後再擷取
            SD_Live.integration_time = integral_time * 1000;    //integral_time(ms) integtration_time(us)
            SD_Live.Avg = avg_time;

            Link_UAI.Link_UAI.UAI_SpectrometerDataOneshot(SD_Live.DeviceHandle, SD_Live.integration_time, SD_Live.Intensity, SD_Live.Avg);

            return SD_Live.Intensity;
        }
        public float[] GetSpectrum(uint integral_time, uint avg_time = 1)
        {
            //每次呼叫立即從分光卡中取得當前頻譜
            SD_Live.integration_time = integral_time * 1000;    //integral_time(ms) integtration_time(us)
            SD_Live.Avg = avg_time;

            Link_UAI.Link_UAI.UAI_SpectrometerDataAcquire(SD_Live.DeviceHandle, SD_Live.integration_time, SD_Live.Intensity, SD_Live.Avg);

            return SD_Live.Intensity;
        }

    }
}
