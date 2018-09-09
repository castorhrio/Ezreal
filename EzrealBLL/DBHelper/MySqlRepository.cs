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
            //if (t != null)
            //{
            //    string sql = "insert into {0} ({1}) values ({2});";
            //    List<MySqlParameter> param = new List<MySqlParameter>();
            //    Type type = typeof(T);
            //    PropertyInfo[] property = type.GetProperties();
            //    var attribute = type.GetCustomAttributes(typeof(TableNameAttribute), false).FirstOrDefault();
            //}
            throw new NotImplementedException();
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
