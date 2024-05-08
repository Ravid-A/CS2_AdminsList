using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CS2_AdminsList.Modules.Models;

namespace CS2_AdminsList.Modules.Handlers;

public static class Listeners
{
    public static void RegisterListeners()
    {
        CS2_AdminsList Plugin = CS2_AdminsList.Instance;
        Plugin.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnMapEnd>(OnMapEnd);
        Plugin.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnClientAuthorized>(OnClientAuthorized);
        Plugin.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnClientDisconnect>(OnClientDisconnect);
    }  

    public static void OnMapEnd()
    {
        CS2_AdminsList.Players.Clear();
    }

    public static void OnClientAuthorized(int playerSlot, SteamID steamID)
    {
        CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);
        SteamID playerSteamID = Utils.GetSteamID(player);

        if (playerSteamID == null)
        {
            Utils.PrintToServer("Failed to get player steamID", ConsoleColor.Red);
            return;
        }

        string query = string.Format("SELECT `name`, `show_in_list` FROM `admin_list_settings` WHERE `auth` = '{0}'", playerSteamID.SteamId64.ToString());
        Database.Query(Database.SQL_FetchUser_CB, query, playerSlot);
    }

    public static void OnClientDisconnect(int playerSlot)
    {
        int index = CS2_AdminsList.Players.FindIndex(player => player.Player.Slot == playerSlot);
        
        if (index == -1)
        {
            return;
        }

        CS2_AdminsList.Players.RemoveAt(index);
    }
}
