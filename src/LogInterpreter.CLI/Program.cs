namespace LogInterpreter.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestsForDev.CompactJsonParser_Manfred_Prod_FilesAndDates();
            //TestsForDev.CompactJsonParser_Manfred_SecurityQueueError();
            //TestsForDev.CompactJsonParser_Manfred_Prod_Analysis_2025_12_24();
            TestsForDev.ParsePipeline();
            return;

            try
            {
                var appArgs = ArgumentParser.Parse(args);
                Console.WriteLine($"Command: {appArgs.Command}");
                var tools = new Tools();
                switch (appArgs.Command?.ToLowerInvariant())
                {
                    case "rewrite":
                        if (appArgs.RewriteArgs != null)
                            tools.Rewrite(appArgs.RewriteArgs.Source!, appArgs.RewriteArgs.Target!);
                        break;
                    // case "command1":
                    //     // tools.Command1(...)
                    //     break;
                    // case "command2":
                    //     // tools.Command2(...)
                    //     break;
                    default:
                        Console.WriteLine($"Unknown or unhandled command: {appArgs.Command}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Usage: LogInterpreter.CLI <command> [parameters]");
            }
        }
    }
}