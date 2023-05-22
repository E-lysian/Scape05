using Scape05.Misc;

namespace Scape05.Data.Items;

public class ItemDefinitionDecoder
{
    private readonly IndexedFileSystem fs;

    public ItemDefinitionDecoder(IndexedFileSystem fs)
    {
        this.fs = fs;
    }

    public void Run()
    {
        try
        {
            var config = fs.GetArchive(0, 2);
            var data = config.GetEntry("obj.dat").GetBuffer();
            var idx = config.GetEntry("obj.idx").GetBuffer();

            using (var msData = new MemoryStream(data))
            using (var msIdx = new MemoryStream(idx))
            using (var dataReader = new BinaryReader(msData))
            using (var idxReader = new BinaryReader(msIdx))
            {
                int count = idxReader.ReadInt16BigEndian(), index = 2;
                var indices = new int[count];
                for (var i = 0; i < count; i++)
                {
                    indices[i] = index;
                    index += idxReader.ReadInt16BigEndian();
                }

                var definitions = new ItemDefinition[count];
                for (var i = 0; i < count; i++)
                {
                    dataReader.BaseStream.Position = indices[i];
                    definitions[i] = Decode(i, dataReader);
                }

                ItemDefinition.Init(definitions);
            }
        }
        catch (IOException e)
        {
            throw new Exception("Error decoding ItemDefinitions.", e);
        }
    }

    private ItemDefinition Decode(int id, BinaryReader buffer)
    {
        var definition = new ItemDefinition(id);

        while (true)
        {
            var opcode = buffer.ReadByte() & 0xFF;

            if (opcode == 0)
            {
                return definition;
            }

            if (opcode == 1)
            {
                buffer.ReadInt16();
            }
            else if (opcode == 2)
            {
                definition.SetName(BufferUtil.ReadString(buffer));
            }
            else if (opcode == 3)
            {
                definition.SetDescription(BufferUtil.ReadString(buffer));
            }
            else if ((opcode >= 4 && opcode <= 8) || opcode == 10)
            {
                buffer.ReadInt16();
            }
            else if (opcode == 11)
            {
                definition.SetStackable(true);
            }
            else if (opcode == 12)
            {
                definition.SetValue(buffer.ReadInt32());
            }
            else if (opcode == 16)
            {
                definition.SetMembersOnly(true);
            }
            else if (opcode == 23)
            {
                buffer.ReadInt16();
                buffer.ReadByte();
            }
            else if (opcode == 24)
            {
                buffer.ReadInt16();
            }
            else if (opcode == 25)
            {
                buffer.ReadInt16();
                buffer.ReadByte();
            }
            else if (opcode == 26)
            {
                buffer.ReadInt16();
            }
            else if (opcode >= 30 && opcode < 35)
            {
                var str = BufferUtil.ReadString(buffer);
                if (str.Equals("hidden", StringComparison.OrdinalIgnoreCase)) str = null;

                definition.SetGroundAction(opcode - 30, str);
            }
            else if (opcode >= 35 && opcode < 40)
            {
                definition.SetInventoryAction(opcode - 35, BufferUtil.ReadString(buffer));
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
            else if (opcode == 78 || opcode == 79 || (opcode >= 90 && opcode <= 93) || opcode == 95)
            {
                buffer.ReadInt16();
            }
            else if (opcode == 97)
            {
                definition.SetNoteInfoId(buffer.ReadInt16() & 0xFFFF);
            }
            else if (opcode == 98)
            {
                definition.SetNoteGraphicId(buffer.ReadInt16() & 0xFFFF);
            }
            else if (opcode >= 100 && opcode < 110)
            {
                buffer.ReadInt16();
                buffer.ReadInt16();
            }
            else if (opcode >= 110 && opcode <= 112)
            {
                buffer.ReadInt16();
            }
            else if (opcode == 113 || opcode == 114)
            {
                buffer.ReadByte();
            }
            else if (opcode == 115)
            {
                definition.SetTeam(buffer.ReadByte() & 0xFF);
            }
        }
    }
}