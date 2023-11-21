using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Setbothp;

[MinimumApiVersion(65)]
public class Setbothp : BasePlugin
{
    public override string ModuleName => "Setbothp";

    public override string ModuleVersion => "v1.1.0";
    public override string ModuleAuthor => "| jackson tougher |";
    public override void Load(bool hotReload)
    {
        Console.WriteLine($"Plugin [Setbothp] {ModuleVersion} by {ModuleAuthor} was loaded =D");
    }
    public void LogToChatAll(string msg)
    {
        Server.PrintToChatAll(msg);
    }
    public void SendConsoleCommand(string command)
    {
        Server.ExecuteCommand(command);
    }

    public const int STANDART_BOT_HP = 100;
    public const int MIN_BOT_HP = 1;
    public const int MAX_BOT_HP = 9999999;
    public int SET_BOT_HP = 0;

    [ConsoleCommand("css_setbothp")]
    public void OnCommandSetBotHp(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        if (controller.SteamID == 1111111111111 || controller.SteamID == 00000000000000) 
        {
            if (Regex.IsMatch(command.GetArg(1), @"^\d+$"))
            {
                SET_BOT_HP = int.Parse(command.GetArg(1));
                controller.PrintToChat($"{ChatColors.Red}[Setbothp]{ChatColors.Olive}config reload... {ChatColors.Green}OK!");
                controller.PrintToChat($"New Bot HP: {ChatColors.Green}{SET_BOT_HP}");
            }
            else
            {
                SET_BOT_HP = STANDART_BOT_HP;
                controller.PrintToChat($"{ChatColors.Red}Incorrect value! Please input correct number");
            }
        }
        else
            controller.PrintToChat($"{ChatColors.Red}You are not Admin!!!");
    }
    public void SetBotHp(List<CCSPlayerController> playersList, int bot_hp)
    {
        playersList.ForEach(player =>
        {
            if (player.IsBot)
            {
                //LogToChatAll("--- find bot");
                if (bot_hp >= MIN_BOT_HP && bot_hp <= MAX_BOT_HP)
                {
                    player.Pawn.Value.Health = bot_hp;
                    //($"--- set bot HP to {bot_hp}HP <--");
                }
                else if (bot_hp < MIN_BOT_HP || bot_hp > MAX_BOT_HP)
                {
                    player.Pawn.Value.Health = STANDART_BOT_HP;
                    //LogToChatAll($"else if === STANDART_BOT_HP");
                    SendConsoleCommand($"[{ModuleName}] incorrect value. Bot health set to standart value: 100HP");
                }
            }
        });
    }
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo @info)
    {
        LogToChatAll(SET_BOT_HP.ToString());
        //LogToChatAll("--- Round Event  ---");
        var players = Utilities.GetPlayers();
        SetBotHp(players, SET_BOT_HP);
        //LogToChatAll("-- call func SetBotHp() on start round ");
        return HookResult.Continue;
    }
}