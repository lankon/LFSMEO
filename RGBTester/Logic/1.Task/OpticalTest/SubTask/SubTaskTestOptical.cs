using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public class SubTaskTestOptical : IBaseTask<SubTaskTestOptical.WORK>
    {
        public SubTaskTestOptical(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,  
                          string set_state = "Default") : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            switch (set_state)
            {
                default:
                    State = WORK.INITIAL;
                    break;
            }
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;
        }

        #region parameter
        private int delay_time = 1;
        private LuminousFlux LF = new LuminousFlux();
        private float[] fSpectrumRawData;
        private double[] SpectrumRawData;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        //private F_StateControl TaskForm;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            SET_LED_DAC,
            GET_SPECTRUM,
            CALCULATE_LUMEN,

            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        protected override void Transition(WORK target)
        {
            if (target != State) //狀態有變化時紀錄
            {
                Tool.SaveLogToFile($"[Task]({TaskName})" + target.ToString());
                F_StateControl.UpdateTask($"({TaskName})\n" + target.ToString());
            }

            State = target;
        }
        protected override WORK AbortState(WORK target)
        {
            WORK target_work = WORK.NONE;

            switch (target)
            {
                //case WORK.RUNNING_2: target_work = WORK.RUNNING_ABORT; break;
                default: target_work = WORK.ABORT; break;
            }

            return target_work;
        }
        /// <summary>
        /// 確認Task狀態
        /// </summary>
        /// <param name="check"></param>
        private void CheckResult(TASK_STATUS check, WORK SUCCESS = WORK.SUCCESS, 
                                                    WORK PAUSE = WORK.PAUSE,
                                                    WORK ABORT = WORK.ABORT,
                                                    WORK FAIL = WORK.FAIL)
        {
            switch (check)
            {
                case TASK_STATUS.SUCCESS:
                    {
                        Transition(SUCCESS);
                    }
                    break;
                case TASK_STATUS.PAUSE:
                    {
                        SetStatus(TASK_STATUS.PAUSE);   //設定目前Task狀態,讓MainTask知道Task狀態
                        SetPauseState(State);           //記錄目前暫停的case
                        SetNextState(State);            //設定使用者按繼續後要回到的case
                        Transition(PAUSE);
                    }
                    break;
                case TASK_STATUS.ABORT:
                    {
                        Transition(ABORT);
                    }
                    break;
                case TASK_STATUS.CONTINUE:
                    {

                    }
                    break;
                case TASK_STATUS.FAIL:
                    {
                        Transition(FAIL);
                    }
                    break;
            }
        }
        
        private void ResetTimeCount(out int tick)
        {
            tick = Environment.TickCount;
        }
        private bool CheckTimeOverMilSec(int tick, int time)
        {
            var time_count = Environment.TickCount - tick;
            bool res = time_count > time;

            return res;
        }
        #endregion

        #region public function
        public override TASK_STATUS Run(TASK_STATUS command = TASK_STATUS.NONE)
        {
            if (GetStatus() == TASK_STATUS.CONTINUE || command != TASK_STATUS.NONE)
            {
                RunLoop(command);
            }

            TASK_STATUS res = GetStatus();

            return res;
        }
        public override void GoToPause()
        {
            //執行暫停後可根據State選擇操作者使用狀態
            //ABORT_CONTINUE
            //ABORT
            //CONTINUE
            //讓使用者選擇
            SetPauseState(State);

            switch (State)
            {
                default:
                    {
                        F_StateControl.SetPauseAbortContinue(TASK_STATUS.ABORT_CONTINUE);
                    }
                    break;
            }

            if (GetSubTaskProcessing()) //判斷是否有SubTask執行
                SubTask.GoToPause();

        }
        #endregion

        protected override void RunLoop(TASK_STATUS task_command)
        {
            if (task_command == TASK_STATUS.ABORT)   //人員傳入ABORT命令
                GoToCaseAbortState(GetPauseState());
            else if(task_command == TASK_STATUS.CONTINUE)    //人員傳入CONTINUE命令
                GoToCaseConitinueState();
                
            switch (State)
            {
                case WORK.INITIAL:
                    {
                        Transition(WORK.SET_LED_DAC);
                    }
                    break;
                case WORK.SET_LED_DAC:
                    {
                        Deps.LightEngine.SetLed_AllColorDAC(Deps.LightEngine.LED_LeftSide, 50, 50, 50);
                        ResetTimeCount(out delay_time);
                        Transition(WORK.GET_SPECTRUM);
                    }
                    break;
                case WORK.GET_SPECTRUM:
                    {
                        if(CheckTimeOverMilSec(delay_time, 100))
                        {
                            fSpectrumRawData = Deps.Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, 100);

                            if(fSpectrumRawData == null)
                            {
                                Tool.SaveLogToFile($"Get Spectrum Failed", level: "ERR");
                                Transition(WORK.ABORT);
                                break;
                            }

                            SpectrumRawData = fSpectrumRawData.Select(x => (double)x).ToArray();

                            Transition(WORK.CALCULATE_LUMEN);
                        }
                    }
                    break;
                case WORK.CALCULATE_LUMEN:
                    {
                        if (CheckTimeOverMilSec(delay_time, 1))
                        {
                            float[] fwavelength = Deps.Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
                            double[] wavelength = fwavelength.Select(x => (double)x).ToArray();

                            double lm = LF.CalculateTotalLumens(wavelength, SpectrumRawData);
                            Transition(WORK.SUCCESS);
                        }
                    }
                    break;


                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level: "INF");
                    }
                    break;
                case WORK.FAIL:
                    {
                        SetStatus(TASK_STATUS.FAIL);
                    }
                    break;
                case WORK.PAUSE:
                    {
                        SetStatus(TASK_STATUS.PAUSE);
                    }
                    break;
                case WORK.ABORT:
                    {
                        SetStatus(TASK_STATUS.ABORT);
                    }
                    break;
                case WORK.CONTINUE:
                    {
                        if (GetNextState() != WORK.NONE)
                            GoToNextState();

                        SetStatus(TASK_STATUS.CONTINUE);
                        SetStatusCommand(TASK_STATUS.CONTINUE);
                    }
                    break;
                case WORK.END:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                    }
                    break;
            }
        }
    }
}
