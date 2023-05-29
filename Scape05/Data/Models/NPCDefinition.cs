using Newtonsoft.Json;

namespace Scape05.Engine.Data.Models;

public class NPCDefinition
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("examine")] public string Examine { get; set; }

    [JsonProperty("combat")] public int Combat { get; set; }

    [JsonProperty("size")] public int Size { get; set; }

    [JsonProperty("attackable")] public bool Attackable { get; set; }

    [JsonProperty("aggressive")] public bool Aggressive { get; set; }

    [JsonProperty("retreats")] public bool Retreats { get; set; }

    [JsonProperty("poisonous")] public bool Poisonous { get; set; }

    [JsonProperty("respawn")] public int Respawn { get; set; }

    [JsonProperty("maxHit")] public int MaxHit { get; set; }

    [JsonProperty("hitpoints")] public int Hitpoints { get; set; }

    [JsonProperty("attackSpeed")] public int AttackSpeed { get; set; }

    [JsonProperty("attackAnim")] public int AttackAnim { get; set; }

    [JsonProperty("defenceAnim")] public int DefenceAnim { get; set; }

    [JsonProperty("deathAnim")] public int DeathAnim { get; set; }

    [JsonProperty("attackBonus")] public int? AttackBonus { get; set; }

    [JsonProperty("defenceMelee")] public int DefenceMelee { get; set; }

    [JsonProperty("defenceRange")] public int DefenceRange { get; set; }

    [JsonProperty("defenceMage")] public int DefenceMage { get; set; }
    
    

    [JsonProperty("strength")] public int Strength { get; set; }

    [JsonProperty("attack")] public int Attack { get; set; }

    [JsonProperty("spawnx")] public int SpawnX { get; set; }

    [JsonProperty("spawny")] public int SpawnY { get; set; }

    [JsonProperty("walk")] public int Walk { get; set; }
}