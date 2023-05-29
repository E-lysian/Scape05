using Scape05.Misc;

namespace Scape05.Entities.Packets;

public class SpawnGroundItemPacketCommand : IPacketCommand
{
    private readonly int itemId;
    private readonly int amount;
    private readonly Location _spawnLocation;

    private SpawnGroundItemPacketCommand(int itemId, int amount, Location spawnLocation)
    {
        this.itemId = itemId;
        this.amount = amount;
        _spawnLocation = spawnLocation;
    }

    public void Execute(Player player)
    {
        player.Writer.CreateFrame(ServerOpCodes.PLAYER_LOCATION);
        player.Writer.WriteByteC(((_spawnLocation.Y >> 3) << 3) - player.BuildArea.OffsetChunkY * 8);
        player.Writer.WriteByteC(((_spawnLocation.X >> 3) << 3) - player.BuildArea.OffsetChunkX * 8);
        
        player.Writer.CreateFrame(ServerOpCodes.FLOORITEM_ADD);
        player.Writer.WriteWordBigEndianA(1042);
        player.Writer.WriteWord(1);
        player.Writer.WriteByte(((_spawnLocation.X & 0x7) << 4) | (_spawnLocation.Y & 0x7));
    }

    public static SpawnGroundItemPacketCommand Create(int itemId, int amount, Location spawnLocation)
    {
        return new SpawnGroundItemPacketCommand(itemId, amount, spawnLocation);
    }
}