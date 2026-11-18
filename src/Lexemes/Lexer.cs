using System.Globalization;
using System.Text;

namespace Alpaca.Lexemes;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {
            "main", TokenType.Main
        },
        {
            "array", TokenType.Array
        },
        {
            "function", TokenType.Function
        },
        {
            "return", TokenType.Return
        },
        {
            "if", TokenType.If
        },
        {
            "else", TokenType.Else
        },
        {
            "for", TokenType.For
        },
        {
            "while", TokenType.While
        },
        {
            "break", TokenType.Break
        },
        {
            "nil", TokenType.Nil
        },
        {
            "var", TokenType.Var
        },
        {
            "struct", TokenType.Struct
        },
        {
            "true", TokenType.True
        },
        {
            "false", TokenType.False
        },
        {
            "int", TokenType.Int
        },
        {
            "string", TokenType.String
        },
        {
            "bool", TokenType.Bool
        },
        {
            "void", TokenType.Void
        },
    };

    private static readonly Dictionary<char, char> SimpleEscapes = new()
    {
        {
            'n', '\n'
        },
        {
            't', '\t'
        },
        {
            '"', '\"'
        },
        {
            '\\', '\\'
        },
    };

    private readonly TextScanner _scanner;

    public Lexer(string code)
    {
        _scanner = new TextScanner(code);
    }

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char c = _scanner.Peek();
        if (char.IsAsciiLetter(c))
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseIntLiteral();
        }

        if (c == '"')
        {
            return ParseStringLiteral();
        }

        switch (c)
        {
            case '+':
                _scanner.Advance();
                return new Token(TokenType.Plus);

            case '-':
                _scanner.Advance();
                return new Token(TokenType.Minus);

            case '*':
                _scanner.Advance();
                return new Token(TokenType.Multiply);

            case '/':
                _scanner.Advance();
                return new Token(TokenType.Divide);

            case '%':
                _scanner.Advance();
                return new Token(TokenType.Percent);

            case '^':
                _scanner.Advance();
                return new Token(TokenType.Power);

            case '=':
                _scanner.Advance();
                return new Token(TokenType.Equal);

            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '>')
                {
                    _scanner.Advance();
                    return new Token(TokenType.NotEqual);
                }

                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessThanOrEqual);
                }

                return new Token(TokenType.LessThan);

            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterThanOrEqual);
                }

                return new Token(TokenType.GreaterThan);

            case '&':
                _scanner.Advance();
                return new Token(TokenType.And);

            case '|':
                _scanner.Advance();
                return new Token(TokenType.Or);
            case '!':
                _scanner.Advance();
                return new Token(TokenType.Not);
            case '.':
                _scanner.Advance();
                return new Token(TokenType.Dot);

            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);

            case ':':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Assign);
                }

                return new Token(TokenType.Error, "expected '=' after ':'");

            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);

            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis);

            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis);

            case '[':
                _scanner.Advance();
                return new Token(TokenType.OpenBracket);

            case ']':
                _scanner.Advance();
                return new Token(TokenType.CloseBracket);

            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBrace);

            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBrace);
        }

        _scanner.Advance();
        return new Token(TokenType.Error, c.ToString());
    }

    private Token ParseIntLiteral()
    {
        StringBuilder sb = new();
        for (char ch = _scanner.Peek(); char.IsAsciiDigit(ch); ch = _scanner.Peek())
        {
            sb.Append(ch);
            _scanner.Advance();
        }

        string digits = sb.ToString();
        if (digits.Length > 1 && digits[0] == '0')
        {
            return new Token(TokenType.Error, digits);
        }

        if (int.TryParse(digits, CultureInfo.InvariantCulture, out int value))
        {
            return new Token(TokenType.IntLiteral, value);
        }

        return new Token(TokenType.Error, digits);
    }

    private Token ParseStringLiteral()
    {
        StringBuilder valueBuilder = new();
        bool hasError = false;

        _scanner.Advance();

        for (char c = _scanner.Peek(); c != '"'; c = _scanner.Peek())
        {
            if (_scanner.IsEnd())
            {
                return new Token(TokenType.Error, valueBuilder.ToString());
            }

            if (c == '\\')
            {
                if (!DecodeEscapeSequence(valueBuilder))
                {
                    hasError = true;
                    valueBuilder.Append('\\');
                }
            }
            else
            {
                valueBuilder.Append(c);
                _scanner.Advance();
            }
        }

        _scanner.Advance();

        if (hasError)
        {
            return new Token(TokenType.Error, valueBuilder.ToString());
        }

        return new Token(TokenType.StringLiteral, valueBuilder.ToString());
    }

    private bool DecodeEscapeSequence(StringBuilder valueBuilder)
    {
        _scanner.Advance();

        char ch1 = _scanner.Peek();

        if (SimpleEscapes.TryGetValue(ch1, out char unescaped1))
        {
            _scanner.Advance();
            valueBuilder.Append(unescaped1);
            return true;
        }

        return false;
    }

    private Token ParseIdentifierOrKeyword()
    {
        StringBuilder sb = new();
        sb.Append(_scanner.Peek());
        _scanner.Advance();

        for (char c = _scanner.Peek(); char.IsAsciiLetter(c) || c == '_' || char.IsAsciiDigit(c); c = _scanner.Peek())
        {
            sb.Append(c);
            _scanner.Advance();
        }

        string value = sb.ToString();

        if (Keywords.TryGetValue(value, out TokenType type))
        {
            return new Token(type);
        }

        return new Token(TokenType.Identifier, value);
    }

    private void SkipWhiteSpacesAndComments()
    {
        do
        {
            SkipWhiteSpaces();
        }
        while (SkipComment());
    }

    private void SkipWhiteSpaces()
    {
        while (IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    private static bool IsWhiteSpace(char c)
    {
        return c is ' ' or '\t' or '\n' or '\r' or '\f';
    }

    private bool SkipComment()
    {
        if (_scanner.Peek() != '/' || _scanner.Peek(1) != '*')
        {
            return false;
        }

        _scanner.Advance();
        _scanner.Advance();

        while (!_scanner.IsEnd())
        {
            if (_scanner.Peek() == '*' && _scanner.Peek(1) == '/')
            {
                _scanner.Advance();
                _scanner.Advance();
                return true;
            }

            _scanner.Advance();
        }

        throw new LexerException("Unclosed comment");
    }
}