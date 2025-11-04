using PWMan.Core;

namespace PWMan.Commands;

public class UnlockVaultCommand : Command
{
    public override string Name { get; protected set; } = "unlock";

    public override string Execute(string[] args)
    {
        var helpRequested = base.Execute(args);
        if (helpRequested != "") { return helpRequested; }

        if (args.Length < 2)
        {
            return "No master password provided.";
        }

        string target = KeyFile.ReadFromFile("keyfile.txt");

        byte[] salt = Convert.FromBase64String(target.Split('?')[1]);

        byte[] key = Vault.KDF.DeriveKey(args[1] ?? "", salt); // derive the key to cache

        string hash = Convert.ToBase64String(key) + "?" + Convert.ToBase64String(salt);

        return Vault.Instance.Unlock(hash) ? "Vault unlocked successfully." : "Incorrect master password.";
    }


    public override string GetHelp()
    {
        return $"Usage: {Name}\nUnlocks the password vault.";
    }
}