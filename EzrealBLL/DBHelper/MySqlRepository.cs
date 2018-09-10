using EzrealBLL.Common;
using EzrealEntity;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool Insert<T>(List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                List<MySqlParameter[]> listparam = new List<MySqlParameter[]>();
                List<string> listSql = new List<string>();
                string sqlTemp = "insert into {0} ({1}) values ({2});";

                Type type = typeof(T);
                PropertyInfo[] propertyInfo = type.GetProperties();
                var attribute = type.GetCustomAttributes(typeof(TableNameAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    throw new Exception("类" + type.Name + "必须添加 TableNameAttribute 属性");
                }

                string tableName = ((TableNameAttribute)attribute).TableName;
                int index = 0;
                foreach (T t in list)
                {
                    string cols = "";
                    string vals = "";
                    List<MySqlParameter> param = new List<MySqlParameter>();
                    foreach (PropertyInfo pi in propertyInfo)
                    {
                        string name = pi.Name;
                        if (cols != "")
                        {
                            cols += ",";
                            vals += ",";
                        }
                        cols += name;
                        vals += string.Format("?{0}", name + index.ToString());
                        param.Add(new MySqlParameter(pi.Name + index.ToString(), type.GetProperty(pi.Name).GetValue(t, null)));
                    }
                    listSql.Add(string.Format(sqlTemp, tableName, cols, vals));
                    listparam.Add(param.ToArray());
                    index++;
                }

                sqlHelper.BatchInsert(listSql, listparam);
                return true;
            }
            else
                return false;
        }

        public bool Update<T>(T t)
        {
            if (t != null)
            {
                string sql = "update {0} set {1} where {2};";
                string sqlSet = "";
                string sqlWhere = "";
                List<MySqlParameter> param = new List<MySqlParameter>();
                Type type = typeof(T);
                PropertyInfo[] propertyInfos = type.GetProperties();
                var attribute = type.GetCustomAttributes(typeof(TableNameAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    throw new Exception("类" + type.Name + "必须添加 TableNameAttribute 属性");
                }

                string tableName = ((TableNameAttribute)attribute).TableName;
                string primaryKey = ((TableNameAttribute)attribute).PrimaryKey;
                foreach (PropertyInfo pi in propertyInfos)
                {
                    string name = pi.Name;
                    object value = type.GetProperty(name).GetValue(t, null);
                    //值为空不更新
                    if (value == null || name.ToLower() == "createtime" || name.ToLower() == "createby")
                    {
                        continue;
                    }
                    if (name == primaryKey)
                    {
                        sqlWhere = string.Format("{0}=?{0}", name);
                    }
                    else
                    {
                        if (sqlSet != "")
                        {
                            sqlSet += ",";
                        }
                        sqlSet += string.Format("{0}=?{0}", name);
                    }
                    param.Add(new MySqlParameter(name, type.GetProperty(name).GetValue(t, null)));
                }
                sql = string.Format(sql, tableName, sqlSet, sqlWhere);
                return sqlHelper.ExecuteNonQuery(sql, param.ToArray()) > 0;
            }
            else
                return false;
        }

        public bool Delete<T>(string columnName, object value)
        {
            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();
            var attribute = type.GetCustomAttributes(typeof(TableNameAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                throw new Exception("类" + type.Name + "必须添加 TableNameAttribute 属性");
            }

            string tableName = ((TableNameAttribute)attribute).TableName;
            StringBuilder sb = new StringBuilder();
            MySqlParameter[] param = null;
            if (columnName == "")
                sb.AppendFormat("delete from {0} where {1}=@{1}", tableName, columnName);
            else
            {
                sb.AppendFormat("delete from {0} where {1}=@{1}", tableName, columnName);
                param = new MySqlParameter[]
                {
                    new MySqlParameter(columnName,value)
                };
            }
            return sqlHelper.ExecuteNonQuery(CommandType.Text, sb.ToString(), param) > 0;
        }

        public List<T> GetList<T>(Hashtable ht)
        {
            Type type = typeof(T);
            PropertyInfo[] propertyInfo = type.GetProperties();
            var attribute = type.GetCustomAttributes(typeof(TableNameAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                throw new Exception("类" + type.Name + "必须添加 TableNameAttribute 属性");
            }
            string tableName = ((TableNameAttribute)attribute).TableName;
            string sql = "select * from " + tableName + " where 1=1";
            List<MySqlParameter> param = new List<MySqlParameter>();
            foreach(string key in ht.Keys)
            {
                sql += string.Format(" and {0}=?{0}", key);
                param.Add(new MySqlParameter(key, ht[key]));
            }
            DataTable dt = sqlHelper.ExecuteDataTable(CommandType.Text, sql, param.ToArray());
            dt.TableName = tableName;
            return ObjectHelper.DataTableConvertToObject<T>(dt).ToList() ?? default(List<T>);
        }
    }
}
