using Application.Posts.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Application.Posts.Entities;

namespace Data.Repositories
{
    public class SQLServerPostRepository : IPostRepository
    {
        private static readonly string tableName = "Posts";
        private readonly string connection = ConfigurationManager.ConnectionStrings["testBlog"].ConnectionString;

        public SQLServerPostRepository()
        {
        }

        public void CreatePost( Post post )
        {
            var sql = string.Format( "INSERT INTO dbo.{0} (Id, CreatedDateTime, ModifiedDateTime, Title, Body, Tags, Author) " +
                "VALUES ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", tableName, Guid.NewGuid(),
                post.Date, post.Date, post.Title, post.Body, post.Tags[0], post.Author );
            SendCommand( sql );
        }

        public IEnumerable<Post> GetAllPosts( string author )
        {
            var posts = new List<Post>();
            using( SqlConnection conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    SqlCommand sqlCommand = new SqlCommand( "dbo.GetAllPostsByAuthor", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add( "@Author", SqlDbType.NVarChar ).Value = author;
                    posts = (List<Post>)MapToPost( sqlCommand.ExecuteReader() );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
            return posts;
        }

        public IEnumerable<Post> GetAllPosts()
        {

            var posts = new List<Post>();
            using( SqlConnection conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    SqlCommand sqlCommand = new SqlCommand( "dbo.GetAllPosts", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    posts = (List<Post>)MapToPost( sqlCommand.ExecuteReader() );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
            return posts;
        }

        private IEnumerable<Post> MapToPost( SqlDataReader reader )
        {
            var posts = new List<Post>();
            while( reader.Read() )
            {
                var post = new Post
                {
                    Title = reader["Title"].ToString(),
                    Body = reader["Body"].ToString(),
                    Author = reader["Author"].ToString(),
                    Date = (DateTime)reader["CreatedDateTime"],
                    Tags = new string[] { reader["Tags"].ToString() }
                };
                posts.Add( post );
            }
            return posts;
        }

        public void DeletePost( string author, DateTime date, string title )
        {
            using( SqlConnection conn = new SqlConnection( connection ) )
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand( "dbo.DeletePostByAuthorDateTitle", conn );
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add( "@Author", SqlDbType.NVarChar ).Value = author;
                    sqlCommand.Parameters.Add( "@Date", SqlDbType.Date ).Value = date.ToString();
                    sqlCommand.Parameters.Add( "@Title", SqlDbType.NVarChar ).Value = title;
                    conn.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }

        private int SendCommand( string command )
        {
            var rowsAffected = 0;
            using( SqlConnection db = new SqlConnection( connection ) )
            {
                try
                {
                    db.Open();
                    SqlCommand myCommand = new SqlCommand( command, db );
                    rowsAffected = myCommand.ExecuteNonQuery();
                    db.Close();
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
            return rowsAffected;
        }
    }
}
