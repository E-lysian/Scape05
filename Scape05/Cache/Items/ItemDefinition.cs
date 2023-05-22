namespace Scape05.Data.Items;

public class ItemDefinition
{
    private static readonly Dictionary<int, int> notes = new();
    private static readonly Dictionary<int, int> notesInverse = new();
    private static ItemDefinition[] definitions;

    private static readonly ItemDefinition NULL_DEF = new(-1);

    private readonly string[] groundActions = new string[5];
    private readonly string[] inventoryActions = new string[5];

    private int noteGraphicId = -1;
    private int noteInfoId = -1;

    static ItemDefinition()
    {
        NULL_DEF.Name = "";
        NULL_DEF.Description = "";
    }

    public ItemDefinition(int id)
    {
        Id = id;
    }

    public int Id { get; }
    public string Description { get; private set; }
    public bool Members { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public int Team { get; set; }
    public int Value { get; set; } = 1;

    public static int Count() => definitions.Length;

    public static ItemDefinition[] GetDefinitions() => definitions;

    public static void Init(ItemDefinition[] definitions)
    {
        ItemDefinition.definitions = definitions;

        for (var id = 0; id < definitions.Length; id++)
        {
            var def = definitions[id];

            if (def.Id != id)
                throw new Exception("Item definition id mismatch.");

            if (def.IsNote())
            {
                def.ConvertToNote();
                notes.Add(def.GetNoteInfoId(), def.Id);
            }
        }
    }

    public static int ItemToNoteId(int id) => notes.ContainsKey(id) ? notes[id] : id;

    public static ItemDefinition Lookup(int id) => id >= 0 && id < definitions.Length ? definitions[id] : NULL_DEF;

    public static int NoteToItemId(int id) => notesInverse.ContainsKey(id) ? notesInverse[id] : id;

    public string GetDescription() => Description;

    public void SetDescription(string description) => Description = description;

    public string GetGroundAction(int id) => id >= 0 && id < groundActions.Length ? groundActions[id] : throw new IndexOutOfRangeException("Ground action id is out of bounds.");

    public int GetId() => Id;

    public string GetInventoryAction(int id) => id >= 0 && id < inventoryActions.Length ? inventoryActions[id] : throw new IndexOutOfRangeException("Inventory action id is out of bounds.");

    public string GetName() => Name;

    public void SetName(string name) => Name = name;

    public int GetNoteGraphicId() => noteGraphicId;

    public void SetNoteGraphicId(int noteGraphicId) => this.noteGraphicId = noteGraphicId;

    public int GetNoteInfoId() => noteInfoId;

    public void SetNoteInfoId(int noteInfoId) => this.noteInfoId = noteInfoId;

    public int GetTeam() => Team;

    public void SetTeam(int team) => Team = team;

    public int GetValue() => Value;

    public void SetValue(int value) => Value = value;

    public bool IsMembersOnly() => Members;

    public void SetMembersOnly(bool members) => Members = members;

    public bool IsNote() => noteGraphicId != -1 && noteInfoId != -1;

    public bool IsStackable() => Stackable;

    public void SetStackable(bool stackable) => Stackable = stackable;

    public void SetGroundAction(int id, string action)
    {
        if (id >= 0 && id < groundActions.Length)
            groundActions[id] = action;
        else
            throw new IndexOutOfRangeException("Ground action id is out of bounds.");
    }

    public void SetInventoryAction(int id, string action)
    {
        if (id >= 0 && id < inventoryActions.Length)
            inventoryActions[id] = action;
        else
            throw new IndexOutOfRangeException("Inventory action id is out of bounds.");
    }

    private void ConvertToNote()
    {
        if (IsNote())
        {
            if (noteInfoId != -1 && noteGraphicId != -1)
            {
                var noteInfoDef = Lookup(noteInfoId);
                var noteGraphicDef = Lookup(noteGraphicId);

                if (noteInfoDef != null && noteGraphicDef != null)
                {
                    if (!notesInverse.ContainsKey(noteInfoDef.Id))
                    {
                        noteInfoDef.noteGraphicId = noteGraphicDef.Id;
                        noteGraphicDef.noteInfoId = noteInfoDef.Id;
                        notesInverse.Add(noteInfoDef.Id, noteGraphicDef.Id);
                    }
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Item cannot be noted.");
        }
    }
}