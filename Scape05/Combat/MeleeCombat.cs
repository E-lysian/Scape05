using Scape05.Entities;

namespace Scape05.Engine.Combat;

public class MeleeCombat : ICombatBase
{
    public IEntity Attacker { get; set; }
    public IEntity Target { get; set; }
    public int DamageTaken { get; set; } = -1;
    public int Tick { get; set; }
    public int WeaponSpeed { get; set; }

    public void Attack()
    {
        if (Target == null || Attacker == null)
            return;
        Console.WriteLine($"CombatTick: {Tick}");

        if (Attacker.CombatBase.Tick >= Attacker.CombatBase.WeaponSpeed)
        {
            Target.CombatBase.DamageTaken = 2;
            Console.WriteLine($"{Attacker.Name} Attacked: {Target.Name}.");
            Attacker.CombatBase.Tick = 0;

            if (Target.CombatBase.Target != Attacker)
            {
                Target.NotifyAttacked(Attacker);
            }
        }

        Attacker.CombatBase.Tick++;
    }
}