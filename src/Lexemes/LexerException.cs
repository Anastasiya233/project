namespace Alpaca.Lexemes;

public class LexerException : Exception
{
    public LexerException(string message)
        : base(message)
    {
    }

    public LexerException()
        : base()
    {
    }

    public LexerException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}