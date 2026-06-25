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
            Runtime.PythonDLL = AppDomain.CurrentDomain.BaseDirectory + @"Python312.dll";
            PythonEngine.Initialize();

            // 3. (選填) 如果你有自訂的 Python 模組(如 Quanta_Interface)，要把它的資料夾路徑加到 sys.path
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                // 假設你的 Python 腳本放在執行檔目錄下的 PythonScripts 資料夾
                string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts");
                sys.path.append(scriptPath);
            }
        }

        #region private function
        private bool IsConnect = false;
        private dynamic MLO_Py;         //取得的Python物件
        private dynamic Sys;
        private dynamic Io;
        private dynamic OldStdout;
        private dynamic OldStderr;
        private dynamic NewStdout;
        private dynamic NewStderr;
        #endregion

        #region private function
        private void InitialReceiveMsg()
        {
            Sys = Py.Import("sys");
            Io = Py.Import("io");

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
            using (Py.GIL())
            {
                try
                {
                    InitialReceiveMsg();
                    RedirectReceiveMsg();

                    dynamic mlo_interfacce = Py.Import("Quanta_Interface");
                    string sConfigPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Config",
                        "MLO_Camera_Config.yaml");

                    MLO_Py = mlo_interfacce.MLO_Camera(sConfigPath);

                    string res = GetReceiveMsg(); // 這裡可拿到 Python print
                    IsConnect = true;
                    return 0;
                }
                catch (Exception)
                {
                    //string res = GetReceiveMsg(); // 失敗時也可看 Python 訊息
                    IsConnect = false;
                    return -1;
                }
                finally
                {
                    ResetReceiveMsg();
                }
            }
        }

        public CCD_TYPE GetCameraType()
        {
            throw new NotImplementedException();
        }

        public int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height, ref PixelFormat pixelFormat)
        {
            throw new NotImplementedException();
        }

        public void SetVirtualImagePath(string path)
        {
            throw new NotImplementedException();
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
    }
}
