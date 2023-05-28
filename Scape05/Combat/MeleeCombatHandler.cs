using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Engine.Combat;

public class MeleeCombatHandler : ICombatManager
{
    private readonly IEntity _attacker;
    public bool ShouldInitiate { get; set; }
    public Weapon Weapon { get; set; }
    public bool InCombat { get; set; }
    public IEntity Target { get; set; }
    public bool IsInitiator { get; set; }
    private int Tick { get; set; }
    public int DamageTaken { get; set; } = -1;
    public CombatHit PerformedDamage { get; set; } = null;
    public bool TookDamage { get; set; }
    public int CombatAnimation { get; set; }


    public MeleeCombatHandler(IEntity attacker)
    {
        _attacker = attacker;
    }

    public void Initiate()
    {
        if (!ShouldInitiate) return;

        if (!InCombat)
        {
            InCombat = true;
            ConsoleColorHelper.Broadcast(2, $"{_attacker.Name} Initiated combat with {Target.Name}!");

            if (IsInitiator)
            {
                Tick = Weapon.Speed - 1;
            }
        }
    }

    public void Attack()
    {
        Initiate();

        if (InCombat)
        {
            Tick++;
            if (Tick % Weapon.Speed == 0)
            {
                Console.WriteLine($"{_attacker.Name} Attacking!");
                PerformedDamage = CalculateDamage();
                Target.CombatManager.Alert(_attacker);
                Tick = 0;

                _attacker.AnimationId = Weapon.Animation.AttackId;


                switch (_attacker)
                {
                    case Player playerEntity:
                        playerEntity.Flags |= PlayerUpdateFlags.Animation;
                        if (TookDamage)
                        {
                            playerEntity.Flags |= PlayerUpdateFlags.SingleHit;
                        }

                        playerEntity.IsUpdateRequired = true;
                        break;
                    case NPC npcEntity:
                        npcEntity.Flags |= NPCUpdateFlags.Animation;
                        if (TookDamage)
                        {
                            npcEntity.Flags |= NPCUpdateFlags.SingleHit;
                        }

                        npcEntity.IsUpdateRequired = true;
                        break;
                    default:
                        Console.WriteLine("Unknown entity type");
                        break;
                }

                Target.CombatManager.TakeDamage(PerformedDamage.Damage);

                /* If the target performed damage to us this tick, this resets every tick */
                if (Target.CombatManager.PerformedDamage != null)
                {
                    Target.AnimationId = Target.CombatManager.Weapon.Animation.AttackId;
                    switch (Target)
                    {
                        case Player playerEntity:
                            playerEntity.Flags |= PlayerUpdateFlags.SingleHit;
                            playerEntity.IsUpdateRequired = true;
                            break;
                        case NPC npcEntity:
                            npcEntity.Flags |= NPCUpdateFlags.SingleHit;
                            npcEntity.IsUpdateRequired = true;
                            break;
                        default:
                            Console.WriteLine("Unknown entity type");
                            break;
                    }
                }
                else
                {
                    Target.AnimationId = Target.CombatManager.Weapon.Animation.BlockId;
                    switch (Target)
                    {
                        case Player playerEntity:
                            playerEntity.Flags |= PlayerUpdateFlags.Animation;
                            playerEntity.Flags |= PlayerUpdateFlags.SingleHit;
                            playerEntity.IsUpdateRequired = true;
                            break;
                        case NPC npcEntity:
                            npcEntity.Flags |= NPCUpdateFlags.Animation;
                            npcEntity.Flags |= NPCUpdateFlags.SingleHit;
                            npcEntity.IsUpdateRequired = true;
                            break;
                        default:
                            Console.WriteLine("Unknown entity type");
                            break;
                    }
                }
            }
        }
    }

    public void Alert(IEntity target)
    {
        /* Set InteractingEntity */
        
        ConsoleColorHelper.Broadcast(3, $"Alert! {_attacker.Name} was attacked by {target.Name}! Retaliate!");
        Target = target;

        switch (Target)
        {
            case Player player:
                player.Flags |= PlayerUpdateFlags.InteractingEntity;
                player.InteractingEntityId = Target.Index;
                player.IsUpdateRequired = true;
                break;
            case NPC npc:
                npc.Flags |= NPCUpdateFlags.InteractingEntity;
                npc.InteractingEntityId = Target.Index + 32768;
                npc.IsUpdateRequired = true;
                break;
            default:
                break;
        }
        
        
        Initiate();
    }

    public void TakeDamage(int damage)
    {
        /* Set block flag */
        DamageTaken = damage;
        _attacker.Health -= damage;
        _attacker.CombatManager.TookDamage = true;
        ConsoleColorHelper.Broadcast(0, $"[{_attacker.Health}] {_attacker.Name} Took {damage} Damage!");
    }

    public void CheckWonBattle()
    {
        if (InCombat && Target.Health <= 0)
        {
            ConsoleColorHelper.Broadcast(1, $"{_attacker.Name} Won over {Target.Name}.");
            _attacker.Health = 10;
            InCombat = false;
            ShouldInitiate = false;
            Target = null;
        }
    }

    public void CheckLostBattle()
    {
        if (InCombat && _attacker.Health <= 0)
        {
            ConsoleColorHelper.Broadcast(2, $"{_attacker.Name} Lost to {Target.Name}.");
            InCombat = false;
            ShouldInitiate = false;
            Target = null;
        }
    }

    public CombatHit CalculateDamage()
    {
        return new CombatHit
        {
            Damage = 2,
            Delay = 0,
            Type = DamageType.Damage
        };
    }
}