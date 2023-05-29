using Newtonsoft.Json;

namespace Scape05.Engine.Data.Models;

public class GlobalDrop
{
    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("itemX")]
    public int ItemX { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("itemY")]
    public int ItemY { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }
}