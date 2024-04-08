using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_AdminsList.Modules.Models;

namespace CS2_AdminsList.Modules;

internal static class Utils
{
    public static string PREFIX = " \x04[AdminsList]\x01";
    public static string PREFIX_MENU = " \x04[AdminsList]\x01";
    public static string PREFIX_CON = "[CS2_AdminsList]";

    public static void ReplyToCommand(CommandInfo commandInfo, string msg)
    {
        commandInfo.ReplyToCommand(msg);
    }

    public static void PrintToChatAll(string msg)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            player.PrintToChat(msg);
        }
    }

    public static void PrintToServer(string msg, ConsoleColor color = ConsoleColor.Cyan)
    {
        Console.ForegroundColor = color;

        msg = $"{PREFIX_CON} {msg}";
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public static void ThrowError(string msg)
    {
        throw new Exception(msg);
    }

    public static SteamID GetSteamID(CCSPlayerController Player)
    {
        SteamID? steamID = Player.AuthorizedSteamID;

        return steamID ?? null!;
    }

    public static SQL_Player GetPlayerFromSlot(int playerSlot)
    {
        return CS2_AdminsList.Players.Find(player => player.Player.Slot == playerSlot) ?? null!;
    }

    public static SQL_Player GetPlayerFromSteamID(string steamID)
    {
        return CS2_AdminsList.Players.Find((player) => {
            SteamID steamIdObj = GetSteamID(player.Player);
            return steamIdObj.SteamId64 == ulong.Parse(steamID);
        }) ?? null!;
    }

    public static SQL_Player GetPlayerFromSteamID(List<SQL_Player> list, string steamID)
    {
        return list.Find((player) => {
            SteamID steamIdObj = GetSteamID(player.Player);
            return steamIdObj.SteamId64 == ulong.Parse(steamID);
        }) ?? null!;
    }

    public static string ReplaceWithColor(string group)
    {
        if (group.Contains('{'))
		{
			string modifiedValue = group;
			foreach (FieldInfo field in typeof(ChatColors).GetFields())
			{
				string pattern = $"{{{field.Name}}}";
				if (group.Contains(pattern, StringComparison.OrdinalIgnoreCase))
				{
					modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)!.ToString(), StringComparison.OrdinalIgnoreCase);
				}
			}
            return modifiedValue;
		}

        return group;
    }
}
