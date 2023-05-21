using Scape05.Entities;
using Scape05.Helpers;
using Scape05.IO;
using Scape05.Misc;

namespace Scape05.Updaters;

public class PlayerUpdater
{
    private readonly Player _player;
    public RSStream UpdateTempBlock { get; set; }

    public PlayerUpdater(Player player)
    {
        _player = player;
    }

    public void UpdateLocalPlayer()
    {
        UpdateTempBlock = new RSStream(new byte[5000]);
        _player.Writer.CreateFrameVarSizeWord(ServerOpCodes.PLAYER_UPDATE);
        _player.Writer.InitBitAccess();

        UpdateLocalPlayerMovement();
        UpdateState(_player, UpdateTempBlock, false, false);
        UpdateRemotePlayers();
        UpdateNewPlayers();

        FinalizeUpdate();

        _player.Writer.EndFrameVarSizeWord();
    }

    private void UpdateRemotePlayers()
    {
        _player.Writer.WriteBits(8, _player.LocalPlayers.Count);

        foreach (var other in _player.LocalPlayers.ToList())
        {
            if (other.Location.IsWithinArea(_player.Location) && !other.NeedsPlacement)
            {
                UpdateRemotePlayerMovementLocally(other, _player.Writer);

                if (other.IsUpdateRequired)
                    UpdateState(other, UpdateTempBlock, false, false);
            }
            else
            {
                RemovePlayer(other);
            }
        }
    }

    private void UpdateNewPlayers()
    {
        for (var i = 0; i < Server.Players.Length; i++)
        {
            if (_player.LocalPlayers.Count() >= 255)
                break;

            var other = Server.Players[i];
            if (other == null || other == _player)
                continue;

            if (!_player.LocalPlayers.Contains(other) && other.Location.IsWithinArea(_player.Location))
            {
                AddPlayer(_player.Writer, _player, other);
                UpdateState(other, UpdateTempBlock, true, false);
            }
        }
    }

    private void FinalizeUpdate()
    {
        if (UpdateTempBlock.CurrentOffset > 0)
        {
            _player.Writer.WriteBits(11, 2047);
            _player.Writer.FinishBitAccess();
            _player.Writer.WriteBytes(UpdateTempBlock.Buffer, UpdateTempBlock.CurrentOffset, 0);
        }
        else
        {
            _player.Writer.FinishBitAccess();
        }
    }

    private void RemovePlayer(Player player)
    {
        _player.Writer.WriteBits(1, 1);
        _player.Writer.WriteBits(2, 3);
        _player.LocalPlayers.Remove(player);
    }


    private void UpdateLocalPlayerMovement()
    {
        var updateRequired = _player.IsUpdateRequired;
        if (_player.NeedsPlacement)
        {
            _player.Writer.WriteBits(1, 1);
            var posX = _player.BuildArea.GetPositionRelativeToOffsetChunkX();
            var posY = _player.BuildArea.GetPositionRelativeToOffsetChunkY();
            var height = _player.Location.Height;
            AppendPlacement(_player.Writer, posX, posY, height, _player.MovementHandler.DiscardMovementQueue,
                updateRequired);
        }
        else
        {
            var pDir = _player.MovementHandler.PrimaryDirection;
            var sDir = _player.MovementHandler.SecondaryDirection;
            if (pDir != -1)
            {
                _player.Writer.WriteBits(1, 1);
                if (sDir != -1)
                    AppendRun(_player.Writer, pDir, sDir, updateRequired);
                else
                    AppendWalk(_player.Writer, pDir, updateRequired);
            }
            else
            {
                if (updateRequired)
                {
                    _player.Writer.WriteBits(1, 1);
                    AppendStand(_player.Writer);
                }
                else
                {
                    _player.Writer.WriteBits(1, 0);
                }
            }
        }
    }

    private static void UpdateRemotePlayerMovementLocally(Player player, RSStream writer)
    {
        var updateRequired = player.IsUpdateRequired;
        var pDir = player.MovementHandler.PrimaryDirection;
        var sDir = player.MovementHandler.SecondaryDirection;
        if (pDir != -1)
        {
            writer.WriteBits(1, 1);
            if (sDir != -1)
                AppendRun(writer, pDir, sDir, updateRequired);
            else
                AppendWalk(writer, pDir, updateRequired);
        }
        else
        {
            if (updateRequired)
            {
                writer.WriteBits(1, 1);
                AppendStand(writer);
            }
            else
            {
                writer.WriteBits(1, 0);
            }
        }
    }

    private void UpdateState(Player player, RSStream updatetempBlock, bool forceAppearance, bool noChat)
    {
        PlayerUpdateFlags mask = player.Flags;

        if (mask >= PlayerUpdateFlags.FullMask)
        {
            mask |= PlayerUpdateFlags.FullMask;
            updatetempBlock.WriteWordBigEndian((int)mask);
        }
        else
        {
            updatetempBlock.WriteByte((byte)mask);
        }

        //if ((mask & PlayerUpdateFlags.Graphics) != 0) AppendGraphics(player, updatetempBlock);
        //if ((mask & PlayerUpdateFlags.Animation) != 0) AppendAnimation(player, updatetempBlock, player.AnimationId);
        //if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendNPCInteract(player, updatetempBlock);
        if ((mask & PlayerUpdateFlags.Appearance) != 0) AppendAppearance(player, updatetempBlock);
        //if ((mask & PlayerUpdateFlags.FaceDirection) != 0) AppendFaceDirection(player, updatetempBlock);
        //if ((mask & PlayerUpdateFlags.SingleHit) != 0) AppendSingleHit(player, updatetempBlock);
    }

    private void AppendAppearance(Player client, RSStream tempB)
    {
        var updateBlockBuffer = new RSStream(new byte[128]);
        updateBlockBuffer.WriteByte(client.AppearanceManager.Gender); // Gender
        updateBlockBuffer.WriteByte(client.HeadIcon); // Skull Icon

        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Helmet);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Cape);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Amulet);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Weapon);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Body, AppearanceSlot.Chest);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Shield);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Body, AppearanceSlot.Arms);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Legs, AppearanceSlot.Legs);
        WriteHairItem(updateBlockBuffer, client);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Gloves, AppearanceSlot.Hands);
        WriteEquipmentItem(updateBlockBuffer, client, EquipmentSlot.Boots, AppearanceSlot.Feet);
        WriteBeardItem(updateBlockBuffer, client);

        WritePlayerColors(updateBlockBuffer, client);

        WriteMovementAnimations(updateBlockBuffer);

        updateBlockBuffer.WriteQWord(client.Name.ToLong());
        updateBlockBuffer.WriteByte(client.CombatLevel);
        updateBlockBuffer.WriteWord(client.TotalLevel);

        tempB.WriteByteC(updateBlockBuffer.CurrentOffset);
        tempB.WriteBytes(updateBlockBuffer.Buffer, updateBlockBuffer.CurrentOffset, 0);
    }

    private void WriteEquipmentItem(RSStream stream, Player client, EquipmentSlot equipmentSlot,
        AppearanceSlot appearanceSlot = AppearanceSlot.None)
    {
        int itemId = client.EquipmentManager.GetItem(equipmentSlot);
        int slotValue = itemId > 1 ? 0x200 + itemId : GetAppearanceSlotValue(client, equipmentSlot, appearanceSlot);
        stream.WriteWord(slotValue);
    }

    private int GetAppearanceSlotValue(Player client, EquipmentSlot equipmentSlot, AppearanceSlot appearanceSlot)
    {
        if (appearanceSlot != AppearanceSlot.None)
        {
            int appearanceItemId = client.AppearanceManager.GetAppearanceSlot(appearanceSlot);
            if (appearanceItemId >= 0)
            {
                return 0x100 + appearanceItemId;
            }
        }

        return 0;
    }

    private void WriteHairItem(RSStream stream, Player client)
    {
        int helmetItemId = client.EquipmentManager.GetItem(EquipmentSlot.Helmet);
        int hairItemId = client.EquipmentManager.GetItem(EquipmentSlot.Hair);
        if (!GameConstants.IsFullHelm(helmetItemId) || !GameConstants.IsFullMask(helmetItemId) ||
            !GameConstants.IsFullHelm(hairItemId) || !GameConstants.IsFullMask(hairItemId))
        {
            stream.WriteWord(0x100 + client.AppearanceManager.GetAppearanceSlot(AppearanceSlot.Head));
        }
        else
        {
            stream.WriteByte(0);
        }
    }

    private void WriteBeardItem(RSStream stream, Player client)
    {
        int helmetItemId = client.EquipmentManager.GetItem(EquipmentSlot.Helmet);
        if (!GameConstants.IsFullHelm(helmetItemId) || !GameConstants.IsFullMask(helmetItemId))
        {
            stream.WriteWord(0x100 + client.AppearanceManager.GetAppearanceSlot(AppearanceSlot.Beard));
        }
        else
        {
            stream.WriteByte(0);
        }
    }

    private void WritePlayerColors(RSStream stream, Player client)
    {
        for (int i = 0; i < 5; i++)
        {
            stream.WriteByte(client.ColorManager.GetColor(i));
        }
    }

    private void WriteMovementAnimations(RSStream stream)
    {
        stream.WriteWord(0x328); // stand
        stream.WriteWord(0x337); // stand turn
        stream.WriteWord(0x333); // walk
        stream.WriteWord(0x334); // turn 180
        stream.WriteWord(0x335); // turn 90 cw
        stream.WriteWord(0x336); // turn 90 ccw
        stream.WriteWord(0x338); // run
    }


    private static void AddPlayer(RSStream writer, Player player, Player other)
    {
        Console.WriteLine($"Add Slot: {other.Index}");
        writer.WriteBits(11, other.Index);
        writer.WriteBits(1, 1);
        writer.WriteBits(1, 1);

        var delta = Location.Delta(player.Location, other.Location);
        writer.WriteBits(5, delta.Y);
        writer.WriteBits(5, delta.X);
    }

    private static void AppendPlacement(RSStream writer, int localX, int localY, int z, bool discardMovementQueue,
        bool attributesUpdate)
    {
        writer.WriteBits(2, 3); // 3 - placement.

        // Append the actual sector.
        writer.WriteBits(2, z);
        writer.WriteBits(1, discardMovementQueue ? 1 : 0);
        writer.WriteBits(1, attributesUpdate ? 1 : 0);
        writer.WriteBits(7, localY);
        writer.WriteBits(7, localX);
    }

    private static void AppendRun(RSStream writer, int pDir, int sDir, bool updateRequired)
    {
        writer.WriteBits(2, 2); // 2 - running.

        // Append the actual sector.
        writer.WriteBits(3, pDir);
        writer.WriteBits(3, sDir);
        writer.WriteBits(1, updateRequired ? 1 : 0);
    }

    private static void AppendWalk(RSStream writer, int pDir, bool updateRequired)
    {
        writer.WriteBits(2, 1); // 1 - walking.

        // Append the actual sector.
        writer.WriteBits(3, pDir);
        writer.WriteBits(1, updateRequired ? 1 : 0);
    }

    private static void AppendStand(RSStream writer)
    {
        writer.WriteBits(2, 0); // 0 - no movement.
    }
}