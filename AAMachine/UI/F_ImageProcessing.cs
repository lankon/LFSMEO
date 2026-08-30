using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;
using AAMachine.Logic.ImageMethod;
using Matrox.MatroxImagingLibrary;

namespace AAMachine.UI
{
    public partial class F_ImageProcessing : Form
    {
        public F_ImageProcessing()
        {
            InitializeComponent();
            this.FormClosed += F_ImageProcessing_FormClosed;

            InitialForm();
        }

        #region parameter define
        private MIL_ID _milApp = MIL.M_NULL;
        private MIL_ID _milSys = MIL.M_NULL;
        private MIL_ID _milDisplay = MIL.M_NULL;
        private MIL_ID _milImage = MIL.M_NULL;
        private MIL_ID _milDisplayImage = MIL.M_NULL;
        private MIL_ID _milGraphicContext = MIL.M_NULL;
        private MIL_ID _milGraphicList = MIL.M_NULL;
        private double HousingTargetOffsetAlongEdge { get; set; } = 1030.0;
        private double HousingTargetOffsetNormalToEdge { get; set; } = -900.0;
        private double HousingTargetLineLength { get; set; } = 500.0;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            //ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            //ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            //ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
            ReleaseMilResources();
        }

        private void EnsureMilResources()
        {
            if (_milApp == MIL.M_NULL)
                MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref _milApp);

            if (_milSys == MIL.M_NULL)
                MIL.MsysAlloc(_milApp, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref _milSys);

            if (_milDisplay == MIL.M_NULL)
            {
                MIL.MdispAlloc(_milSys, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WINDOWED, ref _milDisplay);
                MIL.MdispControl(_milDisplay, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE);
            }

            if (_milGraphicContext == MIL.M_NULL)
                MIL.MgraAlloc(_milSys, ref _milGraphicContext);

            if (_milGraphicList == MIL.M_NULL)
            {
                MIL.MgraAllocList(_milSys, MIL.M_DEFAULT, ref _milGraphicList);
                MIL.MdispControl(_milDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, _milGraphicList);
            }
        }

        private void ReleaseMilResources()
        {
            if (_milDisplay != MIL.M_NULL)
                MIL.MdispSelect(_milDisplay, MIL.M_NULL);

            if (_milGraphicList != MIL.M_NULL)
            {
                MIL.MgraFree(_milGraphicList);
                _milGraphicList = MIL.M_NULL;
            }

            if (_milGraphicContext != MIL.M_NULL)
            {
                MIL.MgraFree(_milGraphicContext);
                _milGraphicContext = MIL.M_NULL;
            }

            if (_milDisplayImage != MIL.M_NULL)
            {
                MIL.MbufFree(_milDisplayImage);
                _milDisplayImage = MIL.M_NULL;
            }

            if (_milImage != MIL.M_NULL)
            {
                MIL.MbufFree(_milImage);
                _milImage = MIL.M_NULL;
            }

            if (_milDisplay != MIL.M_NULL)
            {
                MIL.MdispFree(_milDisplay);
                _milDisplay = MIL.M_NULL;
            }

            if (_milSys != MIL.M_NULL)
            {
                MIL.MsysFree(_milSys);
                _milSys = MIL.M_NULL;
            }

            if (_milApp != MIL.M_NULL)
            {
                MIL.MappFree(_milApp);
                _milApp = MIL.M_NULL;
            }
        }

        private void DisplayImageOnResultPanel(MIL_ID milImage)
        {
            MIL.MdispSelectWindow(_milDisplay, milImage, Pnl_ImageResult.Handle);
        }

        private MIL_ID CreateDisplayImage(MIL_ID sourceImage)
        {
            int imageWidth = InquireInt(sourceImage, MIL.M_SIZE_X);
            int imageHeight = InquireInt(sourceImage, MIL.M_SIZE_Y);
            MIL_INT imageType = InquireMilInt(sourceImage, MIL.M_TYPE);

            MIL_ID displayImage = MIL.M_NULL;

            if (imageType == MIL.M_UNSIGNED + 8)
            {
                MIL.MbufAlloc2d(
                    _milSys,
                    imageWidth,
                    imageHeight,
                    MIL.M_UNSIGNED + 8,
                    MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
                    ref displayImage);

                MIL.MbufCopy(sourceImage, displayImage);
                return displayImage;
            }

            if (imageType == MIL.M_UNSIGNED + 16)
            {
                MIL.MbufAlloc2d(
                    _milSys,
                    imageWidth,
                    imageHeight,
                    MIL.M_UNSIGNED + 8,
                    MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
                    ref displayImage);

                Copy16BitImageTo8BitDisplay(sourceImage, displayImage, imageWidth, imageHeight);
                return displayImage;
            }

            throw new NotSupportedException("Only 8-bit and 16-bit unsigned images can be displayed in this form.");
        }

        private void Copy16BitImageTo8BitDisplay(MIL_ID sourceImage, MIL_ID displayImage, int imageWidth, int imageHeight)
        {
            int pixelCount = checked(imageWidth * imageHeight);
            ushort[] sourceValues = new ushort[pixelCount];
            byte[] displayValues = new byte[pixelCount];

            MIL.MbufGet(sourceImage, sourceValues);

            ushort minValue = ushort.MaxValue;
            ushort maxValue = ushort.MinValue;

            for (int i = 0; i < sourceValues.Length; i++)
            {
                ushort value = sourceValues[i];

                if (value < minValue)
                    minValue = value;

                if (value > maxValue)
                    maxValue = value;
            }

            double range = maxValue - minValue;
            if (range <= 0)
            {
                MIL.MbufPut(displayImage, displayValues);
                return;
            }

            for (int i = 0; i < sourceValues.Length; i++)
            {
                double normalized = (sourceValues[i] - minValue) * 255.0 / range;
                displayValues[i] = (byte)Math.Max(0, Math.Min(255, normalized));
            }

            MIL.MbufPut(displayImage, displayValues);
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

        private void DrawLineProfileOverlay(
            CaptureLineProfile.LineProfileResult profile,
            CaptureLineProfile.LineProfileSegment segment)
        {
            //MIL.MgraClear(_milGraphicContext, _milGraphicList);

            MIL.MgraColor(_milGraphicContext, MIL.M_COLOR_YELLOW);
            MIL.MgraLine(
                _milGraphicContext,
                _milGraphicList,
                profile.StartX,
                profile.StartY,
                profile.EndX,
                profile.EndY);

            if (segment == null)
                return;

            MIL.MgraColor(_milGraphicContext, MIL.M_COLOR_RED);
            MIL.MgraLine(
                _milGraphicContext,
                _milGraphicList,
                segment.StartX,
                segment.StartY,
                segment.EndX,
                segment.EndY);

            DrawCross(segment.CenterX, segment.CenterY, 50);
        }

        private void DrawCross(double centerX, double centerY, double size)
        {
            DrawCross(centerX, centerY, size, MIL.M_COLOR_GREEN);
        }

        private void DrawCross(double centerX, double centerY, double size, double color)
        {
            MIL.MgraColor(_milGraphicContext, color);
            MIL.MgraLine(_milGraphicContext, _milGraphicList, centerX - size, centerY, centerX + size, centerY);
            MIL.MgraLine(_milGraphicContext, _milGraphicList, centerX, centerY - size, centerX, centerY + size);
        }

        private void DrawHousingEdgeOverlay(FindHousingFeature.EdgeResult edge)
        {
            if (edge == null || !edge.Success)
                return;

            MIL.MgraColor(_milGraphicContext, MIL.M_COLOR_RED);
            MIL.MgraLine(
                _milGraphicContext,
                _milGraphicList,
                edge.StartX,
                edge.StartY,
                edge.EndX,
                edge.EndY);

            DrawCross(edge.PositionX, edge.PositionY, 30);
            DrawDerivedHousingTarget(edge);
        }

        private void DrawDerivedHousingTarget(FindHousingFeature.EdgeResult edge)
        {
            double edgeDx = edge.EndX - edge.StartX;
            double edgeDy = edge.EndY - edge.StartY;
            double edgeLength = Math.Sqrt(edgeDx * edgeDx + edgeDy * edgeDy);

            if (edgeLength <= 0.0)
                return;

            double edgeUnitX = edgeDx / edgeLength;
            double edgeUnitY = edgeDy / edgeLength;
            double normalUnitX = -edgeUnitY;
            double normalUnitY = edgeUnitX;

            double targetX = edge.PositionX
                + HousingTargetOffsetAlongEdge * edgeUnitX
                + HousingTargetOffsetNormalToEdge * normalUnitX;

            double targetY = edge.PositionY
                + HousingTargetOffsetAlongEdge * edgeUnitY
                + HousingTargetOffsetNormalToEdge * normalUnitY;

            double halfLineLength = HousingTargetLineLength / 2.0;

            double lineStartX = targetX - halfLineLength * normalUnitX;
            double lineStartY = targetY - halfLineLength * normalUnitY;
            double lineEndX = targetX + halfLineLength * normalUnitX;
            double lineEndY = targetY + halfLineLength * normalUnitY;

            MIL.MgraColor(_milGraphicContext, MIL.M_COLOR_RED);
            MIL.MgraLine(
                _milGraphicContext,
                _milGraphicList,
                lineStartX,
                lineStartY,
                lineEndX,
                lineEndY);

            DrawCross(targetX, targetY, 30, MIL.M_COLOR_GREEN);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        #endregion
        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

                LeavePage();
                //釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void F_SampleFull_Load(object sender, EventArgs e)
        {

        }

        private void F_ImageProcessing_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReleaseMilResources();
        }

        private void Btn_CaptureLineProfile_Click(object sender, EventArgs e)
        {
            var captureLineProfile = new CaptureLineProfile();

            ReleaseMilResources();
            EnsureMilResources();

            MIL.MbufImport(
                @"D:\0.桌面雜物\MirrorAA_InProcess\AA_06_22222_20260804T190723_Tx.png",
                MIL.M_DEFAULT,
                MIL.M_RESTORE,
                _milSys,
                ref _milImage
            );

            _milDisplayImage = CreateDisplayImage(_milImage);

            var profile = captureLineProfile.CaptureByCenterAngle(_milImage,
                                                                    centerX: 6948,
                                                                    centerY: 4922,
                                                                    length: 11000,
                                                                    angleDeg: -28);

            var profile_y = captureLineProfile.CaptureByCenterAngle(_milImage,
                                                                    centerX: 6948,
                                                                    centerY: 4922,
                                                                    length: 11000,
                                                                    angleDeg: -118);


            double value =  captureLineProfile.GetAutoCrossingValue(profile, 0.5);


            string errorMessage;
            var segment = captureLineProfile.FindOuterSegmentAboveValue(profile, value, out errorMessage);
            var segment_y = captureLineProfile.FindOuterSegmentAboveValue(profile_y, value, out errorMessage);

            if (segment != null && segment_y != null)
            {
                TxtBx_CenterX.Text = segment.CenterX.ToString("F2");
                TxtBx_CenterY.Text = segment_y.CenterY.ToString("F2");
            }
            else
            {
                TxtBx_CenterX.Text = "";
                TxtBx_CenterY.Text = "";
                MessageBox.Show(errorMessage, "Line Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            MIL.MgraClear(_milGraphicContext, _milGraphicList);

            DrawLineProfileOverlay(profile, segment);
            DrawLineProfileOverlay(profile_y, segment_y);
            DisplayImageOnResultPanel(_milDisplayImage);
        }

        private void Btn_FindHousing_Click(object sender, EventArgs e)
        {
            ReleaseMilResources();
            EnsureMilResources();

            try
            {
                MIL.MbufImport(
                    @"C:\Users\leo_li\Desktop\上PA看Housing_環光100軸光70.png",
                    MIL.M_DEFAULT,
                    MIL.M_RESTORE,
                    _milSys,
                    ref _milImage
                );

                _milDisplayImage = CreateDisplayImage(_milImage);

                using (FindHousingFeature finder = new FindHousingFeature(_milSys))
                {
                    FindHousingFeature.EdgeResult res = finder.Find(_milImage);

                    MIL.MgraClear(_milGraphicContext, _milGraphicList);
                    DrawHousingEdgeOverlay(res);
                    DisplayImageOnResultPanel(_milDisplayImage);

                    if (res.Success)
                    {
                        TxtBx_CenterX.Text = res.PositionX.ToString("F2");
                        TxtBx_CenterY.Text = res.PositionY.ToString("F2");
                    }
                    else
                    {
                        TxtBx_CenterX.Text = "";
                        TxtBx_CenterY.Text = "";
                        MessageBox.Show("Edge not found.", "Find Housing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            finally
            {
                if (_milImage != MIL.M_NULL)
                {
                    MIL.MbufFree(_milImage);
                    _milImage = MIL.M_NULL;
                }
            }
        }
    }
}
