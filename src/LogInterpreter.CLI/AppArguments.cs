using System.CommandLine;
// using System.CommandLine.Invocation; // Not needed for SetHandler in System.CommandLine 2.x

namespace LogInterpreter.CLI
{
    public class AppArguments
    {
        public string? Command { get; set; }
        public Command1Arguments? Command1Args { get; set; }
        public Command2Arguments? Command2Args { get; set; }
        public RewriteArguments? RewriteArgs { get; set; }
    }

    public class Command1Arguments
    {
        public string? Arg11 { get; set; }
    }

    public class Command2Arguments
    {
        public string? Arg21 { get; set; }
        public int? Arg22 { get; set; }
    }

    public class RewriteArguments
    {
        public string? Source { get; set; }
        public string? Target { get; set; }
    }

    public static class ArgumentParser
    {
        public static (RootCommand rootCommand, Option<string?> arg11Option, Option<string?> arg21Option, Option<int?> arg22Option, Argument<string> rewriteSourceArg, Argument<string> rewriteTargetArg) GetRootCommand()
        {
            // Define command1 with optional arg11
            var arg11 = new Option<string?>("--arg11")
            {
                Description = "Optional argument for command1."
            };
            var command1 = new Command("command1", "Executes command1.\nThis command does X.\n\nOptions:\n  --arg11   Optional string argument.")
            {
                arg11
            };

            // Define command2 with optional arg21 (string) and arg22 (int)
            var arg21 = new Option<string?>("--arg21")
            {
                Description = "Optional string argument for command2."
            };
            var arg22 = new Option<int?>("--arg22")
            {
                Description = "Optional integer argument for command2."
            };
            var command2 = new Command("command2", "Executes command2.\nThis command does Y.\n\nOptions:\n  --arg21   Optional string argument.\n  --arg22   Optional integer argument.")
            {
                arg21,
                arg22
            };

            // Define Rewrite command with two required arguments
            var rewriteSource = new Argument<string>("source")
            {
                Description = "Source file or directory (required)"
            };
            var rewriteTarget = new Argument<string>("target")
            {
                Description = "Target file or directory (required)"
            };
            var rewriteCommand = new Command("rewrite", "Rewrite files or directories. Both source and target are required.")
            {
                rewriteSource,
                rewriteTarget
            };
            rewriteCommand.Aliases.Add("Rewrite");
            rewriteCommand.Aliases.Add("REWRITE");

            var rootCommand = new RootCommand("LogInterpreter CLI")
            {
                command1,
                command2,
                rewriteCommand
            };
            return (rootCommand, arg11, arg21, arg22, rewriteSource, rewriteTarget);
        }

        public static AppArguments Parse(string[] args)
        {
            var appArgs = new AppArguments();
            var (rootCommand, arg11Option, arg21Option, arg22Option, rewriteSourceArg, rewriteTargetArg) = GetRootCommand();
            var result = rootCommand.Parse(args);

            // Help/usage support (root)
            if (args.Length == 0 || (args.Length == 1 && (args[0] == "--help" || args[0] == "-h" || args[0] == "-?")))
            {
                Console.WriteLine("LogInterpreter CLI\n");
                Console.WriteLine("Available commands:");
                foreach (var cmd in rootCommand.Children.OfType<Command>())
                {
                    Console.WriteLine($"  {cmd.Name}   {cmd.Description?.Split('\n')[0] ?? ""}");
                }
                Console.WriteLine("\nUse '<command> --help' for more information about a command.");
                Environment.Exit(0);
            }

            // Per-command help
            if (args.Length >= 2 && (args[1].Equals("--help", StringComparison.OrdinalIgnoreCase) || args[1].Equals("-h", StringComparison.OrdinalIgnoreCase) || args[1].Equals("-?", StringComparison.OrdinalIgnoreCase)))
            {
                var commandName = args[0];
                var command = rootCommand.Children.OfType<Command>().FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
                if (command != null)
                {
                    Console.WriteLine($"{command.Name} - {command.Description?.Split('\n')[0] ?? ""}\n");
                    var descriptionLines = (command.Description ?? "").Split('\n').Skip(1).ToList();
                    foreach (var line in descriptionLines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            Console.WriteLine(line);
                    }
                    Console.WriteLine("\nOptions:");
                    foreach (var opt in command.Children.OfType<Option>())
                    {
                        Console.WriteLine($"  {string.Join(", ", opt.Aliases)}   {opt.Description}");
                    }
                    Environment.Exit(0);
                }
            }
            if (result.Errors != null && result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    Console.Error.WriteLine($"Argument parsing error: {error.Message}");
                }
                Console.Error.WriteLine("\nUsage:");
                Console.Error.WriteLine(rootCommand.Description);
                Environment.Exit(1);
            }

            // Determine which command was invoked
            var commandResult = result.CommandResult;
            var commandNameNormalized = commandResult.Command.Name.ToLowerInvariant();
            if (commandNameNormalized == "command1")
            {
                appArgs.Command = "command1";
                appArgs.Command1Args = new Command1Arguments
                {
                    Arg11 = result.GetValue(arg11Option)
                };
            }
            else if (commandNameNormalized == "command2")
            {
                appArgs.Command = "command2";
                appArgs.Command2Args = new Command2Arguments
                {
                    Arg21 = result.GetValue(arg21Option),
                    Arg22 = result.GetValue(arg22Option)
                };
            }
            else if (commandNameNormalized == "rewrite")
            {
                appArgs.Command = "Rewrite";
                appArgs.RewriteArgs = new RewriteArguments
                {
                    Source = result.GetValue(rewriteSourceArg),
                    Target = result.GetValue(rewriteTargetArg)
                };
            }

            // ...
            return appArgs;
        }
    }
}
