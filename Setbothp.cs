using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Setbothp;
public class Config
{
    public List<ulong> admins { get; set; } = new List<ulong>();
    public int bot_HP { get; set; }
}

[MinimumApiVersion(65)]
public class Setbothp : BasePlugin
{
    public override string ModuleName => "Setbothp";

    public override string ModuleVersion => "v2.0.0";

    public override string ModuleAuthor => "jackson tougher";
    public Config config = new Config();
    public override void Load(bool hotReload)
    {
        var configPath = Path.Join(ModuleDirectory, "Config.json");
        if (!File.Exists(configPath))
        {
            config.admins.Add(76561199414091272); config.admins.Add(76561199414091272);
            config.bot_HP = 100;
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        }
        else config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));

        Console.WriteLine($"Plugin: {ModuleName} ver:{ModuleVersion} by {ModuleAuthor} has been loaded =)");
    }
    public const int MIN_BOT_HP = 1;                                  //
    public const int STANDART_BOT_HP = 100;                          //
    public const int MAX_BOT_HP = 9999999;                          //
    public void OnConfigReload()
    {
        var configPath = Path.Join(ModuleDirectory, "Config.json");
        config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));
    }
    public void SetBotHp(List<CCSPlayerController> playersList)
    {
        playersList.ForEach(player =>
        {
            if (player.IsValid && player.IsBot && !player.IsHLTV)
            {
                if (config.bot_HP >= MIN_BOT_HP && config.bot_HP <= MAX_BOT_HP)
                    player.Pawn.Value.Health = config.bot_HP;
                else if (config.bot_HP < MIN_BOT_HP || config.bot_HP > MAX_BOT_HP)
                    player.Pawn.Value.Health = STANDART_BOT_HP;
            }
        });
    }
    [ConsoleCommand("css_bot_hp")]
    public void OnCommandSetBotHp(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        if (config.admins.Exists(adminID => adminID == controller.SteamID))
        {
            if (Regex.IsMatch(command.GetArg(1), @"^\d+$"))
            {
                if (int.Parse(command.GetArg(1)) == 0)
                {
                    controller.PrintToChat($" {ChatColors.Red}[ {ChatColors.Purple}Botiki {ChatColors.Red}] {ChatColors.Red}Bot HP can`t be zero!");
                }
                else
                {
                    config.bot_HP = int.Parse(command.GetArg(1));
                    controller.PrintToChat($" {ChatColors.Red}[ {ChatColors.Purple}Botiki {ChatColors.Red}] {ChatColors.Default}New Bot HP: {ChatColors.Green}{config.bot_HP}");
                }
            }
            else
            {
                controller.PrintToChat($" {ChatColors.Red}Incorrect value! Please input correct number");
            }
        }
        else
            controller.PrintToChat($" {ChatColors.Red}You are not Admin!!!");
    }
    [ConsoleCommand("css_bothp_reload")]
    public void OnBotikiConfigReload(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        if (config.admins.Exists(adminID => adminID == controller.SteamID))
        {
            OnConfigReload();
            controller.PrintToChat($" {ChatColors.Red}[ {ChatColors.Purple}Botiki {ChatColors.Red}] {ChatColors.Olive}...configuration was reloaded. {ChatColors.Green}OK!");
        }
        else
            controller.PrintToChat($" {ChatColors.Red}You are not Admin!!!");
    }
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        SetBotHp(Utilities.GetPlayers());
        return HookResult.Continue;
    }
}