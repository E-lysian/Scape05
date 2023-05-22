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
                    //npc.IsUpdateRequired = true;
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

        // if (npc.AnimationId != -1)
        //     mask |= NPCUpdateFlags.Animation;
        //
        // if (npc.SingleHitUpdateRequired)
        //     mask |= NPCUpdateFlags.SingleHit;
        //
        // if (npc.CurrentInteractingEntityId != -1)
        //     mask |= NPCUpdateFlags.InteractingEntity;
        //
        // if (npc.ForceChatUpdateRequired)
        //     mask |= NPCUpdateFlags.ForceChat;
        //
        // if (npc.FaceUpdateRequired)
        //     mask |= NPCUpdateFlags.Face;

        //updateBlock.WriteByte((byte)mask);

        // if (npc.AnimationId != -1)
        // {
        //     updateBlock.WriteWordBigEndian(npc.AnimationId);
        //     updateBlock.WriteByte(0);
        // }

        // if (npc.SingleHitUpdateRequired)
        // {
        //     updateBlock.WriteByteA((byte)npc.CombatMethod.LastHit.Damage);
        //     updateBlock.WriteByteC((byte)npc.CombatMethod.LastHit.Type);
        //     updateBlock.WriteByteA(npc.Health);
        //     updateBlock.WriteByte(npc.MaxHealth);
        // }

        //if (npc.CurrentInteractingEntityId != -1)
        //{
        //    var id = npc.CurrentInteractingEntityId + 32768;
        //    if (npc.CurrentInteractingEntityId == 0x00FFFF)
        //        id = 0x00FFFF;

        //    updateBlock.WriteWord(id);
        //}

        // if (npc.ForceChatUpdateRequired)
        //     updateBlock.WriteString(npc.ForceChatText);

        //if (npc.FaceUpdateRequired)
        //{
        //    var position = npc.Face;
        //    if (npc.ModelId == 925 && npc.Position.AbsoluteX == 3267 && npc.Position.AbsoluteY == 3226)
        //    {
        //    }

        //    updateBlock.WriteWordBigEndian(npc.Face == null ? 0 : npc.Face.AbsoluteX * 2 + 1);
        //    updateBlock.WriteWordBigEndian(npc.Face == null ? 0 : npc.Face.AbsoluteY * 2 + 1);
        //    npc.IsUpdateRequired = false;
        //}
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
