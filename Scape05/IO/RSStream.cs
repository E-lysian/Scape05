using System.Text;
using Scape05.Misc;

namespace Scape05.IO;

public class RSStream
{
    /* Bits */
    private static readonly int[] BitMaskOut =
    {
        0, 1, 3, 7, 15, 31, 63, 127, 255, 511,
        1023, 2047, 4095, 8191, 16383, 32767,
        65535, 0x1ffff, 0x3ffff, 0x7ffff,
        0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
        0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff,
        0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff, -1
    };

    /* Frames */
    private static readonly int frameStackSize = 10;
    private readonly int[] frameStack = new int[frameStackSize];
    private int frameStackPtr = -1;

    public RSStream(byte[] buffer)
    {
        Buffer = buffer;
        CurrentOffset = 0;
    }

    public int BitPosition { get; set; }

    /* Buffer */
    public int CurrentOffset { get; set; }
    public byte[] Buffer { get; set; }

    public SessionEncryption packetEncryption { get; set; }

    public void CreateFrame(ServerOpCodes id)
    {
        try
        {
            Buffer[CurrentOffset++] = (byte)((byte)id + packetEncryption.GetNextKey());
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public void CreateFrameVarSize(ServerOpCodes id)
    {
        Buffer[CurrentOffset++] = (byte)((byte)id + packetEncryption.GetNextKey());
        Buffer[CurrentOffset++] = 0; // place holder for size byte
        if (frameStackPtr >= frameStackSize - 1)
            throw new StackOverflowException("Stack overflow");
        frameStack[++frameStackPtr] = CurrentOffset;
    }

    public void CreateFrameVarSizeWord(ServerOpCodes id)
    {
        // creates a variable sized
        // frame
        Buffer[CurrentOffset++] = (byte)((byte)id + packetEncryption.GetNextKey());
        WriteWord(0); // place holder for size word
        if (frameStackPtr >= frameStackSize - 1)
            throw new Exception("Stack overflow");
        frameStack[++frameStackPtr] = CurrentOffset;
    }

    public void EndFrameVarSize()
    {
        if (frameStackPtr < 0)
            throw new Exception("Stack empty");
        WriteFrameSize(CurrentOffset - frameStack[frameStackPtr--]);
    }

    public void EndFrameVarSizeWord()
    {
        if (frameStackPtr < 0)
            throw new Exception("Stack empty");
        WriteFrameSizeWord(CurrentOffset - frameStack[frameStackPtr--]);
    }

    public void WriteFrameSize(int i)
    {
        Buffer[CurrentOffset - i - 1] = (byte)i;
    }

    public void WriteFrameSizeWord(int i)
    {
        Buffer[CurrentOffset - i - 2] = (byte)(i >> 8);
        Buffer[CurrentOffset - i - 1] = (byte)i;
    }

    public void InitBitAccess()
    {
        BitPosition = CurrentOffset * 8;
    }

    public void FinishBitAccess()
    {
        CurrentOffset = (BitPosition + 7) / 8;
    }

    public byte[] ReadBytes(int l)
    {
        var bytes = new byte[l];

        for (var i = l; i < l; i++)
            bytes[i] = Buffer[CurrentOffset++];

        return bytes;
    }


    public void ReadBytes(byte[] bytes, int o, int l)
    {
        for (var i = l; i < l + o; i++)
            bytes[i] = Buffer[CurrentOffset++];
    }

    public void ReadBytesReverse(byte[] bytes, int o, int l)
    {
        for (var i = l + o - 1; i >= l; i--) bytes[i] = Buffer[CurrentOffset++];
    }

    public void ReadBytesReverseA(byte[] bytes, int o, int l)
    {
        for (var i = l + o - 1; i >= l; i--) bytes[i] = (byte)(Buffer[CurrentOffset++] - 128);
    }

    /// <summary>
    ///     Reads a unsigned 32 bit unit of data (0 through 4,294,967,295)
    /// </summary>
    public int ReadDWord()
    {
        CurrentOffset += 4;
        return ((Buffer[CurrentOffset - 4] & 0xff) << 24)
               + ((Buffer[CurrentOffset - 3] & 0xff) << 16)
               + ((Buffer[CurrentOffset - 2] & 0xff) << 8)
               + (Buffer[CurrentOffset - 1] & 0xff);
    }

    public int ReadDWordV1()
    {
        CurrentOffset += 4;
        return ((Buffer[CurrentOffset - 2] & 0xff) << 24)
               + ((Buffer[CurrentOffset - 1] & 0xff) << 16)
               + ((Buffer[CurrentOffset - 4] & 0xff) << 8)
               + (Buffer[CurrentOffset - 3] & 0xff);
    }

    public int ReadDWordV2()
    {
        CurrentOffset += 4;
        return ((Buffer[CurrentOffset - 3] & 0xff) << 24)
               + ((Buffer[CurrentOffset - 4] & 0xff) << 16)
               + ((Buffer[CurrentOffset - 1] & 0xff) << 8)
               + (Buffer[CurrentOffset - 2] & 0xff);
    }

    /// <summary>
    ///     Reads a 64 bit unsigned integer (0 - 18,446,744,073,709,551,615)
    /// </summary>
    public long ReadQWord()
    {
        var l = ReadDWord() & 0xffffffffL;
        var l1 = ReadDWord() & 0xffffffffL;
        return (l << 32) + l1;
    }

    /// <summary>
    ///     Java doesn't have unsigned bytes (0 to 255).
    ///     To make an unsigned byte, we can cast the byte into an int and mask (bitwise and) the new int
    ///     with a 0xff to get the last 8 bits or prevent sign extension.
    ///     byte aByte = -1; int number = aByte & 0xff; // bytes to unsigned byte in an integer
    /// </summary>
    public byte ReadSignedByte()
    {
        return Buffer[CurrentOffset++];
    }

    public byte ReadSignedByteA()
    {
        return (byte)(Buffer[CurrentOffset++] - 128);
    }

    public byte ReadSignedByteC()
    {
        return (byte)-Buffer[CurrentOffset++];
    }

    public byte ReadSignedByteS()
    {
        return (byte)(128 - Buffer[CurrentOffset++]);
    }

    public int ReadSignedWord()
    {
        CurrentOffset += 2;
        var i = ((Buffer[CurrentOffset - 2] & 0xff) << 8)
                + (Buffer[CurrentOffset - 1] & 0xff);
        if (i > 32767) i -= 0x10000;
        return i;
    }

    public int ReadSignedWordA()
    {
        CurrentOffset += 2;
        var i = ((Buffer[CurrentOffset - 2] & 0xff) << 8)
                + ((Buffer[CurrentOffset - 1] - 128) & 0xff);
        if (i > 32767) i -= 0x10000;
        return i;
    }

    public int ReadSignedWordBigEndian()
    {
        CurrentOffset += 2;
        var i = ((Buffer[CurrentOffset - 1] & 0xff) << 8)
                + (Buffer[CurrentOffset - 2] & 0xff);
        if (i > 32767) i -= 0x10000;
        return i;
    }

    public int ReadSignedWordBigEndianA()
    {
        CurrentOffset += 2;
        var i = ((Buffer[CurrentOffset - 1] & 0xff) << 8)
                + ((Buffer[CurrentOffset - 2] - 128) & 0xff);
        if (i > 32767)
            i -= 0x10000;
        return i;
    }

    public string ReadString()
    {
        var i = CurrentOffset;
        while (Buffer[CurrentOffset++] != 10) ;
        return Encoding.Default.GetString(Buffer, i, CurrentOffset - i - 1);
    }

    /// <summary>
    ///     Need to AND since Java doesn't have unsigned bytes.
    /// </summary>
    public byte ReadUnsignedByte()
    {
        return (byte)(Buffer[CurrentOffset++] & 0xff);
    }

    public byte ReadUnsignedByteA()
    {
        return (byte)((Buffer[CurrentOffset++] - 128) & 0xff);
    }

    public byte ReadUnsignedByteC()
    {
        return (byte)(-Buffer[CurrentOffset++] & 0xff);
    }

    public int ReadUnsignedByteS()
    {
        return (128 - Buffer[CurrentOffset++]) & 0xff;
    }

    public int ReadUnsignedWord()
    {
        CurrentOffset += 2;
        return ((Buffer[CurrentOffset - 2] & 0xff) << 8)
               + (Buffer[CurrentOffset - 1] & 0xff);
    }

    public int ReadUnsignedWordBigEndian()
    {
        CurrentOffset += 2;
        return ((Buffer[CurrentOffset - 1] & 0xff) << 8)
               + (Buffer[CurrentOffset - 2] & 0xff);
    }


    public int ReadUnsignedWordA()
    {
        CurrentOffset += 2;
        return ((Buffer[CurrentOffset - 2] & 0xff) << 8) + ((Buffer[CurrentOffset - 1] - 128) & 0xff);
    }

    public int ReadUnsignedWordBigEndianA()
    {
        CurrentOffset += 2;
        return ((Buffer[CurrentOffset - 1] & 0xff) << 8)
               + ((Buffer[CurrentOffset - 2] - 128) & 0xff);
    }


    /// <summary>
    ///     Reads a unsigned 32 bit unit of data (0 through 4,294,967,295)
    /// </summary>
    public int Read3Byte()
    {
        CurrentOffset += 3;
        return ((Buffer[CurrentOffset - 3] & 0xff) << 16)
               + ((Buffer[CurrentOffset - 2] & 0xff) << 8)
               + (Buffer[CurrentOffset - 1] & 0xff);
    }

    public void Write3Byte(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i >> 16);
        Buffer[CurrentOffset++] = (byte)(i >> 8);
        Buffer[CurrentOffset++] = (byte)i;
    }

    public void WriteBits(int numBits, int value)
    {
        var bytePos = BitPosition >> 3;
        var bitOffset = 8 - (BitPosition & 7);
        BitPosition += numBits;
        for (; numBits > bitOffset; bitOffset = 8)
        {
            Buffer[bytePos] &= (byte)~BitMaskOut[bitOffset];
            Buffer[bytePos++] |= (byte)((value >> (numBits - bitOffset)) & BitMaskOut[bitOffset]);
            numBits -= bitOffset;
        }

        if (numBits == bitOffset)
        {
            Buffer[bytePos] &= (byte)~BitMaskOut[bitOffset];
            Buffer[bytePos] |= (byte)(value & BitMaskOut[bitOffset]);
        }
        else
        {
            Buffer[bytePos] &= (byte)~(BitMaskOut[numBits] << (bitOffset - numBits));
            Buffer[bytePos] |= (byte)((value & BitMaskOut[numBits]) << (bitOffset - numBits));
        }
    }

    public void WriteByte(int i)
    {
        Buffer[CurrentOffset++] = (byte)i;
    }

    public void WriteByteA(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i + 128);
    }

    public void WriteByteC(int i)
    {
        Buffer[CurrentOffset++] = (byte)-i;
    }

    public void WriteBytes(byte[] bytes, int o, int l)
    {
        for (var i = l; i < l + o; i++)
            Buffer[CurrentOffset++] = bytes[i];
    }

    public void WriteByteS(int i)
    {
        Buffer[CurrentOffset++] = (byte)(128 - i);
    }


    public void WriteWord(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i >> 8);
        Buffer[CurrentOffset++] = (byte)i;
    }

    public void WriteBytesReverse(byte[] bytes, int o, int l)
    {
        for (var i = l + o - 1; i >= l; i--)
            Buffer[CurrentOffset++] = bytes[i];
    }

    public void WriteBytesReverseA(byte[] bytes, int o, int l)
    {
        for (var k = l + o - 1; k >= l; k--)
            Buffer[CurrentOffset++] = (byte)(bytes[k] + 128);
    }

    /// <summary>
    ///     Writes a unsigned 32 bit unit of data (0 through 4,294,967,295)
    /// </summary>
    public void WriteDWord(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i >> 24);
        Buffer[CurrentOffset++] = (byte)(i >> 16);
        Buffer[CurrentOffset++] = (byte)(i >> 8);
        Buffer[CurrentOffset++] = (byte)i;
    }

    public void WriteDWordV1(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i >> 8);
        Buffer[CurrentOffset++] = (byte)i;
        Buffer[CurrentOffset++] = (byte)(i >> 24);
        Buffer[CurrentOffset++] = (byte)(i >> 16);
    }

    public void WriteDWordV2(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i >> 16);
        Buffer[CurrentOffset++] = (byte)(i >> 24);
        Buffer[CurrentOffset++] = (byte)i;
        Buffer[CurrentOffset++] = (byte)(i >> 8);
    }

    public void WriteDWordBigEndian(int i)
    {
        Buffer[CurrentOffset++] = (byte)i;
        Buffer[CurrentOffset++] = (byte)(i >> 8);
        Buffer[CurrentOffset++] = (byte)(i >> 16);
        Buffer[CurrentOffset++] = (byte)(i >> 24);
    }

    public void WriteQWord(long l)
    {
        Buffer[CurrentOffset++] = (byte)(int)(l >> 56);
        Buffer[CurrentOffset++] = (byte)(int)(l >> 48);
        Buffer[CurrentOffset++] = (byte)(int)(l >> 40);
        Buffer[CurrentOffset++] = (byte)(int)(l >> 32);
        Buffer[CurrentOffset++] = (byte)(int)(l >> 24);
        Buffer[CurrentOffset++] = (byte)(int)(l >> 16);
        Buffer[CurrentOffset++] = (byte)(int)(l >> 8);
        Buffer[CurrentOffset++] = (byte)(int)l;
    }

    public void WriteString(string msg)
    {
        var msgBytes = Encoding.Default.GetBytes(msg);
        System.Buffer.BlockCopy(msgBytes, 0, Buffer, CurrentOffset, msgBytes.Length);
        CurrentOffset += msgBytes.Length;
        /* Append 10 to the end which is the string terminator */
        Buffer[CurrentOffset++] = 10;
    }

    public void WriteWordA(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i >> 8);
        Buffer[CurrentOffset++] = (byte)(i + 128);
    }

    public void WriteWordBigEndian(int i)
    {
        Buffer[CurrentOffset++] = (byte)i;
        Buffer[CurrentOffset++] = (byte)(i >> 8);
    }

    public void WriteWordBigEndian_dup(int i)
    {
        Buffer[CurrentOffset++] = (byte)i;
        Buffer[CurrentOffset++] = (byte)(i >> 8);
    }

    public void WriteWordBigEndianA(int i)
    {
        Buffer[CurrentOffset++] = (byte)(i + 128);
        Buffer[CurrentOffset++] = (byte)(i >> 8);
    }
}