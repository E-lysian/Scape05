using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Misc;

namespace Scape05.Handlers;

public class BattleEndDelayedTask : IDelayedTask
{
    private readonly IEntity _entity;

    public BattleEndDelayedTask(IEntity entity)
    {
        // _entity = entity;
        // _entity.Health = entity.MaxHealth;
        //
        // Console.WriteLine($"+ Tick Registered: {nameof(BattleEndDelayedTask)} with a delay of {Delay} ticks.");
        //
        //
        //
        // switch (entity)
        // {
        //     case Player player:
        //         DelayedTask = () =>
        //         {
        //             ConsoleColorHelper.Broadcast(1, $"+ {nameof(BattleEndDelayedTask)} invoked.");
        //             PacketBuilder.Respawn(player);
        //             /* No longer in combat */
        //             /* Reset animation */
        //         };
        //         break;
        //     case NPC npc:
        //         if (npc.Dead)
        //             return;
        //
        //         var target = new Player();
        //         target = (Player)npc.CombatBase.Target;
        //         
        //         Delay = 3;
        //         DelayedTask = () =>
        //         {
        //             ConsoleColorHelper.Broadcast(1, $"+ {nameof(BattleEndDelayedTask)} invoked.");
        //             PacketBuilder.SendGroundItemPacket(npc.Location, 531, 1, target);
        //             npc.Dead = true;
        //             npc.DelayedTaskHandler.RegisterDelayedTask(new SpawnNPCDelayedTask(8, npc));
        //         };
        //         break;
        //     default:
        //         ConsoleColorHelper.Broadcast(0, "what the heck");
        //         break;
        // }
        //
        // if (_entity.CombatBase.Target != null)
        // {
        //     _entity.CombatBase.Target.CombatBase.Target = null;
        //     _entity.CombatBase.Target = null;
        // }
    }

    public int Delay { get; set; } = 3;
    public Action DelayedTask { get; set; }
}

public class SpawnNPCDelayedTask : IDelayedTask
{
    public int Delay { get; set; }

    public SpawnNPCDelayedTask(int delay, NPC npc)
    {
        Delay = delay;
        DelayedTask = () =>
        {
            npc.Dead = false;
            ConsoleColorHelper.Broadcast(2, $"[{npc.Index}] {npc.Name} has respawned.");
        };
    }

    public Action DelayedTask { get; set; }
}