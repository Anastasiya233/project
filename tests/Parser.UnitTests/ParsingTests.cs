using Alpaca.Lexemes;
using Alpaca.Parsing.Exceptions;
using Alpaca.Runtime;
using Alpaca.VirtualMachine.Builtins;
using Alpaca.VirtualMachine.Instructions;

namespace Alpaca.Parsing.UnitTests;

public class ParsingTests
{
    [Fact]
    public void PeekReturnsTokensByOffset()
    {
        TokenStream stream = new("function main() { }");

        Assert.Equal(TokenType.Function, stream.Peek(0).Type);
        Assert.Equal(TokenType.Main, stream.Peek(1).Type);
        Assert.Equal(TokenType.OpenParenthesis, stream.Peek(2).Type);
    }

    [Fact]
    public void ParseProgramThrowsWhenFunctionKeywordMissing()
    {
        string code = "main() { }";

        UnexpectedLexemeException ex = Assert.Throws<UnexpectedLexemeException>(() =>
        {
            Parser parser = new(code);
            parser.ParseProgram();
        });

        Assert.Equal(TokenType.Main, ex.Actual);
        Assert.Contains(TokenType.Function, ex.Expected);
    }

    [Fact]
    public void ParsePrimaryExpressionThrowsWithCorrectInfo()
    {
        string code = "function main() { + 2; }";

        UnexpectedLexemeException ex = Assert.Throws<UnexpectedLexemeException>(() =>
               {
                   Parser parser = new(code);
                   parser.ParseProgram();
               });

        Assert.Equal(TokenType.Plus, ex.Actual);
        Assert.Contains(TokenType.IntLiteral, ex.Expected);
    }

    [Theory]
    [MemberData(nameof(GetBuiltinCases))]
    public void ParseFunctionCallRecognizesBuiltin(string name, string args, BuiltinFunctionCode expectedCode)
    {
        string code = $"function main() {{ {name}({args}); }}";

        Parser parser = new(code);
        List<Instruction> instructions = parser.ParseProgram();

        Assert.Contains(instructions, i =>
                   i.Code == InstructionCode.CallBuiltin &&
                   i.Operand.AsInt() == (int)expectedCode);
    }

    public static TheoryData<string, string, BuiltinFunctionCode> GetBuiltinCases()
    {
        return new TheoryData<string, string, BuiltinFunctionCode>
        {
            { "print", "\"hi\"", BuiltinFunctionCode.Print },
            { "println", "\"hi\"", BuiltinFunctionCode.Println },
            { "printInt", "42", BuiltinFunctionCode.PrintInt },
            { "read", "", BuiltinFunctionCode.Read },
            { "readln", "", BuiltinFunctionCode.Readln },
        };
    }

    [Fact]
    public void ParseFunctionCallThrowsOnUnknownFunction()
    {
        string code = "function main() { foo(42); }";

        UnknownFunctionException ex = Assert.Throws<UnknownFunctionException>(() =>
               {
                   Parser parser = new(code);
                   parser.ParseProgram();
               });

        Assert.Equal("foo", ex.FunctionName);
        Assert.Contains("foo", ex.Message);
        Assert.Contains("not supported", ex.Message);
    }

    [Fact]
    public void ParseFunctionCallParsesCommaSeparatedArguments()
    {
        // Проверка правила expression_list = expression { "," expression } :
        // вызов printInt с двумя аргументами должен быть разобран без ошибок,
        // и для каждого аргумента должна быть создана инструкция Push.
        string code = "function main() { printInt(1, 2); }";
        Parser parser = new(code);

        List<Instruction> instructions = parser.ParseProgram();

        Assert.Equal(
            new List<(InstructionCode Code, int? Operand)>
            {
                (InstructionCode.Push, 1),
                (InstructionCode.Push, 2),
                (InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintInt),
                (InstructionCode.Pop, null),
                (InstructionCode.Halt, null),
            },
            instructions.Select(i => (i.Code, i.Operand.IsInt() ? (int?)i.Operand.AsInt() : null)).ToList());
    }

    [Fact]
    public void ParseFunctionCallParsesThreeCommaSeparatedExpressions()
    {
        string code = "function main() { printInt(1 + 2, 3 * 4, -5); }";
        Parser parser = new(code);

        List<Instruction> instructions = parser.ParseProgram();

        Assert.Equal(
            new List<InstructionCode>
            {
                InstructionCode.Push,
                InstructionCode.Push,
                InstructionCode.Add,
                InstructionCode.Push,
                InstructionCode.Push,
                InstructionCode.Multiply,
                InstructionCode.Push,
                InstructionCode.Negate,
                InstructionCode.CallBuiltin,
                InstructionCode.Pop,
                InstructionCode.Halt,
            },
            instructions.Select(i => i.Code).ToList());
    }

    [Theory]
    [MemberData(nameof(GetStringLiteralCases))]
    public void ParsePrimaryExpressionAcceptsStringLiteral(string text, string expectedValue)
    {
        string code = $"function main() {{ print({text}); }}";
        Parser parser = new(code);

        List<Instruction> instructions = parser.ParseProgram();

        Assert.Equal(InstructionCode.Push, instructions[0].Code);
        Assert.True(instructions[0].Operand.IsString());
        Assert.Equal(expectedValue, instructions[0].Operand.AsString());
    }

    public static TheoryData<string, string> GetStringLiteralCases()
    {
        return new TheoryData<string, string>
        {
            { "\"\"", string.Empty },
            { "\"Hello, world!\"", "Hello, world!" },
            { @"""tab\tnewline\nquote\"""", "tab\tnewline\nquote\"" },
        };
    }
}
