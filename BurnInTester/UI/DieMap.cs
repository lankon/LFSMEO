using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BurnInTester.UI
{
    public partial class DieMap : Panel
    {
        public DieMap()
        {
            // 開啟雙緩衝，防止繪圖閃爍
            this.DoubleBuffered = true;
            this.Size = new Size(150, 450);
            //this.BackColor = Color.FromArgb(30, 30, 30); // 延續你的黑底風格

            // 初始化測試資料 (預設全過)
            for (int i = 0; i < DieNum; i++) dieStatus[i] = true;
        }

        #region parameter define
        private const int DieNum = 64;                  // 晶粒總數
        private bool[] dieStatus = new bool[DieNum];    // 儲存 64 顆晶粒的狀態: true = OK (綠), false = Fail (紅)
        #endregion

        #region private function
        private void DrawDie(Graphics g, int index, int x, int y, int size, string prefix)
        {
            Color statusColor = dieStatus[index] ? Color.LimeGreen : Color.Red;
            using (Pen pen = new Pen(statusColor, 2))
            using (Brush brush = new SolidBrush(statusColor))
            {
                // 畫圓圈
                g.DrawEllipse(pen, x, y, size, size);

                // 如果是OK畫個簡單的勾，Fail 畫個 X (選用)
                if (!dieStatus[index])
                    g.FillEllipse(new SolidBrush(Color.FromArgb(100, Color.Red)), x, y, size, size);
                else
                    g.FillEllipse(new SolidBrush(Color.FromArgb(100, Color.LimeGreen)), x, y, size, size);

                // 畫文字標籤
                string label = $"{index + 1}";
                g.DrawString(label, new Font("微軟正黑體", 10), Brushes.Black, x + 25, y + 2);
            }
        }
        #endregion

        #region public function
        public void UpdateDieStatus(int index, bool isOk)
        {
            index--; // 轉換為 0-based 索引

            if (index >= 0 && index < DieNum)
            { 
                dieStatus[index] = isOk;
                this.Invalidate(); // 強制重繪
            }
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int circleSize = 20;
            int verticalSpacing = 28;
            int columnSpacing = 80;
            int startX = 20;
            int startY = 10;

            Font font = new Font("Consolas", 9);
            Brush whiteBrush = Brushes.Gold;

            for (int i = 0; i < 16; i++)
            {
                // 1~16
                DrawDie(g, i, startX, startY + (i * verticalSpacing), circleSize, "Die");
                // 17~32
                DrawDie(g, i + 16, startX + columnSpacing, startY + (i * verticalSpacing), circleSize, "Die");
                // 33~48
                DrawDie(g, i + 32, startX + columnSpacing * 2, startY + (i * verticalSpacing), circleSize, "Die");
                // 49~64
                DrawDie(g, i + 48, startX + columnSpacing * 3, startY + (i * verticalSpacing), circleSize, "Die");
            }
        }

        
    }
}
