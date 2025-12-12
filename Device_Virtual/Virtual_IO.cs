using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_Virtual
{
    public class Virtual_IO : IIOCard
    {
        public Virtual_IO()
        {
            _Param.CardType = 1;
            _Param.Input_Status = new bool[lineMaxCount, devMaxCount, portMaxCount];
            _Param.Output_Status = new bool[lineMaxCount, devMaxCount, portMaxCount];
        }

        #region parameter define
        Queue<double>[] AI_Virtual;
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
        #endregion

        public bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            return false;
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
            AI_Virtual = new Queue<double>[portMaxCount];

            for (int i = 0; i < portMaxCount; i++)
                AI_Virtual[i] = new Queue<double>();
            
            return true;
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            _Param.Output_Status[lineNo, devNo, port] = truefalse;

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
            if (AI_Virtual[port] == null)
                return 5;
            
            if(AI_Virtual[port].Count <= 0)
                return 5;
            else
                return AI_Virtual[port].Dequeue();
        }

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



        private List<IORule> _rules = new List<IORule>();

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
