using UnityEngine;
using System.Collections;

public class GameEventCenterTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameEventCenter.SendEvent(GameEvents.OnTestEvent, "aaa");

        GameEventCenter.AddListener(GameEvents.OnTestEvent, TestEvent);

        GameEventCenter.AddListener(GameEvents.OnTestEvent2, TestClass);
    }

    // Update is called once per frame
    void Update () {
        GameEventCenter.SendEvent(GameEvents.OnTestEvent, "bbb");
        GameEventCenter.SendEvent(GameEvents.OnTestEvent2, new TestClass2());
        GameEventCenter.RemoveListener(this);
        
        
	}
    void TestEvent(string message)
    {
        
        Debug.Log(message);
    }
    void TestClass(TestClass1 testClass)
    {
        Debug.Log("testClass");
    }
}

public class TestClass1
{

}
public class TestClass2:TestClass1
{

}