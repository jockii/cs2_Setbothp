using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Setbothp;

public class SetbothpConfig : BasePluginConfig
{
    [JsonPropertyName("INPUT_BOT_HP")]
    public int INPUT_BOT_HP { get; set; } = 100;
}