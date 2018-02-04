using AppDto;

#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public class CharactorAgent
    {
        public GeneralCharactor Charactor;

        public CharactorAgent(GeneralCharactor charactor)
        {
            Charactor = charactor;
        }
    }
}
#endif