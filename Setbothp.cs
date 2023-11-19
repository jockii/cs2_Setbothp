using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json;

namespace Setbothp;

public class Setbothp : BasePlugin
{
    public override string ModuleName => "Setbothp";

    public override string ModuleVersion => "v1.0.0";
    public override string ModuleAuthor => "jackson tougher";

    private static Config _config = null!;

    //private uint _INPUT_BOT_HP = 0;

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"Plugin {ModuleName} {ModuleVersion} by {ModuleAuthor} was loaded =D");
        _config = LoadConfig();
    }

    private Config LoadConfig()
    {
        var configPath = Path.Combine(ModuleDirectory, "setbothp.json");

        if (!File.Exists(configPath)) return CreateConfig(configPath);

        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath))!;

        return config;
    }

    private Config CreateConfig(string configPath)
    {
        var config = new Config
        {
            INPUT_BOT_HP = 100
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine("[SetBotHp] The configuration was successfully saved to a file: " + configPath);

        return config;
    }

    [ConsoleCommand("css_setbothp_reload")]
    public void OnCommandReloadConfig(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        _config = LoadConfig();

        const string msg = "configuration successfully rebooted!";
        Console.WriteLine(msg);
    }

    public void Log(string msg) 
    { 
        Server.PrintToChatAll(msg); 
    }


    public int STANDART_BOT_HP = 100;
    public const int MIN_BOT_HP = 1;
    public const int MAX_BOT_HP = 9999999;

    public void SetBotHp(List<CCSPlayerController> playersList)
    {
        playersList.ForEach(player =>
        {
            if (player.IsBot)
            {
                Log("--- find bot");
                if (1 >= MIN_BOT_HP  || 1 <= MAX_BOT_HP)
                {
                    player.Pawn.Value.Health = 1;
                    Log("--- set bot HP to 1HP");
                }
                else if (0 < MIN_BOT_HP || 200 > MAX_BOT_HP)
                {
                    player.Pawn.Value.Health = STANDART_BOT_HP;
                    Console.WriteLine($"[{ModuleName}] incorrect value. Bot health set to standart value: 100HP");
                }
            }
        });
    }
    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventRoundStart @event, GameEventInfo @info)
    {
        Log("--- Round start  ---");
        var players = Utilities.GetPlayers();
        SetBotHp(players);
        Log("-- call func SetBotHp() on start round ");
        return HookResult.Continue;
    }
    public class Config
    {
        public uint INPUT_BOT_HP { get; set; }
    }
}