﻿using Library.BLL.Services.Contracts;
using Library.DAL.DTOs;
using Library.DAL.Entities;
using Library.DAL.Repositories;
using Library.DAL.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepo;
        public BookService(IBookRepository bookRepo)
        {
            _bookRepo = bookRepo;
        }

        public int Add(Books book, string tableName)
        {
           
            return _bookRepo.Add(book, tableName);
        }

        public bool BorrowBook(int userId, int bookId)
        {
            return _bookRepo.BorrowBook(userId, bookId);
        }

        public bool DeleteRecord(string tableName, int id)
        {
            return _bookRepo.DeleteRecord("Books", id);
        }

        public IEnumerable<Books> GetAll(string tableName)
        {
            return _bookRepo.GetAll(tableName);
        }

        public List<BorrowedBook> GetBorrowedBooksByUser(string userName)
        {
            return _bookRepo.GetBorrowedBooksByUser(userName);
        }

        public Books GetById(string tableName, int? id)
        {
            return _bookRepo.GetById("Books", id);
        }

        public bool ReturnBook(int userId, int bookId)
        {
            return _bookRepo.ReturnBook(userId, bookId);
        }

        public List<BookDto> SearchBooks(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("Search query cannot be empty.");
            }
            var booksToReturn= _bookRepo.SearchBooksByTitleOrAuthor(query.ToLower());
            return booksToReturn;
        }

        public int Update(Books book)
        {
            return _bookRepo.Update(book);
        }
    }
}