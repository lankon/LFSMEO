using BurnInTester.Device;
using DeviceCore;
using System;
using System.Runtime;
using System.Windows.Forms;
using ToolFunction;

namespace BurnInTester.UI
{
    public partial class F_VirtualTemperature : Form
    {
        public F_VirtualTemperature(HW_ParamSetting settings, IFunction_TemperatureControl tc)
        {
            InitializeComponent();

            this.TopMost = true;

            hW_Param = settings;
            VirtualTC = tc as IVirtualTemperatureSimulation;
        }

        #region parameter define
        private bool refreshing;    // 防治同時更新用Flag
        private HW_ParamSetting hW_Param;
        IVirtualTemperatureSimulation VirtualTC = null;

        private sealed class Target
        {
            public string Command;
            public string Description;
            public override string ToString() => Description;
        }
        #endregion

        #region private function
        private void UpdatePage()
        {
            for (int i = 0; i < hW_Param.TC_Box._CtrlBoxNum; i++)
            {
                if (hW_Param.TC_Box.Use[i])
                {
                    Cmbx_BoxCh.Items.Add(new Target
                    {
                        Command = hW_Param.TC_Box.BoxNum[i] + "," + hW_Param.TC_Box.ChNum[i],
                        Description = $"Box {i + 1} (Address {hW_Param.TC_Box.BoxNum[i]}, Channel {hW_Param.TC_Box.ChNum[i]})"
                    });
                }
            }

            if (Cmbx_BoxCh.Items.Count > 0)
                Cmbx_BoxCh.SelectedIndex = 0;
            else
            {
                CkBx_Manual.Enabled = Btn_Reset.Enabled = Btn_ApplyAll.Enabled = false;
                UpdateEnabled();
            }
        }
        private void RefreshSelection()
        {
            var item = Cmbx_BoxCh.SelectedItem as Target;
            bool isManual = false;
            double manualTemperature = 25;
            bool enabled = item != null && VirtualTC.TryGetSimulationSettings(hW_Param.TC1, item.Command, out isManual, out manualTemperature);
            refreshing = true;
            
            CkBx_Manual.Checked = isManual;
            CkBx_Manual.Enabled = enabled;
            NumUpDn_Temperature.Value = Math.Max(0, Math.Min(150, (decimal)manualTemperature));
            TrackBar_Temperature.Value = (int)(NumUpDn_Temperature.Value * 10);
            
            refreshing = false;
            UpdateEnabled();
        }
        private void Apply()
        {
            if (refreshing)
                return;

            var item = Cmbx_BoxCh.SelectedItem as Target;
            if (item == null)
                return;

            bool success = VirtualTC.TrySetSimulation(hW_Param.TC1, item.Command, CkBx_Manual.Checked, (double)NumUpDn_Temperature.Value);
        }
        private void UpdateEnabled()
        {
            bool enable = CkBx_Manual.Enabled && CkBx_Manual.Checked;
            TrackBar_Temperature.Enabled = enable;
            NumUpDn_Temperature.Enabled = enable;
        }
        #endregion
        private void Cmbx_BoxCh_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelection();
        }

        private void CkBx_Manual_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled(); 
            Apply();
        }

        private void NumUpDn_Temperature_ValueChanged(object sender, EventArgs e)
        {
            if (refreshing)
                return;

            TrackBar_Temperature.Value = (int)(NumUpDn_Temperature.Value * 10);
            Apply();
        }

        private void TrackBar_Temperature_ValueChanged(object sender, EventArgs e)
        {
            if (!refreshing)
                NumUpDn_Temperature.Value = TrackBar_Temperature.Value / 10M;
        }

        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            bool success = true;
            foreach (Target item in Cmbx_BoxCh.Items)
                success &= VirtualTC.TrySetSimulation(hW_Param.TC1, item.Command, CkBx_Manual.Checked, 25.0);
        }

        private void Btn_ApplyAll_Click(object sender, EventArgs e)
        {
            bool success = true;
            foreach (Target item in Cmbx_BoxCh.Items)
                success &= VirtualTC.TrySetSimulation(hW_Param.TC1, item.Command, CkBx_Manual.Checked, (double)NumUpDn_Temperature.Value);
        }

        private void F_VirtualTemperature_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
            }
            else
            {
                UpdatePage();
            }
        }
    }
}
