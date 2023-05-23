using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.Outgoing;

public class TakeDamagePacketCommand : IPacketCommand
{
    private TakeDamagePacketCommand()
    {
    }

    public void Execute(Player player)
    {
        player.CombatManager.DamageTaken = 5;
        player.Flags |= PlayerUpdateFlags.SingleHit;
        player.Flags |= PlayerUpdateFlags.Animation;
        player.AnimationId = player.CombatManager.Weapon.Animation.BlockId;
        player.IsUpdateRequired = true;
    }

    public static TakeDamagePacketCommand Create()
    {
        return new TakeDamagePacketCommand();
    }
}