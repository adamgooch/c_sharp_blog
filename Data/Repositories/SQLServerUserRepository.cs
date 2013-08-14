using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Application;
using Application.Users;

namespace Data.Repositories
{
    public class SqlServerUserRepository : IUserRepository
    {
        private readonly string connection = ConfigurationManager.ConnectionStrings["blog"].ConnectionString;

        public void CreateUser( User user )
        {
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
                    sqlCommand.Parameters.Add( "@CreatedDateTime", SqlDbType.DateTime ).Value = DateTime.Now;
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
            using( var conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    var sqlCommand = new SqlCommand( "dbo.GetAllUsers", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    users = (List<User>) MapToUsers( sqlCommand.ExecuteReader() );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
            return users;
        }

        public void SaveUser( User user )
        {
            using( var conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    var sqlCommand = new SqlCommand( "dbo.EditUser", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add( "@Id", SqlDbType.UniqueIdentifier ).Value = user.Id;
                    sqlCommand.Parameters.Add( "@Email", SqlDbType.NVarChar ).Value = user.Email;
                    sqlCommand.Parameters.Add( "@Salt", SqlDbType.VarBinary ).Value = user.Salt;
                    sqlCommand.Parameters.Add( "@PasswordDigest", SqlDbType.VarBinary ).Value = user.PasswordDigest;
                    sqlCommand.Parameters.Add( "@ModifiedDateTime", SqlDbType.DateTime ).Value = DateTime.Now;
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

        public void DeleteByEmail( string email )
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

        public void DeleteById( Guid id )
        {
            using( var conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    var sqlCommand = new SqlCommand( "dbo.DeleteUserById", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add( "@Id", SqlDbType.UniqueIdentifier ).Value = id;
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
                    Id = new Guid( reader["Id"].ToString() ),
                    Email = reader["Email"].ToString(),
                    CreatedDate = (DateTime) reader["CreatedDateTime"],
                    PasswordDigest = (byte[]) reader["PasswordDigest"],
                    Salt = (byte[]) reader["Salt"],
                    VerifiedToken = new Guid( reader["VerifiedToken"].ToString() ),
                    Role = (Role) reader["UserRole"]
                };
                users.Add( user );
            }
            return users;
        }
    }
}
