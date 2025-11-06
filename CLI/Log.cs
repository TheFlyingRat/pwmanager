namespace PWMan.CLI;

public static class Log
{
    private static int _mode = 0;

    public static void SetMode(int a)
    {
        _mode = a;
    }

    public static void Debug(string x)
    {
        if (_mode == 1) { Console.WriteLine(x); }
    }
}