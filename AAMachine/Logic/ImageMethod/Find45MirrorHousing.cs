using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;

using MILX_ImageFunction;

namespace AAMachine.Logic.ImageMethod
{
    public class Find45MirrorHousing
    {
        #region parameter define
        private string SavePath = "D:\\ProcessingImage\\0.45CCD_MirrorHousingResult\\";
        public ResultInfo Result { get; } = new ResultInfo();
        #endregion

        #region private function
        public class ResultInfo
        {
            public double LeftMirrorHousingDistance { get; set; } = double.NaN;
            public double LeftMirrorHousingSignedAngle { get; set; } = double.NaN;
            public double DownMirrorHousingDistance { get; set; } = double.NaN;
            public double DownMirrorHousingSignedAngle { get; set; } = double.NaN;
            public bool IsSuccess { get; set; } = false;
        }

        private class LineInfo
        {
            public double StartX { get; set; }
            public double StartY { get; set; }
            public double EndX { get; set; }
            public double EndY { get; set; }
            public double CenterX { get; set; }
            public double CenterY { get; set; }
        }

        private LineInfo ToLineInfo(MilVisionTool.EdgeResult edgeResult, double offsetX = 0.0, double offsetY = 0.0)
        {
            double startX = edgeResult.StartX + offsetX;
            double startY = edgeResult.StartY + offsetY;
            double endX = edgeResult.EndX + offsetX;
            double endY = edgeResult.EndY + offsetY;

            return new LineInfo
            {
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY,
                CenterX = (startX + endX) / 2.0,
                CenterY = (startY + endY) / 2.0,
            };
        }

        private void ResetParam()
        {
            Result.LeftMirrorHousingSignedAngle = double.NaN;
            Result.LeftMirrorHousingDistance = double.NaN;
            Result.DownMirrorHousingSignedAngle = double.NaN;
            Result.DownMirrorHousingDistance = double.NaN;
            Result.IsSuccess = false;
        }

        private double CalculateLineAngle(LineInfo line)
        {
            return Math.Atan2(line.EndY - line.StartY, line.EndX - line.StartX) * 180.0 / Math.PI;
        }

        private double NormalizeLineAngle(double angle)
        {
            while (angle > 90.0)
                angle -= 180.0;

            while (angle <= -90.0)
                angle += 180.0;

            return angle;
        }

        private double CalculateSignedAngle(LineInfo referenceLine, LineInfo targetLine)
        {
            double referenceAngle = NormalizeLineAngle(CalculateLineAngle(referenceLine));
            double targetAngle = NormalizeLineAngle(CalculateLineAngle(targetLine));
            double angle = targetAngle - referenceAngle;

            while (angle > 90.0)
                angle -= 180.0;

            while (angle <= -90.0)
                angle += 180.0;

            return angle;
        }

        private double CalculatePointToLineDistance(double pointX, double pointY, LineInfo line)
        {
            double lineX = line.EndX - line.StartX;
            double lineY = line.EndY - line.StartY;
            double denominator = Math.Sqrt(lineX * lineX + lineY * lineY);

            if (denominator <= double.Epsilon)
                return double.NaN;

            double numerator = Math.Abs(lineY * pointX - lineX * pointY + line.EndX * line.StartY - line.EndY * line.StartX);
            return numerator / denominator;
        }

        private MilVisionTool.EdgeResult FindBestEdge(
            MilVisionTool func,
            MIL_ID source_img,
            MilVisionTool.EdgeDetectParameters searchParam,
            double angleStart = -2.0,
            double angleEnd = 2.0,
            double angleStep = 0.1,
            double minScore = 0.01)
        {
            if (func == null || source_img == MIL.M_NULL || searchParam == null || angleStep <= 0)
                return new MilVisionTool.EdgeResult();

            double score = minScore;
            MilVisionTool.EdgeResult bestEdgeResult = new MilVisionTool.EdgeResult();

            for (double angle = angleStart; angle <= angleEnd; angle += angleStep)
            {
                MilVisionTool.EdgeResult edge_res = func.EdgeDetect(source_img, new MilVisionTool.EdgeDetectParameters
                {
                    BoxCenterX = searchParam.BoxCenterX,
                    BoxCenterY = searchParam.BoxCenterY,
                    BoxAngle = NormalizeAngle(searchParam.BoxAngle + angle),
                    BoxWidth = searchParam.BoxWidth,
                    BoxHeight = searchParam.BoxHeight,
                    Polarity = searchParam.Polarity,
                });

                if (edge_res.Score > score)
                {
                    score = edge_res.Score;
                    bestEdgeResult = edge_res;
                }
            }

            if (bestEdgeResult.Score < 0.01)
                bestEdgeResult.Success = false;

            return bestEdgeResult;
        }
        private double NormalizeAngle(double angle)
        {
            while (angle < 0.0)
                angle += 360.0;

            while (angle >= 360.0)
                angle -= 360.0;

            return angle;
        }
        #endregion

        #region public function
        public void Find45MirrorHousingEdge(MIL_ID source_image)
        {
            ResetParam();

            if (source_image == MIL.M_NULL)
                return;

            using (MilVisionTool func = new MilVisionTool())
            {
                MIL_ID draw_res_img = MIL.M_NULL;
                MIL_ID draw_img = MIL.M_NULL;
                MIL_ID source_img_l = MIL.M_NULL;
                MIL_ID source_img_d = MIL.M_NULL;
                MIL_ID source_img_h_l = MIL.M_NULL;
                MIL_ID source_img_h_l_crop = MIL.M_NULL;
                MIL_ID source_img_h_d = MIL.M_NULL;
                MIL_ID source_img_h_d_crop = MIL.M_NULL;
                MilVisionTool.EdgeResult bestEdgeResult;
                LineInfo leftMirrorLine = null;
                LineInfo leftHousingLine = null;
                LineInfo downMirrorLine = null;
                LineInfo downHousingLine = null;

                try
                {
                    func.CloneImage(source_image, ref draw_img);

                    #region 搜尋Left Mirror位置
                    func.CloneImage(source_image, ref source_img_l);
                    func.BinaryImage(source_img_l, new MilVisionTool.BinaryParameters { Method = MilVisionTool.BinaryMethod.FIX_AND_GREATER, ThresholdValue = 128 });
                    func.CloseImage(source_img_l, new MilVisionTool.CloseParameters { Iteration = 5 });
                    func.ExportImage(source_img_l, $"{SavePath}\\LeftMirrorPreImage.bmp", MIL.M_BMP);

                    MilVisionTool.EdgeDetectParameters leftMirrorSearch = new MilVisionTool.EdgeDetectParameters
                    {
                        BoxCenterX = 341,
                        BoxCenterY = 764,
                        BoxAngle = 0,
                        BoxWidth = 150,
                        BoxHeight = 900,
                        Polarity = MilVisionTool.EdgePolarity.POSITIVE_EDGE,
                    };
                    bestEdgeResult = FindBestEdge(func, source_img_l, leftMirrorSearch);

                    if (!bestEdgeResult.Success)
                        return; // 找不到最佳邊緣，直接返回

                    leftMirrorLine = ToLineInfo(bestEdgeResult);

                    draw_res_img = func.DrawLine(draw_img, draw_res_img, new MilVisionTool.DrawLineParameters
                    {
                        StartX = (int)bestEdgeResult.StartX,
                        StartY = (int)bestEdgeResult.StartY,
                        EndX = (int)bestEdgeResult.EndX,
                        EndY = (int)bestEdgeResult.EndY,
                    });
                    #endregion

                    #region 搜尋Down Mirror位置
                    func.CloneImage(source_image, ref source_img_d);
                    func.BinaryImage(source_img_d, new MilVisionTool.BinaryParameters { Method = MilVisionTool.BinaryMethod.PERCENTILE_AND_GREATER, ThresholdValue = 70 });
                    func.CloseImage(source_img_d, new MilVisionTool.CloseParameters { Iteration = 5 });
                    func.ExportImage(source_img_d, $"{SavePath}\\DownMirrorPreImage.bmp", MIL.M_BMP);

                    MilVisionTool.EdgeDetectParameters mirrorSearch = new MilVisionTool.EdgeDetectParameters
                    {
                        BoxCenterX = 1168,
                        BoxCenterY = 1448,
                        BoxAngle = 89.99,
                        BoxWidth = 143,
                        BoxHeight = 687,
                        Polarity = MilVisionTool.EdgePolarity.POSITIVE_EDGE,
                    };
                    bestEdgeResult = FindBestEdge(func, source_img_d, mirrorSearch);

                    if (!bestEdgeResult.Success)
                        return; // 找不到最佳邊緣，直接返回

                    downMirrorLine = ToLineInfo(bestEdgeResult);

                    draw_res_img = func.DrawLine(draw_res_img, draw_res_img, new MilVisionTool.DrawLineParameters
                    {
                        StartX = (int)bestEdgeResult.StartX,
                        StartY = (int)bestEdgeResult.StartY,
                        EndX = (int)bestEdgeResult.EndX,
                        EndY = (int)bestEdgeResult.EndY,
                    });
                    #endregion

                    #region 搜尋Left Housing位置
                    int offsetX = 0, offsetY = 447;
                    func.CloneImage(source_image, ref source_img_h_l);
                    func.CropImage(source_img_h_l, ref source_img_h_l_crop, offsetX, offsetY, 2434, 598);
                    source_img_h_l_crop = func.BinaryImage(source_img_h_l_crop, new MilVisionTool.BinaryParameters { Method = MilVisionTool.BinaryMethod.PERCENTILE_AND_GREATER, ThresholdValue = 60 });
                    source_img_h_l_crop = func.CloseImage(source_img_h_l_crop, new MilVisionTool.CloseParameters { Iteration = 4 });
                    func.ExportImage(source_img_h_l_crop, $"{SavePath}\\LeftHousingPreImage.bmp", MIL.M_BMP);

                    #region Blob定位初始位置
                    MilVisionTool.BlobDetectionResult blob_result;
                    blob_result = func.BlobDetect(source_img_h_l_crop, new MilVisionTool.BlobDetectParameters
                    {
                        EnableAreaFilter = true,
                        AreaFilter = MilVisionTool.BlobAreaFilter.IN_RANGE,
                        FilterOperater = MilVisionTool.FilterOperater.INCLUDE_ONLY,
                        MinArea = 40000,
                        MaxArea = 60000,

                        SaveResultImage = true,
                        SavePath = $"{SavePath}\\BlobResult.bmp"
                    });

                    int BlobIndex1 = -1;
                    for (int i = 0; i < blob_result.BlobCount; i++)
                    {

                        if (Math.Abs(blob_result.Blobs[i].CenterX - 199) < 150 &&
                            Math.Abs(blob_result.Blobs[i].CenterY - 317) < 50)
                            BlobIndex1 = i;
                    }

                    if (BlobIndex1 == -1)
                        return; // 找不到目標Blob，直接返回

                    MilVisionTool.EdgeDetectParameters housingLeftSearch = new MilVisionTool.EdgeDetectParameters
                    {
                        BoxCenterX = blob_result.Blobs[BlobIndex1].CenterX,
                        BoxCenterY = blob_result.Blobs[BlobIndex1].CenterY,
                        BoxAngle = 0,
                        BoxWidth = 208,
                        BoxHeight = 368,
                        Polarity = MilVisionTool.EdgePolarity.NEGATIVE_EDGE,
                    };
                    bestEdgeResult = FindBestEdge(func, source_img_h_l_crop, housingLeftSearch);

                    if (!bestEdgeResult.Success)
                        return; // 找不到最佳邊緣，直接返回

                    leftHousingLine = ToLineInfo(bestEdgeResult, offsetX, offsetY);
                    Result.LeftMirrorHousingSignedAngle = CalculateSignedAngle(leftMirrorLine, leftHousingLine);
                    Result.LeftMirrorHousingDistance = CalculatePointToLineDistance(leftHousingLine.CenterX, leftHousingLine.CenterY, leftMirrorLine);

                    draw_res_img = func.DrawLine(draw_res_img, draw_res_img, new MilVisionTool.DrawLineParameters
                    {
                        StartX = (int)bestEdgeResult.StartX + offsetX,
                        StartY = (int)bestEdgeResult.StartY + offsetY,
                        EndX = (int)bestEdgeResult.EndX + offsetX,
                        EndY = (int)bestEdgeResult.EndY + offsetY,
                        //SavePath = $"{SavePath}\\LeftHosuingResult"
                    });
                    #endregion
                    #endregion

                    #region 搜尋Down Housing位置
                    int downOffsetX = 10, downOffsetY = 1630;
                    func.CloneImage(source_image, ref source_img_h_d);
                    func.CropImage(source_img_h_d, ref source_img_h_d_crop, downOffsetX, downOffsetY, 2432, 412);
                    func.BinaryImage(source_img_h_d_crop, new MilVisionTool.BinaryParameters { Method = MilVisionTool.BinaryMethod.PERCENTILE_AND_GREATER, ThresholdValue = 80 });
                    func.CloseImage(source_img_h_d_crop, new MilVisionTool.CloseParameters { Iteration = 5 });
                    func.ExportImage(source_img_h_d_crop, $"{SavePath}\\DownHousingPreImage.bmp", MIL.M_BMP);

                    MilVisionTool.EdgeDetectParameters housingDownSearch = new MilVisionTool.EdgeDetectParameters
                    {
                        BoxCenterX = 1170,
                        BoxCenterY = 137,
                        BoxAngle = 269.7,
                        BoxWidth = 235,
                        BoxHeight = 680,
                        Polarity = MilVisionTool.EdgePolarity.POSITIVE_EDGE,
                    };
                    bestEdgeResult = FindBestEdge(func, source_img_h_d_crop, housingDownSearch);

                    if (!bestEdgeResult.Success)
                        return; // 找不到最佳邊緣，直接返回

                    downHousingLine = ToLineInfo(bestEdgeResult, downOffsetX, downOffsetY);
                    Result.DownMirrorHousingSignedAngle = CalculateSignedAngle(downMirrorLine, downHousingLine);
                    Result.DownMirrorHousingDistance = CalculatePointToLineDistance(downHousingLine.CenterX, downHousingLine.CenterY, downMirrorLine);
                    Result.IsSuccess = true;

                    draw_res_img = func.DrawLine(draw_res_img, draw_res_img, new MilVisionTool.DrawLineParameters
                    {
                        StartX = (int)bestEdgeResult.StartX + downOffsetX,
                        StartY = (int)bestEdgeResult.StartY + downOffsetY,
                        EndX = (int)bestEdgeResult.EndX + downOffsetX,
                        EndY = (int)bestEdgeResult.EndY + downOffsetY,
                        SavePath = $"{SavePath}\\45CCD_MirrorHousing_Result"
                    });
                    #endregion
                }
                finally
                {
                    func.SafeMilBufFree(ref source_img_l);
                    func.SafeMilBufFree(ref source_img_d);

                    func.SafeMilBufFree(ref source_img_h_l);
                    func.SafeMilBufFree(ref source_img_h_l_crop);

                    func.SafeMilBufFree(ref source_img_h_d);
                    func.SafeMilBufFree(ref source_img_h_d_crop);

                    func.SafeMilBufFree(ref draw_img);
                    func.SafeMilBufFree(ref draw_res_img);
                }

            }
        }
        #endregion
    }
}
