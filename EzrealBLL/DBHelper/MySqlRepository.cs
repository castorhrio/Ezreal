using EzrealEntity;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EzrealBLL.DBHelper
{
    public class MySqlRepository : IBaseRepository
    {
        public static MySqlHelper sqlHelper { get; set; }
        public MySqlRepository()
        {
            sqlHelper = new MySqlHelper(ConfigurationManager.AppSettings["MySql"]);
        }

        public bool Insert<T>(T t)
        {
            if (t != null)
            {
                string cols = "";
                string vals = "";
                string sql = "insert into {0} ({1}) values ({2});";
                List<MySqlParameter> param = new List<MySqlParameter>();
                Type type = typeof(T);
                PropertyInfo[] property = type.GetProperties();
                var attribute = type.GetCustomAttributes(typeof(TableNameAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    throw new Exception("类" + type.Name + "必须添加 TableNameAttribute 属性");
                }
                string tableName = ((TableNameAttribute)attribute).TableName;
                foreach(PropertyInfo pi in property)
                {
                    string name = pi.Name;
                    if(cols != "")
                    {
                        cols += ",";
                        vals += ",";
                    }

                    cols += name;
                    vals += string.Format("?{0}", name);
                    param.Add(new MySqlParameter(name, type.GetProperty(name).GetValue(t, null)));
                }

                sql = string.Format(sql, tableName, cols, vals);
                return sqlHelper.ExecuteNonQuery(sql, param.ToArray()) > 0;
            }
            else
                return false;
        }

        public bool Delete<T>(string columnName, object value)
        {
            throw new NotImplementedException();
        }

        public List<T> GetList<T>(Hashtable ht)
        {
            throw new NotImplementedException();
        }



        public bool Insert<T>(List<T> list)
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
