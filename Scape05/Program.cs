using CacheReader.World;
using Scape05.Data;
using Scape05.Data.Items;
using Scape05.Data.Npc;
using Scape05.Data.ObjectsDef;
using Scape05.Engine;
using Scape05.Misc;

void ParseCache(IndexedFileSystem ifs)
{
    new ObjectDefinitionDecoder(ifs).Run();
    new ItemDefinitionDecoder(ifs).Run();
    new NpcDefinitionDecoder(ifs).Run();
}

void LoadRegionFactory(IndexedFileSystem ifs)
{
    RegionFactory.Load(ifs);
    ServerConfig.Startup = false;
}

var ifs = new IndexedFileSystem("./cache", true);

Benchmarker.MeasureTime(() => ParseCache(ifs), "Parsing cache");
Benchmarker.MeasureTime(() => LoadRegionFactory(ifs), "Loading regions");

GameEngine engine = new GameEngine();
engine.Start();