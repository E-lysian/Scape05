using System.IO.Pipes;
using System.Reflection;
using Newtonsoft.Json;
using Scape05.Data.Npc;
using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Engine.Loaders;

public class NPCLoader
{
    public static void Load()
    {
        var json = File.ReadAllText("../../../Data/FinalNPC.json");
        var spawns = JsonConvert.DeserializeObject<List<FinalNPC>>(json).OrderBy(o => o.ModelId).ToList();
        var loadedNpcs = new List<FinalNPC>();

        foreach (var npc in spawns)
        {
            if (DateTime.Parse(npc.ReleaseDate) > DateTime.Parse("2005-06-18"))
                continue;

            loadedNpcs.Add(npc);

            NpcDefinitionDecoder.NpcDefinitions.TryGetValue(npc.ModelId, out var npcDef);
            if (npcDef != null)
            {
                var newNpc = CreateNewNPC(npc, npcDef);
                SetFaceBasedOnWalk(newNpc, npc.Walk);

                Server.AddNPC(newNpc);
            }
        }
    }

    private static NPC CreateNewNPC(FinalNPC npcData, NpcDefinition npcDef)
    {
        var newNpc = new NPC
        {
            Location = new Location(npcData.Spawn.X, npcData.Spawn.Y),
            ModelId = npcData.ModelId,
            CanWalk = npcData.Walk == 1,
            Size = npcDef.Size == 0 ? 1 : npcDef.Size,
            Name = npcDef.Name
        };

        return newNpc;
    }

    private static void SetFaceBasedOnWalk(NPC npc, int walkValue)
    {
        switch (walkValue)
        {
            case 2:
                npc.Face = new Face(npc.Location.X, npc.Location.Y + 1);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 3:
                npc.Face = new Face(npc.Location.X, npc.Location.Y - 1);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 4:
                npc.Face = new Face(npc.Location.X + 1, npc.Location.Y);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 5:
                npc.Face = new Face(npc.Location.X - 1, npc.Location.Y);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            default:
                npc.Face = new Face(npc.Location.X, npc.Location.Y);
                break;
        }
    }
}
public class FinalNPC
{
    public int ModelId { get; set; }
    public string Name { get; set; }
    public string ReleaseDate { get; set; }
    public Pos Spawn { get; set; }
    public int Walk { get; set; }
    public int MaxHit { get; set; }
    public int Attack { get; set; }
    public int Strength { get; set; }
}

public class Pos
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Height { get; set; }
}