using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.DAL.Setting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Library.DAL.DTOs;

namespace Library.DAL.Repositories
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        private readonly ConnectionSetting _connection;
        public UserRepository(IOptions<ConnectionSetting> connection) : base(connection)
        {
            _connection = connection.Value;
        }
        public int Update(Users user)
        {
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();

                string sql = $"UPDATE [Users] SET " +
                    $"UserName=@userName, Email=@email, Password=@password, Address=@address, Phone=@phone " +
                    $"WHERE UserId=@userId";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Set command parameters dynamically
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@userId", user.UserId);
                    cmd.Parameters.AddWithValue("@userName", user.UserName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@address", user.Address);
                    cmd.Parameters.AddWithValue("@phone", user.Phone);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    return result;
                }

            }
        }
    }
}
