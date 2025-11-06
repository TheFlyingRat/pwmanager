namespace PWMan.Core;

public class VaultMetadata
{
    public EncryptionType EncryptionMethod { get; set; }
    public DerivationType DerivationMethod { get; set; }
    public RepositoryType RepositoryType { get; set; }
    public string? SaveFile { get; set; }
    public int SaltSize { get; set; }
    public int KeySize { get; set; }
    public int Iterations { get; set; }
    public int MemorySize { get; set; }
    public int Parallelism { get; set; }
}