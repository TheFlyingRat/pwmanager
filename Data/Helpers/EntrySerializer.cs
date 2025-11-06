using System.Text.Json;
using System.Text.Json.Serialization;
using PWMan.Core;

namespace PWMan.Data;

public static class EntrySerializer


{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        // store enums as strings
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
    };


    // serialize entries into json string
    public static string Serialize(List<Entry> entries)
    {
        return JsonSerializer.Serialize(entries, JsonOptions);
    }

    // deserialize json string into a list of Entry (polymorphic deserialization)
    public static List<Entry> DeserializeEntries(string json)
    {
        try
        {
            var elements = JsonSerializer.Deserialize<List<JsonElement>>(json, JsonOptions) ?? new List<JsonElement>(); // if its null after deserialization, create empty
            var entries = new List<Entry>();

            foreach (var element in elements)
            {
                string typeName = element.GetProperty("EntryType").GetString()!; // null forgive - theres no way for the encrypted part of the save file to be modified to remove EntryType
                EntryType entryType = Enum.Parse<EntryType>(typeName, true); // convert type enum name to the enum value

                Entry? entry;
                switch (entryType)
                {
                    case EntryType.SecureNote:
                        entry = JsonSerializer.Deserialize<SecureNote>(element.GetRawText(), JsonOptions);
                        break;
                    case EntryType.Wifi:
                        entry = JsonSerializer.Deserialize<WifiEntry>(element.GetRawText(), JsonOptions);
                        break;
                    default:
                        entry = JsonSerializer.Deserialize<Entry>(element.GetRawText(), JsonOptions);
                        break;
                }

                if (entry != null) { entries.Add(entry); }
            }

            return entries;
        }
        catch
        {
            return new List<Entry>();
        }
    }
}
