namespace Scape05.Data;

public class IndexedFileSystem : IDisposable
{
    private static FileStream _data;
    private readonly Dictionary<FileDescriptor, Archive> _cache = new(FileSystemConstants.ArchiveCount);
    private readonly FileStream[] _indices = new FileStream[256];
    private readonly bool _readOnly;
    private int[] _crcs;
    private byte[] _crcTable;

    public IndexedFileSystem(string basePath, bool readOnly)
    {
        _readOnly = readOnly;
        DetectLayout(basePath);
    }

    public void Dispose()
    {
        _data?.Close();

        foreach (var index in _indices)
        {
            index?.Close();
        }
    }

    public Archive GetArchive(int type, int file)
    {
        var descriptor = new FileDescriptor(type, file);
        if (_cache.TryGetValue(descriptor, out var cached))
        {
            return cached;
        }

        cached = Archive.Decode(GetFile(descriptor));
        lock (_cache)
        {
            _cache.Add(descriptor, cached);
        }

        return cached;
    }

    public MemoryStream GetFile(FileDescriptor descriptor)
    {
        var index = GetIndex(descriptor);
        var buffer = new byte[index.GetSize()];
        var position = index.GetBlock() * FileSystemConstants.BlockSize;
        var read = 0;
        var size = index.GetSize();
        var blocks = size / FileSystemConstants.ChunkSize;
        if (size % FileSystemConstants.ChunkSize != 0)
        {
            blocks++;
        }

        for (var i = 0; i < blocks; i++)
        {
            var header = new byte[FileSystemConstants.HeaderSize];
            lock (_data)
            {
                _data.Seek(position, SeekOrigin.Begin);
                _data.Read(header, 0, FileSystemConstants.HeaderSize);
            }

            position += FileSystemConstants.HeaderSize;

            var nextFile = ((header[0] & 0xFF) << 8) | (header[1] & 0xFF);
            var curChunk = ((header[2] & 0xFF) << 8) | (header[3] & 0xFF);
            var nextBlock = ((header[4] & 0xFF) << 16) | ((header[5] & 0xFF) << 8) | (header[6] & 0xFF);
            var nextType = header[7] & 0xFF;

            if (i != curChunk)
            {
                throw new InvalidOperationException("Chunk id mismatch.");
            }

            var chunkSize = size - read;
            if (chunkSize > FileSystemConstants.ChunkSize)
            {
                chunkSize = FileSystemConstants.ChunkSize;
            }

            var chunk = new byte[chunkSize];
            lock (_data)
            {
                _data.Seek(position, SeekOrigin.Begin);
                _data.Read(chunk, 0, chunkSize);
            }

            Array.Copy(chunk, 0, buffer, read, chunkSize);

            read += chunkSize;
            position = nextBlock * FileSystemConstants.BlockSize;

            // if we still have more data to read, check the validity of the header
            if (size > read)
            {
                if (nextType != descriptor.GetType() + 1)
                {
                    throw new InvalidOperationException("File type mismatch.");
                }

                if (nextFile != descriptor.GetFile())
                {
                    throw new InvalidOperationException("File id mismatch.");
                }
            }
        }

        return new MemoryStream(buffer);
    }

    public MemoryStream GetFile(int type, int file)
    {
        return GetFile(new FileDescriptor(type, file));
    }

    private void DetectLayout(string baseDirectory)
    {
        var indexCount = 0;
        for (var i = 0; i < _indices.Length; i++)
        {
            var indexFilePath = Path.Combine(baseDirectory, $"main_file_cache.idx{i}");
            if (File.Exists(indexFilePath) && !Directory.Exists(indexFilePath))
            {
                indexCount++;
                _indices[i] = new FileStream(indexFilePath, _readOnly ? FileMode.Open : FileMode.OpenOrCreate,
                    FileAccess.ReadWrite);
            }
        }

        if (indexCount <= 0)
        {
            throw new FileNotFoundException($"No index file(s) present in {baseDirectory}.");
        }

        var resourcesPath = Path.Combine(baseDirectory, "main_file_cache.dat");
        if (File.Exists(resourcesPath) && !Directory.Exists(resourcesPath))
        {
            _data = new FileStream(resourcesPath, _readOnly ? FileMode.Open : FileMode.OpenOrCreate,
                FileAccess.ReadWrite);
        }
        else
        {
            throw new FileNotFoundException("No data file present.");
        }
    }

    private int GetFileCount(int type)
    {
        if (type < 0 || type >= _indices.Length)
        {
            throw new IndexOutOfRangeException("File type out of bounds.");
        }

        var indexFile = _indices[type];
        lock (indexFile)
        {
            return (int)(indexFile.Length / FileSystemConstants.IndexSize);
        }
    }

    private Index GetIndex(FileDescriptor descriptor)
    {
        var index = descriptor.GetType();
        if (index < 0 || index >= _indices.Length)
        {
            throw new IndexOutOfRangeException("File descriptor type out of bounds.");
        }

        var buffer = new byte[FileSystemConstants.IndexSize];
        var indexFile = _indices[index];
        lock (indexFile)
        {
            long position = descriptor.GetFile() * FileSystemConstants.IndexSize;
            if (position >= 0 && indexFile.Length >= position + FileSystemConstants.IndexSize)
            {
                indexFile.Seek(position, SeekOrigin.Begin);
                indexFile.Read(buffer, 0, buffer.Length);
            }
            else
            {
                throw new FileNotFoundException("Could not find index.");
            }
        }

        return Index.Decode(buffer);
    }
}