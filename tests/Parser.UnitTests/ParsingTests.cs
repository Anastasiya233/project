using Alpaca.Lexemes;
using Alpaca.Parsing.Exceptions;
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
            { "printInt", "42", BuiltinFunctionCode.PrintInt },
            { "read", "", BuiltinFunctionCode.Read },
            { "readln", "", BuiltinFunctionCode.Readln },
        };
    }

    [Fact]
    public void ParseFunctionCallThrowsOnUnknownFunction()
    {
        string code = "function main() { foo(42); }";

        Exception ex = Assert.Throws<UnknownFunctionException>(() =>
               {
                   Parser parser = new(code);
                   parser.ParseProgram();
               });

        Assert.Contains("foo", ex.Message);
        Assert.Contains("not supported", ex.Message);
    }

    [Fact]
    public void ParseFunctionCallParsesCommaSeparatedArguments()
    {
        string code = "function main() { foo(1, 2); }";
        Parser parser = new(code);

        Exception ex = Assert.Throws<UnknownFunctionException>(() => parser.ParseProgram());

        Assert.Contains("foo", ex.Message);
        Assert.Contains("not supported", ex.Message);
    }
}