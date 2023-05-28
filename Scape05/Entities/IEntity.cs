using Scape05.Engine.Combat;
using Scape05.Handlers;

namespace Scape05.Entities;

public interface IEntity
{
    public int Index { get; set; }
    public string Name { get; set; }
    public Location Location { get; set; }
    public BuildArea BuildArea { get; set; }
    public int HeadIcon { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public int Size { get; set; }
    public int CombatLevel { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    ICombatManager CombatManager { get; set; }
    public int AnimationId { get; set; }
    public IEntity Follow { get; set; }
    
    void PerformBlockAnimation();
    void PerformAttackAnimation();
    void DisplayHitSplat();
    void NotifyAttacked(IEntity attacker);
    void PerformAnimation(int animId);
    public DelayedTaskHandler DelayedTaskHandler { get; set; }
    public ICombatBase CombatBase { get; set; }
    
}