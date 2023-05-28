using Scape05.Misc;
using Scape05.World.Clipping;

namespace Scape05.Entities.Packets.Implementation;

public class RegularWalkPacket : IPacket
{
    public int OpCode { get; set; } = 164;

    public void Build(Player player)
    {
        
        player.CombatBase.Target = null;
        player.Flags |= PlayerUpdateFlags.InteractingEntity;
        player.InteractingEntityId = 0x00FFFF;
        player.IsUpdateRequired = true;
        
        var _destX = -1;
        var _destY = -1;
        var length = player.PacketHandler.PacketLength;

        var steps = (length - 5) / 2;
        var path = new int[steps, 2];

        var firstStepX = player.Reader.ReadSignedWordBigEndianA();
        for (var i = 0; i < steps; i++)
        {
            path[i, 0] = (sbyte)player.Reader.ReadUnsignedByte();
            path[i, 1] = (sbyte)player.Reader.ReadUnsignedByte();
        }

        var firstStepY = player.Reader.ReadSignedWordBigEndian();
        var running = player.Reader.ReadSignedByteC() == 1;

        player.MovementHandler.Reset();
        player.MovementHandler.SetRunToggled(running);

        Console.WriteLine($"X: {firstStepX} - Y: {firstStepY} - Running: {running}");
        var x = firstStepX;
        var y = firstStepY;
        for (var i = 0; i < steps; i++)
        {
            path[i, 0] += firstStepX;
            path[i, 1] += firstStepY;
            _destX = path[i, 0];
            _destY = path[i, 1];
            /* Add x, y to path */
        }

        /* Used in order to interrupt any ongoing tasks */

        //client.IsUpdateRequired = true;

        var tiles = new List<Location>();
        if (path.Length > 0)
            tiles = PathFinder.getPathFinder().FindRoute(player, _destX, _destY, true, 1, 1);
        else
            tiles = PathFinder.getPathFinder().FindRoute(player, x, y, true, 1, 1);

        if (tiles != null)
        {
            for (var i = 0; i < tiles.Count; i++) player.MovementHandler.AddToPath(tiles[i]);

            /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
            player.MovementHandler.Finish();
        }

        Console.WriteLine($"Built {nameof(RegularWalkPacket)}");
    }
}