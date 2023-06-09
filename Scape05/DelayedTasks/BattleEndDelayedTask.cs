using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Misc;

namespace Scape05.Handlers;

public class BattleEndDelayedTask : IDelayedTask
{
    private readonly IEntity _entity;

    public BattleEndDelayedTask(IEntity entity)
    {

        entity.CombatMethod.CombatTick = 0;
        entity.CombatTarget.CombatMethod.CombatTick = 0;
        
        
       
        entity.CombatTarget.CombatMethod.CanCombat = true;
         
         Console.WriteLine($"+ Tick Registered: {nameof(BattleEndDelayedTask)} with a delay of {Delay} ticks.");
         
         switch (entity)
         {
             case Player player:
                 DelayedTask = () =>
                 {
                     ConsoleColorHelper.Broadcast(1, $"+ {nameof(BattleEndDelayedTask)} invoked.");
                     PacketBuilder.Respawn(player);
                     /* No longer in combat */
                     /* Reset animation */
                     
                     entity.CombatMethod.CanCombat = true;
                     entity.CombatTarget.CombatTarget = null;
                     entity.CombatTarget = null;
                 };
                 break;
             case NPC npc:
                 Delay = 3;
                 DelayedTask = () =>
                 {
                     ConsoleColorHelper.Broadcast(1, $"+ {nameof(BattleEndDelayedTask)} invoked.");
                     PacketBuilder.SendGroundItemPacket(npc.Location, 531, 1, (Player)npc.CombatTarget);
                     npc.DelayedTaskHandler.RegisterDelayedTask(new SpawnNPCDelayedTask(8, npc));
                       
                     entity.CombatMethod.CanCombat = true;
                     entity.CombatTarget.CombatTarget = null;
                     entity.CombatTarget = null;

                     npc.Flags |= NPCUpdateFlags.InteractingEntity;
                     npc.InteractingEntityId = 0x000FF;
                     npc.IsUpdateRequired = true;


                 };
                 break;
             default:
                 ConsoleColorHelper.Broadcast(0, "what the heck");
                 break;
         }
    }

    public int Delay { get; set; } = 1;
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
            npc.Health = npc.MaxHealth;
            ConsoleColorHelper.Broadcast(2, $"[{npc.Index}] {npc.Name} has respawned.");
        };
    }

    public Action DelayedTask { get; set; }
}