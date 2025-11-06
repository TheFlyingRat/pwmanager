using PWMan.Core;

namespace PWMan.CLI
{
    public class REPL
    {
        private readonly CommandInvoker _invoker = new CommandInvoker();

        private readonly int RegisteredCommandCount;

        private void Banner()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine($"          {Name.StylisedAppName} - Version {Version.AppVersion}          ");
            Console.WriteLine("  Type 'help' to see available commands  ");
            Console.WriteLine($"    There are {RegisteredCommandCount} registered commands   ");
            Console.WriteLine("=========================================");
            Console.WriteLine();
        }

        public REPL(List<Command> commands)
        {
            foreach (var command in commands)
            {
                _invoker.RegisterCommand(command);
            }

            // "if only i had time:" allow registering commands while running?

            RegisteredCommandCount = commands.Count;
        }

        public void Start()
        {
            Banner();

            bool running = true;
            while (running)
            {
                Console.Write($"{Name.CLIName}> ");
                string input = Console.ReadLine() ?? "";
                string[] inputArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries); // remove empty entries
                if (input == null) { continue; }

                if (inputArgs.Length == 0) { continue; } // no input, prompt again

                string commandName = inputArgs[0];

                switch (commandName)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "exit" or "quit":
                        if (Vault.Exists && !Vault.Instance.IsLocked) { Vault.Instance.Lock(); }; // ensure vault is locked but also saves any changes
                        Console.WriteLine("Exiting PWMan. Goodbye!");
                        return;
                    default:
                        try
                        {
                            _invoker.Run(commandName, inputArgs);
                        } catch (Exception ex)
                        {
                            Console.WriteLine($"Uncaught Exception: {ex.Message}");
                        }
                        break;
                }
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  help - Show this help message.");
            Console.WriteLine("  exit - Exit the application.");

            // show registered commands and their help too
            foreach (var cmd in _invoker.RegisteredCommands)
            {
                Console.WriteLine($"  {cmd.Name} - {cmd.Help}");
            }
        }
    }
}