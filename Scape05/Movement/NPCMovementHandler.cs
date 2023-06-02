using System.Numerics;
using Scape05.Entities.Packets;
using Scape05.World;
using Scape05.World.Clipping;

namespace Scape05.Entities;

public class NPCMovementHandler
{
    private readonly NPC _npc;
    public int PrimaryDirection { get; set; } = -1;
    public int SecondaryDirection { get; set; } = -1;
    public bool DiscardMovementQueue { get; set; }
    private bool _runToggled;
    private readonly LinkedList<Point> waypoints = new();

    public NPCMovementHandler(NPC npc)
    {
        _npc = npc;
    }

    public bool IsAdjacentTo(Location location, Location otherLocation)
    {
        /* Needs to take size into consideration */
        int deltaX = Math.Abs(location.X - otherLocation.X);
        int deltaY = Math.Abs(location.Y - otherLocation.Y);

        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public void Process()
    {
        /* If it has a target and is not there yet */

        ProcessFollow();

        if (waypoints.Count == 0)
            return;

        var walkPoint = waypoints.First.Value;
        waypoints.RemoveFirst();

        var runPoint = GetRunPoint();

        if (walkPoint != null && walkPoint.Direction != -1)
        {
            MoveToDirection(walkPoint.Direction);
            PrimaryDirection = walkPoint.Direction;
        }

        if (runPoint != null && runPoint.Direction != -1)
        {
            MoveToDirection(runPoint.Direction);
            SecondaryDirection = runPoint.Direction;
        }
    }

    public static bool IsInDiagonalBlock(Location attacker, Location attacked)
    {
        return attacked.X - 1 == attacker.X && attacked.Y + 1 == attacker.Y
               || attacker.X - 1 == attacked.X && attacker.Y + 1 == attacked.Y
               || attacked.X + 1 == attacker.X && attacked.Y - 1 == attacker.Y
               || attacker.X + 1 == attacked.X && attacker.Y - 1 == attacked.Y
               || attacked.X + 1 == attacker.X && attacked.Y + 1 == attacker.Y
               || attacker.X + 1 == attacked.X && attacker.Y + 1 == attacked.Y;
    }

    public int GetMove(int Place1, int Place2)
    {
        if ((Place1 - Place2) == 0)
        {
            return 0;
        }
        else if ((Place1 - Place2) < 0)
        {
            return 1;
        }
        else if ((Place1 - Place2) > 0)
        {
            return -1;
        }

        return 0;
    }

    private void ProcessFollow()
    {
        if (_npc.Follow != null)
        {
            var playerX = _npc.Follow.Location.X;
            var playerY = _npc.Follow.Location.Y;

            var npcX = _npc.Location.X;
            var npcY = _npc.Location.Y;

            var diagonal = ((playerX == npcX - 1 && playerY == npcY + 1) ||
                            (playerX == npcX - 1 && playerY == npcY - 1) ||
                            (playerX == npcX + 1 && playerY == npcY - 1) ||
                            (playerX == npcX + 1 && playerY == npcY + 1));

            /* Check if we're inside the NPC tiles */
            if (Location.IsPlayerInsideNPC(_npc.Follow, _npc))
            {
                Console.WriteLine("Inside the NPC tiles");

                /* W, N, E, S*/
                int[,] directions = new int[,] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

                //In order to be adjacent to the NPC perpendicularly,
                //we need to be either -1 on the X,
                //- 1 on the Y,
                //+ Size on the X
                //+ Size on the Y

                //Check each direction it can move
                //Get all the valid directions
                //Get the one which has the furthest distance away from the player out of those

                var distances = new Dictionary<Location, double>();
                
                for (int i = 0; i < directions.Length / 2; i++)
                {
                    var canMove = Region.canMove(_npc.Location.X, _npc.Location.Y, _npc.Location.X + directions[i, 0],
                        _npc.Location.Y + directions[i, 1], 0, _npc.Size, _npc.Size);

                    if (!canMove)
                        continue;

                    double centerX = _npc.Location.X + directions[i, 0] + _npc.Size / 2 - .5;
                    double centerY = _npc.Location.Y + directions[i, 1] + _npc.Size / 2 - .5;

                    Location newTile = new Location(_npc.Location.X + directions[i, 0],
                        _npc.Location.Y + directions[i, 1]);


                    var distance = CalculateDistance(centerX, centerY,
                        _npc.Follow.Location.X, _npc.Follow.Location.Y);

                    Console.WriteLine($"[{i}] Distance: {distance}");

                    distances.Add(newTile, distance);

                }

                var vals = distances.OrderByDescending(x => x.Value);
                if (vals.Count() > 0)
                {
                    var location = vals.FirstOrDefault().Key;
                    AddToPath(location);
                    Finish();
                }
                // var tiles = new List<Location>();
                // foreach (Location tile in _npc.Follow.Location.GetOuterTiles(1))
                // {
                //     /* Check if tile is valid */
                //     if (!Region.canMove(_npc.Location.X, _npc.Location.Y, tile.X, tile.Y, 0, _npc.Size, _npc.Size))
                //         continue;
                //
                //     tiles.Add(tile);
                // }
                //
                // if (tiles.Count > 0)
                // {
                //     Random random = new Random();
                //     Location randomTile = tiles[random.Next(tiles.Count)];
                //     AddToPath(randomTile);
                //     Finish();
                // }

                return;
            }

            var next = NPCDumbPathFinder.Follow(_npc, _npc.Follow);

            if (next != null)
            {
                AddToPath(new Location(npcX + next.X, npcY + next.Y));
                Finish();
            }
            // if (Region.canMove(npcX, npcY, npcX  + next.X, npcY + next.Y, 0, _npc.Size, _npc.Size))
            // {
            // }
        }
    }

    static double CalculateDistance(double x1, double y1, int x2, int y2)
    {
        double horizontalDistance = Math.Abs(x2 - x1);
        double verticalDistance = Math.Abs(y2 - y1);

        if (horizontalDistance == 0 || verticalDistance == 0)
        {
            // Adjacent tiles
            return 1;
        }
        else
        {
            // Diagonal tiles
            double diagonalDistance = Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));
            return diagonalDistance;
        }
    }

    private void MoveToDirection(int direction)
    {
        _npc.Location.Move(MovementHelper.DIRECTION_DELTA_X[direction],
            MovementHelper.DIRECTION_DELTA_Y[direction]);
    }

    public void AddToPath(Location location)
    {
        if (waypoints.Count == 0)
        {
            Reset();
        }

        var last = waypoints.Last.Value;
        var deltaX = location.X - last.X;
        var deltaY = location.Y - last.Y;
        var max = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

        if (max > 40)
        {
            Console.WriteLine("ehm, stop that :^)");
            return;
        }

        for (var i = 0; i < max; i++)
        {
            if (deltaX < 0)
            {
                deltaX++;
            }
            else if (deltaX > 0)
            {
                deltaX--;
            }

            if (deltaY < 0)
            {
                deltaY++;
            }
            else if (deltaY > 0)
            {
                deltaY--;
            }

            AddStep(location.X - deltaX, location.Y - deltaY);
        }
    }

    private void AddStep(int x, int y)
    {
        if (waypoints.Count >= 100)
        {
            return;
        }

        var last = waypoints.Last.Value;
        var deltaX = x - last.X;
        var deltaY = y - last.Y;
        var direction = MovementHelper.GetDirection(deltaX, deltaY);

        if (direction > -1)
        {
            waypoints.AddLast(new Point(x, y, direction));
        }
    }

    private Point GetRunPoint()
    {
        if (waypoints.Count > 1 && GetRunToggled())
        {
            var runPoint = waypoints.First.Value;
            waypoints.RemoveFirst();
            return runPoint;
        }

        return null;
    }

    public void Reset()
    {
        waypoints.Clear();
        var p = _npc.Location;
        waypoints.AddLast(new Point(p.X, p.Y, -1));
    }

    public void Finish()
    {
        waypoints.RemoveFirst();
    }

    public void SetRunToggled(bool runPath)
    {
        _runToggled = runPath;
    }

    public bool GetRunToggled()
    {
        return _runToggled;
    }
}