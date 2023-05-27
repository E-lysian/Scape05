using System.Numerics;
using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Misc;
using Scape05.World;

namespace Scape05.Engine.Combat;

public class MeleeCombat : ICombatBase
{
    public IEntity Attacker { get; set; }
    public IEntity Target { get; set; }
    public CombatHit DamageTaken { get; set; } = null;
    public bool NeedsToInitiate { get; set; }
    public bool InCombat { get; set; }
    public int Tick { get; set; }
    public int WeaponSpeed { get; set; }

    public void Attack()
    {
        if (Target == null || Attacker == null)
            return;
        Console.WriteLine($"CombatTick: {Tick}");

        // var canMove = Region.canMove(Attacker.Location.X, Attacker.Location.Y, Target.Location.X, Target.Location.Y, 0, 1, 1);
        // if (!canMove)
        // {
        //     return;
        // }
        
        if (CanMeleeAttack())
        {
            

            if (Attacker is Player)
            {
                PacketBuilder.SendMessage("Can attack from here!", (Player)Attacker);
            }

            if (NeedsToInitiate)
            {
                Attacker.CombatBase.Tick = Attacker.CombatBase.WeaponSpeed;
                NeedsToInitiate = false;
                Attacker.CombatBase.InCombat = true;
            }

            if (Attacker.CombatBase.Tick >= Attacker.CombatBase.WeaponSpeed)
            {
                if (Target.CombatBase.Target != Attacker)
                {
                    Target.NotifyAttacked(Attacker);
                }

                var damage = CalculateDamage();
                Target.CombatBase.DamageTaken = damage;
                Target.Health -= damage.Damage;

                Console.WriteLine($"{Attacker.Name} Attacked: {Target.Name}.");
                Attacker.CombatBase.Tick = 0;

                if (Target.Health <= 0)
                {
                    ConsoleColorHelper.Broadcast(1, $"{Attacker.Name} won over {Target.Name}");
                }
            }

            Attacker.CombatBase.Tick++;
        }
        else
        {
            if (Attacker is Player)
            {
                PacketBuilder.SendMessage("Can't attack from here..", (Player)Attacker);
            }
        }
    }

    private bool CanMeleeAttack()
    {
        var horizontally = Attacker.Location.X >= Target.Location.X - 1 &&
                           Attacker.Location.X <= Target.Location.X + Target.Size;
        var vertically = Attacker.Location.Y >= Target.Location.Y - 1 &&
                         Attacker.Location.Y <= Target.Location.Y + Target.Size;

        var delta = Location.Delta(Attacker.Location, Target.Location);

        var t = Math.Abs(delta.X) == Math.Abs(delta.Y) && delta.X != 0 && delta.Y != 0;


        return horizontally && vertically & !t;
    }

    private CombatHit CalculateDamage()
    {
        return new CombatHit
        {
            // Damage = new Random().Next(1,5),
            Damage = 5,
            Type = DamageType.Damage
        };
    }
}