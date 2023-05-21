namespace Scape05.Entities;

public class BuildArea
{
    private readonly IEntity _entity;

    public BuildArea(IEntity entity)
    {
        _entity = entity;
        InitializeBuildArea();
    }

    public int CenterChunkX { get; private set; }
    public int CenterChunkY { get; private set; }
    public int RegionId { get; private set; }
    public int OffsetChunkX { get; private set; }
    public int OffsetChunkY { get; private set; }
    public int BuildAreaStartX { get; private set; }
    public int BuildAreaStartY { get; private set; }

    private void InitializeBuildArea()
    {
        CenterChunkX = _entity.Location.X >> 3;
        CenterChunkY = _entity.Location.Y >> 3;
        RegionId = (((_entity.Location.X >> 6) << 8) & 0xFF00) | ((_entity.Location.Y >> 6) & 0xFF);
        OffsetChunkX = CenterChunkX - 6;
        OffsetChunkY = CenterChunkY - 6;
        BuildAreaStartX = OffsetChunkX << 3;
        BuildAreaStartY = OffsetChunkY << 3;
    }

    public int GetPositionRelativeToOffsetChunkX()
    {
        return _entity.Location.X - OffsetChunkX * 8;
    }

    public int GetPositionRelativeToOffsetChunkY()
    {
        return _entity.Location.Y - OffsetChunkY * 8;
    }

    public int GetCenterChunkX()
    {
        return CenterChunkX;
    }

    public int GetCenterChunkY()
    {
        return CenterChunkY;
    }
}
