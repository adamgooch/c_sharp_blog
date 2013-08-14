using System;
using System.IO;
using NUnit.Framework;
using Data.Repositories;
using Application.Posts.Entities;
using System.Linq;
using System.Configuration;

namespace Tests.Data
{
    [TestFixture]
    class FlatFilePostRepositoryTest
    {
        private FlatFilePostRepository sut;
        private int postNumber = 0;
        private int authorNumber = 1;
        private readonly string rootDirectory = ConfigurationManager.AppSettings["PostRootDirectory"];
        private const string TestAuthor = "Test Author";
        private readonly DateTime testDate = DateTime.Parse( "April 10, 2013" );
        private const string TestTitle = "Test... Post ";
        private const string TestBody = "Lorem ipsum dolor sit amet";
        private readonly string[] testTag = new string[] { "Test Tag" };
        private const string TestPost = "2013_04_10_Test..._Post_";
        private const string TestAuthorDirectory = "test_author";

        [SetUp]
        public void Setup()
        {
            sut = new FlatFilePostRepository();
        }

        [TearDown]
        public void Teardown()
        {
            for( int author = 1; author <= authorNumber; author++ )
            {
                var testAuthorPath = string.Format( "{0}\\{1}",
                    rootDirectory, TestAuthorDirectory + author );
                for( int post = 1; post <= postNumber; post++ )
                {
                    var testFilePath = string.Format( "{0}\\{1}.html", testAuthorPath, TestPost + post );
                    if( File.Exists( testFilePath ) ) File.Delete( testFilePath );
                }
                if( Directory.Exists( testAuthorPath ) )
                    Directory.Delete( testAuthorPath );
            }
            if( Directory.Exists( rootDirectory ) )
                Directory.Delete( rootDirectory );
            postNumber = 0;
            authorNumber = 1;
        }

        [Test]
        public void it_creates_a_directory_for_the_given_author()
        {
            var expectedDirectory = string.Format( "{0}\\{1}",
                rootDirectory, TestAuthorDirectory + authorNumber );
            sut.CreateAuthorDirectory( TestAuthor + authorNumber );
            Assert.IsTrue( Directory.Exists( expectedDirectory ) );
        }

        [Test]
        public void it_converts_the_date_and_title_into_a_filename()
        {
            var result = sut.FileName( testDate, TestTitle );
            Assert.AreEqual( TestPost + ".html", result );
        }

        [Test]
        public void it_creates_a_post_with_the_correct_name()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var expectedFile = string.Format( "{0}\\{1}\\{2}.html",
                rootDirectory, TestAuthorDirectory + authorNumber, TestPost + postNumber );
            Assert.IsTrue( File.Exists( expectedFile ) );
        }

        [Test]
        public void it_maps_a_post_file_to_a_post_object()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var postPath = string.Format( "{0}\\{1}\\{2}.html",
                rootDirectory, TestAuthorDirectory + authorNumber, TestPost + postNumber );
            var result = sut.MapFileToPost( postPath );
            Assert.AreEqual( TestTitle + authorNumber, result.Title );
            Assert.AreEqual( TestBody, result.Body );
            Assert.AreEqual( TestAuthor + authorNumber, result.Author );
            Assert.AreEqual( testDate, result.Date );
            Assert.AreEqual( testTag, result.Tags );
        }

        [Test]
        public void it_gets_all_posts_by_author()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.CreatePost( CreateTestPost( ++authorNumber ) );
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var result = sut.GetAllPosts( TestAuthor + authorNumber ).ToList();
            Assert.AreEqual( 2, result.Count );
        }

        [Test]
        public void it_gets_all_posts()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.CreatePost( CreateTestPost( ++authorNumber ) );
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var result = sut.GetAllPosts().ToList();
            Assert.AreEqual( 3, result.Count );
        }

        [Test]
        public void it_deletes_posts()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.DeletePost( TestAuthor + authorNumber, testDate, TestTitle + postNumber );
            var posts = sut.GetAllPosts().ToList();
            Assert.AreEqual( 0, posts.Count );
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
    }
}
