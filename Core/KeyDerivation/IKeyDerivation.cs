// kdf

namespace PWMan.Core.KeyDerivation;

public interface IKeyDerivation
{
    byte[] DeriveKey(string password, byte[] salt);
}
