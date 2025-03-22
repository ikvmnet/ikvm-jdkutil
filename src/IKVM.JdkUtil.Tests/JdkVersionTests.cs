using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IKVM.JdkUtil.Tests
{

    [TestClass]
    public class JdkVersionTests
    {

        [TestMethod]
        public void CanParseJdk8Version()
        {
            var v = JdkVersion.Parse("1.8.1");
            v.Feature.Should().Be(1);
        }

        [TestMethod]
        public void CanParseJdk9Version()
        {
            var v = JdkVersion.Parse("9.0.0");
            v.Feature.Should().Be(9);
        }

        [TestMethod]
        public void CanSortJdkVersion()
        {
            var jdk8 = JdkVersion.Parse("1.8.1");
            var jdk9 = JdkVersion.Parse("9.0.1");
            var l = new JdkVersion[] { jdk9, jdk8 };
            Array.Sort(l);
            l.Should().ContainInConsecutiveOrder([
                jdk8,
                jdk9
            ]);
        }

    }

}
