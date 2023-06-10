using Scape05.Misc;
using Scape05.World;
using Scape05.World.Clipping;

namespace Scape05.Entities.Packets.Implementation;

public class AttackNPCPacket : IPacket
{
    private Player _attacker;
    private IEntity _target;
    public int OpCode { get; set; } = 72;

    public void Build(Player player)
    {
        _attacker = player;
        var index = player.Reader.ReadSignedWordA();
        Console.WriteLine($"Clicked NPC with index: {index}");

        var npc = Server.NPCs[index];
        _target = npc;

        Console.WriteLine($"NPC index: {npc.Index}");
        Console.WriteLine($"NPC Name: {npc.Name}");
        Console.WriteLine($"NPC CombatLevel: {npc.CombatLevel}");
        Console.WriteLine($"AttackNPCX: {npc.Location.X} - AttackNPCY: {npc.Location.Y}");
        Console.WriteLine($"Built {nameof(AttackNPCPacket)}");

        player.Flags |= PlayerUpdateFlags.InteractingEntity;
        player.InteractingEntityId = _target.Index;
        player.IsUpdateRequired = true;

        if (_attacker.InCombat)
        {
            PacketBuilder.SendMessage("You're already in combat.", _attacker);
            return;
        }

        if (npc.CombatTarget != null && npc.CombatTarget != _attacker)
        {
            PacketBuilder.SendMessage("They are already in combat.", _attacker);
            return;
        }

        if (npc.CombatTarget == null)
        {
            player.CombatTarget = npc;
        }

        player.MovementHandler.Reset();

        var path = PathFinder.getPathFinder()
            .FindRoute(_attacker, npc.Location.X, npc.Location.Y, true, npc.Size, npc.Size);


        if (path != null)
        {
            for (var i = 0; i < path.Count; i++) player.MovementHandler.AddToPath(path[i]);

            /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
            player.MovementHandler.Finish();
        }
    }

    private bool CanMeleeAttack()
    {
        var delta = Location.Delta(_attacker.Location, _target.Location);
        return delta.X == 1 || delta.Y == 1;
    }
}