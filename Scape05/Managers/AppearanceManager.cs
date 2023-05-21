using Scape05.Misc;

namespace Scape05.Managers;

public class AppearanceManager
{
    private int[] _appearance;

    public int Gender { get; set; }
    
    public AppearanceManager()
    {
        _appearance = new int[7];
    }

    public void SetAppearanceSlot(AppearanceSlot slot, int value)
    {
        int index = (int)slot;
        _appearance[index] = value;
    }

    public int GetAppearanceSlot(AppearanceSlot slot)
    {
        int index = (int)slot;
        return _appearance[index];
    }
}