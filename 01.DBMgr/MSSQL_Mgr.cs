using Engine._05.CStackTracer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                        if(dateTime.Year > 1753)
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
                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
