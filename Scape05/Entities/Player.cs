using Scape05.Entities.Packets;
using Scape05.Managers;
using Scape05.Misc;
using Scape05.Updaters;

namespace Scape05.Entities;

public class Player : Client, IEntity
{
    public string Name { get; set; }
    public string Password { get; set; }
    public Location Location { get; set; }
    public BuildArea BuildArea { get; set; }

    /* Extract these? */
    public int CombatLevel { get; set; }
    public int TotalLevel { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public int HeadIcon { get; set; }

    public EquipmentManager EquipmentManager { get; set; } = new();
    public AppearanceManager AppearanceManager { get; set; } = new();
    public ColorManager ColorManager { get; set; } = new();
    public SkillsManager SkillsManager { get; set; } = new();
    public PlayerUpdater PlayerUpdater { get; set; }
    public MovementHandler MovementHandler { get; set; }
    public PlayerUpdateFlags Flags { get; set; } = PlayerUpdateFlags.None;
    public PacketHandler PacketHandler { get; set; }

    public Player()
    {
        PlayerUpdater = new PlayerUpdater(this);
        MovementHandler = new MovementHandler(this);
        PacketHandler = new PacketHandler(this);
        // PacketReceiver = new PacketReceiver(this);

        Location = new Location(3200, 3200);
        BuildArea = new BuildArea(this);

        InitializePlayerColors();
        InitializePlayerAppearance();
        InitializePlayerEquipment();
    }

    public void Login()
    {
        PacketBuilder.SendMapRegion(this);
        PacketBuilder.SendMessage($"{ServerConfig.WELCOME_MSG}", this);
        PacketBuilder.SendPlayerStatus(this);
        
        PacketBuilder.SendSkills(this);
        
        PacketBuilder.SendSidebarInterface(this, 0, 2423);
        PacketBuilder.SendSidebarInterface(this, 1, 3917);
        PacketBuilder.SendSidebarInterface(this, 2, 638);
        PacketBuilder.SendSidebarInterface(this, 3, 3213);
        PacketBuilder.SendSidebarInterface(this, 4, 1644);
        PacketBuilder.SendSidebarInterface(this, 5, 5608);
        PacketBuilder.SendSidebarInterface(this, 6, 1151);
        PacketBuilder.SendSidebarInterface(this, 8, 5065);
        PacketBuilder.SendSidebarInterface(this, 9, 5715);
        PacketBuilder.SendSidebarInterface(this, 10, 2449);
        PacketBuilder.SendSidebarInterface(this, 11, 4445);
        PacketBuilder.SendSidebarInterface(this, 12, 147);
        PacketBuilder.SendSidebarInterface(this, 13, 6299);
        
        PacketBuilder.SendEquipment(this);
        
        Flags |= PlayerUpdateFlags.Appearance;
        NeedsPlacement = true;
        IsUpdateRequired = true;
    }

    private void InitializePlayerEquipment()
    {
        EquipmentManager.EquipItem(EquipmentSlot.Helmet, 3751);
        EquipmentManager.EquipItem(EquipmentSlot.Amulet, 1704);
        EquipmentManager.EquipItem(EquipmentSlot.Cape, 1052); //9786
        EquipmentManager.EquipItem(EquipmentSlot.Weapon, 4151);
        EquipmentManager.EquipItem(EquipmentSlot.Body, 4736);
        EquipmentManager.EquipItem(EquipmentSlot.Shield, 1187);
        EquipmentManager.EquipItem(EquipmentSlot.Gloves, 1580);
        EquipmentManager.EquipItem(EquipmentSlot.Legs, 4087);
        EquipmentManager.EquipItem(EquipmentSlot.Boots, 3105);
    }

    private void InitializePlayerAppearance()
    {
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Chest, 18);
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Arms, 26);
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Legs, 36);
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Head, 1);
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Hands, 33);
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Feet, 42);
        AppearanceManager.SetAppearanceSlot(AppearanceSlot.Beard, 10);
    }

    private void InitializePlayerColors()
    {
        ColorManager.SetColor(0, 5);
        ColorManager.SetColor(1, 8);
        ColorManager.SetColor(2, 9);
        ColorManager.SetColor(3, 5);
        ColorManager.SetColor(4, 0);
    }

    public void Reset()
    {
        NeedsPlacement = false;
    }
}