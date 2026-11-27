namespace DeviceCore
{
    public interface ICameraDisplayService
    {
        void BindingDisplayEvent();
        void SwitchToCameraDisplay(int ccd);
        void DockDisplayToPanel(object container);
        void DockDisplayToLyPnl(object container);
        void FitWindow(int ccd);
    }
}
