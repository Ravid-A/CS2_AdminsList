using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace CS2_AdminsList.Modules.Models;

public class SQL_Player
{
    public CCSPlayerController Player { get; set; } = null!;
    public string Name => Player.PlayerName;
    public bool ShowInList { get; set; } = true;

    public SQL_Player(CCSPlayerController player, bool showInList)
    {
        Player = player;
        ShowInList = showInList;
    }
}