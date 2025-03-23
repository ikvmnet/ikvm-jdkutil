using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IKVM.JdkUtil.Tests
{

    [TestClass]
    public sealed class JdkTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CanResolveJdks()
        {
            foreach (var i in Jdk.Resolve())
                TestContext.WriteLine(i.ToString());
        }

    }

}
