using Alpaca.Runtime;
using Alpaca.VirtualMachine.Builtins;
using Alpaca.VirtualMachine.Instructions;

namespace Alpaca.VirtualMachine;

public class AlpacaVM
{
    private readonly BuiltinFunctions _builtinFunctions;
    private readonly IReadOnlyList<Instruction> _instructions;

    /// <summary>
    /// Указатель на текущую инструкцию.
    /// </summary>
    private int _instructionPointer;

    /// <summary>
    /// Код завершения программы.
    /// </summary>
    private int _exitCode;

    /// <summary>
    /// Стек для вычисления выражений.
    /// </summary>
    private readonly Stack<Value> _evaluationStack;

    /// <summary>
    /// Результат работы программы.
    /// </summary>
    private Value _result;

    public AlpacaVM(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);

        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _result = Value.Void;
    }

    public int ExitCode => _exitCode;

    public Value RunProgram()
    {
        while (true)
        {
            Instruction instruction = _instructions[_instructionPointer++];
            switch (instruction.Code)
            {
                case InstructionCode.Push:
                    _evaluationStack.Push(instruction.Operand);
                    break;

                case InstructionCode.Pop:
                    _evaluationStack.Pop();
                    break;

                case InstructionCode.Add:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.AsInt() + right.AsInt()));
                    }

                    break;

                case InstructionCode.Subtract:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.AsInt() - right.AsInt()));
                    }

                    break;

                case InstructionCode.Multiply:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.AsInt() * right.AsInt()));
                    }

                    break;

                case InstructionCode.Divide:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        if (right.AsInt() == 0)
                        {
                            throw new DivideByZeroException("Division by zero");
                        }

                        _evaluationStack.Push(new Value(left.AsInt() / right.AsInt()));
                    }

                    break;

                case InstructionCode.Modulo:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        if (right.AsInt() == 0)
                        {
                            throw new DivideByZeroException("Modulo by zero");
                        }

                        _evaluationStack.Push(new Value(left.AsInt() % right.AsInt()));
                    }

                    break;

                case InstructionCode.Power:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value((int)Math.Pow(left.AsInt(), right.AsInt())));
                    }

                    break;

                case InstructionCode.Negate:
                    {
                        Value operand = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(-operand.AsInt()));
                    }

                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand.AsInt());
                    break;

                case InstructionCode.Halt:
                    _result = _evaluationStack.Count > 0 ? _evaluationStack.Pop() : Value.Void;
                    _exitCode = 0;
                    return _result;

                default:
                    throw new NotImplementedException($"Unsupported instruction code: {instruction.Code}");
            }
        }
    }

    /// <summary>
    /// Выполняет вызов встроенной функции по её имени.
    /// </summary>
    private void CallBuiltin(BuiltinFunctionCode code)
    {
        switch (code)
        {
            case BuiltinFunctionCode.Print:
                _builtinFunctions.Print(_evaluationStack.Pop());
                _evaluationStack.Push(Value.Void);
                break;

            case BuiltinFunctionCode.Println:
                _builtinFunctions.Println(_evaluationStack.Pop());
                _evaluationStack.Push(Value.Void);
                break;

            case BuiltinFunctionCode.PrintInt:
                _builtinFunctions.PrintInt(_evaluationStack.Pop());
                _evaluationStack.Push(Value.Void);
                break;

            case BuiltinFunctionCode.Read:
                _evaluationStack.Push(_builtinFunctions.Read());
                break;

            case BuiltinFunctionCode.Readln:
                _evaluationStack.Push(_builtinFunctions.Readln());
                break;

            default:
                throw new ArgumentException($"Unknown builtin function code: {code}");
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
        {
            throw new InvalidOperationException("Invalid empty VM program");
        }

        InstructionCode lastInstructionCode = instructions[^1].Code;
        if (lastInstructionCode != InstructionCode.Halt)
        {
            throw new InvalidOperationException(
                $"Last instruction must be {InstructionCode.Halt}, got {lastInstructionCode}"
            );
        }
    }
}