using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;
using Scape05.Engine.Combat;
using Scape05.Engine.Data.Models;
using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Engine.Loaders;

public class NPCLoader
{
    public static List<NPCDefinition> npcs = new();
    public static void Load()
    {
        var npcDefs = File.ReadAllText("../../../Data/NPCDefinitions.json");
        var npcSpawns = File.ReadAllText("../../../Data/NPCSpawns.json");
        
        npcs = JsonConvert.DeserializeObject<List<NPCDefinition>>(npcDefs).OrderBy(o => o.Id).ToList();
        var NPCs = JsonConvert.DeserializeObject<List<NPCSpawn>>(npcSpawns).OrderBy(o => o.Id).ToList();

        foreach (var npc in NPCs)
        {
            LoadNewNPC(npc);
        }
    }

    private static void LoadNewNPC(NPCSpawn npcSpawn)
    {
        var npcDef = npcs.FirstOrDefault(x => x.Id == npcSpawn.Id);
        if (npcDef == null) return;

        if (npcDef.Name == null)
        {
            return;
        }
        
        var npc = new NPC
        {
            ModelId = npcDef.Id,
            Name = npcDef.Name,
            CombatLevel = npcDef.Combat,
            MaxHealth = npcDef.Hitpoints,
            Location = new Location(npcSpawn.X, npcSpawn.Y),
            CanWalk = npcSpawn.Walk == 1,
            Size = npcDef.Size,
            NeedsPlacement = true,
            Dead = false,
            Health = npcDef.Hitpoints == 0 ? 1 : npcDef.Hitpoints
        };
        
        npc.BuildArea = new BuildArea(npc);
        
        npc.CombatManager = new MeleeCombatHandler(npc)
        {
            Weapon = new Weapon(-1, -1, npcDef.AttackSpeed,
                new CombatAnimations(npcDef.AttackAnim, npcDef.DefenceAnim, -1, npcDef.DeathAnim), WeaponType.HAND)
        };

        npc.CombatBase.WeaponSpeed = npcDef.AttackSpeed;
        
        SetFaceBasedOnWalk(npc, npcSpawn.Walk);

        Server.AddNPC(npc);
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