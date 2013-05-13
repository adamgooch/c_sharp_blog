using System;
using Application.Interactors;
using AutoMoq;
using Application.RepositoryContracts;
using Moq;
using NUnit.Framework;
using System.Linq;
using Application.Entities;

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
        public void ItCreatesAPostWithTodaysDate()
        {
            var title = "Title";
            var body = "Body";
            var author = "Adam Gooch";
            var date = DateTime.Now;

            sut.CreatePost(title, body, author);

            //mocker.GetMock<IPostRepository>()
            //    .Verify(x => x.CreatePost(title, body, date, author), Times.Once());
        }

        [Test]
        public void ItGetsAPostByTitle()
        {
            var title = "Title";
            sut.FindByTitle(title);

        }

        [Test]
        public void ItGetsAllPosts()
        {
            var post1 = new Post();
            var post2 = new Post();
            var post3 = new Post();
            var allPosts = new Post[]
            {
                post1,
                post3
            };
            mocker.GetMock<IPostRepository>()
                .Setup(x => x.GetAllPosts())
                .Returns(allPosts);

            var result = sut.GetAllPosts();
            Assert.AreEqual(2, result.Count());
        }
    }
}
