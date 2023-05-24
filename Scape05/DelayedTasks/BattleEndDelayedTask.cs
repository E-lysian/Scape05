using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Misc;

namespace Scape05.Handlers;

public class BattleEndDelayedTask : IDelayedTask
{
    private readonly IEntity _entity;

    public BattleEndDelayedTask(IEntity entity)
    {
        Console.WriteLine($"+ Tick Registered: {nameof(BattleEndDelayedTask)} with a delay of {Delay} ticks.");
        _entity = entity;
        _entity.Health = entity.MaxHealth;
        
        
        switch (entity)
        {
            case Player player:
                DelayedTask = () =>
                {
                    ConsoleColorHelper.Broadcast(1, $"+ {nameof(BattleEndDelayedTask)} invoked.");
                    ConsoleColorHelper.Broadcast(1, $"+ Entity: {_entity.Name} has respawned.");
                    PacketBuilder.Respawn(player);
                    /* No longer in combat */
                    /* Reset animation */
                };
                break;
            case NPC npc:
                DelayedTask = () =>
                {
                    ConsoleColorHelper.Broadcast(1, $"+ {nameof(BattleEndDelayedTask)} invoked.");
                    ConsoleColorHelper.Broadcast(1, $"+ Entity: {_entity.Name} has respawned.");
                };
                break;
            default:
                ConsoleColorHelper.Broadcast(0, "what the heck");
                break;
        }

        
        
    }

    public int Delay { get; set; } = 3;
    public Action DelayedTask { get; set; }
}