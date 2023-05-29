using Engine._05.CStackTracer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._01.DAO
{
    public class MSSQL_DAO : IDisposable
    {
        public MSSQL_DAO()
        {
            var ConnectionStrings = ConfigurationManager.ConnectionStrings;
            string connectionString = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;//App.config에서 작성한 DB정보
            var connection = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
            url = connection;
        }


        ~MSSQL_DAO()
        {
            Dispose();
        }
        private const string DB_HOST = "localhost";
        private const string DB_PORT = "1433";
        private const string DB_USER = "root";
        private const string DB_PASS = "root";
        private const string DB_NAME = "TwoMites";
        private const string CHAR_SET = "UTF8";

        private string host;
        private string port;
        private string user;
        private string pass;
        private string name;

        private string url;
        public object result;
        private DataTable _dataTable = null;
        public DataTable m_dataTable
        {
            get
            {
                var dataTable = _dataTable;
                return dataTable;
            }
            set
            {
                if (_dataTable != null)
                    _dataTable.Dispose();
                _dataTable = value;
            }
        }
        public void Dispose()
        {

        }

        public DataTable GetDataTable(string _query) => _GetDataTable(_query);

        private DataTable _GetDataTable(string _query)
        {
            try
            {
                using (var connection = new SqlConnection(url))
                using (var command = new SqlCommand(_query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0} {1}", reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }
            catch (Exception _e)
            {
                CStackTracer.GetInstance().WriteTraceInfo("DB GetDataTable 에러: " + _e.Message);
                System.Diagnostics.Debug.WriteLine($"예외 == {_e.Message}");
            }
            return null;
        }


        public List<T> SelectList<T>(string query)
        {
            List<T> list = null;
            string connectString = "Server=127.0.0.1;Database=TwoMites;Uid=root;Pwd=root;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
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

        public List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            try
            {
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

    }
}
