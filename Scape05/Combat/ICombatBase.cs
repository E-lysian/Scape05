using Scape05.Entities;

namespace Scape05.Engine.Combat;

public interface ICombatBase
{
    public IEntity Attacker { get; set; }
    public IEntity Target { get; set; }
    public int Tick { get; set; }
    public int WeaponSpeed { get; set; }
    void Attack();
    
    public int DamageTaken { get; set; }
}