#if ENABLE_SKILLEDITOR

namespace SkillEditor
{
    public abstract class SkillEditorInstance<T> where T: SkillEditorInstance<T>, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.Init();
                }
                return _instance;
            }
        }

        protected virtual void Init()
        {
            
        }
    }
}
#endif