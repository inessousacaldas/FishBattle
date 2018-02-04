using System;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public enum ItemType
{
    defaultType,
}

public enum OperatorType
{
    defaultType,
}
public static class ItemSelectMenuHelper
{
    private static void aaa(BagItemDto dto)
    {
    }

    private static Dictionary<OperatorType, Tuple<string, Action<BagItemDto>>> dic = new Dictionary<OperatorType, Tuple<string, Action<BagItemDto>>>
    {
        {OperatorType.defaultType, Tuple.Create<string, Action<BagItemDto>>("aaa", aaa)},
    };

    public static void Show(GameObject go, BagItemDto dto)
    {
        OperatorType[] ty = new OperatorType[10]; // get by ItemType
        Dictionary<string,System.Action<string>> _dic = new Dictionary<string, Action<string>>();
        ty.ForEach(t =>
        {
            Action<string> act = delegate(string s)
            {
                if (dic != null)
                    dic[t].p2(dto);
            };
            _dic[dic[t].p1] = act;
        });

        MultipleSelectionManager.Open(go, _dic, MultipleSelectionManager.Side.LeftTop );
    }

//    开一个界面 传参数 函数列表进去
}
