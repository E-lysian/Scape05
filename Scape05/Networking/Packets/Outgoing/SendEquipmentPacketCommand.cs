using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.ServerPackets;

public class SendEquipmentPacketCommand : IPacketCommand
{
    public void Execute(Player player)
    {
        var equipment = player.EquipmentManager.GetEquipment();
        int itemAmount = 1;

        for (var i = 0; i < equipment.Length; i++)
            if (equipment[i] != 0)
            {
                player.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SLOT_SET);
                player.Writer.WriteWord(GameInterface.Equipment);
                player.Writer.WriteByte(i);
                player.Writer.WriteWord(player.EquipmentManager.GetItem(i) + 1);
                if (itemAmount > 254)
                {
                    player.Writer.WriteByte(255);
                    player.Writer.WriteWord(itemAmount);
                }
                else
                {
                    player.Writer.WriteByte(itemAmount);
                }

                player.Writer.EndFrameVarSizeWord();
            }
    }

    public static SendEquipmentPacketCommand Create()
    {
        return new SendEquipmentPacketCommand();
    }
}