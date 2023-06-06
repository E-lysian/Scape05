using Scape05.Entities;
using Scape05.Handlers;

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

        /* Within Range? */
        /* Extra check to see if player is in combat? */

        if (_owner.CombatTarget != null && _owner.CombatMethod.CanCombat)
        {
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


    public void TakeDamage(DamageInfo info)
    {
        DamageTaken = info;
        // InCombat = true;
        if (_owner.CombatTarget == null)
        {
            _owner.CombatTarget = info.DamageSource;
            SkipTick = true;
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
            _owner.PerformAnimation(_owner.Weapon.Animation.FallAnim);


            _owner.DelayedTaskHandler.RegisterDelayedTask(new BattleEndDelayedTask(_owner));
        }
    }


    public void SetAnimation()
    {
        if (_owner.Health <= 0)
        {
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