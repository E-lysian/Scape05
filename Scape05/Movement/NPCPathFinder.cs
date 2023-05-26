using Scape05.World;

namespace Scape05.Entities;

public class NPCPathFinder
{
    private List<Location> points = new();
    
     public static Location FindPath(int size, int srcX, int srcY, int dstX, int dstY)
    {
        int stepX = 0, stepY = 0;
        Console.WriteLine("srcX: " + srcX + " srcY: " + srcY);
        Console.WriteLine("dstX: " + dstX + " dstY: " + dstY);
        // WEST, should check western on this tile and eastern on dest
        if (srcX > dstX  && !Region.BlockedWest(srcX, srcY, 0) && !Region.BlockedEast(srcX, srcY, 0))
        {
            stepX = -1;
        }
        // EAST, should check eastern on this tile and western on dest
        else if (srcX < dstX && !Region.BlockedEast(srcX, srcY, 0) && !Region.BlockedWest(srcX, srcY, 0))
        {
            stepX = 1;
        }

        // SOUTH, should check southern on this and northern on dest
        if (srcY > dstY && !Region.BlockedSouth(srcX, srcY, 0) && !Region.BlockedNorth(srcX, srcY, 0))
        {
            stepY = -1;
        }
        
        // NORTH, should check northern on this and southern on dest
        else if (srcY < dstY && !Region.BlockedNorth(srcX, srcY, 0) && !Region.BlockedSouth(srcX, srcY, 0))
        {
            stepY = 1;
        }

        if (stepX != 0 || stepY != 0)
        {
            // Console.WriteLine("stepX: " + dstX + " stepY: " + dstY);
            // Path p = new Path();
            // p.AddPoint(new Point(pos.X + stepX, pos.Y + stepY));
            // p.AddPoint(new Point(srcX + pos.X - radius, srcY + pos.Y - radius));
            // return p;
            //
            return new Location(stepX, stepY);
        }

        return null;
    }
}