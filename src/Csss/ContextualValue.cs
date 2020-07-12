using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Csss
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
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

        public TValue Get(TContext context) => _factory == default ? _value : _factory(context);

        public static implicit operator ContextualValue<TContext, TValue>(TValue value) => new(value);

        public static implicit operator ContextualValue<TContext, TValue>(Func<TContext, TValue> factory) => new(factory);

        public static implicit operator ContextualValue<TContext, string>(ContextualValue<TContext, TValue> value)
        => value._factory == null ?
            (new(value._value!.ToString()!)) :
            new ContextualValue<TContext, string>(context => value._factory(context)!.ToString()!);

        private string GetDebuggerDisplay() => ToString();

        public override string ToString() => _factory == null ? "<contextual>" : _value?.ToString() ?? string.Empty;
    }
}