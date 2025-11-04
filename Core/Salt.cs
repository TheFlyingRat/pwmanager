// because lazy, here is the Salt class that was added

namespace PWMan.Core;

public static class Salt
{
    public static byte[] GenerateSalt(int size = 32)
    {
        byte[] salt = new byte[size];
        var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
}
