using Scape05.Misc;

namespace Scape05.Entities;

public class NPC : IEntity
{
    public int Index { get; set; }
    public string Name { get; set; }
    public int ModelId { get; set; }
    public Location Location { get; set; }
    public int HeadIcon { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public NPCUpdateFlags Flags { get; set; }
    public int Size { get; set; } = -1;
    public int CombatLevel { get; set; } = 1;
}