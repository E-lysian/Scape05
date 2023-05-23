using Scape05.Engine.Combat;
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
    public int Health { get; set; } = 20;
    public int MaxHealth { get; set; } = 20;
    public ICombatManager CombatManager { get; set; }
    public int AnimationId { get; set; } = -1;
    public NPCMovementHandler MovementHandler { get; set; }
    public bool CanWalk { get; set; }
    public Face Face { get; set; }

    public NPC()
    {
        MovementHandler = new NPCMovementHandler(this);
        CombatManager = new MeleeCombatHandler(this);
        CombatManager.Weapon = new(4151, 1, 5, new CombatAnimations(422, 404, 1111), WeaponType.SWORD); //422, 404
    }
}