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
        private const string AgingTableName = "AgingProcessTable";
        private const string TemperatureTableName = "TemperatureTable";
        private const string LivTableName = "LIVTable";
        private const string VfTableName = "VFTable";
        private const string IrTableName = "IRTable";

        private const string TitleNoColumnName = "Title_No";
        private const string TitleSkipColumnName = "Title_Skip";
        private const string TitleTestProcessColumnName = "Title_TestProcess";
        private const string TitleContentColumnName = "Title_Content";
        private const string TitleIdColumnName = "Title_ID";
        #endregion

        #region private function
        private DataSet CreateDataSetSchema()
        {
            DataSet dataSet = new DataSet("DocumentElement");
            DataTable agingTable = new DataTable(AgingTableName);
            agingTable.Columns.Add(TitleNoColumnName, typeof(string));
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

        private void AddNestedRelation(DataSet dataSet, DataTable parentTable, DataTable childTable)
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

        private void LoadConditionRows<T>(DataTable table, ICollection<TestCondition> conditions)
            where T : TestCondition, new()
        {
            List<PropertyInfo> properties = GetConditionProperties(typeof(T));

            foreach (DataRow row in table.Rows)
            {
                T condition = new T();
                condition.TestStepId = GetString(row, TitleIdColumnName);

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
                    throw new InvalidOperationException(
                        $"{conditionType.Name}.{property.Name} 必須是 string，才能儲存為測試條件欄位。");

                properties.Add(property);
            }

            properties.Sort((left, right) => left.MetadataToken.CompareTo(right.MetadataToken));
            return properties;
        }

        private void SetString(DataRow row, string columnName, string value)
        {
            if (value == null)
                row[columnName] = DBNull.Value;
            else
                row[columnName] = value;
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
                throw new InvalidOperationException($"無法辨識的測試項目：{value}");

            return process;
        }
        #endregion

        #region public function
        public void Save(string filePath, TestSettingData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            DataSet dataSet = CreateDataSetSchema();
            DataTable agingTable = dataSet.Tables[AgingTableName];

            foreach (TestStep step in data.TestSteps)
            {
                DataRow row = agingTable.NewRow();
                SetString(row, TitleNoColumnName, step.No);
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
                throw new FileNotFoundException("找不到測試條件檔案。", filePath);

            DataSet dataSet = CreateDataSetSchema();
            dataSet.ReadXml(filePath, XmlReadMode.IgnoreSchema);

            TestSettingData data = new TestSettingData();

            foreach (DataRow row in dataSet.Tables[AgingTableName].Rows)
            {
                data.TestSteps.Add(new TestStep
                {
                    No = GetString(row, TitleNoColumnName),
                    Skip = GetString(row, TitleSkipColumnName),
                    TestProcess = ParseTestProcess(GetString(
                        row,
                        TitleTestProcessColumnName,
                        TestProcess.None.ToString())),
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

            return data;
        }
        #endregion
    }
}
