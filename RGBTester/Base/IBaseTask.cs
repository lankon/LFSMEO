using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Windows.Forms;

using DeviceCore;

namespace RGBTester.Base
{
    public enum TASK_STATUS
    {
        NONE,

        SUCCESS,
        FAIL,

        PAUSE,
        ABORT,
        CONTINUE,
        RETRY,

        ABORT_CONTINUE,
    }
    public interface IBaseTaskDependence
    {
        IServiceProvider ServiceProvider { get; }
        IFunction_IO_Card DIOL { get; }
        IFunction_MotionCard DML { get; }
        ILightEngineCommand LightEngine { get; }
        IWriteFile File { get; }

    }
    public class BaseTaskDependence : IBaseTaskDependence
    {
        public IServiceProvider ServiceProvider { get; }
        public IFunction_IO_Card DIOL { get; }
        public IFunction_MotionCard DML { get; }
        public ILightEngineCommand LightEngine { get; }
        public IWriteFile File { get; }

        public BaseTaskDependence(IServiceProvider serviceProvider,
                                  IFunction_IO_Card io, IFunction_MotionCard motion, 
                                  ILightEngineCommand command, IWriteFile file)
        {
            DML = motion;
            DIOL = io;
            LightEngine = command;
            File = file;
            ServiceProvider = serviceProvider;
        }
    }
    public interface IF_BaseTask
    {
        TASK_STATUS Run(TASK_STATUS command = TASK_STATUS.NONE);
        void GoToPause();
        //void SetForm(Form form);
    }
    public abstract class IBaseTask<TWork>: IF_BaseTask where TWork: Enum
    {
        public IBaseTask(IBaseTaskDependence dependencies)
        {
            this.Deps = dependencies;
        }

        #region parameter define
        protected string TaskName = "NonTaskName";              //執行緒名稱
        protected TWork State;                                  //執行緒狀態
        protected readonly IBaseTaskDependence Deps;            //共用方法介面
        private TASK_STATUS status = TASK_STATUS.CONTINUE;      //設定目前狀態機狀態,傳出給MainTask知道
        private TASK_STATUS status_command = TASK_STATUS.NONE;  //設定要傳入SubTask的狀態命令
        private bool is_subtask_processing = false;             //判斷是否有SubTask
        private TWork next_state;                               //設定下一個WORK
        private TWork pause_state;                              //設定暫停的WORK
        #endregion

        #region public
        /// <summary>
        /// 執行Task並回傳Task狀態
        /// </summary>
        /// <param name="task_status"></param>
        /// <returns></returns>
        public virtual TASK_STATUS Run(TASK_STATUS command = TASK_STATUS.NONE)
        {
            if (GetStatus() == TASK_STATUS.CONTINUE || command != TASK_STATUS.NONE)
            {
                RunLoop(command);
            }

            TASK_STATUS res = GetStatus();

            return res;
        }
        /// <summary>
        /// 暫停Task
        /// </summary>
        /// <returns></returns>
        public abstract void GoToPause();
        ///// <summary>
        ///// 傳入需更新UI的Form
        ///// </summary>
        //public virtual void SetForm(Form form)
        //{

        //}
        #endregion

        #region protected
        // ABSTRACT
        /// <summary>
        /// 變換狀態
        /// </summary>
        protected abstract void Transition(TWork target);
        /// <summary>
        /// 設定放棄後執行狀態
        /// </summary>
        protected abstract TWork AbortState(TWork target);

        // VIRTUAL
        protected virtual void RunLoop(TASK_STATUS task_command)
        {

        }

        
        /// <summary>
        /// 設定Task狀態
        /// </summary>
        protected void SetStatus(TASK_STATUS st)
        {
            status = st;
        }
        /// <summary>
        /// 取得Task狀態
        /// </summary>
        protected TASK_STATUS GetStatus()
        {
            return status;
        }
        /// <summary>
        /// 設定狀態命令傳入SubTask
        /// </summary>
        /// <param name="task_status"></param>
        protected void SetStatusCommand(TASK_STATUS task_status)
        {
            status_command = task_status;
        }
        /// <summary>
        /// 取得傳入SubTask狀態命令
        /// </summary>
        /// <returns></returns>
        protected TASK_STATUS GetStatusCommand()
        {
            TASK_STATUS temp_status;
            temp_status = status_command;
            status_command = TASK_STATUS.NONE;   //清空命令

            return temp_status;
        }
        /// <summary>
        /// 設定是否有SubTask執行
        /// </summary>
        protected void SetSubTaskProcessing(bool is_processing)
        {
            is_subtask_processing = is_processing;
        }
        /// <summary>
        /// 取得是否有執行SubTask
        /// </summary>
        /// <returns></returns>
        protected bool GetSubTaskProcessing()
        {
            return is_subtask_processing;
        }
        /// <summary>
        /// 人員傳入Abort命令後執行動作
        /// </summary>
        protected void GoToCaseAbortState(TWork target)
        {
            if (GetSubTaskProcessing())  //SubTask
            {
                Transition(GetPauseState());
                SetStatusCommand(TASK_STATUS.ABORT);
            }
            else
            {
                Transition(AbortState(target));
            }

            SetStatus(TASK_STATUS.CONTINUE);
        }
        /// <summary>
        /// 人員傳入Conitinue命令後執行動作
        /// </summary>
        protected void GoToCaseConitinueState()
        {
            GoToNextState();
            SetStatus(TASK_STATUS.CONTINUE);
            SetStatusCommand(TASK_STATUS.CONTINUE);
        }


        protected void SetNextState(TWork target)
        {
            next_state = target;
        }
        protected TWork GetNextState()
        {
            return next_state;
        }
        protected void GoToNextState()
        {
            TWork next = GetNextState();

            // default(TWork) 對 enum 來說等於第一個成員 (通常是 NONE)
            if (!EqualityComparer<TWork>.Default.Equals(next, default(TWork)))
            {
                Transition(next);
            }
        }
        protected void SetPauseState(TWork work)
        {
            pause_state = work;
        }
        protected TWork GetPauseState()
        {
            return pause_state;
        }
        #endregion
    }
}
