using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.ServerPackets;

public class SendPlayerStatusCommand : IPacketCommand
{
    public void Execute(Player player)
    {
        var index = Array.IndexOf(Server.Players, player);
        Console.WriteLine($"Trying to send Player Index: {index}");
        if (index != -1)
        {
            player.Writer.CreateFrame(ServerOpCodes.PLAYER_STATUS);
            player.Writer.WriteByteA(0);
            player.Writer.WriteWordBigEndianA(index);
        }
    }

    public static SendPlayerStatusCommand Create()
    {
        return new SendPlayerStatusCommand();
    }
}