using Scape05.Misc;

namespace Scape05.Entities.Packets;

public class SendSidebarInterfacePacketCommand : IPacketCommand
{
    private readonly int _menuId;
    private readonly int _form;

    private SendSidebarInterfacePacketCommand(int menuId, int form)
    {
        _menuId = menuId;
        _form = form;
    }

    public void Execute(Player player)
    {
        player.Writer.CreateFrame(ServerOpCodes.SIDEBAR_INTF_ASSIGN);
        player.Writer.WriteWord(_form); /* Tab */
        player.Writer.WriteByteA(_menuId); /* Icon */
    }

    public static SendSidebarInterfacePacketCommand Create(int itemId, int amount)
    {
        return new SendSidebarInterfacePacketCommand(itemId, amount);
    }
}