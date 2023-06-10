using ICSharpCode.SharpZipLib.Core;
using Scape05.Data.Npc;
using Scape05.Engine.Combat;
using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Handlers;
using Scape05.Misc;
using Scape05.World;
using Scape05.World.Clipping;

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
            case "arrow":
                SpawnProjectile();
                break;
            case "tele":
                Teleport();
                break;
            case "npcwalk":
                NPCWalk();
                break;
            case "npcfollow":
                NPCFollow();
                break;
            case "focus":
                FocusPlayer();
                break;
            case "spawn":
                SpawnItem();
                break;
            case "pos":
                PacketBuilder.SendMessage($"X: {player.Location.X} - Y: {player.Location.Y}", player);
                PacketBuilder.SendMessage(
                    $"BuildAreaStartX: {player.BuildArea.BuildAreaStartX} - BuildAreaStartY: {player.BuildArea.BuildAreaStartY}",
                    player);
                PacketBuilder.SendMessage(
                    $"BuildAreaOffsetChunkX: {player.BuildArea.OffsetChunkX} - BuildAreaOffsetChunkY: {player.BuildArea.OffsetChunkY}",
                    player);
                PacketBuilder.SendMessage(
                    $"LocalX: {player.BuildArea.GetPositionRelativeToOffsetChunkX()} - LocalY: {player.BuildArea.GetPositionRelativeToOffsetChunkY()}",
                    player);

                PacketBuilder.SendMessage($"Blocked: {Region.Blocked(_player.Location.X, _player.Location.Y, 0)}",
                    player);

                break;
            default:
                PacketBuilder.SendMessage($"Unknown command: '{_commandArgs[0]}'", player);
                break;
        }
    }

    private void SpawnItem()
    {
        var px = int.TryParse(_commandArgs[1], out int x);
        var py = int.TryParse(_commandArgs[2], out int y);
        var pz = int.TryParse(_commandArgs[3], out int z);

        if (px && py && pz)
        {
            var spawnLocation = new Location(x, y);
            spawnLocation.Height = z;
            PacketBuilder.SendGroundItemPacket(spawnLocation, 1042, 1, _player);
        }
        else
        {
            PacketBuilder.SendMessage(
                $"Invalid command syntax! ::spawn [{typeof(UInt16)}] [{typeof(UInt16)}] [{typeof(UInt16)}]", _player);
        }
    }

    private void Teleport()
    {
        var px = int.TryParse(_commandArgs[1], out int x);
        var py = int.TryParse(_commandArgs[2], out int y);
        var pz = int.TryParse(_commandArgs[3], out int z);

        if (px && py && pz)
        {
            _player.Location = new Location(x, y);
            _player.Location.Height = z;
            _player.NeedsPlacement = true;
            _player.IsUpdateRequired = true;
            PacketBuilder.SendMapRegion(_player);
        }
        else
        {
            PacketBuilder.SendMessage(
                $"Invalid command syntax! ::tele [{typeof(UInt16)}] [{typeof(UInt16)}] [{typeof(UInt16)}]", _player);
        }
    }

    private void FocusPlayer()
    {
        if (int.TryParse(_commandArgs[1], out int npcIndex))
        {
            var npc = Server.NPCs[npcIndex];
            npc.Flags |= NPCUpdateFlags.InteractingEntity;
            npc.InteractingEntityId = _player.Index + 32768;
            npc.IsUpdateRequired = true;
        }
        else
        {
            PacketBuilder.SendMessage($"Invalid command syntax! ::npcwalk [{typeof(UInt16)}]", _player);
        }
    }

    private void NPCWalk()
    {
        if (int.TryParse(_commandArgs[1], out int npcIndex))
        {
            var npc = Server.NPCs[npcIndex];
            npc.MovementHandler.Reset();
            npc.MovementHandler.AddToPath(new Location(npc.Location.X, npc.Location.Y - 1));
            npc.MovementHandler.Finish();
            // npc.IsUpdateRequired = true;
        }
        else
        {
            PacketBuilder.SendMessage($"Invalid command syntax! ::npcwalk [{typeof(UInt16)}]", _player);
        }
    }

    private void NPCFollow()
    {
        if (int.TryParse(_commandArgs[1], out int npcIndex))
        {
            var npc = Server.NPCs[npcIndex];
            npc.Follow = _player;
            npc.Flags |= NPCUpdateFlags.InteractingEntity;
            npc.InteractingEntityId = npc.Follow.Index + 32768;
            npc.IsUpdateRequired = true;

            // npc.MovementHandler.Reset();
            // npc.MovementHandler.AddToPath(new Location(npc.Location.X, npc.Location.Y - 1));
            // npc.MovementHandler.Finish();
            // npc.IsUpdateRequired = true;
        }
        else
        {
            PacketBuilder.SendMessage($"Invalid command syntax! ::npcwalk [{typeof(UInt16)}]", _player);
        }
    }

    private void SpawnProjectile()
    {
        short npcIndex = 2013;
        if (npcIndex <= 0)
        {
            npcIndex = 1;
        }

        var npc = Server.NPCs[npcIndex - 1];
        var isProjectilePathClear =
            PathFinder.isProjectilePathClear(_player.Location.X, _player.Location.Y, 0, npc.Location.X, npc.Location.Y);
        if (!isProjectilePathClear)
        {
            PacketBuilder.SendMessage("Can't range from here.", _player);
            return;
        }

        var pX = _player.Location.X;
        var pY = _player.Location.Y;

        var nX = npc.Location.X;
        var nY = npc.Location.Y;

        short projectileGraphicsId = 18;
        byte yOffset = (byte)((pY - nY) * -1);
        byte xOffset = (byte)((pX - nX) * -1);

        PacketBuilder.SpawnProjectilePacket(_player, 50, xOffset, yOffset, npcIndex, projectileGraphicsId, 43,
            1, 0, 15, 35, 64);


        npc.DelayedTaskHandler.RegisterDelayedTask(new DelayedHitSplatTask(npc, new DamageInfo
        {
            Amount = 1,
            Type = DamageType.Damage,
            DamageSource = _player
        }));
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
                Size = npcDef.Size == 0 ? 1 : npcDef.Size,
                Name = npcDef.Name,
                CombatLevel = npcDef.CombatLevel,
                Face = new Face(_player.Location.X - 1, _player.Location.Y)
            };
            npc.BuildArea = new BuildArea(npc);
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