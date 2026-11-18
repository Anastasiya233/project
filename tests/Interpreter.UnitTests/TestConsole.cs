using System.Text;

namespace Alpaca.Interpreter.UnitTests;

/// <summary>
/// Временная подмена <see cref="Console.Out"/>/<see cref="Console.Error"/> для проверки
/// вывода точечного входа программы в тестах.
/// </summary>
internal sealed class TestConsole : IDisposable
{
    private readonly TextWriter _originalOut = Console.Out;
    private readonly TextWriter _originalError = Console.Error;

    public StringWriter Out { get; } = new();
    public StringWriter Error { get; } = new();

    public TestConsole()
    {
        Console.SetOut(Out);
        Console.SetError(Error);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOut);
        Console.SetError(_originalError);
        Out.Dispose();
        Error.Dispose();
    }
}
