namespace Scape05.Engine.Combat;

public class CombatHit
{
    public int Damage { get; set; }
    public int Delay { get; set; } /* Should this hit splat be delayed, if so, by how many ticks? */
    public DamageType Type { get; set; } /* 0 Block, 1 Damage, 2 Poison.. */
}