using Library.DAL.DTOs;
using Library.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Services.Contracts
{
    public interface IBookService
    {
        IEnumerable<Books> GetAll(string tableName);
        Books GetById(string tableName, int? id);
        int Add(Books entity, string tableName);
        bool DeleteRecord(string tableName, int id);
        int Update(Books entity);

        bool BorrowBook(int userId, int bookId);
        bool ReturnBook(int userId, int bookId);
        List<BorrowedBook> GetBorrowedBooksByUser(string userName);
        List<Books> SearchBooks(string query);
       


    }
}
