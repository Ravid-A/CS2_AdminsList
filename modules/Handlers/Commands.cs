using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CS2_AdminsList.Modules.Models;

namespace CS2_AdminsList.Modules.Handlers;

internal static class Commands
{
    public static void RegisterCommands()
    {
        CS2_AdminsList.Instance.AddCommand("css_admins", "Opens the admins list menu", AdminsListCommand);
        CS2_AdminsList.Instance.AddCommand("css_admins_reloadconfig", "Reloads the config file", ReloadConfigCommand);
    }

    public static void UnRegisterCommands()
    {
        CS2_AdminsList.Instance.RemoveCommand("css_admins", AdminsListCommand);
        CS2_AdminsList.Instance.RemoveCommand("css_admins_reloadconfig", ReloadConfigCommand);
    }

    private static void AdminsListCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
         if (player == null)
        {
            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} This command can only be executed by a player.");
            return;
        }

        if (!player.IsValid)
        {
            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} This command can only be executed by a valid player.");
            return;
        }

        if(commandInfo.ArgCount > 1)
        {
            string arg = commandInfo.GetArg(1);

            if(arg != "0" && arg != "1")
            {
                Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} Invalid argument. Use 0 to be invisible and 1 to be visible in the admin list.");
                return;
            }

            bool state = int.Parse(commandInfo.GetArg(1)) == 1;
            Database.SQL_UpdateUser(player, state);

            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} You'll now be {(state ? "":"in")}visible in the admin list");

            return;
        }

        CS2_AdminsList.ShowAdminsList(player);
    }

    [RequiresPermissions("@css/root")]
    private static void ReloadConfigCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} This command can only be executed by a player.");
            return;
        }

        if (!player.IsValid)
        {
            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} This command can only be executed by a valid player.");
            return;
        }

        CS2_AdminsList._Config = Configs.LoadConfig();

        if(CS2_AdminsList._Config == null)
        {
            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} Failed to load config");
            return;
        }

        if(!CS2_AdminsList._Config.IsValid())
        {
            Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} Config is not valid");
            return;
        }

        Utils.ReplyToCommand(commandInfo, $"{Utils.PREFIX} Config reloaded");
    }
}
