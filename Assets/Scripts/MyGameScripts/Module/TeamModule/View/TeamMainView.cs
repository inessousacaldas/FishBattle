using UnityEngine;

public partial interface ITeamMainView
{
//在这里添加自定义接口
    void UpdateView(ITeamData data);

    GameObject Get_TabContentRoot();
    GameObject Get_CreateTabRoot();

    GameObject Get_RecommendTeamRoot();
}

public sealed partial class TeamMainView
{
//获取控件引用之后在这里完成一些自定义的控件初始化操作
    protected override void LateElementBinding()
    {

    }

    protected override void OnDispose()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ITeamData data)
    {
        HideTabContent();

        switch (data.TeamMainViewData.CurTab)
        {
            case TeamMainViewTab.Team:
                Get_TabContentRoot().gameObject.SetActive(true);
                //TitleNameSprite_UISprite.spriteName = "title_team";
                break;
            case TeamMainViewTab.CreateTeam:
                Get_CreateTabRoot().gameObject.SetActive(true);
                //TitleNameSprite_UISprite.spriteName = "title_team";
                break;
            case TeamMainViewTab.RecommendTeam:
                Get_RecommendTeamRoot().gameObject.SetActive(true);
                //TitleNameSprite_UISprite.spriteName = "title_team";
                break;
        }
    }

    public GameObject Get_TabContentRoot(){
        return TabContentRoot;
    }

    public GameObject Get_CreateTabRoot(){
        return CteateTeamRoot;
    }

    public GameObject Get_RecommendTeamRoot()
    {
        return RecommendTeamRoot;
    }

    private void HideTabContent()
    {
        Get_TabContentRoot().gameObject.SetActive(false);
        Get_CreateTabRoot().gameObject.SetActive(false);
        Get_RecommendTeamRoot().gameObject.SetActive(false);
    }
}
