using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzrealBLL
{
    public interface IBaseRepository
    {
        bool Insert<T>(T t);
        bool Insert<T>(List<T> list);
        bool Update<T>(T t);
        bool Delete<T>(string columnName,object value);
        List<T> GetList<T>(Hashtable ht);
    }
}
