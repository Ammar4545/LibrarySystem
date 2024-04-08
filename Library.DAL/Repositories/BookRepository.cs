using Library.DAL.DTOs;
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
        //public bool BorrowBook(int userId,int bookId)
        //{

        //    using (var connection = new SqlConnection(_connection.SQLString))
        //    {
        //        connection.Open();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            using (var command = connection.CreateCommand())
        //            {
        //                command.Transaction = transaction;

        //                try
        //                {
        //                    // Check if there are available copies
        //                    command.CommandText = "SELECT AvailableCopies FROM Books WHERE BookId = @BookId";
        //                    command.Parameters.AddWithValue("@BookId", bookId);

        //                    int availableCopies = (int)command.ExecuteScalar();

        //                    if (availableCopies > 0)
        //                    {
        //                        // Insert a record into Borrowings
        //                        command.CommandText = "INSERT INTO Borrowings (UserId, BookId, BorrowDate) VALUES (@UserId, @BookId, GETDATE())";
        //                        command.Parameters.AddWithValue("@UserId", userId);

        //                        command.ExecuteNonQuery();

        //                        // Decrease AvailableCopies by 1
        //                        command.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookId = @BookId";

        //                        command.ExecuteNonQuery();

        //                        transaction.Commit();
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        transaction.Rollback();
        //                        return false; // No copies available
        //                    }
        //                }
        //                catch
        //                {
        //                    transaction.Rollback();
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //}
        //public bool ReturnBook(int userId, int bookId)
        //{

        //    using (var connection = new SqlConnection(_connection.SQLString))
        //    {
        //        connection.Open();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            using (var command = connection.CreateCommand())
        //            {
        //                command.Transaction = transaction;

        //                try
        //                {
        //                    // Update Borrowings to set ReturnDate for the book
        //                    command.CommandText = "UPDATE Borrowings SET ReturnDate " +
        //                        "= GETDATE() WHERE UserId = @UserId AND BookId = @BookId AND ReturnDate IS NULL";
        //                    command.Parameters.AddWithValue("@UserId", userId);
        //                    command.Parameters.AddWithValue("@BookId", bookId);

        //                    int rowsAffected = command.ExecuteNonQuery();

        //                    if (rowsAffected > 0)
        //                    {
        //                        // Increase AvailableCopies by 1
        //                        command.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookId = @BookId";

        //                        command.ExecuteNonQuery();

        //                        transaction.Commit();
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        transaction.Rollback();
        //                        return false; // No matching borrowing record found
        //                    }
        //                }
        //                catch
        //                {
        //                    transaction.Rollback();
        //                    throw;
        //                }
        //            }
        //        }
        //    }

        //}
        public bool BorrowBook(int userId, int bookId)
        {
            using (var connection = new SqlConnection(_connection.SQLString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            // Check if there are available copies and insert a record into Borrowings
                            command.CommandText = @"
                        IF EXISTS (SELECT 1 FROM Books WHERE BookId = @BookId AND AvailableCopies > 0)
                        BEGIN
                            INSERT INTO Borrowings (UserId, BookId, BorrowDate)
                            VALUES (@UserId, @BookId, GETDATE());

                            UPDATE Books
                            SET AvailableCopies = AvailableCopies - 1
                            WHERE BookId = @BookId;

                            SELECT 1; -- Indicate success
                        END
                        ELSE
                        BEGIN
                            SELECT 0; -- Indicate failure
                        END";
                            command.Parameters.AddWithValue("@BookId", bookId);
                            command.Parameters.AddWithValue("@UserId", userId);

                            // Execute the command and get the result
                            int result = (int)command.ExecuteScalar();

                            // Commit or rollback transaction based on the result
                            if (result == 1)
                            {
                                transaction.Commit();
                                return true; // Borrowing successful
                            }
                            else
                            {
                                transaction.Rollback();
                                return false; // No copies available
                            }
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
        //public bool ReturnBook(int userId, int bookId)
        //{

        //    using (var connection = new SqlConnection(_connection.SQLString))
        //    {
        //        connection.Open();
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            using (var command = connection.CreateCommand())
        //            {
        //                command.Transaction = transaction;

        //                try
        //                {
        //                    // Update Borrowings to set ReturnDate for the book
        //                    command.CommandText = "UPDATE Borrowings SET ReturnDate " +
        //                        "= GETDATE() WHERE UserId = @UserId AND BookId = @BookId AND ReturnDate IS NULL";
        //                    command.Parameters.AddWithValue("@UserId", userId);
        //                    command.Parameters.AddWithValue("@BookId", bookId);

        //                    int rowsAffected = command.ExecuteNonQuery();

        //                    if (rowsAffected > 0)
        //                    {
        //                        // Increase AvailableCopies by 1
        //                        command.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookId = @BookId";

        //                        command.ExecuteNonQuery();

        //                        transaction.Commit();
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        transaction.Rollback();
        //                        return false; // No matching borrowing record found
        //                    }
        //                }
        //                catch
        //                {
        //                    transaction.Rollback();
        //                    throw;
        //                }
        //            }
        //        }
        //    }

        //}
        public bool ReturnBook(int userId, int bookId)
        {
            using (var connection = new SqlConnection(_connection.SQLString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            // Update the first matching row in Borrowings to set ReturnDate for the book
                            command.CommandText = @"
                                 UPDATE TOP (1) Borrowings
                                 SET ReturnDate = GETDATE()
                                 WHERE UserId = @UserId AND BookId = @BookId AND ReturnDate IS NULL;
                                 
                                 IF @@ROWCOUNT > 0
                                 BEGIN
                                     -- Increase AvailableCopies by 1
                                     UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookId = @BookId;
                                     SELECT 1; -- Indicate success
                                 END
                                 ELSE
                                 BEGIN
                                     SELECT 0; -- Indicate failure
                                 END";
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@BookId", bookId);

                            int result = (int)command.ExecuteScalar();

                            if (result == 1)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false; // No matching borrowing record found or book already returned
                            }
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
        public List<Books> GetBorrowedBooksByUser(int userId)
        {
            List<Books> books = new List<Books>();

            using (var connection = new SqlConnection(_connection.SQLString))
            {
                connection.Open();

                string sql = @"
                    SELECT Books.*
                    FROM Books
                    INNER JOIN Borrowings ON Books.BookId = Borrowings.BookId
                    WHERE Borrowings.UserId = @UserId and Borrowings.ReturnDate IS NULL ";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Populate Book object from reader and add it to the list
                            Books book = new Books
                            {
                                BookId = Convert.ToInt32(reader["BookId"]),
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                Genre = reader["Genre"].ToString(),
                                ISBN = reader["ISBN"].ToString(),
                                Image = reader["Image"].ToString(),
                                // Populate other properties as needed
                            };

                            books.Add(book);
                        }
                    }
                }
            }

            return books;
        }
        public List<Books> SearchBooksByTitleOrAuthor(string searchQuery)
            {
            var books = new List<Books>();
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();
                string sql = @"
                    SELECT BookId, Title, Author, Genre, ISBN, Image
                    FROM Books
                    WHERE Title LIKE @SearchQuery OR Author LIKE @SearchQuery";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchQuery", $"%{searchQuery}%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var book = new Books
                            {
                                BookId = reader.GetInt32(reader.GetOrdinal("BookId")), // Assuming this is not nullable
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                                Author = reader.IsDBNull(reader.GetOrdinal("Author")) ? null : reader.GetString(reader.GetOrdinal("Author")),
                                Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? null : reader.GetString(reader.GetOrdinal("Genre")),
                                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image")),
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
