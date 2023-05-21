namespace Scape05.Misc;

[Flags]
public enum NPCUpdateFlags
{
    None = 0,
    Animation = 0x10,
    SingleHit = 0x8,
    InteractingEntity = 0x20,
    ForceChat = 0x1,
    Graphics = 0x80,
    DoubleHit = 0x40,
    Transform = 0x2,
    Face = 0x4
}