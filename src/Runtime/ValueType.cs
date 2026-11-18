namespace Alpaca.Runtime;

public class ValueType
{
    public static readonly ValueType Void = new("void");

    public static readonly ValueType Int = new("int");

    public static readonly ValueType String = new("string");

    public static readonly ValueType Nil = new("nil");

    private readonly string _name;

    protected ValueType(string name)
    {
        _name = name;
    }

    public override string ToString()
    {
        return _name;
    }
}