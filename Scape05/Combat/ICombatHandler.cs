using Scape05.Entities;

namespace Scape05.Engine.Combat;

public interface ICombatManager
{
    public Weapon Weapon { get; set; }
    bool InCombat { get; set; }
    IEntity Target { get; set; }
    bool IsInitiator { get; set; }
    void Initiate();
    void Attack();
    void TakeDamage(int damage);
    void CheckWonBattle();
    void CheckLostBattle();
    CombatHit CalculateDamage();
    public bool ShouldInitiate { get; set; }
    void Alert(IEntity target);
    
    public int DamageTaken { get; set; }
    public CombatHit PerformedDamage { get; set; }
}