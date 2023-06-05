using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Misc;

namespace Scape05.Networking.Packets.Outgoing;

public class RespawnPacketCommand : IPacketCommand
{
    private RespawnPacketCommand()
    {
    }

    public void Execute(Player player)
    {
        player.Location = new Location(3222,3218);
        player.BuildArea = new BuildArea(player);
        player.IsUpdateRequired = true;
        player.NeedsPlacement = true;
        player.AnimationId = 0x00FFFF;
        player.Flags |= PlayerUpdateFlags.Animation;
        // player.Health = player.MaxHealth;
        PacketBuilder.SendMapRegion(player);
    }

    public static RespawnPacketCommand Create()
    {
        return new RespawnPacketCommand();
    }
}