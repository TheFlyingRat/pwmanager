// command abstract base class

namespace PWMan.Core;

public abstract class Command : IdentifiableObject
{
    public abstract bool Execute(string[] args);

    public virtual string GetHelp()
    {
        return "No help available for this command.";
    }

    public abstract string Name { get; protected set; }
}