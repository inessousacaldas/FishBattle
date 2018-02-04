// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildBuildViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using UniRx;

public partial interface IGuildBuildViewController
{
    void UpdateView(IGuildMainData data);
    void OnClickCheckBtn(IGuildMainData data, GuildBuildItemController ctrl);
    GuildBuildItemController SelCtrl { get; }
}

public partial class GuildBuildViewController
{
    private CompositeDisposable _disposable;
    private List<GuildBuildItemController> itemList = new List<GuildBuildItemController>();
    private GuildBuildItemController selCtrl = null;
    public GuildBuildItemController SelCtrl { get { return selCtrl; } }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        itemList.Clear();
        selCtrl = null;
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(IGuildMainData data)
    {
        UpdateTopView(data);
        var list = data.BuildList;
        int idx = 0;
        if (itemList.Count == 0)
        {
            list.ForEach(e =>
            {
                if (idx != 0)
                {
                    var ctrl = AddChild<GuildBuildItemController, GuildBuildItem>(View.UIGrid_UIGrid.gameObject, GuildBuildItem.NAME);
                    _disposable.Add(ctrl.OncheckBtn_UIButtonClick.Subscribe(f => checkBtn_UIButtonEvt.OnNext(ctrl)));
                    itemList.Add(ctrl);
                }
                idx++;
            });
        }
        idx = 0;
        itemList.ForEach(e =>
        {
            idx++;
            e.UpdateView(data, idx);
        });
        OnClickCheckBtn(data, selCtrl);
    }

    public void UpdateTopView(IGuildMainData data)
    {
        var detail = data.GuildDetailInfo;
        float activity = detail.wealthInfo.activity / 1000;
        int acValue = UnityEngine.Mathf.CeilToInt(activity);
        if (acValue > 100)
            acValue = 100;
        View.guildCapitalLabel_UILabel.text = detail.wealthInfo.assets.ToString();
        View.guildMaintainLabel_UILabel.text = detail.wealthInfo.maintainAssets.ToString();
        View.guildFlourishpointLabel_UILabel.text = detail.wealthInfo.prosperity.ToString();
        View.guildPopularityLabel_UILabel.text = acValue.ToString();
    }
    //点击查看按钮
    public void OnClickCheckBtn(IGuildMainData data, GuildBuildItemController ctrl)
    {
        if (ctrl != null)
        {
            var detail = ctrl.buildDetailMsg;
            View.nameLabel_UILabel.text = detail.BuildName;
            View.shortDesLabel_UILabel.text = detail.ShortDes;
            View.lvLabel_UILabel.text = detail.Lv;
            View.icon_UISprite.spriteName = detail.Icon;
            View.effLabel_UILabel.text = detail.Lv == "0"? "公会还没有建设该建筑": ParseDes(data, detail);
            var pos = ctrl.View.transform.position;
            View.desItem_Transform.position = pos;
            var localPos = View.desItem_Transform.localPosition;
            View.desItem_Transform.localPosition = new UnityEngine.Vector3(localPos.x, localPos.y + 50, localPos.z);

            int idx = detail.Idx;
            var serverTime = SystemTimeManager.Instance.GetUTCTimeStamp();  //取服务器时间
            var finishTime = data.BuildUpTimeList.TryGetValue(idx);
            long value = finishTime - serverTime;
            if (value <= 0)
            {
                View.upgradeBtn_BoxCollider.enabled = true;
                View.upgradeBtnLabel_UILabel.spacingX = 11;
                View.upgradeBtnLabel_UILabel.text = "升级";
            }
            else
            {
                View.upgradeBtn_BoxCollider.enabled = false;
                var left = SetDate(value);
                View.upgradeBtnLabel_UILabel.spacingX = 0;
                View.upgradeBtnLabel_UILabel.text = "升级中 剩余" + left;
            }
            var box = View.effLabel_UILabel.GetComponent<UnityEngine.BoxCollider>();
            int width = View.effLabel_UILabel.width;
            int height = View.effLabel_UILabel.height;
            box.center = new UnityEngine.Vector3(width / 2, -height / 2, 0);
            box.size = new UnityEngine.Vector3(width, height, 0);
        }
        selCtrl = ctrl;
        View.DesPanel.SetActive(ctrl != null);
    }

    private string ParseDes(IGuildMainData data,BuildDetailMsg detail)
    {
        int idx = detail.Idx;
        string des = detail.Des;
        string Des = "";
        switch (idx)
        {
            case 1:
                int grade = data.GuildBaseInfo.grade;
                Des = string.Format(des, grade, grade);
                break;
            case 2:
                string str = "公会成员上限";
                var barPubList = data.GuildBuildList;
                var barPubGrade = data.GuildDetailInfo.buildingInfo.barpubGrade;
                var barPub = barPubList.Find(e => e.grade == barPubGrade && e.type == idx);
                if(barPub != null)
                {
                    var bar = barPub as AppDto.GuildBarPub;
                    if(bar != null)
                    {
                        str += bar.memberAmount + "人";
                        var posList = data.GuildPosition;
                        var posAmount = bar.posAmount;
                        var posAmountAry = posAmount.Split('/');
                        posAmountAry.ForEach(e =>
                        {
                            var ary = e.Split('_');
                            int posIndx = StringHelper.ToInt(ary[0]);
                            var posVal = posList.Find(f => f.id == posIndx);
                            if (posVal != null)
                            {
                                var posName = posVal.name;
                                str += "\n" + posName + ary[1] + "人";
                            }
                        });
                    }
                    Des = str;
                }
                break;
            case 3:
                var treasuryList = data.GuildBuildList;
                var treasuryGrade = data.GuildDetailInfo.buildingInfo.treasuryGrade;
                var treasury = treasuryList.Find(e => e.grade == treasuryGrade && e.type == idx);
                if (treasury != null)
                {
                    var trea = treasury as AppDto.GuildTreasury;
                    Des = string.Format(des, trea == null ? 0 : trea.guildAssetLimit);
                }
                break;
        }

        return Des;
    }

    private string SetDate(long ms)
    {
        var timeSpane = System.TimeSpan.FromMilliseconds(ms);
        int d = timeSpane.Days;
        int h = timeSpane.Hours;
        int m = timeSpane.Minutes;
        if(d >= 1)
        {
            if (h == 0) h = 1;
            return d + "天" + h + "小时";
        }
        //2、当前剩余时间少于1天，即小于24小时并大于等于60分钟。 则显示：XX小时xx分钟
        if (h >= 1)
        {
            return h + "小时";
        }
        //3、当前剩余时间少于60分钟小时并且大于60秒。 则显示：xx分钟
        if (m >= 1)
        {
            return "小于一小时";
        }
        return "小于一小时";
    }
}
