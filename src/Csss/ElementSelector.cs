using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Csss
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public abstract record ElementSelector<TContext> : CssFragment<TContext>
    where TContext : class
    {
        public string Type
        {
            get
            {
                var name = GetType().Name;
                return name.Substring(0, name.Length - nameof(ElementSelector<TContext>).Length - 2);
            }
        }

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
        => new ParentElementSelector<TContext>(lhs) & rhs;

        public static ElementSelector<TContext> operator >(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new DirectParentElementSelector<TContext>(lhs) & rhs;

        [Obsolete]
        public static ElementSelector<TContext> operator <(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new DirectParentElementSelector<TContext>(rhs) & lhs;

        public static ElementSelector<TContext> operator +(ElementSelector<TContext> lhs, ElementSelector<TContext> rhs)
        => new BeforeElementSelector<TContext>(lhs) & rhs;

        internal ElementSelector() { }

        private protected override IEnumerable<ContextualValue<TContext, string>> Compile()
        {
            var target = this switch
            {
                ContainerElementSelector<TContext> container => container.Rearrange(),
                _ => this
            };

            static IEnumerable<ElementSelector<TContext>> Flaten<TElementSelector>(ElementSelector<TContext> selector)
                where TElementSelector : BivariateElementSelector<TContext>
            {
                if (selector is TElementSelector biv)
                {
                    foreach (var item in Flaten<TElementSelector>(biv.Lhs))
                    {
                        yield return item;
                    }

                    foreach (var item in Flaten<TElementSelector>(biv.Rhs))
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return selector;
                }
            }

            var first = true;

            foreach (var item in Flaten<OrElementSelector<TContext>>(target))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return ",";
                }

                foreach (var result in HandleSingle(item))
                {
                    yield return result;
                }
            }

            static IEnumerable<ContextualValue<TContext, string>> HandleSingle(ElementSelector<TContext> target)
            {
                var element = default(ElementElementSelector<TContext>);
                var id = default(IdElementSelector<TContext>);
                var others = default(List<TerminalElementSelector<TContext>>);
                var locators = default(List<LocatorElementSelector<TContext>>);

                foreach (var item in Flaten<AndElementSelector<TContext>>(target))
                {
                    switch (item)
                    {
                        case ElementElementSelector<TContext> { IsNot: false } i:
                            element = element == default ? i : throw new AmbiguousMatchException("Multiple element selectors found.");
                            break;

                        case IdElementSelector<TContext> { IsNot: false } i:
                            id = id == default ? i : throw new AmbiguousMatchException("Multiple id selectors found.");
                            break;

                        case TerminalElementSelector<TContext> i:
                            others ??= new();
                            others.Add(i);
                            break;

                        case LocatorElementSelector<TContext> i:
                            locators ??= new();
                            locators.Add(i);
                            break;
                    }
                }

                if (locators != default)
                {
                    foreach (var locator in locators)
                    {
                        foreach (var result in HandleSingle(locator.Locator))
                        {
                            yield return result;
                        }

                        yield return locator switch
                        {
                            ParentElementSelector<TContext> _ => " ",
                            DirectParentElementSelector<TContext> _ => ">",
                            BeforeElementSelector<TContext> _ => "+",
                            _ => throw new NotSupportedException()
                        };
                    }
                }

                static IEnumerable<ContextualValue<TContext, string>> HandleTerminal(TerminalElementSelector<TContext> selector)
                {
                    if (selector.IsNot)
                    {
                        yield return ":not(";
                    }

                    switch (selector)
                    {
                        case ElementElementSelector<TContext> { Name: var name }:
                            yield return name;
                            break;

                        case IdElementSelector<TContext> { Id: var id }:
                            yield return "#";
                            yield return id;
                            break;

                        case ClassElementSelector<TContext> { Class: var @class }:
                            yield return ".";
                            yield return @class;
                            break;

                        case AttributeElementSelector<TContext> { Attribute: var attribute }:
                            yield return "[";
                            yield return attribute;
                            yield return "]";
                            break;

                        case AttributeEqualsElementSelector<TContext> { Attribute: var attribute, Value: var value }:
                            yield return "[";
                            yield return attribute;
                            yield return "='";
                            yield return value;
                            yield return "']";
                            break;

                        case AttributeIncludsElementSelector<TContext> { Attribute: var attribute, Value: var value }:
                            yield return "[";
                            yield return attribute;
                            yield return "~='";
                            yield return value;
                            yield return "']";
                            break;

                        case AttributeStartsWithElementSelector<TContext> { Attribute: var attribute, Value: var value }:
                            yield return "[";
                            yield return attribute;
                            yield return "|='";
                            yield return value;
                            yield return "']";
                            break;

                        case AllElementSelector<TContext> _:
                            yield return "*";
                            break;

                    };

                    if (selector.IsNot)
                    {
                        yield return ")";
                    }
                }


                if (element != default)
                {
                    foreach (var result in HandleTerminal(element))
                    {
                        yield return result;
                    }
                }

                if (id != default)
                {
                    foreach (var result in HandleTerminal(id))
                    {
                        yield return result;
                    }
                }

                if (others != default)
                {
                    foreach (var terminal in others)
                    {
                        foreach (var result in HandleTerminal(terminal))
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        internal abstract ElementSelector<TContext> Not();

        private string GetDebuggerDisplay() => ToString() ?? string.Empty;
    }

    internal abstract record ContainerElementSelector<TContext> : ElementSelector<TContext>
        where TContext : class
    {
        internal abstract ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor);

        private protected static ElementSelector<TContext> VisitMember(ElementSelector<TContext> selector, Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor)
        => visitor(selector) switch
        {
            ContainerElementSelector<TContext> container => container.Visit(visitor),
            ElementSelector<TContext> result => result
        };

        internal ElementSelector<TContext> Rearrange()
        {
            var result = default(ElementSelector<TContext>);
            var x = 0L;
            var l = 0;

            do
            {
                var c = 0;
                ElementSelector<TContext> Visitor(ElementSelector<TContext> m)
                {
                    while (m is OrElementSelector<TContext> or)
                    {
                        if (c == l)
                        {
                            x <<= 1;
                            l++;
                            m = or.Lhs;
                        }
                        else if ((x >> (l - c - 1) & 1) == 0)
                        {
                            m = or.Lhs;
                        }
                        else
                        {

                            m = or.Rhs;
                        }

                        c++;
                    }

                    return m;
                };

                var visited = Visitor(this);
                if (visited is ContainerElementSelector<TContext> container)
                {
                    visited = container.Visit(Visitor);
                }

                x++;

                while ((x & 1) == 0 && l > 0)
                {
                    x >>= 1;
                    l--;
                }

                result = result == default ? visited : result | visited;
            } while (x >> l == 0);

            return result;
        }
    }

    internal abstract record BivariateElementSelector<TContext>(ElementSelector<TContext> Lhs, ElementSelector<TContext> Rhs) : ContainerElementSelector<TContext>
        where TContext : class
    {
        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Lhs = VisitMember(Lhs, visitor),
            Rhs = VisitMember(Rhs, visitor)
        };

        public override string ToString() => $"[{Lhs} {Type} {Rhs}]";
    }

    internal abstract record LocatorElementSelector<TContext>(ElementSelector<TContext> Locator) : ContainerElementSelector<TContext>
        where TContext : class
    {
        internal override ElementSelector<TContext> Not() => this with
        {
            Locator = Locator.Not()
        };

        internal override ElementSelector<TContext> Visit(Func<ElementSelector<TContext>, ElementSelector<TContext>> visitor) => this with
        {
            Locator = VisitMember(Locator, visitor)
        };

        public override string ToString() => $"{Type}({Locator})";
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

        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}";
    }

    internal sealed record AllElementSelector<TContext> : SingletonElementSelector<TContext, AllElementSelector<TContext>>
    where TContext : class
    { }

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
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Name})";
    }

    internal sealed record IdElementSelector<TContext>(ContextualValue<TContext, string> Id) : TerminalElementSelector<TContext>
    where TContext : class
    {
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Id})";
    }

    internal sealed record ClassElementSelector<TContext>(ContextualValue<TContext, string> Class) : TerminalElementSelector<TContext>
    where TContext : class
    {
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Class})";
    }

    internal sealed record AttributeElementSelector<TContext>(ContextualValue<TContext, string> Attribute) : TerminalElementSelector<TContext>
    where TContext : class
    {
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute})";
    }

    internal sealed record AttributeEqualsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeIncludsElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

    internal sealed record AttributeStartsWithElementSelector<TContext>(ContextualValue<TContext, string> Attribute, ContextualValue<TContext, string> Value) : TerminalElementSelector<TContext>
    where TContext : class
    {
        public override string ToString() => $"{(IsNot ? "!" : "")}{Type}({Attribute}:{Value})";
    }

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
}
