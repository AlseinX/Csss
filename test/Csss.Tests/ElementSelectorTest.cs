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
            var s = !(Element("x") & Element("y")) / !(Element("a") & Class("b"));
            var r = ((ContainerElementSelector<TestContext>)s).Rearrange();
            var r2 = ((ContainerElementSelector<TestContext>)r).Rearrange();
            var result = r.ToString(new());
        }
    }
}
