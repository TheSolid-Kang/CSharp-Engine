using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Engine._01.DBMgr.MSSQL_Mgr;
using Npgsql;
using System.Collections;

namespace Engine._01.DBMgr
{

    public class PostgreSQL_Mgr : DbMgr, IDisposable
    {
        public PostgreSQL_Mgr()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Url = ConfigurationManager.ConnectionStrings["HomeDBConn"].ConnectionString;//App.config에서 작성한 DB정보

        }

        ~PostgreSQL_Mgr()
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
            using (var conn = new NpgsqlConnection(url))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(_query, conn);
                /*
                SqlDataAdapter sda = new SqlDataAdapter();
                ds = new DataSet();
                sda.SelectCommand = cmd;
                sda.Fill(ds);
                */
                cmd.Dispose();
            }
            return ds;
        }

    }
}
