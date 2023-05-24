namespace Scape05.Handlers;

public interface IDelayedTask
{
    public int Delay { get; set; } /* Delay in ticks before executed */
    public Action DelayedTask { get; set; } /* Action to perform when ready */
}