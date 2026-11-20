using System;
using System.Collections.Generic;
using Matrox.MatroxImagingLibrary;

namespace AAMachine.Logic.ImageMethod
{
    public class CaptureLineProfile
    {
        #region parameter define
        public class LineProfileResult
        {
            public double CenterX { get; set; }            // Profile 線中心點 X 座標。

            public double CenterY { get; set; }            // Profile 線中心點 Y 座標。

            public double AngleDeg { get; set; }           // Profile 線角度，單位為度；影像座標中往右下為正角度。

            public double RequestedLength { get; set; }    // 原本要求的 Profile 線長度，尚未限制在影像範圍內。

            public double ActualLength { get; set; }       // 實際取樣的 Profile 線長度，已限制在影像範圍內。

            public double RequestedStartX { get; set; }    // 原本要求的起點 X 座標，尚未限制在影像範圍內。

            public double RequestedStartY { get; set; }    // 原本要求的起點 Y 座標，尚未限制在影像範圍內。

            public double RequestedEndX { get; set; }      // 原本要求的終點 X 座標，尚未限制在影像範圍內。

            public double RequestedEndY { get; set; }      // 原本要求的終點 Y 座標，尚未限制在影像範圍內。

            public int StartX { get; set; }                // 實際送給 MIL 取樣的起點 X 座標，已限制在影像範圍內。

            public int StartY { get; set; }                // 實際送給 MIL 取樣的起點 Y 座標，已限制在影像範圍內。

            public int EndX { get; set; }                  // 實際送給 MIL 取樣的終點 X 座標，已限制在影像範圍內。

            public int EndY { get; set; }                  // 實際送給 MIL 取樣的終點 Y 座標，已限制在影像範圍內。

            public int PixelBitDepth { get; set; }         // 來源影像的位元深度，目前支援 8-bit 或 16-bit。

            public byte[] RawValues8Bit { get; set; }      // 8-bit 影像的原始 Profile 灰階值；如果是 16-bit 影像則為 null。

            public ushort[] RawValues16Bit { get; set; }   // 16-bit 影像的原始 Profile 灰階值；如果是 8-bit 影像則為 null。

            public double[] Values { get; set; }           // 轉成 double 的 Profile 灰階值，用來計算 crossing 和線段。
        }
        public class LineProfileCrossing
        {
            public double Index { get; set; }

            public double X { get; set; }

            public double Y { get; set; }

            public double Distance { get; set; }

            public double Value { get; set; }

            public CrossingPolarity Polarity { get; set; }
        }
        public class LineProfileSegment
        {
            public double StartIndex { get; set; }

            public double EndIndex { get; set; }

            public double StartX { get; set; }

            public double StartY { get; set; }

            public double EndX { get; set; }

            public double EndY { get; set; }

            public double CenterX { get; set; }

            public double CenterY { get; set; }

            public double StartDistance { get; set; }

            public double EndDistance { get; set; }

            public double Length { get; set; }

            public double CrossingValue { get; set; }

            public double MaxValue { get; set; }

            public double AverageValue { get; set; }

            public int SampleCount { get; set; }
        }
        public enum CrossingPolarity
        {
            Rising,
            Falling,
            Both
        }
        #endregion

        #region private function
        private LineProfileResult CaptureByRequestedPoints(
            MIL_ID milImage,
            double requestedStartX,
            double requestedStartY,
            double requestedEndX,
            double requestedEndY,
            double centerX,
            double centerY,
            double angleDeg,
            double length)
        {
            ValidateMilImage(milImage);

            if (length <= 0)
                throw new ArgumentOutOfRangeException("length", "Line profile length must be greater than zero.");

            int imageWidth = InquireInt(milImage, MIL.M_SIZE_X);
            int imageHeight = InquireInt(milImage, MIL.M_SIZE_Y);
            int bandCount = InquireInt(milImage, MIL.M_SIZE_BAND);
            MIL_INT imageType = InquireMilInt(milImage, MIL.M_TYPE);

            if (bandCount != 1)
                throw new NotSupportedException("Only 1-band grayscale images are supported.");

            bool is8Bit = imageType == MIL.M_UNSIGNED + 8;
            bool is16Bit = imageType == MIL.M_UNSIGNED + 16;

            if (!is8Bit && !is16Bit)
                throw new NotSupportedException("Only 8-bit and 16-bit unsigned images are supported in this version.");

            int startX = ClampToInt(requestedStartX, 0, imageWidth - 1);
            int startY = ClampToInt(requestedStartY, 0, imageHeight - 1);
            int endX = ClampToInt(requestedEndX, 0, imageWidth - 1);
            int endY = ClampToInt(requestedEndY, 0, imageHeight - 1);

            int sampleCount = GetSampleCount(startX, startY, endX, endY);
            byte[] rawValues8Bit = null;
            ushort[] rawValues16Bit = null;
            double[] values;

            if (is8Bit)
                values = Capture8BitLine(milImage, startX, startY, endX, endY, sampleCount, out rawValues8Bit);
            else
                values = Capture16BitLine(milImage, startX, startY, endX, endY, sampleCount, out rawValues16Bit);

            return new LineProfileResult
            {
                CenterX = centerX,
                CenterY = centerY,
                AngleDeg = angleDeg,
                RequestedLength = length,
                ActualLength = GetDistance(startX, startY, endX, endY),
                RequestedStartX = requestedStartX,
                RequestedStartY = requestedStartY,
                RequestedEndX = requestedEndX,
                RequestedEndY = requestedEndY,
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY,
                PixelBitDepth = is8Bit ? 8 : 16,
                RawValues8Bit = rawValues8Bit,
                RawValues16Bit = rawValues16Bit,
                Values = values
            };
        }

        private double[] Capture8BitLine(
            MIL_ID milImage,
            int startX,
            int startY,
            int endX,
            int endY,
            int sampleCount,
            out byte[] rawValues)
        {
            rawValues = new byte[sampleCount];
            MIL_INT pixelCount = 0;

            MIL.MbufGetLine(milImage, startX, startY, endX, endY, MIL.M_DEFAULT, ref pixelCount, rawValues);
            ResizeRawValues(ref rawValues, pixelCount);

            double[] values = new double[rawValues.Length];
            for (int i = 0; i < rawValues.Length; i++)
                values[i] = rawValues[i];

            return values;
        }

        private double[] Capture16BitLine(
            MIL_ID milImage,
            int startX,
            int startY,
            int endX,
            int endY,
            int sampleCount,
            out ushort[] rawValues)
        {
            rawValues = new ushort[sampleCount];
            MIL_INT pixelCount = 0;

            MIL.MbufGetLine(milImage, startX, startY, endX, endY, MIL.M_DEFAULT, ref pixelCount, rawValues);
            ResizeRawValues(ref rawValues, pixelCount);

            double[] values = new double[rawValues.Length];
            for (int i = 0; i < rawValues.Length; i++)
                values[i] = rawValues[i];

            return values;
        }

        private void ResizeRawValues<T>(ref T[] rawValues, MIL_INT pixelCount)
        {
            int actualSampleCount = pixelCount > 0 ? checked((int)pixelCount) : rawValues.Length;
            if (actualSampleCount > rawValues.Length)
                actualSampleCount = rawValues.Length;

            if (actualSampleCount < rawValues.Length)
                Array.Resize(ref rawValues, actualSampleCount);
        }

        private void ValidateMilImage(MIL_ID milImage)
        {
            if (milImage == MIL.M_NULL)
                throw new ArgumentException("MIL image cannot be M_NULL.", "milImage");
        }

        private int InquireInt(MIL_ID milImage, MIL_INT inquireType)
        {
            MIL_INT value = 0;
            MIL.MbufInquire(milImage, inquireType, ref value);
            return checked((int)value);
        }

        private MIL_INT InquireMilInt(MIL_ID milImage, MIL_INT inquireType)
        {
            MIL_INT value = 0;
            MIL.MbufInquire(milImage, inquireType, ref value);
            return value;
        }

        private int ClampToInt(double value, int min, int max)
        {
            int rounded = (int)Math.Round(value, MidpointRounding.AwayFromZero);

            if (rounded < min)
                return min;

            if (rounded > max)
                return max;

            return rounded;
        }

        private int GetSampleCount(int startX, int startY, int endX, int endY)
        {
            int dx = Math.Abs(endX - startX);
            int dy = Math.Abs(endY - startY);

            return Math.Max(dx, dy) + 1;
        }

        private double GetDistance(double startX, double startY, double endX, double endY)
        {
            double dx = endX - startX;
            double dy = endY - startY;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private CrossingPolarity? GetDetectedPolarity(double previous, double current, double crossingValue)
        {
            if (previous < crossingValue && current >= crossingValue)
                return CrossingPolarity.Rising;

            if (previous > crossingValue && current <= crossingValue)
                return CrossingPolarity.Falling;

            return null;
        }

        private double GetCrossingIndex(double previous, double current, double crossingValue, int currentIndex)
        {
            if (current.Equals(previous))
                return currentIndex;

            double ratio = (crossingValue - previous) / (current - previous);
            return (currentIndex - 1) + ratio;
        }

        private void GetPointAtIndex(
            LineProfileResult profile,
            double index,
            out double x,
            out double y,
            out double distance)
        {
            double denominator = profile.Values.Length - 1;
            double t = denominator <= 0 ? 0 : index / denominator;

            x = profile.StartX + (profile.EndX - profile.StartX) * t;
            y = profile.StartY + (profile.EndY - profile.StartY) * t;
            distance = profile.ActualLength * t;
        }

        private LineProfileSegment CreateSegment(
            LineProfileResult profile,
            double crossingValue,
            double startIndex,
            double endIndex,
            int firstSampleIndex,
            int lastSampleIndex)
        {
            double startX;
            double startY;
            double startDistance;
            double endX;
            double endY;
            double endDistance;

            GetPointAtIndex(profile, startIndex, out startX, out startY, out startDistance);
            GetPointAtIndex(profile, endIndex, out endX, out endY, out endDistance);

            double maxValue = double.MinValue;
            double sum = 0;
            int sampleCount = 0;

            for (int i = firstSampleIndex; i <= lastSampleIndex; i++)
            {
                double value = profile.Values[i];

                if (value > maxValue)
                    maxValue = value;

                sum += value;
                sampleCount++;
            }

            return new LineProfileSegment
            {
                StartIndex = startIndex,
                EndIndex = endIndex,
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY,
                CenterX = (startX + endX) / 2.0,
                CenterY = (startY + endY) / 2.0,
                StartDistance = startDistance,
                EndDistance = endDistance,
                Length = Math.Abs(endDistance - startDistance),
                CrossingValue = crossingValue,
                MaxValue = maxValue,
                AverageValue = sampleCount > 0 ? sum / sampleCount : 0,
                SampleCount = sampleCount
            };
        }
        #endregion

        #region public function
        public LineProfileResult CaptureByCenterAngle(
            MIL_ID milImage,
            double centerX,
            double centerY,
            double length,
            double angleDeg)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length", "Line profile length must be greater than zero.");

            double angleRad = angleDeg * Math.PI / 180.0;
            double halfLength = length / 2.0;
            double offsetX = Math.Cos(angleRad) * halfLength;
            double offsetY = Math.Sin(angleRad) * halfLength;

            double requestedStartX = centerX - offsetX;
            double requestedStartY = centerY - offsetY;
            double requestedEndX = centerX + offsetX;
            double requestedEndY = centerY + offsetY;

            return CaptureByRequestedPoints(
                milImage,
                requestedStartX,
                requestedStartY,
                requestedEndX,
                requestedEndY,
                centerX,
                centerY,
                angleDeg,
                length);
        }

        public double GetAutoCrossingValue(
            LineProfileResult profile,
            double ratio = 0.5)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            if (profile.Values == null || profile.Values.Length == 0)
                throw new ArgumentException("Profile values cannot be empty.", "profile");

            double min = double.MaxValue;
            double max = double.MinValue;

            for (int i = 0; i < profile.Values.Length; i++)
            {
                double value = profile.Values[i];

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            if (ratio < 0)
                ratio = 0;

            if (ratio > 1)
                ratio = 1;

            return min + (max - min) * ratio;
        }

        public LineProfileSegment FindOuterSegmentAboveValue(
            LineProfileResult profile,
            double crossingValue,
            out string errorMessage)
        {
            errorMessage = "";

            if (profile == null)
            {
                errorMessage = "FindOuterSegmentAboveValue failed: profile is null.";
                return null;
            }

            if (profile.Values == null)
            {
                errorMessage = "FindOuterSegmentAboveValue failed: profile.Values is null.";
                return null;
            }

            if (profile.Values.Length < 2)
            {
                errorMessage = "FindOuterSegmentAboveValue failed: profile.Values length is less than 2.";
                return null;
            }

            int firstSampleIndex = -1;
            int lastSampleIndex = -1;

            for (int i = 0; i < profile.Values.Length; i++)
            {
                if (profile.Values[i] >= crossingValue)
                {
                    firstSampleIndex = i;
                    break;
                }
            }

            for (int i = profile.Values.Length - 1; i >= 0; i--)
            {
                if (profile.Values[i] >= crossingValue)
                {
                    lastSampleIndex = i;
                    break;
                }
            }

            if (firstSampleIndex < 0 || lastSampleIndex < 0 || firstSampleIndex > lastSampleIndex)
            {
                errorMessage = "FindOuterSegmentAboveValue failed: no profile samples are greater than or equal to crossingValue.";
                return null;
            }

            double startIndex = firstSampleIndex;
            if (firstSampleIndex > 0)
                startIndex = GetCrossingIndex(
                    profile.Values[firstSampleIndex - 1],
                    profile.Values[firstSampleIndex],
                    crossingValue,
                    firstSampleIndex);

            double endIndex = lastSampleIndex;
            if (lastSampleIndex < profile.Values.Length - 1)
                endIndex = GetCrossingIndex(
                    profile.Values[lastSampleIndex],
                    profile.Values[lastSampleIndex + 1],
                    crossingValue,
                    lastSampleIndex + 1);

            return CreateSegment(
                profile,
                crossingValue,
                startIndex,
                endIndex,
                firstSampleIndex,
                lastSampleIndex);
        }




        public LineProfileResult CaptureByPoints(
            MIL_ID milImage,
            double x1,
            double y1,
            double x2,
            double y2)
        {
            double centerX = (x1 + x2) / 2.0;
            double centerY = (y1 + y2) / 2.0;
            double length = GetDistance(x1, y1, x2, y2);
            double angleDeg = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            return CaptureByRequestedPoints(milImage, x1, y1, x2, y2, centerX, centerY, angleDeg, length);
        }

        public IReadOnlyList<LineProfileCrossing> FindCrossings(
            LineProfileResult profile,
            double crossingValue,
            CrossingPolarity polarity = CrossingPolarity.Both)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            if (profile.Values == null || profile.Values.Length < 2)
                return new List<LineProfileCrossing>();

            List<LineProfileCrossing> crossings = new List<LineProfileCrossing>();
            double dx = profile.EndX - profile.StartX;
            double dy = profile.EndY - profile.StartY;
            double denominator = profile.Values.Length - 1;

            for (int i = 1; i < profile.Values.Length; i++)
            {
                double previous = profile.Values[i - 1];
                double current = profile.Values[i];

                CrossingPolarity? detectedPolarity = GetDetectedPolarity(previous, current, crossingValue);
                if (detectedPolarity == null)
                    continue;

                if (polarity != CrossingPolarity.Both && polarity != detectedPolarity.Value)
                    continue;

                double ratio = current.Equals(previous) ? 0 : (crossingValue - previous) / (current - previous);
                double index = (i - 1) + ratio;
                double t = index / denominator;

                crossings.Add(new LineProfileCrossing
                {
                    Index = index,
                    X = profile.StartX + dx * t,
                    Y = profile.StartY + dy * t,
                    Distance = profile.ActualLength * t,
                    Value = crossingValue,
                    Polarity = detectedPolarity.Value
                });
            }

            return crossings;
        }

        public IReadOnlyList<LineProfileSegment> FindSegmentsAboveValue(
            LineProfileResult profile,
            double crossingValue,
            int minimumSampleCount = 1)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            if (profile.Values == null || profile.Values.Length < 2)
                return new List<LineProfileSegment>();

            if (minimumSampleCount < 1)
                minimumSampleCount = 1;

            List<LineProfileSegment> segments = new List<LineProfileSegment>();
            bool inSegment = false;
            double segmentStartIndex = 0;
            int firstSampleIndex = 0;

            for (int i = 0; i < profile.Values.Length; i++)
            {
                bool isAbove = profile.Values[i] >= crossingValue;

                if (!inSegment && isAbove)
                {
                    inSegment = true;
                    firstSampleIndex = i;

                    if (i == 0)
                        segmentStartIndex = 0;
                    else
                        segmentStartIndex = GetCrossingIndex(profile.Values[i - 1], profile.Values[i], crossingValue, i);
                }

                if (!inSegment)
                    continue;

                bool isLastSample = i == profile.Values.Length - 1;
                bool nextIsBelow = !isLastSample && profile.Values[i + 1] < crossingValue;

                if (isLastSample || nextIsBelow)
                {
                    double segmentEndIndex = isLastSample
                        ? i
                        : GetCrossingIndex(profile.Values[i], profile.Values[i + 1], crossingValue, i + 1);

                    int lastSampleIndex = i;
                    int sampleCount = lastSampleIndex - firstSampleIndex + 1;

                    if (sampleCount >= minimumSampleCount)
                    {
                        segments.Add(CreateSegment(
                            profile,
                            crossingValue,
                            segmentStartIndex,
                            segmentEndIndex,
                            firstSampleIndex,
                            lastSampleIndex));
                    }

                    inSegment = false;
                }
            }

            return segments;
        }
        #endregion
    }
}
