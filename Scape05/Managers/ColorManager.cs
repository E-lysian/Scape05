namespace Scape05.Managers;

public class ColorManager
{
    private byte[] Colors;

    public ColorManager()
    {
        Colors = new byte[5];
    }

    public void SetColor(int index, byte value)
    {
        if (index < 0 || index >= Colors.Length)
            throw new IndexOutOfRangeException("Invalid color index.");

        Colors[index] = value;
    }

    public byte GetColor(int index)
    {
        if (index < 0 || index >= Colors.Length)
            throw new IndexOutOfRangeException("Invalid color index.");

        return Colors[index];
    }
}