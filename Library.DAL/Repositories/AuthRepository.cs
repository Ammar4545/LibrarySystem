using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.DAL.Setting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ConnectionSetting _connection;

        public AuthRepository(IOptions<ConnectionSetting> connection)
        {
            _connection = connection.Value;
        }
        public Users CheckUser(string userName, string password)
        {
            Users authenticatedUser = new Users();

            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();
                string sql = "select * from [Users] where UserName=@userName and Password=@password";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@password", password);
    
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        authenticatedUser.UserId = Convert.ToInt32(reader["UserId"]);
                        authenticatedUser.UserName = reader["UserName"].ToString();
                    }
                    conn.Close();

                    return authenticatedUser;
                }
            }
        }

        public bool CheckUserExists(string userName, string password)
        {
            bool flag = false;
            var user = CheckUser(userName, password);
            if (user.UserName != null)
            {
                flag = true;
                return flag;
            }

            return flag;
        }
    }
}
