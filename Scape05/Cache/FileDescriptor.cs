namespace Scape05.Data;

public class FileDescriptor
{
    private readonly int _file;
    private readonly int _type;

    public FileDescriptor(int type, int file)
    {
        _type = type;
        _file = file;
    }

    public override bool Equals(object obj)
    {
        if (obj is FileDescriptor other) return _type == other._type && _file == other._file;

        return false;
    }

    public int GetFile()
    {
        return _file;
    }

    public int GetType()
    {
        return _type;
    }

    public override int GetHashCode()
    {
        return _file * FileSystemConstants.ArchiveCount + _type;
    }
}