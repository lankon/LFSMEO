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
        
        public void Update_IO_List(DataGridView DGV, List<IOData> io_list)
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
        #endregion

        #region public function
        public void SetF_IO_Card(Panel pnl, F_IO_Card form)
        {
            form.Dock = DockStyle.Fill;
            form.Visible = true;
            form.TopLevel = false;
            form.Top = 0;
            form.Left = 0;
            form.TopMost = true;

            pnl.Controls.Add(form);

            form.Hide();
        }
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
            if (!DIOL.InitialDone)
                return;

            for (int i = 0; i < IOList.Count; i++)
            {
                bool input_res = false;

                input_res = IOCard_GetInputStatus(i);

                if (input_res && DGV_IO.Rows[i].Cells["Title_IO"].Value.ToString() != "Output")
                {
                    DGV_IO.Rows[i].Cells["Title_Status"].Value = "ON";
                    DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.SkyBlue;
                }
                else if (input_res == false && DGV_IO.Rows[i].Cells["Title_IO"].Value.ToString() != "Output")
                {
                    DGV_IO.Rows[i].Cells["Title_Status"].Value = "OFF";
                    DGV_IO.Rows[i].Cells["Title_Status"].Style.BackColor = Color.White;
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

                if (DGV_IO.Rows[e.RowIndex].Cells["Title_CardType"].Value.ToString() == "AMP_204C")
                    eIOCardType = EIOCardType.AMP_204C;
                else if (DGV_IO.Rows[e.RowIndex].Cells["Title_CardType"].Value.ToString() == "MN200")
                    eIOCardType = EIOCardType.MN200;
                else if (DGV_IO.Rows[e.RowIndex].Cells["Title_CardType"].Value.ToString() == "P32C32")
                    eIOCardType = EIOCardType.P32C32;


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
    }
}
