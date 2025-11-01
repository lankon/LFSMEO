using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolFunction;
using ToolFunction.Base;

//using ToolFunction;


namespace DeviceUI.Motion
{
    public partial class F_AxisButton: Form, IF_MotionSetting
    {
        #region parameter define
        F_MotionSettingManage f_MotionSettingManage;
        List<Panel> PnlPartList = new List<Panel>();
        private int curPnlPart = 0;
        private int CurBtnNum = 0;
        #endregion

        #region private function
        private void SetHint()
        {
            //toolTip1.SetToolTip(Btn_OEM_Setting, "OEM Setting");
        }
        private void InitialForm()
        {
            SetPnlPartPos(Pnl_Part1);

            SetHint();

            if (ApplicationSetting.Get_Int_Recipe<eMotionSetting>((int)eMotionSetting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            PnlPartList.Add(Pnl_Part1);
            PnlPartList.Add(Pnl_Part2);

            CreateDynamicElement();
        }
        private int PreviousPnlPart(List<Panel> list, int index)
        {
            index = index - 1;

            if (index < 0)
                index = 0;

            SetPnlPartPos(list[index]);

            return index;
        }
        private int NextPnlPart(List<Panel> list, int index)
        {
            index = index + 1;

            if (index >= list.Count)
                index = 0;

            SetPnlPartPos(list[index]);

            return index;
        }
        private void SetPnlPartPos(Panel pnl)   //應該可以移到ToolFunction
        {
            pnl.Location = new Point(0, 0);
            pnl.BringToFront();
        }
        private void CreateDynamicElement()
        {
            Button[] AxisButton = new Button[14];
            string[] AxisButtonName = new string[14] { "Y", "Z", "A","AX", "AY", "AZ", "AA", "EX", "EY",
                                                       "EZ", "EA", "IX", "IY", "IZ"};

            for (int i=0; i<AxisButton.Length; i++)
            {
                AxisButton[i] = new Button();
                AxisButton[i].Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                AxisButton[i].Location = new System.Drawing.Point(116+84*i, 3);
                AxisButton[i].Name = $"Btn_Axis{i+1}";
                AxisButton[i].Size = new System.Drawing.Size(78, 82);
                AxisButton[i].TabIndex = 3;
                AxisButton[i].Tag = $"{i+1}";
                AxisButton[i].Text = AxisButtonName[i];
                AxisButton[i].UseVisualStyleBackColor = true;
                AxisButton[i].Click += new System.EventHandler(this.Btn_Axis0_Click);

                Pnl_Part1.Controls.Add(AxisButton[i]);
            }
            
        }
        #endregion

        #region public function
        public int GetCurrentBtnNum()
        {
            return CurBtnNum;
        }
        public void SetMediator(F_MotionSettingManage med)
        {
            f_MotionSettingManage = med;
        }
        #endregion

        public F_AxisButton()
        {
            InitializeComponent();


            InitialForm();
        }

        private void Btn_Axis0_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            CurBtnNum = Tool.StringToInt((string)btn.Tag);

            //f_MotionSettingManage.UpdateParameter();
        }

        private void Btn_PreviousPnlPart1_Click(object sender, EventArgs e)
        {
            curPnlPart = PreviousPnlPart(PnlPartList, curPnlPart);
        }

        private void Btn_NextPnlPart1_Click(object sender, EventArgs e)
        {
            curPnlPart = NextPnlPart(PnlPartList, curPnlPart);
        }
    }
}
