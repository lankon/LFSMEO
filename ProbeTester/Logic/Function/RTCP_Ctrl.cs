using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ProbeTester.Logic
{
    public class RTCP_Controller
    {
        public RTCP_Controller()
        {
        }

        #region parameter define
        private double _dx, _dy, _dz;
        private double _fixedPitchY_deg = 225.0; // 固定的 Y 軸俯仰角度 (Degree)
        public struct RobotPose
        {
            public double X;
            public double Y;
            public double Z;
            public double Tz;
            public double Tx;
            public double Ty;
        }
        public enum AXIS_NAME
        {
            AXIS_X = 0,
            AXIS_Y = 1,
            AXIS_Z = 2,
            AXIS_TX = 3,
            AXIS_TY = 4,
            AXIS_TZ = 5
        }
        #endregion

        #region private function
        private Matrix<double> GetTotalRotationMatrix(double tz_deg, double tx_deg, double ty_deg)
        {
            // 將角度 (Degree) 轉為 弧度 (Radian)
            double rz = tz_deg * Math.PI / 180.0;
            double rx = tx_deg * Math.PI / 180.0;
            double ry = ty_deg * Math.PI / 180.0;

            // 繞Z軸
            var Rz = DenseMatrix.OfArray(new double[,] {
                { Math.Cos(rz), -Math.Sin(rz), 0 },
                { Math.Sin(rz),  Math.Cos(rz), 0 },
                { 0,             0,            1 }
            });

            // 繞X軸
            var Rx = DenseMatrix.OfArray(new double[,] {
                { 1, 0,            0             },
                { 0, Math.Cos(rx), -Math.Sin(rx) },
                { 0, Math.Sin(rx),  Math.Cos(rx) }
            });

            // 繞Y軸
            var Ry = DenseMatrix.OfArray(new double[,] {
                {  Math.Cos(ry), 0, Math.Sin(ry) },
                {  0,            1, 0            },
                { -Math.Sin(ry), 0, Math.Cos(ry) }
            });

            // 必須依照機構相依性進行矩陣相乘，Rz目前在最底層，Rz旋轉會影響到Rx與Ry的旋轉方向
            return Rz * Rx * Ry;
        }
        private Matrix<double> GetHomogeneousMatrix(RobotPose pose)
        {
            double tz = pose.Tz * Math.PI / 180.0;
            double tx = pose.Tx * Math.PI / 180.0;
            double ty = pose.Ty * Math.PI / 180.0;

            var Rz = DenseMatrix.OfArray(new double[,] { { Math.Cos(tz), -Math.Sin(tz), 0, 0 }, { Math.Sin(tz), Math.Cos(tz), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } });
            var Rx = DenseMatrix.OfArray(new double[,] { { 1, 0, 0, 0 }, { 0, Math.Cos(tx), -Math.Sin(tx), 0 }, { 0, Math.Sin(tx), Math.Cos(tx), 0 }, { 0, 0, 0, 1 } });
            var Ry = DenseMatrix.OfArray(new double[,] { { Math.Cos(ty), 0, Math.Sin(ty), 0 }, { 0, 1, 0, 0 }, { -Math.Sin(ty), 0, Math.Cos(ty), 0 }, { 0, 0, 0, 1 } });

            var Trans = DenseMatrix.OfArray(new double[,] { { 1, 0, 0, pose.X }, { 0, 1, 0, pose.Y }, { 0, 0, 1, pose.Z }, { 0, 0, 0, 1 } });

            // 依據您的堆疊機構：平移 * Rz(底) * Rx(中) * Ry(頂)
            return Trans * Rz * Rx * Ry;
        }
        //private Matrix<double> CreateTranslationMatrix(double dx, double dy, double dz)
        //{
        //    return DenseMatrix.OfArray(new double[,] {
        //        { 1, 0, 0, dx },
        //        { 0, 1, 0, dy },
        //        { 0, 0, 1, dz },
        //        { 0, 0, 0,  1 }
        //    });
        //}
        private Matrix<double> CreateToolMatrix(double dx, double dy, double dz, double pitch_deg)
        {
            double rad = pitch_deg * Math.PI / 180.0;

            return DenseMatrix.OfArray(new double[,] {
                {  Math.Cos(rad), 0, Math.Sin(rad), dx },
                {  0,             1, 0,             dy },
                { -Math.Sin(rad), 0, Math.Cos(rad), dz },
                {  0,             0, 0,             1  }
            });
        }
        private Matrix<double> CreateLocalRotationZ(double theta_deg)
        {
            double rad = theta_deg * Math.PI / 180.0;
            return DenseMatrix.OfArray(new double[,] {
                { Math.Cos(rad), -Math.Sin(rad), 0, 0 },
                { Math.Sin(rad),  Math.Cos(rad), 0, 0 },
                { 0,              0,             1, 0 },
                { 0,              0,             0, 1 }
            });
        }
        private Matrix<double> CreateLocalRotationX(double theta_deg)
        {
            double rad = theta_deg * Math.PI / 180.0;
            return DenseMatrix.OfArray(new double[,] {
                { 1, 0,              0,             0 },
                { 0, Math.Cos(rad), -Math.Sin(rad), 0 },
                { 0, Math.Sin(rad),  Math.Cos(rad), 0 },
                { 0, 0,              0,             1 }
            });
        }
        private Matrix<double> CreateLocalTranslation(double delta_x, double delta_y, double delta_z)
        {
            return DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, delta_x },
                { 0, 1, 0, delta_y },
                { 0, 0, 1, delta_z },
                { 0, 0, 0, 1       }
            });
        }
        private double RoundPoseValue(double value)
        {
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }
        private RobotPose ExtractPoseFromMatrix(Matrix<double> T)
        {
            RobotPose pose = new RobotPose();

            // 1. 萃取 XYZ 平移量 (矩陣的第 4 欄，索引為 3)
            pose.X = RoundPoseValue(T[0, 3]);
            pose.Y = RoundPoseValue(T[1, 3]);
            pose.Z = RoundPoseValue(T[2, 3]);

            // 2. 萃取旋轉角度 (解析 3x3 旋轉矩陣區塊)
            // 根據 Rz * Rx * Ry 的推導結果：
            // T[2,1] = sin(Tx)
            // T[2,0] = -cos(Tx)sin(Ty)
            // T[2,2] = cos(Tx)cos(Ty)
            // T[0,1] = -sin(Tz)cos(Tx)
            // T[1,1] = cos(Tz)cos(Tx)

            double sin_tx = T[2, 1];
            double tx_rad = Math.Asin(Math.Max(-1.0, Math.Min(1.0, sin_tx))); // 限制在 -1 到 1 避免浮點數誤差報錯

            double ty_rad, tz_rad;

            // 檢查是否遇到萬向鎖 (Gimbal Lock)，即 cos(Tx) 趨近於 0 (Tx = 90 或 -90 度)
            if (Math.Abs(Math.Cos(tx_rad)) > 1e-6)
            {
                ty_rad = Math.Atan2(-T[2, 0], T[2, 2]);
                tz_rad = Math.Atan2(-T[0, 1], T[1, 1]);
            }
            else
            {
                // 當 Tx = 90 度時的奇異點處理 (實務上機台很少轉到完全垂直)
                ty_rad = 0;
                tz_rad = Math.Atan2(T[1, 0], T[0, 0]);
            }

            // 將弧度轉回度數 (Degree)
            pose.Tx = RoundPoseValue(tx_rad * 180.0 / Math.PI);
            pose.Ty = RoundPoseValue(ty_rad * 180.0 / Math.PI);
            pose.Tz = RoundPoseValue(tz_rad * 180.0 / Math.PI);

            return pose;
        }
        #endregion

        #region public function
        public Vector<double> CalculateTCPOffset(RobotPose[] poses)
        {
            if (poses == null || poses.Length != 4)
            {
                throw new ArgumentException("必須提供剛好 4 個姿態點來進行校正！");
            }

            // 建立 9x3 的 A 矩陣與長度 9 的 B 向量
            var A = Matrix<double>.Build.Dense(9, 3);
            var B = Vector<double>.Build.Dense(9);

            // 取出【第 1 點】作為基準點
            var R1 = GetTotalRotationMatrix(poses[0].Tz, poses[0].Tx, poses[0].Ty);
            var P1 = Vector<double>.Build.Dense(new double[] { poses[0].X, poses[0].Y, poses[0].Z });

            // 依序將【第 2, 3, 4 點】與基準點相減，填入 A 與 B 矩陣
            for (int i = 1; i <= 3; i++)
            {
                var Ri = GetTotalRotationMatrix(poses[i].Tz, poses[i].Tx, poses[i].Ty);
                var Pi = Vector<double>.Build.Dense(new double[] { poses[i].X, poses[i].Y, poses[i].Z });

                // 計算 (R1 - Ri) 與 (Pi - P1)
                var R_diff = R1 - Ri;
                var P_diff = Pi - P1;

                // 將資料填入 9x3 矩陣對應的 Row (每次填 3 個 Row)
                int rowIndexStart = (i - 1) * 3;
                for (int row = 0; row < 3; row++)
                {
                    B[rowIndexStart + row] = P_diff[row]; // 填入 9x1 向量 B

                    for (int col = 0; col < 3; col++)
                    {
                        A[rowIndexStart + row, col] = R_diff[row, col]; // 填入 9x3 矩陣 A
                    }
                }
            }

            // A * X = B
            // X = (A^T * A)^-1 * A^T * B
            // toolOffset = X 
            Vector<double> toolOffset = A.Solve(B);

            _dx = toolOffset[0];
            _dy = toolOffset[1];
            _dz = toolOffset[2];

            return toolOffset;
        }

        public void SetTCPOffset(double dx, double dy, double dz)
        {
            _dx = dx;
            _dy = dy;
            _dz = dz;
        }

        /// <summary>
        /// 執行 RTCP 核心演算：給定當下姿態與想轉的角度，算出 6 軸該去的新位置
        /// </summary>
        /// <param name="currentPose">當前機台 6 軸的絕對位置</param>
        /// <param name="theta_deg">待測物想要繞著「局部 Z 軸 (法向量)」自轉的角度</param>
        /// <returns>計算後 6 軸應該前往的全新目標位置 (Target Pose)</returns>
        public RobotPose CalculateRTCPTarget(RobotPose currentPose, double theta_deg)
        {
            var T_flange_tcp = CreateToolMatrix(_dx, _dy, _dz, _fixedPitchY_deg);
            
            var T_base_flange = GetHomogeneousMatrix(currentPose);
            
            var T_tcp_initial = T_base_flange * T_flange_tcp;

            var R_local_z = CreateLocalRotationZ(theta_deg);

            var T_tcp_new = T_tcp_initial * R_local_z;

            var T_flange_new = T_tcp_new * T_flange_tcp.Inverse();

            RobotPose targetPose = ExtractPoseFromMatrix(T_flange_new);

            return targetPose;
        }

        public RobotPose CalculateRTCPTarget_LocalX(RobotPose currentPose, double theta_deg)
        {
            // Step 1: 工具偏移矩陣 (與之前完全相同)
            var T_flange_tcp = CreateToolMatrix(_dx, _dy, _dz, _fixedPitchY_deg);

            // Step 2: 待測物絕對初始狀態 (與之前完全相同)
            var T_base_flange = GetHomogeneousMatrix(currentPose);
            var T_tcp_initial = T_base_flange * T_flange_tcp;

            // Step 3: 套用局部 X 軸旋轉指令 (🚀 唯一改變的地方！)
            var R_local_x = CreateLocalRotationX(theta_deg);
            var T_tcp_new = T_tcp_initial * R_local_x; // 右乘代表依據局部坐標系旋轉

            // Step 4: 反推法蘭新位置 (與之前完全相同)
            var T_flange_new = T_tcp_new * T_flange_tcp.Inverse();

            // Step 5: 逆向運動學求解 (與之前完全相同)
            RobotPose targetPose = ExtractPoseFromMatrix(T_flange_new);

            return targetPose;
        }

        public RobotPose CalculateLocalTranslationTarget(RobotPose currentPose, double delta_x, double delta_y, double delta_z)
        {
            // Step 1: 工具偏移矩陣
            var T_flange_tcp = CreateToolMatrix(_dx, _dy, _dz, _fixedPitchY_deg);

            // Step 2: 待測物絕對初始狀態 
            var T_base_flange = GetHomogeneousMatrix(currentPose);
            var T_tcp_initial = T_base_flange * T_flange_tcp;

            // Step 3: 套用局部平移指令 (🚀 這裡改成平移矩陣！)
            var Trans_local = CreateLocalTranslation(delta_x, delta_y, delta_z);
            var T_tcp_new = T_tcp_initial * Trans_local; // 右乘代表沿著局部坐標系移動

            // Step 4: 反推法蘭新位置 
            var T_flange_new = T_tcp_new * T_flange_tcp.Inverse();

            // Step 5: 逆向運動學求解 
            // 注意：因為純平移不改變姿態，所以解出來的 Tz, Tx, Ty 理論上會和原本一樣，只有 X, Y, Z 會變
            RobotPose targetPose = ExtractPoseFromMatrix(T_flange_new);

            return targetPose;
        }
        #endregion
    }
}
