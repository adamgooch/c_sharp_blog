using System;
using System.Linq;
using Moq;
using AutoMoq;
using NUnit.Framework;
using Application.Posts.Entities;
using Application.Posts.Interactors;
using Application.Posts.RepositoryContracts;
using System.Collections.Generic;
using SimpleCrypto;

namespace Tests.Application.Interactors
{
    [TestFixture]
    public class PostInteractorTest
    {
        private AutoMoqer mocker;
        private PostInteractor sut;
        
        [SetUp]
        public void Setup()
        {
            mocker = new AutoMoqer();
            sut = mocker.Resolve<PostInteractor>();
        }

        [Test]
        public void it_sets_the_root_directory_for_posts_to_be_saved_in()
        {
            mocker.GetMock<IPostRepository>()
                .Verify( x => x.SetRootDirectory( "C:\\Users\\Adam\\Documents\\Visual Studio 2012\\Projects\\Blog\\site_posts" ) );
        }

        [Test]
        public void it_creates_a_post_with_todays_date()
        {
            var post = new Post
            {
                title = "Title",
                body = "Body",
                author = "Adam Gooch",
                tags = new string[] { "Tag 1", "Tag 2" }
            };
            sut.CreatePost( post );

            mocker.GetMock<IPostRepository>()
                .Verify( x => x.CreatePost( post ), Times.Once() );
            Assert.AreEqual( DateTime.Now.ToString( "yyyy_MM_dd" ), post.date.ToString( "yyyy_MM_dd" ) );
        }

        [Test]
        public void it_gets_all_posts_by_author()
        {
            var author = "Test Author";
            var post1 = new Post();
            var post2 = new Post();
            var post3 = new Post();
            var allPosts = new List<Post> { post1, post3 };
            mocker.GetMock<IPostRepository>()
                .Setup( x => x.GetAllPosts( author ) )
                .Returns( allPosts );

            var result = sut.GetAllPosts( author );

            Assert.AreEqual( 2, result.Count() );
        }

        [Test]
        public void it_gets_the_latest_post()
        {
            var post1 = new Post { date = DateTime.Parse( "April 4, 2013" ) };
            var post2 = new Post { date = DateTime.Parse( "April 2, 2013" ) };
            var post3 = new Post { date = DateTime.Parse( "April 3, 2013" ) };
            var allPosts = new List<Post> { post1, post2, post3 };
            mocker.GetMock<IPostRepository>()
                .Setup( x => x.GetAllPosts() )
                .Returns( allPosts );

            var result = sut.GetLatestPost();

            Assert.AreEqual( post1, result );
        }
    }
}
