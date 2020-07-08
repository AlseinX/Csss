using System;
using System.Diagnostics.CodeAnalysis;

namespace Csss
{
    public readonly struct ContextualValue<TContext, TValue>
    where TContext : class
    {
        [AllowNull]
        private readonly TValue _value;

        private readonly Func<TContext, TValue>? _factory;

        public ContextualValue(TValue value)
        {
            _value = value;
            _factory = default;
        }

        public ContextualValue(Func<TContext, TValue> value)
        {
            _value = default;
            _factory = value;
        }

        public bool Get([MaybeNullWhen(false)] out TValue value, [MaybeNullWhen(true)] out Func<TContext, TValue> factory)
        {
            if (_factory == default)
            {
                value = _value;
                factory = default;
                return true;
            }
            else
            {
                value = default;
                factory = _factory;
                return false;
            }
        }

        public TValue Get(TContext context)
        {
            if (_factory == default)
            {
                return _value;
            }
            else
            {
                return _factory(context);
            }
        }

        public static implicit operator ContextualValue<TContext, TValue>(TValue value) => new(value);

        public static implicit operator ContextualValue<TContext, TValue>(Func<TContext, TValue> factory) => new(factory);
    }
}