using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Data.Repositories;
using Application.Posts.Entities;
using System.Configuration;

namespace Tests.Data
{
    [TestFixture]
    class SQLServerPostRepositoryTest
    {
        private int postNumber = 0;
        private int authorNumber = 1;
        private readonly string testAuthor = "Test Author";
        private readonly DateTime testDate = DateTime.Parse( "April 10, 2013" );
        private readonly string testTitle = "Test... Post ";
        private readonly string testBody = "Lorem ipsum dolor sit amet";
        private readonly string[] testTag = new string[] { "Test Tag" };
        private readonly string testPost = "2013_04_10_Test..._Post_";
        private static readonly string tableName = "Posts";
        private readonly string connection = ConfigurationManager.ConnectionStrings["testBlog"].ConnectionString;

        private SQLServerPostRepository sut;

        [SetUp]
        public void Setup()
        {
            sut = new SQLServerPostRepository();
        }

        [TearDown]
        public void Teardown()
        {
            var sql = "DELETE FROM [dbo].[Posts]";
            SendCommand( sql );
        }

        [Test]
        public void it_creates_a_post_with_the_correct_name()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var posts = sut.GetAllPosts();
            Assert.AreEqual( 1, posts.Count() );
        }

        private Post CreateTestPost( int localAuthorNumber )
        {
            ++postNumber;
            var post = new Post
            {
                author = testAuthor + localAuthorNumber,
                date = testDate,
                title = testTitle + postNumber,
                body = testBody,
                tags = testTag
            };
            return post;
        }

        private void SendCommand( string command )
        {
            using( SqlConnection db = new SqlConnection( connection ) )
            {
                try
                {
                    db.Open();
                    SqlCommand myCommand = new SqlCommand( command, db );
                    myCommand.ExecuteNonQuery();
                    db.Close();
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }
    }
}
