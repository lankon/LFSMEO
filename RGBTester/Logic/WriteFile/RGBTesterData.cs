using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    class RGBTesterData
    {
        public List<int> DACpoint = new List<int>();
        
        public List<double> Vin = new List<double>();
        public List<double> Iin = new List<double>();
        public List<double> Vled = new List<double>();
        public List<double> Vf = new List<double>();
        public List<double> Iled = new List<double>();
        public List<double> Pin = new List<double>();
        public List<double> Pled = new List<double>();
        public List<double> Eff = new List<double>();
    }
}
