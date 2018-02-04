public partial interface IFriendView
{
//在这里添加自定义接口
    void UpdateView(IFriendData data);

    UITable LeftTable { get; }
    UITable ItemTable { get; }
    UIScrollView RightScrollView { get; }
}

public sealed partial class FriendView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {

    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IFriendData data)
    {
	if (data == null)
        {
            return;
        }
    }
}
