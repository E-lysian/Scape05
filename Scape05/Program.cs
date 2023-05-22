using System.Diagnostics;
using Scape05.Data;
using Scape05.Data.Items;
using Scape05.Data.Npc;
using Scape05.Data.ObjectsDef;
using Scape05.Engine;

var sw = new Stopwatch();

Console.WriteLine("Started parsing the cache..");
sw.Start();
var ifs = new IndexedFileSystem("./cache", true);
new ObjectDefinitionDecoder(ifs).Run();
new ItemDefinitionDecoder(ifs).Run();
new NpcDefinitionDecoder(ifs).Run();
Console.WriteLine($"Finished parsing the cache in {sw.Elapsed.Milliseconds}ms");
sw.Stop();

GameEngine engine = new GameEngine();
engine.Start();