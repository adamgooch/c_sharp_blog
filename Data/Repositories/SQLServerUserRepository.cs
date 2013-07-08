using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Application.Users;

namespace Data.Repositories
{
    public class SQLServerUserRepository
    {
        private readonly string connection = ConfigurationManager.ConnectionStrings["testBlog"].ConnectionString;

        public void CreateUser( User user )
        {
            var id = Guid.Empty;
            using( var conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    var sqlCommand = new SqlCommand( "dbo.CreateUser", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add( "@Email", SqlDbType.NVarChar ).Value = user.Email;
                    sqlCommand.Parameters.Add( "@Salt", SqlDbType.VarBinary ).Value = user.Salt;
                    sqlCommand.Parameters.Add( "@PasswordDigest", SqlDbType.VarBinary ).Value = user.PasswordDigest;
                    sqlCommand.Parameters.Add( "@CreatedDateTime", SqlDbType.DateTime ).Value = user.CreatedDate;
                    sqlCommand.Parameters.Add( "@Role", SqlDbType.Int ).Value = user.Role;
                    sqlCommand.Parameters.Add( "@VerifiedToken", SqlDbType.UniqueIdentifier ).Value = user.VerifiedToken;
                    sqlCommand.ExecuteNonQuery();
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    var sqlCommand = new SqlCommand("dbo.GetAllUsers", conn);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    users = (List<User>)MapToUsers(sqlCommand.ExecuteReader());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return users;
        }

        public void DeleteByEmail(string email)
        {
            using( var conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    var sqlCommand = new SqlCommand( "dbo.DeleteUserByEmail", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add( "@Email", SqlDbType.NVarChar ).Value = email;
                    sqlCommand.ExecuteNonQuery();
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }

        private IEnumerable<User> MapToUsers( SqlDataReader reader )
        {
            var users = new List<User>();
            while( reader.Read() )
            {
                var user = new User
                {
                    Email = reader["Email"].ToString(),
                    CreatedDate = (DateTime)reader["CreatedDateTime"],
                    PasswordDigest = (byte[])reader["PasswordDigest"],
                    Salt = (byte[])reader["Salt"],
                    VerifiedToken = new Guid( reader["VerifiedToken"].ToString() ),
                    Role = (int)reader["UserRole"]
                };
                users.Add( user );
            }
            return users;
        }
    }
}
