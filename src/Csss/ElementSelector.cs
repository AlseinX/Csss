using System;
using System.Collections.Generic;
using System.Linq;
using static Csss.CssBuilder<object>;
namespace Csss
{
    public abstract record ElementSelector<TContext> : CssFragment<TContext>
    where TContext : class
    {
        public static ElementSelector<TContext> operator !(ElementSelector<TContext> selector)
        => new NotElementSelector<TContext>(selector);

        public static ElementSelector<TContext> operator &(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new AndElementSelector<TContext>(lhs, rhs);

        public static ElementSelector<TContext> operator |(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new OrElementSelector<TContext>(lhs, rhs);

        public static ElementSelector<TContext> operator /(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new InnerElementSelector<TContext>(lhs, rhs);

        public static ElementSelector<TContext> operator >(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new RightInnerElementSelector<TContext>(lhs, rhs);

        public static ElementSelector<TContext> operator <(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new RightInnerElementSelector<TContext>(rhs, lhs);

        public static ElementSelector<TContext> operator +(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new AfterElementSelector<TContext>(rhs, lhs);

        internal ElementSelector() { }

        private protected override IEnumerable<ContextualValue<TContext, string>> Compile()
        {
            yield break;
        }
    }

    internal sealed record NotElementSelector<TContext>(ElementSelector<TContext> selector) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AndElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record OrElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record InnerElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record RightInnerElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AfterElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record ElementElementSelector<TContext>(ContextualValue<TContext, string> Name) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record IdElementSelector<TContext>(ContextualValue<TContext, string> Id) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record ClassElementSelector<TContext>(ContextualValue<TContext, string> Class) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeElementSelector<TContext>(ContextualValue<TContext, string> Attribute) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeEqualsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeIncludingElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeStartsWithElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : ElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record LinkElementSelector<TContext> : ElementSelector<TContext>
    where TContext : class
    {
        private LinkElementSelector() { }

        public static LinkElementSelector<TContext> Instance { get; } = new();
    }

    internal sealed record VisitedElementSelector<TContext> : ElementSelector<TContext>
    where TContext : class
    {
        private VisitedElementSelector() { }

        public static VisitedElementSelector<TContext> Instance { get; } = new();
    }

    internal sealed record ActiveElementSelector<TContext> : ElementSelector<TContext>
    where TContext : class
    {
        private ActiveElementSelector() { }

        public static ActiveElementSelector<TContext> Instance { get; } = new();
    }

    internal sealed record HoverElementSelector<TContext> : ElementSelector<TContext>
    where TContext : class
    {
        private HoverElementSelector() { }

        public static HoverElementSelector<TContext> Instance { get; } = new();
    }

    internal sealed record FocusElementSelector<TContext> : ElementSelector<TContext>
    where TContext : class
    {
        private FocusElementSelector() { }

        public static FocusElementSelector<TContext> Instance { get; } = new();
    }

    internal sealed record FirstLetterElementSelector<TContext> : ElementSelector<TContext>
    where TContext : class
    {
        private FirstLetterElementSelector() { }

        public static FirstLetterElementSelector<TContext> Instance { get; } = new();
    }

    class A
    {
        void Test()
        {
            var style = (Element(o => "aaa") | !Id("bbb")) > Class("ccc");
            var factory = style.ToFactory();
            var css = factory(new()); // 也可以直接style.ToString(new())
        }
    }
}
