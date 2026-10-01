# FitTech Task 移植到 LFSMEO 規則

這份文件是給 Codex 使用的移植規則。當使用者要求把 FitTech 的某個 Task 移植到 LFSMEO 時，請依照以下規則執行。

## 目標

將 FitTech 的 Task / SubTask 流程邏輯移植成 LFSMEO 專案可用的 `IBaseTask<T>` 架構。

移植時要保留 FitTech 的流程順序與安全判斷，但必須改成 LFSMEO 的：

- Task 外殼
- 狀態機格式
- DML / DIOL 呼叫方式
- Pause / Abort / Continue 機制
- Log / Error / Status 回報方式
- 軸與 IO 命名方式

## 基本原則

1. 先讀 FitTech 原始 Task。
2. 找出它呼叫的所有 SubTask。
3. 先整理流程，不要急著改 code。
4. 主 Task 與 SubTask 要分開移植，不要把所有流程塞進主 Task。
5. 所有從 FitTech 來的全域參數、設定值、位置值、IO mapping、軸 mapping，都要集中整理在檔案最上方。
6. 不移植沒有 LFSMEO 對應機制的 UI 細節，除非使用者明確要求。
7. 移植完成後要確認 `.csproj` 是否有包含新增 `.cs` 檔。
8. 不要修改 FitTech 原本的判斷邏輯寫法，不要為了整理而把 inline 判斷式抽成 helper function，也不要自行加入模式切換、switch 包裝或額外判斷分支。

## 檔案命名規則

FitTech:

```text
ProcessTask_Xxx.cs
SubTask_Xxx.cs
```

LFSMEO:

```text
Task_Xxx.cs
SubTask_Xxx.cs
```

範例：

```text
FitTech: ProcessTask_Load.cs
LFSMEO: Task_LoadNest.cs

FitTech: SubTask_LoadNest.cs
LFSMEO: SubTask_LoadNest.cs
```

## LFSMEO Task 外殼格式

新的 LFSMEO Task 必須使用：

```csharp
public class Task_Xxx : IBaseTask<Task_Xxx.WORK>
```

建構子格式：

```csharp
public Task_Xxx(IBaseTaskDependence dependencies,
    IF_StateControl f_StateControl,
    string set_state = "Default")
    : base(dependencies)
```

基本欄位：

```csharp
private IF_BaseTask SubTask;
private IF_StateControl F_StateControl;
ProbeTesterFunction.AxisHardwareParam Axis;
ProbeTesterFunction ProbeTesterFunc;
```

若需要 timer：

```csharp
private long wait_timer = 0;
```

## 全域參數整理規則

所有 FitTech 來源參數要放在檔案最上方的 `#region`，不要散在流程內。

建議格式：

```csharp
#region FitTech migration settings
// FitTech source: GControls.Tag[EPosSetting.Xxx].Value
private double XxxPos = 0.0;

private int TimeoutMs = 5000;
#endregion

#region axis mapping
private int AxisNestX = 0;
private int AxisNestY = 0;
private int AxisNestZ = 0;
private int AxisNestA = 0;
private int AxisNestTX = 0;
private int AxisNestTY = 0;
#endregion
```

若 FitTech 原本使用：

```csharp
GControls.Tag[EPosSetting.LoadPos_NestX].Value
```

LFSMEO 先整理成：

```csharp
// FitTech source: EPosSetting.LoadPos_NestX
private double LoadPos_NestX = 0.0;
```

之後再依使用者要求接設定檔、Recipe、UI 或實機參數。

## 狀態機移植規則

FitTech 常見狀態：

```csharp
START
PRESET
XXX
WAIT_XXX
SUCCESS
END_ABORT
END_FAIL
END
ALL_END
```

LFSMEO 常見狀態：

```csharp
NONE
START
PRESET
XXX
WAIT_XXX
END,
SUCCESS,
FAIL,
PAUSE,
ABORT,
CONTINUE,
```

規則：

- `END_ABORT` 轉成 `ABORT`
- `END_FAIL` 轉成 `FAIL`
- `RES.Result = ERunResult.SUCCESS` 轉成 `SetStatus(TASK_STATUS.SUCCESS)`
- `RES.Done()` 不直接移植，改由 LFSMEO `SetStatus(...)` 完成
- FitTech `ContinueRun()` 不移植
- FitTech `CheckPause()` 改成 LFSMEO `GoToPause()` / `CheckResult()` 流程

## CheckResult 移植規則

FitTech:

```csharp
RunState res = SubTask.Run();
CheckResult(res, Done: WORK.SUCCESS);
```

LFSMEO:

```csharp
TASK_STATUS check = SubTask.Run(GetStatusCommand());
CheckResult(check, SUCCESS: WORK.SUCCESS);
```

LFSMEO `CheckResult` 需處理：

```csharp
TASK_STATUS.SUCCESS
TASK_STATUS.PAUSE
TASK_STATUS.ABORT
TASK_STATUS.CONTINUE
TASK_STATUS.FAIL
```

當 SubTask 結束、失敗或中止時，要呼叫：

```csharp
SetSubTaskProcessing(false);
```

## DML API 對照

只做 API 名稱轉換，不改判斷邏輯的寫法。

FitTech 若原本是：

```csharp
if (DML.get_motion_complete(NestY) &&
    DML.get_motion_complete(NestZ) && DML.get_motion_complete(NestA) &&
    DML.get_motion_complete(NestTX) && DML.get_motion_complete(NestTY))
```

LFSMEO 要維持 inline 判斷，只替換 API / 軸名稱：

```csharp
if (Deps.DML.Get_Motion_Complete(AxisNestY) &&
    Deps.DML.Get_Motion_Complete(AxisNestZ) && Deps.DML.Get_Motion_Complete(AxisNestA) &&
    Deps.DML.Get_Motion_Complete(AxisNestTX) && Deps.DML.Get_Motion_Complete(AxisNestTY))
```

不要改成：

```csharp
private bool IsNestYZA_TXTYMotionComplete()
{
    return Deps.DML.Get_Motion_Complete(AxisNestY) &&
           Deps.DML.Get_Motion_Complete(AxisNestZ) &&
           Deps.DML.Get_Motion_Complete(AxisNestA) &&
           Deps.DML.Get_Motion_Complete(AxisNestTX) &&
           Deps.DML.Get_Motion_Complete(AxisNestTY);
}
```

FitTech:

```csharp
DML.get_motion_complete(axis)
```

LFSMEO:

```csharp
Deps.DML.Get_Motion_Complete(axis)
```

FitTech:

```csharp
PTPNormal.ptp(axis, pos)
```

LFSMEO:

```csharp
Deps.DML.PTP_Move(axis, pos, MOVE_MODE.ABS)
```

FitTech:

```csharp
DML.go_home(axis)
DML.get_go_home_complete(axis)
```

LFSMEO:

```csharp
Deps.DML.GoHome(axis)
Deps.DML.Get_Home_Complete(axis)
```

FitTech:

```csharp
DML.Z_get_motion_complete()
```

LFSMEO:

```csharp
Deps.DML.Get_Motion_Complete(Axis.AxisZ)
```

## DIOL API 對照

只做 API / IO 名稱轉換，不改判斷邏輯的寫法。

FitTech 若原本是：

```csharp
if (DML.get_motion_complete(NestX) && DIOL.input(EIO.SafePosition))
```

LFSMEO 要維持 inline 判斷，只替換 API / IO mapping：

```csharp
if (Deps.DML.Get_Motion_Complete(AxisNestX) && Deps.DIOL.GetInputStatus(SafePositionSensor))
```

不要改成 helper function 或 switch mode：

```csharp
private bool IsSafePosition()
{
    switch (SafePositionCheckMode)
    {
        ...
    }
}
```

FitTech:

```csharp
DIOL.input(EIO.Xxx)
```

LFSMEO:

```csharp
Deps.DIOL.GetInputStatus(EIOName.Xxx)
```

FitTech:

```csharp
DIOL.output(EIO.Xxx, true)
```

LFSMEO:

```csharp
Deps.DIOL.SetOutputStatus(EIOName.Xxx, true)
```

如果 LFSMEO 沒有同名 `EIOName`：

1. 先搜尋 `DeviceCore/IO/IFunction_IO_Card.cs`
2. 找最接近的 IO
3. 若無法確認，建立集中變數並加註解
4. 不要直接猜硬體 IO

範例：

```csharp
// FitTech source: DIOL.input(EIO.SafePosition)
private EIOName SafePositionSensor = EIOName.SafePos_Sensor;
```

## Timer 對照

FitTech:

```csharp
TMC delay = new TMC();
delay.Reset();
delay.GetTime() > 5
```

LFSMEO:

```csharp
private long delay = 0;
Tool.ResetTimeCount(out delay);
Tool.GetTime(delay, "s") > 5
```

若使用毫秒：

```csharp
Tool.GetTime(delay) > 5000
```

## 軸 Mapping 規則

FitTech 常見軸：

```csharp
NestX
NestY
NestZ
NestA
NestTX
NestTY
```

LFSMEO 優先對照：

```csharp
AxisNestX = Axis.AxisX;
AxisNestY = Axis.AxisY;
AxisNestZ = Axis.AxisZ;
AxisNestA = Axis.AxisRX;
AxisNestTX = Axis.AxisRY;
AxisNestTY = Axis.AxisRZ;
```

注意：

- 不要把 `NestTX` 暫時接到 `AxisX`
- 不要把 `NestTY` 暫時接到 `AxisY`
- 如果軸定義不確定，要先集中在 `SetDefaultAxis()`，並加註解

## 錯誤處理移植規則

FitTech:

```csharp
TowerLight.Request(...)
Scope.InfoBar.SetAlarm()
Scope.Log.AppendError(...)
param.ErrorStatus = true
RES.Result = ERunResult.FAIL
```

LFSMEO:

```csharp
Tool.SaveLogToFile("error message", level: "ERR");
Transition(WORK.FAIL);
```

或：

```csharp
Tool.SaveLogToFile("abort message", level: "ERR");
Transition(WORK.ABORT);
```

在 `FAIL` 狀態：

```csharp
SetStatus(TASK_STATUS.FAIL);
```

在 `ABORT` 狀態：

```csharp
SetStatus(TASK_STATUS.ABORT);
```

## Pause / Abort / Continue 規則

每個 Task / SubTask 都要保留 LFSMEO 模板：

```csharp
public override void GoToPause()
```

並在 `RunLoop` 開頭處理：

```csharp
if (task_command == TASK_STATUS.ABORT)
    GoToCaseAbortState(GetPauseState());
else if (task_command == TASK_STATUS.CONTINUE)
    GoToCaseConitinueState();
```

如果有 SubTask 執行：

```csharp
if (GetSubTaskProcessing())
    SubTask.GoToPause();
```

建立 SubTask 後要設定：

```csharp
SetSubTaskProcessing(true);
```

SubTask 回傳成功、失敗、中止後要設定：

```csharp
SetSubTaskProcessing(false);
```

## SubTask 移植規則

FitTech 如果主 Task 呼叫：

```csharp
LoadNest = new SubTask_LoadNest_DETester(param);
```

LFSMEO 應建立：

```csharp
SubTask = new SubTask_LoadNest(Deps, F_StateControl);
SetSubTaskProcessing(true);
Transition(WORK.WAIT_LOAD_NEST);
```

SubTask 本身也要使用：

```csharp
public class SubTask_Xxx : IBaseTask<SubTask_Xxx.WORK>
```

不要把 FitTech SubTask 內容直接塞進主 Task。

## `.csproj` 規則

LFSMEO 是舊式 `.csproj` 時，新增 `.cs` 檔後要檢查：

```text
ProbeTester/ProbeTester.csproj
```

是否有：

```xml
<Compile Include="Logic\1.Task\Xxx\Task_Xxx.cs" />
<Compile Include="Logic\1.Task\Xxx\SubTask\SubTask_Xxx.cs" />
```

如果沒有，要補上。

## 移植完成後檢查

完成後至少檢查：

1. 新增檔案是否進 `.csproj`
2. `WORK enum` 是否完整
3. 每個 `Transition(...)` 目標是否存在
4. 每個 SubTask 是否有 `SetSubTaskProcessing(true)`
5. SubTask 結束後是否有 `SetSubTaskProcessing(false)`
6. DML / DIOL 是否都改成 `Deps.DML` / `Deps.DIOL`
7. FitTech 的 `GControls.Tag[...]` 是否都集中在最上方
8. FitTech 的 `Scope.*`、`LoaderUI.*`、`TowerLight.*` 是否已轉成 LFSMEO 寫法或明確略過
9. 軸 mapping 是否集中在 `SetDefaultAxis()`
10. IO mapping 是否集中在檔案上方

## 回報格式

移植完成後，回報使用者：

- 移植了哪些檔案
- FitTech 原始檔對應到 LFSMEO 哪些檔案
- 哪些全域參數已集中在檔案最上方
- 哪些 IO / 軸 mapping 需要使用者確認
- build / 驗證結果
- 若未能 build，說明原因
