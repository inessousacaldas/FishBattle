using UnityEngine;
using System.Collections;
using AppDto;

public class MissionSubmitItemViewController :UseLeftViewCellController
{
    private const string ViewName = "IdentifyItemUseView";

    public static MissionSubmitItemViewController Setup(GameObject pos)
    {
        GameObject prefab = AssetPipeline.ResourcePoolManager.Instance.LoadUI(ViewName) as GameObject;
        GameObject module = GameObjectExt.AddChild(pos,prefab);
        MissionSubmitItemViewController leftView  = module.GetMissingComponent<MissionSubmitItemViewController>();
        UIHelper.AdjustDepth(module,1);
        return leftView;
    }

    public override void SetUseDto(BagItemDto dto)
    {
        View.EmptyInfo.SetActive(false);
    }


    public override void SetData(BagItemDto dto)
    {
        //_dto = dto;
        if(!_dtoList.Contains(dto))
        {
            _dtoList.Add(dto);
        }
        else
        {
            _dtoList.Remove(dto);
        }
        View.EmptyInfo.SetActive(false);
        //
        //Itemte
    }

    public override void SetTips(string tips)
    {
        View.EquipmentTipLabel.text = tips;
    }
}
