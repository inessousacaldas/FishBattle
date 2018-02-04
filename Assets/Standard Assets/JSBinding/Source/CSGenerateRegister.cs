using UnityEngine;
public class CSGenerateRegister
{
    public static void RegisterAll()
    {
        if (JSMgr.allCallbackInfo.Count != 0)
        {
            Debug.LogError("Has already register CS binding");
        }

    }
}
