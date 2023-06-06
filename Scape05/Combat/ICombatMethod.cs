using Scape05.Entities;

namespace Scape05.Engine.Combat;

public interface ICombatMethod
{
    void Attack();
    void TakeDamage(DamageInfo damage);
    public DamageInfo DamageTaken { get; set; }
    void SetAnimation();
    bool HasPerformedDamage { get; set; }
    bool HasTakenDamage { get; set; }
    public int CombatTick { get; set; }
    public bool CanCombat { get; set; }
}