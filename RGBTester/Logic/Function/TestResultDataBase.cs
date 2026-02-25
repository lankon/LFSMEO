using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;

namespace RGBTester.Logic
{
    public class TestResultDataBase
    {
        public TestResultDataBase()
        {
            Manager.InitDatabase();
        }

        #region parameter define
        public DatabaseManager Manager = new DatabaseManager();
        #endregion

        public class ProductionLog
        {
            public int ID { get; set; }                     //DataBase ID
            public string ProductType { get; set; }         //產品種類
            public string SN { get; set; }                  //SN
            public DateTime TestTime { get; set; }          //測試時間
            public int? IsPass { get; set; }                 //測試結果 (Pass/Fail)
            public int? Exclude { get; set; }                //是否排除在統計之外
            public string Description { get; set; }         //測試描述或備註
        }
        
        public class ProductionSummary
        {
            public string ProductType { get; set; }         //產品種類
            public int TotalUnits { get; set; }             //總數量
            public int PassUnits { get; set; }              //成功數量
            public int FailUnits { get; set; }              //失敗數量

            public double Yield => TotalUnits > 0 ? Math.Round((double)PassUnits / TotalUnits * 100, 2) : 0;
        }

        public class  YieldResult
        {
            public int TotalUnits { get; set; }
            public int PassUnits { get; set; }
            public int FailUnits { get; set; }
            public int ExcludeUnits => TotalUnits - PassUnits - FailUnits;
            public double Yield => TotalUnits > 0 ? (double)PassUnits / (PassUnits + FailUnits) : 0;
        }

        public class DatabaseManager
        {
            #region parameter define
            private string _connStr = $"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\Setting\\YieldReport.db;Version=3;";
            #endregion

            #region private function
            private void FilterCondition(ref StringBuilder sql, ref DynamicParameters parameters, ProductionLog select_data)
            {
                string type = select_data?.ProductType;
                string sn = select_data?.SN;
                int? is_pass = select_data.IsPass;
                int? exclude = select_data.Exclude;
                string description = select_data?.Description;

                if (!string.IsNullOrWhiteSpace(type))
                {
                    sql.Append(" AND ProductType LIKE @type ");
                    parameters.Add("type", $"%{type}%");
                }

                if (!string.IsNullOrWhiteSpace(sn))
                {
                    sql.Append(" AND SN LIKE @sn ");
                    parameters.Add("sn", $"%{sn}%");
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    sql.Append(" AND Description LIKE @description ");
                    parameters.Add("description", $"%{description}%");
                }

                if (is_pass != 2 && is_pass != -1)
                {
                    sql.Append(" AND IsPass = @is_pass ");
                    parameters.Add("is_pass", is_pass);
                }

                if (exclude != 0 && exclude != -1)
                {
                    sql.Append(" AND Exclude = @exclude ");
                    parameters.Add("exclude", exclude);
                }
            }
            #endregion

            #region public function
            public void InitDatabase()
            {
                using (var conn = new SQLiteConnection(_connStr))
                {
                    string sql = @"
                                    CREATE TABLE IF NOT EXISTS ProductionLogs (
                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    ProductType TEXT,
                                    SN TEXT,
                                    TestTime DATETIME,
                                    IsPass INTEGER,
                                    Exclude INTEGER,
                                    Description TEXT
                                );";
                    conn.Execute(sql);
                }
            }

            public void InsertData(ProductionLog log)
            {
                using (var conn = new SQLiteConnection(_connStr))
                {
                    string sql = @"INSERT INTO ProductionLogs (ProductType, SN, TestTime, IsPass, Exclude, Description) 
                                   VALUES (@ProductType, @SN, @TestTime, @IsPass, @Exclude, @Description)";
                    conn.Execute(sql, log);
                }
            }

            public void UpdateDatabase(int id, string columnName, object newValue)
            {
                if (columnName == "Exclude" && newValue == null)
                    newValue = 0;
                
                using (var conn = new SQLiteConnection(_connStr))
                {
                    string sql = $@"UPDATE ProductionLogs SET [{columnName}] = @newValue WHERE ID = @id";

                    conn.Execute(sql, new { newValue, id });
                }
            }

            public List<ProductionLog> GetResult(ProductionLog select_data, DateTime start, DateTime end)
            {
                if (start == DateTime.MinValue || end == DateTime.MinValue)
                {
                    start = DateTime.Now.AddDays(-1);
                    end = DateTime.Now.AddDays(1);
                }

                using (var conn = new SQLiteConnection(_connStr))
                {
                    StringBuilder sql = new StringBuilder(@"
                        SELECT 
                            ID, ProductType, SN, TestTime, IsPass, Exclude, Description
                        FROM ProductionLogs
                        WHERE TestTime BETWEEN datetime(@start) AND datetime(@end) ");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("start", start);
                    parameters.Add("end", end);

                    FilterCondition(ref sql, ref parameters, select_data);

                    sql.Append(" ORDER BY TestTime DESC");

                    return conn.Query<ProductionLog>(sql.ToString(), parameters).ToList();
                }
            }

            public YieldResult GetSummaryReport(ProductionLog select_data, DateTime start, DateTime end)
            {
                if (start == DateTime.MinValue || end == DateTime.MinValue)
                {
                    start = DateTime.Now.AddDays(-1);
                    end = DateTime.Now.AddDays(1);
                }

                using (var conn = new SQLiteConnection(_connStr))
                {
                    //從資料庫提取資料
                    StringBuilder sql = new StringBuilder(@"
                        SELECT 
                            ProductType,
                            COUNT(*) as TotalUnits,
                            SUM(CASE WHEN IsPass = 1 AND Exclude = 0 THEN 1 ELSE 0 END) as PassUnits,
                            SUM(CASE WHEN IsPass = 0 AND Exclude = 0 THEN 1 ELSE 0 END) as FailUnits
                        FROM ProductionLogs
                        WHERE TestTime BETWEEN datetime(@start) AND datetime(@end) ");

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("start", start);
                    parameters.Add("end", end);

                    FilterCondition(ref sql, ref parameters, select_data);

                    sql.Append(" GROUP BY ProductType ");

                    //取得分類結果
                    List<ProductionSummary> report = conn.Query<ProductionSummary>(sql.ToString(), parameters).ToList();

                    //計算結果
                    int total_units = 0, fail_units = 0, pass_units = 0, yield = 0;
                    for (int i = 0; i < report.Count; i++)
                    {
                        if (report[i].ProductType.Contains(select_data.ProductType))
                        {
                            fail_units += report[i].FailUnits;
                            pass_units += report[i].PassUnits;
                            total_units += report[i].TotalUnits;
                        }
                    }

                    //回傳結果
                    YieldResult yield_result = new YieldResult
                    {
                        TotalUnits = total_units,
                        PassUnits = pass_units,
                        FailUnits = fail_units
                    };
                    return yield_result;
                }
            }
            #endregion
        }
    }
}
