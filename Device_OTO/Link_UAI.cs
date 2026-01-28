using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//typedef enum
//{
//    //Internal Definition
//    API_INT_START = 0,
//    API_SUCCESS = API_INT_START,
//    API_INT_BUFFER_INVALID,
//    API_INT_FEATURE_UNSUPPORTED,
//    API_INT_PROTOCOL_ERROR,
//    API_INT_CALIBRATION_ERROR,
//    API_INT_MEMORY_ERROR,
//    API_INT_ARGUMENT_ERROR,
//    API_INT_HANDLE_INVALID,
//    API_INT_TIMEOUT,
//    //API_INT_DDI_BUFFER_ERROR,
//    API_INT_DATA_NOT_READY,
//    API_INT_DATA_TIME_OUT,
//    API_INT_FILE_IO_ERROR, //20161012 Kevin
//    API_INT_END,
	
//    //Vendor Reservation Code
//    API_EXT_START = 0x80000000
//} ERRORCODE;

namespace Link_UAI
{
    class Link_UAI
    {
        //3.2.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerOpen", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerOpen(uint dev, ref IntPtr handle, UInt32 VID, UInt32 PID);
        //3.2.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerClose", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerClose(IntPtr handle);

        //3.3.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_FirmwareGetVersion", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_FirmwareGetVersion(IntPtr handle, byte[] Version);
        //3.3.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetDeviceAmount", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerGetDeviceAmount(UInt32 VID, UInt32 PID, ref uint NumDevices);
        //3.3.3
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetDeviceList", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetDeviceList(ref UInt32 BufferSize, UInt32* VIDPID);
        //3.3.4
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetModelName", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetModelName(IntPtr handle, byte[] model);
        //3.3.5
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetSerialNumber", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetSerialNumber(IntPtr handle, byte[] SN);
        //3.3.6
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleGetFrameSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectromoduleGetFrameSize(IntPtr handle, ref ushort frame_size);
        //3.3.7
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleGetMaximumIntegrationTime", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleGetMaximumIntegrationTime(IntPtr handle, ref UInt16 time);
        //3.3.8
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleGetMinimumIntegrationTime", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleGetMinimumIntegrationTime(IntPtr handle, ref UInt16 time);
        //3.3.9
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleGetWavelengthCalibrationCoefficients", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleGetWavelengthCalibrationCoefficients(IntPtr handle, double[] coefficients);
        //
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleSetWavelengthCalibrationCoefficients", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleSetWavelengthCalibrationCoefficients(IntPtr handle, double[] coefficients);
        //3.3.10
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleGetWavelengthStart", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleGetWavelengthStart(IntPtr handle, ref float lambda); //kevin
        //3.3.11
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectromoduleGetWavelengthEnd", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleGetWavelengthEnd(IntPtr handle, ref float lambda); //kevin

        //3.4.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerWavelengthAcquire", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerWavelengthAcquire(IntPtr handle, float[] buffer);
        //3.4.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetIntegrationTime", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetIntegrationTime(IntPtr handle, ref UInt32 integration_time_us);
        //3.4.3
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetIntegrationTime", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetIntegrationTime(IntPtr handle, UInt32 integration_time_us);
        //3.4.4
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerDataAcquire", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerDataAcquire(IntPtr handle, uint integration_time_us, float[] buffer, uint average);
        //3.4.5
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerDataOneshot", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerDataOneshot(IntPtr handle, uint integration_time_us, float[] buffer, uint average);

        //DSD test
        //3.5.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetTriggerIO", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetTriggerIO(IntPtr handle, UInt32 enable, UInt32 level);
        //3.5.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetTriggerIO", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetTriggerIO(IntPtr handle, ref UInt32 enable, ref UInt32 level);
        //3.5.3
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerTriggerDataAcquire", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerTriggerDataAcquire(IntPtr handle, float[] buffer);
        //3.5.4
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetTriggerGroupIntegrationTime", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetTriggerGroupIntegrationTime(IntPtr handle, ref UInt32 groupcount, UInt32[] integration_time_us); //kevin	
        //3.5.5
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetTriggerGroupIntegrationTime", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetTriggerGroupIntegrationTime(IntPtr handle, UInt32 groupcount, UInt32[] integration_time_us); //kevin
        //3.5.6
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerCheckTriggerDone", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerCheckTriggerDone(IntPtr handle, ref UInt32 count);
        //3.5.7
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetTriggerData", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetTriggerData(IntPtr handle, UInt32 framesize, UInt32 index, float[] buffer);
        //3.5.8
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerCheckDoneAndGetTriggerData", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerCheckDoneAndGetTriggerData(IntPtr handle, UInt32 framesize, UInt32 index, float[] buffer);
        //3.5.9
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetTriggerDelay", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetTriggerDelay(IntPtr handle, ref UInt32 integration_time_us);
        //3.5.10
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetTriggerDelay", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetTriggerDelay(IntPtr handle, UInt32 integration_time_us);
        //3.5.11
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetBatchMode", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetBatchMode(IntPtr handle, UInt32 count, UInt32 mode);

        //For Calibration
        //3.6.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_BackgroundRemove", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_BackgroundRemove(IntPtr handle, uint integration_time_us, float[] intensity);
        //3.6.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_BackgroundRemoveWithAVG", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_BackgroundRemoveWithAVG(IntPtr handle, uint integration_time_us, float[] intensity);
        //3.6.3
        [DllImport("UserApplication.dll", EntryPoint = "UAI_LinearityCorrection", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_LinearityCorrection(IntPtr handle, uint framesize, float[] intensity);
        //3.6.4
        [DllImport("UserApplication.dll", EntryPoint = "UAI_AbsoluteIntensityCorrection", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_AbsoluteIntensityCorrection(IntPtr handle, float[] intensity, uint integration_time_us);
        //3.6.5
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ContrastIntensityCorrection", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_ContrastIntensityCorrection(IntPtr handle, float[] intensity);
        //3.6.6
        [DllImport("UserApplication.dll", EntryPoint = "UAI_StrayLightCorrection", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_StrayLightCorrection(IntPtr handle, float[] intensity);
        //3.6.7
        [DllImport("UserApplication.dll", EntryPoint = "UINT UAI_DoIntensityCalibration", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_DoIntensityCalibration(IntPtr handle, UInt32 std_size, float[] std_lambda, float[] std_intensity, float[] m_intensity, UInt32 integration_time_us, ushort date);
        //3.6.8
        [DllImport("UserApplication.dll", EntryPoint = "UINT UAI_SpectromoduleSetIntensityCalibration", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleSetIntensityCalibration(IntPtr handle, double[] gain, ushort date, UInt32 integration_time_us);
        //3.6.9
        [DllImport("UserApplication.dll", EntryPoint = "UINT UAI_SpectromoduleGetIntensityCalibration", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectromoduleGetIntensityCalibration(IntPtr handle, double[] gain, ref ushort date, ref UInt32 integration_time_us);

        //For Color
        //3.7.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorInformationAllocation", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorInformationAllocation(IntPtr handle, ref IntPtr color, uint type, uint observer, uint illuminant, float[] lumbda, float[] intensity_r, float[] intensity_m, uint size);
        //3.7.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorOperation", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorOperation(IntPtr color);
        //3.7.3
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorInformationFree", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorInformationFree(IntPtr color);
        //3.7.4
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetXYZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetXYZ(IntPtr color, double[] XYZ);
        //3.7.5
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetXYZRef", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetXYZRef(IntPtr color, double[] XYZ);
        //3.7.6
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetxyz", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetxyz(IntPtr color, double[] xyz);
        //3.7.7
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetxyzRef", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetxyzRef(IntPtr color, double[] xyz);
        //3.7.8
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGet1960UCS", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGet1960UCS(IntPtr color, double[] UVW);
        //3.7.9
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGet1960ucs", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGet1960ucs(IntPtr color, double[] uvw);
        //3.7.10
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGet1976UCS", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGet1976UCS(IntPtr color, double[] UVW);
        //3.7.11
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGet1976ucs", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGet1976ucs(IntPtr color, double[] uvw);
        //3.7.12
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetCCT", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetCCT(IntPtr color, ref double CCT);
        //3.7.13
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetCIETint", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetCIETint(IntPtr color, ref double Tcie);
        //3.7.14
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetCIEWhiteness", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetCIEWhiteness(IntPtr color, ref double Wcie);
        //3.7.15
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetColorRenderingIndex", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetColorRenderingIndex(IntPtr color, double[] cri,double cct);
        //3.7.16
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetColorQualityScale", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetColorQualityScale(IntPtr color, double[] cqs);
        //3.7.17
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetDominantWavelength", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetDominantWavelength(IntPtr color, ref double lumbda_d);
        //3.7.18
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetHunterLab", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetHunterLab(IntPtr color, double[] HLab);
        //3.7.19
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetDuv", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetDuv(double x, double y, double[] duv);
        //3.7.20
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetLab", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetLab(IntPtr color, double[] Lab);
        //3.7.21
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetLuv", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetLuv(IntPtr color, double[] Luv);
        //3.7.22
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetPurity", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetPurity(IntPtr color, ref double purity_e);
        //3.7.23
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetRadiantPower", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetRadiantPower(IntPtr color, ref double RadiantPower);
        //3.7.24
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetUVW", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetUVW(IntPtr color, double[] UVW);
        //3.7.25
        [DllImport("UserApplication.dll", EntryPoint = "UAI_ColorGetuvw", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_ColorGetuvw(IntPtr color, double[] uvw);

        //Lamp control
        //3.8.1
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetExternalPort", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetExternalPort(IntPtr handle, UInt32 port);
        //3.8.2
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetExternalPort", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetExternalPort(IntPtr handle, ref UInt32 port);
        //3.8.3
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerInitUserRom", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerInitUserRom(IntPtr handle);
        //3.8.4
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetUserRom", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerSetUserRom(IntPtr handle, byte[] buffer, UInt32 length, UInt32 offset);
        //3.8.5
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetUserRom", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetUserRom(IntPtr handle, byte[] buffer, UInt32 length, UInt32 offset);
        //3.8.6
        [DllImport("UserApplication.dll", EntryPoint = "UAI_MATHGetCurveInfo", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_MATHGetCurveInfo(float[] lambda, float[] intensity, UInt32 size, float wavelength_start, float wavelength_end, float[] lambdaP, ref float CenterWavelength, float FWHM);

        //Sample for SDK 
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetActivatingDate", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetActivatingDate(IntPtr handle, byte[] Date);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetManufacturingDate", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetManufacturingDate(IntPtr handle, byte[] Date);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetRomSerialNumber", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetRomSerialNumber(IntPtr handle, byte[] ROM_SN);

        [DllImport("DeviceLibrary.dll", EntryPoint = "DLI_SpectrometerSetWiFiGeneral", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 DLI_SpectrometerSetWiFiGeneral(IntPtr handle, byte[] set_buf, UInt32 set_bytes, byte[] rsp_buf, ref UInt32 rep_bytes);

        //-------------------------
        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetXenonPulseDelay", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerSetXenonPulseDelay(IntPtr api_handle, UInt32 time_us);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetXenonPulseDelay", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetXenonPulseDelay(IntPtr api_handle, UInt32* time_us);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetXenonPulseWidth", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerSetXenonPulseWidth(IntPtr api_handle, UInt32 time_us);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetXenonPulseWidth", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetXenonPulseWidth(IntPtr api_handle, UInt32* time_us);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetXenonPulseNumber", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerSetXenonPulseNumber(IntPtr api_handle, UInt32 value);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetXenonPulseNumber", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetXenonPulseNumber(IntPtr api_handle, UInt32* value);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetXenonPulseInterval", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerSetXenonPulseInterval(IntPtr api_handle, UInt32 time_us);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetXenonPulseInterval", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetXenonPulseInterval(IntPtr api_handle, UInt32* time_us);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetXenonMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 UAI_SpectrometerSetXenonMode(IntPtr api_handle, UInt32 onoff);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetXenonMode", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern UInt32 UAI_SpectrometerGetXenonMode(IntPtr api_handle, UInt32* onoff);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerSetTriggerIO")]
        public unsafe static extern UInt32 UAI_SpectrometerSetTriggerIO(IntPtr handle, UInt32 enable, UInt32 timeout, UInt32 level);

        [DllImport("UserApplication.dll", EntryPoint = "UAI_SpectrometerGetTriggerIO")]
        public unsafe static extern UInt32 UAI_SpectrometerGetTriggerIO(IntPtr handle, ref UInt32 enable, UInt32 timeout, ref UInt32 level);
       
        public static int LastIndexOfnumber(byte[] sample)
        {
            int index = sample.Length;
            for (int i = 0; i < sample.Length; i++)
            {
                if (sample[i] == 0)
                    index = i;
            }
            return index;
        }

        public static int IndexOfnumber(byte[] sample)
        {
            int index = sample.Length;
            for (int i = 0; i < sample.Length; i++)
            {
                if (sample[i] == 0)
                {
                    index = i;
                    return index;
                }
            }
            return 0;
        }


        public static void SetTriggerPort(IntPtr DeviceHandle, int index, int ishigh)
        {
            try
            {
                UInt32 GetedResult = 0;
                UAI_SpectrometerGetExternalPort(DeviceHandle, ref GetedResult);
                UAI_SpectrometerGetExternalPort(DeviceHandle, ref GetedResult);
                string s_Result = Convert.ToString(GetedResult, 2).PadLeft(32, '0');
                s_Result = s_Result.Remove(32 - index, 1);
                s_Result = s_Result.Insert(32 - index, ishigh.ToString());
                UAI_SpectrometerSetExternalPort(DeviceHandle, Convert.ToUInt32(s_Result, 2));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static int GetTriggerPort(IntPtr DeviceHandle, int index)
        {
            try
            {
                UInt32 GetedResult = 0;
                UAI_SpectrometerGetExternalPort(DeviceHandle, ref GetedResult);
                UAI_SpectrometerGetExternalPort(DeviceHandle, ref GetedResult);
                string s_Result = Convert.ToString(GetedResult, 2).PadLeft(32, '0');
                int ishigh = Convert.ToInt16(s_Result.Substring(32 - index, 1));
                return ishigh;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return -1;
            }
        }
    }
    class Link_DLI
    {
        [DllImport("Devicelibrary.dll", EntryPoint = "DLI_SpectrometerReadRom")]
        public static extern UInt32 DLI_SpectrometerReadRom(IntPtr handle, UInt32 address, UInt32 length, ref Int16 buffer);

        //Lamp control
        [DllImport("DeviceLibrary.dll", EntryPoint = "DLI_SpectrometerSetExternalPort")]
        public unsafe static extern UInt32 DLI_SpectrometerSetExternalPort(int handle, UInt32 port);

        [DllImport("DeviceLibrary.dll", EntryPoint = "DLI_SpectrometerGetExternalPort")]
        public unsafe static extern UInt32 DLI_SpectrometerGetExternalPort(int handle, ref UInt32 port);


        public static void SetTriggerPort(int DeviceHandle, int index, int ishigh) //index start 1.
        {
            try
            {
                UInt32 GetedResult = 0;
                DLI_SpectrometerGetExternalPort(DeviceHandle, ref GetedResult);
                DLI_SpectrometerGetExternalPort(DeviceHandle, ref GetedResult);
                string s_Result = Convert.ToString(GetedResult, 2).PadLeft(32, '0');
                s_Result = s_Result.Remove(32 - index, 1);
                s_Result = s_Result.Insert(32 - index, ishigh.ToString());
                DLI_SpectrometerSetExternalPort(DeviceHandle, Convert.ToUInt32(s_Result, 2));



            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }


        public static void DoStrayLightCalibration(IntPtr handle, float[] Intensity, Int16[,] table)
        {
            uint errorcode = 0;
            //Int16[,] table = new Int16[Intensity.Length,Intensity.Length];
            float[] result = new float[Intensity.Length];



            for (int y = 0; y < Intensity.Length; y++)
            {
                for (int x = 0; x < Intensity.Length; x++)
                {
                    if (Math.Abs(x - y) < 170)
                    {
                        result[y] += Intensity[x] * table[x, y];
                    }
                }
                result[y] /= 30000;
            }
            Array.Copy(result, Intensity, Intensity.Length);
        }
    }
}
