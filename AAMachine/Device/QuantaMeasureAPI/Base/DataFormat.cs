using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAMachine.Device.QuantaMeasureAPI.Base
{
    public enum MeasurePixelFormat
    {
        Gray8,
        Gray16,
        Gray32
    }

    public enum TestSide
    {
        Left,
        Right,
    }

    public class MeasureImageInfo
    {
        public byte[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Channels { get; set; }
        public MeasurePixelFormat Format { get; set; }
    }

    public class UniformityResultInfo
    {
        public double Away;

        public double MaxMin;

        public double[,] IntensityMap;
    }

    public class RollOffResultInfo
    {
        public double ZoneB { get; set; }

        public double ZoneC { get; set; }
    }

    public class ContrastResultInfo
    {
        public double InField { get; set; }

        public double[] NinePoints { get; set; }
    }
}
