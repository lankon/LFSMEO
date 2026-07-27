using System;
using System.Threading;

using MicroEpsilon;

using DeviceCore;

namespace Device_MicroEpsilon
{
    public class Confocal_IFC2411 : IConfocal
    {
        public Confocal_IFC2411(String sensorName, String port)
        {
            SensorName = sensorName;
            Port = port;
            BaudRate = 921600;
            SensorIndex = 1;
        }

        #region parameter define
        //連線參數
        private string SensorName = "";
        private string Port = "";
        private int BaudRate = 921600;
        private int SensorIndex = 0;
        MEDAQLib sensor = null;
        #endregion

        #region private function
        private bool IsConnected()
        {
            if (sensor == null)
                return false;

            return true;
        }
        #endregion

        public int Connect()
        {
            sensor = new MEDAQLib(SensorName);

            ERR_CODE res = sensor.OpenSensorRS232(Port);

            if (res != ERR_CODE.ERR_NOERROR)
                return (int)res;

            return (int)ERR_CODE.ERR_NOERROR;
        }

        public void Disconnect()
        {
            sensor?.CloseSensor();

            sensor?.Release();

            sensor = null;
        }

        public int SetTriggerMode(EConfocalTriggerSource mode)
        {
            int trigger_source;
            switch (mode)
            {
                default:
                case EConfocalTriggerSource.Continuous:
                    trigger_source = 0;
                    break;
                case EConfocalTriggerSource.Software:
                    trigger_source = 4;
                    break;
            }

            if (sensor == null)
                return (int)ERR_CODE.ERR_NOT_FOUND;

            ERR_CODE res;

            res = sensor.SetIntExecSCmd("Clear_Buffers", "SP_AllDevices", SensorIndex);

            // 設定觸發後測試數量
            res = sensor.SetParameterInt("SP_TriggerCount", 50);
            res = sensor.ExecSCmd("Set_TriggerCount");

            res = sensor.SetParameterInt("SP_TriggerSource", trigger_source);
            res = sensor.ExecSCmd("Set_TriggerSource");

            if (res != ERR_CODE.ERR_NOERROR)
                return (int)res;

            return (int)ERR_CODE.ERR_NOERROR;
        }

        public void Close()
        {
            sensor?.CloseSensor();

            sensor?.Release();

            sensor = null;
        }

        public int SoftwareTrigger()
        {
            if (!IsConnected())
                return (int)ERR_CODE.ERR_NOT_FOUND;

            ERR_CODE res = sensor.ExecSCmd("Software_Trigger");

            if (res != ERR_CODE.ERR_NOERROR)
                return (int)res;

            return (int)ERR_CODE.ERR_NOERROR;
        }

        public int GetValue(ref double Value)
        {
            if (!IsConnected())
                return (int)ERR_CODE.ERR_NOT_FOUND;

            ERR_CODE res;

            // 取得資料組成數量
            int valuesPerFrame = 0;
            res = sensor.ExecSCmd("Get_TransmittedDataInfo");
            res = sensor.GetParameterInt("IA_ValuesPerFrame", ref valuesPerFrame);
            if (res != ERR_CODE.ERR_NOERROR)
                return (int)res;

            // 取得可用資料數量
            int available_data_count = 0;
            res = sensor.DataAvail(ref available_data_count);
            if (res != ERR_CODE.ERR_NOERROR || available_data_count <= 0)
                return (int)res;

            // 取得資料
            int[] raw_data = new int[available_data_count];
            double[] scaled_data = new double[available_data_count];
            int read = 0;
            res = sensor.TransferData(raw_data, scaled_data, available_data_count, ref read);

            if (res != ERR_CODE.ERR_NOERROR)
                return (int)ERR_CODE.ERR_NO_SENSORDATA_AVAILABLE;

            try
            {
                // read:實際讀取到的資料數量
                if (read <= 0)
                    return (int)ERR_CODE.ERR_NO_SENSORDATA_AVAILABLE;

                if (valuesPerFrame == 3)
                {
                    double sum = 0;
                    int count = 0;

                    for (int i = 2; i < read; i += valuesPerFrame)
                    {
                        sum += scaled_data[i];
                        count++;
                    }

                    if (count <= 0)
                        return (int)ERR_CODE.ERR_NO_SENSORDATA_AVAILABLE;

                    Value = sum / count;
                }
                else
                {
                    Value = scaled_data[read - 1];
                }
            }
            catch
            {
                return (int)ERR_CODE.ERR_SENSOR_ANSWER_WARNING;
            }

            return (int)ERR_CODE.ERR_NOERROR;
        }
    }
}
