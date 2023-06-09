﻿using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.World;
using Scape05.World.Clipping;

namespace Scape05.Networking.Packets.Incoming;

public class WalkToObjectPacket : IPacket
{
    private Player _player;
    public int OpCode { get; set; } = 98;

    public void Build(Player player)
    {
        _player = player;

        /* Set Interact Object somehow */
        /* Check if destX/Y contains an object or an NPC */
        /* If NPC, get the NPC wat that pos Server.NPCS.Where(x => x.Position == new Position(destX, destY))*/

        var length = _player.PacketHandler.PacketLength;

        var _destX = -1;
        var _destY = -1;

        var steps = (length - 5) / 2;
        var path = new int[steps, 2];

        var firstStepX = _player.Reader.ReadSignedWordBigEndianA();
        for (var i = 0; i < steps; i++)
        {
            path[i, 0] = (sbyte)_player.Reader.ReadUnsignedByte();
            path[i, 1] = (sbyte)_player.Reader.ReadUnsignedByte();
        }

        var firstStepY = _player.Reader.ReadSignedWordBigEndian();
        var running = _player.Reader.ReadSignedByteC() == 1;
        
        //player.MovementHandler.Reset();
        player.MovementHandler.SetRunToggled(running);
        // var npc = Server.NPCs[_player.InteractingEntityId];
        // PathFinder.getPathFinder().FindRoute(_player, npc.Location.X, npc.Location.Y, true, 1, 1);
        
        // if (_player.InteractingEntityId != -1)
        // {
        //     /* Get the entity size */
        //     /* Get the outer tiles */
        //     /* Check which ones are valid */
        //     /* Pathfind to each tile and select the one path that has the least amount of waypoints */
        //     var npc = Server.NPCs[_player.InteractingEntityId];
        //     var outerTiles = npc.Location.GetOuterTiles(npc.Size);
        //     var dictionary = new List<List<Location>>();
        //     foreach (var outerTile in outerTiles)
        //     {
        //         if (Region.canMove(_player.Location.X, _player.Location.Y, outerTile.X, outerTile.Y, 0, 1, 1))
        //         {
        //             var foundPath = PathFinder.getPathFinder().FindRoute(_player, outerTile.X, outerTile.Y, true, 1, 1);
        //             dictionary.Add(foundPath);
        //         }
        //     }
        // }


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

        //PacketBuilder.SendMessage($"DestX: {_destX} DestY: {_destY}", player);
        return;
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

        Console.WriteLine($"Built {nameof(WalkToObjectPacket)}");
    }
}