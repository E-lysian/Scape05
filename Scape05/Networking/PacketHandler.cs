using Scape05.Entities.Packets;
using Scape05.Entities.Packets.Implementation;
using Scape05.IO;
using Scape05.Misc;
using Scape05.Networking.Packets.Incoming;

namespace Scape05.Entities;

public class PacketHandler
{
    private readonly Player _client;
    private readonly int _maxQueuedPackets = 10;
    private List<IPacket> _packets;
    private readonly Queue<IPacket> _queuedPackets;
    private int _opCode = -1;

    public PacketHandler(Player client)
    {
        _client = client;
        InstantiatePackets();
        _queuedPackets = new Queue<IPacket>();
    }

    public int PacketLength { get; set; } = -1;

    private void InstantiatePackets()
    {
        _packets = new List<IPacket>
        {
            new RegularWalkPacket(),
            new PlayerCommandPacket(),
            new AttackNPCPacket()
        };
    }

    public void Fetch()
    {
        if (!_client.NetworkStream.DataAvailable)
            return;

        FillStream(1);

        if (_opCode == -1)
            _opCode = (byte)(GetReader().ReadSignedByte() - GetInEncryptionKey());

        if (PacketLength == -1)
        {
            PacketLength = GameConstants.INCOMING_SIZES[_opCode];

            if (PacketLength == 0)
            {
                _opCode = -1;
                PacketLength = -1;
                return;
            }

            if (PacketLength == -1)
            {
                FillStream(1);
                PacketLength = GetReader().ReadSignedByte();
            }

            FillStream(PacketLength);
            Console.WriteLine($"+ [{DateTime.Now}] [{_opCode}] Packet Received - Length: {PacketLength}");
        }

        if (_opCode != -1 && _queuedPackets.Count < _maxQueuedPackets)
        {
            EnqueuePacket();
        }

        _opCode = -1;
        PacketLength = -1;
    }

    private void EnqueuePacket()
    {
        var packet = _packets.FirstOrDefault(x => x.OpCode == _opCode);

        if (packet != null)
        {
            var currentCount = _queuedPackets.Count(x => x.GetType() == packet.GetType());

            if (currentCount >= _maxQueuedPackets)
                return;

            var packetInstance = Activator.CreateInstance(packet.GetType()) as IPacket;
            packetInstance?.Build(_client);
            _queuedPackets.Enqueue(packetInstance);
            Console.WriteLine($"ClientId: {_client.Index} - Added {packet.GetType()}!");
        }
    }

    private RSStream GetReader()
    {
        return _client.Reader;
    }

    private int GetInEncryptionKey()
    {
        return _client.InEncryption.GetNextKey();
    }

    private void FillStream(int i)
    {
        _client.FillStream(i);
    }

    public void Build()
    {
        if (_queuedPackets.TryDequeue(out var packet))
        {
            Console.WriteLine($"Dequeued: {packet.OpCode} {nameof(packet)}");
        }
    }
}