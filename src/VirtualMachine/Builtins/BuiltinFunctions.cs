using System.Text;

using Alpaca.Runtime;

namespace Alpaca.VirtualMachine.Builtins;

public class BuiltinFunctions
{
    private readonly IEnvironment _environment;

    private static readonly Dictionary<string, BuiltinFunctionCode> FunctionsByName = new()
    {
        ["print"] = BuiltinFunctionCode.Print,
        ["println"] = BuiltinFunctionCode.Println,
        ["printInt"] = BuiltinFunctionCode.PrintInt,
        ["read"] = BuiltinFunctionCode.Read,
        ["readln"] = BuiltinFunctionCode.Readln,
    };

    public BuiltinFunctions(IEnvironment environment)
    {
        _environment = environment;
    }

    public static bool TryGetCode(string name, out BuiltinFunctionCode code)
    {
        return FunctionsByName.TryGetValue(name, out code);
    }

    public void Print(Value value)
    {
        _environment.Print(value.ToString());
    }

    public void Println(Value value)
    {
        _environment.Print(value.ToString());
        _environment.Print("\n");
        _environment.Flush();
    }

    public void PrintInt(Value value)
    {
        _environment.PrintInt(value.AsInt());
    }

    public Value Read()
    {
        int code = _environment.ReadChar();
        return new Value(code);
    }

    public Value Readln()
    {
        StringBuilder sb = new();
        while (true)
        {
            int code = _environment.ReadChar();
            if (code == -1 || code == '\n')
            {
                break;
            }

            sb.Append((char)code);
        }

        return new Value(sb.ToString());
    }
}