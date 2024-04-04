
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Admin;
using CS2_AdminsList.Modules.Handlers;
using CounterStrikeSharp.API;

using CS2_AdminsList.Modules;
using CS2_AdminsList.Modules.Models;

namespace CS2_AdminsList;

public class CS2_AdminsList : BasePlugin
{
    public override string ModuleName => "CS2_AdminsList Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Ravid";
    public override string ModuleDescription => "CS2_AdminsList Plugin";

    public static CS2_AdminsList Instance { get; private set; } = null!;
    public static Database Db = null!;
    public static Config _Config = null!;
    public static List<SQL_Player> Players = new();

    public override void Load(bool hotReload)
    {
        Instance = this;
        Commands.RegisterCommands();
        Modules.Handlers.Listeners.RegisterListeners();

        _Config = Configs.LoadConfig();

        if(_Config == null)
        {
            Utils.ThrowError("Failed to load config");
        }

        if(!_Config!.IsValid())
        {
            Utils.ThrowError("Config is not valid");
        }


        string connectionString = _Config.BuildConnectionString();

        Database.Connect(connectionString, Database.SQL_ConnectCallback);

        if (hotReload)
        {
            Utilities.GetPlayers().ForEach((player) => Modules.Handlers.Listeners.OnClientAuthorized(player.Slot, null!));
        }
    }

    public override void Unload(bool hotReload)
    {
        Commands.UnRegisterCommands();
    }

    public static void ShowAdminsList(CCSPlayerController player)
    {
        List<CCSPlayerController> players = Utilities.GetPlayers();
        List<SQL_Player> admins = new();

        foreach (var p in players)
        {
            AdminData? adminData = AdminManager.GetPlayerAdminData(p);

            if (adminData != null)
            {
                SQL_Player playerData = Utils.GetPlayerFromSlot(p.Slot);
                admins.Add(playerData);
            }
        }

        Database.admins = admins;

        string query = "SELECT a.player_steamid as `auth` ,ag.name as `group` FROM `sa_admins` a JOIN `sa_admins_groups` ag ON a.flags = ag.id"; 
        Database.Query(Database.SQL_FetchAdmins_CB, query, player.Slot);
    }
}
