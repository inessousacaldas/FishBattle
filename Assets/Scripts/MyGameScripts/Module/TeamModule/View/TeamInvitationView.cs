public partial interface ITeamInvitationView
{
//在这里添加自定义接口
    void UpdateView(ITeamData data);

    UIRecycledList ItemUIRecycledList{ get; }

    UITable GetTable { get; }
    
    UIScrollView GetScrollView { get; }
}

public sealed partial class TeamInvitationView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {

    }

    protected override void OnDispose()
    {

    } 

    public UIRecycledList ItemUIRecycledList {
        get { return ItemGrid_UIRecycledList; }
    }

    public UITable GetTable { get { return ItemGrid_UITable; } }
    
    public UIScrollView GetScrollView{get { return ScrollView; }}
}
