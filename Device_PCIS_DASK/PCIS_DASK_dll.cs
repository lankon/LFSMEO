using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Device_PCIS_DASK.PCIS_DASK
{
    public class Param
    {
        //ADLink PCI Card Type
        public const ushort PCI_9111DG = 20;

        //Channel Count
        public const ushort P9111_CHANNEL_DI = 0;
    }
    public class Functions
    {
        
        #region x86
        //[DllImport("PCI-Dask.dll")]
        //public static extern short Register_Card(ushort CardType, ushort card_num);

        //[DllImport("PCI-Dask.dll")]
        //public static extern short DI_ReadLine(ushort CardNumber, ushort Port, ushort Line, out ushort State);
        #endregion

        #region x64
        [DllImport("PCI-Dask64.dll")]
        public static extern short Register_Card(ushort CardType, ushort card_num);

        [DllImport("PCI-Dask64.dll")]
        public static extern short DI_ReadLine(ushort CardNumber, ushort Port, ushort Line, out ushort State);
        #endregion

    }
}
