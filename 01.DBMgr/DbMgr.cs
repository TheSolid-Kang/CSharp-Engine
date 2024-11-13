using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.DirectoryServices.ActiveDirectory;

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

        /// <summary>
        /// WHAT: 지정된 URL과 쿼리를 사용하여 데이터베이스에서 데이터를 조회하고, 결과를 List<T> 형태로 반환합니다.
        /// HOW: 
        /// 1. SqlConnection을 사용하여 데이터베이스에 연결합니다.
        /// 2. SqlCommand를 사용하여 쿼리를 실행하고 SqlDataReader로 결과를 읽습니다.
        /// 3. DataReaderMapToList<T> 메서드를 호출하여 결과를 List<T>로 변환합니다.
        /// </summary>
        /// <typeparam name="T">반환할 객체의 타입</typeparam>
        /// <param name="_url">데이터베이스 연결 문자열</param>
        /// <param name="_query">실행할 SQL 쿼리</param>
        /// <returns>조회된 데이터를 담은 List<T> 객체</returns>
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

        /// <summary>
        /// WHAT: 지정된 DB_CONNECTION과 쿼리를 사용하여 데이터베이스에서 데이터를 조회하고, 결과를 List<T> 형태로 반환합니다.
        /// HOW: 
        /// 1. ConfigurationManager를 사용하여 연결 문자열을 가져옵니다.
        /// 2. SelectList<T> 메서드를 호출하여 데이터를 조회합니다.
        /// </summary>
        /// <typeparam name="T">반환할 객체의 타입</typeparam>
        /// <param name="_CON">DB_CONNECTION 열거형 값</param>
        /// <param name="_query">실행할 SQL 쿼리</param>
        /// <returns>조회된 데이터를 담은 List<T> 객체</returns>
        public List<T> SelectList<T>(DB_CONNECTION _CON, string _query)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            return SelectList<T>(url, _query);
        }

        /// <summary>
        /// WHAT: IDataReader를 사용하여 데이터를 읽고, 결과를 List<T> 형태로 반환합니다.
        /// HOW: 
        /// 1. IDataReader를 사용하여 데이터를 읽습니다.
        /// 2. 각 행을 객체로 변환합니다.
        /// 3. 변환된 객체를 리스트에 추가합니다.
        /// </summary>
        /// <typeparam name="T">반환할 객체의 타입</typeparam>
        /// <param name="dr">IDataReader 객체</param>
        /// <returns>조회된 데이터를 담은 List<T> 객체</returns>
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

        /// <summary>
        /// WHAT: List<T>를 DataTable로 변환합니다.
        /// HOW: 
        /// 1. DataTable 객체를 생성하고 컬럼을 추가합니다.
        /// 2. List<T>의 각 항목을 DataRow로 변환하여 DataTable에 추가합니다.
        /// </summary>
        /// <typeparam name="T">리스트의 객체 타입</typeparam>
        /// <param name="items">변환할 List<T> 객체</param>
        /// <returns>변환된 DataTable 객체</returns>
        public DataTable ConvertListToDataTable<T>(List<T>? items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var propType = prop.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = Nullable.GetUnderlyingType(propType);
                }
                tb.Columns.Add(prop.Name, propType);
            }
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);
                tb.Rows.Add(values);
            }
            return tb;
        }

        /// <summary>
        /// WHAT: DataTable을 List<T>로 변환합니다.
        /// HOW: 
        /// 1. DataTable의 각 행을 읽습니다.
        /// 2. 각 행을 객체로 변환합니다.
        /// 3. 변환된 객체를 리스트에 추가합니다.
        /// </summary>
        /// <typeparam name="T">리스트의 객체 타입</typeparam>
        /// <param name="_dataTable">변환할 DataTable 객체</param>
        /// <returns>변환된 List<T> 객체</returns>
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

        /// <summary>
        /// WHAT: SqlProcedureExecutor 생성자
        /// HOW: 
        /// 1. 생성자 매개변수로 전달된 연결 문자열을 _connectionString 필드에 저장합니다.
        /// </summary>
        /// <param name="connectionString">데이터베이스 연결 문자열</param>
        public SqlProcedureExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// WHAT: 지정된 프로시저 이름과 파라미터를 사용하여 SQL 프로시저를 실행합니다.
        /// HOW: 
        /// 1. SqlConnection을 사용하여 데이터베이스에 연결합니다.
        /// 2. SqlCommand를 사용하여 프로시저를 설정하고 파라미터를 추가합니다.
        /// 3. ExecuteNonQuery를 호출하여 프로시저를 실행합니다.
        /// </summary>
        /// <param name="procedureName">실행할 프로시저 이름</param>
        /// <param name="parameters">프로시저에 전달할 파라미터 배열</param>
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
}