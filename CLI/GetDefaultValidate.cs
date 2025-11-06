namespace PWMan.CLI;

public static class GetDefaultValidate
{
    public static string? GetString(string query)
    {
        Console.WriteLine(query);
        string input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }

    public static int? GetInt(string query)
    {
        Console.WriteLine(query);
        string input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : Convert.ToInt32(input);
    }

    public static string ValidateString(string query, string[] acceptedValues, string defaultValue)
    {
        Console.WriteLine(query);
        string input = Console.ReadLine();
        return String(acceptedValues, input, defaultValue);
    }

    public static int ValidateInt(string query, int min, int max, int defaultValue)
    {
        Console.WriteLine(query);
        string input = Console.ReadLine();
        int value;
        try { value = Convert.ToInt32(input); } catch { value = defaultValue; } // convert to string (unsuccessful use default)
        if (min > value || value < max) { value = defaultValue; } // was given int? okay range check
        return value;
    }

    private static string String(string[] acceptedValues, string inputValue, string defaultValue)
    {
        if (!acceptedValues.Contains(inputValue.ToLower()))
        {
            return defaultValue;
        }

        return inputValue.ToLower();
    }
}