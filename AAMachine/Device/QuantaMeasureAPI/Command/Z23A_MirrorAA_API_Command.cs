using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;
using System.IO;

using Matrox.MatroxImagingLibrary;
using Z23A_MirrorAA_API;
using AAMachine.Device.QuantaMeasureAPI.Base;

namespace AAMachine.Device.QuantaMeasureAPI.Z23A
{
    public class Z23A_MirrorAA_API_Command
    {
        public Z23A_MirrorAA_API_Command()
        {
            api = new Z23A_MirrorAA();
            api.Initial();
        }

        #region parameter define
        Z23A_MirrorAA api;
        #endregion

        #region private function
        private ImageInfo ConvertImgToApiImg(MeasureImageInfo img)  // 將 MeasureImageInfo 轉換為 ImageInfo
        {
            ImageInfo api_img = new ImageInfo
            {
                Data = img.Data,
                Width = img.Width,
                Height = img.Height,
                Channels = img.Channels,
                Format = ConvertPixelFormat(img.Format)
            };

            return api_img;
        }

        private PixelFormat ConvertPixelFormat(MeasurePixelFormat format)   // 將 MeasurePixelFormat 轉換為 PixelFormat
        {
            switch (format)
            {
                case MeasurePixelFormat.Gray8:
                    return PixelFormat.Gray8;
                case MeasurePixelFormat.Gray16:
                    return PixelFormat.Gray16;
                case MeasurePixelFormat.Gray32:
                    return PixelFormat.Gray32;
                default:
                    return PixelFormat.Gray8;
            }
        }

        private Side ConvertSideToApiSide(TestSide side)   // 將 TestSide 轉換為 Side
        {
            if (side == TestSide.Left)
                return Side.Left;

            return Side.Right;
        }

        private byte[] ConvertRgb24ToGray8(byte[] rgbData)  // 將RGB24影像轉換為Gray8影像
        {
            byte[] grayData = new byte[rgbData.Length / 3];

            for (int i = 0, j = 0; i < rgbData.Length; i += 3, j++)
                grayData[j] = rgbData[i];

            return grayData;
        }

        private bool IsRgbChannelsSame(byte[] rgbData)  // 檢查RGB三個通道是否相同
        {
            /// <summary>
            /// 影像排列方式RGBRGBRGB，檢查RGB三個通道是否相同
            /// </summary>

            for (int i = 0; i < rgbData.Length; i += 3)
            {
                byte r = rgbData[i];
                byte g = rgbData[i + 1];
                byte b = rgbData[i + 2];

                if (r != g || g != b)
                    return false;
            }

            return true;
        }

        private bool IsPlanarRgbChannelsSame(byte[] data, int width, int height)    // 檢查RGB三個通道是否相同
        {
            /// <summary>
            /// 影像排列方式RRRR...GGGG...BBBB，檢查RGB三個通道是否相同
            /// </summary>

            int pixelCount = width * height;
            int gOffset = pixelCount;
            int bOffset = pixelCount * 2;

            for (int i = 0; i < pixelCount; i++)
            {
                byte r = data[i];
                byte g = data[gOffset + i];
                byte b = data[bOffset + i];

                if (r != g || g != b)
                    return false;
            }

            return true;
        }
        #endregion

        #region public function
        // 光學測試項目演算法
        public UniformityResultInfo GetUniformity(MeasureImageInfo image, 
                                                    TestSide side, PointF boresightPx, 
                                                    double rotation, double cameraPPD, 
                                                    double expT, string processFolder = "")
        {
            ImageInfo apiImage = ConvertImgToApiImg(image);

            Side api_side = ConvertSideToApiSide(side);

            UniformityResult res = null;

            try
            {
                res = api.GetUniformity(apiImage, api_side, boresightPx, rotation, cameraPPD, processFolder, expT);
            }
            catch (Exception ex)
            {
                //throw new Exception($"GetUniformity failed: {ex.Message}");
            }

            UniformityResultInfo info = new UniformityResultInfo
            {
                Away = res.Away,
                MaxMin = res.MaxMin,
                IntensityMap = res.IntensityMap
            };

            return info;
        }

        public RollOffResultInfo GetBrightnessRolloff(MeasureImageInfo image,
                                                        TestSide side, PointF boresightPx,
                                                        double rotation, double cameraPPD,
                                                        string processFolder = "")
        {
            ImageInfo apiImage = ConvertImgToApiImg(image);

            Side api_side = ConvertSideToApiSide(side);

            RollOffResult res = null;

            try
            {
                res = api.GetBrightnessRolloff(apiImage, api_side, boresightPx, rotation, cameraPPD, processFolder);
            }
            catch (Exception ex)
            {
                //throw new Exception($"GetUniformity failed: {ex.Message}");
            }

            RollOffResultInfo info = new RollOffResultInfo
            {
                ZoneB = res.ZoneB,
                ZoneC = res.ZoneC,
            };

            return info;
        }

        public List<PointF> GetNinePtsPosition(MeasureImageInfo image,
                                                TestSide side, PointF boresightPx,
                                                double rotation, double cameraPPD,
                                                string processFolder = "")
        {
            ImageInfo apiImage = ConvertImgToApiImg(image);

            Side api_side = ConvertSideToApiSide(side);

            List<PointF> res = null;

            try
            {
                res = api.GetNinePtsPosition(apiImage, api_side, boresightPx, rotation, cameraPPD, processFolder);
            }
            catch (Exception ex)
            {
                //throw new Exception($"GetUniformity failed: {ex.Message}");
            }

            return res;
        }

        public ContrastResultInfo GetSequentialContrast(MeasureImageInfo imgBright, MeasureImageInfo imgDark,
                                                        TestSide side, PointF boresightPx,
                                                        double rotation, double cameraPPD, List<PointF> ninePts,
                                                        double expTBright, double expTDark,
                                                        string processFolder = "")
        {
            ImageInfo BrightImage = ConvertImgToApiImg(imgBright);
            ImageInfo DarkImage = ConvertImgToApiImg(imgDark);

            Side api_side = ConvertSideToApiSide(side);

            ContrastResult res = null;

            try
            {
                res = api.GetSequentialContrast(BrightImage, DarkImage, api_side, boresightPx, rotation, cameraPPD, ninePts, processFolder, expTBright, expTDark);
            }
            catch (Exception ex)
            {
                //throw new Exception($"GetUniformity failed: {ex.Message}");
            }

            ContrastResultInfo info = new ContrastResultInfo
            {
                InField = res.InField,
                NinePoints = res.NinePoints,
            };

            return info;
        }


        // 輔助函式
        public void SaveLigtSpotMapToCsv(double[,] lightMap, string filePath)
        {
            int rows = lightMap.GetLength(0);
            int cols = lightMap.GetLength(1);
            var lines = new List<string>();
            for (int row = 0; row < rows; row++)
            {
                var values = new List<string>();
                for (int col = 0; col < cols; col++)
                {
                    values.Add(lightMap[row, col].ToString(CultureInfo.InvariantCulture));
                }
                lines.Add(string.Join(",", values));
            }
            File.WriteAllLines(filePath, lines);
        }

        public (double CenterX, double CneterY) GetLightSpotGravityCenter(double[,] lightMap, double threshold = 0.05)
        {
            int rows = lightMap.GetLength(0);
            int cols = lightMap.GetLength(1);

            double max = double.MinValue;

            // 找到最大值    
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (lightMap[y, x] > max)
                        max = lightMap[y, x];
                }
            }

            double sumI = 0;
            double sumX = 0;
            double sumY = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    double intensity = lightMap[y, x];

                    if (intensity < threshold)
                        continue;

                    sumI += intensity;
                    sumX += x * intensity;
                    sumY += y * intensity;
                }
            }

            if (sumI <= 0)
                return (double.NaN, double.NaN);

            return (sumX / sumI, sumY / sumI);
        }

        public MeasureImageInfo ConvertMilImageToImageInfo(MIL_ID milImage)
        {
            // 取得影像資訊
            MIL_INT width = 0;
            MIL_INT height = 0;
            MIL_INT channels = 0;
            MIL_INT type = 0;

            MIL.MbufInquire(milImage, MIL.M_SIZE_X, ref width);
            MIL.MbufInquire(milImage, MIL.M_SIZE_Y, ref height);
            MIL.MbufInquire(milImage, MIL.M_SIZE_BAND, ref channels);
            MIL.MbufInquire(milImage, MIL.M_TYPE, ref type);

            int pixelCount = checked((int)(width * height * channels));

            if (type == MIL.M_UNSIGNED + 8 && channels == 1)
            {
                byte[] data = new byte[pixelCount];
                MIL.MbufGet(milImage, data);

                return new MeasureImageInfo
                {
                    Data = data,
                    Width = (int)width,
                    Height = (int)height,
                    Channels = 1,
                    Format = MeasurePixelFormat.Gray8
                };
            }

            if (type == MIL.M_UNSIGNED + 8 && channels == 3)
            {
                byte[] rgbData = new byte[pixelCount];
                MIL.MbufGet(milImage, rgbData);

                if (IsPlanarRgbChannelsSame(rgbData, (int)width, (int)height) == false)
                    throw new NotSupportedException("RGB image channels are not the same. Only grayscale RGB24 images can be converted to Gray8.");

                // 分別取得RGB通道的子影像
                MIL_ID childR = MIL.M_NULL;
                MIL_ID childG = MIL.M_NULL;
                MIL_ID childB = MIL.M_NULL;

                try
                {
                    MIL.MbufChildColor(milImage, MIL.M_RED, ref childR);
                    MIL.MbufChildColor(milImage, MIL.M_GREEN, ref childG);
                    MIL.MbufChildColor(milImage, MIL.M_BLUE, ref childB);

                    byte[] r = new byte[width * height];
                    byte[] g = new byte[width * height];
                    byte[] b = new byte[width * height];

                    MIL.MbufGet(childR, r);
                    MIL.MbufGet(childG, g);
                    MIL.MbufGet(childB, b);

                    return new MeasureImageInfo
                    {
                        Data = r,
                        Width = (int)width,
                        Height = (int)height,
                        Channels = 1,
                        Format = MeasurePixelFormat.Gray8
                    };
                }
                finally
                {
                    if (childR != MIL.M_NULL) MIL.MbufFree(childR);
                    if (childG != MIL.M_NULL) MIL.MbufFree(childG);
                    if (childB != MIL.M_NULL) MIL.MbufFree(childB);
                }
            }

            if (type == MIL.M_UNSIGNED + 16 && channels == 1)
            {
                ushort[] data16 = new ushort[pixelCount];
                MIL.MbufGet(milImage, data16);

                byte[] data = new byte[data16.Length * sizeof(ushort)];
                Buffer.BlockCopy(data16, 0, data, 0, data.Length);

                return new MeasureImageInfo
                {
                    Data = data,
                    Width = (int)width,
                    Height = (int)height,
                    Channels = 1,
                    Format = MeasurePixelFormat.Gray16
                };
            }

            if (type == MIL.M_UNSIGNED + 32 && channels == 1)
            {
                uint[] data32 = new uint[pixelCount];
                MIL.MbufGet(milImage, data32);

                byte[] data = new byte[data32.Length * sizeof(uint)];
                Buffer.BlockCopy(data32, 0, data, 0, data.Length);

                return new MeasureImageInfo
                {
                    Data = data,
                    Width = (int)width,
                    Height = (int)height,
                    Channels = 1,
                    Format = MeasurePixelFormat.Gray32
                };
            }

            throw new NotSupportedException($"Unsupported MIL image format. Type: {type}, Channels: {channels}.");
        }

        public int SaveRawImage(MeasureImageInfo image, string save_path)
        {
            ImageInfo apiImage = ConvertImgToApiImg(image);

            api.SaveRawImage(apiImage, save_path);

            return 0;
        }

        #endregion
    }
}
