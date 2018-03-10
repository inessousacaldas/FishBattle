using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class NpcViewDataManager
{
    public readonly NpcViewManager master;
    private readonly Dictionary<long, BaseNpcUnit> _npcs;
    public QuadTree<BaseNpcUnit> quadTree { get; private set; }
    public List<long> setupNpcList { get; private set; }
    public Dictionary<long, BaseNpcUnit> npcs { get; private set; }
    private List<BaseNpcUnit> _alwaysShowList;
    public ReadOnlyCollection<BaseNpcUnit> alwaysShowList { get; private set; }
    public NpcViewDataManager(NpcViewManager _master)
    {
        master = _master;
        _npcs = new Dictionary<long, BaseNpcUnit>(50);
        npcs = new Dictionary<long, BaseNpcUnit>(50);
        setupNpcList = new List<long>(25);
        _alwaysShowList = new List<BaseNpcUnit>(10);
        alwaysShowList = new ReadOnlyCollection<BaseNpcUnit>(_alwaysShowList);
    }
    public void Setup()
    {
        quadTree = new QuadTree<BaseNpcUnit>(new Vector2(10, 10), 5);
    }

    public void AddNpc(BaseNpcUnit baseNpcUnit)
    {
        _npcs.Add(baseNpcUnit.GetNpcUID(), baseNpcUnit);
        quadTree.Insert(baseNpcUnit);
    }
    public void RemoveNpc(long npcUID)
    {
        BaseNpcUnit baseNpcUnit = _npcs[npcUID];
        if (baseNpcUnit == null)
            return;
        if(_alwaysShowList.Contains(baseNpcUnit))
        {
            _alwaysShowList.Remove(baseNpcUnit);
        }
        else
        {
            quadTree.Remove(baseNpcUnit);
            setupNpcList.Remove(npcUID);
        }

        baseNpcUnit.Destroy();
        _npcs.Remove(npcUID);
    }
    public void AddToAlwaysShow(BaseNpcUnit baseNpcUnit)
    {
        if (_alwaysShowList.Contains(baseNpcUnit) || _npcs.ContainsKey(baseNpcUnit.GetNpcUID()) == false)
            return;
        quadTree.Remove(baseNpcUnit);
        _alwaysShowList.Add(baseNpcUnit);
    }
    public void Dispose()
    {
        if (_npcs != null)
        {
            foreach (var npcUnit in _npcs.Values)
            {
                npcUnit.Destroy();
            }

            _npcs.Clear();
        }
        setupNpcList.Clear();
        quadTree = null;
    }
}

