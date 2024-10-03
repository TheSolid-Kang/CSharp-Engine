using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Engine._01.DBMgr
{
    public class DbMgr
    {
        public enum DB_CONNECTION
        {
            HOME = 0
            , ERP
            , ERP_DEV
            , MES1
            , MATERIAL
            , TWO_MITES
            , CALEB
            , GW
            , YW
            , YWDEV
            , YQMS
            , END
        }

        public List<T> SelectList<T>(string _url, string _query)
        {
            List<T> list = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(_url))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(_query, conn);
                    SqlDataReader dr = cmd.ExecuteReader();
                    list = DataReaderMapToList<T>(dr);
                    cmd.Dispose();
                    dr.Close();
                }
            }
            catch (Exception _e)
            {
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }

            return list;
        }

        public List<T> SelectList<T>(DB_CONNECTION _CON, string _query)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            return SelectList<T>(url, _query);
        }


        private List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            try
            {
                T obj = default(T);
                list = new List<T>(dr.FieldCount);
                while (dr.Read())
                {
                    obj = System.Activator.CreateInstance<T>();
                    foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(dr[prop.Name], System.DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }
            }
            catch (Exception _e)
            {
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }
            return list;
        }
        public DataTable ConvertListToDataTable<T>(List<T>? items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
                tb.Columns.Add(prop.Name, prop.PropertyType);
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);
                tb.Rows.Add(values);
            }
            return tb;
        }

        public List<T> ConvertDataTableToList<T>(DataTable _dataTable)
        {
            List<T> list = new List<T>(_dataTable.Rows.Count);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            try
            {

                foreach (DataRow dataRow in _dataTable.Rows)
                {
                    T obj = System.Activator.CreateInstance<T>();
                    foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (false == dataRow.Table.Columns.Contains(prop.Name))
                            continue;
                        if (!object.Equals(dataRow[prop.Name], System.DBNull.Value))
                        {
                            prop.SetValue(obj, dataRow[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }
            }
            catch (Exception _e)
            {
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }
            return list;
        }
    }

    public class SqlProcedureExecutor
    {
        private string _connectionString;

        public SqlProcedureExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteProcedure(string procedureName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // 파라미터 추가
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("프로시저가 성공적으로 실행되었습니다.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("프로시저 실행 중 오류 발생: " + ex.Message);
                    }
                }
            }
        }
    }

    /*
     class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=your_server;Database=your_database;User Id=your_username;Password=your_password;"; // 연결 문자열을 입력하세요.
        SqlProcedureExecutor executor = new SqlProcedureExecutor(connectionString);

        string procedureName = "YourStoredProcedure"; // 실행할 프로시저 이름
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Parameter1", SqlDbType.Int) { Value = 10 },
            new SqlParameter("@Parameter2", SqlDbType.VarChar) { Value = "Sample" }
        };

        executor.ExecuteProcedure(procedureName, parameters);
    }
}

    SqlProcedureExecutor 클래스: 이 클래스는 SQL Server에 연결하고 프로시저를 실행하는 기능을 제공합니다.
ExecuteProcedure 메서드: 주어진 프로시저 이름과 파라미터 배열을 사용하여 프로시저를 실행합니다.
사용 예제: 연결 문자열, 프로시저 이름, 및 파라미터를 설정하고 ExecuteProcedure 메서드를 호출하여 프로시저를 실행합니다.
이 코드를 사용하여 SQL 프로시저를 쉽게 호출할 수 있습니다. 필요에 따라 예외 처리를 추가하거나, 반환 값을 처리하는 기능을 추가할 수 있습니다.
     */
}
