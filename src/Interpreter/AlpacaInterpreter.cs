using Alpaca.Parsing;
using Alpaca.Runtime;
using Alpaca.VirtualMachine;
using Alpaca.VirtualMachine.Instructions;

namespace Alpaca.Interpreter;

public class AlpacaInterpreter
{
    private readonly IEnvironment _environment;
    private int _exitCode;

    public AlpacaInterpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    public int ExitCode => _exitCode;

    public Value Execute(string code)
    {
        Parser parser = new(code);
        List<Instruction> instructions = parser.ParseProgram();

        AlpacaVM vm = new(_environment, instructions);
        Value result = vm.RunProgram();
        _exitCode = vm.ExitCode;

        return result;
    }
}