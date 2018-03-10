using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEventCenterTest2 : MonoBehaviour
{

    List<GameEventCenterTsetItem> list = new List<GameEventCenterTsetItem>();
    void Start()
    {

    }

    int i = 1500;
    int j = 1500;
    void Update()
    {
        if (i > 0)
        {
            i--;
            list.Add(new GameEventCenterTsetItem());
        }
        else
        {
            if (j > 2)
            {
                j--;
                list[list.Count - 1].Dispose();
                list.RemoveAt(list.Count - 1);
            }
        }
        GameEventCenter.SendEvent(GameEvents.OnTestEvent, "bbb");
        GameEventCenter.SendEvent(GameEvents.OnTestEvent2, new TestClass2());
    }
}

public class GameEventCenterTsetItem
{

    public GameEventCenterTsetItem()
    {
        GameEventCenter.AddListener(GameEvents.OnTestEvent, TestEvent);

        GameEventCenter.AddListener(GameEvents.OnTestEvent2, TestClass);
    }

    private void TestClass(TestClass1 param)
    {
        // throw new NotImplementedException();
    }

    private void TestEvent(string param)
    {
        //throw new NotImplementedException();
    }
    public void Dispose()
    {
        GameEventCenter.RemoveListener(this);
    }
}

//调用C#内部的GetInvokeList消耗比较大，