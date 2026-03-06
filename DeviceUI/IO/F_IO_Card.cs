using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using ToolFunction;
using DeviceCore;

namespace DeviceUI.IO
{
    public partial class F_IO_Card : Form, IF_IO_Card
    {
        public F_IO_Card(IFunction_IO_Card function_IO_Card)
        {
            InitializeComponent();

            DIOL = function_IO_Card;

            DIOL.Set_IO_Form(this);

            if (!Tool.DataGrid_DataLoad(DGV_IO, "IO.xml"))
                Tool.SaveLogToFile("IO表讀取失敗");
        }
        
        #region parameter
        List<IOData> IOList = new List<IOData>();
        IFunction_IO_Card DIOL;
        private bool IO_Init = false;
        #endregion

        #region private function
        private bool IOCard_GetInputStatus(int i)
        {
            bool input_res = false;

            if (IOList[i].Title_IO == "Input" &&
                Enum.TryParse<EIOCardType>(IOList[i].Title_CardType, out var cardType))
            {
                if (cardType == EIOCardType.None)
                    return input_res;
                
                input_res = DIOL.GetInputStatus(cardType,
                                    (byte)IOList[i].Title_CardNum,
                                    (byte)IOList[i].Title_LineNum,
                                    (byte)IOList[i].Title_DevNum,
                                    (byte)IOList[i].Title_IO_Num, i);
            }

            return input_res;
        }
        private double IOCard_GetAIValue(int i)
        {
            double input_res = 0.0;

            if (IOList[i].Title_IO == "Input" &&
                Enum.TryParse<EIOCardType>(IOList[i].Title_CardType, out var cardType))
            {
                if (cardType == EIOCardType.None)
                    return input_res;

                input_res = DIOL.GetAInputStatus(cardType,
                                                (byte)IOList[i].Title_CardNum,
                                                (byte)IOList[i].Title_LineNum,
                                                (byte)IOList[i].Title_DevNum,
                                                (byte)IOList[i].Title_IO_Num,
                                                IOList[i].Title_Range, i);
            }

            return input_res;
        }
        private bool IOCard_GetOutputStatus(int i)
        {
            bool output_res = false;

            if (IOList[i].Title_IO == "Output" &&
                Enum.TryParse<EIOCardType>(IOList[i].Title_CardType, out var cardType))
            {
                output_res = DIOL.GetOutputStatus(cardType,
                                    (byte)IOList[i].Title_CardNum,
                                    (byte)IOList[i].Title_LineNum,
                                    (byte)IOList[i].Title_DevNum,
                                    (byte)IOList[i].Title_IO_Num, i);
            }

            return output_res;
        }
        private void Update_IO_List(DataGridView DGV, List<IOData> io_list)
        {
            io_list.Clear();

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                var data = new IOData()
                {
                    Title_IO = row.Cells["Title_IO"]?.Value?.ToString(),
                    Title_Name = row.Cells["Title_Name"]?.Value?.ToString(),
                    Title_Description = row.Cells["Title_Description"]?.Value?.ToString(),
                    Title_CardType = row.Cells["Title_CardType"]?.Value?.ToString(),
                    Title_IO_Num = Convert.ToInt32(row.Cells["Title_IO_Num"]?.Value ?? "-1"),
                    Title_Status = row.Cells["Title_Status"]?.Value?.ToString(),
                    Title_Inverse = row.Cells["Title_Inverse"]?.Value?.ToString(),
                    Title_CardNum = Convert.ToInt32(row.Cells["Title_CardNum"]?.Value ?? "-1"),
                    Title_LineNum = Convert.ToInt32(row.Cells["Title_LineNum"]?.Value ?? "-1"),
                    Title_DevNum = Convert.ToInt32(row.Cells["Title_DevNum"]?.Value ?? "-1"),
                    Title_Range = row.Cells["Title_Range"]?.Value?.ToString(),
                };

                io_list.Add(data);
            }

            DIOL.LoadConfiguration(io_list);
        }
        private void UpdatePage()
        {
            Timer_IO.Enabled = true;
            UpdateOutputStatus_UI();
        }
        #endregion

        #region public function
        public void Update_IO_List()
        {
            Update_IO_List(DGV_IO, IOList);
        }

        public void UpdateOutputStatus_UI()
        {
            for (int i = 0; i < IOList.Count; i++)
            {
                bool output_res = false;

                output_res = IOCard_GetOutputStatus(i);

                if (output_res && DGV_IO.Rows[i].Cells["Title_IO"].Value.ToString() == "Output")
                {
                    DGV_IO.Rows[i].Cells["Title_Status"].Value = "ON";
                    DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.SkyBlue;
                }
                else if (output_res == false && DGV_IO.Rows[i].Cells["Title_IO"].Value.ToString() == "Output")
                {
                    DGV_IO.Rows[i].Cells["Title_Status"].Value = "OFF";
                    DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.White;
                }
            }
        }
        #endregion

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "None", "None", "None", "None", "-1", "OFF","Digital","False", "0", "0", "0" };

            Tool.DataGrid_AddRow(DGV_IO, context);
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DataSave(DGV_IO, "IO.xml");
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Timer_IO.Enabled = false;
            
            Tool.DataGrid_DeleteRow(DGV_IO);

            Update_IO_List(DGV_IO, IOList);
             
            Timer_IO.Enabled = true;
        }

        private void Btn_RowUp_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowUp(DGV_IO);
        }

        private void Btn_RowDown_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowDown(DGV_IO);
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if(!Tool.DataGrid_DataLoad(DGV_IO, "IO.xml"))
                Tool.SaveLogToFile("IO表讀取失敗");
        }

        private void DGV_IO_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IOList.Count == 0)
                return;
            
            // 取得改變的欄位與行
            int rowIndex = e.RowIndex;
            int colIndex = e.ColumnIndex;

            // 可以用欄位名稱來確認是你要監控的欄位
            var columnName = DGV_IO.Columns[colIndex].Name;

            if (columnName == "Title_IO_Num" && rowIndex >= 0)
            {
                object newValue = DGV_IO.Rows[rowIndex].Cells[colIndex].Value;

                Int32.TryParse((string)newValue, out int port);

                IOList[rowIndex].Title_IO_Num = port;
            }

            Update_IO_List(DGV_IO, IOList);
        }

        private void Timer_IO_Tick(object sender, EventArgs e)
        {
            //要再想一下怎麼做比較好,Timer離開畫面時必須關掉
            
            if (!DIOL.InitialDone)
                return;

            for (int i = 0; i < IOList.Count; i++)
            {
                //[Digital Input]
                if (DGV_IO.Rows[i].Cells["Title_IO"].Value.ToString() == "Input" && DGV_IO.Rows[i].Cells["Title_Range"].Value.ToString() == "Digital")
                {
                    bool input_res = IOCard_GetInputStatus(i);

                    if (input_res)
                    {
                        DGV_IO.Rows[i].Cells["Title_Status"].Value = "ON";
                        DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.SkyBlue;
                    }
                    else if (input_res == false)
                    {
                        DGV_IO.Rows[i].Cells["Title_Status"].Value = "OFF";
                        DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.White;
                    }
                }

                //[Analog Input]
                if (DGV_IO.Rows[i].Cells["Title_IO"].Value.ToString() == "Input" && DGV_IO.Rows[i].Cells["Title_Range"].Value.ToString() != "Digital")
                {
                    double ai_res = IOCard_GetAIValue(i);

                    DGV_IO.Rows[i].Cells["Title_Status"].Value = ai_res.ToString();
                    DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.SkyBlue;
                }
            }
        }

        private void DGV_IO_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!DIOL.InitialDone)
                return;

            if (e.RowIndex >= 0 && 
                DGV_IO.Columns[e.ColumnIndex].Name == "Title_Status" && 
                DGV_IO.Rows[e.RowIndex].Cells["Title_IO"].Value.ToString() == "Output")
            {
                //判斷IO卡
                EIOCardType eIOCardType = EIOCardType.None;

                string cardTypeStr = DGV_IO.Rows[e.RowIndex].Cells["Title_CardType"].Value?.ToString();

                if (!string.IsNullOrEmpty(cardTypeStr) && Enum.TryParse<EIOCardType>(cardTypeStr, out var parsedType))
                    eIOCardType = parsedType;

                // 取得目前儲存格值
                var value = DGV_IO.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                // 可以根據值做切換
                if (value == "ON")
                {
                    DGV_IO.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "OFF";
                    DGV_IO.Rows[e.RowIndex].Cells["Title_Status"].Style.BackColor = Color.White;

                    DIOL.SetOutputStatus(eIOCardType,
                                            (byte)IOList[e.RowIndex].Title_CardNum,
                                            (byte)IOList[e.RowIndex].Title_LineNum,
                                            (byte)IOList[e.RowIndex].Title_DevNum,
                                            (byte)IOList[e.RowIndex].Title_IO_Num,
                                            false);
                }
                else
                {
                    DGV_IO.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "ON";
                    DGV_IO.Rows[e.RowIndex].Cells["Title_Status"].Style.BackColor = Color.SkyBlue;

                    DIOL.SetOutputStatus(eIOCardType,
                                            (byte)IOList[e.RowIndex].Title_CardNum,
                                            (byte)IOList[e.RowIndex].Title_LineNum,
                                            (byte)IOList[e.RowIndex].Title_DevNum,
                                            (byte)IOList[e.RowIndex].Title_IO_Num,
                                            true);
                }

                DGV_IO.ClearSelection();
                DGV_IO.CurrentCell = null;
            }
        }

        private void F_IO_Card_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                Timer_IO.Enabled = false;
                
                //SaveAllEnumSetting();
                //ReadAllEnumSetting();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_IO_Test_Click(object sender, EventArgs e)
        {
            DIOL.AddIORule(0, 0, 1, 3, true,(0, 0, 1, 5, false),(0, 0, 1, 7, true));
            DIOL.AddIORule(0, 0, 1, 3, false, (0, 0, 1, 5, true), (0, 0, 1, 7, false));

            int state = 0;
            switch (state)
            {
                case 0:
                    DIOL.SetOutputStatus(EIOName.GoToSafePos, true);
                    goto case 1;
                case 1:
                    if (DIOL.GetInputStatus(EIOName.SafePos_Sensor_Out) == true &&
                        DIOL.GetInputStatus(EIOName.SafePos_Sensor_In) == false)
                    {
                        int aa = 0;
                        //Success
                    }
                    break;
            }

        }
    }
}
