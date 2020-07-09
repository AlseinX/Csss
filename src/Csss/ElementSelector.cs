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
        => selector.Not();

        public static ElementSelector<TContext> operator &(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => (lhs, rhs) switch
        {
            (AllElementSelector<TContext> { IsNot: false }, _) => rhs,
            (AllElementSelector<TContext> { IsNot: true }, _) => lhs,
            (_, AllElementSelector<TContext> { IsNot: false }) => lhs,
            (_, AllElementSelector<TContext> { IsNot: true }) => rhs,
            _ => new AndElementSelector<TContext>(lhs, rhs)
        };

        public static ElementSelector<TContext> operator |(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => (lhs, rhs) switch
        {
            (AllElementSelector<TContext> { IsNot: false }, _) => lhs,
            (AllElementSelector<TContext> { IsNot: true }, _) => rhs,
            (_, AllElementSelector<TContext> { IsNot: false }) => rhs,
            (_, AllElementSelector<TContext> { IsNot: true }) => lhs,
            _ => new OrElementSelector<TContext>(lhs, rhs)
        };

        public static ElementSelector<TContext> operator /(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => rhs & new ParentElementSelector<TContext>(lhs);

        public static ElementSelector<TContext> operator >(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => rhs & new DirectParentElementSelector<TContext>(lhs);

        [Obsolete]
        public static ElementSelector<TContext> operator <(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => lhs & new DirectParentElementSelector<TContext>(rhs);

        public static ElementSelector<TContext> operator +(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => rhs & new BeforeElementSelector<TContext>(lhs);

        internal ElementSelector() { }

        private protected override IEnumerable<ContextualValue<TContext, string>> Compile()
        {
            var target = this switch
            {
                ContainerElementSelector<TContext> container => container.Rearrange(),
                _ => this
            };

            yield break;
        }


        internal abstract ElementSelector<TContext> Not();
    }

    internal abstract record ContainerElementSelector<TContext> : ElementSelector<TContext>
        where TContext : class
    {
        internal abstract ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor);

        private protected static ElementSelector<TContext> VisitMember(ElementSelector<TContext> selector, Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor)
        => visitor(selector) switch
        {
            ContainerElementSelector<TContext> container => container.Visit(visitor),
            _ => selector
        };

        internal ElementSelector<TContext> Rearrange()
        {
            var result = default(ElementSelector<TContext>);
            var x = 0L;
            var l = 0;

            do
            {
                var c = 0;
                var visited = Visit(m =>
                {
                    if (m is OrElementSelector<TContext> or)
                    {
                        if (c == l)
                        {
                            x <<= 1;
                            l++;
                            c++;
                            return or.Lhs;
                        }

                        if ((x >> (l - c - 1) & 1) == 0)
                        {
                            return or.Lhs;
                        }

                        return or.Rhs;
                    }

                    return m;
                });

                x++;

                while ((x & 1) == 0)
                {
                    if (l == 0)
                    {
                        x = 0;
                        break;
                    }

                    x >>= 1;
                    l--;
                }

                result = result == default ? visited : result | visited;
            } while (x == 0);

            return result;
        }
    }


    internal abstract record TerminalElementSelector<TContext>(bool IsNot = false) : ElementSelector<TContext>
        where TContext : class
    {
        internal override ElementSelector<TContext> Not() => this with { IsNot = true };
    }

    internal abstract record SingletonElementSelector<TContext, TSelf> : TerminalElementSelector<TContext>
        where TContext : class
        where TSelf : SingletonElementSelector<TContext, TSelf>, new()
    {
        static SingletonElementSelector()
        {
            Positive = new();
            Negative = (TSelf)((SingletonElementSelector<TContext, TSelf>)Positive with { IsNot = true });
        }

        internal static TSelf Positive { get; }

        internal static TSelf Negative { get; }
    }

    internal sealed record AllElementSelector<TContext> : SingletonElementSelector<TContext, AllElementSelector<TContext>>
    where TContext : class
    { }

    internal sealed record AndElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ContainerElementSelector<TContext>
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => new OrElementSelector<TContext>(Lhs.Not(), Rhs.Not());

        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Lhs = VisitMember(Lhs, visitor),
            Rhs = VisitMember(Rhs, visitor)
        };
    }

    internal sealed record OrElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ContainerElementSelector<TContext>
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => new AndElementSelector<TContext>(Lhs.Not(), Rhs.Not());

        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Lhs = VisitMember(Lhs, visitor),
            Rhs = VisitMember(Rhs, visitor)
        };
    }

    internal sealed record ParentElementSelector<TContext>(ElementSelector<TContext> Parent) : ContainerElementSelector<TContext>
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => this with { Parent = Parent.Not() };

        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Parent = VisitMember(Parent, visitor)
        };
    }

    internal sealed record DirectParentElementSelector<TContext>(ElementSelector<TContext> Parent) : ContainerElementSelector<TContext>
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => this with { Parent = Parent.Not() };

        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Parent = VisitMember(Parent, visitor)
        };
    }

    internal sealed record BeforeElementSelector<TContext>(ElementSelector<TContext> Before) : ContainerElementSelector<TContext>
    where TContext : class
    {
        internal override ElementSelector<TContext> Not() => this with { Before = Before.Not() };

        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Before = VisitMember(Before, visitor)
        };
    }

    internal sealed record ElementElementSelector<TContext>(ContextualValue<TContext, string> Name) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record IdElementSelector<TContext>(ContextualValue<TContext, string> Id) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record ClassElementSelector<TContext>(ContextualValue<TContext, string> Class) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeElementSelector<TContext>(ContextualValue<TContext, string> Attribute) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeEqualsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeIncludingElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record AttributeStartsWithElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    { }

    internal sealed record LinkElementSelector<TContext> : SingletonElementSelector<TContext, LinkElementSelector<TContext>>
    where TContext : class
    { }

    internal sealed record VisitedElementSelector<TContext> : SingletonElementSelector<TContext, VisitedElementSelector<TContext>>
    where TContext : class
    { }

    internal sealed record ActiveElementSelector<TContext> : SingletonElementSelector<TContext, ActiveElementSelector<TContext>>
    where TContext : class
    { }

    internal sealed record HoverElementSelector<TContext> : SingletonElementSelector<TContext, HoverElementSelector<TContext>>
    where TContext : class
    { }

    internal sealed record FocusElementSelector<TContext> : SingletonElementSelector<TContext, FocusElementSelector<TContext>>
    where TContext : class
    { }

    internal sealed record FirstLetterElementSelector<TContext> : SingletonElementSelector<TContext, FirstLetterElementSelector<TContext>>
    where TContext : class
    { }

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
