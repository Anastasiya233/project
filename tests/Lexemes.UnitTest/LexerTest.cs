using Alpaca.TestUtilities;

namespace Alpaca.Lexemes.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetTokenizeIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetTokenizeLiteralsData))]
    [MemberData(nameof(GetSkipWhitespacesAndCommentsData))]
    [MemberData(nameof(GetTokenizePunctuationData))]
    public void CanTokenizeLexemes(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0, iEnd = actual.Count; i < iEnd; ++i)
        {
            Assert.Equivalent(expected[i], actual[i]);
        }
    }

    [Theory]
    [MemberData(nameof(GetLexerErrorData))]
    public void CanReportLexerErrors(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0, iEnd = actual.Count; i < iEnd; ++i)
        {
            Assert.Equivalent(expected[i], actual[i]);
        }
    }

    [Theory]
    [MemberData(nameof(GetUnclosedCommentData))]
    public void CanReportUnclosedComment(string code)
    {
        Lexer lexer = new(code);
        Assert.Throws<LexerException>(() =>
        {
            for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
            {
            }
        });
    }

    public static TheoryData<string, List<Token>> GetTokenizeIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "alice Bob C00L D_2 x", [
                    new Token(TokenType.Identifier, "alice"),
                    new Token(TokenType.Identifier, "Bob"),
                    new Token(TokenType.Identifier, "C00L"),
                    new Token(TokenType.Identifier, "D_2"),
                    new Token(TokenType.Identifier, "x")
                ]
            },
            {
                "main array function return if else for while break nil var struct true false int string bool void", [
                    new Token(TokenType.Main),
                    new Token(TokenType.Array),
                    new Token(TokenType.Function),
                    new Token(TokenType.Return),
                    new Token(TokenType.If),
                    new Token(TokenType.Else),
                    new Token(TokenType.For),
                    new Token(TokenType.While),
                    new Token(TokenType.Break),
                    new Token(TokenType.Nil),
                    new Token(TokenType.Var),
                    new Token(TokenType.Struct),
                    new Token(TokenType.True),
                    new Token(TokenType.False),
                    new Token(TokenType.Int),
                    new Token(TokenType.String),
                    new Token(TokenType.Bool),
                    new Token(TokenType.Void),
                ]
            },
            {
                "mainx iff variable voided int32 true_ nil2", [
                    new Token(TokenType.Identifier, "mainx"),
                    new Token(TokenType.Identifier, "iff"),
                    new Token(TokenType.Identifier, "variable"),
                    new Token(TokenType.Identifier, "voided"),
                    new Token(TokenType.Identifier, "int32"),
                    new Token(TokenType.Identifier, "true_"),
                    new Token(TokenType.Identifier, "nil2"),
                ]
            },
            {
                "Main IF True INT VOID", [
                    new Token(TokenType.Identifier, "Main"),
                    new Token(TokenType.Identifier, "IF"),
                    new Token(TokenType.Identifier, "True"),
                    new Token(TokenType.Identifier, "INT"),
                    new Token(TokenType.Identifier, "VOID"),
                ]
            },
            {
                "function main() { }", [
                    new Token(TokenType.Function),
                    new Token(TokenType.Main),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "while i < 10 { i := i + 1; break; }", [
                    new Token(TokenType.While),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "i"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Break),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "if a = nil then b", [
                    new Token(TokenType.If),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Nil),
                    new Token(TokenType.Identifier, "then"),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
            {
                "1abc", [
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.Identifier, "abc"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "0", [
                    new Token(TokenType.IntLiteral, 0),
                ]
            },
            {
                "1234 56789", [
                    new Token(TokenType.IntLiteral, 1234),
                    new Token(TokenType.IntLiteral, 56789),
                ]
            },
            {
                "10 100 1000", [
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.IntLiteral, 100),
                    new Token(TokenType.IntLiteral, 1000),
                ]
            },
            {
                "2147483647", [
                    new Token(TokenType.IntLiteral, int.MaxValue),
                ]
            },
            {
                "-5", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 5),
                ]
            },
            {
                "- 5", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 5),
                ]
            },
            {
                "-2147483647", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, int.MaxValue),
                ]
            },
            {
                """
                "" "0" "Hello, world!"
                """,
                [
                    new Token(TokenType.StringLiteral, ""),
                    new Token(TokenType.StringLiteral, "0"),
                    new Token(TokenType.StringLiteral, "Hello, world!"),
                ]
            },
            {
                "\"  a  b  \"", [
                    new Token(TokenType.StringLiteral, "  a  b  "),
                ]
            },
            {
                "\"a;b{c}(d)[e]\"", [
                    new Token(TokenType.StringLiteral, "a;b{c}(d)[e]"),
                ]
            },
            {
                "\"\\n\\t\\\"\\\\\"", [
                    new Token(TokenType.StringLiteral, "\n\t\"\\"),
                ]
            },
            {
                "x := \"line1\\nline2\"", [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, "line1\nline2"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetSkipWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x \t\r\n\fy", [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },
            {
                "/* comments */ a / /* should be */ b * c /* ignored */ ", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "/**/ a", [
                    new Token(TokenType.Identifier, "a"),
                ]
            },
            {
                "/* if else while */ x", [
                    new Token(TokenType.Identifier, "x"),
                ]
            },
            {
                "a /* a * b ** c */ b", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
            {
                "/* /* */ a", [
                    new Token(TokenType.Identifier, "a"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizePunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "+ - * / % ^ = < > . , ; ( ) [ ] { } & | !", [
                    new Token(TokenType.Plus),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Divide),
                    new Token(TokenType.Percent),
                    new Token(TokenType.Power),
                    new Token(TokenType.Equal),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.Dot),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.OpenBracket),
                    new Token(TokenType.CloseBracket),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.CloseBrace),
                    new Token(TokenType.And),
                    new Token(TokenType.Or),
                    new Token(TokenType.Not),
                ]
            },
            {
                "<> <= >= :=", [
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.Assign),
                ]
            },
            {
                "a < b | a > c | b = c", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "a <= b & b >= c & a <> c", [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "x - 5", [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 5),
                ]
            },
            {
                "x - -5", [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 5),
                ]
            },
            {
                "-x", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "x"),
                ]
            },
            {
                "-(5)", [
                    new Token(TokenType.Minus),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.IntLiteral, 5),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                "speed.x := v[0]", [
                    new Token(TokenType.Identifier, "speed"),
                    new Token(TokenType.Dot),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Identifier, "v"),
                    new Token(TokenType.OpenBracket),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseBracket),
                ]
            },
            {
                "struct Point { int x; int y; }", [
                    new Token(TokenType.Struct),
                    new Token(TokenType.Identifier, "Point"),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Int),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Int),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.CloseBrace),
                ]
            },
            {
                "(foo(x);0)", [
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "foo"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetLexerErrorData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "007", [
                    new Token(TokenType.Error, "007"),
                ]
            },
            {
                "00", [
                    new Token(TokenType.Error, "00"),
                ]
            },
            {
                "0123", [
                    new Token(TokenType.Error, "0123"),
                ]
            },
            {
                "0017", [
                    new Token(TokenType.Error, "0017"),
                ]
            },
            {
                "2147483648", [
                    new Token(TokenType.Error, "2147483648"),
                ]
            },
            {
                "99999999999999999999", [
                    new Token(TokenType.Error, "99999999999999999999"),
                ]
            },
            {
                "\"abc", [
                    new Token(TokenType.Error, "abc"),
                ]
            },
            {
                "\"", [
                    new Token(TokenType.Error, ""),
                ]
            },
            {
                "\"\\x\"", [
                    new Token(TokenType.Error, "\\x"),
                ]
            },
            {
                ":", [
                    new Token(TokenType.Error, "expected '=' after ':'"),
                ]
            },
            {
                "@ # $", [
                    new Token(TokenType.Error, "@"),
                    new Token(TokenType.Error, "#"),
                    new Token(TokenType.Error, "$"),
                ]
            },
            {
                "привет", [
                    new Token(TokenType.Error, "п"),
                    new Token(TokenType.Error, "р"),
                    new Token(TokenType.Error, "и"),
                    new Token(TokenType.Error, "в"),
                    new Token(TokenType.Error, "е"),
                    new Token(TokenType.Error, "т"),
                ]
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetStringLiteralTokenizationData))]
    public void TokenizesStringLiterals(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0, iEnd = actual.Count; i < iEnd; ++i)
        {
            Assert.Equivalent(expected[i], actual[i]);
        }
    }

    [Theory]
    [MemberData(nameof(GetRecordingEnvironmentData))]
    public void RecordingEnvironmentCapturesOutputAndReplaysInput(string text, int[] inputCodes)
    {
        RecordingEnvironment env = new();
        env.SetInput(inputCodes);

        env.Print(text);
        env.PrintInt(42);
        env.Flush();

        Assert.Equal(text + "42", env.Output);
        foreach (int expectedCode in inputCodes)
        {
            Assert.Equal(expectedCode, env.ReadChar());
        }

        // После исчерпания входных данных читается -1 (признак конца потока).
        Assert.Equal(-1, env.ReadChar());
    }

    public static TheoryData<string, List<Token>> GetStringLiteralTokenizationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "\"\"", [
                    new Token(TokenType.StringLiteral, ""),
                ]
            },
            {
                "\"abc\"", [
                    new Token(TokenType.StringLiteral, "abc"),
                ]
            },
            {
                "\"Hello, world!\"", [
                    new Token(TokenType.StringLiteral, "Hello, world!"),
                ]
            },
            {
                @"""tab\tnewline\nquote\""slash\"""", [
                    new Token(TokenType.StringLiteral, "tab\tnewline\nquote\"slash\\"),
                ]
            },
            {
                "\"a\" \"b\"", [
                    new Token(TokenType.StringLiteral, "a"),
                    new Token(TokenType.StringLiteral, "b"),
                ]
            },
        };
    }

    public static TheoryData<string, int[]> GetRecordingEnvironmentData()
    {
        return new TheoryData<string, int[]>
        {
            { "hello", [104, 101, 108, 108, 111] },
            { "", Array.Empty<int>() },
            { "\n", [10] },
        };
    }

    public static TheoryData<string> GetUnclosedCommentData()
    {
        return new TheoryData<string>
        {
            { "/* abc" },
            { "a /*" },
            { "a /* c */ b /* d" },
            { "a /* comment" },
        };
    }

    private static List<Token> Tokenize(string code)
    {
        List<Token> results = [];
        Lexer lexer = new(code);
        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}