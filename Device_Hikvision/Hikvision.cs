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
        //CameraOperator cameraOperator = new CameraOperator();
        #region parameter define
        private MV_CC_DEVICE_INFO_LIST MV_INFO_LIST = new MV_CC_DEVICE_INFO_LIST();
        private CameraOperator[] m_pOperator;
        private Dictionary<uint, int> m_KeyIndex = new Dictionary<uint, int>();


        
        //private IntPtr _currentStableBuffer => _rawBufferPtrs[1 - _writeIndex]; //目前穩定的緩衝區指標


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
        private void InitBuffer(uint payloadsize)   //只能呼叫一次
        {
            //for (int i = 0; i < 2; i++)
            //{
            //    if (_rawBufferPtrs[i] != IntPtr.Zero) Marshal.FreeHGlobal(_rawBufferPtrs[i]);
            //    _rawBufferPtrs[i] = Marshal.AllocHGlobal((int)payloadsize);
            //}
        }
        private int CheckConnect(uint id, ref int index)
        {
            m_KeyIndex.TryGetValue(id, out index);

            if (index == -1)
                return (int)ERROR_CODE.ERROR_NONE_ID;

            if (m_pOperator[index].IsConnected == false)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            return (int)ERROR_CODE.STATUS_OK;
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

                nRet = m_pOperator[i].SetIntValue("DeviceStreamChannelPacketSize", 8164);
                nRet = m_pOperator[i].SetIntValue("GevSCPSPacketSize", 8164);
                nRet = m_pOperator[i].SetIntValue("GevHeartbeatTimeout", 500);
                nRet = m_pOperator[i].SetIntValue("GevSCPD", 0);        //GigE Vision Stream Channel Packet Delay

                nRet = m_pOperator[i].SetEnumValue("PixelFormat", (uint)MvGvspPixelType.PixelType_Gvsp_Mono8);
                nRet = m_pOperator[i].SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                nRet = m_pOperator[i].SetEnumValue("TriggerSource", (uint)MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                //nRet = m_pOperator[i].SetEnumValue("AcquisitionMode", (uint)MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                nRet = m_pOperator[i].SetEnumValue("BalanceWhiteAuto", (uint)MV_CAM_BALANCEWHITE_AUTO.MV_BALANCEWHITE_AUTO_OFF);
                nRet = m_pOperator[i].SetEnumValue("ExposureMode", (uint)MV_CAM_EXPOSURE_MODE.MV_EXPOSURE_MODE_TIMED);
                nRet = m_pOperator[i].SetEnumValue("ExposureAuto", (uint)MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF);

                if (device.nTLayerType == MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MV_GIGE_DEVICE_INFO gigeInfo = (MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    m_KeyIndex[gigeInfo.nCurrentIp] = i;

                    byte[] bytes = BitConverter.GetBytes(gigeInfo.nCurrentIp);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

                    IPAddress ipAddr = new IPAddress(bytes);
                    string ipString = ipAddr.ToString(); //IP

                }
                else if (device.nTLayerType == MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MV_USB3_DEVICE_INFO usbInfo = (MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    m_KeyIndex[usbInfo.idProduct] = i;
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

        public int StartGrabbing(uint id)
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

            uint payloadsize = 0;
            ret = m_pOperator[index].GetIntValue("PayloadSize", ref payloadsize);   //影像一偵的大小
            ret = m_pOperator[index].CommandExecute("AcquisitionStart");

            if(ret != (int)ERROR_CODE.STATUS_OK)
            {
                m_pOperator[index].StopGrabbing();
                return (int)ERROR_CODE.ERROR_START_GRABBING_FAIL;
            }
            else
                return (int)ERROR_CODE.STATUS_OK;
        }

        public int StopGrabbing(uint id)
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

        public int SoftwareTrigger(uint id)
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

        public int GetImage(uint id)
        {
            return 0;
            //int ret, index = -1;

            //ret = CheckConnect(id, ref index);
            //if (ret != (int)ERROR_CODE.STATUS_OK) return ret;

            //// 檢查指標是否已初始化
            //if (_rawBufferPtrs[0] == IntPtr.Zero)
            //    return (int)ERROR_CODE.ERROR_GETIMAGE;

            //MV_FRAME_OUT_INFO_EX frameInfo = new MV_FRAME_OUT_INFO_EX();

            //// 直接取圖不需要再Alloc
            //uint bytePerPixel = 0;
            //int nRet = m_pOperator[index].GetOneFrameTimeOut(_rawBufferPtrs[0], ref bytePerPixel, (uint)_rawImageBuffers[0].Length,  ref frameInfo, 3000);
            
            //if (nRet == MyCamera.MV_OK)
            //{
            //    _writeIndex = 1 - _writeIndex;
            //    return 0;
            //}
                
            //return (int)ERROR_CODE.ERROR_GETIMAGE;
        }
    }
}
