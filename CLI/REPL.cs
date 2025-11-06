using PWMan.Core;

namespace PWMan.CLI
{
    public class REPL
    {
        private readonly CommandInvoker _invoker = new CommandInvoker();
        public int RegisteredCommandCount { get; private set; }

        private void Banner()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine($"          {Name.StylisedAppName} - Version {Version.AppVersion}          ");
            Console.WriteLine("  Type 'help' to see available commands  ");
            Console.WriteLine($"    There are {RegisteredCommandCount} registered commands   ");
            Console.WriteLine("=========================================");
            Console.WriteLine();
        }

        public REPL(List<CommandRegistration> registrations)
        {
            foreach (var register in registrations)
            {
                RegisterCommand(register.Command, register.DefaultArgs, register.Name, register.Help, register.RequiresVault);
            }

            // "if only i had time:" allow registering commands while running?

            RegisteredCommandCount = registrations.Count;
        }

        // allow custom command registration with args
        public void RegisterCommand(Command command, string[]? defaultArgs = null, string? name = null, string? help = null, bool requiresVault = true)
        {
            if (help != null) { command.Help = help; }
            if (name != null) { command.Name = name; }
            command.RequiresVault = requiresVault; // true by default
            _invoker.RegisterCommand(command, defaultArgs, name, help, requiresVault);
            RegisteredCommandCount++;
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