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

    public static string ValidateQuery(string query, string[] acceptedValues, string defaultValue)
    {
        Console.WriteLine(query);
        string input = Console.ReadLine();
        return String(acceptedValues, input, defaultValue);
    }

    public static string String(string[] acceptedValues, string inputValue, string defaultValue)
    {
        if (!acceptedValues.Contains(inputValue.ToLower()))
        {
            return defaultValue;
        }

        return inputValue.ToLower();
    }
}