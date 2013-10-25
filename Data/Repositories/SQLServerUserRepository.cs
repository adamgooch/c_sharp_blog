using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Application;
using Application.Users;
using System.Linq;

namespace Data.Repositories
{
    public class SqlServerUserRepository : IUserRepository
    {
        private static readonly string connection = ConfigurationManager.ConnectionStrings["blog"].ConnectionString;

        private const string UserTable = "Users";
        private const string UserId = "id";
        private const string UserEmail = "email";
        private const string UserPassDigest = "pass_digest";
        private const string UserSalt = "salt";
        private const string UserVerifyToken = "verify_token";
        private const string UserActive = "active";
        private const string UserCreatedDate = "created_date";
        private const string UserModifiedDate = "modified_date";
        private const string UserResetPassToken = "reset_pass_token";

        private const string RoleTable = "Roles";
        private const string RoleId = "id";
        private const string RoleName = "name";
        private const string RoleCreatedDate = "created_date";
        private const string RoleModifiedDate = "modified_date";

        private const string UserRoleTable = "UserRoles";
        private const string UserRoleId = "id";
        private const string UserRoleUserId = "user_id";
        private const string UserRoleRoleId = "role_id";

        public bool CreateUser( string email, byte[] passwordDigest, byte[] salt, Guid verificationToken )
        {
            var userId = Guid.NewGuid();
            var addUserCommand = GetAddUserCommand( userId, email, passwordDigest, salt, verificationToken );
            var addRoleCommand = GetAddRoleCommand( userId, 0 );
            return ( ExecuteNonQuery( addUserCommand ) && ExecuteNonQuery( addRoleCommand ) );
        }

        public bool DeleteUser( Guid id )
        {
            var deleteUserRolesCommand = new SqlCommand();
            deleteUserRolesCommand.CommandText = String.Format( "DELETE FROM {0} WHERE {1} = @P1", UserRoleTable, UserRoleUserId );
            deleteUserRolesCommand.Parameters.AddWithValue( "@P1", id );

            var deleteUserCommand = new SqlCommand();
            deleteUserCommand.CommandText = String.Format( "DELETE FROM {0} WHERE {1} = @P1", UserTable, UserId );
            deleteUserCommand.Parameters.AddWithValue( "@P1", id );

            return ( ExecuteNonQuery( deleteUserRolesCommand ) && ExecuteNonQuery( deleteUserCommand ) );
        }

        public bool ActivateUser( Guid token )
        {
            var sqlCommand = new SqlCommand();
            sqlCommand.CommandText = String.Format( "UPDATE {0} SET {1}='{2}', {3}='{4}' WHERE {5} = @P1", UserTable, UserActive, 1, UserModifiedDate, DateTime.Now, UserVerifyToken );
            sqlCommand.Parameters.AddWithValue( "@P1", token );
            return ExecuteNonQuery( sqlCommand );
        }

        public bool SetActive( Guid userId, bool active )
        {
            var sqlCommand = new SqlCommand();
            sqlCommand.CommandText = String.Format( "UPDATE {0} SET {1}='{2}', {3}='{4}' WHERE {5} = @P1", UserTable, UserActive, active, UserModifiedDate, DateTime.Now, UserId );
            sqlCommand.Parameters.AddWithValue( "@P1", userId );
            return ExecuteNonQuery( sqlCommand );
        }

        public bool AddRole( Guid userId, string role )
        {
            var roleId = GetRoleId( role );
            if( UserHasRole( userId, role ) ) return false;
            var addRoleCommand = GetAddRoleCommand( userId, roleId );
            return ExecuteNonQuery( addRoleCommand );
        }

        public bool RemoveRole( Guid userId, string role )
        {
            var roleId = GetRoleId( role );
            if( !UserHasRole( userId, role ) ) return false;
            var removeRoleCommand = GetRemoveRoleCommand( userId, roleId );
            return ExecuteNonQuery( removeRoleCommand );
        }

        public bool SetUserResetPasswordToken( string email, byte[] resetToken )
        {
            var sqlCommand = new SqlCommand();
            sqlCommand.CommandText = String.Format( "UPDATE {0} SET {1} = @P1 WHERE {2} = @P2", UserTable, UserResetPassToken, UserEmail );

            var saltParam = sqlCommand.Parameters.Add( "@P1", SqlDbType.VarBinary, resetToken.Length );
            saltParam.Value = resetToken;
            sqlCommand.Parameters.AddWithValue( "@P2", email );
            return ExecuteNonQuery( sqlCommand );
        }

        public bool ResetPassword( byte[] passwordDigest, byte[] salt, byte[] token )
        {
            var sqlCommand = new SqlCommand();
            sqlCommand.CommandText = String.Format( "UPDATE {0} SET {1} = @P1, {2} = @P2, {3} = NULL WHERE {4} = @P3",
              UserTable, UserPassDigest, UserSalt, UserResetPassToken, UserResetPassToken );

            var passDigestParam = sqlCommand.Parameters.Add( "@P1", SqlDbType.VarBinary, passwordDigest.Length );
            var saltParam = sqlCommand.Parameters.Add( "@P2", SqlDbType.VarBinary, salt.Length );
            saltParam.Value = salt;
            passDigestParam.Value = passwordDigest;
            sqlCommand.Parameters.AddWithValue( "@P3", token );
            return ExecuteNonQuery( sqlCommand );
        }

        public User GetUser( string email )
        {
            var user = new User();
            using( var conn = new SqlConnection( connection ) )
            {
                var sqlCommand = new SqlCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.CommandText = String.Format( "SELECT * FROM {0} WHERE {1} = @P1", UserTable, UserEmail );

                sqlCommand.Parameters.AddWithValue( "@P1", email );
                sqlCommand.Connection = conn;
                conn.Open();
                var reader = sqlCommand.ExecuteReader();
                user = MapToUser( reader ).FirstOrDefault();
                conn.Close();
            }
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var allUsers = new List<User>();
            using( var conn = new SqlConnection( connection ) )
            {
                var sqlCommand = new SqlCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.CommandText = String.Format( "SELECT * FROM {0}", UserTable );

                sqlCommand.Connection = conn;
                conn.Open();
                var reader = sqlCommand.ExecuteReader();
                allUsers = MapToUser( reader ).ToList();
                conn.Close();
            }
            return allUsers;
        }

        public IEnumerable<string> GetAllRoles()
        {
            var allRoles = new List<string>();
            using( var conn = new SqlConnection( connection ) )
            {
                var sqlCommand = new SqlCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.CommandText = String.Format( "SELECT {0} FROM {1}", RoleName, RoleTable );

                sqlCommand.Connection = conn;
                conn.Open();
                var reader = sqlCommand.ExecuteReader();
                allRoles = GetRoleNames( reader ).ToList();
                conn.Close();
            }
            return allRoles;
        }

        private IEnumerable<User> MapToUser( SqlDataReader reader )
        {
            var users = new List<User>();
            while( reader.Read() )
            {
                var user = new User
                {
                    Id = (Guid)reader[UserId],
                    Email = (string)reader[UserEmail],
                    PasswordDigest = (byte[])reader[UserPassDigest],
                    Salt = (byte[])reader[UserSalt],
                    Active = (bool)reader[UserActive]
                };
                user.Roles = GetUserRoles( user.Id );
                users.Add( user );
            }
            return users;
        }

        private IEnumerable<string> GetUserRoles( Guid userId )
        {
            var roles = new List<string>();
            using( var conn = new SqlConnection( connection ) )
            {
                var sqlCommand = new SqlCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.CommandText = String.Format( "SELECT {0} FROM {1} WHERE {2} = @P1", UserRoleRoleId, UserRoleTable, UserRoleUserId );
                sqlCommand.Parameters.AddWithValue( "@P1", userId );
                sqlCommand.Connection = conn;
                conn.Open();
                var reader = sqlCommand.ExecuteReader();
                roles = GetRoles( reader ).ToList();
                conn.Close();
            }
            return roles;
        }

        private IEnumerable<string> GetRoles( SqlDataReader userRoleReader )
        {
            var roles = new List<string>();
            while( userRoleReader.Read() )
            {
                using( var conn = new SqlConnection( connection ) )
                {
                    var sqlCommand = new SqlCommand();
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandText = String.Format( "SELECT {0} FROM {1} WHERE {2} = @P1", RoleName, RoleTable, RoleId );
                    sqlCommand.Parameters.AddWithValue( "@P1", (int)userRoleReader[UserRoleRoleId] );
                    sqlCommand.Connection = conn;
                    conn.Open();
                    var roleNameReader = sqlCommand.ExecuteReader();
                    roles.AddRange( GetRoleNames( roleNameReader ) );
                    conn.Close();
                }
            }
            return roles;
        }

        private IEnumerable<string> GetRoleNames( SqlDataReader reader )
        {
            var roleNames = new List<string>();
            while( reader.Read() )
            {
                roleNames.Add( (string)reader[RoleName] );
            }
            return roleNames;
        }

        private int GetRoleId( string role )
        {
            int roleId = 0;
            using( var conn = new SqlConnection( connection ) )
            {
                var sqlCommand = new SqlCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.CommandText = String.Format( "SELECT {0} FROM {1} WHERE {2} = @P1", RoleId, RoleTable, RoleName );
                sqlCommand.Parameters.AddWithValue( "@P1", role );
                sqlCommand.Connection = conn;
                conn.Open();
                var roleIdReader = sqlCommand.ExecuteReader();
                roleIdReader.Read();
                roleId = (int)roleIdReader[RoleId];
                conn.Close();
            }
            return roleId;
        }

        private bool UserHasRole( Guid userId, string roleCandidate )
        {
            var userRoles = GetUserRoles( userId );
            foreach( string role in userRoles )
            {
                if( role.Equals( roleCandidate ) ) return true;
            }
            return false;
        }

        private SqlCommand GetAddRoleCommand( Guid userId, int role )
        {
            var addRoleCommand = new SqlCommand();
            addRoleCommand.CommandText = String.Format( "INSERT {0} ( {1}, {2} ) VALUES ( @P1, @P2 )",
              UserRoleTable, UserRoleUserId, UserRoleRoleId );
            addRoleCommand.Parameters.AddWithValue( "@P1", userId );
            addRoleCommand.Parameters.AddWithValue( "@P2", role );
            return addRoleCommand;
        }

        private SqlCommand GetRemoveRoleCommand( Guid userId, int role )
        {
            var removeRoleCommand = new SqlCommand();
            removeRoleCommand.CommandText = String.Format( "DELETE FROM {0} WHERE {1} = @P1 AND {2} = @P2",
              UserRoleTable, UserRoleUserId, UserRoleRoleId );
            removeRoleCommand.Parameters.AddWithValue( "@P1", userId );
            removeRoleCommand.Parameters.AddWithValue( "@P2", role );
            return removeRoleCommand;
        }

        private SqlCommand GetAddUserCommand( Guid id, string email, byte[] passwordDigest, byte[] salt, Guid verificationToken )
        {
            var addUserCommand = new SqlCommand();
            addUserCommand.CommandText = String.Format( "INSERT {0} ( {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} ) VALUES ( @P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8 )",
              UserTable, UserId, UserEmail, UserPassDigest, UserSalt, UserVerifyToken, UserActive, UserCreatedDate, UserModifiedDate );

            addUserCommand.Parameters.AddWithValue( "@P1", id );
            addUserCommand.Parameters.AddWithValue( "@P2", email );
            var passDigestParam = addUserCommand.Parameters.Add( "@P3", SqlDbType.VarBinary, passwordDigest.Length );
            passDigestParam.Value = passwordDigest;
            var saltParam = addUserCommand.Parameters.Add( "@P4", SqlDbType.VarBinary, salt.Length );
            saltParam.Value = salt;
            addUserCommand.Parameters.AddWithValue( "@P5", verificationToken );
            addUserCommand.Parameters.AddWithValue( "@P6", 0 );
            addUserCommand.Parameters.AddWithValue( "@P7", DateTime.Now );
            addUserCommand.Parameters.AddWithValue( "@P8", DateTime.Now );
            return addUserCommand;
        }

        private bool ExecuteNonQuery( SqlCommand command )
        {
            try
            {
                var rowsAffected = 0;
                using( var conn = new SqlConnection( connection ) )
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Connection = conn;
                    conn.Open();
                    rowsAffected = command.ExecuteNonQuery();
                    conn.Close();
                }
                return rowsAffected > 0 ? true : false;
            }
            catch
            {
                return false;
            }
        }
    }
}
