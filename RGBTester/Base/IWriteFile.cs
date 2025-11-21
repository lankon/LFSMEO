using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IWriteFile
    {
        void CreateFile(string describe = "");
        void WriteFile(string context = "", string describe = "");
        void CloseFile(string describe = "");
    }
}
