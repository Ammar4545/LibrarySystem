﻿using Library.DAL.DTOs;
using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.DAL.Setting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Repositories
{
    public class BookRepository : GenericRepository<Books>, IBookRepository
    {
        private readonly ConnectionSetting _connection;
        public BookRepository(IOptions<ConnectionSetting> connection) : base(connection)
        {
            _connection = connection.Value;
        }
        public bool BorrowBook(int userId,int bookId)
        {
            
            using (var connection = new SqlConnection(_connection.SQLString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        try
                        {
                            // Check if there are available copies
                            command.CommandText = "SELECT AvailableCopies FROM Books WHERE BookId = @BookId";
                            command.Parameters.AddWithValue("@BookId", bookId);

                            int availableCopies = (int)command.ExecuteScalar();

                            if (availableCopies > 0)
                            {
                                // Insert a record into Borrowings
                                command.CommandText = "INSERT INTO Borrowings (UserId, BookId, BorrowDate) VALUES (@UserId, @BookId, GETDATE())";
                                command.Parameters.AddWithValue("@UserId", userId);

                                command.ExecuteNonQuery();

                                // Decrease AvailableCopies by 1
                                command.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookId = @BookId";

                                command.ExecuteNonQuery();

                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false; // No copies available
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
        public bool ReturnBook(int userId, int bookId)
        {

            using (var connection = new SqlConnection(_connection.SQLString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        try
                        {
                            // Update Borrowings to set ReturnDate for the book
                            command.CommandText = "UPDATE Borrowings SET ReturnDate " +
                                "= GETDATE() WHERE UserId = @UserId AND BookId = @BookId AND ReturnDate IS NULL";
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@BookId", bookId);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Increase AvailableCopies by 1
                                command.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookId = @BookId";

                                command.ExecuteNonQuery();

                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false; // No matching borrowing record found
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }

        }
        public List<BorrowedBook> GetBorrowedBooksByUser(string userName)
        {
            var borrowedBooks = new List<BorrowedBook>();

            string query = @"
                SELECT b.Title, b.Author, br.BorrowDate, br.ReturnDate
                FROM Borrowings br
                JOIN Books b ON br.BookId = b.BookId
                JOIN Users u ON br.UserId = u.UserId
                WHERE u.UserName = @UserName;";

            using (var connection = new SqlConnection(_connection.SQLString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", userName);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new BorrowedBook
                        {
                            Title = reader["Title"].ToString(),
                            Author = reader["Author"].ToString(),
                            BorrowDate = (DateTime)reader["BorrowDate"],
                            ReturnDate = reader["ReturnDate"] as DateTime?
                        };
                        borrowedBooks.Add(book);
                    }
                }
            }

            return borrowedBooks;
        }
        public List<BookDto> SearchBooksByTitleOrAuthor(string searchQuery)
        {
            var books = new List<BookDto>();
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();
                string sql = @"
                    SELECT BookId, Title, Author, Genre, ISBN
                    FROM Books
                    WHERE Title LIKE @SearchQuery OR Author LIKE @SearchQuery";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var book = new BookDto
                            {
                                BookId = reader.GetInt32(reader.GetOrdinal("BookId")), // Assuming this is not nullable
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                                Author = reader.IsDBNull(reader.GetOrdinal("Author")) ? null : reader.GetString(reader.GetOrdinal("Author")),
                                Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? null : reader.GetString(reader.GetOrdinal("Genre")),
                                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                            };
                            books.Add(book);
                        }
                    }
                }
            }
            return books;
        }
        public int Update(Books book)
        {
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();

                string sql = $"UPDATE [Books] SET " +
                    $"Title=@title, Author=@author, Genre=@genre, ISBN=@isbn," +
                    $" TotalCopies=@totalCopies, AvailableCopies=@availableCopies" +
                    $" WHERE BookId=@bookId";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Set command parameters dynamically
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@bookId", book.BookId);
                    cmd.Parameters.AddWithValue("@title", book.Title);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@genre", book.Genre);
                    cmd.Parameters.AddWithValue("@isbn", book.ISBN);
                    cmd.Parameters.AddWithValue("@totalCopies", book.TotalCopies);
                    cmd.Parameters.AddWithValue("@availableCopies", book.AvailableCopies);
                    
                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    return result;
                }

            }
        }
    }
}
