using Engine._05.CStackTracer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Engine._01.DBMgr
{
    public class MSSQL_Mgr : DbMgr, IDisposable
    {

        public MSSQL_Mgr()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Url = ConfigurationManager.ConnectionStrings["HomeDBConn"].ConnectionString;//App.config에서 작성한 DB정보

        }

        ~MSSQL_Mgr()
        {
            Dispose();
        }

        public void Dispose()
        {

        }

        #region SELECT 
        public DataSet GetDataSet(DB_CONNECTION _CON, string _query)
        {
            DataSet ds = null;
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            using (SqlConnection conn = new SqlConnection(url))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(_query, conn);

                SqlDataAdapter sda = new SqlDataAdapter();
                ds = new DataSet();
                sda.SelectCommand = cmd;
                sda.Fill(ds);
                cmd.Dispose();
            }
            return ds;
        }

        public DataTable GetDataTable(DB_CONNECTION _CON, string _query)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            return GetDataTable(url, _query);
        }
        public DataTable GetDataTable(string _url, string _query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        if (0 >= sqlDataAdapter.Fill(dt))
                        {
                            System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                        }
                    }
                }
            }
            return dt;
        }

        public DataTable GetSPDataTable(DB_CONNECTION _CON, string _storedProcedure, SqlParameter[] _sqlParameters)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            return GetSPDataTable(url, _storedProcedure, _sqlParameters);
        }
        public DataTable GetSPDataTable(string _url, string _storedProcedure, SqlParameter[] _sqlParameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_storedProcedure, conn))
                {
                    cmd.Parameters.AddRange(_sqlParameters);
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            if (0 >= sqlDataAdapter.Fill(dt))
                            {
                                System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                            }
                        }
                    }
                    catch (Exception _e)
                    {
                        System.Diagnostics.Debug.WriteLine(_e.Message);
                    }
                }
            }
            return dt;
        }

        public DataSet GetSPDataSet(DB_CONNECTION _CON, string _storedProcedure, SqlParameter[] _sqlParameters)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            return GetSPDataSet(url, _storedProcedure, _sqlParameters);
        }
        public DataSet GetSPDataSet(string _url, string _storedProcedure, SqlParameter[] _sqlParameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_storedProcedure, conn))
                {
                    cmd.Parameters.AddRange(_sqlParameters);
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            if (0 >= sqlDataAdapter.Fill(ds))
                            {
                                System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                            }
                        }
                    }
                    catch (Exception _e)
                    {
                        System.Diagnostics.Debug.WriteLine(_e.Message);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region INSERT
        public void InsertData<T>(DB_CONNECTION _CON, T data)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            InsertData(url, data);
        }
        public void InsertData<T>(string _connectionUrl, T data)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                InsertData(connection, data);
            }
        }
        public void InsertData<T>(SqlConnection connection, T data)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // INSERT 쿼리 생성
            string query = $"INSERT INTO {typeof(T).Name} ({string.Join(", ", properties.Select(p => p.Name))}) VALUES ({string.Join(", ", properties.Select(p => $"@{p.Name}"))})";
            //string query = $"INSERT INTO yw_TADUsers_IF ({string.Join(", ", properties.Select(p => p.Name))}) VALUES ({string.Join(", ", properties.Select(p => $"@{p.Name}"))})";

            // SQL 커맨드 객체 생성 및 파라미터 바인딩
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)property.GetValue(data);
                        if (dateTime.Year > 1753)
                        {
                            command.Parameters.AddWithValue($"@{property.Name}", dateTime);
                        }
                        else
                        {
                            command.Parameters.AddWithValue($"@{property.Name}", "");//'1900-01-01 00:00:00.000' 으로 들어감.
                        }
                    }
                    else
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(data));
                    }
                }

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception _e)
                {
                    Debug.WriteLine(_e.Message);
                }
                // 쿼리 실행
            }
        }

        /// <summary>
        /// 
        /// 사용 예시
        ///private void SaveErpAdUsersTbl_IF()
        ///{
        ///    //1. 변수 초기화
        ///    Dictionary<string, List<Users>> mapADUsers = GetMapADUsers();
        ///
        ///    //2. Insert Query
        ///    using (var mgr = new MSSQL_Mgr())
        ///    {
        ///        foreach (var keyPairs in mapADUsers)
        ///        {
        ///            List<Users> adUsers = keyPairs.Value;
        ///            adUsers.ForEach(adUser => mgr.InsertDataByTableName<Users>(DbMgr.DB_CONNECTION.YWDEV, adUser,"yw_TADUsers_IF"));
        ///        }
        ///    }
        ///
        ///    //3. Update Query 작성
        ///}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_CON"></param>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        public void InsertDataByTableName<T>(DB_CONNECTION _CON, T data, string tableName)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            InsertDataByTableName(url, data, tableName);
        }
        public void InsertDataByTableName<T>(string _connectionUrl, T data, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                InsertDataByTableName(connection, data, tableName);
            }
        }
        public void InsertDataByTableName<T>(SqlConnection connection, T data, string tableName)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // INSERT 쿼리 생성
            string query = $"INSERT INTO {tableName} ({string.Join(", ", properties.Select(p => p.Name))}) VALUES ({string.Join(", ", properties.Select(p => $"@{p.Name}"))})";

            // SQL 커맨드 객체 생성 및 파라미터 바인딩
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)property.GetValue(data);
                        if (dateTime.Year > 1753)
                        {
                            command.Parameters.AddWithValue($"@{property.Name}", dateTime);
                        }
                        else
                        {
                            command.Parameters.AddWithValue($"@{property.Name}", "");//'1900-01-01 00:00:00.000' 으로 들어감.
                        }
                    }
                    else
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(data));
                    }
                }

                // 쿼리 실행
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception _e)
                {
                    Debug.WriteLine(_e.Message);
                }
            }
        }

        #endregion
        #region DELETE
        public void DeleteData<T>(DB_CONNECTION _CON, Dictionary<string, object> conditions)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            DeleteData<T>(url, conditions);
        }
        public void DeleteData<T>(string _connectionUrl, Dictionary<string, object> conditions)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                DeleteData<T>(connection, conditions);
            }
        }
        public void DeleteData<T>(SqlConnection connection, Dictionary<string, object> conditions)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // WHERE 절 생성
            var conditionStrings = new List<string>();
            foreach (var property in properties)
            {
                if (conditions.TryGetValue(property.Name, out var value))
                {
                    conditionStrings.Add($"{property.Name} = @{property.Name}");
                }
            }

            string query = $"DELETE FROM {typeof(T).Name} WHERE {string.Join(" AND ", conditionStrings)}";

            // SQL 커맨드 객체 생성 및 파라미터 바인딩
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var condition in conditions)
                {
                    command.Parameters.AddWithValue($"@{condition.Key}", condition.Value);
                }

                // 쿼리 실행
                command.ExecuteNonQuery();
            }
        }

        public void DeleteDataByKey<T>(DB_CONNECTION _CON, object key)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            DeleteDataByKey<T>(url, key);
        }
        public void DeleteDataByKey<T>(string _connectionUrl, object key)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                DeleteDataByKey<T>(connection, key);
            }
        }
        public void DeleteDataByKey<T>(SqlConnection connection, object key)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // KEY 속성 찾기
            var keyProperty = properties.FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

            if (keyProperty != null)
            {
                string query = $"DELETE FROM {typeof(T).Name} WHERE {keyProperty.Name} = @{keyProperty.Name}";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue($"@{keyProperty.Name}", key);

                    // 쿼리 실행
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                throw new Exception("No key property found in the data model class.");
            }
        }

        #endregion
        #region UPDATE
        public void UpdateDataByKey<T>(DB_CONNECTION _CON, object key, Dictionary<string, object> updatedValues)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            UpdateDataByKey<T>(url, key, updatedValues);
        }
        public void UpdateDataByKey<T>(string _connectionUrl, object key, Dictionary<string, object> updatedValues)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                UpdateDataByKey<T>(connection, key, updatedValues);
            }
        }
        public void UpdateDataByKey<T>(SqlConnection connection, object key, Dictionary<string, object> updatedValues)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // KEY 속성 찾기
            var keyProperty = properties.FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

            if (keyProperty != null)
            {
                // 업데이트 쿼리 생성
                string query = $"UPDATE {typeof(T).Name} SET ";

                foreach (var kvp in updatedValues)
                {
                    query += $"{kvp.Key} = @{kvp.Key}, ";
                }

                query = query.TrimEnd(',', ' ') + $" WHERE {keyProperty.Name} = @{keyProperty.Name}";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // 파라미터 바인딩
                    command.Parameters.AddWithValue($"@{keyProperty.Name}", key);
                    foreach (var kvp in updatedValues)
                    {
                        command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                    }

                    // 쿼리 실행
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                throw new Exception("No key property found in the data model class.");
            }
        }
        #endregion
    }
}
