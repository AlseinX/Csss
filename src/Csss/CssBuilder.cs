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

        public static ElementSelector<TContext> Attribute(Func<TContext, string> factory) => new AttributeElementSelector<TContext>(factory);

        public static ElementSelector<TContext> AttributeEquals(string attribute, string value) => new AttributeEqualsElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeEquals(Func<TContext, string> attributeFactory, Func<TContext, string> valueFactory) => new AttributeEqualsElementSelector<TContext>(attributeFactory, valueFactory);

        public static ElementSelector<TContext> AttributeHasWord(string attribute, string value) => new AttributeHasWordElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeHasWord(Func<TContext, string> attributeFactory, Func<TContext, string> valueFactory) => new AttributeHasWordElementSelector<TContext>(attributeFactory, valueFactory);

        public static ElementSelector<TContext> AttributeFirstWord(string attribute, string value) => new AttributeFirstWordElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeFirstWord(Func<TContext, string> attributeFactory, Func<TContext, string> valueFactory) => new AttributeFirstWordElementSelector<TContext>(attributeFactory, valueFactory);

        public static ElementSelector<TContext> All => AllElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Link => LinkElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Visited => VisitedElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Active => ActiveElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Hover => HoverElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Focus => FocusElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> FirstLetter => FirstLetterElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> FirstLine => FirstLineElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> FirstChild => FirstChildElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Before => BeforeElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> After => AfterElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Lang(string language) => new LangElementSelector<TContext>(language);

        public static ElementSelector<TContext> Lang(Func<TContext, string> factory) => new LangElementSelector<TContext>(factory);

        public static ElementSelector<TContext> AttributeStartsWith(string attribute, string value) => new AttributeStartsWithElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeStartsWith(Func<TContext, string> attributeFactory, Func<TContext, string> valueFactory) => new AttributeStartsWithElementSelector<TContext>(attributeFactory, valueFactory);

        public static ElementSelector<TContext> AttributeEndsWith(string attribute, string value) => new AttributeEndsWithElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeEndsWith(Func<TContext, string> attributeFactory, Func<TContext, string> valueFactory) => new AttributeEndsWithElementSelector<TContext>(attributeFactory, valueFactory);

        public static ElementSelector<TContext> AttributeContains(string attribute, string value) => new AttributeContainsElementSelector<TContext>(attribute, value);

        public static ElementSelector<TContext> AttributeContains(Func<TContext, string> attributeFactory, Func<TContext, string> valueFactory) => new AttributeContainsElementSelector<TContext>(attributeFactory, valueFactory);

        public static ElementSelector<TContext> FirstOfType => FirstOfTypeElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> LastOfType => LastOfTypeElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> OnlyOfType => OnlyOfTypeElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> OnlyChild => OnlyChildElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> NthChild(int value) => new NthChildElementSelector<TContext>(value);

        public static ElementSelector<TContext> NthChild(Func<TContext, int> factory) => new NthChildElementSelector<TContext>(factory);

        public static ElementSelector<TContext> NthLastChild(int value) => new NthLastChildElementSelector<TContext>(value);

        public static ElementSelector<TContext> NthLastChild(Func<TContext, int> factory) => new NthLastChildElementSelector<TContext>(factory);

        public static ElementSelector<TContext> NthOfType(int value) => new NthOfTypeElementSelector<TContext>(value);

        public static ElementSelector<TContext> NthOfType(Func<TContext, int> factory) => new NthOfTypeElementSelector<TContext>(factory);

        public static ElementSelector<TContext> NthLastOfType(int value) => new NthLastOfTypeElementSelector<TContext>(value);

        public static ElementSelector<TContext> NthLastOfType(Func<TContext, int> factory) => new NthLastOfTypeElementSelector<TContext>(factory);

        public static ElementSelector<TContext> LastChild => LastChildElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Root => CheckedElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Empty => EmptyElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Target => CheckedElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Enabled => EnabledElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Disabled => DisabledElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Checked => CheckedElementSelector<TContext>.Positive;

        public static ElementSelector<TContext> Not(string selector) => new NotElementSelector<TContext>(selector);

        public static ElementSelector<TContext> Not(Func<TContext, string> factory) => new NotElementSelector<TContext>(factory);

        public static ElementSelector<TContext> Selection => CheckedElementSelector<TContext>.Positive;
    }
}