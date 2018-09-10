using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EzrealBLL.Common
{
    public class ObjectHelper
    {
        public static DataTable ObjectConvertToDataTable<T>(T obj,string tableName)
        {
            Type t = typeof(T);
            DataTable dt = new DataTable();
            dt.TableName = tableName;
            FieldInfo[] temp = t.GetFields();
            foreach(FieldInfo fi in temp)
            {
                dt.Columns.Add(new DataColumn(fi.Name, fi.FieldType));
            }

            dt.Rows.Add(FillDataRowFields(dt, t, obj));
            return dt;
        }

        public static DataTable ObjectConvertToDataTable<T>(T[] objs)
        {
            Type t = typeof(T);
            DataTable dt = new DataTable();
            foreach(PropertyInfo pi in t.GetProperties())
            {
                dt.Columns.Add(new DataColumn(pi.Name, pi.PropertyType));
            }

            foreach(object obj in objs)
            {
                dt.Rows.Add(FillDataRowFields(dt, t, obj));
            }
            return dt;
        }

        public static T[] DataTableConvertToObject<T>(DataTable dt)
        {
            T[] objs = new T[dt.Rows.Count];
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                objs[i] = ConvertToObject<T>(dt.Rows[i]);
            }

            return objs;
        }

        private static T ConvertToObject<T>(DataRow dr)
        {
            object proValue = null;
            PropertyInfo propertyInfo = null;
            FieldInfo fieldInfo = null;
            T t = Activator.CreateInstance<T>();

            if (dr != null)
            {
                foreach(DataColumn dc in dr.Table.Columns)
                {
                    propertyInfo = t.GetType().GetProperty(dc.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    fieldInfo = t.GetType().GetField(dc.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    proValue = dr[dc];

                    if(proValue != DBNull.Value)
                    {
                        if(proValue != null)
                        {
                            propertyInfo.SetValue(t, Convert.ChangeType(proValue, propertyInfo.PropertyType), null);
                        }
                        else if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(t, Convert.ChangeType(proValue, fieldInfo.FieldType));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            return t;
        }

        private static DataRow FillDataRow(DataTable dt,Type t, object obj)
        {
            DataRow dr = dt.NewRow();
            DataColumnCollection dcc = dt.Columns;
            for(int i = 0; i < dcc.Count; i++)
            {
                string colname = dcc[i].ColumnName;
                PropertyInfo propertyInfo = t.GetProperty(colname);
                if (propertyInfo != null)
                {
                    object o = propertyInfo.GetValue(obj, null);
                    if ((o != null) && (!Convert.IsDBNull(o)) && o.ToString() != null)
                    {
                        dr[colname] = o;
                    }
                }
            }
            return dr;
        }

        private static DataRow FillDataRowFields(DataTable dt,Type t,object obj)
        {
            DataRow dr = dt.NewRow();
            DataColumnCollection dcc = dt.Columns;
            for(int i = 0; i < dcc.Count; i++)
            {
                string colName = dcc[i].ColumnName;
                FieldInfo pInfo = t.GetField(colName);
                if (pInfo != null)
                {
                    object o = pInfo.GetValue(obj);
                    if((o!=null) && (!Convert.IsDBNull(o) && o.ToString() != ""))
                    {
                        dr[colName] = o;
                    }
                }
            }
            return dr;
        }
    }
}
