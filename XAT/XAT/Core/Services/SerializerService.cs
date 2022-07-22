using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using XAT.Services;

namespace XAT.Core.Services;

public class SerializerService : ServiceBase<SettingsService>
{
    public static JsonSerializerOptions Options = new JsonSerializerOptions();

    static SerializerService()
    {
        Options.WriteIndented = true;
        Options.PropertyNameCaseInsensitive = false;
        Options.AllowTrailingCommas = true;
        Options.ReadCommentHandling = JsonCommentHandling.Skip;
        Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        Options.Converters.Add(new JsonStringEnumConverter());
    }

    public static string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }

    public static void SerializeFile(string path, object obj)
    {
        string json = Serialize(obj);
        File.WriteAllText(path, json);
    }

    public static T DeserializeFile<T>(string path)
        where T : new()
    {
        string json = File.ReadAllText(path);
        json = json.Replace("\r", Environment.NewLine);
        T? result = JsonSerializer.Deserialize<T>(json, Options);

        if (result == null)
            throw new Exception("Failed to deserialize object");

        return result;
    }

    public static T Deserialize<T>(string json)
        where T : notnull
    {
        T? result = JsonSerializer.Deserialize<T>(json, Options);

        if (result == null)
            throw new Exception("Failed to deserialize object");

        return result;
    }

    public static object Deserialize(string json, Type type)
    {
        object? result = JsonSerializer.Deserialize(json, type, Options);

        if (result == null)
            throw new Exception("Failed to deserialize object");

        return result;
    }
}
