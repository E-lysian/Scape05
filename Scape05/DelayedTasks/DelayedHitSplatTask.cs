using Scape05.Engine.Combat;
using Scape05.Entities;

namespace Scape05.Handlers;

public class DelayedHitSplatTask : IDelayedTask
{
    public int Delay { get; set; } = 0;
    public Action DelayedTask { get; set; }

    public DelayedHitSplatTask(NPC npc, DamageInfo info)
    {
        DelayedTask = () =>
        {
            npc.CombatMethod.DamageTaken = info;
            npc.DisplayHitSplat();
            npc.PerformBlockAnimation();
        };
    }
    
}