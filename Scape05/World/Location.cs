namespace Scape05.Entities;

public class Location
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Height { get; set; }

    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    internal void Move(int amountX, int amountY)
    {
        X += amountX;
        Y += amountY;
    }

    public bool IsWithinArea(Location playerLocation)
    {
        var delta = Delta(this, playerLocation);
        return delta.X <= 14 && delta.X >= -15 && delta.Y <= 14 && delta.Y >= -15;
    }

    public static Location Delta(Location a, Location b)
    {
        return new Location(b.X - a.X, b.Y - a.Y);
    }
}