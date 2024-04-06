using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Services.Contracts
{
    public interface IUserService
    {
        IEnumerable<Users> GetAll(string tableName);
        Users GetById(string tableName, int? id);
        int Add(Users entity, string tableName);
        bool DeleteRecord(string tableName, int id);
        int Update(Users entity);
    }
}
