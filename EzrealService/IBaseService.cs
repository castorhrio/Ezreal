using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzrealService
{
    public interface IBaseService
    {
        bool Insert<T>(T t);
        bool Insert<T>(List<T> list);
        bool Update<T>(T t);
        bool Delete<T>(string colName, string value);
        T GetModel<T>(string colName, string value);
        T GetModel<T>(Hashtable ht);
        List<T> GetList<T>(string colName, string value);
        List<T> GetList<T>(Hashtable ht);
    }
}
