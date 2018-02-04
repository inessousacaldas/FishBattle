using UnityEngine;

#if ENABLE_SKILLEDITOR
namespace SkillEditor
{
    public interface IViewController
    {
        BaseView BaseView { get; }
        void SetupView(GameObject go);
        void Dispose();
    }

    public abstract class SkillEditorBaseController<V>: IViewController where V : BaseView, new()
    {
        protected V _view;

        public BaseView BaseView
        {
            get { return _view; }
        }

        public void SetupView(GameObject go)
        {
            if (_view == null)
            {
                _view = BaseView.Create<V>(go);

                AfterInitView();
            }
        }

        protected virtual void AfterInitView()
        {
            
        }

        public void Dispose()
        {
            if (_view != null)
            {
                OnDispose();

                _view = null;
            }
        }

        protected virtual void OnDispose()
        {
            
        }
    }
}
#endif