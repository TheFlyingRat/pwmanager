using PWMan.CLI;
using PWMan.Commands;
using PWMan.Core;

static class Program
{
    public static void Main(string[] args)
    {

        List<Command> commands = new List<Command>
        {
            new UnlockVaultCommand(),
            new LockVaultCommand(),
            new AddEntryCommand(),
            new ListEntriesCommand()
        };

        REPL repl = new REPL(commands);
        repl.Start();
    }
}
