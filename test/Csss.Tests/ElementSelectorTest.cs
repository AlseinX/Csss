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
            var s = Element("x") > !(Element("a") & Class("b") & Id("c"));
            var result = s.ToString(new());
        }
    }
}
