using Scape05.Engine.Combat;
using Scape05.Entities.Packets;
using Scape05.Handlers;
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
    public int CombatLevel { get; set; } = 3;
    public int MaxHealth { get; set; } = 15;
    public int Health { get; set; } = 15;
    public ICombatManager CombatManager { get; set; }
    public int AnimationId { get; set; } = -1;
    public int TotalLevel { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; }
    public int Size { get; set; } = 1;
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
        CombatManager = new MeleeCombatHandler(this);
        CombatManager.Weapon = new(4151, 1, 4, new CombatAnimations(1658, 1659, 1111), WeaponType.SWORD);

        CombatBase  = new MeleeCombat
        {
            WeaponSpeed = 4
        };
        
        Location = new Location(3183, 3440);
        // Location = new Location(3200, 3200);
        BuildArea = new BuildArea(this);

        InitializePlayerColors();
        InitializePlayerAppearance();
        InitializePlayerEquipment();
    }
    
    public void PerformBlockAnimation()
    {
        AnimationId = CombatManager.Weapon.Animation.BlockId;
        Flags |= PlayerUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public void PerformAttackAnimation()
    {
        AnimationId = CombatManager.Weapon.Animation.AttackId;
        Flags |= PlayerUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public void DisplayHitSplat()
    {
        Flags |= PlayerUpdateFlags.SingleHit;
        IsUpdateRequired = true;
    }

    public void NotifyAttacked(IEntity attacker)
    {
        Console.WriteLine($"{Name} Notified Attack by: {attacker.Name}");
        // Engage in combat with the attacker
        CombatBase.Attacker = this;
        CombatBase.Target = attacker;
        CombatBase.Tick -= 1;
    }

    public void PerformAnimation(int animId)
    {
        AnimationId = animId;
        Flags |= PlayerUpdateFlags.Animation;
        IsUpdateRequired = true;
    }

    public DelayedTaskHandler DelayedTaskHandler { get; set; } = new();

    public ICombatBase CombatBase { get; set; }  

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
        EquipmentManager.EquipItem(EquipmentSlot.Cape, 6570); //6570 fcape
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
        IsUpdateRequired = false;
        Flags = PlayerUpdateFlags.None;
        MovementHandler.PrimaryDirection = -1;
        MovementHandler.SecondaryDirection = -1;
        AnimationId = -1;
        CombatBase.DamageTaken = null;
    }
}