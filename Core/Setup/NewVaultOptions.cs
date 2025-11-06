namespace PWMan.Core.Setup
{
    public class NewVaultOptions // sets some defaults
    {
        public string Password { get; set; } = "";
        public string Encryption { get; set; } = "aes";
        public string Kdf { get; set; } = "pbkdf2";
        public int Iterations { get; set; } = 300_000;
        public int KeySize { get; set; } = 32;
        public int SaltSize { get; set; } = 16;
        public string SaveType { get; set; } = "json";
        public string SaveFile { get; set; } = "vault.json";
    }
}