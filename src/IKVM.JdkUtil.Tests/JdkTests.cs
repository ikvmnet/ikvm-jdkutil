using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IKVM.JdkUtil.Tests
{

    [TestClass]
    public sealed class JdkTests
    {

        [TestMethod]
        public void CanResolveJdks()
        {
            foreach (var i in Jdk.Resolve())
                Console.WriteLine(i);
        }

    }

}
