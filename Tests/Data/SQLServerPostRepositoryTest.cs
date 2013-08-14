using NUnit.Framework;
using System;
using System.Linq;
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
        private const string TestAuthor = "Test Author";
        private readonly DateTime testDate = DateTime.Parse( "April 10, 2013" );
        private const string TestTitle = "Test... Post ";
        private const string TestBody = "Lorem ipsum dolor sit amet";
        private readonly string[] testTag = new string[] { "Test Tag" };
        private readonly string connection = ConfigurationManager.ConnectionStrings["blog"].ConnectionString;

        private SqlServerPostRepository sut;

        [SetUp]
        public void Setup()
        {
            sut = new SqlServerPostRepository();
        }

        [TearDown]
        public void Teardown()
        {
            SendCommand( "DELETE FROM [dbo].[Posts]" );
        }

        [Test]
        public void it_creates_and_deletes_a_post()
        {
            var testPost = CreateTestPost( authorNumber );
            sut.CreatePost( testPost );
            var posts = sut.GetAllPosts();
            Assert.AreEqual( 1, posts.Count(), "Wrong number of posts" );
            Assert.AreEqual( testPost.Title, posts.First().Title, "Wrong title" );
            Assert.AreEqual( testPost.Body, posts.First().Body, "Wrong body" );
            Assert.AreEqual( testPost.Author, posts.First().Author, "Wrong author" );
            Assert.AreEqual( testPost.Date, posts.First().Date, "Wrong date" );
            Assert.AreEqual( testPost.Tags, posts.First().Tags, "Wrong tags" );
            sut.DeletePost( TestAuthor + authorNumber, testDate, TestTitle + postNumber );
            Assert.AreEqual( 0, sut.GetAllPosts().Count(), "Post was not deleted" );
        }

        [Test]
        public void it_gets_all_posts_by_author()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.CreatePost( CreateTestPost( ++authorNumber ) );
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var result = sut.GetAllPosts( TestAuthor + authorNumber );
            Assert.AreEqual( 2, result.Count(), "Wrong number of posts returned" );
            Assert.AreEqual( TestAuthor + authorNumber, result.First().Author, "Wrong author" );
        }

        private Post CreateTestPost( int localAuthorNumber )
        {
            ++postNumber;
            var post = new Post
            {
                Author = TestAuthor + localAuthorNumber,
                Date = testDate,
                Title = TestTitle + postNumber,
                Body = TestBody,
                Tags = testTag
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
