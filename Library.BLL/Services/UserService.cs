using Library.BLL.Services.Contracts;
using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public int Add(Users user, string tableName)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return _userRepo.Add(user, tableName);
        }

        public bool DeleteRecord(string tableName, int id)
        {
            if (_userRepo.DeleteRecord("Users", id))
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Users> GetAll(string tableName)
        {
            return _userRepo.GetAll( tableName);
        }

        public Users GetById(string tableName, int? id)
        {
            return _userRepo.GetById(tableName,id);
        }
        public int Update(Users user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return _userRepo.Update(user);
        }
    }
}
