using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public enum EIOCardType
    {
        None = 0,
        PCI_9111,
        MN200,
        AMP_204C,
    }

    public enum EIOName
    {
        #region Input
        SafePos_Sensor,

        #endregion

        #region Output

        #endregion
    }

    public class IOData
    {
        public string Title_IO { get; set; }
        public string Title_Name { get; set; }
        public string Title_Description { get; set; }
        public string Title_CardType { get; set; }
        public int Title_CardNum { get; set; }
        public int Title_LineNum { get; set; }
        public int Title_DevNum { get; set; }
        public int Title_IO_Num { get; set; }
        public string Title_Status { get; set; }
        public string Title_Inverse { get; set; }
    }

    public interface IFunction_IO_Card
    {
        bool Initial_All_IO();
        void LoadConfiguration(List<IOData> newIoDataList);
        bool GetInputStatus(EIOCardType CardType, byte lineNo, byte devNo, byte port, int iList);
        bool GetInputStatus(EIOName name);
        bool GetOutputStatus(EIOCardType CardType, byte lineNo, byte devNo, byte port, int iList);
        bool SetOutputStatus(EIOCardType CardType, byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false);
        bool SetOutputStatus(EIOName name, bool truefalse);
    }
}
