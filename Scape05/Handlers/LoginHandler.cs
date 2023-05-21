using Scape05.Entities;
using Scape05.IO;

namespace Scape05.Handlers;

public class LoginHandler
{
    public static Player Invoke(Player client)
    {
        var serverSessionKey = SessionEncryption.GenerateServerSessionKey();

        client.FillStream(2);
        var connectionType = client.Reader.ReadUnsignedByte();
        Console.WriteLine($"ConnectionType: {connectionType}");
        var userHash = client.Reader.ReadUnsignedByte();

        for (var i = 0; i < 8; i++)
            client.Writer.WriteByte(0);

        client.Writer.WriteByte(0); //responseCode 0 -  	Exchanges session keys, player name, password, etc. 
        client.Writer.WriteQWord(serverSessionKey);
        client.DirectFlushStream(); /* Send */

        client.FillStream(2);
        var connectionStatus = client.Reader.ReadUnsignedByte();
        var loginPacketSize = client.Reader.ReadUnsignedByte();
        client.FillStream(loginPacketSize);

        var loginEncryptPacketSize = loginPacketSize - (36 + 1 + 1 + 2);
        var magicNumber = client.Reader.ReadUnsignedByte();
        var revision = client.Reader.ReadSignedWord();
        var clientVersion = client.Reader.ReadUnsignedByte();

        var crcValues = new int[9];
        for (var i = 0; i < crcValues.Length; i++)
            crcValues[i] = client.Reader.ReadDWord();

        var size2 = client.Reader.ReadUnsignedByte();
        var magicNumber2 = client.Reader.ReadUnsignedByte();

        var ISAACSeed = new int[4];
        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] = client.Reader.ReadDWord();
        client.InEncryption = new SessionEncryption(ISAACSeed);

        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] += 50;
        client.OutEncryption = new SessionEncryption(ISAACSeed);
        client.Writer.packetEncryption = client.OutEncryption;

        var UID = client.Reader.ReadDWord();
        client.Name = client.Reader.ReadString();
        client.Password = client.Reader.ReadString();

        client.Writer.WriteByte(2);
        client.Writer.WriteByte(2);
        client.Writer.WriteByte(0);
        client.DirectFlushStream();

        return client;
    }
}