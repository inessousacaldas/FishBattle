using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class TeamSettingWinController : MonoViewController<TeamSettingWinPrefab>
{
    private bool _isBarrage = true;     //弹幕
    private bool _isNeedHelp = true;    //侠义匹配
    private bool _isAutoBuilding = true;  //自动建队

    private List<TeamSetItemController> _itemList = new List<TeamSetItemController>();
    private string[] _nameList = {"组队弹幕", "侠义匹配", "自动建队"};
    private string[] _descList = {"队友的信息以弹幕的形式显示.", "自动匹配时可以匹配到高级玩家助战.", "自动匹配时若无队伍,则自动创建队伍."};

    protected override void AfterInitView()
    {
        for (int i = 0; i < 3; i++)
        {
            var item = AddChild<TeamSetItemController, TeamSetItem>(_view.Table_UITable.gameObject, TeamSetItem.NAME);
            _itemList.Add(item);
            item.UpdateInfo(_nameList[i], _descList[i]);
        }
    }

    protected override void RegistCustomEvent()
    {
        base.RegistCustomEvent();
        EventDelegate.Set(View.CloseBtn_UIButton.onClick, OnCloseBtnClickHandler);
        EventDelegate.Set(View.WinBg_UIButton.onClick, OnCloseBtnClickHandler);
    }

    protected override void OnDispose()
    {
        
    }

    public void OpenTeamSettingWin(string title = ""
        ,string titleSprite = ""
        ,string desc = "")
    {
        SetDesc(desc);
        SetTtitle(title, titleSprite);
        InitConvinienceCheckBox();
    }

    private void SetTtitle(string title, string titleSprite)
    {
        if (string.IsNullOrEmpty(title))
            View.TitleLb_UILabel.gameObject.SetActive(false);
        else
        {
            View.TitleLb_UILabel.gameObject.SetActive(true);
            View.TitleLb_UILabel.text = title;
            return;
        }
    }

    private void SetDesc(string desc)
    {
        if (string.IsNullOrEmpty(desc)) return;

        View.DescLabel_UILabel.text = desc;
    }

    private void InitConvinienceCheckBox()
    {
       
    }

    private void OnCloseBtnClickHandler()
    {
        ProxyWindowModule.CloseTeamSettingWindow();
    }
}
