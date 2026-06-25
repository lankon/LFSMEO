//========================================================
//廣達提供的MLO Function
//========================================================

from typing import Dict, List

import mlcolorimeter as mlcm
import sys
import os
import cv2
import numpy as np
from dataclasses import dataclass
import tifffile as tiff

@dataclass
class ImageData:
    image : np.ndarray
    max_val : float
    name : str
    exposuretime : float
    # graystatus : str

class MLO_Camera:
    def __init__(self, eye1_path, connect = True):
        print(eye1_path)
        self.eye1_path = eye1_path
        path_list = [eye1_path]

        # create a ML_Colorimeter system instance
        self.ml_colorimeter = mlcm.ML_Colorimeter()
        # add mono module into ml_colorimeter system, according to path_list create one or more mono module
        ret = self.ml_colorimeter.ml_add_module(path_list=path_list)
        if not ret.success:
            raise RuntimeError("ml_add_module error")
        # connect all module in the ml_colorimeter system
        if connect:
            ret = self.ml_colorimeter.ml_connect()
            if not ret.success:
                raise RuntimeError("1.ml_connect error")
        self.aperture = "2.93mm"
        # self.aperture = "2.65mm"
        module_id = 1
        self.module_id = module_id
        self.ml_mono = self.ml_colorimeter.ml_bino_manage.ml_get_module_by_id(self.module_id)
        # nd filter to switch during measurement
        # set_nd = mlcm.MLFilterEnum.ND0
        # print(f'{self.ml_mono.ml_get_nd()}')
        
        # ret = self.ml_mono.ml_move_nd_syn(set_nd)

        # # Get_Camera_Setting()
        if connect:
            self.Set_pixel_format()
            self.Set_Binn(1)
            self.Set_ExposureTime_XYZ(2, 11.111, 22.222, 33.333)
            self.Set_ExposureTime_Single(2, 11.111)
            self.Set_Color_Filter(mlcm.MLFilterEnum.Clear)
            self.set_vid("W", 9999999)
        self.motion_name = "CameraMotion"

    def Set_pixel_format(self):
        pixel_format = mlcm.MLPixelFormat.MLMono12
        # pixel_format = mlcm.MLPixelFormat.MLMono8
        # Format of the pixel to use for acquisition.
        ret = self.ml_mono.ml_set_pixel_format(pixel_format)
        if not ret.success:
            raise RuntimeError("ml_set_pixel_format error")
        self.Get_pixel_format()
    def Get_pixel_format(self):
        # print(f'MLMono8/MLMono12')
        get_pixel_format = self.ml_mono.ml_get_pixel_format()
        # print(f'pixel_format:{get_pixel_format}')
        return get_pixel_format
    def Set_Binn(self, Binning = 1, selector = 0):
        # camera binning selector
        if selector == 0:
            binn_selector = mlcm.BinningSelector.Logic
            get_binn_selector = self.ml_mono.ml_get_binning_selector()
            if get_binn_selector == mlcm.BinningSelector.Sensor:
                ret = self.ml_mono.ml_set_binning(mlcm.Binning.ONE_BY_ONE)
                if not ret.success:
                    raise RuntimeError("ml_set_binning error")
        else:
            binn_selector = mlcm.BinningSelector.Sensor

        # camera binning
        if Binning == 1:
            binn = mlcm.Binning.ONE_BY_ONE
        elif Binning == 2:
            binn = mlcm.Binning.TWO_BY_TWO
        elif Binning == 3:
            binn = mlcm.Binning.FOUR_BY_FOUR
        elif Binning == 4:
            binn = mlcm.Binning.EIGHT_BY_EIGHT
        elif Binning == 5:
            binn = mlcm.Binning.SIXTEEN_BY_SIXTEEN
        else:
            binn = mlcm.Binning.ONE_BY_ONE
        # camera binning mode
        binn_mode = mlcm.BinningMode.AVERAGE

        ret = self.ml_mono.ml_set_binning_selector(binn_selector)
        if not ret.success:
            raise RuntimeError("ml_set_binning_selector error")
        # print(f'binn:{binn}')
        ret = self.ml_mono.ml_set_binning(binn)
        if not ret.success:
            raise RuntimeError("ml_set_binning error")

        # Set binning mode for camera.
        ret = self.ml_mono.ml_set_binning_mode(binn_mode)
        if not ret.success:
            raise RuntimeError("ml_set_binning_mode error")
        self.Get_Binn()
    def Get_Binn(self):
        # print('BinningSelector,Logic/Sensor')
        # print('Binning,1:ONE_BY_ONE/2:TWO_BY_TWO/3:FOUR_BY_FOUR/4:EIGHT_BY_EIGHT/5:SIXTEEN_BY_SIXTEEN')
        # print('binn_mode,AVERAGE/SUM')
        get_binn_selector = self.ml_mono.ml_get_binning_selector()
        # print(f'binn_selector:{get_binn_selector}')
        get_binn = self.ml_mono.ml_get_binning()
        # print(f'binn:{get_binn}')
        get_binn_mode = self.ml_mono.ml_get_binning_mode()
        # print(f'binn_mode:{get_binn_mode}')
        return get_binn
    def Set_ExposureTime_Single(self, iExposureMode = 1, ExpT = 100):
        if iExposureMode == 1:
            exposure_mode = mlcm.ExposureMode.Auto
        elif iExposureMode == 2:
            exposure_mode = mlcm.ExposureMode.Fixed
        else:
            exposure_mode = mlcm.ExposureMode.Auto
        exposure_time = ExpT
        self.exposure = mlcm.pyExposureSetting(exposure_mode = exposure_mode, exposure_time = exposure_time)
        # Set exposure for camera, contain auto and fixed.
        ret = self.ml_mono.ml_set_exposure(self.exposure)
        if not ret.success:
            raise RuntimeError("ml_set_exposure error")
        self.Get_ExposureTime_Single()
    def Get_ExposureTime_Single(self):
        # print('ExposureMode,1:Auto/2:Fixed')
        # get_exposure_mode = self.ml_mono.ml_get_auto_exposure_status()
        # print(f'exposure_mode:{self.exposure.exposure_mode}')
        # get_exposure_time = self.ml_mono.ml_get_exposure_time()
        # print(f'exposure_time:{get_exposure_time}')
        # print(f'exposure_time:{self.exposure.exposure_time}')
        return (self.exposure.exposure_mode, self.exposure.exposure_time)
    def Set_ExposureTime_XYZ(self, iExposureMode = 1, ExpT_X = 100, ExpT_Y = 100, ExpT_Z = 100):
        if iExposureMode == 1:
            exposure_mode = mlcm.ExposureMode.Auto
        elif iExposureMode == 2:
            exposure_mode = mlcm.ExposureMode.Fixed
        else:
            exposure_mode = mlcm.ExposureMode.Auto
        self.exposure_map = {
            mlcm.MLFilterEnum.X: mlcm.pyExposureSetting(
                exposure_mode=exposure_mode, exposure_time=ExpT_X
            ),
            mlcm.MLFilterEnum.Y: mlcm.pyExposureSetting(
                exposure_mode=exposure_mode, exposure_time=ExpT_Y
            ),
            mlcm.MLFilterEnum.Z: mlcm.pyExposureSetting(
                exposure_mode=exposure_mode, exposure_time=ExpT_Z
            ),
        }
    def Get_ExposureTime_XYZ(self):
        # print(f'XYZ exposure_time:{self.exposure_map[mlcm.MLFilterEnum.X].exposure_time}, {self.exposure_map[mlcm.MLFilterEnum.Y].exposure_time}, {self.exposure_map[mlcm.MLFilterEnum.Z].exposure_time}')
        return self.exposure_map[mlcm.MLFilterEnum.X].exposure_time, self.exposure_map[mlcm.MLFilterEnum.Y].exposure_time, self.exposure_map[mlcm.MLFilterEnum.Z].exposure_time
    def Get_ExposureMode_XYZ(self):
        # print(f'XYZ exposure_mode:{self.exposure_map[mlcm.MLFilterEnum.X].exposure_mode}')
        return self.exposure_map[mlcm.MLFilterEnum.X].exposure_mode, self.exposure_map[mlcm.MLFilterEnum.Y].exposure_mode, self.exposure_map[mlcm.MLFilterEnum.Z].exposure_mode
    def Get_Camera_Setting(self):
        print('Get_Camera_Setting')
        res = self.Get_pixel_format()
        print(f'pixel_format:{res}')
        res = self.Get_Binn()
        print(f'binn:{res}')
        res = self.Get_ND()
        print(f'nd:{mlcm.MLFilterEnum_to_str(res)}')
        res = self.Get_ExposureMode_XYZ()
        print(f'XYZ exposure_mode:{res[0]},{res[1]},{res[2]}')
        res = self.Get_ExposureTime_XYZ()
        print(f'XYZ exposure_time:{res[0]},{res[1]},{res[2]}')
        res = self.Get_ExposureTime_Single()
        print(f'exposure_mode:{res[0]}')
        print(f'exposure_time:{res[1]}')
    #region Filter
    def Set_ND(self, iND):
        print('Set_ND')
        nd = None
        if iND == 0:
            nd = mlcm.MLFilterEnum.ND0
        elif iND == 1:
            nd = mlcm.MLFilterEnum.ND1
        elif iND == 2:
            nd = mlcm.MLFilterEnum.ND2
        elif iND == 3:
            nd = mlcm.MLFilterEnum.ND3
        else:
            nd = mlcm.MLFilterEnum.ND0
        # print(f'{nd}')
        # print(f'{self.ml_mono.ml_get_nd()}')
        # move nd filter
        ret = self.ml_mono.ml_move_nd_syn(nd)
        print(f'{ret.success}')
        if not ret.success:
            raise RuntimeError("ml_move_nd_syn error")
        self.Get_ND()
    def Get_ND(self):
        # print('ND,0:ND0/1:ND1/2:ND2/3:ND3')
        get_nd = self.ml_mono.ml_get_nd()
        # print(f'nd:{mlcm.MLFilterEnum_to_str(get_nd)}')
        return get_nd
    def Set_Color_Filter(self, Color:mlcm.MLFilterEnum):
        # switch xyz filter
        ret = self.ml_mono.ml_move_xyz_syn(Color)
        if not ret.success:
            raise RuntimeError("ml_move_xyz_syn error")
    #endregion
    #region Focus
    def set_abs_pos(self):
        # control focus motion with absolute postion
        abs_pos = 8
        ret = self.ml_mono.ml_set_pos_abs_syn(motion_name=self.motion_name, pos=abs_pos)
        if not ret.success:
            raise RuntimeError("ml_set_pos_abs_syn error")
        get_pos = self.get_pos()
        print("abs position:" + str(get_pos))
    def set_rel_pos(self):
        # control focus motion with relative position
        get_pos = self.get_pos()
        print("previous position:" + str(get_pos))
        rel_pos = 1
        ret = self.ml_mono.ml_set_pos_rel_syn(motion_name=self.motion_name, pos=rel_pos)
        if not ret.success:
            raise RuntimeError("ml_set_pos_rel_syn error")
        get_pos2 = self.get_pos()
        print("current position:" + str(get_pos2))
    def get_pos(self):
        get_pos = self.ml_mono.ml_get_pos(self.motion_name)
        print("position:" + str(get_pos))
        return get_pos
    def set_vid(self, RGBW, vid):
        # control focus motion with vid setting
        ret = self.ml_mono.ml_update_inf_position(inf_str=RGBW)
        if not ret.success:
            raise RuntimeError("ml_update_inf_position error")
        ret = self.ml_mono.ml_set_vid_syn(vid=vid)
        if not ret.success:
            raise RuntimeError("ml_set_vid_syn error")
        self.get_vid()
    def get_vid(self):
        get_vid = self.ml_mono.ml_get_vid()
        print("vid:" + str(get_vid))
        return get_vid
    #endregion
    #region
    
    #endregion
    #region
    
    #endregion
    #region
    
    #endregion
    #region
    
    #endregion
    #region Capture
    def Capture_luminance_Image(self, OutputPath, SN, sRGBW, iCXYZ, iBinn, iND, iExposureMode = 1, ExpT = 100, VID = 9999999) -> ImageData:
        # print(f'Capture_luminance_Image')
        iR = ImageData(name='',exposuretime=0,image=[],max_val=0)
        try:
            
            nd = None
            if iND == 0:
                nd = mlcm.MLFilterEnum.ND0
            elif iND == 1:
                nd = mlcm.MLFilterEnum.ND1
            elif iND == 2:
                nd = mlcm.MLFilterEnum.ND2
            elif iND == 3:
                nd = mlcm.MLFilterEnum.ND3
            else:
                nd = mlcm.MLFilterEnum.ND0
            self.Set_ND(iND)
            self.Set_ExposureTime_Single(iExposureMode, ExpT)
            self.Set_pixel_format()
            self.Set_Binn(iBinn)

            # xyz filter to switch during measurement
            if iCXYZ == 0:
                color_filter = mlcm.MLFilterEnum.Clear
            elif iCXYZ == 1:
                color_filter = mlcm.MLFilterEnum.X
            elif iCXYZ == 2:
                color_filter = mlcm.MLFilterEnum.Y
            elif iCXYZ == 3:
                color_filter = mlcm.MLFilterEnum.Z
            else:
                color_filter = mlcm.MLFilterEnum.Clear

            rx = mlcm.pyRXCombination(sph=0, cyl=0, axis=0)
            # ret = self.ml_mono.ml_set_rx_syn(rx)
            # if not ret.success:
            #     raise RuntimeError("ml_set_rx_syn error")
            aperture = self.aperture
            ret = self.ml_mono.ml_set_aperture(aperture)
            light_source = sRGBW
            if light_source == "W":
                light_source = "G"
            ret = self.ml_mono.ml_set_light_source(light_source)

            self.set_vid(sRGBW, VID)

            # take a luminance measurement for module_id
            # calibration config for luminance measurement
            cali_config = mlcm.pyCalibrationConfig(
                input_path=self.eye1_path,
                aperture=self.ml_mono.ml_get_aperture(),
                binn=self.Get_Binn(),
                nd_filter_list=[nd],
                color_filter_list=[color_filter],
                rx=rx,
                light_source_list=[self.ml_mono.ml_get_light_source()],
                dark_flag=True,
                ffc_flag=True,
                color_shift_flag=True,
                distortion_flag=True,
                exposure_flag=True,
                luminance_flag=True,
                # flip_rotate_flag = True
            )
            SaveResult = True
            if OutputPath == '':
                SaveResult = False

            # save config
            save_config = mlcm.pySaveDataConfig(
                save_path=OutputPath,
                prefix_name=SN,
                save_raw=False,
                save_result=SaveResult,
                save_calibration=False,
                convert_16bit=False
            )
            # print('luminance_measurement')
            iR = self.luminance_measurement(
                color_filter=color_filter,
                exposure=self.exposure,
                module_id=self.module_id,
                cali_config=cali_config,
                save_config=save_config,
            )
            print(f'iR.max_val:{iR.max_val}')
            # # capture data dict, use to contain capture data of clear filter
            # capture_data_dict = dict()
            # # move color filter
            # ret = self.ml_mono.ml_move_xyz_syn(color_filter)
            # if not ret.success:
            #     raise RuntimeError("ml_move_xyz_syn error")

            # # set exposure by pyExposureSetting
            # ret = self.ml_mono.ml_set_exposure(exposure=self.exposure)
            # if not ret.success:
            #     raise RuntimeError("ml_set_exposure error")

            # # capture single image from camera
            # ret = self.ml_mono.ml_capture_image_syn()
            # if not ret.success:
            #     raise RuntimeError("ml_capture_image_syn error")

            # # get capture data after calling ml_capture_image_syn, contains image and instrument info
            # capture_data = self.ml_mono.ml_get_CaptureData()
            # # insert data into capture_data_dict container
            # capture_data_dict[color_filter] = capture_data
            # self.exposure.exposure_time = capture_data.exposure_time
            # iR.exposuretime = capture_data.exposure_time
            # iR.max_val = np.max(capture_data.image)
            # iR.name = color_filter    

            # # set capture data for measurement
            # ret = self.ml_colorimeter.ml_set_CaptureData(module_id=self.module_id, data=capture_data_dict)
            # if not ret.success:
            #     raise RuntimeError("ml_set_CaptureData error")

            # # load calibration data by calbration config
            # ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
            # if not ret.success:
            #     raise RuntimeError("ml_load_calibration_data error")

            # # execute calibration process for capture data by calibration config
            # ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
            # if not ret.success:
            #     raise RuntimeError("ml_image_process error")

            # # get calibration data after calibration process
            # processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=self.module_id)

            # # save calibration data
            # print('ml_save_processed_data')
            # ret = self.ml_colorimeter.ml_save_processed_data(
            #     module_id=self.module_id, processed_data=processed_data, save_config=save_config
            # )
            # if not ret.success:
            #     raise RuntimeError("ml_save_processed_data error")
            # print('ml_save_processed_data Done')
            
            # iR.image = processed_data[mlcm.CalibrationEnum.Luminance][color_filter].image
            return iR
        except Exception as e:
            print(e)
            return iR
    def Capture_chromaticity_luminance_Image(self, OutputPath, SN, sRGBW, iBinn, iND, iExposureMode = 1, ExpT_X = 100, ExpT_Y = 100, ExpT_Z = 100, VID = 9999999) -> list[ImageData]:
        print('Capture chromaticity Image')
        imgData = []
        try:
            print(f'OutputPath : {OutputPath}')
            print(f'SN : {SN}')
            self.Set_ND(iND)
            self.Set_ExposureTime_XYZ(iExposureMode, ExpT_X, ExpT_Y, ExpT_Z)
            self.Set_pixel_format()
            self.Set_Binn(iBinn)
             

            xyz_list = [mlcm.MLFilterEnum.X, mlcm.MLFilterEnum.Y, mlcm.MLFilterEnum.Z]
            rx = mlcm.pyRXCombination(sph=0, cyl=0, axis=0)
            # ret = self.ml_mono.ml_set_rx_syn(rx)
            # if not ret.success:
            #     raise RuntimeError("ml_set_rx_syn error")
            aperture = self.aperture
            ret = self.ml_mono.ml_set_aperture(aperture)

            light_source= sRGBW
            ret = self.ml_mono.ml_set_light_source(light_source)

            self.set_vid(sRGBW, VID)
            # take a measurement for module_id
            # calibration config for measurement
            cali_config = mlcm.pyCalibrationConfig(
                input_path=self.eye1_path,
                aperture=self.ml_mono.ml_get_aperture(),
                binn=self.Get_Binn(),
                light_source_list = [self.ml_mono.ml_get_light_source()],
                nd_filter_list=[self.Get_ND()],
                color_filter_list=xyz_list,
                rx=rx,
                dark_flag=True,
                ffc_flag=True,
                color_shift_flag=True,
                distortion_flag=True,
                exposure_flag=True,
                four_color_flag=True,
                # flip_rotate_flag = True
            )
            SaveResult = True
            if OutputPath == '':
                SaveResult = False

            # save config
            save_config = mlcm.pySaveDataConfig(
                save_path=OutputPath,
                prefix_name=SN,
                save_raw=False,
                save_result=SaveResult,
                save_calibration=False,
                convert_16bit=False
            )
            # excute chromaticity and luminance measurement for X,Y,Z filter
            imgData = self.chromaticity_luminance_measurement(
                xyz_list=xyz_list,
                exposure_map=self.exposure_map,
                module_id=self.module_id,
                cali_config=cali_config,
                save_config=save_config,
            )
            return imgData
        except Exception as e:
            print(e)
            return imgData
    #Some problem, don't use
    def Capture_hdr_chromaticity_luminance_Image(self, OutputPath, SN, sRGBW, iBinn, iND, max_exposuretime = 1000, VID = 9999999) -> list[ImageData]:
        print('Capture chromaticity luminance hdr Image')
        imgData = []
        try:
            print(f'OutputPath : {OutputPath}')
            print(f'SN : {SN}')
            self.Set_ND(iND)
            # self.Set_ExposureTime_XYZ(iExposureMode, ExpT_X, ExpT_Y, ExpT_Z)
            self.Set_pixel_format()
            self.Set_Binn(iBinn)
             

            xyz_list = [mlcm.MLFilterEnum.X, mlcm.MLFilterEnum.Y, mlcm.MLFilterEnum.Z]
            rx = mlcm.pyRXCombination(sph=0, cyl=0, axis=0)
            # ret = self.ml_mono.ml_set_rx_syn(rx)
            # if not ret.success:
            #     raise RuntimeError("ml_set_rx_syn error")
            aperture = self.aperture
            ret = self.ml_mono.ml_set_aperture(aperture)

            light_source= sRGBW
            ret = self.ml_mono.ml_set_light_source(light_source)

            self.set_vid(sRGBW, VID)

            self.dark_image = cv2.imread(self.eye1_path+"\\Dark\\Dark.tif", -1)
            self.max_ET = 1000

            # take a measurement for module_id
            # calibration config for measurement
            cali_config = mlcm.pyCalibrationConfig(
                input_path=self.eye1_path,
                aperture=self.ml_mono.ml_get_aperture(),
                binn=self.Get_Binn(),
                light_source_list = [self.ml_mono.ml_get_light_source()],
                nd_filter_list=[self.Get_ND()],
                color_filter_list=xyz_list,
                rx=rx,
                dark_flag=True,
                ffc_flag=True,
                color_shift_flag=True,
                distortion_flag=True,
                exposure_flag=True,
                four_color_flag=True,
                # flip_rotate_flag = True
            )
            SaveResult = True
            if OutputPath == '':
                SaveResult = False

            # save config
            save_config = mlcm.pySaveDataConfig(
                save_path=OutputPath,
                prefix_name=SN,
                save_raw=False,
                save_result=SaveResult,
                save_calibration=False,
                convert_16bit=False
            )
            dataA = []
            _dataX = mlcm.pyCaptureData(
                aperture=self.ml_mono.ml_get_aperture(),
                light_source=self.ml_mono.ml_get_light_source(), 
                binn=self.Get_Binn(),
                nd_filter=self.Get_ND(), 
                color_filter=mlcm.MLFilterEnum.X,
                rx=rx,
                exposure_time=1.0,
                image=[],
            )
            _dataY = mlcm.pyCaptureData(
                aperture=self.ml_mono.ml_get_aperture(),
                light_source=self.ml_mono.ml_get_light_source(), 
                binn=self.Get_Binn(),
                nd_filter=self.Get_ND(), 
                color_filter=mlcm.MLFilterEnum.Y,
                rx=rx,
                exposure_time=1.0,
                image=[],
            )
            _dataZ = mlcm.pyCaptureData(
                aperture=self.ml_mono.ml_get_aperture(),
                light_source=self.ml_mono.ml_get_light_source(), 
                binn=self.Get_Binn(),
                nd_filter=self.Get_ND(), 
                color_filter=mlcm.MLFilterEnum.Z,
                rx=rx,
                exposure_time=1.0,
                image=[],
            )
            dataA.append(_dataX)
            dataA.append(_dataY)
            dataA.append(_dataZ)

            # print(f'dataA len : {len(dataA)}')
            # print(f'd{dataA[0].color_filter}')
            # print(f'd{dataA[1].color_filter}')
            # print(f'd{dataA[2].color_filter}')
            # return imgData
            # excute chromaticity and luminance measurement for X,Y,Z filter
            imgData = self.hdr_chromaticity_luminance_measurement(
                xyz_list=xyz_list,
                exposure_map=self.exposure_map,
                module_id=self.module_id,
                cali_config=cali_config,
                save_config=save_config,
                dataA = dataA
            )
            return imgData
        except Exception as e:
            print(e)
            return imgData
    def Capture_hdr_luminance_Image(self, OutputPath, SN, sRGBW, iCXYZ, iBinn, iND, max_exposuretime = 1000, luminance = True, VID = 9999999) -> ImageData:
        iR = ImageData(name='',exposuretime=0,image=[],max_val=0)
        try:
            
            self.Set_ND(iND)
            self.Set_pixel_format()
            self.Set_Binn(iBinn)

            # xyz filter to switch during measurement
            if iCXYZ == 0:
                color_filter = mlcm.MLFilterEnum.Clear
            elif iCXYZ == 1:
                color_filter = mlcm.MLFilterEnum.X
            elif iCXYZ == 2:
                color_filter = mlcm.MLFilterEnum.Y
            elif iCXYZ == 3:
                color_filter = mlcm.MLFilterEnum.Z
            else:
                color_filter = mlcm.MLFilterEnum.Clear

            rx = mlcm.pyRXCombination(sph=0, cyl=0, axis=0)
            # ret = self.ml_mono.ml_set_rx_syn(rx)
            # if not ret.success:
            #     raise RuntimeError("ml_set_rx_syn error")
            aperture = self.aperture
            ret = self.ml_mono.ml_set_aperture(aperture)
            light_source = sRGBW
            if light_source == "W":
                light_source = "G"
            ret = self.ml_mono.ml_set_light_source(light_source)

            self.set_vid(sRGBW, VID)
            
            self.Set_Color_Filter(color_filter)
            
            self.dark_image = cv2.imread(self.eye1_path+"\\Dark\\Dark.tif", -1)
            self.max_ET = max_exposuretime
            # take a luminance measurement for module_id
            # calibration config for luminance measurement
            cali_config = mlcm.pyCalibrationConfig(
                input_path=self.eye1_path,
                aperture=self.ml_mono.ml_get_aperture(),
                binn=self.Get_Binn(),
                nd_filter_list=[self.Get_ND()],
                color_filter_list=[color_filter],
                rx=rx,
                light_source_list=[self.ml_mono.ml_get_light_source()],
                dark_flag=True,
                ffc_flag=True,
                color_shift_flag=True,
                distortion_flag=True,
                exposure_flag=luminance,
                luminance_flag=luminance,
                # flip_rotate_flag = True
            )
            # save config
            SaveResult = True
            if OutputPath == '':
                SaveResult = False
            # save config
            save_config = mlcm.pySaveDataConfig(
                save_path=OutputPath,
                prefix_name=SN,
                save_raw=False,
                save_result=SaveResult,
                save_calibration=False,
                convert_16bit=False
            )
            # load calibration data by calbration config
            ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
            if not ret.success:
                raise RuntimeError("ml_load_calibration_data error")

            _data = mlcm.pyCaptureData(
                aperture=aperture, 
                light_source=light_source, 
                binn=self.Get_Binn(),
                nd_filter=self.Get_ND(), 
                color_filter=color_filter, 
                rx=rx,
                exposure_time=1.0,
                image=[],
            )
            # excute hdr measurement
            iR = self.hdr_luminance_measurement(
                module_id=self.module_id,
                cali_config=cali_config,
                save_config=save_config,
                _data = _data
            )
            return iR
        except Exception as e:
            print(e)
            return iR
    def Capture_RawImageOnly(self, sRGBW, iCXYZ, iBinn, iND, imageprocess = True, iExposureMode = 1, ExpT = 100, VID = 9999999) -> ImageData:
        print('Capture_RawImageOnly')
        try:
            iR = ImageData(name='',exposuretime=0,image=[],max_val=0)
            self.Set_ND(iND)
            self.Set_ExposureTime_Single(iExposureMode, ExpT)
            self.Set_pixel_format()
            self.Set_Binn(iBinn)

            # xyz filter to switch during measurement
            if iCXYZ == 0:
                color_filter = mlcm.MLFilterEnum.Clear
            elif iCXYZ == 1:
                color_filter = mlcm.MLFilterEnum.X
            elif iCXYZ == 2:
                color_filter = mlcm.MLFilterEnum.Y
            elif iCXYZ == 3:
                color_filter = mlcm.MLFilterEnum.Z
            else:
                color_filter = mlcm.MLFilterEnum.Clear

            rx = mlcm.pyRXCombination(sph=0, cyl=0, axis=0)
            # ret = self.ml_mono.ml_set_rx_syn(rx)
            # if not ret.success:
            #     raise RuntimeError("ml_set_rx_syn error")
            aperture = self.aperture
            ret = self.ml_mono.ml_set_aperture(aperture)
            light_source = sRGBW
            ret = self.ml_mono.ml_set_light_source(light_source)
            self.set_vid(sRGBW, VID)

            # take a luminance measurement for module_id
            # calibration config for luminance measurement
            cali_config = mlcm.pyCalibrationConfig(
                input_path=self.eye1_path,
                aperture=self.ml_mono.ml_get_aperture(),
                binn=self.Get_Binn(),
                nd_filter_list=[self.Get_ND()],
                color_filter_list=[color_filter],
                rx=rx,
                light_source_list=[self.ml_mono.ml_get_light_source()],
                dark_flag=True,
                ffc_flag=True,
                color_shift_flag=True,
                distortion_flag=True,
                exposure_flag=False,
                luminance_flag=False
            )

            iR = self.RawImage_Capture(
                color_filter=color_filter,
                exposure=self.exposure,
                module_id=self.module_id,
                cali_config = cali_config,
                ImageProcess=imageprocess
            )
            return iR
        except Exception as e:
            print(e)
            return iR
    def chromaticity_luminance_measurement(
            self,
            xyz_list: List[mlcm.MLFilterEnum],
            exposure_map: Dict[mlcm.MLFilterEnum, mlcm.pyExposureSetting],
            module_id: int,
            cali_config: mlcm.pyCalibrationConfig,
            save_config: mlcm.pySaveDataConfig,
        ) -> list[ImageData]:
        # capture data dict, use to contain X, Y, Z capture data
        capture_data_dict = dict()
        imgData = []
        # capture data for X, Y, Z
        for xyz in xyz_list:
            # move color filter
            ret = self.ml_mono.ml_move_xyz_syn(xyz)
            if not ret.success:
                raise RuntimeError("ml_move_xyz_syn error")
            # set exposure by pyExposureSetting
            ret = self.ml_mono.ml_set_exposure(exposure=exposure_map[xyz])
            if not ret.success:
                raise RuntimeError("ml_set_exposure error")
            # capture single image from camera
            ret = self.ml_mono.ml_capture_image_syn()
            if not ret.success:
                raise RuntimeError("ml_capture_image_syn error")
            # get capture data after calling ml_capture_image_syn, contains image and instrument info
            capture_data = self.ml_mono.ml_get_CaptureData()
            # insert data into capture_data_dict container
            capture_data_dict[xyz] = capture_data
            print(f'{capture_data.exposure_time}')
            exposure_map[xyz].exposure_time = capture_data.exposure_time
            # imgD = ImageData
            # imgD.exposuretime = capture_data.exposure_time
            # imgD.max_val = np.max(capture_data.image)
            # imgD.name = xyz
            imgData.append(ImageData(exposuretime = capture_data.exposure_time, max_val = np.max(capture_data.image), name=xyz, image=[]))
            # imgData.append(ImageData(exposuretime = capture_data.exposure_time, max_val = np.max(capture_data.image), name=xyz, image=[], graystatus=self.Cal_Gray_Status(capture_data.image)))
        # set capture data for measurement
        ret = self.ml_colorimeter.ml_set_CaptureData(module_id=module_id, data=capture_data_dict)
        if not ret.success:
            raise RuntimeError("ml_set_CaptureData error")

        # load calibration data by calbration config
        ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_load_calibration_data error")

        # execute calibration process for capture data by calibration config
        ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_image_process error")
        # processed_data = Dict[self.ml_colorimeter.CalibrationEnum, Dict[self.ml_colorimeter.MLFilterEnum, self.ml_colorimeter.pyProcessedData]]
        # get processed data after calibration process
        processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)
        # print(processed_data)
        all_calibration_enums_list = list(processed_data.keys())
        all_ml_filter_enums = {
            ml_filter 
            for inner_dict in processed_data.values() 
            for ml_filter in inner_dict.keys()
        }
        print(all_calibration_enums_list)
        print(all_ml_filter_enums)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.X].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.X].image)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Y].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Y].image)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Z].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Z].image)


        # try:
            # print(processed_data[mlcm.CalibrationEnum.FourColor])
            # print(processed_data[mlcm.CalibrationEnum.FourColor][mlcm.MLFilterEnum.X].image)
            # out_data = dict()
            # for key1, value1 in processed_data.items():
            #     tdata = dict()
            #     print(f'{key1}')
            #     for key2, value2 in value1.items():
            #         print(f'{key2}')
            #         tdata[key2] = value2.get_data()
        #     #     out_data[key1] = tdata
        # except Exception as e:
        #     print(e)

        # save calibration data
        ret = self.ml_colorimeter.ml_save_processed_data(
            module_id=module_id, processed_data=processed_data, save_config=save_config
        )
        if not ret.success:
            raise RuntimeError("ml_save_processed_data error")
        print('chromaticity_luminance_measurement done')

        
        imgData[0].image = processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.X].image
        imgData[1].image = processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Y].image
        imgData[2].image = processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Z].image
        # cv2.imwrite(r"D:\Z23A\test3\save_X.tif", imgData[0].image)
        # cv2.imwrite(r"D:\Z23A\test3\save_Y.tif", imgData[1].image)
        # cv2.imwrite(r"D:\Z23A\test3\save_Z.tif", imgData[2].image)
        return imgData
    def hdr_chromaticity_luminance_measurement(
            self,
            xyz_list: List[mlcm.MLFilterEnum],
            exposure_map: Dict[mlcm.MLFilterEnum, mlcm.pyExposureSetting],
            module_id: int,
            cali_config: mlcm.pyCalibrationConfig,
            save_config: mlcm.pySaveDataConfig,
            dataA : list[mlcm.pyCaptureData]
        ) -> list[ImageData]:
        # capture data dict, use to contain X, Y, Z capture data
        capture_data_dict = dict()
        imgData = []
        iXYZ = 0
        # capture data for X, Y, Z
        for xyz in xyz_list:
            print(f'{xyz}')
            # move color filter
            ret = self.ml_mono.ml_move_xyz_syn(xyz)
            if not ret.success:
                raise RuntimeError("ml_move_xyz_syn error")
            
            hdr_img, ret = self.ml_mono.ml_capture_HDR(self.dark_image, self.max_ET)
            dataA[iXYZ].image = hdr_img
            # # set exposure by pyExposureSetting
            # ret = self.ml_mono.ml_set_exposure(exposure=exposure_map[xyz])
            # if not ret.success:
            #     raise RuntimeError("ml_set_exposure error")
            # # capture single image from camera
            # ret = self.ml_mono.ml_capture_image_syn()
            # if not ret.success:
            #     raise RuntimeError("ml_capture_image_syn error")
            # # get capture data after calling ml_capture_image_syn, contains image and instrument info
            # capture_data = self.ml_mono.ml_get_CaptureData()
            # insert data into capture_data_dict container
            capture_data_dict[xyz] = dataA[iXYZ]
            # imgD = ImageData
            # imgD.exposuretime = capture_data.exposure_time
            # imgD.max_val = np.max(capture_data.image)
            # imgD.name = xyz
            imgData.append(ImageData(exposuretime = 0, max_val = np.max(dataA[iXYZ].image), name=xyz, image=[]))
            # imgData.append(ImageData(exposuretime = 0, max_val = np.max(dataA[iXYZ].image), name=xyz, image=[], graystatus=self.Cal_Gray_Status(dataA[iXYZ].image)))
            iXYZ += 1
        print('ml_set_CaptureData')
        # set capture data for measurement
        ret = self.ml_colorimeter.ml_set_CaptureData(module_id=module_id, data=capture_data_dict)
        if not ret.success:
            raise RuntimeError("ml_set_CaptureData error")
        print('ml_load_calibration_data')
        # load calibration data by calbration config
        ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_load_calibration_data error")
        print('ml_image_process')
        # execute calibration process for capture data by calibration config
        ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_image_process error")
        # processed_data = Dict[self.ml_colorimeter.CalibrationEnum, Dict[self.ml_colorimeter.MLFilterEnum, self.ml_colorimeter.pyProcessedData]]
        # get processed data after calibration process
        print('ml_get_processed_data')
        processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.X].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.X].image)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Y].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Y].image)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Z].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Z].image)


        # try:
            # print(processed_data[mlcm.CalibrationEnum.FourColor])
            # print(processed_data[mlcm.CalibrationEnum.FourColor][mlcm.MLFilterEnum.X].image)
            # out_data = dict()
            # for key1, value1 in processed_data.items():
            #     tdata = dict()
            #     print(f'{key1}')
            #     for key2, value2 in value1.items():
            #         print(f'{key2}')
            #         tdata[key2] = value2.get_data()
        #     #     out_data[key1] = tdata
        # except Exception as e:
        #     print(e)
        print('ml_save_processed_data')
        # save calibration data
        ret = self.ml_colorimeter.ml_save_processed_data(
            module_id=module_id, processed_data=processed_data, save_config=save_config
        )
        if not ret.success:
            raise RuntimeError("ml_save_processed_data error")
        print('chromaticity_luminance_measurement done')
        
        imgData[0].image = processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.X].image
        imgData[1].image = processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Y].image
        imgData[2].image = processed_data[mlcm.CalibrationEnum.FinalResult][mlcm.MLFilterEnum.Z].image
        print('return imgData')
        return imgData
    def luminance_measurement(
            self,
            color_filter: mlcm.MLFilterEnum,
            exposure: mlcm.pyExposureSetting,
            module_id: int,
            cali_config: mlcm.pyCalibrationConfig,
            save_config: mlcm.pySaveDataConfig,
        ) -> ImageData:
        # capture data dict, use to contain capture data of clear filter
        capture_data_dict = dict()
        imgD = ImageData
        # move color filter
        ret = self.ml_mono.ml_move_xyz_syn(color_filter)
        if not ret.success:
            raise RuntimeError("ml_move_xyz_syn error")

        # set exposure by pyExposureSetting
        ret = self.ml_mono.ml_set_exposure(exposure=exposure)
        if not ret.success:
            raise RuntimeError("ml_set_exposure error")

        # capture single image from camera
        ret = self.ml_mono.ml_capture_image_syn()
        if not ret.success:
            raise RuntimeError("ml_capture_image_syn error")

        # get capture data after calling ml_capture_image_syn, contains image and instrument info
        capture_data = self.ml_mono.ml_get_CaptureData()
        # insert data into capture_data_dict container
        capture_data_dict[color_filter] = capture_data
        exposure.exposure_time = capture_data.exposure_time
        imgD.exposuretime = capture_data.exposure_time
        imgD.max_val = int(np.max(capture_data.image))
        print(f'max_val:{imgD.max_val}')
        imgD.name = color_filter    
        # imgD.graystatus = self.Cal_Gray_Status(capture_data.image)

        # set capture data for measurement
        ret = self.ml_colorimeter.ml_set_CaptureData(module_id=module_id, data=capture_data_dict)
        if not ret.success:
            raise RuntimeError("ml_set_CaptureData error")

        # load calibration data by calbration config
        ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_load_calibration_data error")

        # execute calibration process for capture data by calibration config
        ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_image_process error")

        # get calibration data after calibration process
        processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)
        all_calibration_enums_list = list(processed_data.keys())
        all_ml_filter_enums = {
            ml_filter 
            for inner_dict in processed_data.values() 
            for ml_filter in inner_dict.keys()
        }
        print(all_calibration_enums_list)
        print(all_ml_filter_enums)

        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][color_filter].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][color_filter].image)
        # save calibration data
        print('ml_save_processed_data')
        ret = self.ml_colorimeter.ml_save_processed_data(
            module_id=module_id, processed_data=processed_data, save_config=save_config
        )
        if not ret.success:
            raise RuntimeError("ml_save_processed_data error")
        print('ml_save_processed_data Done')
        
        imgD.image = processed_data[mlcm.CalibrationEnum.FinalResult][color_filter].image
        # cv2.imwrite(r"D:\Z23A\test3\T132.tif", imgD.image)
        return imgD
    def RawImage_Capture(
            self,
            color_filter: mlcm.MLFilterEnum,
            exposure: mlcm.pyExposureSetting,
            module_id: int,
            cali_config: mlcm.pyCalibrationConfig,
            ImageProcess:bool
        ) -> ImageData:
        # capture data dict, use to contain capture data of clear filter
        capture_data_dict = dict()
        # move color filter
        ret = self.ml_mono.ml_move_xyz_syn(color_filter)
        if not ret.success:
            raise RuntimeError("ml_move_xyz_syn error")

        # set exposure by pyExposureSetting
        ret = self.ml_mono.ml_set_exposure(exposure=exposure)
        if not ret.success:
            raise RuntimeError("ml_set_exposure error")

        # capture single image from camera
        ret = self.ml_mono.ml_capture_image_syn()
        if not ret.success:
            raise RuntimeError("ml_capture_image_syn error")

        # get capture data after calling ml_capture_image_syn, contains image and instrument info
        capture_data = self.ml_mono.ml_get_CaptureData()
        # insert data into capture_data_dict container
        capture_data_dict[color_filter] = capture_data
        exposure.exposure_time = capture_data.exposure_time
        imgD = ImageData
        imgD.max_val = np.max(capture_data.image)
        imgD.name = color_filter
        imgD.exposuretime = capture_data.exposure_time
        # imgD.graystatus = self.Cal_Gray_Status(capture_data.image)

        if not ImageProcess:
            imgD.image = capture_data.image
            return imgD

        # set capture data for measurement
        ret = self.ml_colorimeter.ml_set_CaptureData(module_id=module_id, data=capture_data_dict)
        if not ret.success:
            raise RuntimeError("ml_set_CaptureData error")

        # load calibration data by calbration config
        ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_load_calibration_data error")

        # execute calibration process for capture data by calibration config
        ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_image_process error")

        # get calibration data after calibration process
        processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][color_filter].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][color_filter].image)
        # # save calibration data
        # ret = self.ml_colorimeter.ml_save_processed_data(
        #     module_id=module_id, processed_data=processed_data, save_config=save_config
        # )
        # if not ret.success:
        #     raise RuntimeError("ml_save_processed_data error")
        imgD.image = processed_data[mlcm.CalibrationEnum.FinalResult][color_filter].image
        return imgD  
    def hdr_luminance_measurement(
            self,
            module_id: int,
            cali_config: mlcm.pyCalibrationConfig,
            save_config: mlcm.pySaveDataConfig,
            _data: mlcm.pyCaptureData
        ) -> ImageData:
        print('hdr_luminance_measurement')
        # capture data dict, use to contain capture data of clear filter
        capture_data_dict = dict()
        imgD = ImageData
        hdr_img, ret = self.ml_mono.ml_capture_HDR(self.dark_image, self.max_ET)
        _data.image = hdr_img
        # get capture data after calling ml_capture_image_syn, contains image and instrument info
        # exposure_time set to 1.0
        # insert data into capture_data_dict container
        capture_data_dict[_data.color_filter] = _data
        imgD.name = _data.color_filter
        imgD.max_val = np.max(hdr_img)
        
        # imgD.graystatus = self.Cal_Gray_Status(hdr_img)
        # set capture data for measurement
        ret = self.ml_colorimeter.ml_set_CaptureData(
            module_id=module_id, data=capture_data_dict)
        if not ret.success:
            raise RuntimeError("ml_set_CaptureData error")
        # set dark_flag to False
        cali_config.dark_flag = False
        # execute calibration process for capture data by calibration config
        ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
        if not ret.success:
            raise RuntimeError("ml_image_process error")
        # get calibration data after calibration process
        processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)
        cv2.rotate(processed_data[mlcm.CalibrationEnum.FinalResult][_data.color_filter].image, cv2.ROTATE_180, processed_data[mlcm.CalibrationEnum.FinalResult][_data.color_filter].image)
        imgD.image = processed_data[mlcm.CalibrationEnum.FinalResult][_data.color_filter].image
        # save calibration data
        ret = self.ml_colorimeter.ml_save_processed_data(
            module_id=module_id, processed_data=processed_data, save_config=save_config
        )
        if not ret.success:
            raise RuntimeError("ml_save_processed_data error")
        # imgD.image = processed_data[mlcm.CalibrationEnum.FinalResult][_data.color_filter].image
        
        # cv2.imwrite(r"E:\SDK_test_data\HDR\iR_T.tif", imgD.image)
        # =======================================================================================================

        # # set capture data for measurement
        # ret = self.ml_colorimeter.ml_set_CaptureData(
        #     module_id=module_id, data=capture_data_dict)
        # if not ret.success:
        #     raise RuntimeError("ml_set_CaptureData error")

        # # set dark_flag to False
        # cali_config.dark_flag = False
        # cali_config.exposure_flag=False
        # cali_config.luminance_flag=False
        # # execute calibration process for capture data by calibration config
        # ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
        # if not ret.success:
        #     raise RuntimeError("ml_image_process error")

        # # get calibration data after calibration process
        # processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)

        # # save calibration data
        # ret = self.ml_colorimeter.ml_save_processed_data(
        #     module_id=module_id, processed_data=processed_data, save_config=save_config
        # )
        # if not ret.success:
        #     raise RuntimeError("ml_save_processed_data error")
        # imgD.image = imgD.image = processed_data[mlcm.CalibrationEnum.FinalResult][_data.color_filter].image
        return imgD
    #endregion

# def luminance_measurement(
    #         self,
    #         color_filter: mlcm.MLFilterEnum,
    #         exposure: mlcm.pyExposureSetting,
    #         module_id: int,
    #         cali_config: mlcm.pyCalibrationConfig,
    #         save_config: mlcm.pySaveDataConfig,
    #     ) -> ImageData:
    #     # capture data dict, use to contain capture data of clear filter
    #     capture_data_dict = dict()
    #     imgD = ImageData
    #     # move color filter
    #     ret = self.ml_mono.ml_move_xyz_syn(color_filter)
    #     if not ret.success:
    #         raise RuntimeError("ml_move_xyz_syn error")

    #     # set exposure by pyExposureSetting
    #     ret = self.ml_mono.ml_set_exposure(exposure=exposure)
    #     if not ret.success:
    #         raise RuntimeError("ml_set_exposure error")

    #     # capture single image from camera
    #     ret = self.ml_mono.ml_capture_image_syn()
    #     if not ret.success:
    #         raise RuntimeError("ml_capture_image_syn error")

    #     # get capture data after calling ml_capture_image_syn, contains image and instrument info
    #     capture_data = self.ml_mono.ml_get_CaptureData()
    #     # insert data into capture_data_dict container
    #     capture_data_dict[color_filter] = capture_data
    #     exposure.exposure_time = capture_data.exposure_time
    #     imgD.exposuretime = capture_data.exposure_time
    #     imgD.max_val = np.max(capture_data.image)
    #     imgD.name = color_filter

    #     # set capture data for measurement
    #     ret = self.ml_colorimeter.ml_set_CaptureData(module_id=module_id, data=capture_data_dict)
    #     if not ret.success:
    #         raise RuntimeError("ml_set_CaptureData error")

    #     # load calibration data by calbration config
    #     ret = self.ml_colorimeter.ml_load_calibration_data(cali_config=cali_config)
    #     if not ret.success:
    #         raise RuntimeError("ml_load_calibration_data error")

    #     # execute calibration process for capture data by calibration config
    #     ret = self.ml_colorimeter.ml_image_process(cali_config=cali_config)
    #     if not ret.success:
    #         raise RuntimeError("ml_image_process error")

    #     # get calibration data after calibration process
    #     processed_data = self.ml_colorimeter.ml_get_processed_data(module_id=module_id)

    #     # save calibration data
    #     ret = self.ml_colorimeter.ml_save_processed_data(
    #         module_id=module_id, processed_data=processed_data, save_config=save_config
    #     )
    #     if not ret.success:
    #         raise RuntimeError("ml_save_processed_data error")
    #     imgD.image = processed_data[mlcm.CalibrationEnum.Luminance][color_filter].image
    #     return imgD
    def Cal_Gray_Status(self, img):
        res = self.ml_mono.ml_cal_gray_status(img)
        # print(type(res))
        # print(mlcm.GrayStatus.__dict__)
        # print(list(mlcm.GrayStatus.__dict__.keys()))
        return res
    
if __name__ == "__main__":
    # # set mono module calibration configuration path
    # eye1_path = r"D:\MLOptic\MLColorimeter\config\EYE1"
    eye1_path = r"C:\MLO_Install\MLO_Driver\M25415S260501M_config_1b_average\EYE1"
    path_list = [
        eye1_path,
    ]
    try:
        mlo = MLO_Camera(eye1_path)


        mlo.set_vid("G", )
        # img = cv2.imread(r'D:\Quanta\Project\Z23A\MLO\SaveImage\TTT_ND0_Clear_G_Raw.tif', cv2.IMREAD_UNCHANGED)
        # print(len(img))
        # print(img.shape)
        # print(np.max(img))
        # mlo.Cal_Gray_Status(img)
        # mlo.Get_Camera_Setting()
        # mlo.Capture_chromaticity_luminance_Image(r"E:\SDK_test_data\test3", 1, 0)
        # mlo.Capture_chromaticity_luminance_Image('', 1, 0)
        # iR = mlo.Capture_chromaticity_luminance_Image(r'E:\SDK_test_data\HDR', 'Align', 'G',  1, 1)
        # iR = mlo.Capture_chromaticity_luminance_Image(r'', 'Align', 'G',  1, 1)
        # print(iR[0].name)
        # print(iR[1].name)
        # print(iR[2].name)
        # cv2.imwrite(r'E:\SDK_test_data\test3\TX.tif', iR[0].image)
        # cv2.imwrite(r'E:\SDK_test_data\test3\TY.tif', iR[1].image)
        # cv2.imwrite(r'E:\SDK_test_data\test3\TZ.tif', iR[2].image)
        # mlo.Capture_chromaticity_luminance_Image(r"D:\Z23A\test3", "TTT", "G", 4, 2, 2, 766.6639, 111.1082, 3455.519)


        # mlo.set_vid("G", 200)
        # vid = mlo.get_vid()
        # print(f'get vid : {vid}')
        # iR = mlo.Capture_luminance_Image(r"D:\Z23A\test3", f'G_16531456', 'G', 0, 4, 2)

        # mlo.set_vid("G", 2000)
        # vid = mlo.get_vid()
        # print(f'get vid : {vid}')

        # iR = mlo.Capture_luminance_Image(r"D:\Z23A\test3", f'R', 'R', 0, 1, 2, 1, 11.111)
        # print(f'{iR.exposuretime}')
        # iR = mlo.Capture_luminance_Image(r"", '', 'G', 0, 1, 1)
        # print(iR)
        # iR = mlo.Capture_RawImageOnly('G', 2, 1, 0, True, 2, 200)
        # print(f'{iR.exposuretime}')
        # print(f'{mlo.exposure.exposure_time}')
        # mlo.Get_Camera_Setting()
        # cv2.imwrite(r'D:\Z23A\test3\iR.tif', iR.image)
        # print(iR)
        # print(iR.name)
        # print(iR.max_val)
        # iR = mlo.Capture_hdr_chromaticity_luminance_Image(r"D:\Z23A\test3", 'G_HDR_XYZ', 'G', 4, 2, 5000)
        # iR = mlo.Capture_hdr_luminance_Image(r"", '', 'G', 0, 4, 2, 5000)
        # cv2.imwrite(r'D:\Z23A\test3\TYYY.tif', iR.image)
        # cv2.imwrite(r'D:\Z23A\test3\TY.tif', iR[1].image)
        # cv2.imwrite(r'D:\Z23A\test3\TZ.tif', iR[2].image)
        # OutputPath, SN, sRGBW, iBinn, iND, max_exposuretime = 1000
        # iR = mlo.Capture_hdr_luminance_Image(r"D:\Z23A\test3", 'G_HDR', 'G', 0, 4, 2, 5000)
        # cv2.imwrite(r"E:\SDK_test_data\HDR\iR.tif", iR.image)
    except Exception as e:
        print(e)