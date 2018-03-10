using UnityEngine;
using System.Collections;

public class TeamConfirmHeadView:BaseView
{
    public const string NAME ="TeamConfirmHead";
    #region Element Bindings
    public UISprite Head_UISprite;
    public UILabel Name_UILabel;
    public UISprite OccupationBG_UISprite;
    public UISprite OccupationIcon_UISprite;
    public UILabel LevelLabel_UILabel;
    #endregion

    protected override void InitElementBinding()
    {
        var root = this.gameObject;
        Head_UISprite = root.FindScript<UISprite>("Head");
        Name_UILabel = root.FindScript<UILabel>("Name");
        LevelLabel_UILabel = root.FindScript<UILabel>("LevelLabel");
        OccupationBG_UISprite = root.FindScript<UISprite>("OccupationBG");
        OccupationIcon_UISprite = root.FindScript<UISprite>("OccupationBG/OccupationIcon");
    }
}
