using System.Collections.Generic;

namespace Csss
{
    internal sealed record AllElementSelector<TContext> : SingletonElementSelector<TContext, AllElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { "" };
    }

    internal sealed record AndElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : BivariateElementSelector<TContext>(Lhs, Rhs)
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => new OrElementSelector<TContext>(Lhs.Not(), Rhs.Not());
    }

    internal sealed record OrElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : BivariateElementSelector<TContext>(Lhs, Rhs)
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => new AndElementSelector<TContext>(Lhs.Not(), Rhs.Not());
    }

    internal sealed record ParentElementSelector<TContext>(ElementSelector<TContext> Parent) : LocatorElementSelector<TContext>(Parent)
    where TContext : class
    { }

    internal sealed record DirectParentElementSelector<TContext>(ElementSelector<TContext> Parent) : LocatorElementSelector<TContext>(Parent)
    where TContext : class
    { }

    internal sealed record BeforeElementSelector<TContext>(ElementSelector<TContext> Before) : LocatorElementSelector<TContext>(Before)
    where TContext : class
    { }

    internal sealed record ElementElementSelector<TContext>(ContextualValue<TContext, string> Name) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { Name };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Name})";
    }

    internal sealed record IdElementSelector<TContext>(ContextualValue<TContext, string> Id) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "#", Id };
    }

    internal sealed record ClassElementSelector<TContext>(ContextualValue<TContext, string> Class) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ".", Class };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Class})";
    }

    internal sealed record AttributeElementSelector<TContext>(ContextualValue<TContext, string> Attribute) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "]" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute})";
    }

    internal sealed record AttributeEqualsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeIncludsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "~='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeStartsWithElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "|='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record LinkElementSelector<TContext> : SingletonElementSelector<TContext, LinkElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":link" };
    }

    internal sealed record VisitedElementSelector<TContext> : SingletonElementSelector<TContext, VisitedElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":visited" };
    }

    internal sealed record ActiveElementSelector<TContext> : SingletonElementSelector<TContext, ActiveElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":active" };
    }

    internal sealed record HoverElementSelector<TContext> : SingletonElementSelector<TContext, HoverElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":hover" };
    }

    internal sealed record FocusElementSelector<TContext> : SingletonElementSelector<TContext, FocusElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":focus" };
    }

    internal sealed record FirstLetterElementSelector<TContext> : SingletonElementSelector<TContext, FirstLetterElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":first-letter" };
    }
}
