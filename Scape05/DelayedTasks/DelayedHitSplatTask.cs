using Scape05.Engine.Combat;
using Scape05.Entities;

namespace Scape05.Handlers;

public class DelayedHitSplatTask : IDelayedTask
{
    public int Delay { get; set; } = 1;
    public Action DelayedTask { get; set; }

    public DelayedHitSplatTask(NPC npc, Action damageInvoke)
    {
        DelayedTask = () =>
        {
            damageInvoke.Invoke();
        };
    }
    
}