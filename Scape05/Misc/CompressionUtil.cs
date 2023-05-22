using System.IO.Compression;
using ICSharpCode.SharpZipLib.BZip2;

namespace Scape05.Misc;

/// <summary>
///     A utility class for performing compression/decompression.
/// </summary>
public static class CompressionUtil
{
    /// <summary>
    ///     Bzip2s the specified array, removing the header.
    /// </summary>
    /// <param name="uncompressed">The uncompressed array.</param>
    /// <returns>The compressed array.</returns>
    /// <exception cref="IOException">If there is an error compressing the array.</exception>
    public static byte[] Bzip2(byte[] uncompressed)
    {
        using (var memStream = new MemoryStream())
        {
            using (var os = new BZip2OutputStream(memStream))
            {
                os.Write(uncompressed, 0, uncompressed.Length);
            }

            var compressed = memStream.ToArray();
            var newCompressed = new byte[compressed.Length - 4]; // Strip the header
            Array.Copy(compressed, 4, newCompressed, 0, newCompressed.Length);
            return newCompressed;
        }
    }

    /// <summary>
    ///     Debzip2s the compressed array and places the result into the decompressed array.
    /// </summary>
    /// <param name="compressed">The compressed array, <strong>without</strong> the header.</param>
    /// <param name="decompressed">The decompressed array.</param>
    /// <exception cref="IOException">If there is an error decompressing the array.</exception>
    public static void Debzip2(byte[] compressed, byte[] decompressed)
    {
        var newCompressed = new byte[compressed.Length + 4];
        newCompressed[0] = (byte)'B';
        newCompressed[1] = (byte)'Z';
        newCompressed[2] = (byte)'h';
        newCompressed[3] = (byte)'1';
        Array.Copy(compressed, 0, newCompressed, 4, compressed.Length);

        using (var memStream = new MemoryStream(newCompressed))
        {
            using (var isStream = new BZip2InputStream(memStream))
            {
                isStream.Read(decompressed, 0, decompressed.Length);
            }
        }
    }

    /// <summary>
    ///     Degzips the compressed array and places the results into the decompressed array.
    /// </summary>
    /// <param name="compressed">The compressed array.</param>
    /// <param name="decompressed">The decompressed array.</param>
    /// <exception cref="IOException">If an I/O error occurs.</exception>
    public static void Degzip(byte[] compressed, byte[] decompressed)
    {
        using (var memStream = new MemoryStream(compressed))
        {
            using (var isStream = new GZipStream(memStream, CompressionMode.Decompress))
            {
                isStream.Read(decompressed, 0, decompressed.Length);
            }
        }
    }

    public static byte[] Degzip(byte[] compressed)
    {
        using (var ms = new MemoryStream(compressed))
        using (var gzs = new GZipStream(ms, CompressionMode.Decompress))
        using (var result = new MemoryStream())
        {
            var buffer = new byte[1024];
            int read;
            while ((read = gzs.Read(buffer, 0, buffer.Length)) > 0) result.Write(buffer, 0, read);

            return result.ToArray();
        }
    }
}