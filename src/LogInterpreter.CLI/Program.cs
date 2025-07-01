using System;
using LogInterpreter.CLI;

namespace LogInterpreter.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var appArgs = ArgumentParser.Parse(args);
                Console.WriteLine($"Command: {appArgs.Command}");
                // Further processing based on the command
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Usage: LogInterpreter.CLI <command> [parameters]");
            }
        }
    }
}