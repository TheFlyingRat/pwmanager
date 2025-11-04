namespace PWMan.Core;

public abstract class IdentifiableObject
{
    public Guid Id { get; set; }

    public bool AreYou(Guid id)
    {
        return Id == id;
    }
}
