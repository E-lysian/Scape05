using Scape05.Entities;

namespace Scape05.World.Clipping;

public class TileControl
{
    public static Location Generate(int x, int y, int z)
    {
        return new Location(x, y);
    }

    public static Location[] GetTiles(IEntity entity)
    {
        int size = 1;
        int tileCount = 0;

        if (entity is NPC npc)
        {
            size = npc.Size;
        }

        Location[] tiles = new Location[size * size];

        if (tiles.Length == 1)
        {
            tiles[0] = Generate(entity.Location.X, entity.Location.Y, entity.Location.Height);
        }
        else
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    tiles[tileCount++] = Generate(entity.Location.X + x, entity.Location.Y + y, entity.Location.Height);
                }
            }
        }
        
        return tiles;
    }

    public static Location[] GetTiles(IEntity entity, int[] location)
    {
        int size = 1;
        int tileCount = 0;

        if (entity is NPC)
        {
            size = ((NPC)entity).Size;
        }

        Location[] tiles = new Location[size * size];

        if (tiles.Length == 1)
        {
            tiles[0] = Generate(location[0], location[1], location[2]);
        }
        else
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    tiles[tileCount++] = Generate(location[0] + x, location[1] + y, location[2]);
                }
            }
        }
        return tiles;
    }

    public static int CalculateDistance(IEntity npc, IEntity following)
    {
        Location[] tiles = GetTiles(npc);

        var location = CurrentLocation(npc);
        int[] pointer = new int[tiles.Length];

        int lowestCount = 20;
        int count = 0;

        foreach (Location newTiles in tiles)
        {
            // if (newTiles == location)
            // {
            //     pointer[count++] = 0;
            // }
            // else
            // {
                pointer[count++] = CalculateDistance(newTiles, following);
            // }
        }

        foreach (int i in pointer)
        {
            if (i < lowestCount)
            {
                lowestCount = i;
            }
        }
        
        return lowestCount;
    }

    public static int CalculateDistance(Location location, IEntity other)
    {
        int X = Math.Abs(location.X - other.Location.X);
        int Y = Math.Abs(location.Y - other.Location.Y);
        return X > Y ? X : Y;
    }

    public static Location CurrentLocation(IEntity entity)
    {
        return entity.Location;
    }
}
