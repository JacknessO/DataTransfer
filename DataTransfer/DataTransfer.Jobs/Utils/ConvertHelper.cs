using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public class ConvertHelper<T> where T : new()
    {
        public static List<T> ConvertToList(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                List<T> list = new List<T>();

                Type type = typeof(T);

                foreach (DataRow row in dt.Rows)
                {
                    T model = (T)Activator.CreateInstance(type);

                    foreach (PropertyInfo prop in type.GetProperties())
                    {
                        prop.SetValue(model, Convert.ChangeType(row[prop.Name] == DBNull.Value ? 0 : row[prop.Name], prop.PropertyType));
                    }
                    list.Add(model);
                }
                return list;
            }
            return null;
        }

        public static DataTable ConvertToDT(List<T> list)
        {
            Type type = typeof(T); 
            DataTable dt = new DataTable(type.Name);
            DataColumn column = null;  
            foreach (PropertyInfo prop in type.GetProperties())
            {
                column = new DataColumn(prop.Name, prop.PropertyType);
                dt.Columns.Add(column);
            }
            DataRow dr = dt.NewRow();
            foreach (var model in list)
            {
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    dr[prop.Name] = prop.GetValue(model);
                }
                dt.Rows.Add(dr);
            }
            return dt; 
        }
    }
}
