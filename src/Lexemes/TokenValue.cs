using System.Globalization;

namespace Alpaca.Lexemes;

public class TokenValue
{
    private readonly object _value;

    public TokenValue(string value)
    {
        _value = value;
    }

    public TokenValue(int value)
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int d => d.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotImplementedException(),
        };
    }

    public int ToInt()
    {
        return _value switch
        {
            string s => int.Parse(s, CultureInfo.InvariantCulture),
            int i => i,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return _value switch
            {
                string s => other._value is string os && s == os,
                int i => other._value is int oi && i == oi,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}