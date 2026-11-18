using Alpaca.Runtime;
using Alpaca.TestUtilities;
using Alpaca.VirtualMachine;
using Alpaca.VirtualMachine.Builtins;
using Alpaca.VirtualMachine.Instructions;

namespace Alpaca.VM.UnitTests;

public class VMTests
{
    [Fact]
    public void PushAddValueToStack()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal(10, result.AsInt());
    }

    [Fact]
    public void PopRemovesTopValue()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(20)),
            new Instruction(InstructionCode.Pop),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal(10, result.AsInt());
    }

    [Fact]
    public void AddPushesSum()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(5)),
            new Instruction(InstructionCode.Add),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(15, vm.RunProgram().AsInt());
    }

    [Fact]
    public void SubtractPushesDifference()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(3)),
            new Instruction(InstructionCode.Subtract),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(7, vm.RunProgram().AsInt());
    }

    [Fact]
    public void MultiplyPushesProduct()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(6)),
            new Instruction(InstructionCode.Push, new Value(7)),
            new Instruction(InstructionCode.Multiply),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(42, vm.RunProgram().AsInt());
    }

    [Fact]
    public void DividePushesQuotient()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(2)),
            new Instruction(InstructionCode.Divide),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(5, vm.RunProgram().AsInt());
    }

    [Fact]
    public void DivideZeroByNonZeroReturnsZero()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(0)),
            new Instruction(InstructionCode.Push, new Value(2)),
            new Instruction(InstructionCode.Divide),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(0, vm.RunProgram().AsInt());
    }

    [Fact]
    public void DivideByZeroThrows()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(0)),
            new Instruction(InstructionCode.Divide),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Throws<DivideByZeroException>(() => vm.RunProgram());
    }

    [Fact]
    public void ModuloPushesRemainder()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(3)),
            new Instruction(InstructionCode.Modulo),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(1, vm.RunProgram().AsInt());
    }

    [Fact]
    public void ModuloByZeroThrows()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.Push, new Value(0)),
            new Instruction(InstructionCode.Modulo),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Throws<DivideByZeroException>(() => vm.RunProgram());
    }

    [Fact]
    public void PowerPushesResult()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(2)),
            new Instruction(InstructionCode.Push, new Value(3)),
            new Instruction(InstructionCode.Power),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(8, vm.RunProgram().AsInt());
    }

    [Theory]
    [MemberData(nameof(GetPowerCases))]
    public void PowerComputesExactIntegerResult(int baseValue, int exponent, int expected)
    {
        // Возведение в степень должно считаться точным целочисленным алгоритмом:
        // Math.Pow(3, 19) при усечении до int даёт 1162261466 вместо 1162261467.
        RecordingEnvironment env = new();
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(baseValue)),
            new Instruction(InstructionCode.Push, new Value(exponent)),
            new Instruction(InstructionCode.Power),
            new Instruction(InstructionCode.Halt),
        ];

        AlpacaVM vm = new(env, program);
        Assert.Equal(expected, vm.RunProgram().AsInt());
    }

    public static TheoryData<int, int, int> GetPowerCases()
    {
        return new TheoryData<int, int, int>
        {
            { 2, 10, 1024 },
            { 3, 19, 1162261467 },
            { 5, 13, 1220703125 },
            { 7, 10, 282475249 },
            { 2, 0, 1 },
            { 1, 19, 1 },
            { 0, 5, 0 },
            { -2, 3, -8 },
            { -3, 2, 9 },
        };
    }

    [Fact]
    public void PowerNegativeExponentThrows()
    {
        RecordingEnvironment env = new();
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(2)),
            new Instruction(InstructionCode.Push, new Value(-1)),
            new Instruction(InstructionCode.Power),
            new Instruction(InstructionCode.Halt),
        ];

        AlpacaVM vm = new(env, program);
        Assert.Throws<ArgumentOutOfRangeException>(() => vm.RunProgram());
    }

    [Fact]
    public void NegateChangesSign()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(5)),
            new Instruction(InstructionCode.Negate),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Equal(-5, vm.RunProgram().AsInt());
    }

    [Fact]
    public void RunProgramUnknownInstructionThrows()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
    {
        new Instruction((InstructionCode)999, new Value(10)),
        new Instruction(InstructionCode.Halt),
    };

        AlpacaVM vm = new(env, program);
        Assert.Throws<NotImplementedException>(() => vm.RunProgram());
    }

    [Fact]
    public void CallBuiltinPrintIntOutputsValue()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value(42)),
            new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintInt)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        vm.RunProgram();

        Assert.Equal("42", env.Output.Trim());
    }

    [Fact]
    public void CallBuiltinUnknownFunctionThrows()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.CallBuiltin, new Value(999)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Assert.Throws<ArgumentException>(() => vm.RunProgram());
    }

    [Fact]
    public void CallBuiltinPrintOutputsString()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value("hello")),
            new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Print)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal("hello", env.Output);
    }

    [Fact]
    public void CallBuiltinPrintlnOutputsStringWithNewline()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Push, new Value("hello")),
            new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Println)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal("hello\n", env.Output);
    }

    [Fact]
    public void CallBuiltinReadPushesCharCode()
    {
        RecordingEnvironment env = new();
        env.SetInput(65);   // 'A'

        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Read)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal(65, result.AsInt());
    }

    [Fact]
    public void CallBuiltinReadAtEndReturnsMinusOne()
    {
        RecordingEnvironment env = new();

        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Read)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal(-1, result.AsInt());
    }

    [Fact]
    public void CallBuiltinReadlnPushesString()
    {
        RecordingEnvironment env = new();
        env.SetInput(104, 101, 108, 108, 111, 10);   // "hello\n"

        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Readln)),
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.Equal("hello", result.AsString());
    }

    [Fact]
    public void HaltWithEmptyStackReturnsVoid()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new()
        {
            new Instruction(InstructionCode.Halt),
        };

        AlpacaVM vm = new(env, program);
        Value result = vm.RunProgram();

        Assert.True(result.IsVoid());
    }

    [Fact]
    public void ConstructorEmptyProgramThrows()
    {
        RecordingEnvironment env = new();
        List<Instruction> program = new();

        Assert.Throws<InvalidOperationException>(() => new AlpacaVM(env, program));
    }
}