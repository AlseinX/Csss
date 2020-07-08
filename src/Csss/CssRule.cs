using System.Collections.Generic;

namespace Csss
{
    public sealed record CssRule<TContext> : CssFragment<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Compile()
        {
            throw new System.NotImplementedException();
        }
    }
}