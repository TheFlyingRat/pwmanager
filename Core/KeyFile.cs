// probably temporary thing until i figure out how to store the key data alongside the password data

namespace PWMan.Core;

public static class KeyFile
{
    public static void WriteToFile(string path, string hash)
    {
        File.WriteAllText(path, hash);
    }

    public static string ReadFromFile(string path)
    {
        return File.ReadAllText(path);
    }
}
