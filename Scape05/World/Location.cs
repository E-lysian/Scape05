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

    public static bool IsSame(Location l1, Location l2)
    {
        return l1.X == l2.X && l1.Y == l2.Y;
    }

    public static List<Location> InnerTiles(IEntity entity)
    {
        var tiles = new List<Location>();
        for (int x = entity.Location.X; x <= entity.Size; x++)
        {
            for (int y = entity.Location.Y; y <= entity.Size; y++)
            {
                tiles.Add(new Location(x, y));
            }
        }

        return tiles;
    }

    public Location[] GetOuterTiles(int size)
    {
        Location[] tiles = new Location[size * 4];
        int index = 0;

        for (int x = 0; x < size; x++)
        {
            tiles[index++] = new Location(X + x, Y - 1);
            tiles[index++] = new Location(X + x, Y + size);
        }

        for (int y = 0; y < size; y++)
        {
            tiles[index++] = new Location(X - 1, Y + y);
            tiles[index++] = new Location(X + size, Y + y);
        }

        return tiles;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        Location other = (Location)obj;
        return X == other.X && Y == other.Y && Height == other.Height;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Height);
    }

    public static bool operator ==(Location loc1, Location loc2)
    {
        if (ReferenceEquals(loc1, loc2))
            return true;
        if (ReferenceEquals(loc1, null) || ReferenceEquals(loc2, null))
            return false;
        return loc1.Equals(loc2);
    }

    public static bool operator !=(Location loc1, Location loc2)
    {
        return !(loc1 == loc2);
    }
}