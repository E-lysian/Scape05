namespace Scape05.Data;

public static class FileSystemConstants
{
    /// <summary>
    ///     The number of archives in cache 0.
    /// </summary>
    public const int ArchiveCount = 9;

    /// <summary>
    ///     The size of a chunk.
    /// </summary>
    public const int ChunkSize = 512;

    /// <summary>
    ///     The size of a header.
    /// </summary>
    public const int HeaderSize = 8;

    /// <summary>
    ///     The size of a block.
    /// </summary>
    public const int BlockSize = HeaderSize + ChunkSize;

    /// <summary>
    ///     The size of an index.
    /// </summary>
    public const int IndexSize = 6;
}