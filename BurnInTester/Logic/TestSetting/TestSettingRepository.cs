using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BurnInTester.Logic.TestSetting
{
    public sealed class TestSettingRepository
    {
        #region parameter
        private const string StageTableName = "AgingStageTable";
        private const string AgingTableName = "AgingProcessTable";
        private const string TemperatureTableName = "TemperatureTable";
        private const string LivTableName = "LIVTable";
        private const string VfTableName = "VFTable";
        private const string IrTableName = "IRTable";

        private const string StageIdColumnName = "Stage_ID";
        private const string StageDurationColumnName = "Stage_Duration";
        private const string StageExecutionModeColumnName = "Stage_ExecutionMode";
        private const string StageExecutionIntervalColumnName = "Stage_ExecutionIntervalSeconds";
        private const string TitleNoColumnName = "Title_No";
        private const string TitleStageIdColumnName = "Title_StageID";
        private const string TitleStageRoleColumnName = "Title_StageRole";
        private const string TitleMonitorIntervalColumnName = "Title_MonitorIntervalSeconds";
        private const string TitleSkipColumnName = "Title_Skip";
        private const string TitleTestProcessColumnName = "Title_TestProcess";
        private const string TitleContentColumnName = "Title_Content";
        private const string TitleIdColumnName = "Title_ID";
        #endregion

        #region private function
        private DataSet CreateDataSetSchema()
        {
            DataSet dataSet = new DataSet("DocumentElement");

            DataTable stageTable = new DataTable(StageTableName);
            stageTable.Columns.Add(StageIdColumnName, typeof(string));
            stageTable.Columns.Add(StageDurationColumnName, typeof(string));
            stageTable.Columns.Add(StageExecutionModeColumnName, typeof(string));
            stageTable.Columns.Add(StageExecutionIntervalColumnName, typeof(string));
            stageTable.PrimaryKey = new DataColumn[] { stageTable.Columns[StageIdColumnName] };
            dataSet.Tables.Add(stageTable);

            DataTable agingTable = new DataTable(AgingTableName);
            agingTable.Columns.Add(TitleNoColumnName, typeof(string));
            agingTable.Columns.Add(TitleStageIdColumnName, typeof(string));
            agingTable.Columns.Add(TitleStageRoleColumnName, typeof(string));
            agingTable.Columns.Add(TitleMonitorIntervalColumnName, typeof(string));
            agingTable.Columns.Add(TitleSkipColumnName, typeof(string));
            agingTable.Columns.Add(TitleTestProcessColumnName, typeof(string));
            agingTable.Columns.Add(TitleContentColumnName, typeof(string));
            agingTable.Columns.Add(TitleIdColumnName, typeof(string));
            agingTable.PrimaryKey = new DataColumn[] { agingTable.Columns[TitleIdColumnName] };
            dataSet.Tables.Add(agingTable);

            DataTable temperatureTable = CreateDetailTable<TemperatureCondition>(TemperatureTableName);
            DataTable livTable = CreateDetailTable<LivCondition>(LivTableName);
            DataTable vfTable = CreateDetailTable<VfCondition>(VfTableName);
            DataTable irTable = CreateDetailTable<IrCondition>(IrTableName);

            dataSet.Tables.Add(temperatureTable);
            dataSet.Tables.Add(livTable);
            dataSet.Tables.Add(vfTable);
            dataSet.Tables.Add(irTable);

            AddNestedRelation(dataSet, agingTable, temperatureTable);
            AddNestedRelation(dataSet, agingTable, livTable);
            AddNestedRelation(dataSet, agingTable, vfTable);
            AddNestedRelation(dataSet, agingTable, irTable);

            return dataSet;
        }

        private DataTable CreateDetailTable<T>(string tableName)
            where T : TestCondition
        {
            DataTable table = new DataTable(tableName);
            table.Columns.Add(TitleIdColumnName, typeof(string));

            foreach (PropertyInfo property in GetConditionProperties(typeof(T)))
                table.Columns.Add(property.Name, typeof(string));

            table.PrimaryKey = new DataColumn[] { table.Columns[TitleIdColumnName] };
            return table;
        }

        private void AddNestedRelation(
            DataSet dataSet,
            DataTable parentTable,
            DataTable childTable)
        {
            DataRelation relation = dataSet.Relations.Add(
                $"{parentTable.TableName}_{childTable.TableName}",
                parentTable.Columns[TitleIdColumnName],
                childTable.Columns[TitleIdColumnName],
                true);

            relation.Nested = true;
        }

        private void AddConditionRows<T>(DataTable table, IEnumerable<T> conditions)
            where T : TestCondition
        {
            List<PropertyInfo> properties = GetConditionProperties(typeof(T));

            foreach (T condition in conditions)
            {
                DataRow row = table.NewRow();
                SetString(row, TitleIdColumnName, condition.TestStepId);

                foreach (PropertyInfo property in properties)
                    SetString(row, property.Name, (string)property.GetValue(condition, null));

                table.Rows.Add(row);
            }
        }

        private void LoadConditionRows<T>(
            DataTable table,
            ICollection<TestCondition> conditions)
            where T : TestCondition, new()
        {
            List<PropertyInfo> properties = GetConditionProperties(typeof(T));

            foreach (DataRow row in table.Rows)
            {
                T condition = new T
                {
                    TestStepId = GetString(row, TitleIdColumnName)
                };

                foreach (PropertyInfo property in properties)
                    property.SetValue(condition, GetString(row, property.Name), null);

                conditions.Add(condition);
            }
        }

        private List<PropertyInfo> GetConditionProperties(Type conditionType)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            foreach (PropertyInfo property in conditionType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.Name == nameof(TestCondition.TestStepId))
                    continue;

                if (property.GetGetMethod() == null || property.GetSetMethod() == null)
                    continue;

                if (property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException(
                        $"{conditionType.Name}.{property.Name} must be a string property.");
                }

                properties.Add(property);
            }

            properties.Sort((left, right) => left.MetadataToken.CompareTo(right.MetadataToken));
            return properties;
        }

        private void SetString(DataRow row, string columnName, string value)
        {
            row[columnName] = value == null ? (object)DBNull.Value : value;
        }

        private string GetString(DataRow row, string columnName, string defaultValue = "")
        {
            if (!row.Table.Columns.Contains(columnName) || row.IsNull(columnName))
                return defaultValue;

            return row[columnName].ToString();
        }

        private TestProcess ParseTestProcess(string value)
        {
            TestProcess process;
            bool success = Enum.TryParse(value, true, out process);

            if (!success || !Enum.IsDefined(typeof(TestProcess), process))
                throw new InvalidOperationException($"Unknown test process: {value}");

            return process;
        }

        private StageStepRole ParseStageRole(string value, TestProcess process)
        {
            StageStepRole role;
            bool success = Enum.TryParse(value, true, out role);

            if (success && Enum.IsDefined(typeof(StageStepRole), role))
                return role;

            return process == TestProcess.Temperature
                ? StageStepRole.Preparation
                : StageStepRole.Execution;
        }

        private ExecutionRepeatMode ParseExecutionRepeatMode(string value)
        {
            ExecutionRepeatMode mode;
            bool success = Enum.TryParse(value, true, out mode);

            return success && Enum.IsDefined(typeof(ExecutionRepeatMode), mode)
                ? mode
                : ExecutionRepeatMode.Once;
        }

        private void NormalizeStages(TestSettingData data)
        {
            Dictionary<string, AgingStage> stagesById = data.Stages
                .Where(stage => !string.IsNullOrWhiteSpace(stage.Id))
                .GroupBy(stage => stage.Id)
                .ToDictionary(group => group.Key, group => group.First());

            List<AgingStage> normalizedStages = new List<AgingStage>();
            HashSet<string> addedStageIds = new HashSet<string>();

            for (int stepIndex = 0; stepIndex < data.TestSteps.Count; stepIndex++)
            {
                TestStep step = data.TestSteps[stepIndex];
                if (string.IsNullOrWhiteSpace(step.StageId))
                    step.StageId = (stepIndex + 1).ToString();
                if (string.IsNullOrWhiteSpace(step.MonitorIntervalSeconds))
                    step.MonitorIntervalSeconds = "0";

                AgingStage stage;
                if (!stagesById.TryGetValue(step.StageId, out stage))
                {
                    stage = new AgingStage
                    {
                        Id = step.StageId,
                        Duration = GetLegacyStageDuration(data, step),
                        ExecutionMode = ExecutionRepeatMode.Once,
                        ExecutionIntervalSeconds = "0"
                    };
                    stagesById.Add(stage.Id, stage);
                }

                if (string.IsNullOrWhiteSpace(stage.Duration))
                    stage.Duration = GetLegacyStageDuration(data, step);
                if (string.IsNullOrWhiteSpace(stage.ExecutionIntervalSeconds))
                    stage.ExecutionIntervalSeconds = "0";

                if (addedStageIds.Add(stage.Id))
                    normalizedStages.Add(stage);

                TemperatureCondition temperatureCondition = data.Conditions
                    .OfType<TemperatureCondition>()
                    .FirstOrDefault(condition => condition.TestStepId == step.Id);

                if (temperatureCondition != null)
                    temperatureCondition.BurnInDuration = stage.Duration;
            }

            data.Stages.Clear();
            data.Stages.AddRange(normalizedStages);
        }

        private string GetLegacyStageDuration(TestSettingData data, TestStep step)
        {
            TemperatureCondition condition = data.Conditions
                .OfType<TemperatureCondition>()
                .FirstOrDefault(item => item.TestStepId == step.Id);

            return condition == null || string.IsNullOrWhiteSpace(condition.BurnInDuration)
                ? "0"
                : condition.BurnInDuration;
        }
        #endregion

        #region public function
        public void Save(string filePath, TestSettingData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            DataSet dataSet = CreateDataSetSchema();
            DataTable stageTable = dataSet.Tables[StageTableName];
            DataTable agingTable = dataSet.Tables[AgingTableName];

            foreach (AgingStage stage in data.Stages)
            {
                DataRow stageRow = stageTable.NewRow();
                SetString(stageRow, StageIdColumnName, stage.Id);
                SetString(stageRow, StageDurationColumnName, stage.Duration);
                SetString(stageRow, StageExecutionModeColumnName, stage.ExecutionMode.ToString());
                SetString(
                    stageRow,
                    StageExecutionIntervalColumnName,
                    stage.ExecutionIntervalSeconds);
                stageTable.Rows.Add(stageRow);
            }

            foreach (TestStep step in data.TestSteps)
            {
                DataRow row = agingTable.NewRow();
                SetString(row, TitleNoColumnName, step.No);
                SetString(row, TitleStageIdColumnName, step.StageId);
                SetString(row, TitleStageRoleColumnName, step.StageRole.ToString());
                SetString(row, TitleMonitorIntervalColumnName, step.MonitorIntervalSeconds);
                SetString(row, TitleSkipColumnName, step.Skip);
                SetString(row, TitleTestProcessColumnName, step.TestProcess.ToString());
                SetString(row, TitleContentColumnName, step.Content);
                SetString(row, TitleIdColumnName, step.Id);
                agingTable.Rows.Add(row);
            }

            AddConditionRows(
                dataSet.Tables[TemperatureTableName],
                data.Conditions.OfType<TemperatureCondition>());
            AddConditionRows(
                dataSet.Tables[LivTableName],
                data.Conditions.OfType<LivCondition>());
            AddConditionRows(
                dataSet.Tables[VfTableName],
                data.Conditions.OfType<VfCondition>());
            AddConditionRows(
                dataSet.Tables[IrTableName],
                data.Conditions.OfType<IrCondition>());

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
                Directory.CreateDirectory(directoryPath);

            dataSet.WriteXml(filePath);
        }

        public TestSettingData Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Test setting file was not found.", filePath);

            DataSet dataSet = CreateDataSetSchema();
            dataSet.ReadXml(filePath, XmlReadMode.IgnoreSchema);

            TestSettingData data = new TestSettingData();

            foreach (DataRow row in dataSet.Tables[StageTableName].Rows)
            {
                data.Stages.Add(new AgingStage
                {
                    Id = GetString(row, StageIdColumnName),
                    Duration = GetString(row, StageDurationColumnName, "0"),
                    ExecutionMode = ParseExecutionRepeatMode(GetString(
                        row,
                        StageExecutionModeColumnName,
                        ExecutionRepeatMode.Once.ToString())),
                    ExecutionIntervalSeconds = GetString(
                        row,
                        StageExecutionIntervalColumnName,
                        "0")
                });
            }

            foreach (DataRow row in dataSet.Tables[AgingTableName].Rows)
            {
                TestProcess process = ParseTestProcess(GetString(
                    row,
                    TitleTestProcessColumnName,
                    TestProcess.None.ToString()));

                data.TestSteps.Add(new TestStep
                {
                    No = GetString(row, TitleNoColumnName),
                    StageId = GetString(row, TitleStageIdColumnName),
                    StageRole = ParseStageRole(
                        GetString(row, TitleStageRoleColumnName),
                        process),
                    MonitorIntervalSeconds = GetString(
                        row,
                        TitleMonitorIntervalColumnName,
                        "0"),
                    Skip = GetString(row, TitleSkipColumnName),
                    TestProcess = process,
                    Content = GetString(row, TitleContentColumnName),
                    Id = GetString(row, TitleIdColumnName)
                });
            }

            LoadConditionRows<TemperatureCondition>(
                dataSet.Tables[TemperatureTableName], data.Conditions);
            LoadConditionRows<LivCondition>(
                dataSet.Tables[LivTableName], data.Conditions);
            LoadConditionRows<VfCondition>(
                dataSet.Tables[VfTableName], data.Conditions);
            LoadConditionRows<IrCondition>(
                dataSet.Tables[IrTableName], data.Conditions);

            NormalizeStages(data);
            return data;
        }
        #endregion
    }
}
