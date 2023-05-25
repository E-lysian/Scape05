using CacheReader.World;
using Scape05.Data;
using Scape05.Data.ObjectsDef;

namespace Scape05.World;

public class Region
{
    private static int i = 0;

    public Region(int id, bool members)
    {
        Id = id;
        Members = members;
    }

    private int[][][] _clips { get; } = new int[4][][];
    private int[][][] _projectileClips { get; } = new int[4][][];
    private List<Objects> _realObjects { get; } = new();

    public int Id { get; }

    public bool Members { get; }

    public static Region GetRegion(int x, int y)
    {
        if (RegionFactory.GetRegions().TryGetValue(GetRegionId(x, y), out var region)) return region;

        return null;
    }

    public static int GetRegionId(int x, int y)
    {
        var regionX = x >> 3;
        var regionY = y >> 3;
        var regionId = ((regionX >> 3) << 8) + (regionY >> 3);
        return regionId;
    }

    private void AddClip(int x, int y, int height, int shift)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_clips[height] == null)
        {
            _clips[height] = new int[64][];
            for (var i = 0; i < 64; i++) _clips[height][i] = new int[64];
        }

        _clips[height][x - regionAbsX][y - regionAbsY] |= shift;
    }

    private void AddProjectileClip(int x, int y, int height, int shift)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_projectileClips[height] == null)
        {
            _projectileClips[height] = new int[64][];
            for (var i = 0; i < 64; i++) _projectileClips[height][i] = new int[64];
        }

        _projectileClips[height][x - regionAbsX][y - regionAbsY] |= shift;
    }

    private int GetClip(int x, int y, int height)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_clips[height] == null) return 0;

        return _clips[height][x - regionAbsX][y - regionAbsY];
    }

    private int GetProjectileClip(int x, int y, int height)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_projectileClips[height] == null) return 0;

        return _projectileClips[height][x - regionAbsX][y - regionAbsY];
    }

    public static void AddClipping(int x, int y, int height, int shift)
    {
        if (RegionFactory.GetRegions().TryGetValue(GetRegionId(x, y), out var region))
            region.AddClip(x, y, height, shift);
    }

    private static void AddProjectileClipping(int x, int y, int height, int shift)
    {
        if (RegionFactory.GetRegions().TryGetValue(GetRegionId(x, y), out var region))
            region.AddProjectileClip(x, y, height, shift);
    }

    public static int GetClipping(int x, int y, int height)
    {
        if (height > 3) height = 0;

        foreach (var r in RegionFactory.GetRegions())
            if (r.Value.Id == GetRegionId(x, y))
                return r.Value.GetClip(x, y, height);

        return 0;
    }

    public static int GetProjectileClipping(int x, int y, int height)
    {
        if (height > 3) height = 0;

        foreach (var r in RegionFactory.GetRegions())
            if (r.Value.Id == GetRegionId(x, y))
                return r.Value.GetProjectileClip(x, y, height);

        return 0;
    }

    private static void AddClippingForSolidObject(int x, int y, int height, int xLength, int yLength, bool flag)
    {
        var clipping = 256;
        if (flag) clipping += 0x20000;

        for (var i = x; i < x + xLength; i++)
        for (var i2 = y; i2 < y + yLength; i2++)
            AddClipping(i, i2, height, clipping);
    }

    private static void AddProjectileClippingForSolidObject(int x, int y, int height, int xLength, int yLength,
        bool flag)
    {
        var clipping = 256;
        if (flag) clipping += 0x20000;

        for (var i = x; i < x + xLength; i++)
        for (var j = y; j < y + yLength; j++)
            AddProjectileClipping(i, j, height, clipping);
    }

    private static void AddClippingForVariableObject(int x, int y, int height, int type, int direction, bool flag)
    {
        if (type == 0)
        {
            if (direction == 0)
            {
                AddClipping(x, y, height, 128);
                AddClipping(x - 1, y, height, 8);
            }
            else if (direction == 1)
            {
                AddClipping(x, y, height, 2);
                AddClipping(x, y + 1, height, 32);
            }
            else if (direction == 2)
            {
                AddClipping(x, y, height, 8);
                AddClipping(x + 1, y, height, 128);
            }
            else if (direction == 3)
            {
                AddClipping(x, y, height, 32);
                AddClipping(x, y - 1, height, 2);
            }
        }
        else if (type == 1 || type == 3)
        {
            if (direction == 0)
            {
                AddClipping(x, y, height, 1);
                AddClipping(x - 1, y, height, 16);
            }
            else if (direction == 1)
            {
                AddClipping(x, y, height, 4);
                AddClipping(x + 1, y + 1, height, 64);
            }
            else if (direction == 2)
            {
                AddClipping(x, y, height, 16);
                AddClipping(x + 1, y - 1, height, 1);
            }
            else if (direction == 3)
            {
                AddClipping(x, y, height, 64);
                AddClipping(x - 1, y - 1, height, 4);
            }
        }
        else if (type == 2)
        {
            if (direction == 0)
            {
                AddClipping(x, y, height, 130);
                AddClipping(x - 1, y, height, 8);
                AddClipping(x, y + 1, height, 32);
            }
            else if (direction == 1)
            {
                AddClipping(x, y, height, 10);
                AddClipping(x, y + 1, height, 32);
                AddClipping(x + 1, y, height, 128);
            }
            else if (direction == 2)
            {
                AddClipping(x, y, height, 40);
                AddClipping(x + 1, y, height, 128);
                AddClipping(x, y - 1, height, 2);
            }
            else if (direction == 3)
            {
                AddClipping(x, y, height, 160);
                AddClipping(x, y - 1, height, 2);
                AddClipping(x - 1, y, height, 8);
            }
        }

        if (flag)
        {
            if (type == 0)
            {
                if (direction == 0)
                {
                    AddClipping(x, y, height, 65536);
                    AddClipping(x - 1, y, height, 4096);
                }
                else if (direction == 1)
                {
                    AddClipping(x, y, height, 1024);
                    AddClipping(x, y + 1, height, 16384);
                }
                else if (direction == 2)
                {
                    AddClipping(x, y, height, 4096);
                    AddClipping(x + 1, y, height, 65536);
                }
                else if (direction == 3)
                {
                    AddClipping(x, y, height, 16384);
                    AddClipping(x, y - 1, height, 1024);
                }
            }

            if (type == 1 || type == 3)
            {
                if (direction == 0)
                {
                    AddClipping(x, y, height, 512);
                    AddClipping(x - 1, y + 1, height, 8192);
                }
                else if (direction == 1)
                {
                    AddClipping(x, y, height, 2048);
                    AddClipping(x + 1, y + 1, height, 32768);
                }
                else if (direction == 2)
                {
                    AddClipping(x, y, height, 8192);
                    AddClipping(x + 1, y + 1, height, 512);
                }
                else if (direction == 3)
                {
                    AddClipping(x, y, height, 32768);
                    AddClipping(x - 1, y - 1, height, 2048);
                }
            }
            else if (type == 2)
            {
                if (direction == 0)
                {
                    AddClipping(x, y, height, 66560);
                    AddClipping(x - 1, y, height, 4096);
                    AddClipping(x, y + 1, height, 16384);
                }
                else if (direction == 1)
                {
                    AddClipping(x, y, height, 5120);
                    AddClipping(x, y + 1, height, 16384);
                    AddClipping(x + 1, y, height, 65536);
                }
                else if (direction == 2)
                {
                    AddClipping(x, y, height, 20480);
                    AddClipping(x + 1, y, height, 65536);
                    AddClipping(x, y - 1, height, 1024);
                }
                else if (direction == 3)
                {
                    AddClipping(x, y, height, 81920);
                    AddClipping(x, y - 1, height, 1024);
                    AddClipping(x - 1, y, height, 4096);
                }
            }
        }
    }

    private static void AddProjectileClippingForVariableObject(int x, int y, int height, int type, int direction,
        bool flag)
    {
        if (type == 0)
        {
            if (direction == 0)
            {
                AddProjectileClipping(x, y, height, 128);
                AddProjectileClipping(x - 1, y, height, 8);
            }
            else if (direction == 1)
            {
                AddProjectileClipping(x, y, height, 2);
                AddProjectileClipping(x, y + 1, height, 32);
            }
            else if (direction == 2)
            {
                AddProjectileClipping(x, y, height, 8);
                AddProjectileClipping(x + 1, y, height, 128);
            }
            else if (direction == 3)
            {
                AddProjectileClipping(x, y, height, 32);
                AddProjectileClipping(x, y - 1, height, 2);
            }
        }
        else if (type == 1 || type == 3)
        {
            if (direction == 0)
            {
                AddProjectileClipping(x, y, height, 1);
                AddProjectileClipping(x - 1, y, height, 16);
            }
            else if (direction == 1)
            {
                AddProjectileClipping(x, y, height, 4);
                AddProjectileClipping(x + 1, y + 1, height, 64);
            }
            else if (direction == 2)
            {
                AddProjectileClipping(x, y, height, 16);
                AddProjectileClipping(x + 1, y - 1, height, 1);
            }
            else if (direction == 3)
            {
                AddProjectileClipping(x, y, height, 64);
                AddProjectileClipping(x - 1, y - 1, height, 4);
            }
        }
        else if (type == 2)
        {
            if (direction == 0)
            {
                AddProjectileClipping(x, y, height, 130);
                AddProjectileClipping(x - 1, y, height, 8);
                AddProjectileClipping(x, y + 1, height, 32);
            }
            else if (direction == 1)
            {
                AddProjectileClipping(x, y, height, 10);
                AddProjectileClipping(x, y + 1, height, 32);
                AddProjectileClipping(x + 1, y, height, 128);
            }
            else if (direction == 2)
            {
                AddProjectileClipping(x, y, height, 40);
                AddProjectileClipping(x + 1, y, height, 128);
                AddProjectileClipping(x, y - 1, height, 2);
            }
            else if (direction == 3)
            {
                AddProjectileClipping(x, y, height, 160);
                AddProjectileClipping(x, y - 1, height, 2);
                AddProjectileClipping(x - 1, y, height, 8);
            }
        }


        if (flag)
        {
            if (type == 0)
            {
                if (direction == 0)
                {
                    AddProjectileClipping(x, y, height, 65536);
                    AddProjectileClipping(x - 1, y, height, 4096);
                }
                else if (direction == 1)
                {
                    AddProjectileClipping(x, y, height, 1024);
                    AddProjectileClipping(x, y + 1, height, 16384);
                }
                else if (direction == 2)
                {
                    AddProjectileClipping(x, y, height, 4096);
                    AddProjectileClipping(x + 1, y, height, 65536);
                }
                else if (direction == 3)
                {
                    AddProjectileClipping(x, y, height, 16384);
                    AddProjectileClipping(x, y - 1, height, 1024);
                }
            }

            if (type == 1 || type == 3)
            {
                if (direction == 0)
                {
                    AddProjectileClipping(x, y, height, 512);
                    AddProjectileClipping(x - 1, y + 1, height, 8192);
                }
                else if (direction == 1)
                {
                    AddProjectileClipping(x, y, height, 2048);
                    AddProjectileClipping(x + 1, y + 1, height, 32768);
                }
                else if (direction == 2)
                {
                    AddProjectileClipping(x, y, height, 8192);
                    AddProjectileClipping(x + 1, y + 1, height, 512);
                }
                else if (direction == 3)
                {
                    AddProjectileClipping(x, y, height, 32768);
                    AddProjectileClipping(x - 1, y - 1, height, 2048);
                }
            }
            else if (type == 2)
            {
                if (direction == 0)
                {
                    AddProjectileClipping(x, y, height, 66560);
                    AddProjectileClipping(x - 1, y, height, 4096);
                    AddProjectileClipping(x, y + 1, height, 16384);
                }
                else if (direction == 1)
                {
                    AddProjectileClipping(x, y, height, 5120);
                    AddProjectileClipping(x, y + 1, height, 16384);
                    AddProjectileClipping(x + 1, y, height, 65536);
                }
                else if (direction == 2)
                {
                    AddProjectileClipping(x, y, height, 20480);
                    AddProjectileClipping(x + 1, y, height, 65536);
                    AddProjectileClipping(x, y - 1, height, 1024);
                }
                else if (direction == 3)
                {
                    AddProjectileClipping(x, y, height, 81920);
                    AddProjectileClipping(x, y - 1, height, 1024);
                    AddProjectileClipping(x - 1, y, height, 4096);
                }
            }
        }
    }

    public static void AddObject(int objectId, int x, int y, int height, int type, int direction, bool startUp)
    {
        if (ObjectDefinition.Lookup(objectId) == null)
        {
        }

        int xLength;
        int yLength;

        if (direction != 1 && direction != 3)
        {
            xLength = ObjectDefinition.Lookup(objectId).Width;
            yLength = ObjectDefinition.Lookup(objectId).Length;
        }
        else
        {
            xLength = ObjectDefinition.Lookup(objectId).Length;
            yLength = ObjectDefinition.Lookup(objectId).Width;
        }

        if (type == 22)
        {
            if (ObjectDefinition.Lookup(objectId).IsInteractive && ObjectDefinition.Lookup(objectId).IsSolid)
            {
                AddClipping(x, y, height, 0x200000);

                if (ObjectDefinition.Lookup(objectId).IsImpenetrable) AddProjectileClipping(x, y, height, 0x200000);
            }
        }
        else if (type >= 9)
        {
            if (ObjectDefinition.Lookup(objectId).IsSolid)
            {
                AddClippingForSolidObject(x, y, height, xLength, yLength, ObjectDefinition.Lookup(objectId).IsClipped);

                if (ObjectDefinition.Lookup(objectId).IsImpenetrable)
                    AddProjectileClippingForSolidObject(x, y, height, xLength, yLength,
                        ObjectDefinition.Lookup(objectId).IsClipped);
            }
        }
        else if (type >= 0 && type <= 3)
        {
            if (ObjectDefinition.Lookup(objectId).IsSolid)
            {
                AddClippingForVariableObject(x, y, height, type, direction,
                    ObjectDefinition.Lookup(objectId).IsClipped);

                if (ObjectDefinition.Lookup(objectId).IsImpenetrable)
                    AddProjectileClippingForVariableObject(x, y, height, type, direction,
                        ObjectDefinition.Lookup(objectId).IsClipped);
            }
        }

        var r = GetRegion(x, y);

        if (r != null)
        {
            if (startUp)
                r._realObjects.Add(new Objects(objectId, x, y, height, direction, type, 0));
            else if (!ObjectExists(objectId, x, y, height))
                r._realObjects.Add(new Objects(objectId, x, y, height, direction, type, 0));
        }
    }

    public static bool BlockedNorth(int x, int y, int z)
    {
        return (GetClipping(x, y + 1, z) & 0x1280120) != 0;
    }

    public static bool BlockedEast(int x, int y, int z)
    {
        return (GetClipping(x + 1, y, z) & 0x1280180) != 0;
    }

    public static bool BlockedWest(int x, int y, int z)
    {
        return (GetClipping(x - 1, y, z) & 0x1280108) != 0;
    }

    public static bool BlockedSouth(int x, int y, int z)
    {
        return (GetClipping(x, y - 1, z) & 0x1280102) != 0;
    }

    public static bool ProjectileBlockedNorth(int x, int y, int z)
    {
        return (GetProjectileClipping(x, y + 1, z) & 0x1280120) != 0;
    }

    public static bool ProjectileBlockedEast(int x, int y, int z)
    {
        return (GetProjectileClipping(x + 1, y, z) & 0x1280180) != 0;
    }

    public static bool ProjectileBlockedSouth(int x, int y, int z)
    {
        return (GetProjectileClipping(x, y - 1, z) & 0x1280102) != 0;
    }

    public static bool ProjectileBlockedWest(int x, int y, int z)
    {
        return (GetProjectileClipping(x - 1, y, z) & 0x1280108) != 0;
    }

    public static bool ProjectileBlockedNorthEast(int x, int y, int z)
    {
        return (GetProjectileClipping(x + 1, y + 1, z) & 0x12801e0) != 0;
    }

    public static bool ProjectileBlockedNorthWest(int x, int y, int z)
    {
        return (GetProjectileClipping(x - 1, y + 1, z) & 0x1280138) != 0;
    }

    public static bool ProjectileBlockedSouthEast(int x, int y, int z)
    {
        return (GetProjectileClipping(x + 1, y - 1, z) & 0x1280183) != 0;
    }

    public static bool ProjectileBlockedSouthWest(int x, int y, int z)
    {
        return (GetProjectileClipping(x - 1, y - 1, z) & 0x128010e) != 0;
    }

    public static bool GetClipping(int x, int y, int height, int moveTypeX, int moveTypeY)
    {
        try
        {
            if (height > 3) height = 0;

            var checkX = x + moveTypeX;
            var checkY = y + moveTypeY;
            if (moveTypeX == -1 && moveTypeY == 0) return (GetClipping(x, y, height) & 0x1280108) == 0;

            if (moveTypeX == 1 && moveTypeY == 0) return (GetClipping(x, y, height) & 0x1280180) == 0;

            if (moveTypeX == 0 && moveTypeY == -1) return (GetClipping(x, y, height) & 0x1280102) == 0;

            if (moveTypeX == 0 && moveTypeY == 1) return (GetClipping(x, y, height) & 0x1280120) == 0;

            if (moveTypeX == -1 && moveTypeY == -1)
                return (GetClipping(x, y, height) & 0x128010e) == 0
                       && (GetClipping(checkX - 1, checkY, height) & 0x1280108) == 0
                       && (GetClipping(checkX - 1, checkY, height) & 0x1280102) == 0;

            if (moveTypeX == 1 && moveTypeY == -1)
                return (GetClipping(x, y, height) & 0x1280183) == 0
                       && (GetClipping(checkX + 1, checkY, height) & 0x1280180) == 0
                       && (GetClipping(checkX, checkY - 1, height) & 0x1280102) == 0;

            if (moveTypeX == -1 && moveTypeY == 1)
                return (GetClipping(x, y, height) & 0x1280138) == 0
                       && (GetClipping(checkX - 1, checkY, height) & 0x1280108) == 0
                       && (GetClipping(checkX, checkY + 1, height) & 0x1280120) == 0;

            if (moveTypeX == 1 && moveTypeY == 1)
                return (GetClipping(x, y, height) & 0x12801e0) == 0
                       && (GetClipping(checkX + 1, checkY, height) & 0x1280180) == 0
                       && (GetClipping(checkX, checkY + 1, height) & 0x1280120) == 0;

            Console.WriteLine("[FATAL ERROR]: At getClipping: " + x + ", "
                              + y + ", " + height + ", " + moveTypeX + ", "
                              + moveTypeY);
            return false;
        }
        catch (Exception e)
        {
            return true;
        }
    }

    public static Objects GetObject(int id, int x, int y, int z)
    {
        var r = GetRegion(x, y);
        if (r == null)
            return null;

        foreach (var o in r._realObjects)
            if (o.Id == id)
                if (o.X == x && o.Y == y && o.Height == z)
                    return o;

        return null;
    }


    public Objects GetRealObject(Region region, int x, int y, int z, int objId)
    {
        var r = region;
        if (r == null)
            return null;

        foreach (var o in r._realObjects)
            if (o.Id == objId)
                if (o.X == x && o.Y == y && o.Height == z)
                    return o;

        return null;
    }

    public static bool ObjectExists(int id, int x, int y, int z)
    {
        var r = GetRegion(x, y);
        if (r == null)
            return false;

        foreach (var o in r._realObjects)
            if (o.Id == id)
                if (o.X == x && o.Y == y && o.Height == z)
                    return true;

        return false;
    }

    private void RemoveClip(int x, int y, int height)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_clips[height] == null)
        {
            _clips[height] = new int[64][];
            for (var i = 0; i < 64; i++) _clips[height][i] = new int[64];
        }

        _clips[height][x - regionAbsX][y - regionAbsY] = 0;
    }

    public static bool CanMove(int x, int y, int z, int direction)
    {
        switch (direction)
        {
            case 6:
                return (GetClipping(x, y - 1, z) & 0x1280102) == 0;

            case 3:
                return (GetClipping(x - 1, y, z) & 0x1280108) == 0;

            case 1:
                return (GetClipping(x, y + 1, z) & 0x1280120) == 0;

            case 4:
                return (GetClipping(x + 1, y, z) & 0x1280180) == 0;

            case 5:
                return (GetClipping(x - 1, y - 1, z) & 0x128010e) == 0
                       && (GetClipping(x - 1, y, z) & 0x1280108) == 0
                       && (GetClipping(x, y - 1, z) & 0x1280102) == 0;

            case 0:
                return (GetClipping(x - 1, y + 1, z) & 0x1280138) == 0
                       && (GetClipping(x - 1, y, z) & 0x1280108) == 0
                       && (GetClipping(x, y + 1, z) & 0x1280120) == 0;

            case 7:
                return (GetClipping(x + 1, y - 1, z) & 0x1280183) == 0
                       && (GetClipping(x + 1, y, z) & 0x1280180) == 0
                       && (GetClipping(x, y - 1, z) & 0x1280102) == 0;

            case 2:
                return (GetClipping(x + 1, y + 1, z) & 0x12801e0) == 0
                       && (GetClipping(x + 1, y, z) & 0x1280180) == 0
                       && (GetClipping(x, y + 1, z) & 0x1280120) == 0;

            default:
                throw new ArgumentException("Invalid direction: " + direction);
        }
    }

    public static bool CanShoot(int x, int y, int z, int direction)
    {
        switch (direction)
        {
            case 0:
                return !ProjectileBlockedNorthWest(x, y, z) && !ProjectileBlockedNorth(x, y, z)
                                                            && !ProjectileBlockedWest(x, y, z);
            case 1:
                return !ProjectileBlockedNorth(x, y, z);
            case 2:
                return !ProjectileBlockedNorthEast(x, y, z) && !ProjectileBlockedNorth(x, y, z)
                                                            && !ProjectileBlockedEast(x, y, z);
            case 3:
                return !ProjectileBlockedWest(x, y, z);
            case 4:
                return !ProjectileBlockedEast(x, y, z);
            case 5:
                return !ProjectileBlockedSouthWest(x, y, z) && !ProjectileBlockedSouth(x, y, z)
                                                            && !ProjectileBlockedWest(x, y, z);
            case 6:
                return !ProjectileBlockedSouth(x, y, z);
            case 7:
                return !ProjectileBlockedSouthEast(x, y, z) && !ProjectileBlockedSouth(x, y, z)
                                                            && !ProjectileBlockedEast(x, y, z);
            default:
                throw new ArgumentException("Invalid direction: " + direction);
        }
    }

    public static bool canMove(int startX, int startY, int endX, int endY, int height, int xLength, int yLength)
    {
        var diffX = endX - startX;
        var diffY = endY - startY;
        var max = Math.Max(Math.Abs(diffX), Math.Abs(diffY));

        for (var ii = 0; ii < max; ii++)
        {
            var currentX = endX - diffX;
            var currentY = endY - diffY;
            for (var i = 0; i < xLength; i++)
            for (var i2 = 0; i2 < yLength; i2++)
                if (diffX < 0 && diffY < 0)
                {
                    if ((GetClipping(currentX + i - 1,
                            currentY + i2 - 1, height) & 0x128010e) != 0
                        || (GetClipping(currentX + i - 1, currentY
                                                          + i2, height) & 0x1280108) != 0
                        || (GetClipping(currentX + i,
                            currentY + i2 - 1, height) & 0x1280102) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY > 0)
                {
                    if ((GetClipping(currentX + i + 1, currentY + i2 + 1,
                            height) & 0x12801e0) != 0
                        || (GetClipping(currentX + i + 1,
                            currentY + i2, height) & 0x1280180) != 0
                        || (GetClipping(currentX + i,
                            currentY + i2 + 1, height) & 0x1280120) != 0)
                        return false;
                }
                else if (diffX < 0 && diffY > 0)
                {
                    if ((GetClipping(currentX + i - 1, currentY + i2 + 1,
                            height) & 0x1280138) != 0
                        || (GetClipping(currentX + i - 1, currentY
                                                          + i2, height) & 0x1280108) != 0
                        || (GetClipping(currentX + i,
                            currentY + i2 + 1, height) & 0x1280120) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY < 0)
                {
                    if ((GetClipping(currentX + i + 1, currentY + i2 - 1,
                            height) & 0x1280183) != 0
                        || (GetClipping(currentX + i + 1,
                            currentY + i2, height) & 0x1280180) != 0
                        || (GetClipping(currentX + i,
                            currentY + i2 - 1, height) & 0x1280102) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY == 0)
                {
                    if ((GetClipping(currentX + i + 1, currentY + i2,
                            height) & 0x1280180) != 0)
                        return false;
                }
                else if (diffX < 0 && diffY == 0)
                {
                    if ((GetClipping(currentX + i - 1, currentY + i2,
                            height) & 0x1280108) != 0)
                        return false;
                }
                else if (diffX == 0 && diffY > 0)
                {
                    if ((GetClipping(currentX + i, currentY + i2 + 1,
                            height) & 0x1280120) != 0)
                        return false;
                }
                else if (diffX == 0
                         && diffY < 0
                         && (GetClipping(currentX + i, currentY + i2 - 1,
                             height) & 0x1280102) != 0)
                {
                    return false;
                }

            if (diffX < 0)
                diffX++;
            else if (diffX > 0)
                diffX--;
            if (diffY < 0)
                diffY++;
            else if (diffY > 0)
                diffY--;
        }

        return true;
    }

    public static bool canInteract(int dstX, int dstY, int absX, int absY, int curX,
        int curY, int sizeX, int sizeY, int walkToData)
    {
        if ((walkToData & 0x80000000) != 0)
            if (curX == dstX && curY == dstY)
                return false;

        var maxX = dstX + sizeX - 1;
        var maxY = dstY + sizeY - 1;

        var region = GetRegion(absX, absY);
        var clipping = region.GetClip(absX, absY, 0);

        if (curX >= dstX && maxX >= curX && dstY <= curY && maxY >= curY) return true;

        if (curX == dstX - 1 && curY >= dstY && curY <= maxY && (clipping & 8) == 0 &&
            (walkToData & 8) == 0) return true;

        if (curX == maxX + 1 && curY >= dstY && curY <= maxY && (clipping & 0x80) == 0 &&
            (walkToData & 2) == 0) return true;

        return (curY == dstY - 1 && curX >= dstX && curX <= maxX && (clipping & 2) == 0 && (walkToData & 4) == 0)
               || (curY == maxY + 1 && curX >= dstX && curX <= maxX && (clipping & 0x20) == 0
                   && (walkToData & 1) == 0);
    }
}