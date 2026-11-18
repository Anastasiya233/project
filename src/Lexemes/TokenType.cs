namespace Alpaca.Lexemes;

public enum TokenType
{
    /// <summary>
    /// Ключевое слово main.
    /// </summary>
    Main,

    /// <summary>
    /// Ключевое слово array.
    /// </summary>
    Array,

    /// <summary>
    /// Ключевое слово function.
    /// </summary>
    Function,

    /// <summary>
    /// Ключевое слово return.
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово if.
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово else.
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово for.
    /// </summary>
    For,

    /// <summary>
    /// Ключевое слово while.
    /// </summary>
    While,

    /// <summary>
    /// Ключевое слово break.
    /// </summary>
    Break,

    /// <summary>
    /// Ключевое слово nil.
    /// </summary>
    Nil,

    /// <summary>
    /// Ключевое слово var.
    /// </summary>
    Var,

    /// <summary>
    /// Ключевое слово struct.
    /// </summary>
    Struct,

    /// <summary>
    /// Ключевое слово true.
    /// </summary>
    True,

    /// <summary>
    /// Ключевое слово false.
    /// </summary>
    False,

    /// <summary>
    /// Ключевое слово int.
    /// </summary>
    Int,

    /// <summary>
    /// Ключевое слово string.
    /// </summary>
    String,

    /// <summary>
    /// Ключевое слово bool.
    /// </summary>
    Bool,

    /// <summary>
    /// Ключевое слово void.
    /// </summary>
    Void,

    /// <summary>
    /// Литерал целого числа.
    /// </summary>
    IntLiteral,

    /// <summary>
    /// Литерал строки.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Идентификатор.
    /// </summary>
    Identifier,

    /// <summary>
    /// Оператор доступа к полю ".".
    /// </summary>
    Dot,

    /// <summary>
    /// Оператор сложения "+".
    /// </summary>
    Plus,

    /// <summary>
    /// Оператор вычитания "-".
    /// </summary>
    Minus,

    /// <summary>
    /// Оператор умножения "*".
    /// </summary>
    Multiply,

    /// <summary>
    /// Оператор деления "/".
    /// </summary>
    Divide,

    /// <summary>
    /// Оператор деления с остатком "%".
    /// </summary>
    Percent,

    /// <summary>
    /// Оператор возведения в степень "^".
    /// </summary>
    Power,

    /// <summary>
    /// Оператор сравнения "равно".
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения "не равно".
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Логический оператор "и".
    /// </summary>
    And,

    /// <summary>
    /// Логический оператор "или".
    /// </summary>
    Or,

    /// <summary>
    /// Логический оператор "не".
    /// </summary>
    Not,

    /// <summary>
    /// Оператор присваивания ":=".
    /// </summary>
    Assign,

    /// <summary>
    /// Открывающая круглая скобка "(".
    /// </summary>
    OpenParenthesis,

    /// <summary>
    /// Закрывающая круглая скобка ")".
    /// </summary>
    CloseParenthesis,

    /// <summary>
    /// Открывающая фигурная скобка "{".
    /// </summary>
    OpenBrace,

    /// <summary>
    /// Закрывающая фигурная скобка "}".
    /// </summary>
    CloseBrace,

    /// <summary>
    /// Открывающая квадратная скобка "[".
    /// </summary>
    OpenBracket,

    /// <summary>
    /// Закрывающая квадратная скобка "]".
    /// </summary>
    CloseBracket,

    /// <summary>
    /// Запятая ",".
    /// </summary>
    Comma,

    /// <summary>
    /// Точка с запятой ";".
    /// </summary>
    Semicolon,

    /// <summary>
    /// Недопустимая лексема.
    /// </summary>
    Error,

    /// <summary>
    /// Конец потока токенов.
    /// </summary>
    EndOfFile,
}