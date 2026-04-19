using Device_Guishan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ToolFunction;

namespace BurnInTester.Logic
{
    public class Guishan_TC_Test
    {
        private Fittech_3Ch_Independent_RTD TC = new Fittech_3Ch_Independent_RTD();


        #region parameter define
        public event Action<string> UpdateTemperature;
        public event Action<string> UpdateErrorCount;
        private Queue<WORK> qState = new Queue<WORK>();
        private double SV = 0;
        private int Resp_Delay = 0;
        private int Send_Delay = 0;
        private int ErrorCount = 0;
        private WORK State = WORK.IDLE;
        enum WORK
        {
            IDLE,
            START,
            STOP,
            ASK_PV,
            GET_ANSWER,
        }
        #endregion

        #region private function

        private void Process()
        {
            int res = -1;
            string answer = "";

            while (true)
            {
                switch (State)
                {
                    case WORK.IDLE:
                        {
                            if (qState.Count > 0)
                            {
                                State = qState.Dequeue();
                                Tool.SaveLogToFile("State changed to: " + State);
                            }
                            else
                            {
                                State = WORK.ASK_PV;
                            }
                        }
                        break;
                    case WORK.START:
                        {
                            Thread.Sleep(Send_Delay);
                            res = TC.Start(SV);

                            if (res == 0)
                                State = WORK.GET_ANSWER;
                            else
                            {
                                Tool.SaveLogToFile("Start TC fail");
                                State = WORK.IDLE;
                            }
                        }
                        break;
                    case WORK.STOP:
                        {
                            Thread.Sleep(Send_Delay);
                            res = TC.Stop();
                            if (res == 0)
                                State = WORK.GET_ANSWER;
                            else
                            {
                                Tool.SaveLogToFile("Stop TC fail");
                                State = WORK.IDLE;
                            }
                        }
                        break;
                    case WORK.ASK_PV:
                        {
                            Thread.Sleep(Send_Delay);
                            res = TC.AskPV();
                            
                            if (res == 0)
                            {
                                Tool.SaveLogToFile("Ask PV success");
                                State = WORK.GET_ANSWER;
                            }
                            else
                            {
                                Tool.SaveLogToFile("Ask PV fail");
                                State = WORK.IDLE;
                            }
                        }
                        break;
                    case WORK.GET_ANSWER:
                        {
                            Thread.Sleep(Resp_Delay);
                            res = TC.GetAnswer(out answer);
                            if (res == 0)
                            {
                                Tool.SaveLogToFile("Get answer: " + answer);

                                if(!answer.Contains(','))
                                    UpdateTemperature?.Invoke(answer);

                                State = WORK.IDLE;
                            }
                            else
                            {
                                ErrorCount++;
                                UpdateErrorCount?.Invoke(ErrorCount.ToString());
                                Tool.SaveLogToFile("Get answer fail");
                                State = WORK.IDLE;
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        public bool Open()
        {
            int res = -1;
            res = TC.Open("COM1", "9600", "8", "1", "None");

            if(res != 0)
            {
                Tool.SaveLogToFile("Open TC fail");
                return false;
            }

            Thread.Sleep(500);
            res = TC.Initialize();

            if (res == 0)
            {
                Task task = Task.Run(() => Process());
                return true;
            }
            else
                return false;
        }

        public bool Start(double sv, int resp_delay, int send_delay)
        {
            ErrorCount = 0;
            UpdateErrorCount?.Invoke(ErrorCount.ToString());

            SV = sv;
            Resp_Delay = resp_delay;
            Send_Delay = send_delay;

            qState.Enqueue(WORK.START);
            return true;
        }

        public bool Stop()
        {
            qState.Enqueue(WORK.STOP);
            return true;
        }

    }
}
