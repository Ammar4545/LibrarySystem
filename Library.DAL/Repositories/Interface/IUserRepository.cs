using Library.DAL.DTOs;
using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<Users> 
    {
        int Update(Users entity);
    }
}
