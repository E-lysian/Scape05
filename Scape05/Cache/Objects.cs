using Scape05.Data.ObjectsDef;

namespace Scape05.Data;

public class Objects
{
    public bool Bait { get; set; }
    public string BelongsTo { get; set; }
    public long Delay { get; set; }
    public long OriginalDelay { get; set; }
    public int Direction { get; set; }
    public int Height { get; set; }
    public int Id { get; set; }
    public int Ticks { get; set; }
    public int Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Xp { get; set; }
    public int Item { get; set; }
    public int Owner { get; set; }
    public int Target { get; set; }
    public int Times { get; set; }

    public Objects(int id, int x, int y, int height, int direction, int type, int ticks)
    {
        Id = id;
        X = x;
        Y = y;
        Height = height;
        Direction = direction;
        Type = type;
        Ticks = ticks;
    }

    public int GetId()
    {
        return Id;
    }

    public int GetX()
    {
        return X;
    }

    public int GetY()
    {
        return Y;
    }

    public int[] GetSize()
    {
        var def = ObjectDefinition.Lookup(Id);
        if (def == null) return new[] { 1, 1 };
        if (Id == 2781) return new[] { 3, 3 };
        int xLength;
        int yLength;
        if (Direction != 1 && Direction != 3)
        {
            xLength = def.Width;
            yLength = def.Length;
        }
        else
        {
            xLength = def.Length;
            yLength = def.Width;
        }

        return new[] { xLength, yLength };
    }

    public override string ToString()
    {
        return $"GameObject{{ Id={Id}, X={X}, Y={Y}, Height={Height} }}";
    }

    public int GetHeight()
    {
        return Height;
    }

    public int GetFace()
    {
        return Direction;
    }

    public int GetType()
    {
        return Type;
    }
}