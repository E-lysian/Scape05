using Scape05.Entities;

namespace Scape05.Engine.Combat;

public class DamageInfo
{
    public IEntity DamageSource { get; set; }
    public int Amount { get; set; }
    public DamageType Type { get; set; } /* 0 Block, 1 Damage, 2 Poison.. */
}