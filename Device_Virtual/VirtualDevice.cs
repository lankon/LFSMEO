using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_Virtual
{
    class VirtualDevice : IIOCard
    {
        #region parameter define
        Device_Parameter _Param = new Device_Parameter();

        struct Device_Parameter
        {
            public ushort CardType;
            public bool[,,] Output_Status;  //紀錄[LineNo,DevNo,Port]對應的Output訊號
            public bool[,,] Input_Status;   //紀錄[LineNo,DevNo,Port]對應的Input訊號
        }
        #endregion


        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }

        public bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Virtual";
        }

        public bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            return true;
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            _Param.Output_Status[lineNo, devNo, port] = truefalse;


            throw new NotImplementedException();
        }

        public void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            
        }

        public void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            
        }

        private readonly List<IORule> _rules = new List<IORule>();

        private void ApplyRules()
        {
            //foreach (var rule in _rules)
            //{
            //    // 檢查該規則的輸出位址是否已經被設定
            //    if (_simulatedOutputs.TryGetValue(rule.OutputAddress, out bool currentOutputValue))
            //    {
            //        // 檢查當前的輸出值是否符合規則的觸發條件
            //        if (currentOutputValue == rule.OutputValue)
            //        {
            //            // 觸發規則：更新模擬輸入的狀態
            //            _simulatedInputs[rule.InputAddress] = rule.InputValue;
            //            Console.WriteLine($"[RULE TRIGGERED] DO-{rule.OutputAddress} {rule.OutputValue} -> DI-{rule.InputAddress} set to {rule.InputValue}");
            //        }
            //        // 注意：如果輸出值不符合規則條件 (例如 DO-5 OFF 了)，我們不會在這裡主動重置 DI-3
            //        // 除非有另一條明確的規則處理 DO-5 == false 的情況。
            //    }
            //}
        }

        public class IORule
        {
            // 觸發條件 (Cause)
            public int OutputAddress { get; set; }
            public bool OutputValue { get; set; }

            // 結果 (Effect)
            public int InputAddress { get; set; }
            public bool InputValue { get; set; }
        }

    }
}
