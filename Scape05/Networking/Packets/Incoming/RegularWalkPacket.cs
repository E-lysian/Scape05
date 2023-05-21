namespace Scape05.Entities.Packets.Implementation;

public class RegularWalkPacket : IPacket
{
    public int OpCode { get; set; } = 164;
    public void Build(Client client)
    {
        throw new NotImplementedException();
    }
}