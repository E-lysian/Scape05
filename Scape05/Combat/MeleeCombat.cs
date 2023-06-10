using Scape05.Entities;
using Scape05.Handlers;
using Scape05.Misc;
using Scape05.World;

namespace Scape05.Engine.Combat;

public class MeleeCombat : ICombatMethod
{
    private readonly IEntity _owner;
    public bool HasPerformedDamage { get; set; }
    public bool HasTakenDamage { get; set; }
    public int CombatTick { get; set; }
    public DamageInfo DamageTaken { get; set; }
    public bool SkipTick { get; set; }

    public bool CanCombat { get; set; } = true;


    public MeleeCombat(IEntity owner)
    {
        _owner = owner;
    }

    public void Attack()
    {
        if (_owner.Weapon.Speed == 0 || _owner.Weapon == null || !CanCombat)
            return;

        /* Extra check to see if player is in combat? */

        /* If inside the target, step away */

        if (_owner.CombatTarget != null && _owner.CombatMethod.CanCombat)
        {
            var withinRange = WithinRange();
            Console.WriteLine(
                $"[{_owner.Name}] Within Range: {withinRange} of {_owner.CombatTarget} Size: {_owner.CombatTarget.Size}");
            if (!withinRange)
            {
                return;
            }

            _owner.InCombat = true;
            if (SkipTick)
            {
                SkipTick = false;
                return;
            }

            if (CombatTick % _owner.Weapon.Speed == 0)
            {
                if (_owner.CombatTarget != null)
                {
                    _owner.CombatTarget.CombatMethod.TakeDamage(new DamageInfo
                    {
                        DamageSource = _owner,
                        Amount = 5,
                        Type = DamageType.Damage
                    });

                    HasPerformedDamage = true;

                    Console.WriteLine($"+ [{_owner.Name}] Attacked: [{_owner.CombatTarget.Name}]");
                }

                CombatTick = 0;
            }

            if (CanCombat)
            {
                Console.WriteLine($"[{_owner.Name}]: CanCombat!");
                CombatTick++;
            }
        }
    }

    private bool WithinRange()
    {
        var horizontally = _owner.Location.X >= _owner.CombatTarget.Location.X - _owner.Size &&
                           _owner.Location.X <= _owner.CombatTarget.Location.X + _owner.CombatTarget.Size;
        var vertically = _owner.Location.Y >= _owner.CombatTarget.Location.Y - _owner.Size &&
                         _owner.Location.Y <= _owner.CombatTarget.Location.Y + _owner.CombatTarget.Size;

        var withinRange = horizontally && vertically;
        return withinRange;
    }


    public void TakeDamage(DamageInfo info)
    {
        DamageTaken = info;
        // InCombat = true;
        if (_owner.CombatTarget == null)
        {
            _owner.CombatTarget = info.DamageSource;
            SkipTick = true;
            
        }
        
        if (_owner is NPC)
        {
            var npc = (NPC)_owner;
            npc.Follow = _owner.CombatTarget;
                
            npc.Flags |= NPCUpdateFlags.InteractingEntity;
            npc.InteractingEntityId = npc.Follow.Index + 32768;
        }
        
        if (_owner is Player)
        {
            var player = (Player)_owner;
            player.Follow = _owner.CombatTarget;
                
            player.Flags |= PlayerUpdateFlags.InteractingEntity;
            player.InteractingEntityId = _owner.CombatTarget.Index;
        }

        if (info.Amount > _owner.Health)
        {
            info.Amount = _owner.Health;
            _owner.Health -= _owner.Health;
        }
        else
        {
            _owner.Health -= info.Amount;
        }

        _owner.DisplayHitSplat();
        HasTakenDamage = true;

        if (_owner.Health <= 0)
        {
            _owner.InCombat = false;
            _owner.CombatTarget.InCombat = false;

            _owner.CombatMethod.CanCombat = false;
            _owner.CombatTarget.CombatMethod.CanCombat = false;
            
            HasPerformedDamage = false;
            HasTakenDamage = false;

            _owner.DelayedTaskHandler.RegisterDelayedTask(new BattleEndDelayedTask(_owner));
        }
    }


    public void SetAnimation()
    {
        if (_owner.Health <= 0)
        {
            _owner.PerformAnimation(_owner.Weapon.Animation.FallAnim);
            return;
        }
        if (HasPerformedDamage)
        {
            /* Set Attack Animation */
            _owner.PerformAttackAnimation();
        }
        else if (HasTakenDamage)
        {
            /* Set Block Animation */
            _owner.PerformBlockAnimation();
        }

        HasPerformedDamage = false;
        HasTakenDamage = false;
    }
}