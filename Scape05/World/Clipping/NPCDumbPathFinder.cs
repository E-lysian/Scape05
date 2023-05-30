using Scape05.Entities;

namespace Scape05.World.Clipping;

public class NPCDumbPathFinder
{
    
    public static sbyte[] directionDeltaX = { 0, 1, 1, 1, 0, -1, -1, -1 };
    public static sbyte[] directionDeltaY = { 1, 1, 0, -1, -1, -1, 0, 1 };
    private static int NORTH = 0, EAST = 1, SOUTH = 2, WEST = 3;
    public static readonly int[][] DIR = { new int[] { -1, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 },
        new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { -1, -1 },
        new int[] { 0, -1 }, new int[] { 1, -1 } };


    public static Location Follow(NPC npc, IEntity following)
    {
        var npcTiles = TileControl.GetTiles(npc);

        var npcLocation = TileControl.CurrentLocation(npc);
        var followingLocation = TileControl.CurrentLocation(following);

        bool[] moves = new bool[4];

        int dir = -1;

        int distance = TileControl.CalculateDistance(npc, following);
        if (distance > 16)
        {
            return null;
        }

        if (distance > 1)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                moves[i] = true;
            }

            /** remove false moves **/
            if (npcLocation.X < followingLocation.X)
            {
                moves[EAST] = true;
                moves[WEST] = false;
            }
            else if (npcLocation.X > followingLocation.X)
            {
                moves[WEST] = true;
                moves[EAST] = false;
            }
            else
            {
                moves[EAST] = false;
                moves[WEST] = false;
            }

            if (npcLocation.Y > followingLocation.Y)
            {
                moves[SOUTH] = true;
                moves[NORTH] = false;
            }
            else if (npcLocation.Y < followingLocation.Y)
            {
                moves[NORTH] = true;
                moves[SOUTH] = false;
            }
            else
            {
                moves[NORTH] = false;
                moves[SOUTH] = false;
            }

            foreach (Location tiles in npcTiles)
            {
                if (tiles.X == following.Location.X) //same x line
                {
                    moves[EAST] = false;
                    moves[WEST] = false;
                }
                else if (tiles.Y == following.Location.Y) //same y line
                {
                    moves[NORTH] = false;
                    moves[SOUTH] = false;
                }
            }

            bool[] blocked = new bool[3];

            if (moves[NORTH] && moves[EAST])
            {
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedNorth(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[0] = true;
                    }

                    if (Region.BlockedEast(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[1] = true;
                    }

                    if (Region.BlockedNorthEast(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[2] = true;
                    }
                }

                if (!blocked[2] && !blocked[0] && !blocked[1]) //northeast
                {
                    dir = 2;
                }
                else if (!blocked[0]) //north
                {
                    dir = 0;
                }
                else if (!blocked[1]) //east
                {
                    dir = 4;
                }
            }
            else if (moves[NORTH] && moves[WEST])
            {
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedNorth(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[0] = true;
                    }

                    if (Region.BlockedWest(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[1] = true;
                    }

                    if (Region.BlockedNorthWest(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[2] = true;
                    }
                }

                if (!blocked[2] && !blocked[0] && !blocked[1]) //north-west
                {
                    dir = 14;
                }
                else if (!blocked[0]) //north
                {
                    dir = 0;
                }
                else if (!blocked[1]) //west
                {
                    dir = 12;
                }
            }
            else if (moves[SOUTH] && moves[EAST])
            {
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedSouth(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[0] = true;
                    }

                    if (Region.BlockedEast(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[1] = true;
                    }

                    if (Region.BlockedSouthEast(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[2] = true;
                    }
                }

                if (!blocked[2] && !blocked[0] && !blocked[1]) //south-east
                {
                    dir = 6;
                }
                else if (!blocked[0]) //south
                {
                    dir = 8;
                }
                else if (!blocked[1]) //east
                {
                    dir = 4;
                }
            }
            else if (moves[SOUTH] && moves[WEST])
            {
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedSouth(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[0] = true;
                    }

                    if (Region.BlockedWest(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[1] = true;
                    }

                    if (Region.BlockedSouthWest(tiles.X, tiles.Y, tiles.Height))
                    {
                        blocked[2] = true;
                    }
                }

                if (!blocked[2] && !blocked[0] && !blocked[1]) //south-west
                {
                    dir = 10;
                }
                else if (!blocked[0]) //south
                {
                    dir = 8;
                }
                else if (!blocked[1]) //west
                {
                    dir = 12;
                }
            }
            else if (moves[NORTH])
            {
                dir = 0;
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedNorth(tiles.X, tiles.Y, tiles.Height))
                    {
                        dir = -1;
                    }
                }
            }
            else if (moves[EAST])
            {
                dir = 4;
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedEast(tiles.X, tiles.Y, tiles.Height))
                    {
                        dir = -1;
                    }
                }
            }
            else if (moves[SOUTH])
            {
                dir = 8;
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedSouth(tiles.X, tiles.Y, tiles.Height))
                    {
                        dir = -1;
                    }
                }
            }
            else if (moves[WEST])
            {
                dir = 12;
                foreach (Location tiles in npcTiles)
                {
                    if (Region.BlockedWest(tiles.X, tiles.Y, tiles.Height))
                    {
                        dir = -1;
                    }
                }
            }
            
            if (dir == -1)
            {
                return null;
            }
            
            dir >>= 1;	
			
            if (dir < 0) {
                return null;
            }
            
            
            
            var moveX = directionDeltaX[dir];
            var moveY = directionDeltaY[dir];
            
            var next = new Location(moveX, moveY);
            Console.WriteLine($"NextX: {next.X} - NextY: {next.Y}");
            return next;

            // npc.getNextNPCMovement();
            // npc.updateRequired = true;
        }

        return null;
    }
    
    // public static void WalkTowards(NPC npc, int waypointx, int waypointy) {
    //     int[,] points = {{-1, 0}, {1, 0}, {0, -1}, {0, 1}};
    //     
    //     int x = npc.Location.X;
    //     int y = npc.Location.Y;
		  //
    //     if (waypointx == x && waypointy == y) {
    //         return;
    //     }
    //
    //     foreach (var point in points)
    //     {
    //         point[][0]
    //     }
    //     
    //     int direction = -1;
    //     int xDifference = waypointx - x;
    //     int yDifference = waypointy - y;
    //
    //     int toX = 0;
    //     int toY = 0;
    //
    //     if (xDifference > 0) {
    //         toX = 1;
    //     } else if (xDifference < 0) {
    //         toX = -1;
    //     }
    //
    //     if (yDifference > 0) {
    //         toY = 1;
    //     } else if (yDifference < 0) {
    //         toY = -1;
    //     }
    //
    //     int toDir = NPCClipping.getDirection(x, y, x + toX, y + toY);
    //     
    //     var canMove = Region.canMove(Attacker.Location.X, Attacker.Location.Y, Target.Location.X, Target.Location.Y, 0, 1, 1);
    //     
    //     if (canMoveTo(npc, toDir)) {
    //         direction = toDir;
    //     } else {
    //         if (toDir == 0) {
    //             if (canMoveTo(npc, 3)) {
    //                 direction = 3;
    //             } else if (canMoveTo(npc, 1)) {
    //                 direction = 1;
    //             }
    //         } else if (toDir == 2) {
    //             if (canMoveTo(npc, 1)) {
    //                 direction = 1;
    //             } else if (canMoveTo(npc, 4)) {
    //                 direction = 4;
    //             }
    //         } else if (toDir == 5) {
    //             if (canMoveTo(npc, 3)) {
    //                 direction = 3;
    //             } else if (canMoveTo(npc, 6)) {
    //                 direction = 6;
    //             }
    //         } else if (toDir == 7) {
    //             if (canMoveTo(npc, 4)) {
    //                 direction = 4;
    //             } else if (canMoveTo(npc, 6)) {
    //                 direction = 6;
    //             }
    //         }
    //     }
    //
    //     if (direction == -1) {
    //         return;
    //     }
		  //
    //     if (direction == -1) {
    //         return;
    //     }
    //
    //     var moveX = DIR[direction][0];
    //     var moveY = DIR[direction][1];
    //
    //     npc.getNextNPCMovement();
    //     npc.updateRequired = true;
    // }
    
}

