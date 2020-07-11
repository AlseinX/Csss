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
        => new AheadElementSelector<TContext>(lhs) & rhs;

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

                var appendAll = true;

                if (element != default)
                {
                    foreach (var result in element.Compile())
                    {
                        yield return result;
                    }

                    appendAll = false;
                }

                if (id != default)
                {
                    foreach (var result in id.Compile())
                    {
                        yield return result;
                    }

                    appendAll = false;
                }

                if (others != default)
                {
                    foreach (var terminal in others)
                    {
                        foreach (var result in terminal.Compile())
                        {
                            yield return result;
                        }
                    }

                    appendAll = false;
                }

                if (appendAll)
                {
                    yield return "*";
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

        private protected sealed override IEnumerable<ContextualValue<TContext, string>> Compile()
        {
            IEnumerable<ContextualValue<TContext, string>> Result()
            {
                if (IsNot)
                {
                    yield return ":not(";
                }

                foreach (var result in Output)
                {
                    yield return result;
                }

                if (IsNot)
                {
                    yield return ")";
                }
            }

            return Result();
        }

        private protected abstract IEnumerable<ContextualValue<TContext, string>> Output { get; }
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

}
