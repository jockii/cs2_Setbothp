using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Setbothp;

public class SetbothpConfig : BasePluginConfig
{
    [JsonPropertyName("SET_BOT_HP")]
    public int SET_BOT_HP { get; set; } = 100;
}