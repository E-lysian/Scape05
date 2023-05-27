using Scape05.Entities.Packets;
using Scape05.World;

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

    public static bool IsInDiagonalBlock(Location attacker, Location attacked) {
        return attacked.X - 1 == attacker.X && attacked.Y + 1 == attacker.Y
               || attacker.X - 1 == attacked.X && attacker.Y + 1 == attacked.Y
               || attacked.X + 1 == attacker.X && attacked.Y - 1 == attacker.Y
               || attacker.X + 1 == attacked.X && attacker.Y - 1 == attacked.Y
               || attacked.X + 1 == attacker.X && attacked.Y + 1 == attacker.Y
               || attacker.X + 1 == attacked.X && attacker.Y + 1 == attacked.Y;
    }
    
    private void ProcessFollow()
    {
        
        if (_npc.Follow != null)
        {

            if (!IsInDiagonalBlock(_npc.Location, _npc.Follow.Location) && Math.Abs(_npc.Location.X - _npc.Follow.Location.X) <= 1 && Math.Abs(_npc.Location.Y - _npc.Follow.Location.Y) <= 1)
            {
                return;
            }
            
            //Reset();
            /* Follow Logic */
            /* If standing on the same tile, step away */
            if (Location.IsSame(_npc.Location, _npc.Follow.Location))
            {
                var tiles = new List<Location>();
                foreach (Location tile in _npc.Follow.Location.GetOuterTiles(1))
                {
                    /* Check if tile is valid */
                    if (!Region.canMove(_npc.Location.X, _npc.Location.Y, tile.X, tile.Y, 0, 1, 1))
                        continue;

                    tiles.Add(tile);
                }

                if (tiles.Count > 0)
                {
                    Random random = new Random();
                    Location randomTile = tiles[random.Next(tiles.Count)];
                    AddToPath(randomTile);
                    Finish();
                }

                return;
            }

            int deltaX = _npc.Follow.Location.X - _npc.Location.X;
            int deltaY = _npc.Follow.Location.Y - _npc.Location.Y;

            if (deltaX < -1)
            {
                deltaX = -1;
            }
            else if (deltaX > 1)
            {
                deltaX = 1;
            }

            if (deltaY < -1)
            {
                deltaY = -1;
            }
            else if (deltaY > 1)
            {
                deltaY = 1;
            }

            /* Diagonal */
            int directionId = MovementHelper.DirectionFromDelta(deltaX, deltaY);

            var startX = _npc.Location.X;
            var startY = _npc.Location.Y;

            var none = new Location(startX, startY);
            var west = new Location(startX - 1, startY);
            var east = new Location(startX + 1, startY);
            var south = new Location(startX, startY - 1);
            var north = new Location(startX, startY + 1);

            var direction = none;

            switch (directionId)
            {
                case 0: /* North West */
                    if (Region.canMove(startX, startY, west.X, west.Y, 0, 1, 1))
                    {
                        direction = west;
                    }
                    else if (Region.canMove(startX, startY, north.X, north.Y, 0, 1, 1))
                    {
                        direction = north;
                    }
                    else
                    {
                        direction = none;
                    }

                    break;
                case 2: /* North East*/
                    if (Region.canMove(startX, startY, north.X, north.Y, 0, 1, 1))
                    {
                        direction = north;
                    }
                    else if (Region.canMove(startX, startY, east.X, east.Y, 0, 1, 1))
                    {
                        direction = east;
                    }
                    else
                    {
                        direction = none;
                    }

                    break;
                case 5: /* South West */
                    if (Region.canMove(startX, startY, west.X, west.Y, 0, 1, 1))
                    {
                        direction = west;
                    }
                    else if (Region.canMove(startX, startY, south.X, south.Y, 0, 1, 1))
                    {
                        direction = south;
                    }
                    else
                    {
                        direction = none;
                    }

                    break;
                case 7: /* South East*/
                    if (Region.canMove(startX, startY, east.X, east.Y, 0, 1, 1))
                    {
                        direction = east;
                    }
                    else if (Region.canMove(startX, startY, south.X, south.Y, 0, 1, 1))
                    {
                        direction = south;
                    }
                    else
                    {
                        direction = none;
                    }

                    break;
                default:
                    break;
            }

            if (direction == _npc.Location)
            {
                return;
            }

            if (directionId == 6) /* South */
            {
                direction.Y -= 1;
            }
            else if (directionId == 4) /* East */
            {
                direction.X += 1;
            }
            else if (directionId == 3) /* West */
            {
                direction.X -= 1;
            }
            else if (directionId == 1) /* North */
            {
                direction.Y += 1;
            }

            var next = new Location(direction.X, direction.Y);
            Console.WriteLine($"NextX: {next.X} - NextY: {next.Y}");
            if (Region.canMove(startX, startY, next.X, next.Y, 0, 1, 1))
            {
                AddToPath(next);
                Finish();
            }
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