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
            var original = (Attribute("a") | Hover) + !(Class("b") & Id("c")) & Active;
            var round1 = ((ContainerElementSelector<TestContext>)original).Rearrange();
            var round2 = ((ContainerElementSelector<TestContext>)round1).Rearrange();
            Assert.Equal(round1, round2);
        }

        [Fact]
        public void OutputTest()
        {
            var selector = (Attribute("a") | Hover) + !(Class("b") & Id("c")) & Active;
            Assert.Equal("[a]+:not(.b):active,[a]+:not(#c):active,:hover+:not(.b):active,:hover+:not(#c):active", selector.ToString(new()));
        }
    }
}
