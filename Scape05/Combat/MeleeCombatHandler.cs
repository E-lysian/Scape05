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

                var damage = CalculateDamage();
                Target.CombatManager.TakeDamage(damage.Damage);
                Target.CombatManager.Alert(_attacker);
                Tick = 0;
            }
        }
    }

    public void Alert(IEntity target)
    {
        ConsoleColorHelper.Broadcast(3, $"Alert! {_attacker.Name} was attacked by {target.Name}! Retaliate!");
        Target = target;
        ShouldInitiate = true;
        Initiate();
    }

    public void TakeDamage(int damage)
    {
        _attacker.Health -= damage;
        ConsoleColorHelper.Broadcast(0, $"[{_attacker.Health}] {_attacker.Name} Took {damage} Damage!");
    }

    public void CheckWonBattle()
    {
        if (InCombat && Target.Health <= 0)
        {
            ConsoleColorHelper.Broadcast(1, $"{_attacker.Name} Won over {Target.Name}.");
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