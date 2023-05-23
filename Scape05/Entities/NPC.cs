using Scape05.Misc;

namespace Scape05.Entities;

public class NPC : IEntity
{
    public int Index { get; set; }
    public string Name { get; set; }
    public int ModelId { get; set; }
    public Location Location { get; set; }
    public int HeadIcon { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public NPCUpdateFlags Flags { get; set; }
    public int Size { get; set; } = -1;
    public int CombatLevel { get; set; } = 1;
    public NPCMovementHandler MovementHandler { get; set; }
    public bool CanWalk { get; set; }
    public Face Face { get; set; }

    public NPC()
    {
        MovementHandler = new NPCMovementHandler(this);
    }

    public void Reset()
    {
        //Flags = NPCUpdateFlags.None;
        //IsUpdateRequired = false;
        // MovementHandler.PrimaryDirection = -1;
        // MovementHandler.SecondaryDirection = -1;
    }
}

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