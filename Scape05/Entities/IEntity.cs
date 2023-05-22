namespace Scape05.Entities;

public interface IEntity
{
    public int Index { get; set; }
    public string Name { get; set; }
    public Location Location { get; set; }
    public int HeadIcon { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public int Size { get; set; }
    public int CombatLevel { get; set; }
}