using UnityEngine;
using AppDto;

public class BattleItemUseViewController : UseLeftViewCellController
{

    private const string ViewName = "IdentifyItemUseView";

    public static BattleItemUseViewController Setup(GameObject pos)
    {
        GameObject prefab = AssetPipeline.ResourcePoolManager.Instance.LoadUI(ViewName) as GameObject;
        GameObject module = GameObjectExt.AddChild(pos, prefab);
        BattleItemUseViewController leftView = module.GetMissingComponent<BattleItemUseViewController>();

        UIHelper.AdjustDepth(module, 1);
        return leftView;
    }

    public override void SetUseDto(BagItemDto dto)
    {
        View.EmptyInfo.SetActive(false);

        //View.CountLabel.text = string.Format("剩余使用次数：{0}",10);
    }

    public override void SetData(BagItemDto dto)
    {
        _dto = dto;
        View.EmptyInfo.SetActive(false);
        GameDebuger.TODO(@"ItemTextTipManager.Instance.ShowItem(dto, View.EquipmentTipLabel, true);");
    }

    public void UpdateItemUsedCount(int count)
    {
        View.CountLabel.text = string.Format("剩余使用次数：{0}", 10 - count);
    }
}

