using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.Outgoing;

public class SpawnProjectilePacketCommand : IPacketCommand
{
    private readonly Player _player;
    private readonly byte _projectileAngle;
    private readonly byte _projectileOffsetX;
    private readonly byte _projectileOffsetY;
    private readonly short _projectileTarget;
    private readonly short _projectileGraphicId;
    private readonly byte _projectileHeightStart;
    private readonly byte _projectileHeightEnd;
    private readonly short _projectileCreatedTime;
    private readonly short _projectileSpeed;
    private readonly byte _projectileInitialSlope;
    private readonly byte _projectileDistanceFromSource;

    private SpawnProjectilePacketCommand(Player player, byte projectileAngle, byte projectileOffsetX, byte projectileOffsetY,
        short projectileTarget, short projectileGraphicId, byte projectileHeightStart, byte projectileHeightEnd,
        short projectileCreatedTime, short projectileSpeed, byte projectileInitialSlope,
        byte projectileDistanceFromSource)
    {
        _player = player;
        _projectileAngle = projectileAngle;
        _projectileOffsetX = projectileOffsetX;
        _projectileOffsetY = projectileOffsetY;
        _projectileTarget = projectileTarget;
        _projectileGraphicId = projectileGraphicId;
        _projectileHeightStart = projectileHeightStart;
        _projectileHeightEnd = projectileHeightEnd;
        _projectileCreatedTime = projectileCreatedTime;
        _projectileSpeed = projectileSpeed;
        _projectileInitialSlope = projectileInitialSlope;
        _projectileDistanceFromSource = projectileDistanceFromSource;
    }

    public void Execute(Player player)
    {
        
       
        var npc = Server.NPCs[2017];
        player.Writer.CreateFrame(ServerOpCodes.PLAYER_LOCATION);
        player.Writer.WriteByteC((player.Location.Y - (player.BuildArea.OffsetChunkY * 8)) - 2);
        player.Writer.WriteByteC((player.Location.X - (player.BuildArea.OffsetChunkX * 8)) - 3);
        
        player.Writer.CreateFrame(ServerOpCodes.PROJECTILE);
        player.Writer.WriteByte(_projectileAngle);
        player.Writer.WriteByte(_projectileOffsetX);
        player.Writer.WriteByte(_projectileOffsetY);

        player.Writer.WriteWord(_projectileTarget);
        player.Writer.WriteWord(_projectileGraphicId);

        player.Writer.WriteByte(_projectileHeightStart);
        player.Writer.WriteByte(_projectileHeightEnd);

        player.Writer.WriteWord(_projectileCreatedTime);
        player.Writer.WriteWord(_projectileSpeed);

        player.Writer.WriteByte(_projectileInitialSlope);
        player.Writer.WriteByte(_projectileDistanceFromSource);
    }

    public static SpawnProjectilePacketCommand Create(Player player, byte projectileAngle, byte projectileOffsetX,
        byte projectileOffsetY, short projectileTarget, short projectileGraphicId, byte projectileHeightStart,
        byte projectileHeightEnd, short projectileCreatedTime, short projectileSpeed, byte projectileInitialSlope,
        byte projectileDistanceFromSource)
    {
        return new SpawnProjectilePacketCommand(player, projectileAngle, projectileOffsetX, projectileOffsetY, projectileTarget,
            projectileGraphicId, projectileHeightStart, projectileHeightEnd, projectileCreatedTime, projectileSpeed,
            projectileInitialSlope, projectileDistanceFromSource);
    }
}