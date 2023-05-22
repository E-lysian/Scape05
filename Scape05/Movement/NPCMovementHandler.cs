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

    public void Process()
    {
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