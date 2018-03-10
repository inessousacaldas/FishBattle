using UniRx;
using UnityEngine;

public partial interface ITeamApplicationView
{
//在这里添加自定义接口
    void UpdateView(ITeamData data);

    GameObject ItemGrid_GO { get; }

    float ItemSize { get; }

    UITable GetTable { get; }
       
    UIRecycledList GetRecycledList { get; }
}

public sealed partial class TeamApplicationView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {
       
    }

    protected override void OnDispose()
    {
        
    }

    public GameObject ItemGrid_GO {
        get { return itemGrid_UIRecycledList.gameObject; }
    }

    public float ItemSize {
        get { return itemGrid_UIRecycledList.itemSize; }
    }

    public UITable GetTable
    {
        get { return itemGrid_UITable;}
    }

    public UIRecycledList GetRecycledList
    {
        get { return itemGrid_UIRecycledList; }
    }
}

