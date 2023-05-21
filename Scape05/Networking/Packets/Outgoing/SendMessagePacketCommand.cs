using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.ServerPackets;

public class SendMessagePacketCommand : IPacketCommand
{
    private readonly string _message;

    private SendMessagePacketCommand(string message)
    {
        _message = message;
    }

    public void Execute(Player player)
    {
        player.Writer.CreateFrameVarSize(ServerOpCodes.MSG_SEND);
        player.Writer.WriteString(_message);
        player.Writer.EndFrameVarSize();
    }

    public static SendMessagePacketCommand Create(string message)
    {
        return new SendMessagePacketCommand(message);
    }
}