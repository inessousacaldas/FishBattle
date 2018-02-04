using UnityEngine;
using System.Collections;

public class CopyPanelItemView :BaseView
{
    public const string NAME ="CopyViewItem";
    #region Element Bindings
    public UIButton CopyViewItem_UIButton;
    public UILabel CopyName_UILabel;
    public Transform CopyDeslLabel_UILabel;
    public UILabel CopyLevel_UILabel;
    public UISprite BossHead_UISprite;
    public UISprite EliteIcon_UISprite;
    public UISprite SelectIcon_UISprite;
    #endregion

    protected override void InitElementBinding()
    {
        var root = this.gameObject;
        CopyViewItem_UIButton = root.GetComponent<UIButton>();
        SelectIcon_UISprite = root.FindScript<UISprite>("SelectIcon");
        CopyName_UILabel = root.FindScript <UILabel>("Label");
        CopyLevel_UILabel = root.FindScript<UILabel>("LevelLabel");
        EliteIcon_UISprite = root.FindScript<UISprite>("EliteIcon");
        CopyDeslLabel_UILabel = root.FindScript<Transform>("DeslLabel");
        BossHead_UISprite = root.FindScript<UISprite>("HeadSprite");
    }

}
