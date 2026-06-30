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
            try
            {
                if (PythonEngine.IsInitialized)
                    return;

                string pythonHome = @"C:\Program Files\Python312";
                string pythonDll = System.IO.Path.Combine(pythonHome, "python312.dll");

                if (!System.IO.Directory.Exists(pythonHome))
                {
                    Console.WriteLine($"Python home not found: {pythonHome}");
                    return;
                }

                if (!System.IO.File.Exists(pythonDll))
                {
                    Console.WriteLine($"Python DLL not found: {pythonDll}");
                    return;
                }

                Runtime.PythonDLL = pythonDll;
                Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome, EnvironmentVariableTarget.Process);

                PythonEngine.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Python initialization error: {ex.Message}");
            }
        }

        #region private function
        private bool IsConnect = false;     //相機連線狀態
        private dynamic MLO_Camera;         //取得的Python相機物件
        private dynamic Sys;                //Python sys module，用於設定stdout/stderr與module搜尋路徑
        private dynamic Io;                 //Python io module，用於建立StringIO接收Python輸出
        private dynamic OldStdout;          //Python原本的stdout，ResetReceiveMsg時還原使用
        private dynamic OldStderr;          //Python原本的stderr，ResetReceiveMsg時還原使用
        private dynamic NewStdout;          //暫存Python stdout輸出的StringIO物件
        private dynamic NewStderr;          //暫存Python stderr輸出的StringIO物件
        private dynamic LastImageData;      //保留最後一次取像回傳物件，避免Python物件被GC釋放
        private dynamic LastImgArray;       //保留最後一次取像的numpy array，確保IntPtr指向的記憶體仍有效
        private dynamic Np;                 //Python numpy module，用於影像陣列格式轉換

        enum ERROR_CODE
        {
            SUCCESS = 0,
            PYTHON_NOT_INITIALIZED,
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
            Sys = Py.Import("sys");
            Io = Py.Import("io");

            string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"PythonScripts");

            if (!Sys.path.__contains__(scriptPath))
                Sys.path.append(scriptPath);

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

                    //MLO_Camera = mlo_interface.MLO_Camera(sConfigPath);

                    string res = GetReceiveMsg();   //這裡可拿到 Python print

                    if (!res.Contains("MLColorimeter_py version:"))
                        return (int)ERROR_CODE.CAMERA_NOT_CONNECTED;

                    IsConnect = true;
                    return (int)ERROR_CODE.SUCCESS;
                }
                catch (Exception)
                {
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
            if (!PythonEngine.IsInitialized)
                return (int)ERROR_CODE.PYTHON_NOT_INITIALIZED;

            if (!IsConnect || MLO_Camera == null)
                return (int)ERROR_CODE.CAMERA_NOT_CONNECTED;

            try
            {
                using (Py.GIL())
                {
                    if (Np == null)
                        Np = Py.Import("numpy");


                    LastImageData = MLO_Camera.Capture_RawImageOnly("G", 0, 1, 0);
                    LastImgArray = LastImageData.image;

                    // 確認numpy array是連續記憶體；不是的話轉成 contiguous copy
                    bool isContiguous = (bool)LastImgArray.flags.c_contiguous;
                    if (!isContiguous)
                        LastImgArray = Np.ascontiguousarray(LastImgArray);

                    int height = LastImgArray.shape[0];
                    int width = LastImgArray.shape[1];
                    string dtype = LastImgArray.dtype.name.ToString();

                    image_height = height;
                    image_width = width;

                    if (dtype == "uint8")
                        pixelFormat = PixelFormat.Format8bppIndexed;
                    else if (dtype == "uint16")
                        pixelFormat = PixelFormat.Format16bppGrayScale;
                    else
                        return (int)ERROR_CODE.INVALID_PARAMETER;

                    long ptrAddress = LastImgArray.ctypes.data;
                    image = new IntPtr(ptrAddress);

                    if (image == IntPtr.Zero)
                        return (int)ERROR_CODE.IMAGE_ACQUISITION_FAILED;

                    return (int)ERROR_CODE.SUCCESS;
                }
            }
            catch (Exception)
            {
                image = IntPtr.Zero;
                image_width = 0;
                image_height = 0;
                return (int)ERROR_CODE.IMAGE_ACQUISITION_FAILED;
            }
        }

        public int SoftwareTrigger(string id)
        {
            return (int)ERROR_CODE.SUCCESS;
        }

        public int StartGrabbing(string id)
        {
            return (int)ERROR_CODE.SUCCESS;
        }

        public int StopGrabbing(string id)
        {
            return (int)ERROR_CODE.SUCCESS;
        }

        public void SetVirtualImagePath(string path)
        {
            return;
        }
    }
}
