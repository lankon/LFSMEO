using DeviceCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Device_Virtual
{
    public class Virtual_IO : IIOCard, IIOCardVirtual
    {
        public Virtual_IO()
        {
            _Param.CardType = 1;
            _Param.Input_Status = new bool[lineMaxCount, devMaxCount, portMaxCount];
            _Param.Output_Status = new bool[lineMaxCount, devMaxCount, portMaxCount];
        }

        #region parameter define
        Queue<double>[] AI_Virtual;
        private List<IORule> IO_Rules = new List<IORule>();
        Device_Parameter _Param = new Device_Parameter();
        private int lineMaxCount = 5;
        private int devMaxCount = 2;
        private int portMaxCount = 16;

        struct Device_Parameter
        {
            public ushort CardType;
            public bool[,,] Output_Status;  //紀錄[LineNo,DevNo,Port]對應的Output訊號
            public bool[,,] Input_Status;   //紀錄[LineNo,DevNo,Port]對應的Input訊號
        }

        private class IORule
        {
            // 觸發條件 (Cause)
            public int OutputAddress { get; set; }
            public bool OutputValue { get; set; }

            // 結果 (Effect) 支援多個 Input
            public List<(int InputAddress, bool InputValue)> Effects { get; set; } = new List<(int, bool)>();
        }
        #endregion

        #region public function
        public bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            if (port < 0 || port >= portMaxCount)
                return false;
            if (DevNo < 0 || DevNo >= devMaxCount)
                return false;
            if (lineNo < 0 || lineNo >= lineMaxCount)
                return false;

            return _Param.Input_Status[lineNo, DevNo, port];
        }

        public string GetName()
        {
            return EIOCardType.Virtual.ToString();
        }

        public bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            bool res = _Param.Output_Status[lineNo, DevNo, port];

            return res;
        }

        public bool Open()
        {
            AI_Virtual = new Queue<double>[portMaxCount];

            for (int i = 0; i < portMaxCount; i++)
                AI_Virtual[i] = new Queue<double>();
            
            return true;
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            _Param.Output_Status[lineNo, devNo, port] = truefalse;

            // 將多維座標轉成唯一 OutputAddress（你可根據專案需求自訂算法）
            int outputAddress = GetAddress(lineNo, devNo, port);

            // 檢查規則並觸發對應 Input       
            foreach (var rule in IO_Rules)
            {
                if (rule.OutputAddress == outputAddress && rule.OutputValue == truefalse)
                {
                    foreach (var effect in rule.Effects)
                    {
                        // 反推 inputAddress 回 lineNo, devNo, port
                        var (inputLine, inputDev, inputPort) = ParseAddress(effect.InputAddress);
                        _Param.Input_Status[inputLine, inputDev, inputPort] = effect.InputValue;
                    }
                }
            }

            return true;
        }

        public void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {

        }

        public void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {

        }

        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, string range = "")
        {
            long targetTicks = Stopwatch.Frequency / 1000_000;//us
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks * 40) { }

            if (AI_Virtual[port] == null)
                return 5;
            
            if(AI_Virtual[port].Count <= 0)
                return 5;
            else
                return AI_Virtual[port].Dequeue();
        }

        //[Virtual IO Card Function]
        public int Add_AI_VirtualData(byte port, double value)
        {
            AI_Virtual[port].Enqueue(value);

            return 0;
        }

        public int Clear_AI_VirtualData()
        {
            for(int i=0; i<AI_Virtual.Length; i++)
            {
                AI_Virtual[i].Clear();
            }

            return 0;
        }

        public void AddIORule(int outputCardNo, int outputLineNo, int outputDevNo, int outputPort, bool outputValue,
                              params (int inputCardNo, int inputLineNo, int inputDevNo, int inputPort, bool inputValue)[] effects)
        {
            int outputAddress = GetAddress(outputLineNo, outputDevNo, outputPort);

            var effectList = effects
                .Select(e => (GetAddress(e.inputLineNo, e.inputDevNo, e.inputPort), e.inputValue))
                .ToList();

            IO_Rules.Add(new IORule
            {
                OutputAddress = outputAddress,
                OutputValue = outputValue,
                Effects = effectList
            });
        }
        #endregion

        #region private function
        private int GetAddress(int lineNo, int devNo, int port)
        {
            return lineNo * devMaxCount * portMaxCount + devNo * portMaxCount + port;
        }

        private (int lineNo, int devNo, int port) ParseAddress(int address)
        {
            int port = address % portMaxCount;
            int devNo = (address / portMaxCount) % devMaxCount;
            int lineNo = address / (devMaxCount * portMaxCount);
            return (lineNo, devNo, port);
        }
        #endregion
    }
}
