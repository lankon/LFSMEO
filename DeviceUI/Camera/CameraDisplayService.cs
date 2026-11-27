using System;
using System.Windows.Forms;

using DeviceCore;

namespace DeviceUI.Camera
{
    public class CameraDisplayService : ICameraDisplayService
    {
        public CameraDisplayService(IFunction_Camera function_Camera)
        {
            Function_Camera = function_Camera;
            InitialDisplayPanels();
        }

        #region parameter define
        IFunction_Camera Function_Camera;
        CameraDisplayPanel[] DisplayPanels;
        #endregion

        #region private function
        private void InitialDisplayPanels()
        {
            //此Function程式執行後只能呼叫一次
            int length = CCD_NAME.GetNames(typeof(CCD_NAME)).Length;
            DisplayPanels = new CameraDisplayPanel[length];
            for (int i = 0; i < DisplayPanels.Length; i++)
            {
                DisplayPanels[i] = new CameraDisplayPanel(Function_Camera, i);
                IntPtr forceHandle = DisplayPanels[i].Handle;   //在UI執行緒先建立Handle，供影像更新使用
            }
        }
        #endregion

        #region public function
        public void BindingDisplayEvent()
        {
            //此Fcuntion程式執行後只能呼叫一次
            int i = 0;

            foreach (CameraDisplayPanel panel in DisplayPanels)
            {
                Function_Camera.Subscribe(i, (s, fe) =>
                {
                    int realID = fe.CCD_Index;

                    if (DisplayPanels[realID].IsUpdating == true)   //UI還在更新,放棄更新畫面(丟禎)
                        return;

                    if (fe.Frame == null || !fe.Frame.TryAddRef())
                        return;

                    DisplayPanels[realID].IsUpdating = true;

                    try
                    {
                        DisplayPanels[realID].BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                DisplayPanels[realID].CurrentFrame = fe.Frame;
                                DisplayPanels[realID].Update(); //呼叫UI立刻繪圖
                            }
                            finally
                            {
                                DisplayPanels[realID].IsUpdating = false;
                            }
                        }));
                    }
                    catch
                    {
                        fe.Frame.Dispose();
                        DisplayPanels[realID].IsUpdating = false;
                    }
                });
                i++;
            }
        }
        public void SwitchToCameraDisplay(int ccd)
        {
            if (DisplayPanels[ccd].InvokeRequired)
                DisplayPanels[ccd].Invoke(new Action(() => SwitchToCameraDisplay(ccd)));
            else
                DisplayPanels[ccd].BringToFront();
        }
        public void DockDisplayToPanel(object container)
        {
            if (container is Control parent)
            {
                foreach (CameraDisplayPanel panel in DisplayPanels)
                {
                    parent.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                }
            }
        }
        public void DockDisplayToLyPnl(object container)
        {
            if (container is TableLayoutPanel tableLayoutPanel)
            {
                foreach (CameraDisplayPanel panel in DisplayPanels)
                {
                    tableLayoutPanel.Controls.Add(panel, 0, 1);
                    panel.Dock = DockStyle.Fill;
                }
            }
        }
        public void FitWindow(int ccd)
        {
            DisplayPanels[ccd].FitWindow();
        }
        #endregion
    }
}
