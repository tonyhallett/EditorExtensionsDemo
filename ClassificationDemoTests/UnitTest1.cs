using EditorExtensionsDemo;

namespace ClassificationDemoTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            new ClassificationViewModel(null,null,null,null);
            Assert.Pass();
        }
    }
}