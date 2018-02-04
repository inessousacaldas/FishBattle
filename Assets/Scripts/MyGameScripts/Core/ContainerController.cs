using UnityEngine;
using System.Collections.Generic;

public class ContainerController<T> where T: MonoBehaviour
{
    protected bool _hasSetup = false;
    protected List<T> _itemList = new List<T>();

    public List<T> ItemList
    {
        get { return _itemList; }
    }

    public T CurrentItem
    {
        get
        {
            return _useCount > 0
                ? _itemList[_useCount - 1]
                : null;
        }
    }

    protected int _useCount;

    public int UseCount
    {
        get { return _useCount; }
    }

    public virtual void Setup()
    {
        if (!_hasSetup)
        {
            _hasSetup = true;
            Init();
        }
    }

    protected virtual void Init()
    {
        _useCount = 0;
    }

    public virtual void Dispose()
    {
        if (_hasSetup)
        {
            _hasSetup = false;
            OnDispose();
        }
    }

    protected virtual void OnDispose()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            GameObject.Destroy(_itemList[i].gameObject);
        }        
        _itemList.Clear();
        _useCount = 0;
    }


    /// <summary>
    /// 封装掉释放方法
    /// </summary>
    /// <param name="controller"></param>
    public static void StaticDespawn<U>(ContainerController<U> controller) where U: MonoController
    {
        for (int i = 0; i < controller.ItemList.Count; i++)
        {
            var c = controller.ItemList[i];
            AssetPipeline.ResourcePoolManager.Instance.DespawnUI(c.gameObject);
            c.Dispose();
        }
        controller.ItemList.Clear();
        controller.Dispose();
    }

    /// <summary>
    /// 是否符合添加状况
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool AddItem(int index, T item = null)
    {
        if (!IsUseCache(index))
        {
            if (item == null)
            {
                return false;
            }
            else
            {
                _itemList.Add(item);
                _useCount++;
                return true;
            }
        }
        else
        {
            if (item != null)
            {
                return false;
            }
            else
            {
                _itemList[index].gameObject.SetActive(true);
                _useCount++;
                return true;
            }
        }
    }


    public virtual void AddItemByPool(int index, string path, GameObject parent)
    {
        if (!IsUseCache(index))
        {
            var go = AssetPipeline.ResourcePoolManager.Instance.SpawnUIGo(path, parent);
            var controller = go.GetMissingComponent<T>();
            AddItem(index, controller);
        }
        else
        {
            AddItem(index);
        }
    }

    /// <summary>
    /// 检查是否需要往里面塞东西
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual bool IsUseCache(int index)
    {
        return index < _itemList.Count;
    }


    public virtual void StartAdding()
    {
        _useCount = 0;
    }


    public virtual void EndAdding()
    {
        for (int i = _useCount; i < _itemList.Count; i++)
        {
            _itemList[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 将列表的所有元素制空
    /// </summary>
    public virtual void DeactiveItemList()
    {
        StartAdding();
        EndAdding();
    }
}
