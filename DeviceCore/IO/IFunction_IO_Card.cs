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
        Virtual,
        PCI_9111DG,
        MN200,
        AMP_204C,
        P32C32,
        PCI_9111HR,
    }

    public enum EIOName
    {
        #region Input

        #region RGBTester DAQ Point
        Left_Vin,
        Left_Iin_HCM,
        Left_Iin_LCM,
        Left_VLED,
        Left_VLED_R,
        Left_VLED_G,
        Left_VLED_B,
        Left_ILED,

        Right_Vin,
        Right_Iin_HCM,
        Right_Iin_LCM,
        Right_VLED,
        Right_VLED_R,
        Right_VLED_G,
        Right_VLED_B,
        Right_ILED,
        #endregion

        SphereUpSensor,
        SphereDownSensor,
        SphereLeftSensor,
        SphereRightSensor,
        ChuckUpSensor,
        ChuckDownSensor,
        ChuckLeftSensor,
        ChuckRightSensor,

        SafePos_Sensor_In,
        SafePos_Sensor_Out,

        CCD_FiducialMaskWork_Sensor,
        CCD_FiducialMaskIdle_Sensor,
        SafePos_Sensor,
        GoToSafePos,
        #endregion

        #region Output
        SphereUp,
        SphereDown,
        SphereLeft,
        SphereRight,
        ChuckUp,
        ChuckDown,
        ChuckLeft,
        ChuckRight,
        

        Vacuum_Pump,
        CCD_FiducialMaskWork,
        CCD_FiducialMaskIdle,
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
        public string Title_Range { get; set; }
    }

    public interface IFunction_IO_Card
    {
        bool InitialDone { get; }

        void Set_IO_Form(IF_IO_Card f_io);
        bool Initial_All_IO();
        void LoadConfiguration(List<IOData> newIoDataList);
        double GetAInputStatus(EIOCardType CardType, byte cardNo, byte lineNo, byte devNo, byte port,string range, int iList);
        double GetAInputStatus(EIOName name);
        bool GetInputStatus(EIOCardType CardType, byte cardNo, byte lineNo, byte devNo, byte port, int iList);
        bool GetInputStatus(EIOName name);
        bool GetOutputStatus(EIOCardType CardType, byte cardNo, byte lineNo, byte devNo, byte port, int iList);
        bool SetOutputStatus(EIOCardType CardType, byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false);
        bool SetOutputStatus(EIOName name, bool truefalse);
        
        //[Virtual IO Function]
        int Add_AI_VirtualData(EIOName name, double truefalse);
        int Clear_AI_VirtualData();
        void AddIORule(int outputCardNo, int outputLineNo, int outputDevNo, int outputPort, bool outputValue,
                              params (int inputCardNo, int inputLineNo, int inputDevNo, int inputPort, bool inputValue)[] effects);
        int AddIORule(EIOName out_name, bool out_value, params (EIOName in_name, bool int_value)[] effects);
    }
}
