using Scape05.Data;
using Scape05.Misc;
using Scape05.World;

namespace CacheReader.World;

public class RegionFactory
{
    // private static Region[] regions;

    private static Dictionary<int, Region> regions = new();
    // public static List<RealObject> RealObjects { get; set; } = new();


    public static Dictionary<int, Region> GetRegions()
    {
        return regions;
    }

    public static void Load(IndexedFileSystem fs)
    {
        var archive = Archive.Decode(fs.GetFile(new FileDescriptor(0, 5)));
        var entry = archive.GetEntry("map_index");
        var buffer = new MemoryStream(entry.GetBuffer());

        var size = buffer.Length / 7;
        // regions = new Region[size];
        var regionIds = new int[size];
        var mapGroundFileIds = new int[size];
        var mapObjectsFileIds = new int[size];
        var isMembers = new bool[size];

        for (var i = 0; i < size; i++)
        {
            regionIds[i] = buffer.ReadInt16BE();
            mapGroundFileIds[i] = buffer.ReadInt16BE();
            mapObjectsFileIds[i] = buffer.ReadInt16BE();
            isMembers[i] = buffer.ReadByte() == 0;
        }

        for (var i = 0; i < size; i++) regions.Add(regionIds[i], new Region(regionIds[i], isMembers[i]));

        for (var i = 0; i < size; i++)
        {
            var file1 = CompressionUtil.Degzip(fs.GetFile(4, mapObjectsFileIds[i]).ToArray());
            var file2 = CompressionUtil.Degzip(fs.GetFile(4, mapGroundFileIds[i]).ToArray());

            if (file1 == null || file2 == null) continue;

            try
            {
                LoadMaps(regionIds[i], new MemoryStream(file1), new MemoryStream(file2));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading map region: " + regionIds[i]);
            }
        }
    }

    private static void LoadMaps(int regionId, MemoryStream str1, MemoryStream str2)
    {
        var regionX = (regionId >> 8) * 64; // Region ID is bitshifted to get X position
        var regionY = (regionId & 0xff) * 64; // Region ID is bitshifted and AND'd against 0xff to get Y position
        var positionArray = new int[4, 64, 64];

        for (var localz = 0; localz < 4; localz++)
        for (var localx = 0; localx < 64; localx++)
        for (var localy = 0; localy < 64; localy++)
            while (true)
            {
                var v = str2.ReadByte();
                if (v == 0) break;

                if (v == 1)
                {
                    str2.Skip(1);
                    break;
                }

                if (v <= 49)
                    str2.Skip(1);
                else if (v <= 81) positionArray[localz, localx, localy] = v - 49; // Clipping data is gathered.
            }


        for (var localz = 0; localz < 4; localz++)
        for (var localx = 0; localx < 64; localx++)
        for (var localy = 0; localy < 64; localy++)
            if ((positionArray[localz, localx, localy] & 1) == 1)
            {
                var height = localz;
                if ((positionArray[1, localx, localy] & 2) == 2) height--;

                if (height >= 0 && height <= 3)
                    //GameEngine.getLogger(Region.class).debug("Adding clipping at x,y " + (regionX + localx) + "," + (regionY + localy) + " at height: " + localz);
                    Region.AddClipping(regionX + localx, regionY + localy, height, 0x200000);
            }


        var objectId = -1;
        int incr;
        while ((incr = str1.GetUSmart()) != 0)
        {
            objectId += incr;
            var location = 0;
            int incr2;
            while ((incr2 = str1.GetUSmart()) != 0)
            {
                location += incr2 - 1;
                var objectX = (location >> 6) & 0x3f;
                var objectY = location & 0x3f;
                var objectHeight = location >> 12;
                var objectData = str1.GetUByte();
                var type = objectData >> 2;
                var direction = objectData & 0x3;
                if (objectX < 0 || objectX >= 64 || objectY < 0 ||
                    objectY >= 64) continue; //Checks the object position is not outside the bounds of a region (0-64)

                if ((positionArray[1, objectX, objectY] & 2) == 2) objectHeight--;

                if (objectHeight >= 0 && objectHeight <= 3)
                    Region.AddObject(objectId, regionX + objectX, regionY + objectY, objectHeight, type, direction, ServerConfig.Startup);
            }
        }
    }
}