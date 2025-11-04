// command abstract base class

namespace PWMan.Core;

public abstract class Command : IdentifiableObject
{
    public virtual string Execute(string[] args)
    {
        // Console.WriteLine ("args: " + string.Join(", ", args));
        if (args.Length > 1 && (args[1] == "help" || args[1] == "h")) // "unlock help" (therefore check args[1])
        {
            return GetHelp();
        }
        return ""; // default implementation does nothing
    }

    public virtual string GetHelp()
    {
        return "No help available for this command.";
    }

    public abstract string Name { get; protected set; }
}