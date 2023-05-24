using Scape05.Misc;

namespace Scape05.Data.Npc;

public class NpcDefinitionDecoder
{
    private readonly IndexedFileSystem fs;
    public static Dictionary<int, NpcDefinition> NpcDefinitions { get; set; } = new();

    public NpcDefinitionDecoder(IndexedFileSystem fs)
    {
        this.fs = fs;
    }

    public void Run()
    {
        try
        {
            var config = fs.GetArchive(0, 2);
            var data = config.GetEntry("npc.dat").GetBuffer();
            var idx = config.GetEntry("npc.idx").GetBuffer();

            using (var msData = new MemoryStream(data))
            using (var msIdx = new MemoryStream(idx))
            using (var dataReader = new BinaryReader(msData))
            using (var idxReader = new BinaryReader(msIdx))
            {
                int count = idxReader.ReadInt16BigEndian();
                int index = 2;
                var indices = new int[count];
                for (var i = 0; i < count; i++)
                {
                    indices[i] = index;
                    index += idxReader.ReadInt16BigEndian();
                }

                for (var i = 0; i < count; i++)
                {
                    dataReader.BaseStream.Position = indices[i];
                    var npcDef = Decode(i, dataReader);
                    NpcDefinitions.Add(npcDef.Id, npcDef);
                }
            }
        }
        catch (IOException e)
        {
            throw new Exception("Error decoding NpcDefinition.", e);
        }
    }

    public static NpcDefinition GetByModelId(int modelId)
    {
        return null;
    }
    
    private NpcDefinition Decode(int id, BinaryReader buffer)
    {
        var definition = new NpcDefinition();
        definition.Id = id;
        while (true)
        {
            var opcode = buffer.ReadByte() & 0xFF;

            if (opcode == 0)
            {
                //2745
                return definition;
            }

            if (opcode == 1)
            {
                int length = buffer.ReadByte();
                int[] models = new int[length];
                for (int index = 0; index < length; index++)
                {
                    models[index] = buffer.ReadInt16();
                }
            }
            else if (opcode == 2)
            {
                definition.Name = BufferUtil.ReadString(buffer);
            }
            else if (opcode == 3)
            {
                definition.Description = BufferUtil.ReadString(buffer);
            }
            else if (opcode == 12)
            {
                definition.Size = buffer.ReadByte();
            }
            else if (opcode == 13)
            {
                definition.StandAnimation = buffer.ReadInt16();
            }
            else if (opcode == 14)
            {
                definition.WalkAnimationIndex = buffer.ReadInt16();
            }
            else if (opcode == 17)
            {
                definition.WalkAnimationIndex1 = buffer.ReadInt16();
                definition.Turn180AnimationIndex = buffer.ReadInt16();
                definition.Turn90CWAnimationIndex = buffer.ReadInt16();
                definition.Turn90CCWAnimationIndex = buffer.ReadInt16();
            }
            else if (opcode >= 30 && opcode < 40)
            {
                string action = BufferUtil.ReadString(buffer);
                if (action.Equals("hidden"))
                {
                    action = null;
                }
                // definition.setInteraction(opcode - 30, action);
            }
            else if (opcode == 40)
            {
                int colourCount = buffer.ReadByte();
                for (var i = 0; i < colourCount; i++)
                {
                    buffer.ReadInt16();
                    buffer.ReadInt16();
                }
            }
            else if (opcode == 60)
            {
                int colourCount = buffer.ReadByte();
                for (var i = 0; i < colourCount; i++)
                {
                    buffer.ReadInt16();
                }
            }
            else if (opcode >= 90 && opcode < 92)
            {
                buffer.ReadInt16();
            }
            else if (opcode == 93)
            {
                definition.DrawMinimapDot = true;
            }
            else if (opcode == 95)
            {
                definition.CombatLevel = buffer.ReadInt16BigEndian();
            }
            else if (opcode == 97)
            {
                definition.ScaleXZ = buffer.ReadInt16();
            }
            else if (opcode == 98)
            {
                definition.ScaleY = buffer.ReadInt16();
            }
            else if (opcode == 99)
            {
                definition.Invisible = true;
            }
            else if (opcode == 100)
            {
                definition.LightModifier = buffer.ReadByte();
            }
            else if (opcode == 101)
            {
                definition.ShadowModifier = buffer.ReadByte();
            }
            else if (opcode == 102)
            {
                definition.HeadIcon = buffer.ReadInt16();
            }
            else if (opcode == 103)
            {
                definition.DegreesToTurn = buffer.ReadInt16();
            }
            else if (opcode == 106)
            {
                var varBitID = buffer.ReadInt16();
                if (varBitID == 65535)
                {
                    definition.VarbitId = -1;
                }

                var sessionSettingID = buffer.ReadInt16();
                if (sessionSettingID == 65535)
                {
                    definition.SessionSettingId = -1;
                }

                int[] childIds;
                var npcChildren = buffer.ReadByte();
                childIds = new int[npcChildren + 1];
                for (int n = 0; n < npcChildren; n++)
                {
                    var data = buffer.ReadInt16();
                    if (data == 65535)
                    {
                        data = -1;
                    }

                    childIds[n] = data;
                }
            }
        }
    }
}