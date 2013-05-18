using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Data.Repositories;
using Application.Posts.Entities;
using System.Linq;

namespace Tests.Data
{
    [TestFixture]
    class PostRepositoryTest
    {
        private PostRepository sut;
        private int postNumber = 0;
        private int authorNumber = 1;
        private readonly string rootDirectory = "C:\\Users\\Adam\\Documents\\Visual Studio 2012\\Projects\\Blog\\Tests\\bin\\posts";
        private readonly string testAuthor = "Test Author";
        private readonly DateTime testDate = DateTime.Parse( "April 10, 2013" );
        private readonly string testTitle = "Test... Post ";
        private readonly string testBody = "Lorem ipsum dolor sit amet";
        private readonly string[] testTag = new string[]{ "Test Tag" };
        private readonly string testPost = "2013_04_10_Test..._Post_";
        private readonly string testAuthorDirectory = "test_author";
        
        [SetUp]
        public void Setup()
        {
            sut = new PostRepository();
            sut.SetRootDirectory( rootDirectory );
        }

        [TearDown]
        public void Teardown()
        {
            for( int author = 1; author <= authorNumber; author++ )
            {
                var testAuthorPath = string.Format( "{0}\\{1}", 
                    rootDirectory, testAuthorDirectory + author );
                for( int post = 1; post <= postNumber; post++ )
                {
                    var testFilePath = string.Format( "{0}\\{1}.html", testAuthorPath, testPost + post );
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
                rootDirectory, testAuthorDirectory + authorNumber );
            sut.CreateAuthorDirectory( testAuthor + authorNumber );
            Assert.IsTrue( Directory.Exists( expectedDirectory ) );
        }

        [Test]
        public void it_converts_the_date_and_title_into_a_filename()
        {
            var result = sut.FileName( testDate, testTitle );
            Assert.AreEqual( testPost + ".html", result );
        }

        [Test]
        public void it_creates_a_post_with_the_correct_name()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var expectedFile = string.Format( "{0}\\{1}\\{2}.html",
                rootDirectory, testAuthorDirectory + authorNumber, testPost + postNumber);
            Assert.IsTrue( File.Exists( expectedFile ) );
        }

        [Test]
        public void it_maps_a_post_file_to_a_post_object()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var postPath = string.Format( "{0}\\{1}\\{2}.html",
                rootDirectory, testAuthorDirectory + authorNumber, testPost + postNumber );
            var result = sut.MapFileToPost( postPath );
            Assert.AreEqual( testTitle + authorNumber, result.title );
            Assert.AreEqual( testBody, result.body );
            Assert.AreEqual( testAuthor + authorNumber, result.author );
            Assert.AreEqual( testDate, result.date );
            Assert.AreEqual( testTag, result.tags );
        }

        [Test]
        public void it_gets_all_posts_by_author()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.CreatePost( CreateTestPost( ++authorNumber ) );
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var result = (List<Post>)sut.GetAllPosts( testAuthor + authorNumber );
            Assert.AreEqual( 2, result.Count );
        }

        [Test]
        public void it_gets_all_posts()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.CreatePost( CreateTestPost( ++authorNumber ) );
            sut.CreatePost( CreateTestPost( authorNumber ) );
            var result = (List<Post>)sut.GetAllPosts();
            Assert.AreEqual( 3, result.Count );
        }

        [Test]
        public void it_deletes_posts()
        {
            sut.CreatePost( CreateTestPost( authorNumber ) );
            sut.DeletePost( testAuthor + authorNumber, testDate, testTitle + postNumber );
            var posts = sut.GetAllPosts();
            Assert.AreEqual( 0, Enumerable.Count( posts ) );
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
    }
}
