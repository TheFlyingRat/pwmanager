// command abstract base class

namespace PWMan.Core;

public abstract class Command : IdentifiableObject
{
    public Command(string name, string help = "No help available for this command.")
    {
        Name = name;
        Help = help;
    }

    public abstract string Execute(string[] args);
    public string Name { get; set; } // for custom commands
    public string Help { get; set; } // for custom commands
    public bool RequiresVault { get; set; } = true;
}