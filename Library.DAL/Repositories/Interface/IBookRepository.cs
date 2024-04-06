using Library.DAL.DTOs;
using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Library.DAL.Repositories.Interface
{
    public interface IBookRepository : IGenericRepository<Books>
    {
        bool BorrowBook(int userId, int bookId);
        bool ReturnBook(int userId, int bookId);
        List<BorrowedBook> GetBorrowedBooksByUser(string userName);
        List<Books> SearchBooksByTitleOrAuthor(string searchQuery);
        int Update(Books entity);
    }
}
