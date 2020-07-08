using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csss
{
    public abstract record CssFragment<TContext>
    where TContext : class
    {
        private protected abstract IEnumerable<ContextualValue<TContext, string>> Compile();

        public Func<TContext, string> ToFactory()
        {
            static IEnumerable<ContextualValue<TContext, string>> Arrange(IEnumerable<ContextualValue<TContext, string>> source)
            {
                var sb = new StringBuilder();

                foreach (var v in source)
                {
                    if (v.Get(out var value, out var factory))
                    {
                        sb.Append(value);
                    }
                    else
                    {
                        if (sb.Length > 0)
                        {
                            yield return sb.ToString();
                        }

                        sb.Clear();

                        yield return factory;
                    }
                }

                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                }
            }

            var cache = Arrange(Compile()).ToList();

            return context =>
            {
                var sb = new StringBuilder();

                foreach (var v in cache)
                {
                    if (v.Get(out var value, out var factory))
                    {
                        sb.Append(value);
                    }
                    else
                    {
                        sb.Append(factory(context));
                    }
                }

                return sb.ToString();
            };
        }

        public string ToString(TContext context)
        {
            var sb = new StringBuilder();

            foreach (var v in Compile())
            {
                if (v.Get(out var value, out var factory))
                {
                    sb.Append(value);
                }
                else
                {
                    sb.Append(factory(context));
                }
            }

            return sb.ToString();
        }
    }
}