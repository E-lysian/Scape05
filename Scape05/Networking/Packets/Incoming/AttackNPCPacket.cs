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

        if (npc.CombatTarget == null)
        {
            player.CombatTarget = npc;
        }
    }

    private bool CanMeleeAttack()
    {
        var delta = Location.Delta(_attacker.Location, _target.Location);
        return delta.X == 1 || delta.Y == 1;
    }
}