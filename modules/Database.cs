// Not in use but... **DO NOT REMOVE**
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CS2_AdminsList.Modules.Models;
using MySqlConnector;

namespace CS2_AdminsList.Modules;
    
class Admin
{
    public string Name { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public bool Hidden {get; set; } = false;

    public Admin(string Name, string Group, bool Hidden)
    {
        this.Name = Name;
        this.Group = Group;
        this.Hidden = Hidden;
    }
}

public class Database
{
    public static List<SQL_Player> admins = new();

    private static string _connectionString = string.Empty;

    public delegate void ConnectCallback(string connectionString, Exception exception, dynamic data);
    public delegate void QueryCallback(MySqlDataReader reader, Exception exception, dynamic data);

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static void Connect(string connectionString, ConnectCallback callback, dynamic data = null!)
    {
        try
        {
            var connection = new MySqlConnection(connectionString);

            connection.Open();

            callback(connectionString, null!, data);

            connection.Close();
        }
        catch (Exception e)
        {
            callback(null!, e, data);
        }
    }

    public static string EscapeString(string buffer)
    {
        return MySqlHelper.EscapeString(buffer);
    }

    public static void Query(QueryCallback callback, string query, dynamic data = null!)
    {
        try 
        {
            if (string.IsNullOrEmpty(query))
            {
                Utils.ThrowError("Query cannot be null or empty.");
            }

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using(var command = new MySqlCommand(query, connection))
            {
                using(var reader = command.ExecuteReader())
                {
                    callback(reader, null!, data);
                }
            }

            connection.Close();
        }
        catch (Exception e)
        {
            callback(null!, e, data);
        }
    }

    public static void SQL_CheckForErrors(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if (exception != null!)
        {
            Utils.ThrowError($"Database error, {exception.Message}");
        }
    } 

    public static void SQL_ConnectCallback(string connectionString, Exception exception, dynamic data)
    {
        if (connectionString == null!)
        {
            Utils.ThrowError($"Failed to connect to database: {exception.Message}");
            return;
        }

        CS2_AdminsList.Db = new Database(connectionString);

        Utils.PrintToServer("Connected to database");

        SQL_CreateTable();
    }

    public static void SQL_CreateTable()
    {
        const string query = "CREATE TABLE IF NOT EXISTS `admin_list_settings` ( `id` INT NOT NULL AUTO_INCREMENT , `auth` VARCHAR(128) NOT NULL , `name` VARCHAR(128) NOT NULL , `show_in_list` INT(1) NOT NULL DEFAULT 1, PRIMARY KEY (`id`), UNIQUE (`auth`)) ENGINE = InnoDB;";

        Query(SQL_CheckForErrors, query);
    }

    public static void SQL_FetchUser_CB(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if (exception != null!)
        {
            Utils.ThrowError($"Database error, {exception.Message}");
            return;
        }

        CCSPlayerController cCSPlayerController = Utilities.GetPlayerFromSlot(data);

        string name = string.Empty;
        bool showInList = true;

        if (reader.HasRows)
        {
            while(reader.Read())
            {
                name = reader.GetString("name");
                showInList = reader.GetBoolean("show_in_list");
            }
        }
        else
        {
            SteamID steamID = Utils.GetSteamID(cCSPlayerController);

            if(steamID == null!)
            {
                Utils.ThrowError("Failed to get steamID");
                return;
            }

            name = cCSPlayerController.PlayerName;

            Query(SQL_CheckForErrors, $"INSERT INTO `admin_list_settings` (`auth`, `name`) VALUES ('{steamID.SteamId64}', '{EscapeString(name)}')");
        }

        SQL_Player player = new SQL_Player(cCSPlayerController, showInList);
        CS2_AdminsList.Players.Add(player);
    }

    public static void SQL_UpdateUser(CCSPlayerController player, bool state)
    {
        CS2_AdminsList.Players.Where(x => x.Player.Slot == player.Slot).FirstOrDefault(player => player.ShowInList = state);

        SteamID steamID = Utils.GetSteamID(player);

        if(steamID == null!)
        {
            Utils.ThrowError("Failed to get steamID");
            return;
        }

        Query(SQL_CheckForErrors, $"UPDATE `admin_list_settings` SET `name` = '{EscapeString(player.PlayerName)}', `show_in_list` = '{(state ? 1 : 0)}' WHERE `auth` = '{steamID.SteamId64}'");
    }

    public static void SQL_FetchAdmins_CB(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if (exception != null!)
        {
            Utils.ThrowError($"Database error, {exception.Message}");
            return;
        }

        CCSPlayerController cCSPlayerController = Utilities.GetPlayerFromSlot(data);

        if(cCSPlayerController == null!)
        {
            Utils.ThrowError("cCSPlayerController is null!");
            return;
        }

        if(!cCSPlayerController.IsValid)
        {
            Utils.ThrowError("cCSPlayerController is not valid!");
            return;
        }

        if (reader.HasRows)
        {

            List<Admin> adminsData = new();

            while(reader.Read())
            {
                string auth = reader.GetString("auth");
                string group = reader.GetString("group");

                SQL_Player player = Utils.GetPlayerFromSteamID(auth);

                if(player == null!)
                {
                    continue;
                }

                SQL_Player admin = Utils.GetPlayerFromSteamID(admins, auth);

                if(admin == null!)
                {
                    continue;
                }

                if(!player.ShowInList && !AdminManager.CanPlayerTarget(cCSPlayerController, player.Player))
                {
                    continue;
                }

                Admin adminData = new Admin(player.Name, group, !player.ShowInList);
                adminsData.Add(adminData);
            }

            ChatMenu chatMenu = new ChatMenu($"{Utils.PREFIX_MENU} {adminsData.Count} Admins Are Online:");

            foreach(Admin admin in adminsData)
            {
                string color = CS2_AdminsList._Config.Groups[admin.Group];
                string group = $"{{{color}}}[{admin.Group}]\x01";

                chatMenu.AddMenuOption($"{admin.Name} {Utils.ReplaceWithColor(group)}\x01 {(admin.Hidden ? "[Hidden]":"")}", (p, c) => {}, true);
            }

            if(adminsData.Count == 0)
            {
                chatMenu.AddMenuOption("No admins are online", (p, c) => {}, true);
            }

            MenuManager.OpenChatMenu(cCSPlayerController, chatMenu);
        }
    }
}
