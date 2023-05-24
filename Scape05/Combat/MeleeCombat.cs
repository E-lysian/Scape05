using Scape05.Entities;

namespace Scape05.Engine.Combat;

public class MeleeCombat : ICombatBase
{
    public IEntity Attacker { get; set; }
    public IEntity Target { get; set; }
    public CombatHit DamageTaken { get; set; } = null;
    public bool NeedsToInitiate { get; set; }
    public int Tick { get; set; }
    public int WeaponSpeed { get; set; }

    public void Attack()
    {
        if (Target == null || Attacker == null)
            return;
        Console.WriteLine($"CombatTick: {Tick}");

        if (NeedsToInitiate)
        {
            Attacker.CombatBase.Tick = Attacker.CombatBase.WeaponSpeed;
            NeedsToInitiate = false;
        }
        
        if (Attacker.CombatBase.Tick >= Attacker.CombatBase.WeaponSpeed)
        {
            var damage  = CalculateDamage();
            Target.CombatBase.DamageTaken = damage;
            Target.Health -= damage.Damage;
            
            Console.WriteLine($"{Attacker.Name} Attacked: {Target.Name}.");
            Attacker.CombatBase.Tick = 0;

            if (Target.CombatBase.Target != Attacker)
            {
                Target.NotifyAttacked(Attacker);
            }
        }

        Attacker.CombatBase.Tick++;
    }

    private CombatHit CalculateDamage()
    {
        return new CombatHit
        {
            Damage = new Random().Next(1,5),
            Type = DamageType.Damage
        };
    }
}