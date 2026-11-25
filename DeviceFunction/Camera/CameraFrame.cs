using DeviceCore;
using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DeviceFunction
{
    internal sealed class CameraFrame : IImageFrame
    {
        // 保存影像,並記錄被引用次數
        public CameraFrame(IntPtr ImageData, int Width, int Height, int Stride,
                           PixelFormat Format, int CcdIndex, Action<CameraFrame> ReleaseAction)
        {
            this.ImageData = ImageData;
            this.Width = Width;
            this.Height = Height;
            this.Stride = Stride;
            this.Format = Format;
            CCD_Index = CcdIndex;
            this.ReleaseAction = ReleaseAction;
            RefCount = 1;
        }

        #region parameter
        private readonly object Sync = new object();
        private readonly Action<CameraFrame> ReleaseAction;
        private int RefCount;   // 紀錄目前被引用數量

        public IntPtr ImageData { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public int CCD_Index { get; private set; }
        public PixelFormat Format { get; private set; }
        #endregion

        public bool TryAddRef()
        {
            lock (Sync)
            {
                if (RefCount <= 0)
                    return false;

                RefCount++;
                return true;
            }
        }

        public void Dispose()
        {
            lock (Sync)
            {
                if (RefCount <= 0)
                    return;

                RefCount--;
                if (RefCount > 0)
                    return;
            }

            ReleaseAction?.Invoke(this);
        }
    }

    internal sealed class CameraFrameBufferPool
    {
        // 管理影像Buffer
        private readonly object Sync = new object();
        private readonly FrameSlot[] Slots;
        private readonly int CcdIndex;

        public CameraFrameBufferPool(int CcdIndex, int Count)
        {
            this.CcdIndex = CcdIndex;
            Slots = new FrameSlot[Count];

            for (int i = 0; i < Slots.Length; i++)
                Slots[i] = new FrameSlot();
        }

        public CameraFrame TryRent(int Width, int Height, int Stride, PixelFormat Format)
        {
            lock (Sync)
            {
                for (int i = 0; i < Slots.Length; i++)
                {
                    FrameSlot Slot = Slots[i];
                    if (Slot.InUse)
                        continue;

                    int RequiredBytes = Stride * Height;
                    if (Slot.Buffer == IntPtr.Zero || Slot.Capacity < RequiredBytes)
                    {
                        if (Slot.Buffer != IntPtr.Zero)
                            Marshal.FreeHGlobal(Slot.Buffer);

                        Slot.Buffer = Marshal.AllocHGlobal(RequiredBytes);
                        Slot.Capacity = RequiredBytes;
                    }

                    Slot.InUse = true;
                    Slot.Frame = new CameraFrame(Slot.Buffer, Width, Height, Stride, Format, CcdIndex, Release);
                    return Slot.Frame;
                }
            }

            return null;
        }

        private void Release(CameraFrame Frame)
        {
            lock (Sync)
            {
                for (int I = 0; I < Slots.Length; I++)
                {
                    FrameSlot Slot = Slots[I];
                    if (!object.ReferenceEquals(Slot.Frame, Frame))
                        continue;

                    Slot.Frame = null;
                    Slot.InUse = false;
                    return;
                }
            }
        }

        private sealed class FrameSlot
        {
            public IntPtr Buffer = IntPtr.Zero;
            public int Capacity = 0;        // 可用記憶體容量
            public bool InUse = false;
            public CameraFrame Frame;
        }
    }
}
