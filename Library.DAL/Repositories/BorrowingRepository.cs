using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.DAL.Setting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Repositories
{
    public class BorrowingRepository : GenericRepository<Borrowings>, IBorrowingRepository
    {
        public BorrowingRepository(IOptions<ConnectionSetting> connection) : base(connection)
        {
        }
    }
}
