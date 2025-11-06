namespace PWMan.Core;

public class VaultMetadata
{
    public string? EncryptionMethod { get; set; }
    public string? DerivationMethod { get; set; }
    public string? RepositoryType { get; set; }
    public string? SaveFile { get; set; }
    public int SaltSize { get; set; }
    public int KeySize { get; set; }
    public int Iterations { get; set; }
}