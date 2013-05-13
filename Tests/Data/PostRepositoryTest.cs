using Data.Repositories;
using NUnit.Framework;
using System;
using System.IO;

namespace Tests.Data
{
    [TestFixture]
    class PostRepositoryTest
    {
        private PostRepository sut;
        private string rootDirectory;
        private readonly string testAuthor = "Adam Gooch";
        private readonly string testAuthorDirectory = "adam_gooch";
        private readonly DateTime testDate = DateTime.Parse( "April 10, 2013" );
        private readonly string testTitle = "Test Post";
        private readonly string testPost = "2013_04_10_Test_Post.html";
        
        [SetUp]
        public void Setup()
        {
            sut = new PostRepository();
            rootDirectory = "C:/Users/Adam/Documents/Visual Studio 2012/Projects/Blog/Tests/bin/posts";
            sut.SetRootDirectory( rootDirectory );
        }

        [TearDown]
        public void Teardown()
        {
            var testAuthorPath = string.Format( "{0}/{1}", rootDirectory, testAuthorDirectory );
            var testFilePath = string.Format( "{0}/{1}", testAuthorPath, testPost );
            if( File.Exists( testFilePath ) )
                File.Delete( testFilePath );
            if( Directory.Exists( testAuthorPath ) )
                Directory.Delete( testAuthorPath );
            if( Directory.Exists( rootDirectory ) )
                Directory.Delete( rootDirectory );
        }

        [Test]
        public void it_creates_a_directory_for_the_given_author()
        {
            var expectedDirectory = string.Format( "{0}/{1}", rootDirectory, testAuthorDirectory );
            sut.CreateAuthorDirectory( testAuthor );
            Assert.IsTrue( Directory.Exists( expectedDirectory ) );
        }

        [Test]
        public void it_converts_the_date_and_title_into_a_filename()
        {
            var result = sut.FileName( testDate, testTitle );
            Assert.AreEqual( testPost, result );
        }

        [Test]
        public void it_creates_a_post_with_the_correct_name()
        {
            var body = "Test Body";
            var expectedFile = string.Format( "{0}/{1}/{2}",
                rootDirectory, testAuthorDirectory, testPost );
            sut.CreatePost( testTitle, body, testDate, testAuthor );
            Assert.IsTrue( File.Exists( expectedFile ) );
        }

        [Test]
        public void it_creates_a_post_with_the_given_content()
        {
            var body = "<h1>Lorem Ipsum</h1>";
            var filePath = string.Format( "{0}/{1}/{2}",
                rootDirectory, testAuthorDirectory, testPost );
            sut.CreatePost( testTitle, body, testDate, testAuthor );
            byte[] readBuffer = File.ReadAllBytes( filePath );
            var fileContents = GetString( readBuffer );
            Assert.AreEqual( body, fileContents );
        }

        private string GetString( byte[] bytes )
        {
            char[] chars = new char[bytes.Length / sizeof( char )];
            System.Buffer.BlockCopy( bytes, 0, chars, 0, bytes.Length );
            return new string( chars );
        }
    }
}
