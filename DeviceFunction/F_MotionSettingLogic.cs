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

                //[Speed Config]
                eF_AxisSetting.TxtBx_FastMaxVelocity,
                eF_AxisSetting.TxtBx_FastInitVelocity,
                eF_AxisSetting.TxtBx_Fast_ACC,
                eF_AxisSetting.TxtBx_Fast_DEC,
                eF_AxisSetting.TxtBx_FastSfac,

                //[Home Setting]
                eF_AxisSetting.Cmbx_HomeMode,
                eF_AxisSetting.Cmbx_HomeDirection,
                eF_AxisSetting.TxtBx_ORGPosition,
                eF_AxisSetting.TxtBx_ORGShiftPosition,
                eF_AxisSetting.TxtBx_HomeVelocity,
                eF_AxisSetting.TxtBx_ORGVelocity,
                eF_AxisSetting.TxtBx_HomeAcc,
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
            
            //[Speed Config]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_FastMaxVelocity, config[axis].FAST_MAX_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_FastInitVelocity, config[axis].FAST_INIT_SPEED.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Fast_ACC, config[axis].FAST_ACC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_Fast_DEC, config[axis].FAST_DEC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_FastSfac, config[axis].FAST_Sfac.ToString());

            //[Home Setting]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_HomeMode, config[axis].MODE.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_HomeDirection, config[axis].DIRECTION.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_ORGPosition, config[axis].HOME_POS.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_ORGShiftPosition, config[axis].HOME_SHIFT.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_HomeVelocity, config[axis].MAX_VELOCITY.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_ORGVelocity, config[axis].HOEM_FIND_ORG_VELOCITY.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_HomeAcc, config[axis].HOME_ACC.ToString());


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
            bool res = Function_MotionCard.PTP_Move(GetCurrentBtnNum(), 100.0, "Abs", MOVE_VELOCITY_MODE.FAST);
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
