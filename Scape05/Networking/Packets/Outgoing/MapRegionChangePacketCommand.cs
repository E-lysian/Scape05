using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.ServerPackets;

public class MapRegionChangePacketCommand : IPacketCommand
{
    public void Execute(Player player)
    {
        player.BuildArea = new BuildArea(player);

        player.Writer.CreateFrame(ServerOpCodes.REGION_LOAD);
        player.Writer.WriteWordA(player.BuildArea.GetCenterChunkX()); //Center point of the build area
        player.Writer.WriteWord(player.BuildArea.GetCenterChunkY()); //Center point of the build area

    }
    
    public static MapRegionChangePacketCommand Create()
    {
        return new MapRegionChangePacketCommand();
    }
}