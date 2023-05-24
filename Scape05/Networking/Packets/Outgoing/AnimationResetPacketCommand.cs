using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.Outgoing;

public class AnimationResetPacketCommand : IPacketCommand
{
    private AnimationResetPacketCommand()
    {
    }

    public void Execute(Player player)
    {
        player.Writer.CreateFrame(ServerOpCodes.ANIM_ALL_RESET);
    }

    public static AnimationResetPacketCommand Create()
    {
        return new AnimationResetPacketCommand();
    }
}