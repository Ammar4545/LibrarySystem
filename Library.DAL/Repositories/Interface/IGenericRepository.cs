using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Repositories.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string tableName);
        T GetById(string tableName, int? id);
        int Add(T entity, string tableName);
        bool DeleteRecord(string tableName, int id);
       
    }
}
