using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            , YWDEV
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
    }
}
