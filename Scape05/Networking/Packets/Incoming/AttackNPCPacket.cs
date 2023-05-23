namespace Scape05.Entities.Packets.Implementation;

public class AttackNPCPacket : IPacket
{
    public int OpCode { get; set; } = 72;

    public void Build(Player player)
    {
        var index = player.Reader.ReadSignedWordA();
        Console.WriteLine($"Clicked NPC with index: {index}");

        var npc = Server.NPCs[index];
        Console.WriteLine($"NPC index: {npc.Index}");
        Console.WriteLine($"NPC Name: {npc.Name}");
        Console.WriteLine($"NPC CombatLevel: {npc.CombatLevel}");
        Console.WriteLine($"AttackNPCX: {npc.Location.X} - AttackNPCY: {npc.Location.Y}");
        Console.WriteLine($"Built {nameof(AttackNPCPacket)}");

        player.CombatManager.ShouldInitiate = true;
        player.CombatManager.IsInitiator = true;
        player.CombatManager.Target = npc;
    }
}