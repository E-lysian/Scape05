namespace Scape05.Data;

/// <summary>
/// Represents a single entry in an <see cref="Archive"/>.
/// </summary>
public sealed class ArchiveEntry
{
    private readonly byte[] buffer;
    private readonly int identifier;

    /// <summary>
    /// Creates a new archive entry.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <param name="buffer">The buffer.</param>
    public ArchiveEntry(int identifier, byte[] buffer)
    {
        this.identifier = identifier;
        this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
    }

    /// <summary>
    /// Gets the buffer of this entry.
    /// </summary>
    /// <returns>The buffer of this entry.</returns>
    public byte[] GetBuffer()
    {
        return (byte[])buffer.Clone();
    }

    /// <summary>
    /// Gets the identifier of this entry.
    /// </summary>
    /// <returns>The identifier of this entry.</returns>
    public int GetIdentifier()
    {
        return identifier;
    }
}