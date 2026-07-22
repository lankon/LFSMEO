using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;
using Z23A_MirrorAA_API;
using AAMachine.Device.QuantaMeasureAPI.Base;

namespace AAMachine.Device.QuantaMeasureAPI.Z23A
{
    public class Z23A_Measure_API_Command
    {
        public Z23A_Measure_API_Command()
        {
            api = new Z23A_MirrorAA();
        }

        #region parameter define
        Z23A_MirrorAA api;
        #endregion

        #region private function
        private PixelFormat ConvertPixelFormat(MeasurePixelFormat format)
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

        private MeasureImageInfo ConvertMilImageToImageInfo(MIL_ID milImage)
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

        /// <summary>
        /// 影像排列方式RGBRGBRGB，檢查RGB三個通道是否相同
        /// </summary>
        private bool IsRgbChannelsSame(byte[] rgbData)
        {
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

        /// <summary>
        /// 影像排列方式RRRR...GGGG...BBBB，檢查RGB三個通道是否相同
        /// </summary>
        private bool IsPlanarRgbChannelsSame(byte[] data, int width, int height)
        {
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

        private byte[] ConvertRgb24ToGray8(byte[] rgbData)
        {
            byte[] grayData = new byte[rgbData.Length / 3];

            for (int i = 0, j = 0; i < rgbData.Length; i += 3, j++)
                grayData[j] = rgbData[i];

            return grayData;
        }
        #endregion

        #region public function
        public int SaveRawImage(MeasureImageInfo image, string save_path)
        {
            ImageInfo apiImage = new ImageInfo
            {
                Data = image.Data,
                Width = image.Width,
                Height = image.Height,
                Channels = image.Channels,
                Format = ConvertPixelFormat(image.Format)
            };

            api.SaveRawImage(apiImage, save_path);

            return 0;
        }
        #endregion
    }
}
