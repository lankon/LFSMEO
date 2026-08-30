using System;
using System.IO;

using Matrox.MatroxImagingLibrary;

namespace AAMachine.Logic.ImageMethod
{
    public class FindHousingFeature : IDisposable
    {
        public FindHousingFeature()
        {
            InitializeLocalMilSystem();
        }

        #region parameter define
        private MIL_ID localMilApplication = MIL.M_NULL;
        private MIL_ID localMilSystem = MIL.M_NULL;
        private bool disposed;
        private class BlobResult
        {
            public double CenterX { get; set; } = 0.0;
            public double CenterY { get; set; } = 0.0;
            public double Area { get; set; } = 0.0;
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
        public double BinarizeThreshold { get; set; } = 90.0;  // Threshold value for the binarization operation
        public int CloseIterations { get; set; } = 6;       // Number of iterations for the morphological close operation
        public double MinBlobArea { get; set; } = 1000;     // Minimum area of the blob to be considered as a valid feature
        public class FindCenterResult
        {
            public bool Found { get; set; }

            public double CenterX { get; set; }

            public double CenterY { get; set; }

            public double Angle { get; set; }

            public double Score { get; set; }

            public MIL_INT Count { get; set; }

            public double BlobArea { get; set; }

            public double Left { get; set; }

            public double Top { get; set; }

            public double Right { get; set; }

            public double Bottom { get; set; }
        }
        #endregion

        #region private function
        private BlobResult CalculateBlobResult(MIL_ID blobResult)
        {
            MIL_INT count = 0;
            double numFound = 0;
            MIL.MblobGetResult(blobResult, MIL.M_GENERAL, MIL.M_NUMBER, ref numFound);
            count = (MIL_INT)numFound;

            if (count <= 0)
                return new BlobResult();

            MIL_INT target_index = 0;
            double centerX = 0.0;
            double centerY = 0.0;
            double area = 0.0;
            bool success = false;

            for (MIL_INT index = 0; index < numFound; index++)
            {
                MIL.MblobGetResult(blobResult, MIL.M_BLOB_INDEX(index), MIL.M_AREA, ref area);
                MIL.MblobGetResult(blobResult, MIL.M_BLOB_INDEX(index), MIL.M_CENTER_OF_GRAVITY_X, ref centerX);
                MIL.MblobGetResult(blobResult, MIL.M_BLOB_INDEX(index), MIL.M_CENTER_OF_GRAVITY_Y, ref centerY);

                if (Math.Abs(centerX - 236) < 50 && Math.Abs(centerY - 770) < 50 && area >40000)
                {
                    success = true;
                    break;
                }
            }

            if (!success)
                return new BlobResult();
            else
            {
                return new BlobResult
                {
                    CenterX = centerX,
                    CenterY = centerY,
                    Area = area
                };
            }
        }
        private EdgeResult CalculateEdgeResult(MIL_ID image, double centerX, double centerY, double angle)
        {
            MIL_ID EdgeMeasMarker = MIL.M_NULL;

            try
            {
                MIL.MmeasAllocMarker(localMilSystem, MIL.M_EDGE, MIL.M_DEFAULT, ref EdgeMeasMarker);

                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_SEARCH_REGION_INPUT_UNITS, MIL.M_PIXEL, MIL.M_NULL);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_ANGLE_REFERENCE, MIL.M_BOX_CENTER, MIL.M_NULL);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_SIZE, 85.000000477075, 312.0);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_CENTER, centerX, centerY);
                MIL.MmeasSetMarker(EdgeMeasMarker, MIL.M_BOX_ANGLE, angle, MIL.M_NULL);

                MIL.MmeasFindMarker(MIL.M_DEFAULT, image, EdgeMeasMarker, MIL.M_DEFAULT);

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

        private MIL_ID CloneImageBuffer(MIL_ID source)
        {
            MIL_ID destination = MIL.M_NULL;

            MIL.MbufClone(
                source,
                MIL.M_DEFAULT,
                MIL.M_DEFAULT,
                MIL.M_DEFAULT,
                MIL.M_DEFAULT,
                MIL.M_DEFAULT,
                MIL.M_DEFAULT,
                ref destination);

            MIL.MbufClear(destination, 0.0);

            return destination;
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

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(FindHousingFeature));
        }

        private void SafeMilBufFree(ref MIL_ID buffer)
        {
            if (buffer == MIL.M_NULL) return;

            MIL.MbufFree(buffer);
            buffer = MIL.M_NULL;
        }

        private MIL_ID ImportImage(string file)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentException("Image file is empty.", nameof(file));

            if (!File.Exists(file))
                throw new FileNotFoundException("Image file not found.", file);

            MIL_ID image = MIL.M_NULL;

            MIL.MbufImport(
                file,
                MIL.M_DEFAULT,
                MIL.M_RESTORE + MIL.M_NO_GRAB + MIL.M_NO_COMPRESS,
                localMilSystem,
                ref image);

            return image;
        }
        #endregion

        #region public function
        public EdgeResult Find(byte[] source, int width, int height)
        {
            ThrowIfDisposed();

            if (source == null) throw new ArgumentNullException(nameof(source));
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            MIL_ID image = MIL.M_NULL;

            try
            {
                image = ByteToMil(source, width, height);

                return Find(image);
            }
            finally
            {
                SafeMilBufFree(ref image);
            }
        }

        public EdgeResult Find(string sourceFile)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(sourceFile))
                throw new ArgumentException("Source file is empty.", nameof(sourceFile));

            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Source file not found.", sourceFile);

            MIL_ID image = MIL.M_NULL;

            try
            {
                image = ImportImage(sourceFile);

                return Find(image);
            }
            finally
            {
                SafeMilBufFree(ref image);
            }
        }

        public EdgeResult Find(MIL_ID image)
        {
            ThrowIfDisposed();

            if (image == MIL.M_NULL)
                throw new ArgumentException("Source image is null.", nameof(image));

            MIL_ID binarizeDestination = MIL.M_NULL;
            MIL_ID closeDestination = MIL.M_NULL;
            MIL_ID blobContext = MIL.M_NULL;
            MIL_ID blobResult = MIL.M_NULL;

            try
            {
                // Blob Calculate
                binarizeDestination = CloneImageBuffer(image);
                closeDestination = CloneImageBuffer(binarizeDestination);

                MIL.MblobAlloc(localMilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref blobContext);
                MIL.MblobControl(blobContext, MIL.M_BOX, MIL.M_ENABLE);
                MIL.MblobControl(blobContext, MIL.M_CENTER_OF_GRAVITY, MIL.M_ENABLE);
                MIL.MblobAllocResult(localMilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref blobResult);

                MIL.MimBinarize(image, binarizeDestination, MIL.M_FIXED + MIL.M_GREATER, BinarizeThreshold, MIL.M_NULL);
                MIL.MimClose(binarizeDestination, closeDestination, CloseIterations, MIL.M_GRAYSCALE);
                MIL.MblobCalculate(blobContext, closeDestination, MIL.M_NULL, blobResult);

                if (MinBlobArea > 0.0)
                    MIL.MblobSelect(blobResult, MIL.M_DELETE, MIL.M_AREA, MIL.M_LESS, MinBlobArea, MIL.M_NULL);

                var blob_res = CalculateBlobResult(blobResult);

                // Calculate Edge
                double score = 0.1;
                EdgeResult bestEdgeResult = new EdgeResult();
                for (int i = -10; i<= 10; i++)
                {
                    double angle = i * 0.1;
                    EdgeResult edge_res = CalculateEdgeResult(closeDestination, blob_res.CenterX + 25, blob_res.CenterY, angle);
                    
                    if(edge_res.Score > score)
                    {
                        score = edge_res.Score;
                        bestEdgeResult = edge_res;
                    }
                }
                
                return bestEdgeResult;
                
            }
            finally
            {
                if (blobResult != MIL.M_NULL)
                    MIL.MblobFree(blobResult);

                if (blobContext != MIL.M_NULL)
                    MIL.MblobFree(blobContext);

                SafeMilBufFree(ref closeDestination);
                SafeMilBufFree(ref binarizeDestination);
            }
        }

        public MIL_ID ByteToMil(byte[] source, int width, int height)
        {
            ThrowIfDisposed();

            if (source == null) throw new ArgumentNullException(nameof(source));
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            MIL_ID image = MIL.M_NULL;

            MIL.MbufAlloc2d(
                localMilSystem,
                width,
                height,
                8 + MIL.M_UNSIGNED,
                MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP,
                ref image);

            MIL.MbufPut(image, source);

            return image;
        }

        public void ExportImage(byte[] source, int width, int height, string outputFile, long fileFormat = MIL.M_TIFF)
        {
            ThrowIfDisposed();

            if (source == null) throw new ArgumentNullException(nameof(source));
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            MIL_ID image = MIL.M_NULL;

            try
            {
                image = ByteToMil(source, width, height);

                ExportImage(image, outputFile, fileFormat);
            }
            finally
            {
                SafeMilBufFree(ref image);
            }
        }

        public void ExportImage(MIL_ID image, string outputFile, long fileFormat = MIL.M_TIFF)
        {
            ThrowIfDisposed();

            if (image == MIL.M_NULL)
                throw new ArgumentException("Source image is null.", nameof(image));

            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentException("Output file is empty.", nameof(outputFile));

            string directory = Path.GetDirectoryName(outputFile);

            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            MIL.MbufExport(outputFile, fileFormat, image);
        }

        public void Dispose()
        {
            if (disposed) return;

            ReleaseLocalMilSystem();
            disposed = true;

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
