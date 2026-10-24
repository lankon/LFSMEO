using System;
using System.IO;

using Matrox.MatroxImagingLibrary;

namespace ProbeTester.Logic
{
    public class HousingFindConer : IDisposable
    {
        public HousingFindConer()
        {
            InitializeLocalMilSystem();
        }

        #region parameter define
        private MIL_ID localMilApplication = MIL.M_NULL;
        private MIL_ID localMilSystem = MIL.M_NULL;
        private bool disposed;
        private double VerticalEdgeBoxSizeX { get; set; } = 416.0;
        private double VerticalEdgeBoxSizeY { get; set; } = 1724.0;
        private double VerticalEdgeBoxCenterX { get; set; } = 1736.0;
        private double VerticalEdgeBoxCenterY { get; set; } = 1042.0;
        private double VerticalEdgeBoxAngle { get; set; } = 0.0;
        private double HorizontalEdgeBoxSizeX { get; set; } = 320.0;
        private double HorizontalEdgeBoxSizeY { get; set; } = 308.0;
        private double HorizontalEdgeBoxCenterX { get; set; } = 1646.0;
        private double HorizontalEdgeBoxCenterY { get; set; } = 1716.0;
        private double HorizontalEdgeBoxAngle { get; set; } = 270.0;
        public class FindCenterResult
        {
            public bool Found { get; set; }

            public double CenterX { get; set; }

            public double CenterY { get; set; }

            public double Angle { get; set; }

            public double Score { get; set; }

            public MIL_INT Count { get; set; }

        }
        #endregion

        #region private function
        private FindCenterResult ReadEdgeMarkerResult(MIL_ID verticalEdgeMarker, MIL_ID horizontalEdgeMarker)
        {
            MIL_INT verticalCount = 0;
            MIL_INT horizontalCount = 0;

            MIL.MmeasGetResult(verticalEdgeMarker, MIL.M_NUMBER, ref verticalCount);
            MIL.MmeasGetResult(horizontalEdgeMarker, MIL.M_NUMBER, ref horizontalCount);

            if (verticalCount <= 0 || horizontalCount <= 0)
                return CreateNotFoundResult();

            double verticalX = 0.0;
            double horizontalY = 0.0;
            double verticalScore = 0.0;
            double horizontalScore = 0.0;

            MIL.MmeasGetResult(verticalEdgeMarker, MIL.M_POSITION_X, ref verticalX);
            MIL.MmeasGetResult(horizontalEdgeMarker, MIL.M_POSITION_Y, ref horizontalY);
            MIL.MmeasGetResult(verticalEdgeMarker, MIL.M_SCORE, ref verticalScore);
            MIL.MmeasGetResult(horizontalEdgeMarker, MIL.M_SCORE, ref horizontalScore);

            return new FindCenterResult()
            {
                Found = true,
                CenterX = verticalX,
                CenterY = horizontalY,
                Angle = 0.0,
                Score = (verticalScore + horizontalScore) / 2.0,
                Count = verticalCount < horizontalCount ? verticalCount : horizontalCount,
            };
        }

        private void SetEdgeMarker(
            MIL_ID edgeMarker,
            double boxSizeX,
            double boxSizeY,
            double boxCenterX,
            double boxCenterY,
            double boxAngle)
        {
            MIL.MmeasSetMarker(edgeMarker, MIL.M_SEARCH_REGION_INPUT_UNITS, MIL.M_PIXEL, MIL.M_NULL);
            MIL.MmeasSetMarker(edgeMarker, MIL.M_BOX_ANGLE_REFERENCE, MIL.M_BOX_CENTER, MIL.M_NULL);
            MIL.MmeasSetMarker(edgeMarker, MIL.M_BOX_SIZE, boxSizeX, boxSizeY);
            MIL.MmeasSetMarker(edgeMarker, MIL.M_BOX_CENTER, boxCenterX, boxCenterY);
            MIL.MmeasSetMarker(edgeMarker, MIL.M_BOX_ANGLE, boxAngle, MIL.M_NULL);
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
                throw new ObjectDisposedException(nameof(MirrorFindCenter));
        }

        private void SafeMilBufFree(ref MIL_ID buffer)
        {
            if (buffer == MIL.M_NULL) return;

            MIL.MbufFree(buffer);
            buffer = MIL.M_NULL;
        }

        private FindCenterResult CreateNotFoundResult()
        {
            return new FindCenterResult()
            {
                Found = false,
                CenterX = 0.0,
                CenterY = 0.0,
                Angle = 0.0,
                Score = 0.0,
                Count = 0,
            };
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
        public FindCenterResult Find(byte[] source, int width, int height)
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

        public FindCenterResult Find(string sourceFile)
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

        public FindCenterResult Find(MIL_ID image)
        {
            ThrowIfDisposed();

            if (image == MIL.M_NULL)
                throw new ArgumentException("Source image is null.", nameof(image));

            MIL_ID edgeMeasMarker = MIL.M_NULL;
            MIL_ID edgeMeasMarker2 = MIL.M_NULL;

            try
            {
                MIL.MmeasAllocMarker(localMilSystem, MIL.M_EDGE, MIL.M_DEFAULT, ref edgeMeasMarker);
                MIL.MmeasAllocMarker(localMilSystem, MIL.M_EDGE, MIL.M_DEFAULT, ref edgeMeasMarker2);

                SetEdgeMarker(edgeMeasMarker, VerticalEdgeBoxSizeX, VerticalEdgeBoxSizeY, VerticalEdgeBoxCenterX, VerticalEdgeBoxCenterY, VerticalEdgeBoxAngle);
                MIL.MmeasFindMarker(MIL.M_DEFAULT, image, edgeMeasMarker, MIL.M_DEFAULT);

                SetEdgeMarker(edgeMeasMarker2, HorizontalEdgeBoxSizeX, HorizontalEdgeBoxSizeY, HorizontalEdgeBoxCenterX, HorizontalEdgeBoxCenterY, HorizontalEdgeBoxAngle);
                MIL.MmeasFindMarker(MIL.M_DEFAULT, image, edgeMeasMarker2, MIL.M_DEFAULT);

                return ReadEdgeMarkerResult(edgeMeasMarker, edgeMeasMarker2);
            }
            finally
            {
                if (edgeMeasMarker2 != MIL.M_NULL)
                    MIL.MmeasFree(edgeMeasMarker2);

                if (edgeMeasMarker != MIL.M_NULL)
                    MIL.MmeasFree(edgeMeasMarker);
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



