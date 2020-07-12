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

    internal sealed record AheadElementSelector<TContext>(ElementSelector<TContext> Ahead) : LocatorElementSelector<TContext>(Ahead)
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

    internal sealed record AttributeHasWordElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "~='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeFirstWordElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
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

    internal sealed record FirstLineElementSelector<TContext> : SingletonElementSelector<TContext, FirstLineElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":first-line" };
    }

    internal sealed record FirstChildElementSelector<TContext> : SingletonElementSelector<TContext, FirstChildElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":first-child" };
    }

    internal sealed record BeforeElementSelector<TContext> : SingletonElementSelector<TContext, BeforeElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":before" };
    }

    internal sealed record AfterElementSelector<TContext> : SingletonElementSelector<TContext, AfterElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":after" };
    }

    internal sealed record LangElementSelector<TContext>(ContextualValue<TContext, string> Language) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ":lang(", Language, ")" };
    }

    internal sealed record AttributeStartsWithElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "^='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeEndsWithElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "$='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeContainsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { "[", Attribute, "*='", Value, "']" };

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record FirstOfTypeElementSelector<TContext> : SingletonElementSelector<TContext, FirstOfTypeElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":first-of-type" };
    }

    internal sealed record LastOfTypeElementSelector<TContext> : SingletonElementSelector<TContext, LastOfTypeElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":last-of-type" };
    }

    internal sealed record OnlyOfTypeElementSelector<TContext> : SingletonElementSelector<TContext, OnlyOfTypeElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":only-of-type" };
    }

    internal sealed record OnlyChildElementSelector<TContext> : SingletonElementSelector<TContext, OnlyChildElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":only-child" };
    }

    internal sealed record NthChildElementSelector<TContext>(ContextualValue<TContext, int> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ":nth-child(", Value, ")" };
    }

    internal sealed record NthLastChildElementSelector<TContext>(ContextualValue<TContext, int> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ":nth-last-child(", Value, ")" };
    }

    internal sealed record NthOfTypeElementSelector<TContext>(ContextualValue<TContext, int> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ":nth-of-type(", Value, ")" };
    }

    internal sealed record NthLastOfTypeElementSelector<TContext>(ContextualValue<TContext, int> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ":nth-last-of-type(", Value, ")" };
    }

    internal sealed record LastChildElementSelector<TContext> : SingletonElementSelector<TContext, LastChildElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":last-child" };
    }

    internal sealed record RootElementSelector<TContext> : SingletonElementSelector<TContext, RootElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":root" };
    }

    internal sealed record EmptyElementSelector<TContext> : SingletonElementSelector<TContext, EmptyElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":empty" };
    }

    internal sealed record TargetElementSelector<TContext> : SingletonElementSelector<TContext, TargetElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":target" };
    }

    internal sealed record EnabledElementSelector<TContext> : SingletonElementSelector<TContext, EnabledElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":enabled" };
    }

    internal sealed record DisabledElementSelector<TContext> : SingletonElementSelector<TContext, DisabledElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":disabled" };
    }

    internal sealed record CheckedElementSelector<TContext> : SingletonElementSelector<TContext, CheckedElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { ":checked" };
    }

    internal sealed record NotElementSelector<TContext>(ContextualValue<TContext, string> Selector) : TerminalElementSelector<TContext>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new[] { ":not(", Selector, ")" };
    }

    internal sealed record SelectionElementSelector<TContext> : SingletonElementSelector<TContext, SelectionElementSelector<TContext>>
    where TContext : class
    {
        private protected override IEnumerable<ContextualValue<TContext, string>> Output => new ContextualValue<TContext, string>[] { "::selection" };
    }
}
