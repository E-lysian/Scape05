using Scape05.Misc;

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

        if (!_attacker.CombatBase.InCombat)
        {
            player.CombatBase.Attacker = player;
            player.CombatBase.Target = npc;
            player.CombatBase.NeedsToInitiate = true;
            // if (CanMeleeAttack())
            // {
            // }
            // else
            // {
            //     PacketBuilder.SendMessage("Can't attack from here.", _attacker);
            // }
        }
        else
        {
            PacketBuilder.SendMessage("You're already in combat.", _attacker);
        }

        // player.CombatManager.ShouldInitiate = true;
        // player.CombatManager.IsInitiator = true;
        // player.CombatManager.Target = npc;
    }

    private bool CanMeleeAttack()
    {
        var delta = Location.Delta(_attacker.Location, _target.Location);
        return delta.X == 1 || delta.Y == 1;
    }
}