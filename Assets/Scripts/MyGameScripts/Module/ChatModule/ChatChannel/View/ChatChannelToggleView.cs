public partial interface IChatChannelToggleView
{
//在这里添加自定义接口
    void UpdateView(IChatData data);
}

public sealed partial class ChatChannelToggleView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {

    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IChatData data)
    {
	if (data == null)
        {
            return;
        }
    }
}
