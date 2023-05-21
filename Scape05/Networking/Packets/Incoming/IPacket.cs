namespace Scape05.Entities.Packets;

public interface IPacket
{
    public int OpCode { get; set; }
    void Build(Client client);
}