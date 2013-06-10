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
        public void it_creates_a_post()
        {
            var testPost = CreateTestPost( authorNumber );
            sut.CreatePost( testPost );
            var posts = sut.GetAllPosts();
            Assert.AreEqual( 1, posts.Count(), "Wrong number of posts" );
            Assert.AreEqual( testPost.title, posts.First().title, "Wrong title" );
            Assert.AreEqual( testPost.body, posts.First().body, "Wrong body" );
            Assert.AreEqual( testPost.author, posts.First().author, "Wrong author" );
            Assert.AreEqual( testPost.date, posts.First().date, "Wrong date" );
            Assert.AreEqual( testPost.tags, posts.First().tags, "Wrong tags" );
            sut.DeletePost( testAuthor + authorNumber, testDate, testTitle + postNumber );
            Assert.AreEqual( 0, sut.GetAllPosts().Count(), "Post was not deleted" );
        }

        [Test]
        public void it_gets_all_posts_by_author()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.CreatePost( CreateTestPost( ++authorNumber ) );
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var result = sut.GetAllPosts( testAuthor + authorNumber );
            Assert.AreEqual( 2, result.Count(), "Wrong number of posts returned" );
            Assert.AreEqual( testAuthor + authorNumber, result.First().author, "Wrong author" );
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
