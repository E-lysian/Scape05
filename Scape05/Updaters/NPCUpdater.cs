using Scape05.Entities;
using Scape05.IO;
using Scape05.Misc;

namespace Scape05.Updaters;

public class NPCUpdater
{
    // public static void GenerateMovement()
    // {
    //     foreach (var npc in Server.NPCs)
    //     {
    //         if (npc == null)
    //             continue;
    //
    //         var shouldMove = new Random().Next(0, 3) == 0;
    //         if (shouldMove && npc.CanWalk)
    //         {
    //             var direction = npc.Move();
    //             if (direction != Direction.NONE)
    //                 npc.PrimaryDirection = (int)direction;
    //         }
    //     }
    // }

    public static void Update()
    {
        foreach (var player in Server.Players)
        {
            if (!(player is Player client))
                continue;

            var updateBlock = new RSStream(new byte[4096]);

            client.Writer.CreateFrameVarSizeWord(ServerOpCodes.NPC_UPDATE);
            client.Writer.InitBitAccess();

            client.Writer.WriteBits(8, client.LocalNpcs.Count);

            foreach (var npc in client.LocalNpcs.ToList())
            {
                if (Server.NPCs[npc.Index] != null && player.Location.IsWithinArea(npc.Location) && !npc.NeedsPlacement)
                {
                    UpdateMovement(npc, client.Writer);
                    
                    // npc.CombatManager.Attack();
                    
                    if (npc.IsUpdateRequired)
                        AppendUpdates(npc, updateBlock);
                }
                else
                {
                    client.LocalNpcs.Remove(npc);
                    client.Writer.WriteBits(1, 1);
                    client.Writer.WriteBits(2, 3);
                }
            }

            foreach (var npc in Server.NPCs)
            {
                if (client.LocalNpcs.Count >= 255)
                    break;

                if (npc == null || client.LocalNpcs.Contains(npc))
                    continue;

                if (npc.Location.IsWithinArea(player.Location))
                {
                    client.LocalNpcs.AddLast(npc);
                    npc.Flags |= NPCUpdateFlags.Face;
                    npc.IsUpdateRequired = true;

                    AddNPC(client, npc, client.Writer);
                    AppendUpdates(npc, updateBlock);
                }
                else
                {
                    client.LocalNpcs.Remove(npc);
                }
            }

            if (updateBlock.CurrentOffset > 0)
            {
                client.Writer.WriteBits(14, 16383);
                client.Writer.FinishBitAccess();
                client.Writer.WriteBytes(updateBlock.Buffer, updateBlock.CurrentOffset, 0);
            }
            else
            {
                client.Writer.FinishBitAccess();
            }

            client.Writer.EndFrameVarSizeWord();
            client.DirectFlushStream();
        }

        foreach (var npc in Server.NPCs)
        {
            if (npc == null) continue;

            npc.Flags = NPCUpdateFlags.None;
            npc.IsUpdateRequired = false;
            //npc.AnimationUpdateRequired = false;
            //npc.GraphicsUpdateRequired = false;
            //npc.SingleHitUpdateRequired = false;
            //npc.ForceChatUpdateRequired = false;
            //npc.TransformUpdateRequired = false;
            //npc.FaceUpdateRequired = false;
            //npc.IsUpdateRequired = false;

            npc.MovementHandler.PrimaryDirection = -1;
            npc.MovementHandler.SecondaryDirection = -1;
        }
    }

    private static void AppendUpdates(NPC npc, RSStream updateBlock)
    {
        var mask = NPCUpdateFlags.None;
        
        if (npc.Flags.HasFlag(NPCUpdateFlags.Animation))
        {
            mask |= NPCUpdateFlags.Animation;
        }

        if (npc.Flags.HasFlag(NPCUpdateFlags.InteractingEntity))
        {
            mask |= NPCUpdateFlags.InteractingEntity;
        }
        
        if (npc.Flags.HasFlag(NPCUpdateFlags.SingleHit))
        {
            mask |= NPCUpdateFlags.SingleHit;
        }
        
        if (npc.Flags.HasFlag(NPCUpdateFlags.Face))
        {
            mask |= NPCUpdateFlags.Face;
        }

        updateBlock.WriteByte((byte)mask);

        if ((mask & NPCUpdateFlags.Animation) != 0)
        {
            updateBlock.WriteWordBigEndian(npc.AnimationId); //866 //1365 wc
            updateBlock.WriteByte(0); //delay
        }
        
        
        if ((mask & NPCUpdateFlags.InteractingEntity) != 0)
        {
            var id = npc.InteractingEntityId;
            updateBlock.WriteWord(id);
        }
        
        if ((mask & NPCUpdateFlags.SingleHit) != 0)
        {
            updateBlock.WriteByteA((byte)npc.CombatBase.DamageTaken.Damage); //hitDamage
            updateBlock.WriteByteC((byte)npc.CombatBase.DamageTaken.Type); //hitType
            updateBlock.WriteByteA(npc.Health); //currentHealth
            updateBlock.WriteByte(npc.MaxHealth); //maxHealth
        }
        
        if ((mask & NPCUpdateFlags.Face) != 0)
        {
            updateBlock.WriteWordBigEndian(npc.Face == null ? 0 : npc.Face.X);
            updateBlock.WriteWordBigEndian(npc.Face == null ? 0 : npc.Face.Y);
        }
    }


    private static void AddNPC(Player player, NPC npc, RSStream playerWriter)
    {
        playerWriter.WriteBits(14, npc.Index);
        playerWriter.WriteBits(5, npc.Location.Y - player.Location.Y);
        playerWriter.WriteBits(5, npc.Location.X - player.Location.X);
        playerWriter.WriteBits(1, 0);
        playerWriter.WriteBits(12, npc.ModelId);
        playerWriter.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
    }

    private static void UpdateMovement(NPC npc, RSStream writer)
    {
        if (npc.MovementHandler.SecondaryDirection == -1)
        {
            if (npc.MovementHandler.PrimaryDirection == -1)
            {
                if (npc.IsUpdateRequired)
                {
                    writer.WriteBits(1, 1);
                    writer.WriteBits(2, 0);
                }
                else
                {
                    writer.WriteBits(1, 0);
                }
            }
            else
            {
                writer.WriteBits(1, 1);
                writer.WriteBits(2, 1);
                writer.WriteBits(3, npc.MovementHandler.PrimaryDirection);
                writer.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
            }
        }
        else
        {
            writer.WriteBits(1, 1);
            writer.WriteBits(2, 2);
            writer.WriteBits(3, npc.MovementHandler.PrimaryDirection);
            writer.WriteBits(3, npc.MovementHandler.SecondaryDirection);
            writer.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
        }
    }
}