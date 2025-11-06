namespace PWMan.Core;

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

        // was help requested? (would be first argument)
        if (args != null && args.Length > 1 && (args[1] == "help" || args[1] == "h"))
        {
            Console.WriteLine(commands[name].Command.Help);
            return;
        }

        if (!Vault.Exists && name != "load" && name != "create")
        {
            Console.WriteLine("Cannot execute without a vault! Did you load or create first?");
            return;
        }

        string[] runArgs = args ?? commands[name].DefaultArgs;
        string response = commands[name].Command.Execute(runArgs);

        Console.WriteLine(response);
    }
    public List<Command> RegisteredCommands { get { return commands.Values.Select(v => v.Command).ToList(); } }
}