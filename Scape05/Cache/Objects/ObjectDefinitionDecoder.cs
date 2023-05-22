using Scape05.Misc;

namespace Scape05.Data.ObjectsDef;

public sealed class ObjectDefinitionDecoder
    {
        private readonly IndexedFileSystem fs;

        public ObjectDefinitionDecoder(IndexedFileSystem fs)
        {
            this.fs = fs ?? throw new ArgumentNullException(nameof(fs));
        }

        public void Run()
        {
            try
            {
                var config = fs.GetArchive(0, 2);
                var data = config.GetEntry("loc.dat").GetBuffer();
                var idx = config.GetEntry("loc.idx").GetBuffer();

                using (var msData = new MemoryStream(data))
                using (var msIdx = new MemoryStream(idx))
                using (var dataReader = new BinaryReader(msData))
                using (var idxReader = new BinaryReader(msIdx))
                {
                    int count = idxReader.ReadInt16BigEndian();
                    int index = 2;
                    var indices = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        indices[i] = index;
                        index += idxReader.ReadInt16BigEndian();
                    }

                    var definitions = new ObjectDefinition[count];
                    for (int i = 0; i < count; i++)
                    {
                        dataReader.BaseStream.Position = indices[i];
                        definitions[i] = Decode(i, dataReader);
                    }

                    ObjectDefinition.Init(definitions);
                }
            }
            catch (IOException e)
            {
                throw new Exception("Error decoding ObjectDefinitions.", e);
            }
        }

        private ObjectDefinition Decode(int id, BinaryReader data)
        {
            bool actions = false;
            int[] modelId = null;
            int[] modelType = null;

            var definition = new ObjectDefinition(id);

            while (true)
            {
                int opcode = data.ReadByte();

                if (opcode == 0)
                {
                    definition.IsInteractive = modelId != null && (modelType == null || modelType[0] == 10);
                    if (actions) definition.IsInteractive = true;

                    return definition;
                }

                switch (opcode)
                {
                    case 1:
                        int amount = data.ReadByte();
                        if (amount > 0)
                        {
                            if (modelId == null)
                            {
                                modelType = new int[amount];
                                modelId = new int[amount];
                                for (int i = 0; i < amount; i++)
                                {
                                    modelId[i] = data.ReadInt16BigEndian();
                                    modelType[i] = data.ReadByte();
                                }
                            }
                            else
                            {
                                for (int i = 0; i < amount; i++)
                                {
                                    data.ReadInt16BigEndian();
                                    data.ReadByte();
                                }
                            }
                        }
                        break;
                    case 2:
                        definition.Name = BufferUtil.ReadString(data);
                        break;
                    case 3:
                        definition.Description = BufferUtil.ReadString(data);
                        break;
                    case 5:
                        int amount2 = data.ReadByte();
                        if (amount2 > 0)
                        {
                            if (modelId == null)
                            {
                                modelType = null;
                                modelId = new int[amount2];
                                for (int i = 0; i < amount2; i++) modelId[i] = data.ReadInt16BigEndian();
                            }
                            else
                            {
                                for (int i = 0; i < amount2; i++) data.ReadInt16BigEndian();
                            }
                        }
                        break;
                    case 14:
                        definition.Width = data.ReadByte();
                        break;
                    case 15:
                        definition.Length = data.ReadByte();
                        break;
                    case 17:
                        definition.IsSolid = false;
                        break;
                    case 18:
                        definition.IsImpenetrable = false;
                        break;
                    case 19:
                        definition.IsInteractive = data.ReadByte() == 1;
                        break;
                    case 23:
                        definition.Wall = true;
                        break;
                    case 24:
                        data.ReadInt16BigEndian();
                        break;
                    case 28:
                    case 29:
                        data.ReadByte();
                        break;
                    case >= 30 and < 39:
                        actions = true;
                        string[] strings = definition.MenuActions;
                        if (strings == null) strings = new string[10];
                        string action = BufferUtil.ReadString(data);
                        strings[opcode - 30] = action;
                        definition.MenuActions = strings;
                        definition.IsInteractive = true;
                        break;
                    case 39:
                        data.ReadByte();
                        break;
                    case 40:
                        int amount3 = data.ReadByte();
                        for (int i = 0; i < amount3; i++)
                        {
                            data.ReadInt16BigEndian();
                            data.ReadInt16BigEndian();
                        }
                        break;
                    case 64:
                        definition.IsClipped = false;
                        break;
                    case 60:
                    case >= 65 and <= 68:
                        data.ReadInt16BigEndian();
                        break;
                    case 69:
                        definition.Face = data.ReadByte();
                        break;
                    case >= 70 and <= 72:
                        data.ReadInt16BigEndian();
                        break;
                    case 73:
                        definition.IsObstructive = true;
                        break;
                    case 75:
                        data.ReadByte();
                        break;
                    case 77:
                        data.ReadInt16BigEndian();
                        data.ReadInt16BigEndian();
                        int count2 = data.ReadByte();
                        for (int i = 0; i <= count2; i++) data.ReadInt16BigEndian();
                        break;
                    default:
                        // Console.WriteLine("Unknown object config: " + opcode);
                        break;
                }
            }
        }
    }