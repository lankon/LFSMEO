using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using System.Threading;
using System.Threading.Tasks;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class Task_MotionInitialize_DETester : IBaseTask<Task_MotionInitialize_DETester.WORK>
    {
        public Task_MotionInitialize_DETester(IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.START;

            switch (set_state)
            {
                default:
                    State = WORK.START;
                    break;
            }
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;
        }

        #region parameter
        #region FitTech migration settings
        // FitTech source: GControls.Tag[axis, EAxisParam.AxisEnabled].Ivalue == 1
        private bool NestXEnabled = true;
        private bool NestYEnabled = true;
        private bool NestZEnabled = true;
        private bool NestTXEnabled = true;
        private bool NestTYEnabled = true;
        private bool NestAEnabled = true;

        // FitTech source: DIOL.input(EIO.SideCCDLightInSensor)
        private EIOName SideCCDLightInSensor = EIOName.SideCCDLightInSensor;
        private int SideCCDLightTimeoutSec = 5;
        #endregion

        #region axis mapping
        private int NestX = 0;
        private int NestY = 0;
        private int NestZ = 0;
        private int NestTX = 0;
        private int NestTY = 0;
        private int NestA = 0;
        #endregion

        private long wait_timer = 0;
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            START,

            CHECK_POWER,

            CHECK_MOTIONPOWER,
            MOTIONPOWER_WAIT,

            CHECK_LIMIT,
            CHECK_SAFETY,

            GO_HOME_NEST_Y,
            WAIT_GO_HOME_NEST_Y,

            GO_HOME_NEST_Z_TX_TY_TZ,
            WAIT_GO_HOME_NEST_Z_TX_TY_TZ,

            GO_HOME_NEST_X,
            WAIT_GO_HOME_NEST_X,

            INIT_DONE,
            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        private void Preset()
        {
            ProbeTesterFunc = Deps.ServiceProvider.GetRequiredService<ProbeTesterFunction>();
            Axis = ProbeTesterFunc.Axis_HardwareParam;

            SetDefaultAxis();
        }

        private void SetDefaultAxis()
        {
            NestX = Axis.AxisX;
            NestY = Axis.AxisY;
            NestZ = Axis.AxisZ;
            NestTX = Axis.AxisRY;
            NestTY = Axis.AxisRZ;
            NestA = Axis.AxisRX;
        }


        private void MultiAxisGoHome(int[] axisArray, WORK next_state)
        {
            for (int i = 0; i < axisArray.Length; i++)
            {
                Deps.DML.GoHome(axisArray[i]);
                Thread.Sleep(50);
            }
                
            Transition(next_state);
        }

        private void WaitMultiAxisGoHome(int[] axis, WORK next_state)
        {
            bool finish = true;
            
            for (int i = 0; i < axis.Length; i++)
            {
                if (!Deps.DML.Get_Home_Complete(axis[i]))
                {
                    finish = false;
                }
            }

            if(finish)
                Transition(next_state);
        }

        private bool CheckAxisLimit()
        {
            var axesToCheck = new (int axis, string name)[]
            {
                (NestX, "Nest X"),
                (NestY, "Nest Y"),
                (NestZ, "Nest Z"),
                (NestTX, "Nest TX"),
                (NestTY, "Nest TY"),
                (NestA, "Nest A"),
            };

            string ss = "";
            bool anyAtLimit = false;

            foreach (var (axis, name) in axesToCheck)
            {
                if (false)
                {
                    ss += " " + name + " ";

                    anyAtLimit = true;
                }
            }

            if (anyAtLimit)
            {
                Tool.SaveLogToFile($"{ss} Axis on limit, remove to continue.", level: "ERR");
                return false;
            }

            return true;
        }

        private void SetIOStatus()
        {
            Deps.DIOL.SetOutputStatus(EIOName.SideCCDLightStretch, false);
            Deps.DIOL.SetOutputStatus(EIOName.SideCCDLightReStretch, true);
        }

        protected override void Transition(WORK target)
        {
            if (target != State)
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
                default: target_work = WORK.ABORT; break;
            }

            return target_work;
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
            SetPauseState(State);

            switch (State)
            {
                default:
                    {
                        F_StateControl.SetPauseAbortContinue(TASK_STATUS.ABORT_CONTINUE);
                    }
                    break;
            }
        }
        #endregion

        protected override void RunLoop(TASK_STATUS task_command)
        {
            if (task_command == TASK_STATUS.ABORT)
                GoToCaseAbortState(GetPauseState());
            else if (task_command == TASK_STATUS.CONTINUE)
                GoToCaseConitinueState();

            switch (State)
            {
                #region START
                case WORK.START:
                    {
                        Preset();

                        SetIOStatus();
                        Tool.ResetTimeCount(out wait_timer);

                        Transition(WORK.CHECK_POWER);
                    }
                    break;
                #endregion

                #region CHECK
                case WORK.CHECK_POWER:
                    {
                        Transition(WORK.CHECK_MOTIONPOWER);
                    }
                    break;

                case WORK.CHECK_MOTIONPOWER:
                    {
                        Transition(WORK.CHECK_LIMIT);
                    }
                    break;

                case WORK.CHECK_LIMIT:
                    {
                        bool res = CheckAxisLimit();

                        if (res == true)
                            Transition(WORK.CHECK_SAFETY);
                    }
                    break;

                case WORK.CHECK_SAFETY:
                    {
                        bool IsSafe = true;

                        if (!IsSafe)
                        {
                            Transition(WORK.FAIL);

                            break;
                        }

                        Transition(WORK.GO_HOME_NEST_X);
                    }
                    break;
                #endregion

                #region GO_HOME_NEST_X
                case WORK.GO_HOME_NEST_X:
                    {
                        if (Deps.DIOL.GetInputStatus(SideCCDLightInSensor))
                        {
                            int[] axis = { NestX };

                            MultiAxisGoHome(axis, WORK.WAIT_GO_HOME_NEST_X);
                        }
                        else if (!Deps.DIOL.GetInputStatus(SideCCDLightInSensor) && Tool.GetTime(wait_timer, "s") > SideCCDLightTimeoutSec)
                        {
                            Tool.SaveLogToFile("Cylinder is not retracted.", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;
                case WORK.WAIT_GO_HOME_NEST_X:
                    {
                        int[] axis = { NestX };

                        bool[] axisEnabled = { NestXEnabled };

                        WaitMultiAxisGoHome(axis, WORK.GO_HOME_NEST_Z_TX_TY_TZ);
                    }
                    break;
                #endregion

                #region GO_HOME_NEST_Z_TX_TY_TZ
                case WORK.GO_HOME_NEST_Z_TX_TY_TZ:
                    {
                        int[] axis = { NestZ, NestTX, NestTY, NestA };

                        bool[] axisEnabled = { NestZEnabled, NestTXEnabled, NestTYEnabled, NestAEnabled };

                        MultiAxisGoHome(axis, WORK.WAIT_GO_HOME_NEST_Z_TX_TY_TZ);
                    }
                    break;
                case WORK.WAIT_GO_HOME_NEST_Z_TX_TY_TZ:
                    {
                        int[] axis = { NestZ, NestTX, NestTY, NestA };

                        bool[] axisEnabled = { NestZEnabled, NestTXEnabled, NestTYEnabled, NestAEnabled };

                        WaitMultiAxisGoHome(axis, WORK.GO_HOME_NEST_Y);
                    }
                    break;
                #endregion

                #region GO_HOME_NEST_Y
                case WORK.GO_HOME_NEST_Y:
                    {
                        int[] axis = { NestY };

                        bool[] axisEnabled = { NestYEnabled };

                        MultiAxisGoHome(axis, WORK.WAIT_GO_HOME_NEST_Y);
                    }
                    break;
                case WORK.WAIT_GO_HOME_NEST_Y:
                    {
                        int[] axis = { NestY };

                        bool[] axisEnabled = { NestYEnabled };

                        WaitMultiAxisGoHome(axis, WORK.INIT_DONE);
                    }
                    break;
                #endregion

                #region INIT_DONE
                case WORK.INIT_DONE:
                    {
                        Transition(WORK.SUCCESS);
                    }
                    break;
                #endregion

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
    #endregion
}
