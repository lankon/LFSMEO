using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using ToolFunction;
using DeviceCore;

namespace DeviceFunction
{
    public class Function_IO_Card: IFunction_IO_Card
    {
        public Function_IO_Card(IEnumerable<IIOCard> cards)
        {
            Cards = cards;
        }

        #region parameter define
        private List<IIOCard> IO = new List<IIOCard>();
        private List<IOData> IO_List = new List<IOData>();
        private Dictionary<string, IOData> ioListDict;
        private IEnumerable<IIOCard> Cards;
        private IF_IO_Card F_IO;
        public bool InitialDone { get; private set; } = false;
        #endregion

        #region private function
        private void Process()
        {
            while(true)
            {
                //Thread持續讀取Input訊號
                for (int k = 0; k < IO.Count; k++)
                {
                    if(IO[k].GetName() == "MN200")
                    {
                        //List<byte> LineNo = IO[k].Get_IO_LineNo();
                        //List<byte> DevNo = IO[k].Get_IO_DevNo();

                        //for (byte i = 0; i < LineNo.Count; i++)
                        //{
                        //    IO[k].UpdateInput(lineNo: LineNo[i], devNo: DevNo[i]);
                        //}
                    }
                    else if(IO[k].GetName() == "PCI_9111DG")
                    {
                        for (byte i = 0; i <= 15; i++)
                            IO[k].UpdateInput(port: i);
                    }
                    else if (IO[k].GetName() == "AMP_204C")
                    {
                        IO[k].UpdateInput();
                    }
                }

                Thread.Sleep(15);
            }
        }
        #endregion

        #region public function
        public void Set_IO_Form(IF_IO_Card f_io)
        {
            F_IO = f_io;
        }
        public bool Initial_All_IO()
        {
            bool UseMN200 = false, UseP32C32 = false, UsePcisDask = false, UseAPS = false ;

            foreach(IIOCard card in Cards)
            {
                if(card.Open() == true)
                    IO.Add(card);
            }

            for (int i = 0; i < IO.Count; i++)
            {
                if (IO[i].GetName() == "MN200")
                    UseMN200 = true;
                if (IO[i].GetName() == "PCI_9111DG" || IO[i].GetName() == "PCI_9111HR")
                    UsePcisDask = true;
                if (IO[i].GetName() == "AMP_204C")
                    UseAPS = true;
            }

            if (!UseMN200 && !UseP32C32 && !UsePcisDask && !UseAPS)    //沒有任何一張IO卡
            {
                Tool.SaveLogToFile("IO卡Initial失敗");
                InitialDone = false;
                return false;
            }
            Task task = Task.Run(() => Process());

            F_IO.Update_IO_List();
            F_IO.UpdateOutputStatus_UI();

            InitialDone = true;

            return true;
        }
        //public void Update_IO_List(DataGridView DGV, List<IOData> io_list)
        //{
        //    io_list.Clear();
        //    IO_List.Clear();

        //    foreach (DataGridViewRow row in DGV.Rows)
        //    {
        //        if (row.IsNewRow) continue;

        //        var data = new IOData()
        //        {
        //            Title_IO = row.Cells["Title_IO"]?.Value?.ToString(),
        //            Title_Name = row.Cells["Title_Name"]?.Value?.ToString(),
        //            Title_Description = row.Cells["Title_Description"]?.Value?.ToString(),
        //            Title_CardType = row.Cells["Title_CardType"]?.Value?.ToString(),
        //            Title_IO_Num = Convert.ToInt32(row.Cells["Title_IO_Num"]?.Value ?? "-1"),
        //            Title_Status = row.Cells["Title_Status"]?.Value?.ToString(),
        //            Title_Inverse = row.Cells["Title_Inverse"]?.Value?.ToString(),
        //            Title_CardNum = Convert.ToInt32(row.Cells["Title_CardNum"]?.Value ?? "-1"),
        //            Title_LineNum = Convert.ToInt32(row.Cells["Title_LineNum"]?.Value ?? "-1"),
        //            Title_DevNum = Convert.ToInt32(row.Cells["Title_DevNum"]?.Value ?? "-1"),
        //        };

        //        io_list.Add(data);
        //        IO_List.Add(data);
        //    }

        //    ioListDict = IO_List.GroupBy(x => x.Title_Name).ToDictionary(g => g.Key, g => g.First());
        //}
        public void LoadConfiguration(List<IOData> newIoDataList)
        {
            IO_List.Clear();
            IO_List.AddRange(newIoDataList);
            ioListDict = IO_List
                        .GroupBy(x => x.Title_Name)
                        .ToDictionary(g => g.Key, g => g.First());
        }
        public double GetAInputStatus(EIOCardType CardType, byte card, byte lineNo, byte devNo, byte port, int iList)
        {
            for (int i = 0; i < IO.Count; i++)
            {
                if (IO[i].GetName() != CardType.ToString())
                    continue;

                return IO[i].GetAInput(card, lineNo, devNo, port);
            }

            return -1;
        }
        public double GetAInputStatus(EIOName name)
        {
            ioListDict.TryGetValue(name.ToString(), out IOData iOData);

            byte card = (byte)iOData.Title_CardNum;
            byte lineNo = (byte)iOData.Title_LineNum;
            byte devNo = (byte)iOData.Title_DevNum;
            byte port = (byte)iOData.Title_IO_Num;
            string range = iOData.Title_Range;

            for (int j = 0; j < IO.Count; j++)
            {
                if (IO[j].GetName() != iOData.Title_CardType || range == "Digital")
                    continue;

                return IO[j].GetAInput(card, lineNo, devNo, port);
            }

            return -1;
        }
        public bool GetInputStatus(EIOCardType CardType, byte card, byte lineNo, byte devNo, byte port, int iList)
        {
            for (int i = 0; i < IO.Count; i++)
            {
                if (IO[i].GetName() != CardType.ToString())
                    continue;

                if (IO_List[iList].Title_Inverse == "True" || IO_List[iList].Title_Inverse == "true")
                    return !IO[i].GetInputStatus(card, lineNo, devNo, port);
                else if (IO_List[iList].Title_Inverse == "False" || IO_List[iList].Title_Inverse == "false")
                    return IO[i].GetInputStatus(card, lineNo, devNo, port);
            }

            return false;
        }
        public bool GetInputStatus(EIOName name)
        {
            ioListDict.TryGetValue(name.ToString(), out IOData iOData);

            byte card = (byte)iOData.Title_CardNum;
            byte lineNo = (byte)iOData.Title_LineNum;
            byte devNo = (byte)iOData.Title_DevNum;
            byte port = (byte)iOData.Title_IO_Num;
                    
            for (int j = 0; j < IO.Count; j++)
            {
                if (IO[j].GetName() != iOData.Title_CardType)
                    continue;

                if (iOData.Title_Inverse == "True" || iOData.Title_Inverse == "true")
                    return !IO[j].GetInputStatus(card, lineNo, devNo, port);
                else if (iOData.Title_Inverse == "False" || iOData.Title_Inverse == "false")
                    return IO[j].GetInputStatus(card, lineNo, devNo, port);
            }
            
            return false;
        }
        public bool GetOutputStatus(EIOCardType CardType,byte cardNo, byte lineNo, byte devNo, byte port, int iList)
        {
            for (int i = 0; i < IO.Count; i++)
            {
                if (IO[i].GetName() != CardType.ToString())
                    continue;

                IO[i].GetOutputStatus(cardNo, lineNo, devNo, port);

                if (IO_List[iList].Title_Inverse == "True" || IO_List[iList].Title_Inverse == "true")
                    return !IO[i].GetOutputStatus(cardNo, lineNo, devNo, port);
                else if (IO_List[iList].Title_Inverse == "False" || IO_List[iList].Title_Inverse == "false")
                    return IO[i].GetOutputStatus(cardNo, lineNo, devNo, port);
            }

            return false;
        }
        public bool SetOutputStatus(EIOCardType CardType, byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            for (int i = 0; i < IO.Count; i++)
            {
                if (IO[i].GetName() != CardType.ToString())
                    continue;

                IO[i].SetOutputStatus(cardNo, lineNo, devNo, port, truefalse);
            }

            return true;
        }
        public bool SetOutputStatus(EIOName name, bool truefalse)
        {
            ioListDict.TryGetValue(name.ToString(), out IOData iOData);

            byte cardNo = (byte)iOData.Title_CardNum;
            byte lineNo = (byte)iOData.Title_LineNum;
            byte devNo = (byte)iOData.Title_DevNum;
            byte port = (byte)iOData.Title_IO_Num;

            for (int j = 0; j < IO.Count; j++)
            {
                if (IO[j].GetName() != iOData.Title_CardType)
                    continue;

                if (iOData.Title_Inverse == "True" || iOData.Title_Inverse == "true")
                    IO[j].SetOutputStatus(cardNo, lineNo, devNo, port, !truefalse);
                else if (iOData.Title_Inverse == "False" || iOData.Title_Inverse == "false")
                    IO[j].SetOutputStatus(cardNo, lineNo, devNo, port, truefalse);
            }

            return true;
        }
        #endregion
    }
}
