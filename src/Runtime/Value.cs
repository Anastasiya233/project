using System;
using System.Globalization;

namespace Alpaca.Runtime;

public class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VoidValue.Value);
    public static readonly Value Nil = new(NilValue.Value);
    private readonly object _value;

    public Value(string value)
    {
        _value = value;
    }

    public Value(int value)
    {
        _value = value;
    }

    private Value(object value)
    {
        _value = value;
    }

    public bool IsVoid()
    {
        return _value is VoidValue;
    }

    public bool IsString()
    {
        return _value is string;
    }

    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {_value} is not a string"),
        };
    }

    public bool IsInt()
    {
        return _value is int;
    }

    public int AsInt()
    {
        return _value switch
        {
            int i => i,
            _ => throw new InvalidOperationException($"Value {_value} is not an integer"),
        };
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int i => i.ToString(CultureInfo.InvariantCulture),
            VoidValue v => v.ToString(),
            NilValue v => v.ToString(),
            _ => throw new InvalidOperationException($"Unexpected value {_value} of type {_value.GetType()}"),
        };
    }

    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        return _value switch
        {
            string s => other.IsString() && other.AsString() == s,
            int i => other.IsInt() && other.AsInt() == i,
            VoidValue => other._value is VoidValue,
            NilValue => other._value is NilValue,
            _ => false,
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}