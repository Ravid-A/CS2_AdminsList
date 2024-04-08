using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;
using MySqlConnector;

namespace CS2_AdminsList.Modules;

public class Config
{ 
    public string Host { get; init; } = string.Empty;
    public string Database { get; init; } = string.Empty;
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public uint Port { get; init; } = 3306;

    public Dictionary<string, string> Groups { get; init; } = new();
    

    public Config()
    {
        Host = string.Empty;
        Database = string.Empty;
        User = string.Empty;
        Password = string.Empty;
        Port = 3306;
        Groups = new()
        {
            { "Management", "red" },
            { "Admin", "green" },
        };
    }

    public bool IsValid()
    {
        return Database != string.Empty && Host != string.Empty && User != string.Empty && Password != string.Empty && 0 < Port && Port < 65535;
    }

    public string BuildConnectionString()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Database = Database,
            UserID = User,
            Password = Password,
            Server = Host,
            Port = Port,
        };

        return builder.ConnectionString;
    }
}

public static class Configs
{
    private const string ConfigPath = "cs2_adminslist.json";

    public static Config LoadConfig()
    {
        var configPath = Path.Combine(CS2_AdminsList.Instance.ModuleDirectory, ConfigPath);
        if (!File.Exists(configPath))
        {
            return CreateConfig(configPath);
        }

        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath))!;

        return config;
    }

    private static Config CreateConfig(string configPath)
    {
        var config = new Config();

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
        return config;
    }
}
