using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_PCIS_DASK
{
    public class Pcis_dask : IIOCard
    {
        public Pcis_dask(string card_type)
        {
            if (card_type == "PCI_9111HR")
            {

            }
            else if (card_type == "PCI_9111DG")
            {
                pCI_Parm.CardType = DASK64.PCI_9111DG;
                pCI_Parm.Input_Status = new bool[lineMaxCount, devMaxCount, portMaxCount];
            }
        }

        #region parameter define
        private ushort card;            //卡片handle
        private ushort[] dataBuffer = new ushort[512];
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

        #region public function
        public string GetName()
        {
            if (pCI_Parm.CardType == DASK64.PCI_9111DG)
                return "PCI_9111DG";
            else if (pCI_Parm.CardType == DASK64.PCI_9111HR)
                return "PCI_9111HR";
            else
                return "None";
        }

        public bool Open()
        {
            try
            {
                card = (ushort)DASK64.Register_Card(pCI_Parm.CardType, 0); // 假設 CardType
            }
            catch
            {
                return false;
            }

            if (card < 0 || card > 65530)
            {
                Console.WriteLine("Register_IO_Card failed.");
                return false;
            }

            return true;
        }

        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            if (card < 0 || card > 65530)
                return -1;

            ushort rawValue = 0;
            double voltage = 0;

            DASK64.AI_ReadChannel((ushort)card, port, DASK64.AD_B_5_V, out rawValue);
            DASK64.AI_VoltScale((ushort)card, DASK64.AD_B_5_V, (short)rawValue, out voltage);

            return voltage;
        }
        #endregion



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
