using UnityEngine;

public sealed partial class ItemCommonTipsView : BaseView
{
    public const string NAME = "ItemCommonTipsView";
    #region Element Bindings

    /// bind gameobject
    public GameObject ItemGainTipsCellAnchor;

    protected override void InitElementBinding ()
    {
        var root = this.gameObject;
        ItemGainTipsCellAnchor = root.FindGameObject("ItemGainTipsCellAnchor");
    }
    #endregion
}