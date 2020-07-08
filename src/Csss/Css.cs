using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Csss
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed record Css<TContext> : CssFragment<TContext>
    where TContext : class
    {
        internal readonly List<CssRule<TContext>> _rules = new();

        public IReadOnlyCollection<CssRule<TContext>> Rules => _rules;

        internal Css() { }

        public override bool Equals(object? obj)
        => obj is Css<TContext> sheet && _rules.SequenceEqual(sheet._rules);

        public override int GetHashCode()
        => _rules.Aggregate(0, (n, i) => n ^ i.GetHashCode());

        public override string ToString() => string.Join("\n", _rules.Select(r => r.ToString()));

        public static implicit operator string(Css<TContext> sheet) => sheet.ToString();

        private string GetDebuggerDisplay() => ToString();

        private protected override IEnumerable<ContextualValue<TContext, string>> Compile()
        {
            throw new System.NotImplementedException();
        }
    }
}