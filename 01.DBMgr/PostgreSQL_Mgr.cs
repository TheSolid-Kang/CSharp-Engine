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

    public class PostgreSQL_Mgr : IDisposable
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
        , END
        }

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

        public List<T> SelectList<T>(DB_CONNECTION _CON, string _query)
        {
            string url = ConfigurationManager.ConnectionStrings[Enum.GetName(_CON)].ConnectionString;
            List<T> list = null;
            try
            {
                using (var conn = new NpgsqlConnection(url))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        //컬럼 이용시 \"컬럼명\"
                        cmd.CommandText = _query;
                        cmd.Connection = conn;
                        using (var dr = cmd.ExecuteReader())
                        {
                            list = DataReaderMapToList<T>(dr);
                        }
                    }
                    conn.Close();
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
    }
}
