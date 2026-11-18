
using Alpaca.Parsing.Exceptions;
using Alpaca.TestUtilities;
using Alpaca.VirtualMachine;

namespace Alpaca.Interpreter.UnitTests;

public class InterpreterTests
{
    [Theory]
    [MemberData(nameof(GetArithmeticCases))]
    public void CanEvaluateArithmetic(string code, string expected)
    {
        RecordingEnvironment env = new();
        AlpacaInterpreter interpreter = new(env);
        interpreter.Execute(code);
        Assert.Equal(expected, env.Output.Trim());
    }

    public static TheoryData<string, string> GetArithmeticCases()
    {
        return new TheoryData<string, string>
        {
            { "function main() { printInt(10 + 7 * (2 * 3 + 4) + 5 + -5); }", "80" },
            { "function main() { printInt(4 ^ 3 ^ 2); }", "262144" },
            { "function main() { printInt(10 - 10 / 2); printInt(10 % 3); }", "51" },
            { "function main() { printInt(-2 ^ 2); }", "4" },
            { "function main() { printInt(-(2 ^ 2)); }", "-4" },
            { "function main() { printInt(3 ^ 19); }", "1162261467" },
        };
    }

    [Fact]
    public void ExecutePropagatesSyntaxError()
    {
        AlpacaInterpreter interpreter = new(new RecordingEnvironment());

        Assert.Throws<UnexpectedLexemeException>(() => interpreter.Execute("function main() { + 2; }"));
    }

    [Fact]
    public void ExecutePropagatesUnknownFunctionError()
    {
        AlpacaInterpreter interpreter = new(new RecordingEnvironment());

        UnknownFunctionException ex = Assert.Throws<UnknownFunctionException>(
            () => interpreter.Execute("function main() { foo(1); }"));
        Assert.Equal("foo", ex.FunctionName);
    }

    [Fact]
    public void ExecutePropagatesDivideByZeroError()
    {
        AlpacaInterpreter interpreter = new(new RecordingEnvironment());

        Assert.Throws<DivideByZeroException>(() => interpreter.Execute("function main() { printInt(1 / 0); }"));
    }

    [Fact]
    public void MainReturnsNonZeroExitCodeAndWritesToStderrOnSyntaxError()
    {
        RunProgramScenario("function main() { + 2; }", out int exitCode, out string stdout, out string stderr);

        Assert.Equal(1, exitCode);
        Assert.Contains("UnexpectedLexemeException", stderr);
        Assert.Equal(string.Empty, stdout);
    }

    [Fact]
    public void MainReturnsNonZeroExitCodeAndWritesToStderrOnDivideByZero()
    {
        RunProgramScenario("function main() { printInt(1 / 0); }", out int exitCode, out string stdout, out string stderr);

        Assert.Equal(1, exitCode);
        Assert.Contains("Division by zero", stderr);
        Assert.Equal(string.Empty, stdout);
    }

    [Fact]
    public void MainReturnsZeroExitCodeForValidProgram()
    {
        RunProgramScenario("function main() { printInt(2 + 3); }", out int exitCode, out string stdout, out string stderr);

        Assert.Equal(0, exitCode);
        Assert.Equal("5", stdout.Trim());
        Assert.Equal(string.Empty, stderr);
    }

    [Fact]
    public void MainReturnsNonZeroExitCodeWhenFileNotFound()
    {
        using TestConsole console = new();
        int exitCode = Program.Main([Path.GetTempFileName() + ".missing"]);

        Assert.Equal(1, exitCode);
        Assert.Contains("not found", console.Error.ToString());
    }

    private static void RunProgramScenario(string sourceCode, out int exitCode, out string stdout, out string stderr)
    {
        string tempFile = Path.Combine(Path.GetTempPath(), $"alpaca-{Guid.NewGuid():N}.alp");
        File.WriteAllText(tempFile, sourceCode);
        try
        {
            using TestConsole console = new();
            exitCode = Program.Main([tempFile]);
            stdout = console.Out.ToString();
            stderr = console.Error.ToString();
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
