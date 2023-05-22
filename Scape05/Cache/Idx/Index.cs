namespace Scape05.Data;

/// <summary>
/// Represents an index pointing to a file in the main_file_cache.dat file.
/// </summary>
public class Index
{
    private readonly int block; // The first block of the file.
    private readonly int size; // The size of the file.

    /// <summary>
    /// Creates a new instance of the Index class.
    /// </summary>
    /// <param name="size">The size of the file.</param>
    /// <param name="block">The first block of the file.</param>
    public Index(int size, int block)
    {
        this.size = size;
        this.block = block;
    }

    /// <summary>
    /// Decodes a buffer into an Index object.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>The decoded Index object.</returns>
    /// <exception cref="ArgumentException">Thrown when the buffer length is invalid.</exception>
    public static Index Decode(byte[] buffer)
    {
        if (buffer.Length != FileSystemConstants.IndexSize)
        {
            throw new ArgumentException("Incorrect buffer length.");
        }

        var size = ((buffer[0] & 0xFF) << 16) | ((buffer[1] & 0xFF) << 8) | (buffer[2] & 0xFF);
        var block = ((buffer[3] & 0xFF) << 16) | ((buffer[4] & 0xFF) << 8) | (buffer[5] & 0xFF);

        return new Index(size, block);
    }

    /// <summary>
    /// Gets the first block of the file.
    /// </summary>
    /// <returns>The first block of the file.</returns>
    public int GetBlock() => block;

    /// <summary>
    /// Gets the size of the file.
    /// </summary>
    /// <returns>The size of the file.</returns>
    public int GetSize() => size;
}