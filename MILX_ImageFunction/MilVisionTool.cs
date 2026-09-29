using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;

namespace MILX_ImageFunction
{
    public class MilVisionTool : IDisposable
    {
        private MIL_ID localMilApplication = MIL.M_NULL;
        private MIL_ID localMilSystem = MIL.M_NULL;

        public MilVisionTool()
        {
            InitializeLocalMilSystem();
        }

        ~MilVisionTool()
        {
            Dispose();
        }

        public void Dispose()
        {
            ReleaseLocalMilSystem();
        }

        #region Tool Functions
        public MIL_ID ImportImage(string file)
        {
            MIL_ID image = MIL.M_NULL;

            if (!File.Exists(file))
                return image;

            MIL.MbufImport(
                file,
                MIL.M_DEFAULT,
                MIL.M_RESTORE + MIL.M_NO_GRAB + MIL.M_NO_COMPRESS,
                localMilSystem,
                ref image);

            return image;
        }

        public void CloneImage(MIL_ID source, ref MIL_ID destination)
        {
            if (source == MIL.M_NULL)
                return;

            MIL_ID oldDestination = destination;
            MIL_ID newDestination = MIL.M_NULL;

            MIL.MbufClone(source, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT,
                MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, ref newDestination);

            MIL.MbufCopy(source, newDestination);

            SafeMilBufFree(ref oldDestination);
            destination = newDestination;
        }

        public MIL_ID CloneToRGBImage(MIL_ID source_img, MIL_ID resultImage)
        {
            MIL_ID sysId = (MIL_ID)MIL.MbufInquire(source_img, MIL.M_OWNER_SYSTEM, MIL.M_NULL);

            int width = 0;
            int height = 0;
            int band = 0;

            MIL.MbufInquire(source_img, MIL.M_SIZE_X, ref width);
            MIL.MbufInquire(source_img, MIL.M_SIZE_Y, ref height);
            MIL.MbufInquire(source_img, MIL.M_SIZE_BAND, ref band);

            MIL_ID redBand = MIL.M_NULL;
            MIL_ID greenBand = MIL.M_NULL;
            MIL_ID blueBand = MIL.M_NULL;

            try
            {
                MIL.MbufAllocColor(
                    sysId,
                    3,
                    width,
                    height,
                    8 + MIL.M_UNSIGNED,
                    MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP,
                    ref resultImage);

                MIL.MbufChildColor(resultImage, MIL.M_RED, ref redBand);
                MIL.MbufChildColor(resultImage, MIL.M_GREEN, ref greenBand);
                MIL.MbufChildColor(resultImage, MIL.M_BLUE, ref blueBand);

                MIL.MbufCopy(source_img, redBand);
                MIL.MbufCopy(source_img, greenBand);
                MIL.MbufCopy(source_img, blueBand);
            }
            finally
            {
                SafeMilBufFree(ref redBand);
                SafeMilBufFree(ref greenBand);
                SafeMilBufFree(ref blueBand);
            }

            return resultImage;
        }

        public MIL_ID CropImage(MIL_ID source_img, ref MIL_ID result_img, int roiX, int roiY, int roiW, int roiH)
        {
            MIL_ID sysId = (MIL_ID)MIL.MbufInquire(source_img, MIL.M_OWNER_SYSTEM, MIL.M_NULL);
            MIL_ID cropped = MIL.M_NULL;

            MIL.MbufAllocColor(
                sysId,
                1,
                roiW,
                roiH,
                8 + MIL.M_UNSIGNED,
                MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP,
                ref cropped);

            MIL.MbufCopyColor2d(
                source_img,
                cropped,
                MIL.M_ALL_BANDS,
                roiX,
                roiY,
                MIL.M_ALL_BANDS,
                0,
                0,
                roiW,
                roiH);

            CloneImage(cropped, ref result_img);
            SafeMilBufFree(ref cropped);

            return result_img;
        }

        public void ExportImage(MIL_ID image, string outputFile, long fileFormat)
        {
            if (image == MIL.M_NULL)
                return;

            try
            {
                string directory = Path.GetDirectoryName(outputFile);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                MIL.MbufExport(outputFile, fileFormat, image);
            }
            catch
            {
                // Debug 輸圖失敗時不影響主流程。
            }
        }

        public void SafeMilBufFree(ref MIL_ID buffer)
        {
            if (buffer == MIL.M_NULL) return;

            MIL.MbufFree(buffer);
            buffer = MIL.M_NULL;
        }

        private void InitializeLocalMilSystem()
        {
            if (localMilApplication == MIL.M_NULL)
                MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref localMilApplication);

            if (localMilSystem == MIL.M_NULL)
                MIL.MsysAlloc(localMilApplication, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref localMilSystem);
        }

        private void ReleaseLocalMilSystem()
        {
            if (localMilSystem != MIL.M_NULL)
            {
                MIL.MsysFree(localMilSystem);
                localMilSystem = MIL.M_NULL;
            }

            if (localMilApplication != MIL.M_NULL)
            {
                MIL.MappFree(localMilApplication);
                localMilApplication = MIL.M_NULL;
            }
        }
        #endregion

        #region Draw Functions
        public class DrawLineParameters
        {
            public int StartX { get; set; } = 0;
            public int StartY { get; set; } = 0;
            public int EndX { get; set; } = 0;
            public int EndY { get; set; } = 0;
            public string SavePath { get; set; } = string.Empty;
        }

        public MIL_ID DrawLine(MIL_ID source_img, MIL_ID resultImage, DrawLineParameters param)
        {
            if (source_img == MIL.M_NULL || param == null)
                return resultImage;

            MIL_ID graCtx = MIL.M_NULL;

            try
            {
                int width = 0;
                int height = 0;
                int band = 0;
                MIL_ID sysId = (MIL_ID)MIL.MbufInquire(source_img, MIL.M_OWNER_SYSTEM, MIL.M_NULL);

                MIL.MbufInquire(source_img, MIL.M_SIZE_X, ref width);
                MIL.MbufInquire(source_img, MIL.M_SIZE_Y, ref height);
                MIL.MbufInquire(source_img, MIL.M_SIZE_BAND, ref band);

                if (width <= 0 || height <= 0)
                    return resultImage;

                if (resultImage == MIL.M_NULL)
                {
                    if (band >= 3)
                        CloneImage(source_img, ref resultImage);      // 如果是彩色影像，直接複製 source_img 到 resultImage
                    else
                        resultImage = CloneToRGBImage(source_img, resultImage); // 將灰階影像轉為 RGB 影像，以便繪製彩色線條
                }

                MIL.MgraAlloc(sysId, ref graCtx);
                MIL.MgraControl(graCtx, MIL.M_LINE_THICKNESS, 3);
                MIL.MgraControl(graCtx, MIL.M_COLOR, MIL.M_COLOR_GREEN);
                MIL.MgraLine(graCtx, resultImage, param.StartX, param.StartY, param.EndX, param.EndY);

                if (param.SavePath != string.Empty)
                    ExportImage(resultImage, param.SavePath + ".bmp", MIL.M_BMP);
            }
            finally
            {
                if (graCtx != MIL.M_NULL)
                    MIL.MgraFree(graCtx);

                //SafeMilBufFree(ref resultImage);
            }

            return resultImage;
        }
        #endregion

        #region Binary Image
        public enum BinaryMethod
        {
            FIX_AND_GREATER,            // 固定閾值二值化,適用影像亮度固定
            DOMINANT_AND_GREATER,       // 
            PERCENTILE_AND_GREATER,
            BIMODAL_AND_GREATER,
        }

        public class BinaryParameters
        {
            public BinaryMethod Method { get; set; } = BinaryMethod.DOMINANT_AND_GREATER;
            public double ThresholdValue { get; set; } = 0.0;
        }

        public MIL_ID BinaryImage(MIL_ID sourceImage, BinaryParameters param)
        {
            if(param.Method == BinaryMethod.FIX_AND_GREATER)
                MIL.MimBinarize(sourceImage, sourceImage, MIL.M_FIXED + MIL.M_GREATER, param.ThresholdValue, MIL.M_NULL);
            else if(param.Method == BinaryMethod.DOMINANT_AND_GREATER)
                MIL.MimBinarize(sourceImage, sourceImage, MIL.M_DOMINANT + MIL.M_GREATER, MIL.M_NULL, MIL.M_NULL);
            else if(param.Method == BinaryMethod.PERCENTILE_AND_GREATER)
                MIL.MimBinarize(sourceImage, sourceImage, MIL.M_PERCENTILE_VALUE + MIL.M_GREATER, param.ThresholdValue, MIL.M_NULL);
            else if(param.Method == BinaryMethod.BIMODAL_AND_GREATER)
                MIL.MimBinarize(sourceImage, sourceImage, MIL.M_BIMODAL + MIL.M_GREATER, 0.0, 255.0);

            return sourceImage;
        }
        #endregion

        #region Open Image
        public class OpenParameters
        {
            public int Iteration { get; set; } = 3;
        }

        public MIL_ID OpenImage(MIL_ID sourceImage, OpenParameters param)
        {
            MIL.MimOpen(sourceImage, sourceImage, param.Iteration, MIL.M_GRAYSCALE);
            
            return sourceImage;
        }
        #endregion

        #region Close Image
        public class CloseParameters
        {
            public int Iteration { get; set; } = 3;
        }

        public MIL_ID CloseImage(MIL_ID sourceImage, CloseParameters param)
        {
            MIL.MimClose(sourceImage, sourceImage, param.Iteration, MIL.M_GRAYSCALE);

            return sourceImage;
        }
        #endregion

        #region Blob Analysis
        public enum BlobAreaFilter
        {
            NONE,
            LESS_OR_EQUAL,
            IN_RANGE,
            //GREATER,
            //GREATER_OR_EQUAL,
        }

        public enum FilterOperater
        {
            DELETE,         
            INCLUDE,        
            INCLUDE_ONLY,   // 只保留符合條件的 blob，其他全部刪除
        }

        public class BlobDetectParameters
        {
            // 面積過濾
            public bool EnableAreaFilter { get; set; } = false;
            public BlobAreaFilter AreaFilter { get; set; } = BlobAreaFilter.NONE;
            public FilterOperater FilterOperater { get; set; } = FilterOperater.INCLUDE;
            public double MinArea { get; set; }
            public double MaxArea { get; set; }

            // 儲存結果影像
            public bool SaveResultImage { get; set; } = false;
            public string SavePath { get; set; } = string.Empty;
        }

        public class BlobInfo
        {
            public int CenterX { get; set; }
            public int CenterY { get; set; }
            public double Area { get; set; }
            //public double BoxMinX { get; set; }
            //public double BoxMinY { get; set; }
            //public double BoxMaxX { get; set; }
            //public double BoxMaxY { get; set; }
        }

        public class BlobDetectionResult
        {
            public List<BlobInfo> Blobs { get; set; } = new List<BlobInfo>();
            public int BlobCount => Blobs.Count;
        }

        public BlobDetectionResult BlobDetect(MIL_ID blobImage, BlobDetectParameters param)
        {
            MIL_ID blobContext = MIL.M_NULL;
            MIL_ID blobResult = MIL.M_NULL;
            int blobNum = 0;

            MIL.MblobAlloc(localMilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref blobContext);
            MIL.MblobAllocResult(localMilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref blobResult);

            MIL.MblobControl(blobContext, MIL.M_CENTER_OF_GRAVITY + MIL.M_BINARY, MIL.M_ENABLE);
            MIL.MblobControl(blobContext, MIL.M_BOX, MIL.M_ENABLE);

            MIL.MblobCalculate(blobContext, blobImage, MIL.M_NULL, blobResult);

            // 面積過濾
            if(param.EnableAreaFilter)
            {
                long operation = MIL.M_INCLUDE;

                if(param.FilterOperater == FilterOperater.DELETE)
                    operation = MIL.M_DELETE;
                else if(param.FilterOperater == FilterOperater.INCLUDE)
                    operation = MIL.M_INCLUDE;
                else if (param.FilterOperater == FilterOperater.INCLUDE_ONLY)
                    operation = MIL.M_INCLUDE_ONLY;

                if (param.AreaFilter == BlobAreaFilter.LESS_OR_EQUAL)
                    MIL.MblobSelect(blobResult, operation, MIL.M_AREA, MIL.M_LESS_OR_EQUAL, param.MinArea, 0);
                else if (param.AreaFilter == BlobAreaFilter.IN_RANGE)
                    MIL.MblobSelect(blobResult, operation, MIL.M_AREA, MIL.M_IN_RANGE, param.MinArea, param.MaxArea);
            }
            
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref blobNum);

            BlobDetectionResult BlobRes = new BlobDetectionResult();
            if (blobNum <= 0)    // 沒有找到任何 blob
            {
                if (blobResult != MIL.M_NULL)
                    MIL.MblobFree(blobResult);

                if (blobContext != MIL.M_NULL)
                    MIL.MblobFree(blobContext);

                return BlobRes;
            }

            int[] blobX = new int[blobNum];
            int[] blobY = new int[blobNum];
            double[] boxArea = new double[blobNum];
            double[] boxMinX = new double[blobNum];
            double[] boxMinY = new double[blobNum];
            double[] boxMaxX = new double[blobNum];
            double[] boxMaxY = new double[blobNum];

            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_CENTER_OF_GRAVITY_X + MIL.M_TYPE_LONG, blobX);
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_CENTER_OF_GRAVITY_Y + MIL.M_TYPE_LONG, blobY);
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_AREA + MIL.M_TYPE_DOUBLE, boxArea);
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_BOX_X_MIN + MIL.M_TYPE_DOUBLE, boxMinX);
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_BOX_Y_MIN + MIL.M_TYPE_DOUBLE, boxMinY);
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_BOX_X_MAX + MIL.M_TYPE_DOUBLE, boxMaxX);
            MIL.MblobGetResult(blobResult, MIL.M_DEFAULT, MIL.M_BOX_Y_MAX + MIL.M_TYPE_DOUBLE, boxMaxY);

            for (int i = 0; i < blobNum; i++)
            {
                BlobRes.Blobs.Add(new BlobInfo
                {
                    CenterX = blobX[i],
                    CenterY = blobY[i],
                    Area = boxArea[i],
                    //BoxMinX = boxMinX[i],
                    //BoxMinY = boxMinY[i],
                    //BoxMaxX = boxMaxX[i],
                    //BoxMaxY = boxMaxY[i]
                });
            }

            if (param.SaveResultImage)
                SaveBlobResultImage(blobImage, blobResult, param.SavePath);

            if (blobResult != MIL.M_NULL)
                MIL.MblobFree(blobResult);

            if (blobContext != MIL.M_NULL)
                MIL.MblobFree(blobContext);

            return BlobRes;
        }
        
        private void SaveBlobResultImage(MIL_ID source_img, MIL_ID blobResult, string savePath)
        {
            MIL_ID resultImage = MIL.M_NULL;
            MIL.MbufClone(source_img, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, ref resultImage);
            MIL.MbufCopy(source_img, resultImage);
            MIL.MgraControl(MIL.M_DEFAULT, MIL.M_LINE_THICKNESS, 3);
            MIL.MgraControl(MIL.M_DEFAULT, MIL.M_COLOR, MIL.M_COLOR_GREEN);
            MIL.MblobDraw(MIL.M_DEFAULT, blobResult, resultImage, MIL.M_DRAW_BOX, MIL.M_INCLUDED_BLOBS, MIL.M_DEFAULT);
            MIL.MgraControl(MIL.M_DEFAULT, MIL.M_COLOR, MIL.M_COLOR_LIGHT_BLUE);
            MIL.MblobDraw(MIL.M_DEFAULT, blobResult, resultImage, MIL.M_DRAW_CENTER_OF_GRAVITY, MIL.M_INCLUDED_BLOBS, MIL.M_DEFAULT);
            MIL.MgraControl(MIL.M_DEFAULT, MIL.M_LINE_THICKNESS, MIL.M_DEFAULT);
            ExportImage(resultImage, savePath, MIL.M_BMP);
            SafeMilBufFree(ref resultImage);
        }

        #endregion

        #region Edge Detection
        public class EdgeDetectParameters
        {
            // Box 設定
            public double BoxCenterX { get; set; } = 0.0;
            public double BoxCenterY { get; set; } = 0.0;
            public double BoxAngle { get; set; } = 0.0;
            public double BoxWidth { get; set; } = 0.0;
            public double BoxHeight { get; set; } = 0.0;

            // 極性判斷
            public EdgePolarity Polarity { get; set; } = EdgePolarity.ANY_EDGE;
        }

        public enum EdgePolarity
        {
            ANY_EDGE,
            POSITIVE_EDGE,
            NEGATIVE_EDGE,
        }

        public class EdgeResult
        {
            public double Score { get; set; } = 0.0;
            public double PositionX { get; set; } = 0.0;
            public double PositionY { get; set; } = 0.0;
            public double Angle { get; set; } = 0.0;

            // Line end points of the detected edge
            public double StartX { get; set; } = 0.0;
            public double StartY { get; set; } = 0.0;
            public double EndX { get; set; } = 0.0;
            public double EndY { get; set; } = 0.0;

            public bool Success = false;
        }

        public EdgeResult EdgeDetect(MIL_ID sourceImage, EdgeDetectParameters param)
        {
            MIL_ID EdgeMeasMarker = MIL.M_NULL;

            try
            {
                MIL.MmeasAllocMarker(localMilSystem, MIL.M_EDGE, MIL.M_DEFAULT, ref EdgeMeasMarker);

                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_POLARITY, ToMilPolarity(param.Polarity), MIL.M_DEFAULT);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_SEARCH_REGION_INPUT_UNITS, MIL.M_PIXEL, MIL.M_NULL);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_ANGLE_REFERENCE, MIL.M_BOX_CENTER, MIL.M_NULL);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_SIZE, param.BoxWidth, param.BoxHeight);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_CENTER, param.BoxCenterX, param.BoxCenterY);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_ANGLE, param.BoxAngle, MIL.M_NULL);

                MIL.MmeasFindMarker(MIL.M_DEFAULT, sourceImage, EdgeMeasMarker, MIL.M_DEFAULT);

                MIL_INT count = 0;
                MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref count);

                if (count > 0)
                {
                    double score = 0.0;
                    double positionX = 0.0;
                    double positionY = 0.0;
                    double resultAngle = 0.0;
                    double lineEndPointFirstX = 0.0;
                    double lineEndPointFirstY = 0.0;
                    double lineEndPointSecondX = 0.0;
                    double lineEndPointSecondY = 0.0;

                    MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_SCORE, ref score);
                    MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_POSITION_X, ref positionX);
                    MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_POSITION_Y, ref positionY);
                    MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_ANGLE, ref resultAngle);

                    MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_LINE_END_POINT_FIRST, ref lineEndPointFirstX,
                                                                                   ref lineEndPointFirstY);

                    MIL.MmeasGetResult(EdgeMeasMarker, MIL.M_LINE_END_POINT_SECOND, ref lineEndPointSecondX,
                                                                                    ref lineEndPointSecondY);

                    return new EdgeResult
                    {
                        Score = score,
                        PositionX = positionX,
                        PositionY = positionY,
                        Angle = resultAngle,
                        StartX = lineEndPointFirstX,
                        StartY = lineEndPointFirstY,
                        EndX = lineEndPointSecondX,
                        EndY = lineEndPointSecondY,
                        Success = true
                    };
                }

                return new EdgeResult();
            }
            finally
            {
                if (EdgeMeasMarker != MIL.M_NULL)
                    MIL.MmeasFree(EdgeMeasMarker);
            }

        }

        private int ToMilPolarity(EdgePolarity polarity)
        {
            switch (polarity)
            {
                case EdgePolarity.NEGATIVE_EDGE:
                    return MIL.M_NEGATIVE;
                case EdgePolarity.POSITIVE_EDGE:
                    return MIL.M_POSITIVE;
                case EdgePolarity.ANY_EDGE:
                    return MIL.M_ANY;
                default:
                    return MIL.M_ANY;
            }
        }
        #endregion
    }
}
