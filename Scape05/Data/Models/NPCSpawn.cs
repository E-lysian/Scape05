using Newtonsoft.Json;

namespace Scape05.Engine.Data.Models;

public class NPCSpawn
{
    [JsonProperty("maxHit")] public int MaxHit { get; set; }

    [JsonProperty("strength")] public int Strength { get; set; }

    [JsonProperty("attack")] public int Attack { get; set; }

    [JsonProperty("x")] public int X { get; set; }

    [JsonProperty("y")] public int Y { get; set; }

    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("walk")] public int Walk { get; set; }

    [JsonProperty("height")] public int Height { get; set; }
}