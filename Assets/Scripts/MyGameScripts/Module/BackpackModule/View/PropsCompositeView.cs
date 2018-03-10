public partial interface IPropsCompositeView
{
//在这里添加自定义接口
    void UpdateView(IBackpackData data);
}
    
public sealed partial class PropsCompositeView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {
        btnLabel_UILabel.text = "合 成";
    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IBackpackData _data)
    {
	    if (_data == null)
        {
            return;
        }
        var data = _data.CompositeViewData;
        switch (data.CurCompositTab)
        {
            case CompositeTabType.Composite:
                CompositeContent.SetActive(true);
                DecompositeContent.SetActive(false);
//                TitleNameSprite_UISprite.spriteName = "";
                break;
            case CompositeTabType.DeComposite:
                CompositeContent.SetActive(false);
                DecompositeContent.SetActive(true);
//                TitleNameSprite_UISprite.spriteName = "";
                break;    
        }
    }
}
