public partial interface IFacePanelView
{
//在这里添加自定义接口
 
}

public sealed partial class FacePanelView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {

    }

    protected override void OnDispose()
    {

    }

}
