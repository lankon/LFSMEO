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
        public List<double> Vf_R = new List<double>();
        public List<double> Vf_G = new List<double>();
        public List<double> Vf_B = new List<double>();
        public List<double> Iled = new List<double>();
        public List<double> Pin = new List<double>();
        public List<double> Pled_R = new List<double>();
        public List<double> Pled_G = new List<double>();
        public List<double> Pled_B = new List<double>();
        public List<double> Eff_R = new List<double>();
        public List<double> Eff_G = new List<double>();
        public List<double> Eff_B = new List<double>();
    }
}
