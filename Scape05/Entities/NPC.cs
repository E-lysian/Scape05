using Scape05.Engine.Combat;
using Scape05.Handlers;
using Scape05.Misc;

namespace Scape05.Entities;

public class NPC : IEntity
{
    public int GraphicsId { get; set; }
    public int Index { get; set; }
    public string Name { get; set; }
    public int ModelId { get; set; }
    public Location Location { get; set; }
    public BuildArea BuildArea { get; set; }
    public int HeadIcon { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public NPCUpdateFlags Flags { get; set; }
    public int Size { get; set; } = -1;
    public int CombatLevel { get; set; } = 1;
    public int Health { get; set; } = 25;
    public int MaxHealth { get; set; } = 25;
    public int AnimationId { get; set; } = -1;
    
    public void DisplayHitSplat()
    {
        Flags |= NPCUpdateFlags.SingleHit;
        IsUpdateRequired = true;
    }

    public Weapon Weapon { get; set; }

    public void PerformAnimation(int animId)
    {
        AnimationId = animId;
        Flags |= NPCUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public DelayedTaskHandler DelayedTaskHandler { get; set; } = new();
    
    public NPCMovementHandler MovementHandler { get; set; }
    public bool CanWalk { get; set; }
    public Face Face { get; set; }
    
    public IEntity Follow { get; set; } = null;
    public ICombatMethod CombatMethod { get; set; }
    public IEntity CombatTarget { get; set; }
    
    public int InteractingEntityId { get; set; } = 0x00FFFF;
    public bool Dead { get; set; }

    public NPC()
    {
        MovementHandler = new NPCMovementHandler(this);
    }

    public void PerformBlockAnimation()
    {
        AnimationId = Weapon.Animation.BlockId;
        Flags |= NPCUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public void PerformAttackAnimation()
    {
        AnimationId = Weapon.Animation.AttackId;
        Flags |= NPCUpdateFlags.Animation;
        IsUpdateRequired = true;
    }
    
    public void Reset()
    {
        NeedsPlacement = false;
        IsUpdateRequired = false;
        Flags = NPCUpdateFlags.None;
        //MovementHandler.PrimaryDirection = -1;
        //MovementHandler.SecondaryDirection = -1;
        AnimationId = -1;
    }
}