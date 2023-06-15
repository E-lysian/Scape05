using Scape05.Entities;

namespace Scape05.Handlers;

public class DelayedProjectileTask : IDelayedTask
{
    public int Delay { get; set; } = 0;
    public Action DelayedTask { get; set; }

    public DelayedProjectileTask(Action action)
    {
        DelayedTask = action;
    }
}