using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.DAL.Setting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Library.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ConnectionSetting _connection;
        public GenericRepository(IOptions<ConnectionSetting> connection)
        {
            _connection = connection.Value;
        }
        public IEnumerable<T> GetAll(string tableName)
        {
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();

                string sql = $"select * from {tableName}";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<T> entities = new List<T>();
                    while (reader.Read())
                    {
                        // Map reader to entity type T
                        T entity = MapReaderToObject<T>(reader);
                        entities.Add(entity);
                    }

                    return entities;
                }
            }

        }
        public T GetById(string tableName, int? id)
        {
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();
                string identityColumnName = GetIdentityColumnName<T>();

                if (identityColumnName == null)
                {
                    throw new InvalidOperationException("Identity column not found.");
                }
                string sql = $"select * from {tableName} where {identityColumnName}=@id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {

                        T entity = Activator.CreateInstance<T>();
                        entity = MapReaderToObject<T>(reader);
                        return entity;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }
        public int Add(T entity, string tableName)
        {

            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();

                string sql = $"INSERT INTO [{tableName}] ({GetColumnNames(entity)}) " +
                             $"VALUES ({GetParameterNames(entity)})";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Set command parameters dynamically
                    SetParameters(cmd, entity);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    return result;
                }

            }
        }
        public bool DeleteRecord(string tableName, int id)
        {
            if (CanDelete(tableName, id))
            {
                // Proceed with deletion
                Delete(tableName, id);
                return true;
            }
            else
            {
                // Inform that the record cannot be deleted due to outstanding borrowings
                return false;
            }
        }



        #region PrivateMethod
        private bool CanDelete(string tableName, int id)
        {
            string columnName = tableName == "Users" ? "UserId" : "BookId";
            int outstandingCount = 0;

            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();

                // Query to count outstanding borrowings (without a ReturnedDate) for the given record
                string checkSql = $@"
                    SELECT COUNT(*) 
                    FROM [Borrowings] 
                    WHERE [{columnName}] = @id AND [ReturnDate] IS NULL";

                using (SqlCommand cmd = new SqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    outstandingCount = (int)cmd.ExecuteScalar();
                }
            }

            // The record can be deleted if there are no outstanding borrowings
            return outstandingCount == 0;
        }
        private void Delete(string tableName, int id)
        {
            using (SqlConnection conn = new SqlConnection(_connection.SQLString))
            {
                conn.Open();

                string identityColumnName = GetIdentityColumnName<T>();
                if (identityColumnName == null)
                {
                    throw new InvalidOperationException("Identity column not found.");
                }

                string sql = $"DELETE from [{tableName}] WHERE {identityColumnName}=@id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.Parameters.AddWithValue("@id", id);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    return;
                }

            }
        }
        
        private T MapReaderToObject<T>(SqlDataReader reader)
        {
            // Create an instance of type T using reflection
            T entity = Activator.CreateInstance<T>();

            // Get the properties of type T
            var properties = typeof(T).GetProperties();

            // Iterate through the columns returned by the SqlDataReader
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // Get the name of the column
                string columnName = reader.GetName(i);

                // Find the property in type T that matches the column name
                var property = properties.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (property != null && reader[columnName] != DBNull.Value)
                {
                    // Set the value of the property in the entity
                    property.SetValue(entity, Convert.ChangeType(reader[columnName], property.PropertyType));
                }
            }

            return entity;
        }
        private string GetColumnNames(T entity)
        {
            var properties = typeof(T).GetProperties().Where(p => !IsIdentityColumn(p)); // Exclude identity column
            return string.Join(",", properties.Select(p => $"[{p.Name}]"));
        }
        private bool IsIdentityColumn(PropertyInfo property)
        {
            // Check if the property has the 'Identity' attribute
            return property.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute" &&
                ((DatabaseGeneratedAttribute)attr).DatabaseGeneratedOption == DatabaseGeneratedOption.Identity);
        }
        private string GetParameterNames(T entity)
        {
            var properties = typeof(T).GetProperties().Where(p => !IsIdentityColumn(p)); // Exclude identity column
            return string.Join(",", properties.Select(p => $"@{p.Name}"));
        }
        private void SetParameters(SqlCommand cmd, T entity)
        {
            var properties = typeof(T).GetProperties().Where(p => !IsIdentityColumn(p)); // Exclude identity column
            foreach (var prop in properties)
            {
                cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(entity) ?? DBNull.Value);
            }
        }
        private string GetIdentityColumnName<T>()
        {
            Type type = typeof(T);
            // Get the properties of the entity type
            var properties = type.GetProperties();
            // Iterate through the properties to find the identity column
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true)
                                         .FirstOrDefault() as DatabaseGeneratedAttribute;
                if (attribute != null && attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                {
                    return property.Name;
                }
            }
            // If no identity column is found, return null or throw an exception as per your requirement
            return null;
        }

       
        #endregion
    }
}
