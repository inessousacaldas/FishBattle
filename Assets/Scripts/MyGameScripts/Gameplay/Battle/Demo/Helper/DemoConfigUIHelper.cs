using System.Collections.Generic;
using AppDto;

public static class DemoConfigUIHelper
{
    public static void InitializePopListWithMonsterJobDesc(UIPopupList pUIPopupList, bool pSelectFirst = true)
    {
        InitializePopList<Monster>(pUIPopupList, ModelManager.BattleDemoConfig.MonsterJobIdDic, pSelectFirst);
    }

    public static void InitializePopListWithCharacterNames(UIPopupList pUIPopupList, bool pSelectFirst = true)
    {
        InitializePopList<int>(pUIPopupList, ModelManager.BattleDemoConfig.CharacterNameDic, pSelectFirst);
    }

    public static void InitializePopListWithCharacterType(UIPopupList pUIPopupList)
    {
        InitializePopList<GeneralCharactor.CharactorType>(pUIPopupList, ModelManager.BattleDemoConfig.CharacterTypeDic);
    }

    public static void InitializePopList<T>(UIPopupList pUIPopupList, Dictionary<string,T> pDataProvider, bool pSelectFirst = true)
    {
        pUIPopupList.Clear();
        Dictionary<string,T> tDataProvider = pDataProvider;
        if (null == tDataProvider || tDataProvider.Count <= 0)
        {
            GameDebuger.LogError(string.Format("InitializePopList {0} failed , pDataProvider is null !", pUIPopupList));
            return;
        }
        Dictionary<string,T>.Enumerator tEnum = tDataProvider.GetEnumerator();
        bool tInited = false;
        while (tEnum.MoveNext())
        {
            pUIPopupList.AddItem(tEnum.Current.Key); 
            if (!pSelectFirst || tInited)
                continue;
            tInited = true;
            pUIPopupList.value = tEnum.Current.Key;
        }
    }
}