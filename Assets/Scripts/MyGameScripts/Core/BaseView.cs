using System.Reflection;
using UnityEngine;

public abstract class BaseView
{
    public GameObject gameObject {
        get { return _gameObject; }
    }

    public GameObject _gameObject;
    #region 兼容旧版本
    public Transform transform
    {
        get { return gameObject.transform; }
    }
//
//    public T GetComponent<T>()
//    {
//        return gameObject.GetComponent<T>();
//    }
//
    public static bool IsViewDestroy(BaseView view)
    {
        return view == null || view.gameObject == null;
    }


//    #region 处理MonoBehaviour被销毁，view不为空的情况。
//    public static bool operator !=(BaseView view1, BaseView view2)
//    {
//        return !(view1 == view2);
//    }
//
//    public static bool operator ==(BaseView view1, BaseView view2)
//    {
//        if(Equals(view1, null) || view1.gameObject == null)
//        {
//            if(Equals(view2, null) || view2.gameObject == null)
//            {
//                return true;
//            }
//            return false;
//        }
//
//        //view1 不为null
//        if (Equals(view2, null) || view2.gameObject == null)
//        {
//            return false;
//        }
//
//        return Equals(view1, view2);
//    }
//
//    public override bool Equals(object obj)
//    {
//        return base.Equals(obj);
//    }
//
//    public override int GetHashCode()
//    {
//        return base.GetHashCode();
//    }
//
//    #endregion
//
    #endregion

    public static T Create<T>(Transform root) where T:BaseView, new()
    {
        var view = new T {_gameObject = root.gameObject};
        view.Setup();
        return view;
    }

    public static T Create<T>(GameObject root) where T: BaseView, new()
    {
        var view = new T {_gameObject = root};
        view.Setup();
        return view;
    }

    private void Setup() {
        InitElementBinding();
        LateElementBinding();

        InitReactiveEvents();
        
    }

    /// implementation should initialize all reactive events to EventStream system
    /// and call this method at the end of OnBindViewElements
    protected virtual void InitReactiveEvents()
    {
    }


    protected virtual void InitElementBinding(){
        
    }

    protected virtual void LateElementBinding(){
    }

    protected virtual void OnDispose(){

    }

    protected virtual void ClearReactiveEvents()
    {
    }

    protected virtual void CustomClearReactiveEvents()
    {
    }

    public void Dispose()
    {
        ClearReactiveEvents();
        CustomClearReactiveEvents();
        OnDispose ();
    }

    #region 组件自动赋值
    public void InitAllChildComponents()
    {
        var fields = GetType().GetFields();
        for (var i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            if (typeof(Component).IsAssignableFrom(field.FieldType))
            {
                field.SetValue(this, GetChildComponentByName(field.Name, field.FieldType));
            }
        }
    }

    public Component GetChildComponentByName(string name, System.Type type)
    {
        Transform tran = GetChildByName(transform,name);
        if (tran != null)
        {
            return tran.GetComponent(type);
        }
        else
        {
            return null;
        }
    }

    public T GetChildComponentByName<T>(string name) where T : Component
    {
        Transform tran = GetChildByName(transform,name);
        if(tran != null)
        {
            return tran.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }
    public static Transform GetChildByName(Transform tar, string name, bool inActive = true)
    {
        if (tar != null)
        {
            Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
            if (tarList != null)
            {
                foreach (Transform t in tarList)
                {
                    if (t.name == name)
                    {
                        return t;
                    }
                }
            }
        }
        return null;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        //SetActive(false);
        gameObject.SetActive(false);
    }
    #endregion
}

public static class BaseViewExt
{
    public static T[] GetComponentsInChildren<T> (this BaseView pBaseView, bool pIncludeInactive = false) where T : Component
    {
        if (null == pBaseView || null == pBaseView.gameObject) {
            GameDebuger.LogError ("GetComponentsInChildren failed for null == pBaseView or null == pBaseView.gameObject");
            return null;
        }

        return pBaseView.gameObject.GetComponentsInChildren<T>(pIncludeInactive);
    }

    public static T GetComponent<T> (this BaseView pBaseView) where T : Component
    {
        if (null == pBaseView) {
            GameDebuger.LogError ("GetComponentsInChildren failed for null == pBaseView !");
            return null;
        }

        if (null == pBaseView.gameObject) {
            GameDebuger.LogError ("GetComponentsInChildren failed for null == pBaseView.gameObject !");
            return null;
        }

        return pBaseView.gameObject.GetComponent<T> ();
    }
}