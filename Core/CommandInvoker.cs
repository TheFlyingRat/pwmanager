using PWMan.Commands;

namespace PWMan.Core;

public class CommandInvoker
{
    private Dictionary<string, (Command Command, string[] DefaultArgs)> commands = new Dictionary<string, (Command, string[])>();

    public void RegisterCommand(Command command, string[]? defaultArgs = null, string? name = null, string? help = null, bool requiresVault = true)
    {
        name ??= command.Name; // unless a custom name is given, use the command's own name (null coalescing operator)

        command.Help = help ?? command.Help; // override help if provided
        command.Name = name; // override name is provided
        command.RequiresVault = requiresVault; // if a custom command RequiresVault
        commands[name] = (command, defaultArgs ?? []); // set the local dictionary
    }

    // Run using either overridden args (if provided) or the stored args
    public string Run(string name, string[] args)
    {
        if (!commands.ContainsKey(name))
        {
            return $"Unknown command: {name}";
        }

        // was help requested? (would be first argument)
        if (args != null && args.Length > 1 && (args[1] == "help" || args[1] == "h"))
        {
            return commands[name].Command.Help;
        }

        // no vault exist yet?
        if (!Vault.Exists)
        {
            // only can load or create if theres no vault
            if (commands[name].Command.RequiresVault)
            {
                return "Cannot execute without a vault! Did you load or create first?";
            }
        }

        // if it exists (locked or unlocked)
        else
        {
            // prevent creating or loading another
            if (commands[name].Command is LoadVaultCommand || commands[name].Command is CreateVaultCommand)
            {
                return "A vault is already loaded. Please unload before creating/loading another one!";
            }
        }

        string[] runArgs = args!.Length > 1 ? args : commands[name].DefaultArgs;
        string response = commands[name].Command.Execute(runArgs);

        return response;
    }
    public List<Command> RegisteredCommands { get { return commands.Values.Select(v => v.Command).ToList(); } }
}