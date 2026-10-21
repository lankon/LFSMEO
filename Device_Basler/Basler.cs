using DeviceCore;
using PylonC.NET;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Device_Basler
{
    public class Basler : ICamera, IDisposable
    {
        #region parameter define
        private const int CCDMAX = 6;
        private const int STREAM_BUFFER_COUNT = 16;
        private const uint GET_IMAGE_TIMEOUT = 3000;

        private readonly List<CameraContext> m_pOperator = new List<CameraContext>();
        private readonly Dictionary<string, int> m_KeyIndex = new Dictionary<string, int>();

        private bool m_PylonInitialized;
        private bool m_Disposed;

        private enum ERROR_CODE
        {
            STATUS_OK = 0,

            ERROR_CONNECT_FAIL = -1,
            ERROR_START_GRABBING_FAIL = -2,
            ERROR_STOP_GRABBING_FAIL = -3,
            ERROR_NONE_ID = -4,
            ERROR_NONE_CONNECT = -5,
            ERROR_TRIGGER = -6,
            ERROR_GETIMAGE = -7,
        }

        private sealed class CameraContext
        {
            public readonly PylonBuffer<byte>[] StreamBuffers = new PylonBuffer<byte>[STREAM_BUFFER_COUNT]; // Pylon 取像資料緩衝區
            public readonly PYLON_STREAMBUFFER_HANDLE[] BufferHandles = new PYLON_STREAMBUFFER_HANDLE[STREAM_BUFFER_COUNT]; // 已註冊的取像緩衝區 Handle
            public readonly IntPtr[] RawBufferPtrs = new IntPtr[2]; // 回傳給 UI 的非受控影像雙緩衝區

            public string ID;                              // GigE 相機 IP 或 USB 相機序號
            public PYLON_DEVICE_HANDLE Device;             // Pylon 相機裝置 Handle
            public PYLON_STREAMGRABBER_HANDLE Stream;      // Pylon 影像串流 Handle
            public PYLON_WAITOBJECT_HANDLE StreamEvent;    // 等待影像到達的事件 Handle
            public byte[] ConvertedBuffer;                 // 像素格式轉換及 Stride 對齊後的影像資料
            public int PayloadSize;                        // 相機單張影像 Payload 大小
            public int RegisteredBufferCount;              // 已成功註冊至 Stream 的緩衝區數量
            public int RawBufferSize;                      // 每個非受控影像緩衝區的配置大小
            public int WriteIndex;                         // 目前寫入的雙緩衝區索引
            public bool DeviceOpened;                      // DeviceOpen 是否已成功執行
            public bool StreamOpened;                      // StreamGrabberOpen 是否已成功執行
            public bool GrabPrepared;                      // StreamGrabberPrepareGrab 是否已完成
            public bool IsConnected;                       // 相機是否已成功開啟並完成初始化
            public bool IsGrabbing;                        // 相機是否已執行 AcquisitionStart
        }
        #endregion

        #region private function
        private int CheckConnect(string id, out CameraContext camera)
        {
            camera = null;

            int index;
            if (String.IsNullOrWhiteSpace(id) || !m_KeyIndex.TryGetValue(id, out index))
                return (int)ERROR_CODE.ERROR_NONE_ID;

            if (index < 0 || index >= m_pOperator.Count)
                return (int)ERROR_CODE.ERROR_NONE_ID;

            camera = m_pOperator[index];
            if (camera == null || !camera.IsConnected)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            return (int)ERROR_CODE.STATUS_OK;
        }

        private string GetCameraID(PYLON_DEVICE_HANDLE device)
        {
            if (Pylon.DeviceFeatureIsReadable(device, "GevCurrentIPAddress"))
            {
                uint address = unchecked((uint)Pylon.DeviceGetIntegerFeature(device, "GevCurrentIPAddress"));
                return String.Format("{0}.{1}.{2}.{3}",
                                     (address >> 24) & 0xFF,
                                     (address >> 16) & 0xFF,
                                     (address >> 8) & 0xFF,
                                     address & 0xFF);
            }

            if (Pylon.DeviceFeatureIsReadable(device, "DeviceSerialNumber"))
                return Pylon.DeviceFeatureToString(device, "DeviceSerialNumber");

            return null;
        }

        private void SetEnumFeatureIfWritable(PYLON_DEVICE_HANDLE device, string feature, string value)
        {
            if (Pylon.DeviceFeatureIsAvailable(device, feature) &&
                Pylon.DeviceFeatureIsWritable(device, feature))
            {
                Pylon.DeviceFeatureFromString(device, feature, value);
            }
        }

        private void CameraInitialSetting(CameraContext camera)
        {
            PYLON_DEVICE_HANDLE device = camera.Device;

            if (!Pylon.DeviceFeatureIsWritable(device, "PixelFormat"))
                throw new InvalidOperationException("PixelFormat is not writable.");

            if (Pylon.DeviceFeatureIsAvailable(device, "EnumEntry_PixelFormat_YUV422Packed"))
                Pylon.DeviceFeatureFromString(device, "PixelFormat", "YUV422Packed");
            else if (Pylon.DeviceFeatureIsAvailable(device, "EnumEntry_PixelFormat_Mono8"))
                Pylon.DeviceFeatureFromString(device, "PixelFormat", "Mono8");
            else
                throw new NotSupportedException("The camera does not support YUV422Packed or Mono8.");

            if (Pylon.DeviceFeatureIsWritable(device, "TriggerSelector"))
            {
                if (Pylon.DeviceFeatureIsAvailable(device, "EnumEntry_TriggerSelector_FrameStart"))
                    Pylon.DeviceFeatureFromString(device, "TriggerSelector", "FrameStart");
                else if (Pylon.DeviceFeatureIsAvailable(device, "EnumEntry_TriggerSelector_AcquisitionStart"))
                    Pylon.DeviceFeatureFromString(device, "TriggerSelector", "AcquisitionStart");
            }

            SetEnumFeatureIfWritable(device, "TriggerMode", "On");
            SetEnumFeatureIfWritable(device, "TriggerSource", "Software");
            SetEnumFeatureIfWritable(device, "AcquisitionMode", "Continuous");

            if (Pylon.DeviceFeatureIsWritable(device, "GevSCPSPacketSize"))
                Pylon.DeviceSetIntegerFeature(device, "GevSCPSPacketSize", 1500);

            SetEnumFeatureIfWritable(device, "TestImageSelector", "Off");

            long payloadSize = Pylon.DeviceGetIntegerFeature(device, "PayloadSize");
            if (payloadSize <= 0 || payloadSize > Int32.MaxValue)
                throw new InvalidOperationException("Invalid camera payload size.");

            camera.PayloadSize = (int)payloadSize;
        }

        private void PrepareStream(CameraContext camera)
        {
            if (camera.GrabPrepared)
                return;

            try
            {
                if (Pylon.DeviceGetNumStreamGrabberChannels(camera.Device) == 0)
                    throw new InvalidOperationException("The camera has no stream grabber channel.");

                camera.Stream = Pylon.DeviceGetStreamGrabber(camera.Device, 0);
                if (camera.Stream == null || !camera.Stream.IsValid)
                    throw new InvalidOperationException("Unable to get the camera stream grabber.");

                Pylon.StreamGrabberOpen(camera.Stream);
                camera.StreamOpened = true;
                camera.StreamEvent = Pylon.StreamGrabberGetWaitObject(camera.Stream);

                Pylon.StreamGrabberSetMaxNumBuffer(camera.Stream, STREAM_BUFFER_COUNT);
                Pylon.StreamGrabberSetMaxBufferSize(camera.Stream, (uint)camera.PayloadSize);

                for (int i = 0; i < STREAM_BUFFER_COUNT; i++)
                    camera.StreamBuffers[i] = new PylonBuffer<byte>((uint)camera.PayloadSize, true);

                Pylon.StreamGrabberPrepareGrab(camera.Stream);
                camera.GrabPrepared = true;

                for (int i = 0; i < STREAM_BUFFER_COUNT; i++)
                {
                    camera.BufferHandles[i] = Pylon.StreamGrabberRegisterBuffer(camera.Stream, ref camera.StreamBuffers[i]);
                    camera.RegisteredBufferCount++;
                }
            }
            catch
            {
                ReleaseStream(camera);
                throw;
            }
        }

        private void QueueAllBuffers(CameraContext camera)
        {
            for (int i = 0; i < camera.RegisteredBufferCount; i++)
                Pylon.StreamGrabberQueueBuffer(camera.Stream, camera.BufferHandles[i], i);
        }

        private void CancelAndDrain(CameraContext camera)
        {
            if (!camera.StreamOpened || camera.Stream == null || !camera.Stream.IsValid)
                return;

            try
            {
                Pylon.StreamGrabberCancelGrab(camera.Stream);
            }
            catch
            {
            }

            try
            {
                PylonGrabResult_t grabResult;
                while (Pylon.StreamGrabberRetrieveResult(camera.Stream, out grabResult))
                {
                }
            }
            catch
            {
            }
        }

        private void ReleaseStream(CameraContext camera)
        {
            if (camera.IsGrabbing && camera.DeviceOpened)
            {
                try
                {
                    Pylon.DeviceExecuteCommandFeature(camera.Device, "AcquisitionStop");
                }
                catch
                {
                }
            }

            CancelAndDrain(camera);
            camera.IsGrabbing = false;

            if (camera.StreamOpened && camera.Stream != null && camera.Stream.IsValid)
            {
                for (int i = 0; i < camera.RegisteredBufferCount; i++)
                {
                    try
                    {
                        Pylon.StreamGrabberDeregisterBuffer(camera.Stream, camera.BufferHandles[i]);
                    }
                    catch
                    {
                    }
                }
            }

            camera.RegisteredBufferCount = 0;

            for (int i = 0; i < camera.StreamBuffers.Length; i++)
            {
                if (camera.StreamBuffers[i] != null)
                {
                    camera.StreamBuffers[i].Dispose();
                    camera.StreamBuffers[i] = null;
                }

                camera.BufferHandles[i] = null;
            }

            if (camera.GrabPrepared && camera.StreamOpened && camera.Stream != null && camera.Stream.IsValid)
            {
                try
                {
                    Pylon.StreamGrabberFinishGrab(camera.Stream);
                }
                catch
                {
                }
            }

            camera.GrabPrepared = false;

            if (camera.StreamOpened && camera.Stream != null && camera.Stream.IsValid)
            {
                try
                {
                    Pylon.StreamGrabberClose(camera.Stream);
                }
                catch
                {
                }
            }

            camera.StreamOpened = false;
            camera.StreamEvent = null;
            camera.Stream = null;
        }

        private void ReleaseCamera(CameraContext camera)
        {
            if (camera == null)
                return;

            camera.IsConnected = false;
            ReleaseStream(camera);

            for (int i = 0; i < camera.RawBufferPtrs.Length; i++)
            {
                if (camera.RawBufferPtrs[i] != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(camera.RawBufferPtrs[i]);
                    camera.RawBufferPtrs[i] = IntPtr.Zero;
                }
            }

            camera.RawBufferSize = 0;
            camera.ConvertedBuffer = null;

            if (camera.DeviceOpened && camera.Device != null && camera.Device.IsValid)
            {
                try
                {
                    Pylon.DeviceClose(camera.Device);
                }
                catch
                {
                }
            }

            camera.DeviceOpened = false;

            if (camera.Device != null && camera.Device.IsValid)
            {
                try
                {
                    Pylon.DestroyDevice(camera.Device);
                }
                catch
                {
                }
            }

            camera.Device = null;
        }

        private void ReleaseAllCameras()
        {
            for (int i = 0; i < m_pOperator.Count; i++)
                ReleaseCamera(m_pOperator[i]);

            m_pOperator.Clear();
            m_KeyIndex.Clear();

            if (m_PylonInitialized)
            {
                try
                {
                    Pylon.Terminate();
                }
                catch
                {
                }

                m_PylonInitialized = false;
            }
        }

        private bool EnsureRawBuffers(CameraContext camera, int bufferSize)
        {
            if (bufferSize <= 0)
                return false;

            if (camera.RawBufferSize == bufferSize &&
                camera.RawBufferPtrs[0] != IntPtr.Zero &&
                camera.RawBufferPtrs[1] != IntPtr.Zero)
            {
                return true;
            }

            for (int i = 0; i < camera.RawBufferPtrs.Length; i++)
            {
                if (camera.RawBufferPtrs[i] != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(camera.RawBufferPtrs[i]);
                    camera.RawBufferPtrs[i] = IntPtr.Zero;
                }
            }

            camera.RawBufferSize = 0;

            try
            {
                camera.RawBufferPtrs[0] = Marshal.AllocHGlobal(bufferSize);
                camera.RawBufferPtrs[1] = Marshal.AllocHGlobal(bufferSize);
                camera.RawBufferSize = bufferSize;
                camera.WriteIndex = 0;
                return true;
            }
            catch
            {
                for (int i = 0; i < camera.RawBufferPtrs.Length; i++)
                {
                    if (camera.RawBufferPtrs[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(camera.RawBufferPtrs[i]);
                        camera.RawBufferPtrs[i] = IntPtr.Zero;
                    }
                }

                return false;
            }
        }

        private int GetAlignedStride(int width, int bytesPerPixel)
        {
            return checked(((checked(width * bytesPerPixel) + 3) / 4) * 4);
        }

        private byte ClampToByte(int value)
        {
            if (value < 0)
                return 0;
            if (value > 255)
                return 255;
            return (byte)value;
        }

        private void ConvertYuvPixelPair(byte y1, byte y2, byte u, byte v,
                                                byte[] destination, int destinationOffset)
        {
            int cb = u - 128;
            int cr = v - 128;
            int redOffset = (359 * cr) >> 8;
            int greenOffset = -((88 * cb + 183 * cr) >> 8);
            int blueOffset = (454 * cb) >> 8;

            destination[destinationOffset] = ClampToByte(y1 + blueOffset);
            destination[destinationOffset + 1] = ClampToByte(y1 + greenOffset);
            destination[destinationOffset + 2] = ClampToByte(y1 + redOffset);
            destination[destinationOffset + 3] = ClampToByte(y2 + blueOffset);
            destination[destinationOffset + 4] = ClampToByte(y2 + greenOffset);
            destination[destinationOffset + 5] = ClampToByte(y2 + redOffset);
        }

        private bool ConvertImage(CameraContext camera, byte[] source, PylonGrabResult_t grabResult,
                                         out IntPtr image, out int imageWidth, out int imageHeight,
                                         out PixelFormat pixelFormat)
        {
            image = IntPtr.Zero;
            imageWidth = grabResult.SizeX;
            imageHeight = grabResult.SizeY;
            pixelFormat = PixelFormat.Undefined;

            if (source == null || imageWidth <= 0 || imageHeight <= 0 || grabResult.PaddingX < 0)
                return false;

            int sourceBytesPerPixel;
            int destinationBytesPerPixel;

            switch (grabResult.PixelType)
            {
                case EPylonPixelType.PixelType_Mono8:
                    sourceBytesPerPixel = 1;
                    destinationBytesPerPixel = 1;
                    pixelFormat = PixelFormat.Format8bppIndexed;
                    break;

                case EPylonPixelType.PixelType_YUV422packed:
                case EPylonPixelType.PixelType_YUV422_YUYV_Packed:
                    if ((imageWidth & 1) != 0)
                        return false;
                    sourceBytesPerPixel = 2;
                    destinationBytesPerPixel = 3;
                    pixelFormat = PixelFormat.Format24bppRgb;
                    break;

                case EPylonPixelType.PixelType_BGR8packed:
                case EPylonPixelType.PixelType_RGB8packed:
                    sourceBytesPerPixel = 3;
                    destinationBytesPerPixel = 3;
                    pixelFormat = PixelFormat.Format24bppRgb;
                    break;

                default:
                    return false;
            }

            int sourceRowSize;
            int sourceStride;
            int destinationStride;
            int destinationSize;

            try
            {
                sourceRowSize = checked(imageWidth * sourceBytesPerPixel);
                sourceStride = checked(sourceRowSize + grabResult.PaddingX);
                destinationStride = GetAlignedStride(imageWidth, destinationBytesPerPixel);
                destinationSize = checked(destinationStride * imageHeight);
            }
            catch (OverflowException)
            {
                return false;
            }

            long requiredSourceSize = (long)sourceStride * imageHeight;
            if (requiredSourceSize > source.Length || requiredSourceSize > grabResult.PayloadSize)
                return false;

            if (camera.ConvertedBuffer == null || camera.ConvertedBuffer.Length != destinationSize)
                camera.ConvertedBuffer = new byte[destinationSize];

            byte[] destination = camera.ConvertedBuffer;

            for (int y = 0; y < imageHeight; y++)
            {
                int sourceOffset = y * sourceStride;
                int destinationOffset = y * destinationStride;

                switch (grabResult.PixelType)
                {
                    case EPylonPixelType.PixelType_Mono8:
                    case EPylonPixelType.PixelType_BGR8packed:
                        Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, sourceRowSize);
                        break;

                    case EPylonPixelType.PixelType_RGB8packed:
                        for (int x = 0; x < imageWidth; x++)
                        {
                            int sourcePixel = sourceOffset + x * 3;
                            int destinationPixel = destinationOffset + x * 3;
                            destination[destinationPixel] = source[sourcePixel + 2];
                            destination[destinationPixel + 1] = source[sourcePixel + 1];
                            destination[destinationPixel + 2] = source[sourcePixel];
                        }
                        break;

                    case EPylonPixelType.PixelType_YUV422packed:
                        for (int x = 0; x < imageWidth; x += 2)
                        {
                            int sourcePixel = sourceOffset + x * 2;
                            int destinationPixel = destinationOffset + x * 3;
                            ConvertYuvPixelPair(source[sourcePixel + 1], source[sourcePixel + 3],
                                                source[sourcePixel], source[sourcePixel + 2],
                                                destination, destinationPixel);
                        }
                        break;

                    case EPylonPixelType.PixelType_YUV422_YUYV_Packed:
                        for (int x = 0; x < imageWidth; x += 2)
                        {
                            int sourcePixel = sourceOffset + x * 2;
                            int destinationPixel = destinationOffset + x * 3;
                            ConvertYuvPixelPair(source[sourcePixel], source[sourcePixel + 2],
                                                source[sourcePixel + 1], source[sourcePixel + 3],
                                                destination, destinationPixel);
                        }
                        break;
                }

                for (int padding = imageWidth * destinationBytesPerPixel;
                     padding < destinationStride;
                     padding++)
                {
                    destination[destinationOffset + padding] = 0;
                }
            }

            if (!EnsureRawBuffers(camera, destinationSize))
                return false;

            IntPtr currentPtr = camera.RawBufferPtrs[camera.WriteIndex];
            Marshal.Copy(destination, 0, currentPtr, destinationSize);
            camera.WriteIndex = 1 - camera.WriteIndex;
            image = currentPtr;
            return true;
        }
        #endregion

        #region public function
        public CCD_TYPE GetCameraType()
        {
            return CCD_TYPE.Basler;
        }

        public int Connect()
        {
            if (m_Disposed)
                return (int)ERROR_CODE.ERROR_CONNECT_FAIL;

            ReleaseAllCameras();

            try
            {
                Pylon.Initialize();
                m_PylonInitialized = true;

                uint totalCameraCount = Pylon.EnumerateDevices();
                uint connectCount = Math.Min(totalCameraCount, (uint)CCDMAX);

                for (uint cameraIndex = 0; cameraIndex < connectCount; cameraIndex++)
                {
                    CameraContext camera = new CameraContext();

                    try
                    {
                        camera.Device = Pylon.CreateDeviceByIndex(cameraIndex);
                        if (camera.Device == null || !camera.Device.IsValid)
                            throw new InvalidOperationException("Unable to create the camera device.");

                        Pylon.DeviceOpen(camera.Device,
                                         Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
                        camera.DeviceOpened = true;

                        camera.ID = GetCameraID(camera.Device);
                        if (String.IsNullOrWhiteSpace(camera.ID) || m_KeyIndex.ContainsKey(camera.ID))
                            throw new InvalidOperationException("Camera ID is empty or duplicated.");

                        CameraInitialSetting(camera);
                        camera.IsConnected = true;

                        m_KeyIndex.Add(camera.ID, m_pOperator.Count);
                        m_pOperator.Add(camera);
                    }
                    catch
                    {
                        ReleaseCamera(camera);
                    }
                }

                if (m_pOperator.Count == 0)
                {
                    ReleaseAllCameras();
                    return (int)ERROR_CODE.ERROR_CONNECT_FAIL;
                }

                return (int)ERROR_CODE.STATUS_OK;
            }
            catch
            {
                ReleaseAllCameras();
                return (int)ERROR_CODE.ERROR_CONNECT_FAIL;
            }
        }

        public int StartGrabbing(string id)
        {
            CameraContext camera;
            int ret = CheckConnect(id, out camera);
            if (ret != (int)ERROR_CODE.STATUS_OK)
                return ret;

            if (!camera.IsConnected)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            if (camera.IsGrabbing)
                return (int)ERROR_CODE.STATUS_OK;

            try
            {
                PrepareStream(camera);
                QueueAllBuffers(camera);
                Pylon.DeviceExecuteCommandFeature(camera.Device, "AcquisitionStart");
                camera.IsGrabbing = true;
                return (int)ERROR_CODE.STATUS_OK;
            }
            catch
            {
                CancelAndDrain(camera);
                camera.IsGrabbing = false;
                return (int)ERROR_CODE.ERROR_START_GRABBING_FAIL;
            }
        }

        public int StopGrabbing(string id)
        {
            CameraContext camera;
            int ret = CheckConnect(id, out camera);
            if (ret != (int)ERROR_CODE.STATUS_OK)
                return ret;

            if (!camera.IsConnected)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            if (!camera.IsGrabbing)
                return (int)ERROR_CODE.STATUS_OK;

            bool stopSucceeded = true;
            try
            {
                Pylon.DeviceExecuteCommandFeature(camera.Device, "AcquisitionStop");
            }
            catch
            {
                stopSucceeded = false;
            }

            CancelAndDrain(camera);
            camera.IsGrabbing = false;

            return stopSucceeded
                ? (int)ERROR_CODE.STATUS_OK
                : (int)ERROR_CODE.ERROR_STOP_GRABBING_FAIL;
        }

        public int SoftwareTrigger(string id)
        {
            CameraContext camera;
            int ret = CheckConnect(id, out camera);
            if (ret != (int)ERROR_CODE.STATUS_OK)
                return ret;

            if (!camera.IsConnected)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            if (!camera.IsGrabbing)
                return (int)ERROR_CODE.ERROR_TRIGGER;

            try
            {
                Pylon.DeviceExecuteCommandFeature(camera.Device, "TriggerSoftware");
                return (int)ERROR_CODE.STATUS_OK;
            }
            catch
            {
                return (int)ERROR_CODE.ERROR_TRIGGER;
            }
        }

        public int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height,
                            ref PixelFormat pixelFormat)
        {
            CameraContext camera;
            int ret = CheckConnect(id, out camera);
            if (ret != (int)ERROR_CODE.STATUS_OK)
                return ret;

            image = IntPtr.Zero;
            image_width = 0;
            image_height = 0;
            pixelFormat = PixelFormat.Undefined;

            if (!camera.IsConnected)
                return (int)ERROR_CODE.ERROR_NONE_CONNECT;

            if (!camera.IsGrabbing || camera.StreamEvent == null || !camera.StreamEvent.IsValid)
                return (int)ERROR_CODE.ERROR_GETIMAGE;

            List<PylonGrabResult_t> retrievedResults = new List<PylonGrabResult_t>(STREAM_BUFFER_COUNT);
            PylonGrabResult_t latestGrabbedResult = null;
            int result = (int)ERROR_CODE.ERROR_GETIMAGE;
            bool requeueSucceeded = true;

            try
            {
                if (!Pylon.WaitObjectWait(camera.StreamEvent, GET_IMAGE_TIMEOUT))
                    return (int)ERROR_CODE.ERROR_GETIMAGE;

                for (int i = 0; i < camera.RegisteredBufferCount; i++)
                {
                    PylonGrabResult_t grabResult;
                    if (!Pylon.StreamGrabberRetrieveResult(camera.Stream, out grabResult))
                        break;

                    retrievedResults.Add(grabResult);
                    if (grabResult.Status == EPylonGrabStatus.Grabbed)
                        latestGrabbedResult = grabResult;
                }

                if (latestGrabbedResult == null ||
                    latestGrabbedResult.Context < 0 ||
                    latestGrabbedResult.Context >= camera.RegisteredBufferCount)
                {
                    result = (int)ERROR_CODE.ERROR_GETIMAGE;
                }
                else
                {
                    PylonBuffer<byte> sourceBuffer = camera.StreamBuffers[latestGrabbedResult.Context];
                    result = ConvertImage(camera, sourceBuffer.Array, latestGrabbedResult,
                                          out image, out image_width, out image_height,
                                          out pixelFormat)
                        ? (int)ERROR_CODE.STATUS_OK
                        : (int)ERROR_CODE.ERROR_GETIMAGE;
                }
            }
            catch
            {
                result = (int)ERROR_CODE.ERROR_GETIMAGE;
            }
            finally
            {
                for (int i = 0; i < retrievedResults.Count; i++)
                {
                    try
                    {
                        PylonGrabResult_t grabResult = retrievedResults[i];
                        Pylon.StreamGrabberQueueBuffer(camera.Stream,
                                                      grabResult.hBuffer,
                                                      grabResult.Context);
                    }
                    catch
                    {
                        requeueSucceeded = false;
                    }
                }
            }

            if (!requeueSucceeded)
            {
                try
                {
                    Pylon.DeviceExecuteCommandFeature(camera.Device, "AcquisitionStop");
                }
                catch
                {
                }

                CancelAndDrain(camera);
                camera.IsGrabbing = false;
                return (int)ERROR_CODE.ERROR_GETIMAGE;
            }

            return result;
        }

        public void SetVirtualImagePath(string path)
        {
            // Basler is a physical camera provider and does not use a virtual image path.
        }

        public void Dispose()
        {
            if (m_Disposed)
                return;

            ReleaseAllCameras();
            m_Disposed = true;

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
