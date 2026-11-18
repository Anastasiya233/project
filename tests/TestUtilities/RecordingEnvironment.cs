using System.Text;

using Alpaca.VirtualMachine;

namespace Alpaca.TestUtilities;

/// <summary>
/// Тестовая реализация <see cref="IEnvironment"/>: пишет вывод в строку,
/// читает ввод из заранее подготовленной очереди символов.
/// </summary>
public class RecordingEnvironment : IEnvironment
{
    private readonly Queue<int> _input = new();
    private readonly StringBuilder _output = new();

    /// <summary>
    /// Всё, что программа напечатала в стандартный поток вывода.
    /// </summary>
    public string Output => _output.ToString();

    /// <summary>
    /// Кладёт в поток ввода коды символов, которые будет возвращать <see cref="ReadChar"/>.
    /// </summary>
    public void SetInput(params int[] chars)
    {
        foreach (int c in chars)
        {
            _input.Enqueue(c);
        }
    }

    public int ReadChar()
    {
        return _input.Count > 0
            ? _input.Dequeue()
            : -1;
    }

    public void Print(string text) => _output.Append(text);

    public void PrintInt(int value) => _output.Append(value);

    public void Flush()
    {
    }
}
