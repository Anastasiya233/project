using System.Text;

public class FakeEnvironment : IEnvironment
{
    private readonly Queue<int> _input = new();
    private readonly StringBuilder _output = new();

    public string Output => _output.ToString();

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