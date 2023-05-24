using Scape05.Entities.Packets;
using Scape05.Misc;
using Scape05.Updaters;

namespace Scape05.Entities;

public class Server
{
    public static Player[] Players { get; } = new Player[ServerConfig.MAX_PLAYERS];
    public static NPC[] NPCs { get; } = new NPC[ServerConfig.MAX_NPCS];

    public void Process()
    {
        FetchPackets();

        /* Combat Updates */
        PerformAttack(Players, NPCs);


        /* Player Movement & Appearance*/
        UpdatePlayers();

        /* NPC Movement & Appearance */
        UpdateNPCs();

        FlushClients();

        ResetPlayers();
        ResetNPCs();
        
        foreach (var entity in Players.Concat<IEntity>(NPCs))
        {
            if (entity == null) continue;
            // entity.CombatBase.DamageTaken = null;
            if (entity.Health <= 0)
            {
                if (entity.CombatBase.Target != null)
                {
                    entity.CombatBase.Target.CombatBase.Target = null;
                    entity.CombatBase.Target = null;
                }

                entity.PerformAnimation(836);
            }
        }
        

    }


    private void FetchPackets()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            for (var j = 0; j < 10; j++)
                player.PacketHandler.Fetch();

            for (int i = 0; i < 10; i++)
            {
                player.PacketHandler.Build();
            }

            player.MovementHandler.Process();
        }
    }

    /* Update players according to the new data that we've received from packets */
    private void UpdatePlayers()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            player.PlayerUpdater.UpdateLocalPlayer();
        }
    }

    private void UpdateNPCs()
    {
        NPCUpdater.Update();
    }

    void PerformAttack(IEntity[] players, IEntity[] npcs)
    {
        foreach (var entity in players.Concat(npcs))
        {
            if (entity == null) continue;
            entity.CombatBase.Attack();
        }

        foreach (var player in players)
        {
            if (player == null) continue;

            var attacker = player.CombatBase.Attacker;
            var target = player.CombatBase.Target;

            if (target == null)
            {
                continue;
            }

            if (attacker.CombatBase.DamageTaken != null && target.CombatBase.DamageTaken != null)
            {
                Console.WriteLine($"{player.Name} took damage this tick, and so did their target {target.Name}");

                // Both attacker and target perform attack animations
                attacker.PerformAttackAnimation();
                target.PerformAttackAnimation();

                // Both attacker and target display hit splats
                attacker.DisplayHitSplat();
                target.DisplayHitSplat();
            }
            else if (player.CombatBase.DamageTaken != null)
            {
                Console.WriteLine($"{player.Name} took damage this tick.");

                // Attacker performs attack animation
                target.PerformAttackAnimation();

                // Target displays hit splat
                attacker.DisplayHitSplat();

                // Target performs block animation
                //target.PerformBlockAnimation();
                attacker.PerformBlockAnimation();
            }
            else if (target.CombatBase.DamageTaken != null)
            {
                Console.WriteLine($"{target.Name} took damage this tick.");
                attacker.PerformAttackAnimation();

                // Target displays hit splat
                target.DisplayHitSplat();
                target.PerformBlockAnimation();
                // Target performs block animation
                target.PerformBlockAnimation();
            }
        }
    }

    // private void UpdateCombat()
    // {
    //     /* Calculate damage and add hit splats */
    //     foreach (var player in Players)
    //     {
    //         if (player == null) continue;
    //         player.CombatManager.Attack();
    //     }
    //
    //     foreach (var npc in NPCs)
    //     {
    //         if (npc == null) continue;
    //         npc.CombatManager.Attack();
    //     }
    //
    //
    //     // foreach (var player in Players)
    //     // {
    //     //     if (player == null) continue;
    //     //     if (player.CombatManager.DamageTaken != -1)
    //     //     {
    //     //         player.Flags |= PlayerUpdateFlags.SingleHit;
    //     //         player.IsUpdateRequired = true;
    //     //     }
    //     // }
    //
    //     // /* Add Animations */
    //     // foreach (var player in Players)
    //     // {
    //     //     var animationId = player.CombatManager.Weapon.animation.AttackId;
    //     //     /* If we hit a 0, and got hit a 0, aka target and attacker hit the same tick */
    //     //     if (player.CombatManager.DamageTaken <= 0 && player.CombatManager.PerformedDamage.Damage <= 0)
    //     //     {
    //     //         /* Set attack animation */
    //     //         animationId = player.CombatManager.Weapon.animation.AttackId;
    //     //         continue;
    //     //     }
    //     //
    //     //     /* I hit, they missed */
    //     //     if (player.CombatManager.DamageTaken <= 0 && player.CombatManager.PerformedDamage.Damage > 0)
    //     //     {
    //     //         /* Set attack animation */
    //     //         animationId = player.CombatManager.Weapon.animation.AttackId;
    //     //         continue;
    //     //     }
    //     //
    //     //     if (player.CombatManager.DamageTaken != -1)
    //     //     {
    //     //         animationId = player.CombatManager.Weapon.animation.BlockId;
    //     //     }
    //     // }
    //
    //
    //     /* Reset-ish */
    //     foreach (var player in Players)
    //     {
    //         if (player == null) continue;
    //         player.CombatManager.CheckWonBattle();
    //         player.CombatManager.CheckLostBattle();
    //         player.CombatManager.PerformedDamage = null;
    //         player.CombatManager.TookDamage = false;
    //     }
    //
    //     foreach (var npc in NPCs)
    //     {
    //         if (npc == null) continue;
    //         npc.CombatManager.CheckWonBattle();
    //         npc.CombatManager.CheckLostBattle();
    //         npc.CombatManager.PerformedDamage = null;
    //         npc.CombatManager.TookDamage = false;
    //
    //     }
    // }

    /* Send packets that we've accumulated */
    private void FlushClients()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            player.DirectFlushStream();
        }
    }

    /* Reset player state */
    private void ResetPlayers()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            player.Reset();
        }
    }

    private void ResetNPCs()
    {
        foreach (var npc in NPCs)
        {
            if (npc == null)
                continue;

            npc.Reset();
        }
    }

    public static void AddNPC(NPC npc)
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null) continue;

            npc.Index = i;
            NPCs[i] = npc;
            return;
        }

        Console.WriteLine("Can't add anymore NPCs");
    }
}