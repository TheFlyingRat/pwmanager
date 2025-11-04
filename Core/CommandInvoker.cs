namespace PWMan.Core
{
    public class CommandInvoker
    {
        private Dictionary<string, (Command Command, string[] DefaultArgs)> commands = new Dictionary<string, (Command, string[])>();

        public void RegisterCommand(Command command, string[]? defaultArgs = null, string? name = null)
        {
            name ??= command.Name; // unless a custom name is given, use the command's own name (null coalescing operator)
            commands[name] = (command, defaultArgs ?? []);
        }

        public Command? GetCommand(string name)
        {
            if (commands.ContainsKey(name))
                return commands[name].Command;
            return null;
        }

        // Run using either overridden args (if provided) or the stored args
        public void Run(string name, string[]? args = null)
        {
            if (!commands.ContainsKey(name))
            {
                Console.WriteLine($"Unknown command: {name}");
                return;
            }

            string[] runArgs = args ?? commands[name].DefaultArgs;
            string response = commands[name].Command.Execute(runArgs);

            Console.WriteLine(response);
        }
    }
}