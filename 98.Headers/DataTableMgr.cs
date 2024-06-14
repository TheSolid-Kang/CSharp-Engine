using Engine._05.CStackTracer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine._98.Headers
{
    public class DataTableMgr
    {
        public static DataTable ToDataTable<T>(List<T>? items)
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

        public static List<T> ConvertDataTableToList<T>(DataTable _dataTable)
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
                            prop.SetValue(obj, dataRow[prop.Name], null);
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
