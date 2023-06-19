using Scape05.Entities;
using Scape05.Entities.Packets;
using Scape05.Handlers;
using Scape05.Misc;
using Scape05.World.Clipping;

namespace Scape05.Engine.Combat;

public class RangeCombat : ICombatMethod
{
    public RangeCombat(IEntity owner)
    {
        _owner = owner;
    }

    public void Attack()
    {
        if (_owner.CombatTarget == null) return;

        /* Check distance delta validity */
        var withinRange = WithinRange();
        if (!withinRange)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Can't reach..");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Can reach!");
        Console.ForegroundColor = ConsoleColor.White;
        var player = (Player)_owner;
        player.MovementHandler.Reset();

        if (CombatTick % _owner.Weapon.Speed == 0)
        {
            var target = _owner.CombatTarget;
            if (target != null)
            {
                _owner.HitQueue.TryDequeue(out var hit);
                if (hit == null)
                {
                    hit = new DamageInfo
                    {
                        DamageSource = _owner,
                        Type = DamageType.Block,
                        Amount = 0
                    };
                }

                short npcIndex = (short)(target.Index);
                if (npcIndex <= 0)
                {
                    npcIndex = 1;
                }

                var npc = Server.NPCs[npcIndex];
                var pX = _owner.Location.X + _owner.Size / 2;
                var pY = _owner.Location.Y + _owner.Size / 2;
                var nX = npc.Location.X + npc.Size / 2;
                var nY = npc.Location.Y + npc.Size / 2;
                short projectileGraphicsId = 18;
                byte yOffset = (byte)(nY - pY);
                byte xOffset = (byte)(nX - pX);
                // _owner.PerformAnimation(426);
                
                _owner.DelayedTaskHandler.RegisterDelayedTask(new DelayedProjectileTask(() =>
                    PacketBuilder.SpawnProjectilePacket((Player)_owner, 50, xOffset, yOffset, (short)(npcIndex + 1),
                        projectileGraphicsId, 43,
                        15, 15, 28, 20, 64)));

                npc.DelayedTaskHandler.RegisterDelayedTask(new DelayedHitSplatTask(npc,
                    () => { target.CombatMethod.TakeDamage(hit); }));
                HasPerformedDamage = true;
                Console.WriteLine($"+ [{_owner.Name}] Attacked: [{target.Name}]");
            }

            CombatTick = 0;
        }

        CombatTick++;

        // if (_owner.Weapon.Speed == 0 || _owner.Weapon == null || !CanCombat)
        //     return;
        //
        // /* Extra check to see if player is in combat? */
        //
        // /* If inside the target, step away */
        //
        // if (_owner.CombatTarget != null && _owner.CombatMethod.CanCombat)
        // {
        //     /* Check if we can range from here, if so stop movement */
        //     Console.WriteLine("Trying to attack..");
        //
        //     var withinRange = WithinRange();
        //     if (!withinRange)
        //     {
        //         Console.ForegroundColor = ConsoleColor.Red;
        //         Console.WriteLine("Can't reach..");
        //         Console.ForegroundColor = ConsoleColor.White;
        //         return;
        //     }
        //
        //     Console.ForegroundColor = ConsoleColor.Green;
        //     Console.WriteLine("Can reach!");
        //     Console.ForegroundColor = ConsoleColor.White;
        //     var player = (Player)_owner;
        //     player.MovementHandler.Reset();
        //
        //     _owner.InCombat = true;
        //     if (SkipTick)
        //     {
        //         SkipTick = false;
        //         return;
        //     }
        //
        //     if (CombatTick % _owner.Weapon.Speed == 0)
        //     {
        //         if (_owner.CombatTarget != null)
        //         {
        //             short npcIndex = (short)(_owner.CombatTarget.Index);
        //             if (npcIndex <= 0)
        //             {
        //                 npcIndex = 1;
        //             }
        //
        //             var npc = Server.NPCs[npcIndex];
        //
        //             var pX = _owner.Location.X + _owner.Size / 2;
        //             var pY = _owner.Location.Y + _owner.Size / 2;
        //
        //             var nX = npc.Location.X + npc.Size / 2;
        //             var nY = npc.Location.Y + npc.Size / 2;
        //             short projectileGraphicsId = 18;
        //             byte yOffset = (byte)(nY - pY);
        //             byte xOffset = (byte)(nX - pX);
        //             _owner.PerformAnimation(426);
        //             _owner.DelayedTaskHandler.RegisterDelayedTask(new DelayedProjectileTask(() =>
        //                 PacketBuilder.SpawnProjectilePacket((Player)_owner, 50, xOffset, yOffset, (short)(npcIndex + 1),
        //                     projectileGraphicsId, 43,
        //                     15, 15, 28, 20, 64)));
        //
        //             npc.DelayedTaskHandler.RegisterDelayedTask(new DelayedHitSplatTask(npc, () =>
        //             {
        //                 _owner.CombatTarget.CombatMethod.TakeDamage(new DamageInfo
        //                 {
        //                     DamageSource = _owner,
        //                     Amount = 0,
        //                     Type = DamageType.Damage
        //                 });
        //             }));
        //
        //             // _owner.CombatTarget.CombatMethod.TakeDamage(new DamageInfo
        //             // {
        //             //     DamageSource = _owner,
        //             //     Amount = 0,
        //             //     Type = DamageType.Damage
        //             // });
        //
        //             Console.WriteLine($"+ [{_owner.Name}] Attacked: [{_owner.CombatTarget.Name}]");
        //         }
        //
        //         CombatTick = 0;
        //     }
        //
        //     if (CanCombat)
        //     {
        //         Console.WriteLine($"[{_owner.Name}]: CanCombat!");
        //         CombatTick++;
        //     }
        // }
    }

    private bool WithinRange()
    {
        // var horizontally = _owner.Location.X >= _owner.CombatTarget.Location.X - _owner.Size &&
        //                    _owner.Location.X <= _owner.CombatTarget.Location.X + _owner.CombatTarget.Size;
        // var vertically = _owner.Location.Y >= _owner.CombatTarget.Location.Y - _owner.Size &&
        //                  _owner.Location.Y <= _owner.CombatTarget.Location.Y + _owner.CombatTarget.Size;
        //
        // var withinRange = horizontally && vertically;
        // return withinRange;

        return PathFinder.isProjectilePathClear(_owner.Location.X, _owner.Location.Y, 0, _owner.CombatTarget.Location.X,
            _owner.CombatTarget.Location.Y);
    }


    public void TakeDamage(DamageInfo info)
    {
        DamageTaken = info;
        // InCombat = true;
        if (_owner.CombatTarget == null)
        {
            _owner.CombatTarget = info.DamageSource;
            SkipTick = true;
        }

        if (_owner is NPC)
        {
            var npc = (NPC)_owner;
            npc.Follow = _owner.CombatTarget;

            npc.Flags |= NPCUpdateFlags.InteractingEntity;
            npc.InteractingEntityId = npc.Follow.Index + 32768;
        }

        if (_owner is Player)
        {
            var player = (Player)_owner;
            player.Follow = _owner.CombatTarget;

            player.Flags |= PlayerUpdateFlags.InteractingEntity;
            player.InteractingEntityId = _owner.CombatTarget.Index;
        }

        if (info.Amount > _owner.Health)
        {
            info.Amount = _owner.Health;
            _owner.Health -= _owner.Health;
        }
        else
        {
            _owner.Health -= info.Amount;
        }

        _owner.DisplayHitSplat();
        HasTakenDamage = true;

        if (_owner.Health <= 0)
        {
            _owner.InCombat = false;
            _owner.CombatTarget.InCombat = false;

            _owner.CombatMethod.CanCombat = false;
            _owner.CombatTarget.CombatMethod.CanCombat = false;

            HasPerformedDamage = false;
            HasTakenDamage = false;

            _owner.DelayedTaskHandler.RegisterDelayedTask(new BattleEndDelayedTask(_owner));
        }
    }


    public void SetAnimation()
    {
        if (_owner.Health <= 0)
        {
            _owner.PerformAnimation(_owner.Weapon.Animation.FallAnim);
            return;
        }

        switch (_owner)
        {
            case Player player:
                if (player.MovementHandler.PrimaryDirection != -1)
                    return;
                break;
            case NPC npc:
                if (npc.MovementHandler.PrimaryDirection != -1)
                    return;
                break;
        }


        if (HasPerformedDamage)
        {
            /* Set Attack Animation */
            _owner.PerformAttackAnimation();
        }
        else if (HasTakenDamage)
        {
            /* Set Block Animation */
            _owner.PerformBlockAnimation();
        }

        HasPerformedDamage = false;
        HasTakenDamage = false;
    }

    private readonly IEntity _owner;
    public bool HasPerformedDamage { get; set; }
    public bool HasTakenDamage { get; set; }
    public int CombatTick { get; set; }
    public DamageInfo DamageTaken { get; set; }
    public bool SkipTick { get; set; }
    public bool CanCombat { get; set; } = true;
}