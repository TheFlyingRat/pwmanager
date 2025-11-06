using PWMan.Core;

namespace PWMan.CLI;

public static class GetDefaultValidate
{
    public static string GetString(string query, string defaultValue)
    {
        Console.Write(query);
        string input = Console.ReadLine()?.Trim() ?? "";
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    public static string GetStringRequired(string query)
    {
        string? input = null;
        while (input == null)
        {
            Console.Write(query);
            string value = Console.ReadLine()?.Trim() ?? "";
            input = string.IsNullOrWhiteSpace(value) ? null : value;
        }
        return input;
    }

    public static EncryptionType ValidateEncryption(string prompt, EncryptionType defaultValue)
    {
        // get all names within the enum
        string[] options = Enum.GetNames<EncryptionType>();

        // prompt
        Console.Write($"{prompt} ({string.Join(", ", options)}) [{defaultValue}]: ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(input)) { return defaultValue; }

        // try to convert the input string into an EncryptionType (case-insensitive)
        try
        {
            return Enum.Parse<EncryptionType>(input, ignoreCase: true);
        }
        catch
        {
            Console.WriteLine($"Invalid choice. Using default: {defaultValue}");
            return defaultValue;
        }
    }

    public static DerivationType ValidateDerivation(string prompt, DerivationType defaultValue)
    {
        // get all names within the enum
        string[] options = Enum.GetNames<DerivationType>();

        // prompt
        Console.Write($"{prompt} ({string.Join(", ", options)}) [{defaultValue}]: ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(input)) { return defaultValue; }

        // try to convert the input string into an DerivationType (case-insensitive)
        try
        {
            return Enum.Parse<DerivationType>(input, ignoreCase: true);
        }
        catch
        {
            Console.WriteLine($"Invalid choice. Using default: {defaultValue}");
            return defaultValue;
        }
    }
    
    public static RepositoryType ValidateRepositoryType(string prompt, RepositoryType defaultValue)
    {
        // get all names within the enum
        string[] options = Enum.GetNames<RepositoryType>();

        // prompt
        Console.Write($"{prompt} ({string.Join(", ", options)}) [{defaultValue}]: ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(input)) { return defaultValue; }
        
        // try to convert the input string into an RepositoryType (case-insensitive)
        try
        {
            return Enum.Parse<RepositoryType>(input, ignoreCase: true);
        }
        catch
        {
            Console.WriteLine($"Invalid choice. Using default: {defaultValue}");
            return defaultValue;
        }
    }

    public static string ValidateString(string query, string[] acceptedValues, string defaultValue)
    {
        Console.Write(query);
        string input = Console.ReadLine()?.Trim() ?? "";
        return String(acceptedValues, input, defaultValue);
    }

    public static int ValidateInt(string query, int min, int max, int defaultValue)
    {
        Console.Write(query);
        string input = Console.ReadLine()?.Trim() ?? "";
        int value;
        try { value = Convert.ToInt32(input); } catch { Console.WriteLine("Invalid choice. Using default: " + defaultValue); value = defaultValue; } // convert to string (unsuccessful use default)
        if (value < min || value > max) { Console.WriteLine("Invalid choice. Using default: " + defaultValue); value = defaultValue; } // was given int? okay range check
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