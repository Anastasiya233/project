using Alpaca.Lexemes;
using Alpaca.Parsing.Exceptions;
using Alpaca.Runtime;
using Alpaca.VirtualMachine.Builtins;
using Alpaca.VirtualMachine.Instructions;

namespace Alpaca.Parsing;

public class Parser
{
    private readonly TokenStream _tokens;
    private readonly List<Instruction> _instructions = [];

    public Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    public List<Instruction> ParseProgram()
    {
        Match(TokenType.Function);
        Match(TokenType.Main);
        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);

        ParseBlock();
        Match(TokenType.EndOfFile);
        _instructions.Add(new Instruction(InstructionCode.Halt));

        return _instructions;
    }

    private void ParseBlock()
    {
        Match(TokenType.OpenBrace);
        while (_tokens.Peek().Type != TokenType.CloseBrace && _tokens.Peek().Type != TokenType.EndOfFile)
        {
            ParseStatement();
        }

        Match(TokenType.CloseBrace);
    }

    private void ParseStatement()
    {
        ParseExpression();
        Match(TokenType.Semicolon);
        _instructions.Add(new Instruction(InstructionCode.Pop));
    }

    private void ParseExpression()
    {
        ParseAdditiveExpression();
    }

    private void ParseAdditiveExpression()
    {
        ParseMultiplicativeExpression();
        while (true)
        {
            TokenType type = _tokens.Peek().Type;
            if (type == TokenType.Plus)
            {
                _tokens.Advance();
                ParseMultiplicativeExpression();
                _instructions.Add(new Instruction(InstructionCode.Add));
            }
            else if (type == TokenType.Minus)
            {
                _tokens.Advance();
                ParseMultiplicativeExpression();
                _instructions.Add(new Instruction(InstructionCode.Subtract));
            }
            else
            {
                break;
            }
        }
    }

    private void ParseMultiplicativeExpression()
    {
        ParsePowerExpression();
        while (true)
        {
            TokenType type = _tokens.Peek().Type;
            if (type == TokenType.Multiply)
            {
                _tokens.Advance();
                ParsePowerExpression();
                _instructions.Add(new Instruction(InstructionCode.Multiply));
            }
            else if (type == TokenType.Divide)
            {
                _tokens.Advance();
                ParsePowerExpression();
                _instructions.Add(new Instruction(InstructionCode.Divide));
            }
            else if (type == TokenType.Percent)
            {
                _tokens.Advance();
                ParsePowerExpression();
                _instructions.Add(new Instruction(InstructionCode.Modulo));
            }
            else
            {
                break;
            }
        }
    }

    private void ParsePowerExpression()
    {
        ParseUnaryExpression();

        if (_tokens.Peek().Type == TokenType.Power)
        {
            _tokens.Advance();
            ParsePowerExpression();
            _instructions.Add(new Instruction(InstructionCode.Power));
        }
    }

    private void ParseUnaryExpression()
    {
        if (_tokens.Peek().Type == TokenType.Minus)
        {
            _tokens.Advance();
            ParseUnaryExpression();
            _instructions.Add(new Instruction(InstructionCode.Negate));
        }
        else
        {
            ParsePrimaryExpression();
        }
    }

    private void ParsePrimaryExpression()
    {
        Token t = _tokens.Peek();
        switch (t.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                _instructions.Add(new Instruction(InstructionCode.Push, new Value(t.Value!.ToInt())));
                break;

            case TokenType.OpenParenthesis:
                _tokens.Advance();
                ParseExpression();
                Match(TokenType.CloseParenthesis);
                break;

            case TokenType.Identifier:
                ParseFunctionCallExpression();
                break;

            default:
                throw new UnexpectedLexemeException(t, [TokenType.IntLiteral, TokenType.Identifier, TokenType.OpenParenthesis]);
        }
    }

    private void ParseFunctionCallExpression()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.OpenParenthesis);

        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            ParseExpression();
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                ParseExpression();
            }
        }

        Match(TokenType.CloseParenthesis);

        if (BuiltinFunctions.TryGetCode(name, out BuiltinFunctionCode code))
        {
            _instructions.Add(new Instruction(InstructionCode.CallBuiltin, new Value((int)code)));
        }
        else
        {
            throw new UnknownFunctionException(name);
        }
    }

    private Token Match(TokenType expected)
    {
        Token t = _tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(t, expected);
        }

        _tokens.Advance();
        return t;
    }
}