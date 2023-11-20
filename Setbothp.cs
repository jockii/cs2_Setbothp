using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace Setbothp;

public class Setbothp : BasePlugin, IPluginConfig<SetbothpConfig>
{
    public override string ModuleName => "Setbothp";

    public override string ModuleVersion => "v1.0.0";
    public override string ModuleAuthor => "\"jackson tougher\"";

    public SetbothpConfig Config { get; set; } = new();

    private int _SET_BOT_HP = 0;

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"Plugin [Setbothp] {ModuleVersion} by {ModuleAuthor} was loaded =D");
    }

    public void OnConfigParsed(SetbothpConfig config)
    {
        this.Config = config;
        _SET_BOT_HP = config.SET_BOT_HP;
    }

    public async void Pause()
    {
        await Task.Delay(3000);
    }
    public void Log(string msg) 
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

    [ConsoleCommand("css_setbothp_reload")] 
    public void OnCommandSetBotHp(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        SendConsoleCommand("css_plugins stop Setbothp");
        Pause();
        SendConsoleCommand("css_plugins start Setbothp");

        controller.PrintToChat("");
        //command.ReplyToCommand("");

        const string msg = "[Setbothp] configuration successfully rebooted!";
        Console.WriteLine(msg);
    }

    public void SetBotHp(List<CCSPlayerController> playersList)
    {
        playersList.ForEach(player =>
        {
            if (player.IsBot)
            {
                //Log("--- find bot");
                if (_SET_BOT_HP >= MIN_BOT_HP  || _SET_BOT_HP <= MAX_BOT_HP)
                {
                    player.Pawn.Value.Health = _SET_BOT_HP;
                    //Log($"--- set bot HP to {_INPUT_BOT_HP}HP <--");
                }
                else if (_SET_BOT_HP < MIN_BOT_HP || _SET_BOT_HP > MAX_BOT_HP)
                {
                    player.Pawn.Value.Health = STANDART_BOT_HP;
                    //Log($"else if === STANDART_BOT_HP");
                    Console.WriteLine($"[{ModuleName}] incorrect value. Bot health set to standart value: 100HP");
                }
            }
        });
    }
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo @info)
    {
        //Log("--- Round Event  ---");
        var players = Utilities.GetPlayers();
        SetBotHp(players);
        //Log("-- call func SetBotHp() on start round ");
        return HookResult.Continue;
    }
}