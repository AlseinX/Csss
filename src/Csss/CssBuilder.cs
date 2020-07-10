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

        public static ElementSelector<TContext> Attribute(string attribute) => new AttributeElementSelector<TContext>(attribute);

        public static ElementSelector<TContext> AttributeEquals(string attribute, string value) => new AttributeEqualsElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeIncludes(string attribute, string value) => new AttributeIncludsElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeStartsWith(string attribute, string value) => new AttributeStartsWithElementSelector<TContext>(attribute, value);
    }
}