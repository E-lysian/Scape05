using Scape05.Misc;

namespace Scape05.Managers;

public class EquipmentManager
{
    private int[] _equipment;

    public EquipmentManager()
    {
        _equipment = new int[12];
    }

    public int[] GetEquipment()
    {
        return _equipment;
    }
    
    public void EquipItem(EquipmentSlot slot, int itemId)
    {
        int index = (int)slot;
        _equipment[index] = itemId;
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        int index = (int)slot;
        _equipment[index] = 0;
    }

    public int GetItem(EquipmentSlot slot)
    {
        int index = (int)slot;
        return _equipment[index];
    }
    
    public int GetItem(int slot)
    {
        int index = slot;
        return _equipment[index];
    }
}
