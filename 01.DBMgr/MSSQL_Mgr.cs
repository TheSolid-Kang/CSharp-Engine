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
                        if (0 <= sqlDataAdapter.Fill(dt))
                        {
                            System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                        }
                    }
                }
            }
            return dt;
        }

        public DataTable GetSPDataTable(DB_CONNECTION _CON, string _query)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            return GetSPDataTable(url, _query);
        }
        public DataTable GetSPDataTable(string _url, string _query)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_query, conn))
                {
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            sqlDataAdapter.Fill(ds);
                            //cmd.ex
                            System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
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
    }
}
