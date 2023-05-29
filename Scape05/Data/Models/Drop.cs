using Newtonsoft.Json;

namespace Scape05.Engine.Data.Models;

public class Drop
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("items")]
    public List<Item> Items { get; set; }
}

public class Item
{
    [JsonProperty("item_id")]
    public int ItemId { get; set; }

    [JsonProperty("chance")]
    public int Chance { get; set; }

    [JsonProperty("amounts")]
    public List<int> Amounts { get; set; }
}