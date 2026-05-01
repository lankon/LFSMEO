using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using ToolFunction;
using RGBTester.Base;
using Microsoft.Extensions.DependencyInjection;

namespace RGBTester.Logic
{
    public class TaskRGBTest: IBaseTask<TaskRGBTest.WORK>
    {
        public TaskRGBTest(IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            if (set_state == "Left")
                OnlyLeftTest = true;
            else if (set_state == "Right")
                OnlyRightTest = true;

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
        private string SN;                              //測試樣品SN
        private IF_BaseTask SubTask;                    //子流程
        private IF_StateControl F_StateControl;
        private ePartTestItem part_test_mode;
        private RGBTesterFunction RGBfunc;
        private bool OnlyLeftTest = false;
        private bool OnlyRightTest = false;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            MOVE_TO_TEST_POS,
            WAIT_MOVE_TO_TEST_POS,

            LEFT_GLASSES_TEST,
            RIGHT_GLASSES_TEST,

            WAIT_LEFT_GLASSES_TEST,
            WAIT_RIGHT_GLASSES_TEST,

            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        private void CreateTestFile()
        {
            part_test_mode = (ePartTestItem)ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_PartTest);

            string SN = RGBfunc.SerialNumber;
            Tool.SaveLogToFile("測試樣品SN:" + SN);

            Deps.File.SetModuleAndCustomer(RGBfunc.GetModuleType());

            if (!OnlyRightTest)
            {
                if (Scope.TaskRGBTest.IsSingleTest == false || 
                   (Scope.TaskRGBTest.IsSingleTest == true && part_test_mode != ePartTestItem.BurinIn))
                {
                    Deps.File.CreateFile("Left_R");
                    Deps.File.CreateFile("Left_G");
                    Deps.File.CreateFile("Left_B");
                    if (RGBfunc.GetModuleType() == eModuleType.Function_Test)
                        Deps.File.CreateFile("Left_B2");

                    Deps.File.CreateFile("Left_Calibration");
                }

                if (Scope.TaskRGBTest.IsSingleTest == false ||
                   (Scope.TaskRGBTest.IsSingleTest == true && part_test_mode == ePartTestItem.BurinIn))
                {
                    Deps.File.CreateFile("Left_BurnIn");
                }
            }

            if(!OnlyLeftTest)
            {
                if (Scope.TaskRGBTest.IsSingleTest == false ||
                   (Scope.TaskRGBTest.IsSingleTest == true && part_test_mode != ePartTestItem.BurinIn))
                {
                    Deps.File.CreateFile("Right_R");
                    Deps.File.CreateFile("Right_G");
                    Deps.File.CreateFile("Right_B");

                    if(RGBfunc.GetModuleType() == eModuleType.Function_Test)
                        Deps.File.CreateFile("Right_B2");

                    Deps.File.CreateFile("Right_Calibration");
                }

                if (Scope.TaskRGBTest.IsSingleTest == false ||
                   (Scope.TaskRGBTest.IsSingleTest == true && part_test_mode == ePartTestItem.BurinIn))
                {
                    Deps.File.CreateFile("Right_BurnIn");
                }
            }
        }
        
        private readonly string[] TestKeys = { "R", "G", "B", "Calibration", "BurnIn", "B2" };
        private void ExecuteFileAction(Action<string> fileAction)
        {
            // 處理 Left
            if (!OnlyRightTest)
            {
                foreach (var key in TestKeys) fileAction($"Left_{key}");
            }

            // 處理 Right
            if (!OnlyLeftTest)
            {
                foreach (var key in TestKeys) fileAction($"Right_{key}");
            }
        }
        private void CloseTestFile() => ExecuteFileAction(k => Deps.File.CloseFile(k));
        private void CopyAndCloseTestFile() => ExecuteFileAction(k => Deps.File.CopyAndCloseTestFile(k));
        private void CloseAndDeleteTestFile() => ExecuteFileAction(k => Deps.File.CloseAndDeleteFile(k));


        private void Preset() 
        {
            RGBfunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();
            CreateTestFile();
        }
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
                        Preset();

                        if(RGBfunc.GetModuleType() == eModuleType.Function_Test)
                        {
                            Transition(WORK.MOVE_TO_TEST_POS);
                        }
                        else
                        {
                            if (OnlyRightTest)
                                Transition(WORK.RIGHT_GLASSES_TEST);
                            else
                                Transition(WORK.LEFT_GLASSES_TEST);
                        }
                    }
                    break;

                #region MOVE_TO_TEST_POS
                case WORK.MOVE_TO_TEST_POS:
                    {
                        Tool.SaveLogToFile("MOVE_TO_TEST_POS", level: "INF");
                        SubTask = new SubTaskMoveToElectrical(Deps, F_StateControl);
                        SetSubTaskProcessing(true);

                        Transition(WORK.WAIT_MOVE_TO_TEST_POS);
                    }
                    break;
                case WORK.WAIT_MOVE_TO_TEST_POS:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        if (check == TASK_STATUS.SUCCESS)
                        {
                            if (OnlyRightTest)
                                Transition(WORK.RIGHT_GLASSES_TEST);
                            else
                                Transition(WORK.LEFT_GLASSES_TEST);
                        }
                    }
                    break;
                #endregion

                #region Left
                case WORK.LEFT_GLASSES_TEST:
                    {
                        Tool.SaveLogToFile("LEFT_GLASSES_TEST", level: "INF");
                        SubTask = new SubTaskRGBTest(Deps, F_StateControl,"Left");
                        SetSubTaskProcessing(true);

                        Deps.File.ResetCalibrationData();

                        Transition(WORK.WAIT_LEFT_GLASSES_TEST);
                    }
                    break;
                case WORK.WAIT_LEFT_GLASSES_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        if (check == TASK_STATUS.SUCCESS && part_test_mode != ePartTestItem.BurinIn)
                            Deps.File.WriteCalibrationResult(RGBfunc.SerialNumber, "Left_Calibration");

                        if(OnlyLeftTest) 
                            CheckResult(check, SUCCESS: WORK.SUCCESS);
                        else
                            CheckResult(check, SUCCESS: WORK.RIGHT_GLASSES_TEST);
                    }
                    break;
                #endregion

                #region Right
                case WORK.RIGHT_GLASSES_TEST:
                    {
                        Tool.SaveLogToFile("RIGHT_GLASSES_TEST", level: "INF");
                        SubTask = new SubTaskRGBTest(Deps, F_StateControl,"Right");
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_RIGHT_GLASSES_TEST);
                    }
                    break; 
                case WORK.WAIT_RIGHT_GLASSES_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        if (check == TASK_STATUS.SUCCESS && part_test_mode != ePartTestItem.BurinIn)
                            Deps.File.WriteCalibrationResult(RGBfunc.SerialNumber, "Right_Calibration");

                        CheckResult(check, SUCCESS: WORK.SUCCESS);
                    }
                    break;
                #endregion

                case WORK.SUCCESS:
                    {
                        Scope.TaskRGBTest.IsSingleTest = false;
                        CopyAndCloseTestFile();
                        CloseTestFile();
                        //CloseTestFile();
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level:"INF");
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
                        Scope.TaskRGBTest.IsSingleTest = false;
                        CloseAndDeleteTestFile();
                        SetStatus(TASK_STATUS.ABORT);
                        //SaveHistoryCurrentState(WORK.ABORT);
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
