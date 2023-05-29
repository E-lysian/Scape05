using Scape05.Misc;

namespace Scape05.Entities.Packets;

public class SpawnGroundItemPacketCommand : IPacketCommand
{
    private readonly int itemId;
    private readonly int amount;

    private SpawnGroundItemPacketCommand(int itemId, int amount, Location spawnLocation)
    {
        this.itemId = itemId;
        this.amount = amount;
    }

    public void Execute(Player player)
    {
        player.Writer.CreateFrame(ServerOpCodes.PLAYER_LOCATION);
        player.Writer.WriteByteC(player.Location.Y - 8 * player.BuildArea.OffsetChunkY);
        player.Writer.WriteByteC(player.Location.X - 8 * player.BuildArea.OffsetChunkX);
        
        player.Writer.CreateFrame(ServerOpCodes.FLOORITEM_ADD);
        player.Writer.WriteWordBigEndianA(1042);
        player.Writer.WriteWord(1);
        player.Writer.WriteByte(0);
    }

    public static SpawnGroundItemPacketCommand Create(int itemId, int amount, Location spawnLocation)
    {
        return new SpawnGroundItemPacketCommand(itemId, amount, spawnLocation);
    }
}