using Alpaca.TestUtilities;

namespace Alpaca.Interpreter.UnitTests;

public class InterpreterTests
{
    [Theory]
    [MemberData(nameof(GetArithmeticCases))]
    public void CanEvaluateArithmetic(string code, string expected)
    {
        FakeEnvironment env = new();
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
        };
    }
}