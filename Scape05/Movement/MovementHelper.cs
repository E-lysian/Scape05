namespace Scape05.Entities;

public static  class MovementHelper
{
    public static sbyte[] DeltaX = { 0, 1, 1, 1, 0, -1, -1, -1 };
    public static sbyte[] DeltaY = { 1, 1, 0, -1, -1, -1, 0, 1 };
    public static sbyte[] DirectionToClient = { 1, 2, 4, 7, 6, 5, 3, 0 };

    public static int[] DIRECTION_DELTA_X = { -1, 0, 1, -1, 1, -1, 0, 1 };
    public static int[] DIRECTION_DELTA_Y = { 1, 1, 1, 0, 0, -1, -1, -1 };
    
    public static int GetDirection(int dx, int dy)
    {
        if (dx < 0)
            return (dy < 0) ? 5 : (dy > 0) ? 0 : 3;
        if (dx > 0)
            return (dy < 0) ? 7 : (dy > 0) ? 2 : 4;
        if (dy < 0)
            return 6;
        if (dy > 0)
            return 1;
        return -1;
    }
    
    public static int GetDirection(int srcX, int srcY, int destX, int destY)
    {
        int dx = destX - srcX;
        int dy = destY - srcY;
        double angle = Math.Atan2(dy, dx) * (180.0 / Math.PI);
        int quadrant = (dx < 0) ? 2 : 0;
        int octant = (int)((90.0 - angle) / 45.0);
        return (dx == 0) ? ((dy > 0) ? 0 : 8) : (quadrant + octant) & 0xF;
    }
    
    
    public static int DirectionFromDelta(int deltaX, int deltaY)
    {
        for (int i = 0; i < DeltaX.Length; i++)
        {
            if (DeltaX[i] == deltaX && DeltaY[i] == deltaY)
                return DirectionToClient[i];
        }

        throw new ArgumentException($"Cannot find direction {deltaX} {deltaY}");
    }


}