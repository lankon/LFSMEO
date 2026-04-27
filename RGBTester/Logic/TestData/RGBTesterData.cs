using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    class RGBTesterData
    {
        // [TestCondition]
        public List<int> DACpoint = new List<int>();

        // [DAQ Point]
        public List<double> Vled = new List<double>();

        // [TestItem]
        public List<double> Vin = new List<double>();
        public List<double> Iin = new List<double>();
        public List<double> Pin = new List<double>();
        public List<double> Vf = new List<double>();
        public List<double> Iled = new List<double>();
        public List<double> Pled = new List<double>();
        public List<double> Eff = new List<double>();
        public List<double> DISP_6V0 = new List<double>();
        public List<double> DISP_1V2 = new List<double>();

        // [Record]
        public List<double> CycleTime = new List<double>();
        public List<double> Temperature = new List<double>();
    }
}
