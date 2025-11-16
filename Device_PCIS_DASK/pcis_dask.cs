using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_PCIS_DASK
{
    public class Pcis_dask_param
    {
        //ADLink PCI Card Type
        public const ushort PCI_9111DG = DASK64.PCI_9111DG;

        //Channel Count
        public const ushort P9111_CHANNEL_DI = DASK64.P9111_CHANNEL_DI;
    }

    public class Pcis_dask : IIOCard
    {
        #region parameter define
        private ushort card;     //卡片handle
        private CallbackDelegate64 aiHalfReadyCallback;
        private CallbackDelegate64 aiEndCallback;
        private ushort[] dataBuffer = new ushort[512];
        private const uint ReadCount = 1024;
        private const uint HalfReadCount = 512;
        private int lineMaxCount = 5;
        private int devMaxCount = 2;
        private int portMaxCount = 16;
        PCI_DASK_Parameter pCI_Parm = new PCI_DASK_Parameter();

        struct PCI_DASK_Parameter
        {
            public ushort CardType;                    
            public bool[,,] Input_Status;   //紀錄[LineNo,DevNo,Port]對應的Input訊號
        }
        #endregion

        public Pcis_dask(ushort card_type)
        {
            pCI_Parm.CardType = card_type;

            if(card_type == DASK64.PCI_9111DG)
            {
                pCI_Parm.Input_Status = new bool[lineMaxCount, devMaxCount, portMaxCount];
            }
        }

        public string GetName()
        {
            if (pCI_Parm.CardType == DASK64.PCI_9111DG || 
                pCI_Parm.CardType == DASK64.PCI_9111HR)
                return "PCI_9111";
            else
                return "None";
        }

        //public  bool Open()
        //{
        //    //註冊卡片
        //    card = (ushort)DASK64.Register_Card(pCI_Parm.CardType, 0);

        //    //設定觸發方式
        //    DASK64.AI_9111_Config(card, DASK64.TRIG_INT_PACER, DASK64.P9111_TRGMOD_SOFT, 0);

        //    //啟用DoubleBuffer
        //    DASK64.AI_AsyncDblBufferMode(card, true);

        //    ushort[] buffer = null;
        //    //連續掃描
        //    //通道數:15
        //    //AD_Range:+-10V
        //    //Buffer:雙緩衝模式下沒有用途
        //    //ReadCount:9111需為1024的倍數
        //    //SampleRate:採樣頻率(Hz)
        //    //SyncMode:同步模式
        //    DASK64.AI_ContScanChannels(card, 15, DASK64.AD_B_10_V, buffer, 1024, 10000, DASK64.ASYNCH_OP);

        //    byte ready = 0;
        //    byte stop = 0;
        //    //判斷是否準備好
        //    DASK64.AI_AsyncDblBufferHalfReady(card, out ready, out stop);

        //    ushort[] return_buffer = null;
        //    //取得回傳資料
        //    DASK64.AI_AsyncDblBufferTransfer(card, return_buffer);

        //    DASK64.AI_AsyncClear(card );

        //    if (card < 0)
        //        return false;

        //    return true;
        //}

        public bool Open()
        {
            // 1. 註冊卡片
            try
            {
                card = (ushort)DASK64.Register_Card(DASK64.PCI_9111HR, 0); // 假設 CardType
            }
            catch
            {
                return false;
            }

            if (card < 0)
            {
                Console.WriteLine("Register_Card failed.");
                return false;
            }

            // 2. 設定觸發方式
            short res = DASK64.AI_9111_Config(card, DASK64.TRIG_INT_PACER, DASK64.P9111_TRGMOD_SOFT, 0);

            // 3. 啟用DoubleBuffer
            DASK64.AI_AsyncDblBufferMode(card, true);

            // 4. 建立並註冊 Callback 函式
            aiHalfReadyCallback = new CallbackDelegate64(OnHalfBufferReady);
            aiEndCallback = new CallbackDelegate64(OnAcquisitionEnd);

            // 告訴 Dask 當 "半緩衝區就緒" (DBEvent) 時呼叫 OnHalfBufferReady
            DASK64.AI_EventCallBack_x64(card, 1, (short)DASK64.DBEvent, aiHalfReadyCallback);

            // 告訴 Dask 當 "採集結束" (AIEnd) 時呼叫 OnAcquisitionEnd
            DASK64.AI_EventCallBack_x64(card, 1, (short)DASK64.AIEnd, aiEndCallback);


            // 5. 連續掃描
            // Buffer 參數在雙緩衝模式下無用，傳入 null
            short err = DASK64.AI_ContScanChannels(card, 7, DASK64.AD_B_5_V, null, ReadCount, 10000, DASK64.ASYNCH_OP);

            if (err != 0)
            {
                Console.WriteLine("AI_ContScanChannels failed.");
                Stop(); // 呼叫停止函式
                return false;
            }

            return true;
        }

        private void OnHalfBufferReady()
        {
            try
            {
                // 取得回傳資料
                short err = DASK64.AI_AsyncDblBufferTransfer(card, dataBuffer);
                if (err == 0)
                {
                    // *** 在這裡處理您的 dataBuffer (512 筆資料) ***
                    // 注意：若要更新 UI，必須將資料封送 (Marshal) 回 UI 執行緒
                    Console.WriteLine($"Callback: 讀取 {dataBuffer.Length} 筆資料。第一筆: {dataBuffer[0]}");
                }
                else
                {
                    Console.WriteLine("AI_AsyncDblBufferTransfer failed in callback.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Callback 發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// **[新增]** 採集結束時由 DASK 呼叫
        /// (參照 User's Manual 3.5 節範例 [cite: 932, 933, 934, 935, 936])
        /// </summary>
        private void OnAcquisitionEnd()
        {
            Console.WriteLine("採集已完成 (AIEnd event)。");
            // 這裡可以設置一個旗標(Flag)通知主程式
        }

        public void Stop()
        {
            if (card >= 0)
            {
                //呼叫停止 (並修正參數)
                //(參照 Function Reference AI_AsyncClear 頁面 [cite: 2482])
                uint accessCount;
                DASK64.AI_AsyncClear(card, out accessCount);

                // **[新增]** 移除事件回呼
                DASK64.AI_EventCallBack_x64(card, 0, (short)DASK64.DBEvent, null);
                DASK64.AI_EventCallBack_x64(card, 0, (short)DASK64.AIEnd, null);

                DASK64.Release_Card(card);
                Console.WriteLine("採集已停止並釋放卡片。");
            }
        }



        public void Get_AI_Signal()
        {

        }

        public void Read_AI_Signal()
        {

        }

        public  void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            //port:點位
            DASK64.DI_ReadLine(lineNo, DASK64.P9111_CHANNEL_DI, port, out ushort state);

            if (state == 1)
                pCI_Parm.Input_Status[lineNo, devNo, port] = true;
            else
                pCI_Parm.Input_Status[lineNo, devNo, port] = false;
        }
        public  bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            if (pCI_Parm.CardType == DASK64.PCI_9111DG)
            {
                if (port < 0 || port >= portMaxCount)
                    return false;
                if (DevNo < 0 || DevNo >= devMaxCount)
                    return false;
                if (lineNo < 0 || lineNo >= lineMaxCount)
                    return false;
            }

            return pCI_Parm.Input_Status[lineNo, DevNo, port];
        }

        public  bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            throw new NotImplementedException();
        }

        public  void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }

        public  bool GetOutputStatus(byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }
    }
}
