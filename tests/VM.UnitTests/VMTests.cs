using Alpaca.Runtime;
using Alpaca.TestUtilities;
using Alpaca.VirtualMachine;
using Alpaca.VirtualMachine.Builtins;
using Alpaca.VirtualMachine.Instructions;

namespace VM.UnitTests;

public class VMTests
{
    [Fact]
    public void PushAddValueToStack()
    {
        FakeEnvironment env = new FakeEnvironment();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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

    [Fact]
    public void PowerThreeToNineteenReturnsExactValue()
    {
        FakeEnvironment env = new();
        List<Instruction> program = new()
    {
        new Instruction(InstructionCode.Push, new Value(3)),
        new Instruction(InstructionCode.Push, new Value(19)),
        new Instruction(InstructionCode.Power),
        new Instruction(InstructionCode.Halt),
    };

        AlpacaVM vm = new(env, program);
        Assert.Equal(1162261467, vm.RunProgram().AsInt());
    }

    [Fact]
    public void NegateChangesSignTo()
    {
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();

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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
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
        FakeEnvironment env = new();
        List<Instruction> program = new();

        Assert.Throws<InvalidOperationException>(() => new AlpacaVM(env, program));
    }
}