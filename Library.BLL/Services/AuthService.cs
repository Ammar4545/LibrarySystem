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
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        public AuthService(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public Users CheckUser(string userName, string password)
        {
            return _authRepo.CheckUser(userName, password);
             
        }

        public bool CheckUserExists(string userName, string password)
        {
           return _authRepo.CheckUserExists(userName, password);
        }
    }
}
