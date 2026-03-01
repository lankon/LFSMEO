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
        private MyCamera m_MyCamera;
        private CameraOperator[] m_pOperator;
        private Dictionary<uint, int> m_KeyIndex = new Dictionary<uint, int>();


        private GCHandle _rawBufferHandle;
        private GCHandle _processedBufferHandle;
        private IntPtr _rawBufferPtr = IntPtr.Zero;
        private IntPtr _processedBufferPtr = IntPtr.Zero;
        private byte[] _rawImageBuffer;
        private byte[] _processedImageBuffer;


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
        private int InitializeBuffers(uint payloadSize)
        {
            try
            {
                // 1. 根據相機 PayloadSize 分配空間
                // 建議多留一點空間，例如 * 3 是為了轉成 RGB24 (3 bytes per pixel) 使用
                _rawImageBuffer = new byte[payloadSize];
                _processedImageBuffer = new byte[payloadSize * 3];

                // 2. 釘住記憶體 (Pinning)
                _rawBufferHandle = GCHandle.Alloc(_rawImageBuffer, GCHandleType.Pinned);
                _rawBufferPtr = _rawBufferHandle.AddrOfPinnedObject();

                _processedBufferHandle = GCHandle.Alloc(_processedImageBuffer, GCHandleType.Pinned);
                _processedBufferPtr = _processedBufferHandle.AddrOfPinnedObject();

                return (int)ERROR_CODE.STATUS_OK;
            }
            catch (Exception ex)
            {
                return -1;
            }
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
        public void Deinitialize()
        {
            if (_rawBufferHandle.IsAllocated) _rawBufferHandle.Free();
            if (_processedBufferHandle.IsAllocated) _processedBufferHandle.Free();

            _rawBufferPtr = IntPtr.Zero;
            _processedBufferPtr = IntPtr.Zero;
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
            int ret, index = -1;

            ret = CheckConnect(id, ref index);
            if (ret != (int)ERROR_CODE.STATUS_OK) return ret;

            // 檢查指標是否已初始化
            if (_rawBufferPtr == IntPtr.Zero || _processedBufferPtr == IntPtr.Zero)
                return (int)ERROR_CODE.ERROR_GETIMAGE;

            MV_FRAME_OUT_INFO_EX frameInfo = new MV_FRAME_OUT_INFO_EX();

            // 1. 直接取圖，不需要再 Alloc
            uint bytePerPixel = 0;
            int nRet = m_pOperator[index].GetOneFrameTimeOut(_rawBufferPtr, ref bytePerPixel, (uint)_rawImageBuffer.Length,  ref frameInfo, 3000);
            if (nRet != MyCamera.MV_OK) return (int)ERROR_CODE.ERROR_GETIMAGE;

            //// 2. 設定轉換參數
            //MyCamera.MV_PIXEL_CONVERT_PARAM convertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM
            //{
            //    nWidth = frameInfo.nWidth,
            //    nHeight = frameInfo.nHeight,
            //    pSrcData = _rawBufferPtr,
            //    nSrcDataLen = frameInfo.nFrameLen,
            //    enSrcPixelType = frameInfo.enPixelType,
            //    pDstBuffer = _processedBufferPtr,
            //    nDstBufferSize = (uint)_processedImageBuffer.Length
            //};

            //// 根據你的機台設定決定目標格式 (Leo 習慣使用 Mono8 或 BGR8)
            //convertParam.enDstPixelType = (frameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
            //                              ? MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8
            //                              : MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;

            //nRet = Camera.MV_CC_ConvertPixelType_NET(ref convertParam);


            return 0;
        }

        //public override int Set_ExposureAutoTime(int t) //20220428 Evans
        //{
        //    if (!Manager.IsConnected)
        //    {
        //        return -1;
        //    }

        //    int nRet;

        //    nRet = SetIntValue("AutoExposureTimeUpperLimit", (uint)t);

        //    return nRet;
        //}





    }
}
