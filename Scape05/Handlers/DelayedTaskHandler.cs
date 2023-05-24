namespace Scape05.Handlers;

public class DelayedTaskHandler
{
    private readonly List<IDelayedTask> delayedTasks = new();

    public void RegisterDelayedTask(IDelayedTask delayedTask)
    {
        delayedTasks.Add(delayedTask);
    }

    public void HandleDelayedTasks()
    {
        foreach (var tickTask in delayedTasks.ToList())
            if (tickTask.Delay <= 0)
            {
                tickTask.DelayedTask.Invoke();
                delayedTasks.Remove(tickTask);
            }
            else
            {
                tickTask.Delay--;
            }
    }
}