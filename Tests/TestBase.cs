using AutoMoq;
using NUnit.Framework;

namespace Tests
{
    public class TestBase<T>
        where T : class
    {
        protected AutoMoqer Mocks;
        protected T ClassUnderTest;

        [TestFixtureSetUp]
        public void Init()
        {
            Mocks = new AutoMoqer();
            ClassUnderTest = Mocks.Resolve<T>();

            Arrange();
            Act();
        }

        [TestFixtureTearDown]
        public void TestCleanup()
        {
            Finally();
        }

        public virtual void Arrange() { }
        public virtual void Act() { }
        public virtual void Finally() { }
    }
}
