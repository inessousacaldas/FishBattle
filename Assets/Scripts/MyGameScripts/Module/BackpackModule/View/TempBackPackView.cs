using UnityEngine;

public partial interface ITempBackPackView
{
//在这里添加自定义接口
    void UpdateView(IBackpackData data);

    GameObject TempBackAnchor { get; }
    GameObject MyBackAnchor { get; }
}

public sealed partial class TempBackPackView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {
        transLabel_UILabel.text = "转移";
        transAllLabel_UILabel.text = "全部转移";
        arrayLabel_UILabel.text = "整理";
        CompositeLabel_UILabel.text = "合成";
        decomposeLabel_UILabel.text = "分解";
    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IBackpackData data)
    {
	    if (data == null)
        {
            return;
        }
    }

    public GameObject TempBackAnchor {
        get { return TempAnchor; }
    }

    public GameObject MyBackAnchor {
        get { return BackAnchor; }
    }
}
