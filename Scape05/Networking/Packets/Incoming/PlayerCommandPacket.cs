using Scape05.Data.Npc;
using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Misc;

namespace Scape05.Networking.Packets.Incoming;

public class PlayerCommandPacket : IPacket
{
    private string[] _commandArgs;
    private Player _player;
    public int OpCode { get; set; } = 103;

    public void Build(Player player)
    {
        _player = player;
        _commandArgs = player.Reader.ReadString().Split(' ');

        switch (_commandArgs[0].ToLower())
        {
            case "npc":
                NPCCommand();
                break;
            case "damage":
                PacketBuilder.TakeDamage(player);
                break;
            default:
                PacketBuilder.SendMessage($"Unknown command: '{_commandArgs[0]}'", player);
                break;
        }
    }

    private void NPCCommand()
    {
        if (_commandArgs.Length > 2 || !int.TryParse(_commandArgs[1], out int npcModelId))
        {
            PacketBuilder.SendMessage($"Invalid command syntax! ::npc [{typeof(UInt16)}]", _player);
            return;
        }
        
        if (NpcDefinitionDecoder.NpcDefinitions.TryGetValue(npcModelId, out var npcDef))
        {
            var npc = new NPC
            {
                ModelId = npcModelId,
                Location = new Location(_player.Location.X, _player.Location.Y),
                Size = npcDef.Size,
                Name = npcDef.Name,
                CombatLevel = npcDef.CombatLevel,
                Face =  new Face(_player.Location.X - 1, _player.Location.Y)
            };
            npc.Flags |= NPCUpdateFlags.Face;
            npc.IsUpdateRequired = true;
            Server.AddNPC(npc);
        }
        else
        {
            PacketBuilder.SendMessage($"Could not find NPC with ModelID: {npcModelId}", _player);
        }
    }
}