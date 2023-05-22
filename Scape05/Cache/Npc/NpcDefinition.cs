namespace Scape05.Data.Npc;

public class NpcDefinition
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Invisible { get; set; }
    public byte Size { get; set; }
    public short StandAnimation { get; set; }
    public short WalkAnimationIndex { get; set; }
    public short WalkAnimationIndex1 { get; set; }
    public short Turn180AnimationIndex { get; set; }
    public short Turn90CWAnimationIndex { get; set; }
    public short Turn90CCWAnimationIndex { get; set; }
    public bool DrawMinimapDot { get; set; }
    public int CombatLevel { get; set; }
    public short HeadIcon { get; set; }
    public byte ShadowModifier { get; set; }
    public byte LightModifier { get; set; }
    public short ScaleY { get; set; }
    public short ScaleXZ { get; set; }
    public short DegreesToTurn { get; set; }
    public int VarbitId { get; set; }
    public int SessionSettingId { get; set; }
}
