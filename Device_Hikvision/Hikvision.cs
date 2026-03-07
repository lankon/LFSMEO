using DeviceCore;
using DeviceSourceHikvision;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MvCamCtrl.NET.MyCamera;

namespace Device_Hikvision
{
    public class Hikvision: ICamera
    {
        #region parameter define
        private MV_CC_DEVICE_INFO_LIST MV_INFO_LIST = new MV_CC_DEVICE_INFO_LIST();
        private CameraOperator[] m_pOperator;
        private Dictionary<string, int> m_KeyIndex = new Dictionary<string, int>();

        enum ERROR_CODE
        {
            STATUS_OK = MV_OK,

            ERROR_CONNECT_FAIL = -1,
            ERROR_START_GRABBING_FAIL = -2,
            ERROR_STOP_GRABBING_FAIL = -3,
            ERROR_NONE_ID = -4,
            ERROR_NONE_CONNECT = -5,
            ERROR_TRIGGER = -6,
            ERROR_GETIMAGE = -7,
        }
        #endregion

        #region private function
        private void InitBuffer(int index)
        {
            uint image_size = 0;
            m_pOperator[index].GetIntValue("PayloadSize", ref image_size);
            int iimage_size = (int)image_size;

            for (int i = 0; i < 2; i++)
            {
                if(m_pOperator[index].RawBufferPtrs[i] != IntPtr.Zero)
                    Marshal.FreeHGlobal(m_pOperator[index].RawBufferPtrs[i]);

                m_pOperator[index].RawBufferPtrs[i] = Marshal.AllocHGlobal(iimage_size);
                m_pOperator[index].ImageSize = iimage_size;
            }
        }
        private int CheckConnect(string id, ref int index)
        {
            m_KeyIndex.TryGetValue(id, out index);

            if (index == -1)
                return (int)ERROR_CODE.ERROR_NONE_ID;

            if (m_pOperator[index].IsConnected == false)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            return (int)ERROR_CODE.STATUS_OK;
        }
        private void CameraInitialSetting(int index)
        {
            int nRet = -1;
            
            nRet = m_pOperator[index].SetIntValue("DeviceStreamChannelPacketSize", 8164);
            nRet = m_pOperator[index].SetIntValue("GevSCPSPacketSize", 8164);
            nRet = m_pOperator[index].SetIntValue("GevHeartbeatTimeout", 500);
            nRet = m_pOperator[index].SetIntValue("GevSCPD", 0);        //GigE Vision Stream Channel Packet Delay

            nRet = m_pOperator[index].SetEnumValue("PixelFormat", (uint)MvGvspPixelType.PixelType_Gvsp_Mono8);
            nRet = m_pOperator[index].SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
            nRet = m_pOperator[index].SetEnumValue("TriggerSource", (uint)MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
            //nRet = m_pOperator[i].SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            nRet = m_pOperator[index].SetEnumValue("BalanceWhiteAuto", (uint)MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_OFF);
            nRet = m_pOperator[index].SetEnumValue("ExposureMode", (uint)MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED);
            nRet = m_pOperator[index].SetEnumValue("ExposureAuto", (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF);
            uint payloadsize = 0;
            nRet = m_pOperator[index].GetIntValue("PayloadSize", ref payloadsize);
        }
        #endregion

        public int Connect()
        {
            int nRet = (int)ERROR_CODE.ERROR_CONNECT_FAIL;

            try
            {
                nRet = CameraOperator.EnumDevices(MV_GIGE_DEVICE | MV_USB_DEVICE, ref MV_INFO_LIST);
            }
            catch
            {
                return (int)ERROR_CODE.ERROR_CONNECT_FAIL;
            }
            
            if (MV_INFO_LIST.nDeviceNum == 0 || nRet != (int)ERROR_CODE.STATUS_OK)
                return (int)ERROR_CODE.ERROR_CONNECT_FAIL;

            m_pOperator = new CameraOperator[MV_INFO_LIST.nDeviceNum];

            int OpenedNum = 0;
            for (int i=0; i< MV_INFO_LIST.nDeviceNum; i++)
            {
                m_pOperator[i] = new CameraOperator();

                MV_CC_DEVICE_INFO device =
                    (MV_CC_DEVICE_INFO)Marshal.PtrToStructure(MV_INFO_LIST.pDeviceInfo[i],
                                                              typeof(MV_CC_DEVICE_INFO));

                nRet = m_pOperator[i].Open(ref device);

                if (nRet != (int)ERROR_CODE.STATUS_OK)
                    continue;

                CameraInitialSetting(i);
                InitBuffer(i);

                if (device.nTLayerType == MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MV_GIGE_DEVICE_INFO gigeInfo = (MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    byte[] bytes = BitConverter.GetBytes(gigeInfo.nCurrentIp);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

                    IPAddress ipAddr = new IPAddress(bytes);
                    m_pOperator[i].ID = ipAddr.ToString();  //IP
                    m_KeyIndex[m_pOperator[i].ID] = i;
                }
                else if (device.nTLayerType == MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MV_USB3_DEVICE_INFO usbInfo = (MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    m_KeyIndex[usbInfo.idProduct.ToString()] = i;
                }

                m_pOperator[i].IsConnected = true;
                OpenedNum++;
            }

            if(OpenedNum > 0)
                nRet = (int)ERROR_CODE.STATUS_OK;
            else
                nRet = (int)ERROR_CODE.ERROR_CONNECT_FAIL;

            return nRet;
        }

        public int StartGrabbing(string id)
        {
            m_KeyIndex.TryGetValue(id, out int index);

            if (index == -1)
                return (int)ERROR_CODE.ERROR_NONE_ID;

            if(m_pOperator[index].IsConnected == false)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            int ret = 0;
            ret = m_pOperator[index].StartGrabbing();

            if (ret != (int)ERROR_CODE.STATUS_OK)
                return (int)ERROR_CODE.ERROR_START_GRABBING_FAIL;

            ret = m_pOperator[index].CommandExecute("AcquisitionStart");

            if(ret != (int)ERROR_CODE.STATUS_OK)
            {
                m_pOperator[index].StopGrabbing();
                return (int)ERROR_CODE.ERROR_START_GRABBING_FAIL;
            }
            else
                return (int)ERROR_CODE.STATUS_OK;
        }

        public int StopGrabbing(string id)
        {
            m_KeyIndex.TryGetValue(id, out int index);

            if (index == -1)
                return (int)ERROR_CODE.ERROR_NONE_ID;

            if (m_pOperator[index].IsConnected == false)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            int ret = 0;
            ret = m_pOperator[index].CommandExecute("AcquisitionStop");

            ret = m_pOperator[index].StopGrabbing();

            if (ret != (int)ERROR_CODE.STATUS_OK)
                return (int)ERROR_CODE.ERROR_STOP_GRABBING_FAIL;
            else
                return (int)ERROR_CODE.STATUS_OK;
        }

        public int SoftwareTrigger(string id)
        {
            int ret, index = -1;

            ret = CheckConnect(id, ref index);
            if (ret != (int)ERROR_CODE.STATUS_OK) return ret;

            //判斷是否為TriggerMode
            uint value = 0;
            m_pOperator[index].GetEnumValue("TriggerMode", ref value);
            if(value == (int)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF)
                ret = m_pOperator[index].SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);

            if (ret != (int)ERROR_CODE.STATUS_OK)
                return (int)ERROR_CODE.ERROR_TRIGGER;
            
            ret = m_pOperator[index].CommandExecute("TriggerSoftware");

            if (ret != (int)ERROR_CODE.STATUS_OK)
                return (int)ERROR_CODE.ERROR_TRIGGER;
            else
                return (int)ERROR_CODE.STATUS_OK;
        }

        public int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height)
        {
            int ret, index = -1;

            ret = CheckConnect(id, ref index);
            if (ret != (int)ERROR_CODE.STATUS_OK) return ret;

            // 檢查指標是否已初始化
            if (m_pOperator[index].RawBufferPtrs[m_pOperator[index].WriteIndex]  == IntPtr.Zero)
                return (int)ERROR_CODE.ERROR_GETIMAGE;

            int currentIndex = m_pOperator[index].WriteIndex;
            IntPtr currentPtr = m_pOperator[index].RawBufferPtrs[currentIndex];

            if (currentPtr == IntPtr.Zero)
                return (int)ERROR_CODE.ERROR_GETIMAGE;

            MV_FRAME_OUT_INFO_EX frameInfo = new MV_FRAME_OUT_INFO_EX();

            // 直接取圖不需要再Alloc
            uint bytePerPixel = 0;
            int nRet = m_pOperator[index].GetOneFrameTimeOut(currentPtr, ref bytePerPixel, 
                                                             (uint)m_pOperator[index].ImageSize, 
                                                             ref frameInfo, 3000);

            if (nRet == MyCamera.MV_OK)
            {
                image = currentPtr;
                image_height = frameInfo.nHeight;
                image_width = frameInfo.nWidth;

                m_pOperator[index].WriteIndex = 1 - m_pOperator[index].WriteIndex;
                return 0;
            }

            return (int)ERROR_CODE.ERROR_GETIMAGE;
        }
    }
}
