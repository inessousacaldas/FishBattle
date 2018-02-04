using UnityEngine;
using System.Collections;

public class TeamConfirmHeadView:BaseView
{
    public const string NAME ="TeamConfirmHead";
    #region Element Bindings
    public UISprite Head_UISprite;
    public UILabel Name_UILabel;
    public UISprite Stats_UISprite;
    #endregion

    protected override void InitElementBinding()
    {
        var root = this.gameObject;
        Head_UISprite = root.FindScript<UISprite>("Head");
        Stats_UISprite = root.FindScript<UISprite>("stats");
        Name_UILabel = root.FindScript<UILabel>("Name");
    }
}
