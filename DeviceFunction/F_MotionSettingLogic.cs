using DeviceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using ToolFunction;

namespace DeviceFunction
{
    public class F_MotionSettingLogic
    {
        public F_MotionSettingLogic(IFunction_MotionCard function_MotionCard)
        {
            Function_MotionCard = function_MotionCard;
        }

        #region parameter define
        public IF_MotionSetting MotionSetting;
        public IFunction_MotionCard Function_MotionCard;
        public IF_AxisButton AxisButton;
        public IF_AxisSetting AxisSetting;
        eF_AxisSetting[] AxisParam = new eF_AxisSetting[]
            {
                //新增軸參數時需添加
                
                //[Axis Config]
                eF_AxisSetting.Cmbx_AxisType,
                eF_AxisSetting.TxtBx_AxisStation,
                eF_AxisSetting.Cmbx_AxisUse,
                eF_AxisSetting.Cmbx_AxisLimitLogic,
                eF_AxisSetting.Cmbx_AxisLimitStopMode,
                eF_AxisSetting.TxtBx_DriverResolution,

                //[Hardware Configuration]
                eF_AxisSetting.TxtBx_AxisPitch,

                //[Software Configuration]
                eF_AxisSetting.TxtBx_AxisName,
                eF_AxisSetting.Cmbx_UseSoftLimit,
                eF_AxisSetting.TxtBx_SoftPEL,
                eF_AxisSetting.TxtBx_SoftMEL,

                //[Speed Config]
                eF_AxisSetting.TxtBx_FastMaxVelocity,
                eF_AxisSetting.TxtBx_FastInitVelocity,
                eF_AxisSetting.TxtBx_Fast_ACC,
                eF_AxisSetting.TxtBx_Fast_DEC,
                eF_AxisSetting.TxtBx_FastSfac,
                eF_AxisSetting.TxtBx_SlowMaxVelocity,
                eF_AxisSetting.TxtBx_SlowInitVelocity,
                eF_AxisSetting.TxtBx_Slow_ACC,
                eF_AxisSetting.TxtBx_Slow_DEC,
                eF_AxisSetting.TxtBx_SlowSfac,
                eF_AxisSetting.TxtBx_NormalMaxVelocity,
                eF_AxisSetting.TxtBx_NormalInitVelocity,
                eF_AxisSetting.TxtBx_Normal_ACC,
                eF_AxisSetting.TxtBx_Normal_DEC,
                eF_AxisSetting.TxtBx_NormalSfac,

                //[Home Setting]
                eF_AxisSetting.Cmbx_HomeDirection,
                eF_AxisSetting.TxtBx_ORGPosition,
                eF_AxisSetting.TxtBx_1stHomeVelocity,
                eF_AxisSetting.TxtBx_1stHomeAcc,
                eF_AxisSetting.TxtBx_1stHomeDec,
                eF_AxisSetting.TxtBx_1stORGOffset,
                eF_AxisSetting.TxtBx_2ndHomeVelocity,
                eF_AxisSetting.TxtBx_2ndHomeAcc,
                eF_AxisSetting.TxtBx_2ndHomeDec,
                eF_AxisSetting.TxtBx_2ndHomeOffsetVelocity,
                eF_AxisSetting.TxtBx_2ndORGOffset,
            };
        #endregion

        #region public function

        #region [Set Form Interface]
        //[Set Form Interface]
        public void SetAxisButtonIF(IF_AxisButton f_AxisButton)
        {
            AxisButton = f_AxisButton;
        }

        public void SetAxisSettingIF(IF_AxisSetting f_AxisSetting)
        {
            AxisSetting = f_AxisSetting;
        }
        #endregion

        #region [Save & Update Axis Info]
        //[Save & Update Axis Info]
        public void UpdateAxisInfo2Form(int axis)
        {
            //新增軸參數時需添加

            Function_MotionCard.LoadAxisConfig();
            var config = Function_MotionCard.GetAxisConfig();

            //[Axis Configuration]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisType, config[axis].AXIS_TYPE);
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_LineNo, config[axis].LINE_NO.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_AxisStation, config[axis].DEV_NO.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisUse, config[axis].AXIS_USE.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisLimitLogic, config[axis].LIMIT_LOGIC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisLimitStopMode, config[axis].STOP_MODE.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_DriverResolution, config[axis].DRIVER_RESOLUTION.ToString());

            //[Hardware Configuration]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_AxisPitch, config[axis].PITCH.ToString());

            //[Software Configuration]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_AxisName, config[axis].AXIS_NANE);
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_UseSoftLimit, config[axis].SW_LIMIT.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_SoftPEL, config[axis].PEL_POS.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_SoftMEL, config[axis].MEL_POS.ToString());

            //[Speed Config]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_FastMaxVelocity, config[axis].FAST_MAX_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_FastInitVelocity, config[axis].FAST_INIT_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Fast_ACC, config[axis].FAST_ACC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Fast_DEC, config[axis].FAST_DEC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_FastSfac, config[axis].FAST_Sfac.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_SlowMaxVelocity, config[axis].SLOW_MAX_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_SlowInitVelocity, config[axis].SLOW_INIT_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Slow_ACC, config[axis].SLOW_ACC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Slow_DEC, config[axis].SLOW_DEC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_SlowSfac, config[axis].SLOW_Sfac.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_NormalMaxVelocity, config[axis].NORMAL_MAX_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_NormalInitVelocity, config[axis].NORMAL_INIT_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Normal_ACC, config[axis].NORMAL_ACC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Normal_DEC, config[axis].NORMAL_DEC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_NormalSfac, config[axis].NORMAL_Sfac.ToString());

            //[Home Setting]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_HomeDirection, config[axis].DIRECTION.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_ORGPosition, config[axis].HOME_POS.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_1stHomeVelocity, config[axis].MAX_VELOCITY_1ST.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_1stHomeAcc, config[axis].HOME_ACC_1ST.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_1stHomeDec, config[axis].HOME_DEC_1ST.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_1stORGOffset, config[axis].HOME_OFFSET_1ST.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_2ndHomeVelocity, config[axis].MAX_VELOCITY_2ND.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_2ndHomeAcc, config[axis].HOME_ACC_2ND.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_2ndHomeDec, config[axis].HOME_DEC_2ND.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_2ndHomeOffsetVelocity, config[axis].HOME_OFFSET_VELOCITY_2ND.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_2ndORGOffset, config[axis].HOME_OFFSET_2ND.ToString());

            AxisSetting.UpdateParmeter();
        }
        public void SaveAxis()
        {
            AxisSetting.SaveAxisParameter();

            int num = GetCurrentBtnNum();
            string set;

            Dictionary<string, string> param = new Dictionary<string, string>();
            for (int i = 0; i < AxisParam.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eF_AxisSetting>((int)AxisParam[i]);
                param.Add(AxisParam[i].ToString(), set);
            }

            string axisName = $"Axis{num}";
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            Function_MotionCard.SaveAxisConfig(AppPath + @"\Setting\AxisConfig.xml", axisName, param);
        }
        public bool SaveAndLoadAxisConfig()
        {
            SaveAxis();
            bool res = Function_MotionCard.LoadAxisConfig();

            return res;
        }
        #endregion

        #region [Motion Function]
        //[Motion]
        public void GoHome()
        {
            SaveAxis();
            Function_MotionCard.LoadAxisConfig();

            Task.Run(async () =>
            { 
                await Function_MotionCard.GoHome(GetCurrentBtnNum());
            });
        }

        public bool PTP_MoveTest()
        {
            SaveAxis();
            Function_MotionCard.LoadAxisConfig();

            long CycleTime = 0;
            Tool.ResetTimeCount(out CycleTime);
            bool Terminate = false;

            bool res = Function_MotionCard.PTP_Move(GetCurrentBtnNum(), 50.0, "Abs", MOVE_VELOCITY_MODE.FAST);
            
            while(!Terminate)
            {
                if(Function_MotionCard.Get_Motion_Complete(GetCurrentBtnNum()))
                {
                    Terminate = true;
                }
            }
            
            Tool.SaveLogToFile($"PTP Move Time:{Tool.GetTime(CycleTime)}");
            
            return res;
        }
        public bool SingleMove(int dir)
        {
            bool res = Function_MotionCard.SingleMove(GetCurrentBtnNum(), dir, 50.0);
            return res;
        }
        #endregion

        public int GetCurrentBtnNum()
        {
            return AxisButton.GetCurrentBtnNum();
        }

        
        #endregion
    }
}
