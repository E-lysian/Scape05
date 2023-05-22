using Scape05.Misc;

namespace Scape05.Data;

public sealed class Archive
{
    private readonly ArchiveEntry[] entries;

    public Archive(ArchiveEntry[] entries)
    {
        this.entries = entries ?? throw new ArgumentNullException(nameof(entries));
    }

    public static Archive Decode(MemoryStream buffer)
    {
        var extractedSize = BufferUtil.ReadUnsignedMedium(buffer);
        var size = BufferUtil.ReadUnsignedMedium(buffer);
        var extracted = false;

        if (size != extractedSize)
        {
            var compressed = new byte[size];
            var decompressed = new byte[extractedSize];
            buffer.Read(compressed, 0, size);
            CompressionUtil.Debzip2(compressed, decompressed);
            buffer = new MemoryStream(decompressed);
            extracted = true;
        }

        int entryCount = buffer.ReadInt16BE();
        var identifiers = new int[entryCount];
        var extractedSizes = new int[entryCount];
        var sizes = new int[entryCount];

        for (int i = 0; i < entryCount; i++)
        {
            identifiers[i] = buffer.ReadInt32BE();
            extractedSizes[i] = BufferUtil.ReadUnsignedMedium(buffer);
            sizes[i] = BufferUtil.ReadUnsignedMedium(buffer);
        }

        var entries = new ArchiveEntry[entryCount];
        for (int entry = 0; entry < entryCount; entry++)
        {
            MemoryStream entryBuffer;
            if (!extracted)
            {
                var compressed = new byte[sizes[entry]];
                var decompressed = new byte[extractedSizes[entry]];
                buffer.Read(compressed, 0, sizes[entry]);
                CompressionUtil.Debzip2(compressed, decompressed);
                entryBuffer = new MemoryStream(decompressed);
            }
            else
            {
                var buf = new byte[extractedSizes[entry]];
                buffer.Read(buf, 0, extractedSizes[entry]);
                entryBuffer = new MemoryStream(buf);
            }

            entries[entry] = new ArchiveEntry(identifiers[entry], entryBuffer.ToArray());
        }

        return new Archive(entries);
    }

    public ArchiveEntry GetEntry(string name)
    {
        int hash = Hash(name);

        foreach (var entry in entries)
        {
            if (entry.GetIdentifier() == hash)
            {
                return entry;
            }
        }

        throw new FileNotFoundException("Could not find entry: " + name + ".");
    }

    public static int Hash(string name)
    {
        return name.ToUpper().ToCharArray().Aggregate(0, (hash, character) => hash * 61 + character - 32);
    }
}