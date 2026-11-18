using Alpaca.Lexemes;

namespace Alpaca.Parsing.Exceptions;

#pragma warning disable RCS1194
public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(Token actual, TokenType expected)
        : base($"Unexpected lexeme {actual} where expected {expected}")
    {
        Actual = actual.Type;
        Expected = [expected];
    }

    public UnexpectedLexemeException(Token actual, List<TokenType> expected)
        : base($"Unexpected lexeme {actual} where expected one of {string.Join(", ", expected)}")
    {
        Actual = actual.Type;
        Expected = expected;
    }

    public TokenType Actual { get; }

    public List<TokenType> Expected { get; }
}
#pragma warning restore RCS1194