using Scape05.Engine.Combat;
using Scape05.Handlers;
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
    public int Health { get; set; } = 25;
    public int MaxHealth { get; set; } = 25;
    public ICombatManager CombatManager { get; set; }
    public int AnimationId { get; set; } = -1;

    public void PerformBlockAnimation()
    {
        AnimationId = CombatManager.Weapon.Animation.BlockId;
        Flags |= NPCUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public void PerformAttackAnimation()
    {
        AnimationId = CombatManager.Weapon.Animation.AttackId;
        Flags |= NPCUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public void DisplayHitSplat()
    {
        Flags |= NPCUpdateFlags.SingleHit;
        IsUpdateRequired = true;
    }

    public void NotifyAttacked(IEntity attacker)
    {
        Console.WriteLine($"{Name} Notified Attack by: {attacker.Name}");
        // Engage in combat with the attacker
        CombatBase.Attacker = this;
        CombatBase.Target = attacker;
        CombatBase.Tick = 2;
    }

    public void PerformAnimation(int animId)
    {
        AnimationId = animId;
        Flags |= NPCUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public DelayedTaskHandler DelayedTaskHandler { get; set; } = new();

    public ICombatBase CombatBase { get; set; }
    public NPCMovementHandler MovementHandler { get; set; }
    public bool CanWalk { get; set; }
    public Face Face { get; set; }

    public NPC()
    {
        MovementHandler = new NPCMovementHandler(this);
        CombatManager = new MeleeCombatHandler(this);
        CombatManager.Weapon = new(4151, 1, 5, new CombatAnimations(422, 404, 1111), WeaponType.SWORD); //422, 404
        CombatBase  = new MeleeCombat
        {
            WeaponSpeed = 6
        };
    }

    public void Reset()
    {
        NeedsPlacement = false;
        IsUpdateRequired = false;
        Flags = NPCUpdateFlags.None;
        MovementHandler.PrimaryDirection = -1;
        MovementHandler.SecondaryDirection = -1;
        AnimationId = -1;
        CombatBase.DamageTaken = null;
    }
}