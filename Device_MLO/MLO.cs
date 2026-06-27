using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceCore;
using Python.Runtime;

namespace Device_MLO
{
    public class MLO:ICamera
    {
        public MLO()
        {
            //設定Python執行環境
            Runtime.PythonDLL = @"C:\Program Files\Python312\python312.dll";
            string pythonHome = @"C:\Program Files\Python312";
            Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome, EnvironmentVariableTarget.Process);

            //初始化Python引擎
            PythonEngine.Initialize();
        }

        #region private function
        private bool IsConnect = false;
        private dynamic MLO_Camera;         //取得的Python物件
        private dynamic Sys;
        private dynamic Io;
        private dynamic OldStdout;
        private dynamic OldStderr;
        private dynamic NewStdout;
        private dynamic NewStderr;

        enum ERROR_CODE
        {
            SUCCESS = 0,
            PYTHON_NOT_INITIALIZED ,
            PYTHON_IMPORT_FAILED,
            CAMERA_NOT_CONNECTED,
            IMAGE_ACQUISITION_FAILED,
            INVALID_PARAMETER,
            UNKNOWN_ERROR
        }
        #endregion

        #region private function
        private void InitialReceiveMsg()
        {
            using (Py.GIL())
            {
                Sys = Py.Import("sys");
                Io = Py.Import("io");
            }

            OldStdout = Sys.stdout;
            OldStderr = Sys.stderr;
            NewStdout = Io.StringIO();
            NewStderr = Io.StringIO();
        }
        private void ClearReceiveMsg()
        {
            NewStdout.truncate(0);
            NewStdout.seek(0);
            NewStderr.truncate(0);
            NewStderr.seek(0);
        }
        private void RedirectReceiveMsg()
        {
            ClearReceiveMsg();

            Sys.stdout = NewStdout;
            Sys.stderr = NewStderr;
        }
        private string GetReceiveMsg()
        {
            string msg = NewStdout.getvalue().ToString();
            msg = msg + ";" + NewStderr.getvalue().ToString();

            //msg format:Stdout:Stderr
            return msg;
        }
        private void ResetReceiveMsg()
        {
            Sys.stdout = OldStdout;
            Sys.stderr = OldStderr;
        }
        #endregion

        public int Connect()
        {
            if (!PythonEngine.IsInitialized)
                return (int)ERROR_CODE.PYTHON_NOT_INITIALIZED;

            using (Py.GIL())
            {
                try
                {
                    InitialReceiveMsg();
                    RedirectReceiveMsg();

                    dynamic mlo_interfacce = Py.Import("Quanta_Interface");
                    //string sConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    //                                            "Config","MLO_Camera_Config.yaml");

                    //MLO_Camera = mlo_interfacce.MLO_Camera(sConfigPath);

                    string res = GetReceiveMsg();   //這裡可拿到 Python print

                    if (!res.Contains("MLColorimeter_py version:"))
                        return (int)ERROR_CODE.CAMERA_NOT_CONNECTED;

                    IsConnect = true;
                    return (int)ERROR_CODE.SUCCESS;
                }
                catch (Exception)
                {
                    //string res = GetReceiveMsg(); // 失敗時也可看 Python 訊息
                    IsConnect = false;
                    return (int)ERROR_CODE.UNKNOWN_ERROR;
                }
                finally
                {
                    ResetReceiveMsg();
                }
            }
        }

        public CCD_TYPE GetCameraType()
        {
            return CCD_TYPE.MLO;
        }

        public int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height, ref PixelFormat pixelFormat)
        {
            dynamic imageData = MLO_Camera.Capture_RawImageOnly("G", 0, 1, 0);

            // 1. 從 Dataclass 中取出 numpy 陣列與其他參數
            dynamic imgArray = imageData.image;
            double maxVal = imageData.max_val;
            double exposureTime = imageData.exposuretime;

            // 2. 獲取 Numpy 陣列的形狀 (Shape) 與 資料型態 (dtype)
            int height = imgArray.shape[0];
            int width = imgArray.shape[1];
            string dtype = imgArray.dtype.name; // 例如 "uint8" 或 "uint16"

            // 獲取記憶體指標位址 (IntPtr)
            long ptrAddress = imgArray.ctypes.data;
            image = new IntPtr(ptrAddress);

            //// 4. 根據您的格式 (文件中有 MLMono8 或 MLMono12) 建立 C# 影像物件
            //if (dtype == "uint16") // 對應 MLMono12
            //{
            //    // 使用 OpenCVSharp 的 Mat 直接封裝該記憶體指標 (零複製)
            //    using (Mat mat = new Mat(height, width, MatType.CV_16UC1, rawPtr))
            //    {
            //        // 在此進行 C# 的影像處理或缺陷檢測演算法
            //        // mat.DataPointer 就是該記憶體位址
            //    }
            //}
            //else if (dtype == "uint8") // 對應 MLMono8
            //{
            //    using (Mat mat = new Mat(height, width, MatType.CV_8UC1, rawPtr))
            //    {
            //        // 處理 8-bit 影像
            //    }
            //}

            return (int)ERROR_CODE.SUCCESS;
        }

        public int SoftwareTrigger(string id)
        {
            throw new NotImplementedException();
        }

        public int StartGrabbing(string id)
        {
            throw new NotImplementedException();
        }

        public int StopGrabbing(string id)
        {
            throw new NotImplementedException();
        }

        public void SetVirtualImagePath(string path)
        {
            throw new NotImplementedException();
        }
    }
}
