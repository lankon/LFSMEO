using System;

using Matrox.MatroxImagingLibrary;

using MILX_ImageFunction;

namespace AAMachine.Logic.ImageMethod
{
    public class FindPositiveMirrorHousing
    {
        #region parameter define
        private string SavePath = "D:\\ProcessingImage\\0.PositiveCCD_MirrorHousingResult\\";
        public ResultInfo Result { get; } = new ResultInfo();
        #endregion

        #region private function
        public class ResultInfo
        {
            public double MirrorHousingDistance { get; set; } = double.NaN;             // 間隔距離
            public double MirrorHousingSignedAngle { get; set; } = double.NaN;          // 角度
            public MilVisionTool.EdgeResult HousingEdge { get; set; } = new MilVisionTool.EdgeResult(); // Housing邊緣
            public MilVisionTool.EdgeResult MirrorEdge { get; set; } = new MilVisionTool.EdgeResult();  // Mirror邊緣
            public bool IsSuccess { get; set; } = false;                                // 是否成功
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

        private LineInfo ToLineInfo(MilVisionTool.EdgeResult edgeResult)
        {
            return new LineInfo
            {
                StartX = edgeResult.StartX,
                StartY = edgeResult.StartY,
                EndX = edgeResult.EndX,
                EndY = edgeResult.EndY,
                CenterX = (edgeResult.StartX + edgeResult.EndX) / 2.0,
                CenterY = (edgeResult.StartY + edgeResult.EndY) / 2.0,
            };
        }

        private void ResetParam()
        {
            Result.MirrorHousingDistance = double.NaN;
            Result.MirrorHousingSignedAngle = double.NaN;
            Result.HousingEdge = new MilVisionTool.EdgeResult();
            Result.MirrorEdge = new MilVisionTool.EdgeResult();
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
            double minScore = 0.001)
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

            if (bestEdgeResult.Score <= minScore)
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
        public void FindPositiveMirrorHousingEdge(string sourceFile)
        {
            ResetParam();

            using (MilVisionTool func = new MilVisionTool())
            {
                MIL_ID source_image = MIL.M_NULL;

                try
                {
                    source_image = func.ImportImage(sourceFile);
                    FindPositiveMirrorHousingEdge(func, source_image);
                }
                finally
                {
                    func.SafeMilBufFree(ref source_image);
                }
            }
        }

        public void FindPositiveMirrorHousingEdge(MIL_ID source_image)
        {
            ResetParam();

            if (source_image == MIL.M_NULL)
                return;

            using (MilVisionTool func = new MilVisionTool())
                FindPositiveMirrorHousingEdge(func, source_image);
        }

        private void FindPositiveMirrorHousingEdge(MilVisionTool func, MIL_ID source_image)
        {
            if (func == null || source_image == MIL.M_NULL)
                return;

            {
                MIL_ID source_img = MIL.M_NULL;
                MIL_ID draw_img = MIL.M_NULL;
                MIL_ID draw_res_img = MIL.M_NULL;

                try
                {
                    func.CloneImage(source_image, ref source_img);
                    func.CloneImage(source_image, ref draw_img);

                    source_img = func.BinaryImage(source_img, new MilVisionTool.BinaryParameters { Method = MilVisionTool.BinaryMethod.DOMINANT_AND_GREATER });
                    source_img = func.OpenImage(source_img, new MilVisionTool.OpenParameters { Iteration = 3 });
                    source_img = func.CloseImage(source_img, new MilVisionTool.CloseParameters { Iteration = 3 });
                    func.ExportImage(source_img, $"{SavePath}\\PreImage.bmp", MIL.M_BMP);

                    #region Blob Detection
                    MilVisionTool.BlobDetectionResult blob_result = func.BlobDetect(source_img, new MilVisionTool.BlobDetectParameters
                    {
                        EnableAreaFilter = true,
                        AreaFilter = MilVisionTool.BlobAreaFilter.IN_RANGE,
                        FilterOperater = MilVisionTool.FilterOperater.INCLUDE_ONLY,
                        MinArea = 30000,
                        MaxArea = 50000,

                        SaveResultImage = true,
                        SavePath = $"{SavePath}\\BlobResult.bmp"
                    });

                    int BlobIndex1 = -1, BlobIndex2 = -1;
                    for (int i = 0; i < blob_result.BlobCount; i++)
                    {
                        if (Math.Abs(blob_result.Blobs[i].CenterX - 911) < 50 &&
                            Math.Abs(blob_result.Blobs[i].CenterY - 892) < 50)
                            BlobIndex1 = i;

                        if (Math.Abs(blob_result.Blobs[i].CenterX - 1468) < 50 &&
                            Math.Abs(blob_result.Blobs[i].CenterY - 1482) < 50)
                            BlobIndex2 = i;
                    }

                    if (BlobIndex1 == -1 || BlobIndex2 == -1)
                        return;
                    #endregion

                    #region Housing Edge Detection
                    double baseCenterX = (blob_result.Blobs[BlobIndex1].CenterX + blob_result.Blobs[BlobIndex2].CenterX) / 2.0;
                    double baseCenterY = (blob_result.Blobs[BlobIndex1].CenterY + blob_result.Blobs[BlobIndex2].CenterY) / 2.0;

                    MilVisionTool.EdgeDetectParameters housingSearch = new MilVisionTool.EdgeDetectParameters
                    {
                        BoxCenterX = baseCenterX + 146,
                        BoxCenterY = baseCenterY - 134,
                        BoxAngle = 42.82,
                        BoxWidth = 190,
                        BoxHeight = 1070,
                        Polarity = MilVisionTool.EdgePolarity.POSITIVE_EDGE,
                    };
                    MilVisionTool.EdgeResult housingEdge = FindBestEdge(func, source_img, housingSearch);

                    if (!housingEdge.Success)
                        return;

                    Result.HousingEdge = housingEdge;

                    draw_res_img = func.DrawLine(draw_img, draw_res_img, new MilVisionTool.DrawLineParameters
                    {
                        StartX = (int)housingEdge.StartX,
                        StartY = (int)housingEdge.StartY,
                        EndX = (int)housingEdge.EndX,
                        EndY = (int)housingEdge.EndY,
                        //SavePath = $"{SavePath}\\Housing"
                    });
                    #endregion

                    #region Mirror Edge Detection
                    MilVisionTool.EdgeDetectParameters mirrorSearch = new MilVisionTool.EdgeDetectParameters
                    {
                        BoxCenterX = baseCenterX + 325,
                        BoxCenterY = baseCenterY - 369,
                        BoxAngle = 224.14,
                        BoxWidth = 455,
                        BoxHeight = 1070,
                        Polarity = MilVisionTool.EdgePolarity.NEGATIVE_EDGE,
                    };
                    MilVisionTool.EdgeResult mirrorEdge = FindBestEdge(func, source_img, mirrorSearch);

                    if (!mirrorEdge.Success)
                        return;

                    Result.MirrorEdge = mirrorEdge;
                    #endregion

                    LineInfo mirrorLine = ToLineInfo(mirrorEdge);
                    LineInfo housingLine = ToLineInfo(housingEdge);
                    Result.MirrorHousingSignedAngle = CalculateSignedAngle(mirrorLine, housingLine);
                    Result.MirrorHousingDistance = CalculatePointToLineDistance(housingLine.CenterX, housingLine.CenterY, mirrorLine);
                    Result.IsSuccess = true;

                    draw_res_img = func.DrawLine(draw_res_img, draw_res_img, new MilVisionTool.DrawLineParameters
                    {
                        StartX = (int)mirrorEdge.StartX,
                        StartY = (int)mirrorEdge.StartY,
                        EndX = (int)mirrorEdge.EndX,
                        EndY = (int)mirrorEdge.EndY,
                        SavePath = $"{SavePath}\\PositiveCCD_MirrorHousing_Result"
                    });
                    
                }
                finally
                {
                    func.SafeMilBufFree(ref source_img);
                    func.SafeMilBufFree(ref draw_img);
                    func.SafeMilBufFree(ref draw_res_img);
                }
            }
        }

        #endregion
    }
}
