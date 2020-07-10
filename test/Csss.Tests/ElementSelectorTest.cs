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

        [Fact]
        public void OutputTest2()
        {
            var selector = !(Class("a") & Class("b") & Class("c") & Class("d")) > !(Class("e") & Class("f") & Class("g") & Class("h")) & Active;
            var round1 = ((ContainerElementSelector<TestContext>)selector).Rearrange();
            var result = selector.ToString(null);
        }
    }
}
