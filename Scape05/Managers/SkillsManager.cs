namespace Scape05.Managers;

public class SkillsManager
{
    private int[] _skills;
    private int[] _experience;

    public SkillsManager()
    {
        _skills = new int[22];
        _experience = new int[22];
        InitializeSkills();
    }

    public int[] GetSkills()
    {
        return _skills;
    }
    
    public int[] GetExperience()
    {
        return _experience;
    }
    
    public int GetSkill(int index)
    {
        return _skills[index];
    }

    public void SetSkill(int index, int value)
    {
        _skills[index] = value;
    }

    public int GetExperience(int index)
    {
        return _experience[index];
    }

    public void SetExperience(int index, int value)
    {
        _experience[index] = value;
    }

    private void InitializeSkills()
    {
        for (var i = 0; i < _skills.Length; i++)
        {
            if (i == 3)
            {
                _skills[i] = 10;
                _experience[i] = 1154;
            }
            else
            {
                _skills[i] = 1;
                _experience[i] = 0;
            }
        }
    }
}