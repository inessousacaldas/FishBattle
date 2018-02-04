using System;
using System.Collections.Generic;

public class CollectionHelper
{
    /// <summary>
    /// 遍历list中的说有元素， 忽略掉null的
    /// </summary>
    /// <param name="pFunc"> bool = true 继续遍历， bool = false 停止遍历</param>
    public static void TravelAllElemntWithoutNull<T>(List<T> pList, Func<T, bool> pFunc)
    {
        if (pList == null || pFunc == null)
            return;

        for (int i = 0; i < pList.Count; ++i)
        {
            T tElement = pList[i];
            if (tElement == null)
                continue;

            //如返回值 = true 则停止遍历
            if (pFunc(tElement) == false)
                break;
        }
    }

    /// <summary>
    /// 获取字典中的Value， 若为 null，则新建一个并加入字典中
    /// </summary>
    public static bool GetOrCreateValue_Dictionary<T1, T2>(Dictionary<T1, T2> pDic, T1 pKey, ref T2 pValue, Func<T1, T2> pCrateFunc)
    {
        if (pDic == null)
            return false;

        pDic.TryGetValue(pKey, out pValue);
        if (pValue != null)
            return true;

        if (pCrateFunc == null)       
            return false;

        pValue = pCrateFunc(pKey);

        if (pDic.ContainsKey(pKey) == true)
            pDic[pKey] = pValue;
        else
            pDic.Add(pKey, pValue);
            
        return true;
    }
}

