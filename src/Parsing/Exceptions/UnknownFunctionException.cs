namespace Alpaca.Parsing.Exceptions;

#pragma warning disable RCS1194
public class UnknownFunctionException : Exception
{
    public UnknownFunctionException(string functionName)
        : base($"Function '{functionName}' is not supported.")
    {
        FunctionName = functionName;
    }

    public string FunctionName { get; }
}