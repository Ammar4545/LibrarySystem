using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Repositories.Interface
{
    public interface IAuthRepository
    {
        Users CheckUser(string userName, string password);

        bool CheckUserExists(string userName, string password);
    }
}
