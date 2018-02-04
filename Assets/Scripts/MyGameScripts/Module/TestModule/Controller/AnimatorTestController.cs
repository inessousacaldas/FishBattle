using UnityEngine;
using System;

public class AnimatorTestController : MonoViewController<AnimatorPanel>
{
    /// <summary>
    /// 从DataModel中取得相关数据对界面进行初始化
    /// </summary>

    protected override void AfterInitView ()
    {
        AddTestAnimButton("休闲待机", ModelHelper.AnimType.idle);
        AddTestAnimButton("战斗待机", ModelHelper.AnimType.battle);
        AddTestAnimButton("战斗进场", ModelHelper.AnimType.showup);
        AddTestAnimButton("战斗跑步", ModelHelper.AnimType.forward);
        AddTestAnimButton("行走动作", ModelHelper.AnimType.run);
        AddTestAnimButton("受击动作", ModelHelper.AnimType.hit);
        AddTestAnimButton("眩晕动作", ModelHelper.AnimType.dizzy);
        AddTestAnimButton("胜利结算", ModelHelper.AnimType.victory);
        
        AddTestAnimButton("死亡动作", ModelHelper.AnimType.death);
        AddTestAnimButton("普通攻击", ModelHelper.AnimType.attack);
        AddTestAnimButton("技能蓄力", ModelHelper.AnimType.sing);
        AddTestAnimButton("魔法技能", ModelHelper.AnimType.magic);
        AddTestAnimButton("战技动作1", ModelHelper.AnimType.spe1);
        AddTestAnimButton("战技动作2", ModelHelper.AnimType.spe2);
        AddTestAnimButton("战技动作3", ModelHelper.AnimType.spe3);
        AddTestAnimButton("表演动作", ModelHelper.AnimType.show);
        AddTestAnimButton("奥义动作", ModelHelper.AnimType.super);

        AddTestAnimButton("交互动作1", ModelHelper.AnimType.mood1);
        AddTestAnimButton("交互动作2", ModelHelper.AnimType.mood2);
        AddTestAnimButton("交互动作3", ModelHelper.AnimType.mood3);
        AddTestAnimButton("交互动作4", ModelHelper.AnimType.mood4);
        View.grid_UIGrid.Reposition();
    }

    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.CloseBtn_UIButton.onClick, OnCloseButtonClick);
        EventDelegate.Set(View.mutate_Button_UIButton.onClick, On_Mutate);
        EventDelegate.Set(View.ChangeButton_UIButton.onClick, On_ChangeView);
        EventDelegate.Set(View.ChangeWeaponButton_UIButton.onClick, On_ChangeWeaponView);
        EventDelegate.Set(View.ChangeSizeButton_UIButton.onClick, OnChangeSizeButtonClick);
        EventDelegate.Set(View.ChangeRideUpButton_UIButton.onClick, OnChangeRideUpButtonClick);
        EventDelegate.Set(View.ChangeRideDownButton_UIButton.onClick, OnChangeRideDownButtonClick);
    }

    private void AddTestAnimButton(string buttonName, ModelHelper.AnimType animTypeName)
    {
        var btn = DoAddButton(View.grid_UIGrid.gameObject, buttonName);
        var uibutton = btn.GetMissingComponent<UIButton>();
        EventDelegate.Set(uibutton.onClick, delegate()
            {
                WorldManager.Instance.GetHeroView().TextPlayAnimation(animTypeName);
            });
    }

    private GameObject DoAddButton(GameObject parent, string label, string goName = "Button")
    {
        var go = AddCachedChild(parent, "BaseSmallButton");
        go.name = goName;
        go.GetComponentInChildren< UILabel >().text = label;
        go.transform.localPosition = new Vector3(0.5f, -1 * (parent.transform.childCount - 1) * 25, 0);
        //NGUITools.AdjustDepth(go,20);
        return go;
    }

    private void On_ChangeView()
    {
        try
        {
            WorldManager.Instance.GetHeroView().ChangeView(StringHelper.ToInt(View.ChangeInput_UIInput.value));
        }
        catch (Exception e)
        {
            TipManager.AddTip("变身id必须为数字");
        }
    }

    private void On_ChangeWeaponView()
    {
        try
        {
            WorldManager.Instance.GetHeroView().ChangeWeaponView(StringHelper.ToInt(View.ChangeWeaponInput_UIInput.value));
        }
        catch (Exception e)
        {
            TipManager.AddTip("武器id必须为数字");
        }
    }

    private void OnChangeSizeButtonClick()
    {
        try
        {
            WorldManager.Instance.GetHeroView().ChangeSize(float.Parse(View.ChangeSizeInput_UIInput.value));
        }
        catch (Exception e)
        {
            TipManager.AddTip("缩放比率必须为数字");
        }
    }
    
    private void OnChangeRideUpButtonClick()
    {
        try
        {
            WorldManager.Instance.GetHeroView().UpdateRide(StringHelper.ToInt(View.ChangeRideInput_UIInput.value));
            ModelManager.Player.tempRideId = StringHelper.ToInt(View.ChangeRideInput_UIInput.value);
        }
        catch (Exception e)
        {
            TipManager.AddTip("坐骑模型id必须为数字");
        }
    }
   
    private void OnChangeRideDownButtonClick()
    {
        WorldManager.Instance.GetHeroView().UpdateRide(0);
        ModelManager.Player.tempRideId = 0;
    }

    private void OnCloseButtonClick()
    {
        ProxyAnimatorTestModule.Close();
    }

    private void On_Mutate()
    {
        WorldManager.Instance.GetHeroView().MutateTest(View.mutateColorInput_UIInput.value);
    }
}
