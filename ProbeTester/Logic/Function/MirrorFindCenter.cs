using System;
using System.IO;

using Matrox.MatroxImagingLibrary;

namespace ProbeTester.Logic
{
    public class MirrorFindCenter : IDisposable
    {
        public MirrorFindCenter()
        {
            InitializeLocalMilSystem();
        }

        #region parameter define
        private MIL_ID localMilApplication = MIL.M_NULL;
        private MIL_ID localMilSystem = MIL.M_NULL;
        private bool disposed;
        public double RectangleWidth { get; set; } = 1270.0;
        public double RectangleHeight { get; set; } = 1270.0;
        public double Foreground { get; set; } = MIL.M_FOREGROUND_WHITE;
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
        private FindCenterResult ReadRectangleResult(MIL_ID rectangleShapeResult)
        {
            MIL_INT count = 0;

            MIL.MmodGetResult(
                rectangleShapeResult,
                MIL.M_DEFAULT,
                MIL.M_NUMBER,
                ref count);

            if (count <= 0)
                return CreateNotFoundResult();

            double centerX = 0.0;
            double centerY = 0.0;
            double angle = 0.0;
            double score = 0.0;

            MIL.MmodGetResult(rectangleShapeResult, 0, MIL.M_POSITION_X, ref centerX);
            MIL.MmodGetResult(rectangleShapeResult, 0, MIL.M_POSITION_Y, ref centerY);
            MIL.MmodGetResult(rectangleShapeResult, 0, MIL.M_ANGLE, ref angle);
            MIL.MmodGetResult(rectangleShapeResult, 0, MIL.M_SCORE, ref score);

            if (angle > 180.0)
                angle -= 360.0;

            return new FindCenterResult()
            {
                Found = true,
                CenterX = centerX,
                CenterY = centerY,
                Angle = angle,
                Score = score,
                Count = count,
            };
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

            MIL_ID rectangleShapeContext = MIL.M_NULL;
            MIL_ID rectangleShapeResult = MIL.M_NULL;

            try
            {
                MIL.MmodAlloc(
                    localMilSystem,
                    MIL.M_SHAPE_RECTANGLE,
                    MIL.M_DEFAULT,
                    ref rectangleShapeContext);

                MIL.MmodDefine(
                    rectangleShapeContext,
                    MIL.M_RECTANGLE,
                    Foreground,
                    RectangleWidth,
                    RectangleHeight,
                    MIL.M_DEFAULT,
                    MIL.M_DEFAULT);

                MIL.MmodAllocResult(
                    localMilSystem,
                    MIL.M_SHAPE_RECTANGLE,
                    ref rectangleShapeResult);

                MIL.MmodPreprocess(rectangleShapeContext, MIL.M_DEFAULT);
                MIL.MmodFind(rectangleShapeContext, image, rectangleShapeResult);

                return ReadRectangleResult(rectangleShapeResult);
            }
            finally
            {
                if (rectangleShapeResult != MIL.M_NULL)
                    MIL.MmodFree(rectangleShapeResult);

                if (rectangleShapeContext != MIL.M_NULL)
                    MIL.MmodFree(rectangleShapeContext);
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
