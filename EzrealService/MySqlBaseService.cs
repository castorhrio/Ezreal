using EzrealBLL;
using EzrealBLL.DBHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzrealService
{
    public class MySqlBaseService : IBaseService
    {
        public IBaseRepository repository = new MySqlRepository();

        public bool Insert<T>(T t)
        {
            return repository.Insert<T>(t);
        }

        public bool Insert<T>(List<T> list)
        {
            return repository.Insert<T>(list);
        }

        public bool Delete<T>(string colName, string value)
        {
            return repository.Delete<T>(colName, value);
        }

        public bool Update<T>(T t)
        {
            return repository.Update<T>(t);
        }

        public T GetModel<T>(string colName, string value)
        {
            return repository.GetList<T>(colName, value).FirstOrDefault();
        }

        public T GetModel<T>(Hashtable ht)
        {
            return repository.GetList<T>(ht).FirstOrDefault();
        }

        public List<T> GetList<T>()
        {
            return repository.GetList<T>("", "");
        }

        public List<T> GetList<T>(string colName, string value)
        {
            return repository.GetList<T>(colName, value);
        }

        public List<T> GetList<T>(Hashtable ht)
        {
            return repository.GetList<T>(ht);
        }
    }
}
