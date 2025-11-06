using PWMan;
using PWMan.CLI;
using PWMan.Commands;
using PWMan.Core;

static class Program
{
    public static void Main(string[] args)
    {
        List<CommandRegistration> commands = new List<CommandRegistration>()
        {
            // basic commands
            new CommandRegistration(new CreateVaultCommand(), RequiresVault: false),

            new CommandRegistration(new LoadVaultCommand(), RequiresVault: false),
            new CommandRegistration(new UnloadVaultCommand()),

            new CommandRegistration(new LockVaultCommand()),
            new CommandRegistration(new UnlockVaultCommand()),

            new CommandRegistration(new AddEntryCommand()),
            new CommandRegistration(new GetEntryCommand()),

            new CommandRegistration(new SearchEntriesCommand()),
            new CommandRegistration(new ListEntriesCommand()),
            new CommandRegistration(new DeleteEntryCommand()),

            new CommandRegistration(new SaveVaultCommand()),

            // custom commands
            new CommandRegistration(new LoadVaultCommand(), DefaultArgs: ["load", "json", "vault.dat"], Name: "lv", Help: "Shortcut to load JSON format 'vault.dat'", RequiresVault: false)
        };

        Log.SetMode(0);

        REPL repl = new REPL(commands);

        repl.Start();
    }
}
