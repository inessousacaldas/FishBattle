using AppDto;

public class PreviewVideo : Video
{
    //预览的技能
    public int SkillId;

    private Skill _Skill;

    public  Skill Skill
    {
        get
        {
            if (_Skill != null)
            {
                return _Skill;
            }
            else
            {
                _Skill = DataCache.getDtoByCls<Skill>(SkillId);
                return _Skill;
            }
        }
        set
        {
            _Skill = value;
        }
    }
}