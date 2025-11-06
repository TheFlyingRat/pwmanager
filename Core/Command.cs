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
    public string Name { get; }
    public string Help { get; }
}