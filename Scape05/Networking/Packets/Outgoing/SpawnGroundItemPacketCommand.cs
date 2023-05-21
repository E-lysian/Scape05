namespace Scape05.Entities.Packets;

public class SpawnGroundItemPacketCommand : IPacketCommand
{
    private readonly int itemId;
    private readonly int amount;

    private SpawnGroundItemPacketCommand(int itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }

    public void Execute(Player player)
    {
        player.Writer.WriteByte(itemId);
        player.Writer.WriteByte(amount);
    }

    public static SpawnGroundItemPacketCommand Create(int itemId, int amount)
    {
        return new SpawnGroundItemPacketCommand(itemId, amount);
    }
}