using System.Net.Sockets;
using Scape05.IO;

namespace Scape05.Entities;

public class Client
{
    public int Index { get; set; }
    public RSStream Reader { get; set; }
    public RSStream Writer { get; set; }
    public TcpClient Socket { get; set; }
    public NetworkStream NetworkStream { get; set; }
    public LinkedList<Player> LocalPlayers { get; set; } = new();
    public SessionEncryption InEncryption { get; set; }
    public SessionEncryption OutEncryption { get; set; }

    public void FillStream(int count)
    {
        Reader.CurrentOffset = 0;
        NetworkStream.Read(Reader.Buffer, 0, count);
    }

    public void DirectFlushStream()
    {
        NetworkStream.Write(Writer.Buffer, 0, Writer.CurrentOffset);
        Writer.CurrentOffset = 0; /* Reset */
        NetworkStream.Flush();
    }
}