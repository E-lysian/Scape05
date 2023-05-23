namespace Scape05.Entities;

public class Face
{
    public int X { get; set; }
    public int Y { get; set; }

    public Face(int x, int y)
    {
        X = x * 2 + 1;
        Y = y * 2 + 1;
    }
}