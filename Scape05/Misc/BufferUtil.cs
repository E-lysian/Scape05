using System.Text;

namespace Scape05.Misc;

/**
 * A utility class which contains byte buffer related utility methods.
 */
public static class BufferUtil
{
    public const int StringTerminator = 10;

    public static int ReadSmart(MemoryStream stream)
    {
        var peek = stream.ReadByte();
        if (peek >= 128)
            return (short)(((peek << 8) & 0xFF00) | stream.ReadByte());
        return peek;
    }

    public static string ReadString(BinaryReader reader)
    {
        var builder = new StringBuilder();
        char character;
        while ((character = (char)reader.ReadByte()) != StringTerminator) builder.Append(character);
        return builder.ToString();
    }


    public static int ReadUnsignedMedium(MemoryStream stream)
    {
        return (stream.ReadByte() << 16) | (stream.ReadByte() << 8) | stream.ReadByte();
    }

    public static short ReadInt16BE(this MemoryStream stream)
    {
        var bytes = new byte[2];
        stream.Read(bytes, 0, 2);
        return (short)((bytes[0] << 8) | bytes[1]);
    }

    public static int ReadInt32BE(this MemoryStream stream)
    {
        var bytes = new byte[4];
        stream.Read(bytes, 0, 4);
        return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
    }

    public static short ReadInt16BigEndian(this BinaryReader binaryReader)
    {
        var bytes = new byte[2];
        binaryReader.Read(bytes, 0, 2);
        var value = (short)((bytes[0] << 8) | bytes[1]);
        return value;
    }

    public static void Skip(this MemoryStream ms, int count)
    {
        ms.Seek(count, SeekOrigin.Current);
    }

    public static int GetUByte(this MemoryStream stream)
    {
        return stream.ReadByte() & 0xFF;
    }

    public static int GetUShort(this MemoryStream stream)
    {
        var b1 = stream.ReadByte();
        var b2 = stream.ReadByte();
        return (b1 << 8) + b2;
    }

    public static int GetUSmart(this MemoryStream stream)
    {
        var i = stream.ReadByte();
        if (i < 128) return i;

        stream.Position--;
        return stream.GetUShort() - 32768;
    }
}