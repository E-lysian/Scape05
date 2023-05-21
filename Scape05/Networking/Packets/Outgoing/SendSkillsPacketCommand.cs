using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Networking.Packets.ServerPackets;

public class SendSkillsPacketCommand : IPacketCommand
{
    public void Execute(Player player)
    {
        var skills = player.SkillsManager.GetSkills();
        var experience = player.SkillsManager.GetExperience();
        for (var skillId = 0; skillId < skills.Length; skillId++)
        {
            player.Writer.CreateFrame(ServerOpCodes.PLAYER_SKILL);
            player.Writer.WriteByte(skillId);
            player.Writer.WriteDWordV1(experience[skillId]);
            player.Writer.WriteByte(skills[skillId]);
        }
    }

    public static SendSkillsPacketCommand Create()
    {
        return new SendSkillsPacketCommand();
    }
}