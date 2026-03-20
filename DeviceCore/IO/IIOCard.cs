using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public struct IO_PARAMETER
    {
        public byte[,] DevNoType;       //紀錄[LineNo,DevNo]對應的Type
        public bool[,,] Input_Status;   //紀錄[LineNo,DevNo,Port]對應的Input訊號
        public bool[,,] Output_Status;  //紀錄[LineNo,DevNo,Port]對應的Output訊號
        public List<byte> IO_LineNo;    //紀錄IO Type Line No.
        public List<byte> IO_DevNo;     //紀錄IO Type Device No.
    }

    public interface IIOCard
    {
        bool Open();
        string GetName();
        IO_PARAMETER Get_IO_Info();
        void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0);
        bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port);
        void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0);
        bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port);
        bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false);
        double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, string range = "");
        //int Add_AI_VirtualData(byte port, double value);
        //int Clear_AI_VirtualData();
    }
}
