using System;
using System.IO;

using Alpaca.VirtualMachine;

namespace Alpaca.Interpreter;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: Alpaca.Interpreter <file-path>");
            return 1;
        }

        string sourcePath = args[0];
        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Error: source file '{sourcePath}' not found.");
            return 1;
        }

        try
        {
            string sourceCode = File.ReadAllText(sourcePath);

            ConsoleEnvironment environment = new();
            AlpacaInterpreter interpreter = new(environment);
            interpreter.Execute(sourceCode);

            return interpreter.ExitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Interpreter threw an {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }
}