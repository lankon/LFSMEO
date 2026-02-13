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
using DeviceCore;
using DeviceFunction;

namespace DeviceUI.Motion
{
    public partial class F_AxisButton: Form, IF_AxisButton
    {
        public F_AxisButton(F_MotionSettingLogic f_MotionSettingLogic)
        {
            InitializeComponent();
            InitialForm();

            f_MotionSettingLogic.SetAxisButtonIF(this);
            MotionSettingLogic = f_MotionSettingLogic;
        }

        #region parameter define
        List<Panel> PnlPartList = new List<Panel>();
        F_MotionSettingLogic MotionSettingLogic;
        Label[] Labl_PostionAxis = new Label[AxisCount];
        private int curPnlPart = 0;
        private int CurBtnNum = 0;
        private const int AxisCount = 14;     //軸數量
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

            //if (ApplicationSetting.Get_Int_Recipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);

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
            Button[] AxisButton = new Button[AxisCount];
            string[] AxisButtonName = new string[AxisCount] { "Y", "Z", "A","AX", "AY", "AZ", "AA", "EX", "EY",
                                                              "EZ", "EA", "IX", "IY", "IZ"};

            for (int i=0; i<AxisButton.Length; i++)
            {
                AxisButton[i] = new Button();
                AxisButton[i].Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                AxisButton[i].Location = new System.Drawing.Point(116 + 84 * i, 3);
                AxisButton[i].Name = $"Btn_Axis{i + 1}";
                AxisButton[i].Size = new System.Drawing.Size(78, 82);
                AxisButton[i].TabIndex = 3;
                AxisButton[i].Tag = $"{i + 1}";
                AxisButton[i].Text = AxisButtonName[i];
                AxisButton[i].UseVisualStyleBackColor = true;
                AxisButton[i].Click += new System.EventHandler(this.Btn_Axis0_Click);

                Pnl_Part1.Controls.Add(AxisButton[i]);
            }

            for(int i=0; i< Labl_PostionAxis.Length; i++)
            {
                Labl_PostionAxis[i] = new Label();
                Labl_PostionAxis[i].AutoSize = true;
                Labl_PostionAxis[i].ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
                Labl_PostionAxis[i].Location = new System.Drawing.Point(132+84*i, 64);
                Labl_PostionAxis[i].Name = $"Labl_PostionAxis{i+1}";
                Labl_PostionAxis[i].Size = new System.Drawing.Size(44, 12);
                Labl_PostionAxis[i].TabIndex = 4;
                Labl_PostionAxis[i].Text = "000.000";
                Labl_PostionAxis[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                Pnl_Part1.Controls.Add(Labl_PostionAxis[i]);
                Labl_PostionAxis[i].BringToFront();
            }

        }
        private void StartUpdatePosition(bool start)
        {
            if (start == true)
                Timer_UpdatePosition.Start();
            else
                Timer_UpdatePosition.Stop();
        }
        #endregion

        #region public function
        public int GetCurrentBtnNum()
        {
            return CurBtnNum;
        }
        
        public void StartUpdatePositionInvoke(bool start)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    StartUpdatePosition(start);
                }));
            }
            else
            {
                StartUpdatePosition(start);
            }
        }
        public void ShowFormName(bool show)
        {
            if (show)
                Tool.ShowFormName(this);
        }
        #endregion

        private void Btn_Axis0_Click(object sender, EventArgs e)
        {
            MotionSettingLogic.SaveAxis();

            Button btn = sender as Button;
            CurBtnNum = Tool.StringToInt((string)btn.Tag);

            MotionSettingLogic.UpdateAxisInfo2Form(CurBtnNum);
        }

        private void Btn_PreviousPnlPart1_Click(object sender, EventArgs e)
        {
            curPnlPart = PreviousPnlPart(PnlPartList, curPnlPart);
        }

        private void Btn_NextPnlPart1_Click(object sender, EventArgs e)
        {
            curPnlPart = NextPnlPart(PnlPartList, curPnlPart);
        }

        private void Timer_UpdatePosition_Tick(object sender, EventArgs e)
        {
            Labl_PostionAxis0.Text = MotionSettingLogic.Function_MotionCard.GetPosition(0).ToString("000.000");
            for (int i=0; i< AxisCount; i++)
            {
                Labl_PostionAxis[i].Text = MotionSettingLogic.Function_MotionCard.GetPosition(i + 1).ToString("000.000");
            }
        }

    }
}
