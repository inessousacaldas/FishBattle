using AppDto;
using UniRx;
using Asyn;
using System;
using System.Collections.Generic;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
using AssetPipeline;
using UnityEngine;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner friendDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = FriendDataMgr.DataMgr; });
    }
}

public sealed partial class FriendDataMgr : AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete)
    {
        Action act = delegate ()
        {
            onComplete(this);
        };

        FriendNetMsg.ReqFriendInfo(act);
    }

    private int[] _flowerCounts = new int[3]
    {
        99, 520, 999
    };
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendOnlineNotify>(HandleFriendOnlineNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendGradeNotify>(HandleFriendGradeNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendDegreeNotify>(HandleFriendDegreeNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<LatestTeammateNotify>(HandleTeammateNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendActionNotify>(HandleFriendActionNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendDynamicNotify>(HandleFriendDynamicNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FriendFlowersNotify>(HandleFriendFlowerNotify));

        //送花count 判断
        var flowerCountStr = DataCache.GetStaticConfigValues(AppStaticConfigs.FLOWERS_COUNT);
        var strs = flowerCountStr.Split('-');
        var cnt = _flowerCounts.Length; 
        strs.ForEachI((str, index) =>
        {
            if (index < cnt)
            {
                var s = -1;
                int.TryParse(str, out s);
                if (s > 0)
                    _flowerCounts[index] = s;
                else
                GameDebuger.LogError("送花数量错误 ： " + flowerCountStr + ", 请检查配表");
            }
        });
    }

    //好友上下线状态变化
    private void HandleFriendOnlineNotify(FriendOnlineNotify notify)
    {
        FriendInfoDto dto = _data.CacheFriendInfoDtos.Find(d => d.friendId == notify.id);
        if (dto != null)
        {
            dto.online = notify.online;
            dto.offlineTime = !notify.online ? SystemTimeManager.Instance.GetUTCTimeStamp() : dto.offlineTime;
        }
        else if(_data.CacheTeammateList.Find(d => d.friendId == notify.id) != null)
        {
            _data.CacheTeammateList.Find(d => d.friendId == notify.id).online = notify.online;
        }

        FireData();
    }

    //好友升级状态变化
    private void HandleFriendGradeNotify(FriendGradeNotify notify)
    {
        FriendInfoDto dto = _data.CacheFriendInfoDtos.Find(d => d.friendId == notify.id);
        if (dto != null)
        {
            dto.grade = notify.newLevel;
        }

        FireData();
    }

    private string DegreeTogetherNum = DataCache.getDtoByCls<StaticConfig>(AppStaticConfigs.FRIEND_DEGREE_TOGETHER_AMOUNT).value;
    //好友度
    private void HandleFriendDegreeNotify(FriendDegreeNotify notify)
    {
        #region 好友系统 刷新
        FriendInfoDto dto = DataMgr._data.CacheFriendInfoDtos.Find(d => d.friendId == notify.friendPlayerId);
        if (dto != null)
        {
            switch(notify.rule)
            {
                case (int)FriendDegreeNotify.FriendDegreeRuleEnum.Delete:
                    dto.degree = notify.amount;
                    break;
                case (int)FriendDegreeNotify.FriendDegreeRuleEnum.Divorce:
                    dto.degree -= notify.amount;
                    break;
                    //互为好友 友好度为10
                case (int)FriendDegreeNotify.FriendDegreeRuleEnum.Together:
                    dto.degree = StringHelper.ToInt(DegreeTogetherNum);
                    break;
                default:
                    dto.degree += notify.amount;
                    break;
            }
        }

        FireData();
        #endregion

        #region 送花系统刷新
        FlowerMainViewDataMgr.DataMgr.RefreshFriendDegree(notify.friendPlayerId, dto.degree);
        #endregion
    }

    //最近队友
    private void HandleTeammateNotify(LatestTeammateNotify notify)
    {
        notify.friendInfoDtos.ForEach(dto =>
        {
            if(_data.CacheTeammateList.Find(item=> item.friendId == dto.friendId) == null && dto.friendId!=ModelManager.Player.GetPlayerId())
            {
                _data.CacheTeammateList.Add(dto);
            }
        });

        FireData();
    }

    //好友互动 被点赞、丢鸡蛋
    private void HandleFriendActionNotify(FriendActionNotify notify)
    {
        var name = "人";
        if (DataMgr._data.CacheFriendInfoDtos.Find(item => item.friendId == notify.id) != null)
            name = DataMgr._data.CacheFriendInfoDtos.Find(item => item.friendId == notify.id).name;

        if (notify.actionId == (int)FriendActionNotify.FriendActionEnum.Egg)
            TipManager.AddTip(string.Format("你被{0}扔鸡蛋了。", name));
        else if (notify.actionId == (int)FriendActionNotify.FriendActionEnum.Like)
            TipManager.AddTip(string.Format("你被{0}点赞了。", name));
    }

    //好友互动 弹窗
    private void HandleFriendDynamicNotify(FriendDynamicNotify notify)
    {
        var friendName = DataMgr._data.GetFriendDtoById(notify.playerId) == null ? string.Empty : DataMgr._data.GetFriendDtoById(notify.playerId).name;
        var ctrl = FriendShowViewController.Show<FriendShowViewController>(FriendShowView.NAME, UILayerType.ThreeModule, false, true);
        ctrl.UpdateView(notify, friendName);
    }

    //好友送花
    private void HandleFriendFlowerNotify(FriendFlowersNotify notify)
    {
        if(notify.toId == ModelManager.Player.GetPlayerId() && notify.flowersCount >= _flowerCounts[0])
        {
            var ctrl = FlowerReceiveViewController.Show<FlowerReceiveViewController>(FlowerReceiveView.NAME, UILayerType.ThreeModule, true);
            ctrl.UpdateView(notify);
        }

        var itemData = ItemHelper.GetGeneralItemByItemId(notify.itemId);
        var propsData = itemData as Props;
        if (itemData == null || propsData == null || propsData.propsParam as PropsParam_16 == null) return;

        //计算播放时间
        var propsParam = propsData.propsParam as PropsParam_16;
        var strs = propsParam.second.Split('-');
        if (strs.Length < 3) return;

        if (notify.flowersCount < _flowerCounts[0])
        {
            if (notify.toId == ModelManager.Player.GetPlayerId())
                TipManager.AddTip(string.Format("{0}向你赠送{1}朵{2}", notify.fromName, notify.flowersCount, itemData.name));
        }
        else if (notify.flowersCount < _flowerCounts[1])
        {
            if (notify.toId == ModelManager.Player.GetPlayerId())
                ShowFlowerEff(propsParam.clientPlay, strs[0]);
        }
        else if (notify.flowersCount < _flowerCounts[2])
            ShowFlowerEff(propsParam.clientPlay, strs[1]);
        else
            ShowFlowerEff(propsParam.clientPlay, strs[2]);
    }

    private GameObject _flowerEff;
    public void ShowFlowerEff(string name, string second)
    {
        if(_flowerEff != null)
            DisposeFlowerEff();

        ResourcePoolManager.Instance.SpawnEffectAsync(name, (inst) =>
        {
            if (null == inst)
            {
                GameDebuger.LogWarning(string.Format("特效资源加载失败，名字：{0}", name));
                return;
            }

            GameObjectExt.AddPoolChild(LayerManager.Root.SceneCamera.gameObject, inst);
            _flowerEff = inst;
            JSTimer.Instance.SetupCoolDown(string.Format("FriendDataFlowerEff_{0}", name), float.Parse(second), null, DisposeFlowerEff);
        }, () =>
        {
            GameDebuger.LogWarning(string.Format("特效资源加载失败，名字：{0}", name));
        });
    }

    private void DisposeFlowerEff()
    {
        if (_flowerEff != null)
            AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(_flowerEff);
    }

    public bool IsMyFriend(long id)
    {
        return _data.CacheFriendInfoDtos.Find(dto => dto.friendId == id) == null ? false : true;
    }

    public void AddFriend(FriendInfoDto dto)
    {
        _data.CacheFriendInfoDtos.Add(dto);

        //如果之前拉黑该好友
        if(isMyBlack(dto.friendId))
            _data.CacheBlackList.Remove(_data.CacheBlackList.Find(item => item.friendId == dto.friendId));

        //添加好友请求返回 刷新好友界面
        FireData();
    }

    public void AddBlack(FriendInfoDto dto)
    {
        _data.CacheBlackList.Add(dto);

        DeleteFriend(dto.friendId);
    }

    public void DeleteFriend(long id)
    {
        if (_data.CacheFriendInfoDtos.Find(item=>item.friendId == id) != null)
        {
            var tempDto = _data.CacheFriendInfoDtos.Find(item => item.friendId == id);
            _data.CacheFriendInfoDtos.Remove(_data.CacheFriendInfoDtos.Find(item => item.friendId == id));
            //先删除再添加 因为添加时会判断是否是好友
            PrivateMsgDataMgr.DataMgr.AddFriendInfoDto(tempDto);
        }
    }

    public void DeleteBlack(long id)
    {
        if (_data.CacheBlackList.Find(item => item.friendId == id) != null)
        {
            _data.CacheBlackList.Remove(_data.CacheBlackList.Find(item => item.friendId == id));
        }
    }

    public bool isMyBlack(long id)
    {
        return _data.CacheBlackList.Find(dto => dto.friendId == id) == null ? false : true;
    }

    public void SetDefaultTabView()
    {
        _data.CurTab = FriendViewTab.MyFriend;
    }

    public FriendInfoDto GetFriendDtoById(long id)
    {
        return _data.CacheFriendInfoDtos.Find(dto => dto.friendId == id) == null ? null
            : _data.CacheFriendInfoDtos.Find(dto => dto.friendId == id);
    }

    public IEnumerable<FriendInfoDto> GetMyFriendList()
    {
        return _data.CacheFriendInfoDtos;
    }

    public void OnDispose(){
            
    }
}
