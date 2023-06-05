using Scape05.Entities;

namespace Scape05.Engine.Combat;

public class MeleeCombat : ICombatMethod
{
    private readonly IEntity _owner;
    public bool HasPerformedDamage { get; set; }
    public bool HasTakenDamage { get; set; }
    public int CombatTick { get; set; }
    public DamageInfo DamageTaken { get; set; }
    public bool SkipTick { get; set; }

    public MeleeCombat(IEntity owner)
    {
        _owner = owner;
    }

    public void Attack()
    {
        if (_owner.Weapon.Speed == 0 || _owner.Weapon == null)
            return;
        
        /* Extra check to see if player is in combat? */
        
        if (_owner.CombatTarget != null)
        {
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
                        Amount = 1,
                        Type = DamageType.Damage
                    });

                    HasPerformedDamage = true;

                    Console.WriteLine($"+ [{_owner.Name}] Attacked: [{_owner.CombatTarget.Name}]");
                }

                CombatTick = 0;
            }

            CombatTick++;
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        DamageTaken = info;

        if (_owner.CombatTarget == null)
        {
            _owner.CombatTarget = info.DamageSource;
            SkipTick = true;
        }

        _owner.Health -= info.Amount;
        HasTakenDamage = true;
        _owner.DisplayHitSplat();
    }


    public void SetAnimation()
    {
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