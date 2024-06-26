﻿using System;
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
}
