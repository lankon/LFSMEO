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
        // 運動鏈參數
        private double[] _L1 = new double[3]; // 滑台到 RZ
        private double[] _L2 = new double[3]; // RZ 到 RX
        private double[] _L3 = new double[3]; // RX 到 RY
        private double[] _L4 = new double[3]; // RY 到 45度懸臂轉折點 (關鍵修正！)
        private double[] _Tool = new double[3]; // 懸臂轉折點到 TCP

        private double _FixedPitchY_Deg = 45.0;

        public struct RobotPose { public double X, Y, Z, Tz, Tx, Ty; }

        

        private Matrix<double> Trans(double[] vec) => DenseMatrix.OfArray(new double[,] { { 1, 0, 0, vec[0] }, { 0, 1, 0, vec[1] }, { 0, 0, 1, vec[2] }, { 0, 0, 0, 1 } });
        private Matrix<double> RotX(double deg) { double r = deg * Math.PI / 180; return DenseMatrix.OfArray(new double[,] { { 1, 0, 0, 0 }, { 0, Math.Cos(r), -Math.Sin(r), 0 }, { 0, Math.Sin(r), Math.Cos(r), 0 }, { 0, 0, 0, 1 } }); }
        private Matrix<double> RotY(double deg) { double r = deg * Math.PI / 180; return DenseMatrix.OfArray(new double[,] { { Math.Cos(r), 0, Math.Sin(r), 0 }, { 0, 1, 0, 0 }, { -Math.Sin(r), 0, Math.Cos(r), 0 }, { 0, 0, 0, 1 } }); }
        private Matrix<double> RotZ(double deg) { double r = deg * Math.PI / 180; return DenseMatrix.OfArray(new double[,] { { Math.Cos(r), -Math.Sin(r), 0, 0 }, { Math.Sin(r), Math.Cos(r), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }); }

        private Matrix<double> GetArmMatrix(double tz_deg, double tx_deg, double ty_deg)
        {
            return Trans(_L1) * RotZ(tz_deg)
                 * Trans(_L2) * RotX(tx_deg)
                 * Trans(_L3) * RotY(ty_deg)
                 * Trans(_L4) * RotY(_FixedPitchY_Deg)
                 * Trans(_Tool);
        }

        private Matrix<double> GetArmRotationMatrix(double tz_deg, double tx_deg, double ty_deg)
        {
            return RotZ(tz_deg) * RotX(tx_deg) * RotY(ty_deg) * RotY(_FixedPitchY_Deg);
        }

        private double[] ExtractMachineAngles(Matrix<double> jointRotation)
        {
            double sinTx = Math.Max(-1.0, Math.Min(1.0, jointRotation[2, 1]));
            double txRad = Math.Asin(sinTx);
            double tyRad;
            double tzRad;

            if (Math.Abs(Math.Cos(txRad)) > 1e-6)
            {
                tyRad = Math.Atan2(-jointRotation[2, 0], jointRotation[2, 2]);
                tzRad = Math.Atan2(-jointRotation[0, 1], jointRotation[1, 1]);
            }
            else
            {
                // Gimbal lock: keep Ty at 0 because Rz and Ry become coupled.
                tyRad = 0;
                tzRad = Math.Atan2(jointRotation[1, 0], jointRotation[0, 0]);
            }

            return new double[]
            {
                tzRad * 180.0 / Math.PI,
                txRad * 180.0 / Math.PI,
                tyRad * 180.0 / Math.PI
            };
        }

        /// <summary>
        /// 完整運動學鏈：加入 L4 (懸臂起始間隙)
        /// 順序：Slide -> L1 -> Rz -> L2 -> Rx -> L3 -> Ry -> L4 -> Rot_45 -> Tool
        /// </summary>
        private Matrix<double> GetForwardKinematics(RobotPose pose)
        {
            return Trans(new double[] { pose.X, pose.Y, pose.Z })
                 * GetArmMatrix(pose.Tz, pose.Tx, pose.Ty);
        }

        public void SetKinematicParameters(
            double[] L1, double[] L2, double[] L3, double[] L4_CantileverGap, double[] Tool, double fixedPitchDeg = 225.0)
        {
            _L1 = L1; _L2 = L2; _L3 = L3; _L4 = L4_CantileverGap; _Tool = Tool;
            _FixedPitchY_Deg = fixedPitchDeg;
        }

        public RobotPose CalculateRTCPTarget(RobotPose currentPose, double tx_deg, double ty_deg, double tz_deg)
        {
            var targetTcpMatrix = GetForwardKinematics(currentPose);

            // tx/ty/tz are local TCP-frame rotations. The fixed 45-degree cantilever
            // is already part of the current TCP frame, so apply the delta after it.
            var tcpLocalDelta = RotX(tx_deg) * RotY(ty_deg) * RotZ(tz_deg);
            var targetTcpRotation = GetArmRotationMatrix(currentPose.Tz, currentPose.Tx, currentPose.Ty) * tcpLocalDelta;

            // Remove the fixed cantilever pitch to recover the machine joint rotation.
            var targetJointRotation = targetTcpRotation * RotY(_FixedPitchY_Deg).Inverse();
            var targetAngles = ExtractMachineAngles(targetJointRotation);

            double target_Tz = targetAngles[0];
            double target_Tx = targetAngles[1];
            double target_Ty = targetAngles[2];

            var targetArmMatrix = GetArmMatrix(target_Tz, target_Tx, target_Ty);

            return new RobotPose
            {
                X = targetTcpMatrix[0, 3] - targetArmMatrix[0, 3],
                Y = targetTcpMatrix[1, 3] - targetArmMatrix[1, 3],
                Z = targetTcpMatrix[2, 3] - targetArmMatrix[2, 3],
                Tx = target_Tx,
                Ty = target_Ty,
                Tz = target_Tz
            };
        }
    }

    public class BlackBoxRTCP_Controller
    {
        #region parameter define
        private Vector<double> _A = Vector<double>.Build.Dense(3);
        private Vector<double> _B = Vector<double>.Build.Dense(3);
        private Vector<double> _C = Vector<double>.Build.Dense(3);
        private double _FixedPitchY_Deg = 45.0;
        private bool _IsCalibrated;

        public struct RobotPose { public double X, Y, Z, Tz, Tx, Ty; }

        public struct CalibrationSample
        {
            public string Group;
            public double X, Y, Z, Tz, Tx, Ty;
        }

        public struct CalibrationResult
        {
            public double RmsError;
            public double MaxError;
            public int SampleCount;
            public int GroupCount;
            public double[] A;
            public double[] B;
            public double[] C;
        }
        #endregion

        #region private functions
        private Vector<double> GetOffset(double tz_deg, double tx_deg, double ty_deg)
        {
            var rz = RotZ(tz_deg);
            var rzrx = rz * RotX(tx_deg);
            var rzrxry = rzrx * RotY(ty_deg);

            return rz * _A + rzrx * _B + rzrxry * _C;
        }

        private Matrix<double> GetMachineRotation(double tz_deg, double tx_deg, double ty_deg)
        {
            // 必須依照實際機械堆疊架構來計算旋轉矩陣，目前順序為 Rz -> Rx -> Ry，Rz在最底層
            return RotZ(tz_deg) * RotX(tx_deg) * RotY(ty_deg);
        }

        private Matrix<double> RotX(double deg)
        {
            double r = deg * Math.PI / 180.0;
            return DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0 },
                { 0, Math.Cos(r), -Math.Sin(r) },
                { 0, Math.Sin(r),  Math.Cos(r) }
            });
        }

        private Matrix<double> RotY(double deg)
        {
            double r = deg * Math.PI / 180.0;
            return DenseMatrix.OfArray(new double[,] {
                {  Math.Cos(r), 0, Math.Sin(r) },
                {  0,           1, 0           },
                { -Math.Sin(r), 0, Math.Cos(r) }
            });
        }

        private Matrix<double> RotZ(double deg)
        {
            double r = deg * Math.PI / 180.0;
            return DenseMatrix.OfArray(new double[,] {
                { Math.Cos(r), -Math.Sin(r), 0 },
                { Math.Sin(r),  Math.Cos(r), 0 },
                { 0,            0,           1 }
            });
        }

        
        

        private double[] ExtractMachineAngles(Matrix<double> jointRotation)
        {
            double sinTx = Math.Max(-1.0, Math.Min(1.0, jointRotation[2, 1]));
            double txRad = Math.Asin(sinTx);
            double tyRad;
            double tzRad;

            if (Math.Abs(Math.Cos(txRad)) > 1e-6)
            {
                tyRad = Math.Atan2(-jointRotation[2, 0], jointRotation[2, 2]);
                tzRad = Math.Atan2(-jointRotation[0, 1], jointRotation[1, 1]);
            }
            else
            {
                tyRad = 0;
                tzRad = Math.Atan2(jointRotation[1, 0], jointRotation[0, 0]);
            }

            return new double[]
            {
                tzRad * 180.0 / Math.PI,
                txRad * 180.0 / Math.PI,
                tyRad * 180.0 / Math.PI
            };
        }
        #endregion

        #region public function
        public CalibrationResult Fit(IEnumerable<CalibrationSample> samples)
        {
            var data = samples?.ToList() ?? new List<CalibrationSample>();
            if (data.Count < 4)
            {
                throw new ArgumentException("至少需要 4 筆同一點或多點量測資料才能擬合黑箱 RTCP 模型。");
            }

            var groups = data.GroupBy(s => string.IsNullOrWhiteSpace(s.Group) ? "A" : s.Group)
                             .OrderBy(g => g.Key)
                             .ToList();
            if (groups.Any(g => g.Count() < 2))
            {
                throw new ArgumentException("每一個 Group 至少需要 2 筆資料，才能用相對位移擬合。");
            }

            int measurementCount = groups.Sum(g => g.Count() - 1);
            int unknownCount = 9;
            double regularization = 1e-8;
            var lhs = Matrix<double>.Build.Dense(measurementCount * 3 + unknownCount, unknownCount);
            var rhs = Vector<double>.Build.Dense(measurementCount * 3 + unknownCount);
            int equation = 0;

            foreach (var group in groups)
            {
                var groupSamples = group.ToList();
                var reference = groupSamples[0];
                var refRz = RotZ(reference.Tz);
                var refRzRx = refRz * RotX(reference.Tx);
                var refRzRxRy = refRzRx * RotY(reference.Ty);
                var refMatrices = new Matrix<double>[] { refRz, refRzRx, refRzRxRy };

                for (int i = 1; i < groupSamples.Count; i++)
                {
                    var sample = groupSamples[i];
                    var rz = RotZ(sample.Tz);
                    var rzrx = rz * RotX(sample.Tx);
                    var rzrxry = rzrx * RotY(sample.Ty);
                    var matrices = new Matrix<double>[] { rz, rzrx, rzrxry };
                    var xyzDiff = new double[]
                    {
                        reference.X - sample.X,
                        reference.Y - sample.Y,
                        reference.Z - sample.Z
                    };

                    for (int row = 0; row < 3; row++)
                    {
                        for (int block = 0; block < 3; block++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                lhs[equation, block * 3 + col] = matrices[block][row, col] - refMatrices[block][row, col];
                            }
                        }

                        rhs[equation] = xyzDiff[row];
                        equation++;
                    }
                }
            }

            double regularizationScale = Math.Sqrt(regularization);
            for (int i = 0; i < unknownCount; i++)
            {
                lhs[equation + i, i] = regularizationScale;
                rhs[equation + i] = 0.0;
            }

            var solution = lhs.Solve(rhs);
            _A = solution.SubVector(0, 3);
            _B = solution.SubVector(3, 3);
            _C = solution.SubVector(6, 3);
            _IsCalibrated = true;

            double sumSquaredError = 0.0;
            double maxError = 0.0;

            foreach (var sample in data)
            {
                var sameGroup = string.IsNullOrWhiteSpace(sample.Group) ? "A" : sample.Group;
                var groupSamples = data.Where(s => (string.IsNullOrWhiteSpace(s.Group) ? "A" : s.Group) == sameGroup);
                var center = Vector<double>.Build.Dense(3);
                int count = 0;
                foreach (var groupSample in groupSamples)
                {
                    center += Vector<double>.Build.Dense(new double[] { groupSample.X, groupSample.Y, groupSample.Z }) +
                              GetOffset(groupSample.Tz, groupSample.Tx, groupSample.Ty);
                    count++;
                }
                center /= count;

                var tcp = Vector<double>.Build.Dense(new double[] { sample.X, sample.Y, sample.Z }) +
                          GetOffset(sample.Tz, sample.Tx, sample.Ty);
                double error = (tcp - center).L2Norm();
                sumSquaredError += error * error;
                maxError = Math.Max(maxError, error);
            }

            return new CalibrationResult
            {
                RmsError = Math.Sqrt(sumSquaredError / data.Count),
                MaxError = maxError,
                SampleCount = data.Count,
                GroupCount = groups.Count,
                A = _A.ToArray(),
                B = _B.ToArray(),
                C = _C.ToArray()
            };
        }

        public void SetModel(double[] A, double[] B, double[] C)
        {
            if (A == null || B == null || C == null || A.Length != 3 || B.Length != 3 || C.Length != 3)
            {
                throw new ArgumentException("A/B/C 都必須是長度為 3 的向量。");
            }

            _A = Vector<double>.Build.Dense(A);
            _B = Vector<double>.Build.Dense(B);
            _C = Vector<double>.Build.Dense(C);
            _IsCalibrated = true;
        }

        public RobotPose CalculateRTCPTargetByMachineAngles(RobotPose currentPose, double targetTx, double targetTy, double targetTz)
        {
            if (!_IsCalibrated)
            {
                throw new InvalidOperationException("請先呼叫 Fit() 或 SetModel() 完成黑箱 RTCP 校正。");
            }

            var currentXYZ = Vector<double>.Build.Dense(new double[] { currentPose.X, currentPose.Y, currentPose.Z });
            var currentTcp = currentXYZ + GetOffset(currentPose.Tz, currentPose.Tx, currentPose.Ty);
            var targetOffset = GetOffset(targetTz, targetTx, targetTy);
            var targetXYZ = currentTcp - targetOffset;

            return new RobotPose
            {
                X = targetXYZ[0],
                Y = targetXYZ[1],
                Z = targetXYZ[2],
                Tx = targetTx,
                Ty = targetTy,
                Tz = targetTz
            };
        }

        public RobotPose CalculateRTCPTargetByTcpLocalRotation(RobotPose currentPose, double tx_deg, double ty_deg, double tz_deg)
        {
            var currentTcpRotation = GetMachineRotation(currentPose.Tz, currentPose.Tx, currentPose.Ty) * RotY(_FixedPitchY_Deg);
            var tcpLocalDelta = RotX(tx_deg) * RotY(ty_deg) * RotZ(tz_deg);
            var targetTcpRotation = currentTcpRotation * tcpLocalDelta;
            var targetMachineRotation = targetTcpRotation * RotY(-_FixedPitchY_Deg);
            var targetAngles = ExtractMachineAngles(targetMachineRotation);

            return CalculateRTCPTargetByMachineAngles(currentPose, targetAngles[1], targetAngles[2], targetAngles[0]);
        }

        public void SetFixedPitch(double fixedPitchDeg)
        {
            _FixedPitchY_Deg = fixedPitchDeg;
        }
        #endregion
    }
}
