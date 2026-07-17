using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DETester.Device.QuantaMeasureAPI.Base
{
    public enum MeasurePixelFormat
    {
        Gray8,
        Gray16,
        Gray32
    }

    public class MeasureImageInfo
    {
        public byte[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Channels { get; set; }
        public MeasurePixelFormat Format { get; set; }
    }
}
