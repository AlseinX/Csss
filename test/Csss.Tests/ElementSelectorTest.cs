using System;
using Xunit;
using static Csss.CssBuilder<Csss.Tests.TestContext>;

namespace Csss.Tests
{
    public class ElementSelectorTest
    {
        [Fact]
        public void RearrangeTest()
        {
            var original = (Attribute("x") | AttributeEquals("y", "1") | AttributeIncludes("z", "2")) + !(Element("a") & Class("b") & Class("c"));
            var round1 = ((ContainerElementSelector<TestContext>)original).Rearrange();
            var round2 = ((ContainerElementSelector<TestContext>)round1).Rearrange();
            Assert.Equal("[x]+:not(a),[x]+:not(.b),[x]+:not(.c),[y='1']+:not(a),[y='1']+:not(.b),[y='1']+:not(.c),[z~='2']+:not(a),[z~='2']+:not(.b),[z~='2']+:not(.c)", round2.ToString(new()));
        }
    }
}
