using System.Text;

namespace Scape05.Misc;

public class ByteStreamExt
{
    public byte[] Buffer;
    public int CurrentOffset;

    public ByteStreamExt(byte[] buffer)
    {
        Buffer = buffer;
        CurrentOffset = 0;
    }

    public void Skip(int length)
    {
        CurrentOffset += length;
    }

    public int ReadUnsignedByte()
    {
        return Buffer[CurrentOffset++] & 0xff;
    }

    public sbyte ReadSignedByte()
    {
        return (sbyte)Buffer[CurrentOffset++];
    }

    public int ReadUnsignedWord()
    {
        CurrentOffset += 2;
        return ((Buffer[CurrentOffset - 2] & 0xff) << 8) + (Buffer[CurrentOffset - 1] & 0xff);
    }

    public int GetUSmart()
    {
        var i = Buffer[CurrentOffset] & 0xff;
        if (i < 128)
            return ReadUnsignedByte();
        return ReadUnsignedWord() - 32768;
    }

    public int ReadSignedWord()
    {
        CurrentOffset += 2;
        var i = ((Buffer[CurrentOffset - 2] & 0xff) << 8) + (Buffer[CurrentOffset - 1] & 0xff);
        if (i > 32767) i -= 0x10000;

        return i;
    }

    public int Read3Bytes()
    {
        CurrentOffset += 3;
        return ((Buffer[CurrentOffset - 3] & 0xff) << 16) + ((Buffer[CurrentOffset - 2] & 0xff) << 8) +
               (Buffer[CurrentOffset - 1] & 0xff);
    }

    public int ReadR3Bytes()
    {
        CurrentOffset += 3;
        return ((Buffer[CurrentOffset - 1] & 0xff) << 16) + ((Buffer[CurrentOffset - 2] & 0xff) << 8) +
               (Buffer[CurrentOffset - 3] & 0xff);
    }

    public int ReadDWord()
    {
        CurrentOffset += 4;
        return ((Buffer[CurrentOffset - 4] & 0xff) << 24) + ((Buffer[CurrentOffset - 3] & 0xff) << 16) +
               ((Buffer[CurrentOffset - 2] & 0xff) << 8) + (Buffer[CurrentOffset - 1] & 0xff);
    }

    public long ReadQWord()
    {
        var l = ReadDWord() & 0xffffffffL;
        var l1 = ReadDWord() & 0xffffffffL;
        return (l << 32) + l1;
    }

    public string ReadString()
    {
        var i = CurrentOffset;
        while (Buffer[CurrentOffset++] != 10)
            ;
        return Encoding.ASCII.GetString(Buffer, i, CurrentOffset - i - 1);
    }

    public string ReadNewString()
    {
        var i = CurrentOffset;
        while (Buffer[CurrentOffset++] != 0)
            ;
        return Encoding.ASCII.GetString(Buffer, i, CurrentOffset - i - 1);
    }

    public byte[] ReadBytes()
    {
        var i = CurrentOffset;
        while (Buffer[CurrentOffset++] != 10)
        {
        }

        var abyte0 = new byte[CurrentOffset - i - 1];
        Array.Copy(Buffer, i, abyte0, 0, CurrentOffset - 1 - i);
        return abyte0;
    }

    public void ReadBytes(int i, int j, byte[] abyte0)
    {
        for (var l = j; l < j + i; l++)
            abyte0[l] = Buffer[CurrentOffset++];
    }
}