using Engine._05.CStackTracer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._01.DBMgr
{
    public class MSSQL_Mgr : IDisposable
    {
        public MSSQL_Mgr()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            Url = ConfigurationManager.ConnectionStrings["HomeDBConn"].ConnectionString;//App.config에서 작성한 DB정보
        }


        ~MSSQL_Mgr()
        {
            Dispose();
        }
        public string Url { get; set; }

        public void Dispose()
        {

        }



        public List<T> SelectList<T>(string query)
        {
            List<T> list = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(Url))
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
