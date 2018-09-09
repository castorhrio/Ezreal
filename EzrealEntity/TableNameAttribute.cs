using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzrealEntity
{
    /// <summary>
    /// 实体与数据表的映射
    /// </summary>
    public class TableNameAttribute:Attribute
    {
        private string tableName;
        private string primaryKey;

        public string TableName
        {
            set { tableName = value; }
            get { return tableName; }
        }

        public string PrimaryKey
        {
            set { primaryKey = value; }
            get { return primaryKey; }
        }


        public TableNameAttribute(string tableName,string primaryKey = "ID")
        {
            this.tableName = tableName;
            this.primaryKey = primaryKey;
        }
    }
}
