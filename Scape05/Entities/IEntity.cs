using Scape05.Engine.Combat;

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
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    ICombatManager CombatManager { get; set; }
    public int AnimationId { get; set; }
}