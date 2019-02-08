using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace Util
{
    public class Converter
    {
        public static DataTable ToDataTable<T>(T list) where T : IList
        {
            var table = new DataTable(typeof(T).GetGenericArguments()[0].Name);
            if ((typeof(T).GetGenericArguments()[0].Equals(typeof(string)))
                || (typeof(T).GetGenericArguments()[0].IsPrimitive))
            {
                var t = typeof(T).GetGenericArguments()[0];
                DataColumn dc = new DataColumn("Value", t);
                table.Columns.Add(dc);
                foreach (var item in list)
                {
                    DataRow dr = table.NewRow();
                    dr[0] = item;
                    table.Rows.Add(dr);
                }
            }
            else
            {

                typeof(T).GetGenericArguments()[0].GetProperties().
                    ToList().ForEach(p => table.Columns.Add(p.Name, p.PropertyType));
                foreach (var item in list)
                {
                    var row = table.NewRow();
                    item.GetType().GetProperties().
                        ToList().ForEach(p => row[p.Name] = p.GetValue(item, null));
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public static T ToList<T>(DataTable table) where T : IList, new()
        {
            var list = new T();
            foreach (DataRow row in table.Rows)
            {
                if ((typeof(T).GetGenericArguments()[0].Equals(typeof(string)))
                    || (typeof(T).GetGenericArguments()[0].IsPrimitive))
                {
                    list.Add(row[0]);
                }
                else
                {
                    var item = Activator.CreateInstance(typeof(T).GetGenericArguments()[0]);
                    list.GetType().GetGenericArguments()[0].GetProperties().ToList().
                        ForEach(p => p.SetValue(item, row[p.Name], null));
                    list.Add(item);
                }
            }

            return list;
        }

        public static Dictionary<string, string> ToDictionary(NameValueCollection nvc)
        {
            var d = new Dictionary<string, string>();
            foreach (string key in nvc.Keys)
            {
                d.Add(key, nvc[key]);
            }
            return d;
        }
    }
}
