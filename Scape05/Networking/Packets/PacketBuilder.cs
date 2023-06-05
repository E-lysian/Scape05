using Scape05.Networking.Packets.Outgoing;
using Scape05.Networking.Packets.ServerPackets;

namespace Scape05.Entities.Packets;

public static class PacketBuilder
{
    public static void SendGroundItemPacket(Location location, int itemId, int amount, Player player)
    {
        SpawnGroundItemPacketCommand packetCommand = SpawnGroundItemPacketCommand.Create(itemId, amount, location);
        packetCommand.Execute(player);
    }

    public static void SendMapRegion(Player player)
    {
        MapRegionChangePacketCommand packetCommand = MapRegionChangePacketCommand.Create();
        packetCommand.Execute(player);
    }

    public static void SendMessage(string message, Player player)
    {
        SendMessagePacketCommand packetCommand = SendMessagePacketCommand.Create(message);
        packetCommand.Execute(player);
    }

    public static void SendPlayerStatus(Player player)
    {
        SendPlayerStatusCommand packetCommand = SendPlayerStatusCommand.Create();
        packetCommand.Execute(player);
    }

    public static void SendSidebarInterface(Player player, int menuId, int form)
    {
        SendSidebarInterfacePacketCommand packetCommand = SendSidebarInterfacePacketCommand.Create(menuId, form);
        packetCommand.Execute(player);
    }

    public static void SendSkills(Player player)
    {
        SendSkillsPacketCommand packetCommand = SendSkillsPacketCommand.Create();
        packetCommand.Execute(player);
    }

    public static void SendEquipment(Player player)
    {
        SendEquipmentPacketCommand packetCommand = SendEquipmentPacketCommand.Create();
        packetCommand.Execute(player);
    }
    
    public static void Respawn(Player player)
    {
        RespawnPacketCommand packetCommand = RespawnPacketCommand.Create();
        packetCommand.Execute(player);
    }
    
    public static void ResetAnimation(Player player)
    {
        AnimationResetPacketCommand packetCommand = AnimationResetPacketCommand.Create();
        packetCommand.Execute(player);
    }
}

