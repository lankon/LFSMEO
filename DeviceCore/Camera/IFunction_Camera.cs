using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Drawing.Imaging;

namespace DeviceCore
{
    public enum CCD_TYPE
    {         
        //Basler,
        Virtual,
        Hikvision,
        //Dalsa,
    }
    public enum CCD_NAME
    {
        CCD_0,
        CCD_1,
        CCD_2,
        CCD_3,
        CCD_4,
        CCD_5,
    }
    public enum IMAGE_FORMAT
    {
        MONO8,
        RGB8,
    }

    public interface IFunction_Camera
    {
        #region [Event]
        /// <summary>
        /// 訂閱指定 CCD 的影像更新事件。
        /// </summary>
        /// <param name="ccd">要訂閱的 CCD 編號。</param>
        /// <param name="handler">當新影像準備完成時要呼叫的事件處理函式。</param>
        void Subscribe(int ccd, EventHandler<ImageReadyEventArgs> handler);
        #endregion

        #region [Initial]

        /// <summary>
        /// 初始化所有可用的相機裝置，並準備後續的相機綁定流程。
        /// 通常應在程式啟動時呼叫一次。
        /// </summary>
        /// <returns>初始化成功時回傳 0；失敗時回傳實作定義的錯誤碼。</returns>
        int Initial_All_Camera();

        /// <summary>
        /// 將設定檔中的 CCD 資訊對應到已連線的相機裝置清單。
        /// 應在載入相機設定並完成裝置初始化後呼叫。
        /// </summary>
        void BindingCamera();

        #endregion

        #region [Camera Grab]

        /// <summary>
        /// 啟動指定 CCD 的取像流程。
        /// </summary>
        /// <param name="ccd">要啟動取像的 CCD 編號。</param>
        /// <returns>取像啟動成功時回傳 true；否則回傳 false。</returns>
        bool StartGrab(int ccd);

        /// <summary>
        /// 停止指定 CCD 的取像流程。
        /// </summary>
        /// <param name="ccd">要停止取像的 CCD 編號。</param>
        /// <returns>取像停止成功時回傳 true；否則回傳 false。</returns>
        bool StopGrab(int ccd);
        #endregion

        #region [Camera Trigger && LIVE]

        /// <summary>
        /// 對指定 CCD 發送軟體觸發命令。
        /// </summary>
        /// <param name="ccd">要觸發的 CCD 編號。</param>
        /// <returns>觸發命令成功時回傳 true；否則回傳 false。</returns>
        bool SoftTrigger(int ccd);

        /// <summary>
        /// 啟動指定 CCD 的 Live 連續取像。
        /// Live 模式會持續觸發、取像，並送出影像更新事件。
        /// </summary>
        /// <param name="ccdIndex">要啟動 Live 的 CCD 編號。</param>
        /// <returns>Live 啟動成功或已經在執行時回傳 true；否則回傳 false。</returns>
        bool StartLive(int ccdIndex);

        /// <summary>
        /// 停止指定 CCD 的 Live 連續取像。
        /// </summary>
        /// <param name="ccdIndex">要停止 Live 的 CCD 編號。</param>
        /// <returns>Live 停止成功時回傳 true；否則回傳 false。</returns>
        bool StopLive(int ccdIndex);

        /// <summary>
        /// 暫停或恢復指定 CCD 的 Live 迴圈，不會完整停止取像流程。
        /// </summary>
        /// <param name="ccd">要暫停或恢復的 CCD 編號。</param>
        /// <param name="is_pause">true 表示暫停 Live 更新；false 表示恢復 Live 更新。</param>
        void PauseLive(int ccd, bool is_pause);

        #endregion

        #region [Get Image]

        /// <summary>
        /// 從指定 CCD 取得一張影像，並透過影像更新事件送出。
        /// 若為虛擬相機，<paramref name="image_path"/> 代表要載入的影像檔路徑。
        /// </summary>
        /// <param name="ccd">要取像的 CCD 編號。</param>
        /// <param name="image_path">虛擬影像路徑，或由實作定義的影像來源路徑。</param>
        /// <returns>影像取得並送出成功時回傳 true；否則回傳 false。</returns>
        bool GetImageDisplay(int ccd, string image_path);

        #endregion

        #region [Read&Save Axis Information]

        /// <summary>
        /// 將相機設定參數儲存到指定設定檔。
        /// </summary>
        /// <param name="filePath">設定檔路徑。</param>
        /// <param name="axisName">相機節點名稱，例如 Camera0 或 Camera1。</param>
        /// <param name="parameters">要儲存的參數鍵值集合。</param>
        void SaveCameraConfig(string filePath, string axisName, Dictionary<string, string> parameters);

        /// <summary>
        /// 從預設相機設定檔載入相機設定。
        /// </summary>
        /// <returns>設定載入成功時回傳 true；否則回傳 false。</returns>
        bool LoadCameraConfig(); 

        /// <summary>
        /// 取得目前已載入的相機設定清單。
        /// </summary>
        /// <returns>依 CCD 編號排列的唯讀相機設定清單。</returns>
        IReadOnlyList<CAMERA_INFO> GetCameraConfig();
        #endregion
    }
}
