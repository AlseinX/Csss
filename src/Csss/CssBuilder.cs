using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csss
{
    public static class CssBuilder
    {
    }

    public static class CssBuilder<TContext>
    where TContext : class
    {
        public static ElementSelector<TContext> Element(string value) => new ElementElementSelector<TContext>(value);

        public static ElementSelector<TContext> Element(Func<TContext, string> factory) => new ElementElementSelector<TContext>(factory);

        public static ElementSelector<TContext> Id(string value) => new IdElementSelector<TContext>(value);

        public static ElementSelector<TContext> Id(Func<TContext, string> factory) => new IdElementSelector<TContext>(factory);

        public static ElementSelector<TContext> Class(string value) => new ClassElementSelector<TContext>(value);

        public static ElementSelector<TContext> Class(Func<TContext, string> factory) => new ClassElementSelector<TContext>(factory);
    }
}