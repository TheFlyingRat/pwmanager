namespace PWMan.Core;

public class VaultMetadata
{
    public string EncryptionMethod { get; set; } = "aes";
    public string DerivationMethod { get; set; } = "argon2";
    public string RepositoryType { get; set; } = "json";
    public string SaveFile { get; set; } = "vault.json";
    public int SaltSize { get; set; } = 16;
    public int KeySize { get; set; } = 32;
    public int Iterations { get; set; } = 100000;
}