namespace Alpaca.Runtime;

public record struct NilValue
{
    public static readonly NilValue Value = default;

    public override string ToString()
    {
        return "nil";
    }
}