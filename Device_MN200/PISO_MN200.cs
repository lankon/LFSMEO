using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Device_MN200.PISO_MN200
{
	public class Param
	{
		// Communication Speed Configuration
		public const Byte COMMSPEED_2_5M = 0;
		public const Byte COMMSPEED_5M = 1;
		public const Byte COMMSPEED_10M = 2;
		public const Byte COMMSPEED_20M = 3;

		// Output Pulse Configuration
		public const Byte PULSE_MODE_PULSE_LOGIC_LOW_DIR_FORWARD_HIGH = 0;
		public const Byte PULSE_MODE_PULSE_LOGIC_HIGH_DIR_FORWARD_HIGH = 1;
		public const Byte PULSE_MODE_PULSE_LOGIC_LOW_DIR_FORWARD_LOW = 2;
		public const Byte PULSE_MODE_PULSE_LOGIC_HIGH_DIR_FORWARD_LOW = 3;

		public const Byte PULSE_MODE_CW_LOGIC_LOW = 4;
		public const Byte PULSE_MODE_CW_LOGIC_HIGH = 7;

		public const Byte PULSE_MODE_A_LEAD_B = 5;
		public const Byte PULSE_MODE_A_LAG_B = 6;

		// Encoder Configuration
		public const Byte ENCODER_MODE_AB = 0; // Quadrature pulse input
		public const Byte ENCODER_MODE_AB_MULT_2 = 1;
		public const Byte ENCODER_MODE_AB_MULT_4 = 2;
		public const Byte ENCODER_MODE_CW_CCW = 3;  // Up/Down pulse input

		// Motion related IO Configuration
		public const Byte SLOWDOWN = 0;
		public const Byte SUDDEN_STOP = 0;
		public const Byte SLOWDOWN_STOP = 1;

		public const Byte DISABLE_FEATURE = 0;
		public const Byte ENABLE_FEATURE = 1;

		public const Byte LOGIC_ACTIVE_LOW = 0;
		public const Byte LOGIC_ACTIVE_HIGH = 1;

		public const Byte ERC_LEN_12us = 0;
		public const Byte ERC_LEN_93us = 1;
		public const Byte ERC_LEN_371us = 2;
		public const Byte ERC_LEN_1500us = 3;
		public const Byte ERC_LEN_12ms = 4;
		public const Byte ERC_LEN_48ms = 5;
		public const Byte ERC_LEN_95ms = 6;
		public const Byte ERC_LEN_LEVEL_OUTPUT = 7;

		// Filter Selection
		public const Byte FILTER_DATA_EMG_EL_SD_ORG_ALM_INP = 0;
		public const Byte FILTER_DATA_ENCODER = 1;

		// Cmp/Ring Source Selection
		public const Byte PULSE_COMMAND = 0;
		public const Byte ENCODER_POSITION = 1;

		// SERVO/ERC/ALM_RESET ON/OFF
		public const Byte TURN_OFF = 0;
		public const Byte TURN_ON = 1;

		// Move Direction
		public const Byte MOVE_DIRECTION_REVERSE = 0;
		public const Byte MOVE_DIRECTION_FORWARD = 1;

		public const Byte MOVE_DIRECTION_CW = 0;
		public const Byte MOVE_DIRECTION_CCW = 1;

		// Speed Parameter
		public const Byte ADC_MODE_RATE = 0;// Specify the rate (number of pulses per second)
		public const Byte ADC_MODE_TIME = 1;// Specify the time (seconds)
		public const Byte ADC_MODE_RATE_NOACC = 2;
		public const Byte ADC_MODE_TIME_NOACC = 3;
		public const Byte ADC_MODE_RATE_NODEC = 4;
		public const Byte ADC_MODE_TIME_NODEC = 5;

		// Fix Move Mode
		public const Byte FIX_MOVE_MODE_REL = 0x41;
		public const Byte FIX_MOVE_MODE_ABS_BY_OUTPLS = 0x42;
		public const Byte FIX_MOVE_MODE_ABS_BY_ENC = 0x43;
		public const Byte FIX_MOVE_MODE_ZERO_RETURN_BY_OUTPLS = 0x44;
		public const Byte FIX_MOVE_MODE_ZERO_RETURN_BY_ENC = 0x45;

		// Compare Trigger Direction
		public const Byte CMPTRIG_DIRECTION_BOTH = 8;
		public const Byte CMPTRIG_DIRECTION_FORWARD = 9;
		public const Byte CMPTRIG_DIRECTION_REVERSE = 10;

		// Device Information
		public const Byte DEV_INFO_NO_DEV = 0x00;
		public const Byte DEV_INFO_MOTION_DEV = 0x8b;
		public const Byte DEV_INFO_IO_32OUT_DEV = 0x80;
		public const Byte DEV_INFO_IO_16IN_16OUT_DEV = 0x82;
		public const Byte DEV_INFO_IO_32IN_DEV = 0x84;
		public const Byte DEV_INF_AI_8IN_DEV = 0x8f;

		// Status
		public const Byte MOTION_NOT_DONE = 0;
		public const Byte MOTION_DONE = 1;

		public const Byte CONTINUE_INTERPOLATION_NEXT_NOT_READY = 0;
		public const Byte CONTINUE_INTERPOLATION_NEXT_READY = 1;

		public const Byte CONTINUE_INTERPOLATION_START = 0;
		public const Byte CONTINUE_INTERPOLATION_NEXT = 1;

		public const UInt32 ERR_STATUS_SW_PEL_STOP = 0x00000001;
		public const UInt32 ERR_STATUS_SW_MEL_STOP = 0x00000002;
		public const UInt32 ERR_STATUS_PEL_STOP = 0x00000008;
		public const UInt32 ERR_STATUS_MEL_STOP = 0x00000010;
		public const UInt32 ERR_STATUS_ALM_STOP = 0x00000020;
		public const UInt32 ERR_STATUS_EMG_STOP = 0x00000080;
		public const UInt32 ERR_STATUS_SD_STOP = 0x00000100;

		// 2015.02.09 mn_cmp_stop()
		public const Byte CMP_STOP_METHOD_CNT_EQUAL = 1;
		public const Byte CMP_STOP_METHOD_CNT_FWD_EQUAL = 2;
		public const Byte CMP_STOP_METHOD_CNT_REV_EQUAL = 3;
		public const Byte CMP_STOP_METHOD_CNT_EXCEED = 4;
		public const Byte CMP_STOP_METHOD_CNT_LESS = 5;

		public const Byte CUST_REPLACE_SPEED_PAR = 0x2;
	}

	public enum MotionConfig
	{
		PULSE_MODE = 0,
		SDLTC_LOGIC = 2,
		EL_PROC = 3,
		SD_ENA = 31,
		SD_PROC = 4,
		SDLTC_ENA = 5,
		SD_LOGIC = 6,
		ORG_LOGIC = 7,
		ALM_PROC = 8,
		ALM_LOGIC = 9,
		ERC_ERR_ENA = 10,
		ERC_ORG_ENA = 11,
		ERC_LEN = 12,
		ERC_LOGIC = 15,
		MANUAL_SD_ENA = 16,
		MANUAL_TRI_DRV_ELIMINATE_ENA = 17,
		INP_ENA = 18,
		INP_LOGIC = 19,
		RESET_OUTPLS_AFTER_ORG_ENA = 20,
		RESET_ENC_AFTER_ORG_ENA = 21,
		RESET_CNT3_AFTER_ORG_ENA = 22,
		ENC_MODE = 23,
		ENC_REV_ENA = 25,
		ENC_Z_LOGIC = 26,
		OUTPLS_REV_ENA = 27,
		CNT3_MODE = 28,
		ALL_DATA = 32
	};

	// Speed Parameter
	public struct SPEED_PAR
	{
		public double Start_Speed;          // Initial & Stop Speed (PPS)
		public double Drive_Speed;          // Operation Speed (PPS)
		public double Correction_Speed;     // Correction Speed (PPS)
		public double Acc;                  // Acceleration data (s/ PPS/S)
		public double Dec;                  // Deceleration data (s/ PPS/S)
		public Byte AccDec_Mode;            // ADC_MODE_RATE / ADC_MODE_TIME
		public Byte SCurve_Enable;          // DISABLE / Enable
		public double SCurveAcc_Sect;       // Acceleration S-curve range (PPS)
		public double SCurveDec_Sect;       // Deceleration S-curve range (PPS)
		public MaxSpeed Max_Speed;                  // selecting MaxSpeed to set the speed magnification rate
	}

	public enum MaxSpeed
	{
		MAXSPEED_DO_NOT_CHANGE = 0,
		MAXSPEED_10K,               // 0.1 ~ 10,000 PPS
		MAXSPEED_20K,               // 0.2 ~ 20,000 PPS
		MAXSPEED_50K,               // 0.5 ~ 50,000 PPS
		MAXSPEED_100K,              //  1 ~ 100,000 PPS
		MAXSPEED_200K,              //  2 ~ 200,000 PPS
		MAXSPEED_500K,              //  5 ~ 200,000 PPS
		MAXSPEED_1M,                // 10 ~ 1,000,000 PPS
		MAXSPEED_2M,                // 20 ~ 2,000,000 PPS
		MAXSPEED_5M,                // 50 ~ 5,000,000 PPS
		MAXSPEED_6M                 // 66 ~ 6,666,666 PPS
	};

	public struct MOTION_DEV_IO
	{
		public Byte SVON;
		public Byte RESET_ALM;
		public Byte RDY;
		public Byte ALM;
		public Byte PEL;
		public Byte MEL;
		public Byte ORG;
		public Byte SDLTC;
		public Byte SDIN;
		public Byte INP;
		public Byte EMG;
		public Byte EZ;
		public Byte ERC;
	}

	public struct AXIS_RANGE_SETTINGS
	{
		public double MinSpeed;
		public double MaxSpeed;
		public double MinAccTime;
		public double MaxAccTime;
		public UInt32 MinAccRates;
		public UInt32 MaxAccRates;
	}

	public class ErrCode
	{
		//Error Code
		public const Int16 SUCCESS = 0;
		public const Int16 ERROR_NO_CARD_FOUND = -100;
		public const Int16 ERROR_IOCTL_FAILED = -101;
		public const Int16 ERROR_INVALID_LINE_NO = -102;
		public const Int16 ERROR_COMM_NOT_START = -103;
		public const Int16 ERROR_INVALID_DEV_NO = -104;
		public const Int16 ERROR_NO_DEV_FOUND = -105;
		public const Int16 ERROR_SET_IO_DEV = -106;
		public const Int16 ERROR_SET_MOTION_DEV = -107;
		public const Int16 ERROR_START_SPEED_EXCEED_DRIVING_SPEED = -108;
		public const Int16 ERROR_INVALID_MAX_SPEED_SELECTION = -109;
		public const Int16 ERROR_SET_START_SPEED_OUT_RANGE = -110;
		public const Int16 ERROR_SET_DRIVING_SPEED_OUT_RANGE = -111;
		public const Int16 ERROR_INVALID_SCURVE_ENABLE = -112;
		public const Int16 ERROR_INVALID_ADC_MODE = -113;
		public const Int16 ERROR_INVALID_ACC_DATA = -114;
		public const Int16 ERROR_SET_ACC_DOUBLE_DEC = -115;
		public const Int16 ERROR_SET_ACC_OUT_RANGE = -116;
		public const Int16 ERROR_SET_DEC_OUT_RANGE = -117;
		public const Int16 ERROR_SET_ACC_SECT_OUT_RANGE = -118;
		public const Int16 ERROR_SET_DEC_SECT_OUT_RANGE = -119;
		public const Int16 ERROR_SET_CORRECTION_SPD_OUT_RANGE = -120;
		public const Int16 ERROR_SET_SCAN_INDEX_OUT_RANGE = -121;
		public const Int16 ERROR_INVALID_COMM_SPEED = -122;
		public const Int16 ERROR_COMM_NOT_STOP = -123;
		public const Int16 ERROR_SET_DATA = -124;
		public const Int16 ERROR_INVALID_CONFIG_ITEM = -125;
		public const Int16 ERROR_INVALID_FILTER_ITEM = -126;
		public const Int16 ERROR_INVALID_SOFTWARE_LIMIT_SOURCE = -127;
		public const Int16 ERROR_INVALID_STOP_MODE = -128;
		public const Int16 ERROR_CONFLICT_WITH_VRING = -129;
		public const Int16 ERROR_INVALID_MOVE_DIRECTION = -130;
		public const Int16 ERROR_INVALID_HOME_MODE = -131;
		public const Int16 ERROR_INVALID_EZ_COUNT = -132;
		public const Int16 ERROR_MOVE_HOLD = -133;
		public const Int16 ERROR_EMG_SIGNAL_ON = -134;
		public const Int16 ERROR_ALM_SIGNAL_ON = -135;
		public const Int16 ERROR_MEL_SIGNAL_ON = -136;
		public const Int16 ERROR_PEL_SIGNAL_ON = -137;
		public const Int16 ERROR_WAIT_INP = -138;
		public const Int16 ERROR_WAIT_ERC = -139;
		public const Int16 ERROR_WAIT_BACKLASH_CORRECT = -140;
		public const Int16 ERROR_WAIT_PULSE_IN = -141;
		public const Int16 ERROR_MOTION_NOT_COMPLETE = -142;
		public const Int16 ERROR_INVALID_FIX_MOVE_MODE = -143;
		public const Int16 ERROR_REGISTER_FULL = -144;
		public const Int16 ERROR_INVALID_POSITION = -145;
		public const Int16 ERROR_INVALID_GROUPNO = -146;
		public const Int16 ERROR_INVALID_NUM_DEV = -147;
		public const Int16 ERROR_GROUP_ALREADY_HOLD = -148;
		public const Int16 ERROR_SET_ARC_FINISH_POS = -149;
		public const Int16 ERROR_SET_BIT_DUMMY_DEV = -150;
		public const Int16 ERROR_INVALID_DEV_HOLD = -151;
		public const Int16 ERROR_GROUP_NOT_HOLD = -152;
		public const Int16 ERROR_INVALID_VRING_SOURCE = -153;
		public const Int16 ERROR_INVALID_VRING_VALUE = -154;
		public const Int16 ERROR_INVALID_CMPTRIG_SOURCE = -155;
		public const Int16 ERROR_INVALID_CMPTRIG_DIRECTION = -156;
		public const Int16 ERROR_INVALID_CMPTRIG_PITCH = -157;
		public const Int16 ERROR_MOTION_IS_COMPLETED = -158;
		public const Int16 ERROR_INVALID_CARD_ID = -159;
		public const Int16 ERROR_INVALID_OUTPUT_DATA = -160;
		public const Int16 ERROR_INVALID_BITNO = -161;
		public const Int16 ERROR_SET_BITNO = -162;
		public const Int16 ERROR_INVALID_PORTNO = -163;
		public const Int16 ERROR_SET_PORTNO = -164;
		public const Int16 ERROR_CARD_ID_DUPLICATED = -165;
		public const Int16 ERROR_CONFIG_FILE_LOAD = -166;
		public const Int16 ERROR_CONFIG_FILE_MATCH = -167;
		public const Int16 ERROR_COMM_DISCONNECT = -168;
		public const Int16 ERROR_INVALID_WORDNO = -169;
		public const Int16 ERROR_SET_WORDNO = -170;
		public const Int16 ERROR_STEP_HOME_FAILED = -171;
		public const Int16 ERROR_CONFLICT_WITH_CMPTRIG = -172;
		public const Int16 ERROR_SET_CPU_EMU_DEV = -173;
		public const Int16 ERROR_INVALID_CPU_EMU_START_ADDRESS = -174;
		public const Int16 ERROR_MATCH_CPU_EMU_CMD = -175;
		public const Int16 ERROR_SET_CPU_EMU_ADDRESS_REFRESH = -176;
		public const Int16 ERROR_SET_CPU_EMU_COMM_WAIT = -177;
		public const Int16 ERROR_SET_CPU_EMU_PROCESS_MODE = -178;
		public const Int16 ERROR_INVALID_CHANNELNO = -179;
		public const Int16 ERROR_SET_AO_VALUE_OUT_RANGE = -180;
		public const Int16 ERROR_SET_AI_DEV = -181;
		public const Int16 ERROR_SET_AO_OFFSET_OUT_RANGE = -182;
		public const Int16 ERROR_SET_AO_GAIN_OUT_RANGE = -183;
		public const Int16 ERROR_INVALID_CALIBRATION_ITEM = -184;
		public const Int16 ERROR_INVALID_CALIBRATION_SOURCE = -185;
		public const Int16 ERROR_INVALID_AXIS_NO = -186;
		public const Int16 ERROR_INVALID_INTERP_TYPE = -187;
		public const Int16 ERROR_INVALID_DECELERATE_POINT = -188;
		public const Int16 ERROR_INVALID_TERMINATE_THREAD_OPT = -189;
		public const Int16 ERROR_INVALID_ENC_MODE_OPT = -190;
		public const Int16 ERROR_NO_DO_BACKUP_FILE_FOUND = 191;
		public const Int16 ERROR_NO_DLL_VERSION_FOUND = -192;
	}


	public class Functions
	{
		// OS Configuration Routines
		[DllImport("MN200DLL.dll", EntryPoint = "mn_open_all")]
		public static extern Int16 mn_open_all(
											ref Byte pNumLine
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn200_get_lineinfo")]
		public static extern Int16 mn200_get_lineinfo(
											Byte ScannedIndex,
											ref Byte pLineNo
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn200_get_cardinfo")]
		public static extern Int16 mn200_get_cardinfo(
											Byte ScannedIndex,
											ref Byte pCardID
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_close_all")]
		public static extern Int16 mn_close_all();

		[DllImport("MN200DLL.dll", EntryPoint = "mn_reset")]
		public static extern Int16 mn_reset(Byte bLineNo);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_comm_speed")]
		public static extern Int16 mn_set_comm_speed(
											Byte bLineNo,
											Byte bCommSpeed
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_start_line")]
		public static extern Int16 mn_start_line(
											Byte bLineNo,
											ref Byte pNumDev
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_stop_line")]
		public static extern Int16 mn_stop_line(
											Byte bLineNo
											);

		// Hardware Configuration Routines (Optional)
		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_motion_cfg")]
		public static extern Int16 mn_set_motion_cfg(
											Byte bLineNo,
											Byte bDevNo,
											MotionConfig CfgItem,
											UInt32 dwData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_motion_cfg")]
		public static extern Int16 mn_get_motion_cfg(
											Byte bLineNo,
											Byte bDevNo,
											MotionConfig CfgItem,
											ref UInt32 pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_filter")]
		public static extern Int16 mn_set_filter(
											Byte bLineNo,
											Byte bDevNo,
											Byte bFilterSelection,
											Byte bFilterEnable
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_softlimit")]
		public static extern Int16 mn_set_softlimit(
										   Byte bLineNo,
										   Byte bDevNo,
										   Byte bSWLimitEnable,
										   Byte bCmpSource,
										   Byte bStopMode,
										   Int32 LimitPositive,
										   Int32 LimitNegaitive
										   );


		[DllImport("MN200DLL.dll", EntryPoint = "mn_servo_on")]
		public static extern Int16 mn_servo_on(
											Byte bLineNo,
											Byte bDevNo,
											Byte bServoOn
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_erc")]
		public static extern Int16 mn_set_erc(
										   Byte bLineNo,
										   Byte bDevNo,
										   Byte bErcOn
										   );

		[DllImport("MN200DLL.dll", EntryPoint = "mn_alarm_reset")]
		public static extern Int16 mn_alarm_reset(
											Byte bLineNo,
											Byte bDevNo,
											Byte bAlarmRstOn
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_load_ezgo_cfg")]
		public static extern Int16 mn_load_ezgo_cfg(
											Byte bLineNo,
											string FileName
											);
		// AUTOMATIC HOME SEARCH
		[DllImport("MN200DLL.dll", EntryPoint = "mn_home_start")]
		public static extern Int16 mn_home_start(
											Byte bLineNo,
											Byte bDevNo,
											SPEED_PAR SpeedPar,
											Byte bDirection,
											Byte bHomeMode,
											Byte bEZcount
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_leave_home")]
		public static extern Int16 mn_leave_home(
											Byte bLineNo,
											Byte bDevNo,
											SPEED_PAR SpeedPar,
											Byte bDirection,
											Byte bHomeMode,
											Byte bEZcount
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_home_search")]
		public static extern Int16 mn_home_search(
											Byte bLineNo,
											Byte bDevNo,
											SPEED_PAR SpeedPar,
											Byte bDirection,
											Int32 lOrgWidth,
											Byte bHomeMode,
											Byte bEZcount
											);

		// INDEPENDENT MOVING FUNCTIONS
		[DllImport("MN200DLL.dll", EntryPoint = "mn_velocity_move")]
		public static extern Int16 mn_velocity_move(
											Byte bLineNo,
											Byte bDevNo,
											SPEED_PAR SpeedPar,
											Byte bDirection
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_fix_move")]
		public static extern Int16 mn_fix_move(
											Byte bLineNo,
											Byte bDevNo,
											SPEED_PAR SpeedPar,
											Int32 Position,
											Byte bMoveType
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_stop_move")]
		public static extern Int16 mn_stop_move(
											Byte bLineNo,
										  	Byte bDevNo,
											Byte bStopMode
											);

		// INTERPOLATION MOVING FUNCTIONS
		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_group")]
		public static extern Int16 mn_set_group(
											Byte bLineNo,
											Byte bGrpNo,
											Byte bNumDev,
											Byte[] bDevNo
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_group")]
		public static extern Int16 mn_get_group(
											Byte bLineNo,
											Byte bGrpNo,
											ref Byte pNumDev,
											Byte[] bDevNo
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_line2_move")]
		public static extern Int16 mn_line2_move(
											Byte bLineNo,
											Byte bDev1No,
											Byte bDev2No,
											SPEED_PAR SpeedPar,
										  	Int32 Dev1Pos,
										  	Int32 Dev2Pos,
											Byte bCnstSpdEnable
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_arc2_move")]
		public static extern Int16 mn_arc2_move(
										   Byte bLineNo,
										   Byte bDev1No,
										   Byte bDev2No,
										   SPEED_PAR SpeedPar,
										   Byte bDirection,
										   Int32 Dev1CenterPos,
										   Int32 Dev2CenterPos,
										   Int32 Dev1FinishPos,
										   Int32 Dev2FinishPos,
										   UInt32 Low32BitDummyDevNo,
										   UInt32 High32BitDummyDevNo,
										   Byte bCnstSpdEnable
										   );

		[DllImport("MN200DLL.dll", EntryPoint = "mn_line3_move")]
		public static extern Int16 mn_line3_move(
											Byte bLineNo,
											Byte bDev1No,
											Byte bDev2No,
											Byte bDev3No,
											SPEED_PAR SpeedPar,
										  	Int32 Dev1Pos,
										  	Int32 Dev2Pos,
										  	Int32 Dev3Pos
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_linen_move")]
		public static extern Int16 mn_linen_move(
											Byte bLineNo,
										  	Byte[] bDevNo,
											SPEED_PAR SpeedPar,
											Int32[] DevPos,
											Byte bNumDev
											);

		// CONTINUOUS INTERPOLATION MOVING
		[DllImport("MN200DLL.dll", EntryPoint = "mn_conti_interp_next_ready")]
		public static extern Int16 mn_conti_interp_next_ready(
											Byte bLineNo,
											Byte bDevNo,
											ref Byte pReady
											);

		// OTHER MOTION FUNCTIONS		
		[DllImport("MN200DLL.dll", EntryPoint = "mn_group_stop_move")]
		public static extern Int16 mn_group_stop_move(
											Byte bLineNo,
										  	Byte bGrpNo,
											Byte bStopMode
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_group_hold_move")]
		public static extern Int16 mn_group_hold_move(
										 Byte bLineNo,
										   Byte bGrpNo
										 );

		[DllImport("MN200DLL.dll", EntryPoint = "mn_group_start_move")]
		public static extern Int16 mn_group_start_move(
											Byte bLineNo,
										  	Byte bGrpNo
											);
		// ADVANCED MOTION CONFIGURATIONS
		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_vring")]
		public static extern Int16 mn_set_vring(
											Byte bLineNo,
										  	Byte bDevNo,
										  	Byte bVRINGEnable,
											Byte bVRingSource,
											UInt32 dwVRingValue
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_cnst_cmptrig_config")]
		public static extern Int16 mn_cnst_cmptrig_config(
											Byte bLineNo,
										  	Byte bDevNo,
										  	Byte bCmpTrigEnable,
										  	Byte bCmpSource,
										  	Byte bDirection,
											UInt16 wCnstPitch
											);
		//2018.11.07
		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_customization")]
		public static extern Int16 mn_set_customization(
											Byte bLineNo,
											Byte bDevNo,
											UInt16 wCustParam,
											UInt16 wValue
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_customization")]
		public static extern Int16 mn_get_customization(
											Byte bLineNo,
											Byte bDevNo,
											UInt16 wCustParam,
											ref UInt16 wValue
											);

		// MISCELLANEOUS FUNCTIONS
		[DllImport("MN200DLL.dll", EntryPoint = "mn_change_v")]
		public static extern Int16 mn_change_v(
											Byte bLineNo,
										  	Byte bDevNo,
											SPEED_PAR SpeedPar,
											Byte bWaitCmpEnable
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_change_p")]
		public static extern Int16 mn_change_p(
											Byte bLineNo,
										  	Byte bDevNo,
											Int32 Position
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_cmdcounter")]
		public static extern Int16 mn_set_cmdcounter(
											Byte bLineNo,
										  	Byte bDevNo,
										  	Int32 Data
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_enccounter")]
		public static extern Int16 mn_set_enccounter(
											Byte bLineNo,
										  	Byte bDevNo,
										  	Int32 Data
										  	);

		// STATUS
		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_dev_info")]
		public static extern Int16 mn_get_dev_info(
											Byte bLineNo,
										  	Byte bDevNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_motion_done")]
		public static extern Int16 mn_motion_done(
											Byte bLineNo,
										  	Byte bDevNo,
											ref Byte pDone
											);


		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_cmdcounter")]
		public static extern Int16 mn_get_cmdcounter(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref Int32 pData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_enccounter")]
		public static extern Int16 mn_get_enccounter(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref Int32 pData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_speed")]
		public static extern Int16 mn_get_speed(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref double pData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_mdio_status")]
		public static extern Int16 mn_get_mdio_status(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref MOTION_DEV_IO pMotionIO
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_error_status")]
		public static extern Int16 mn_get_error_status(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref UInt32 pData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_latch_cmdcounter")]
		public static extern Int16 mn_get_latch_cmdcounter(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref Int32 pData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_latch_enccounter")]
		public static extern Int16 mn_get_latch_enccounter(
											Byte bLineNo,
										  	Byte bDevNo,
										  	ref Int32 pData
										  	);


		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_speed_range")]
		public static extern Int16 mn_get_speed_range(
											Byte bLineNo,
											Byte bDevNo,
											ref MaxSpeed pMaxSpeed,
											ref AXIS_RANGE_SETTINGS pAxisRange
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_tcurve_acc_range")]
		public static extern Int16 mn_get_tcurve_acc_range(
											Byte bLineNo,
											Byte bDevNo,
											ref MaxSpeed pMaxSpeed,
											double lfStartSpeed,
											double lfDriveSpeed,
											ref AXIS_RANGE_SETTINGS pAxisRange
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_scurve_acc_range")]
		public static extern Int16 mn_get_scurve_acc_range(
											Byte bLineNo,
											Byte bDevNo,
											ref MaxSpeed pMaxSpeed,
											double lfStartSpeed,
											double lfDriveSpeed,
											double lfScurveSect,
											ref AXIS_RANGE_SETTINGS pAxisRange
											);
		// Parallel I/O
		[DllImport("MN200DLL.dll", EntryPoint = "mn200_get_di")]
		public static extern Int16 mn200_get_di(
										  	Byte bCardID,
										  	ref Byte pData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn200_set_do")]
		public static extern Int16 mn200_set_do(
										  	Byte bCardID,
										  	Byte bData
										  	);

		[DllImport("MN200DLL.dll", EntryPoint = "mn200_get_do")]
		public static extern Int16 mn200_get_do(
										  	Byte bCardID,
										  	ref Byte pData
										  	);

		// Serial I/O
		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_di_bit")]
		public static extern Int16 mn_get_di_bit(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bBitNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_do_bit")]
		public static extern Int16 mn_set_do_bit(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bBitNo,
											Byte bData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_do_bit")]
		public static extern Int16 mn_get_do_bit(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bBitNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_di_byte")]
		public static extern Int16 mn_get_di_byte(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bByteNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_do_byte")]
		public static extern Int16 mn_set_do_byte(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bByteNo,
											Byte bData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_do_byte")]
		public static extern Int16 mn_get_do_byte(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bByteNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_di_word")]
		public static extern Int16 mn_get_di_word(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bWordNo,
											ref UInt16 pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_do_word")]
		public static extern Int16 mn_set_do_word(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bWordNo,
											UInt16 wData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_do_word")]
		public static extern Int16 mn_get_do_word(
										 	Byte bLineNo,
											Byte bDevNo,
											Byte bWordNo,
											ref UInt16 pData
											);

		//ADVANCED IO FUNCTIONS
		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_port_bit")]
		public static extern Int16 mn_get_port_bit(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bPortNo,
											Byte bBitNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_port_bit")]
		public static extern Int16 mn_set_port_bit(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bPortNo,
											Byte bBitNo,
											Byte bData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_port_byte")]
		public static extern Int16 mn_get_port_byte(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bPortNo,
											ref Byte pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_port_byte")]
		public static extern Int16 mn_set_port_byte(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bPortNo,
											Byte bData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_line_status")]
		public static extern Int16 mn_get_line_status(
											Byte bLinoNo,
											ref UInt16 pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_slave_error_table")]
		public static extern Int16 mn_get_slave_error_table(
											Byte bLinoNo,
											UInt32[] pData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_clear_slave_error_flag")]
		public static extern Int16 mn_clear_slave_error_flag(
											Byte bLinoNo,
											UInt32[] pData
											);

		//Analog IO
		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_ao")]
		public static extern Int16 mn_set_ao(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bChannelNo,
											float fData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_ao_offset")]
		public static extern Int16 mn_set_ao_offset(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bChannelNo,
											Int16 OffsetLSB
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_ao_gain")]
		public static extern Int16 mn_set_ao_gain(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bChannelNo,
											float GainValue
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_ai")]
		public static extern Int16 mn_get_ai(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bChannelNo,
											ref float fData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_get_ai_all")]
		public static extern Int16 mn_get_ai_all(
											Byte bLinoNo,
											Byte bDevNo,
											float[] fData
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_cal")]
		public static extern Int16 mn_set_cal(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bItem,
											Byte bValue
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_set_cal_src")]
		public static extern Int16 mn_set_cal_src(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bItem
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_save_cal")]
		public static extern Int16 mn_save_cal(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bItem
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn_load_cal")]
		public static extern Int16 mn_load_cal(
											Byte bLinoNo,
											Byte bDevNo,
											Byte bItem
											);

		[DllImport("MN200DLL.dll", EntryPoint = "mn200_firmware_ver_get")]
		public static extern Int16 mn200_firmware_ver_get(
											Byte bCardID,
											ref UInt16 pFirmwareVer
											);

	}
}
