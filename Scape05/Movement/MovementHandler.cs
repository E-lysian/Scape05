using Scape05.Entities.Packets;

namespace Scape05.Entities;

public class MovementHandler
{
    public int PrimaryDirection { get; set; } = -1;
    public int SecondaryDirection { get; set; } = -1;
    public bool DiscardMovementQueue { get; set; }
    
    private readonly Player _client;
    private bool _runToggled;
    private LinkedList<Point> waypoints = new LinkedList<Point>();

    public MovementHandler(Player client)
    {
        _client = client;
    }

    public void Process()
    {
        Point walkPoint = null;
        Point runPoint = null;
        if (waypoints.Count <= 0)
            return;

        walkPoint = waypoints.First.Value;
        waypoints.RemoveFirst();

        if (waypoints.Count > 1 && IsRunPath())
        {
            runPoint = waypoints.First.Value;
            waypoints.RemoveFirst();
        }

        if (walkPoint != null && walkPoint.GetDirection() != -1)
        {
            _client.Location.Move(MovementHelper.DIRECTION_DELTA_X[walkPoint.GetDirection()],
                MovementHelper.DIRECTION_DELTA_Y[walkPoint.GetDirection()]);

            /* Validate walkPoint and make sure it's not clipping, aka a door for instance */
            PrimaryDirection = walkPoint.GetDirection();
        }

        if (runPoint != null && runPoint.GetDirection() != -1)
        {
            _client.Location.Move(MovementHelper.DIRECTION_DELTA_X[runPoint.GetDirection()],
                MovementHelper.DIRECTION_DELTA_Y[runPoint.GetDirection()]);
            SecondaryDirection = runPoint.GetDirection();
        }

        var deltaX = _client.BuildArea.GetPositionRelativeToOffsetChunkX();
        var deltaY = _client.BuildArea.GetPositionRelativeToOffsetChunkY();

        if (deltaX < 16 || deltaX >= 88 || deltaY < 16 || deltaY > 88)
        {
            PacketBuilder.SendMapRegion(_client);
            Console.WriteLine("Sent Region Packet!");
        }
    }

    private bool IsRunPath()
    {
        return _runToggled;
    }

    public void AddToPath(Location location)
    {
        if (waypoints.Count == 0)
        {
            Reset();
        }

        var last = waypoints.Last.Value;

        var x = location.X;
        var lastX = last.X;

        var y = location.Y;
        var lastY = last.Y;

        var deltaX = x - lastX;
        var deltaY = y - lastY;

        var max = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY)); //3

        for (var i = 0; i < max; i++)
        {
            if (max > 40)
            {
                Console.WriteLine("ehm, stop that :^)");
            }

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

        Console.WriteLine($"Direction: {direction}");
    }

    public void Reset()
    {
        waypoints.Clear();
        var p = _client.Location;
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

public class Point : Tile
{
    private int _direction;

    public Point(int x, int y, int direction) : base(x, y)
    {
        SetDirection(direction);
    }

    public int GetDirection()
    {
        return _direction;
    }

    public void SetDirection(int direction)
    {
        _direction = direction;
    }
}

public class Tile
{
    public int X { get; }
    public int Y { get; }

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
    }
}
