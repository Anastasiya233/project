using System;
using System.Collections.Generic;
using System.Text;

namespace Alpaca.VirtualMachine.Builtins
{
    public enum BuiltinFunctionCode
    {
        /// <summary>
        /// `void print(string s)` — печатает строку `s` в стандартный поток вывода
        /// </summary>
        Print = 0,

        /// <summary>
        /// `void println(string s)` — печатает строку `s` и перенос строки в стандартный поток вывода
        /// </summary>
        Println = 1,

        /// <summary>
        /// `void printInt(int x)` — печатает целое число `x` в стандартный поток вывода
        /// </summary>
        PrintInt = 2,

        /// <summary>
        /// `int read()` — читает один символ из стандартного потока ввода и возвращает его числовой код
        /// </summary>
        Read = 3,

        /// <summary>
        /// `string readln()` — читает строку до символа переноса строки из стандартного потока ввода
        /// </summary>
        Readln = 4,
    }
}