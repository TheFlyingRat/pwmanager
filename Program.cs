using PWMan;
using PWMan.CLI;
using PWMan.Commands;
using PWMan.Core;

static class Program
{
    public static void Main(string[] args)
    {
        List<Command> commands = new List<Command>
        {
            new CreateVaultCommand(),

            new LoadVaultCommand(),
            new UnloadVaultCommand(),

            new LockVaultCommand(),
            new UnlockVaultCommand(),

            new AddEntryCommand(),
            new ListEntriesCommand(),
            new DeleteEntryCommand(),
            new SearchEntriesCommand(),
            new GetEntryCommand(),

            new SaveVaultCommand(), // not really required due to autosave
        };

        Log.SetMode(0);

        REPL repl = new REPL(commands);

        repl.RegisterCommand(new LoadVaultCommand(), ["load", "json", "vault.dat"], "lv", "Shortcut to load JSON format 'vault.dat'", false);

        repl.Start();
    }
}
