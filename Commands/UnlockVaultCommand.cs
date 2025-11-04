using System.Net.Http.Headers;
using PWMan.Core;

namespace PWMan.Commands;

public class UnlockVaultCommand : Command
{
    public override string Name { get; protected set; } = "unlock-vault";

    public override bool Execute(string[] args)
    {
        if (args[0] == "")
        {
            Console.WriteLine("No master password provided.");
            return false;
        }

        string target = KeyFile.ReadFromFile("keyfile.txt");

        byte[] salt = Convert.FromBase64String(target.Split('?')[1]);

        byte[] key = Vault.KDF.DeriveKey(args[0] ?? "", salt); // derive the key to cache

        string hash = Convert.ToBase64String(key) + "?" + Convert.ToBase64String(salt);

        // Console.WriteLine("Full hash is: " + hash);

        return Vault.Instance.Unlock(hash);
    }


    public override string GetHelp()
    {
        return "Usage: unlock-vault\nUnlocks the password vault.";
    }
}