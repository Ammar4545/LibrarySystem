using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Services.Contracts
{
    public interface IAuthService
    {
        Users CheckUser(string userName, string password);
        bool CheckUserExists(string userName, string password);
    }
}
