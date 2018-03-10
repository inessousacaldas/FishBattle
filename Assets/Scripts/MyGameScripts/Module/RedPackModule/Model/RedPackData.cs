// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Wangsimin
// Created  : 2/27/2018 4:27:42 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;
public enum RedPackChannelType
{
    World = ChatChannel.ChatChannelEnum.World,//世界红包
    Guild = ChatChannel.ChatChannelEnum.Guild//公会红包
}
public interface IRedPackConfirmViewData
{
    //发送红包界面
    string Word { get;}
    int Total { get; }
    int Count { get;  }
    string Title { get;}
    string NameTitle { get;}
    //二次确认界面
}


public interface IRedPackMainViewData
{
    RedPackChannelType CurTabMainView { get; }
    IEnumerable<RedPackDetailDto> GetWorldRedPacks { get; }
    IEnumerable<RedPackDetailDto> GetGuildRedPacks { get; }

    //bool isRedPackNotNull { get; }
}
public interface IRedPackData
{
    IRedPackConfirmViewData ConfirmViewData { get; }
    RedPack.RedPackType RedPackType { get; }
    IRedPackMainViewData GetRedPacketViewData { get; }
    ISendRedPackViewData SendRedPacketViewData { get; }
    RedPackInfoDto GetRedPackInfoDto { get; }
}

public sealed partial class RedPackDataMgr
{
    public sealed partial class RedPackData : IRedPackData, IRedPackMainViewData, ISendRedPackViewData, IRedPackConfirmViewData
    {
        public IRedPackMainViewData GetRedPacketViewData { get { return this; } }
        public ISendRedPackViewData SendRedPacketViewData { get { return this; } }
        public IRedPackConfirmViewData ConfirmViewData { get { return this; } }
        // 配表数据
        const int redPackNum = 50;//红包总数

        // 服务器数据
        public Dictionary<RedPackChannelType, List<RedPackDetailDto>> _redPackDic = new Dictionary<RedPackChannelType, List<RedPackDetailDto>>();
        
        // 客户端状态数据
        // 红包主界面        
        public RedPackChannelType curTabMainView;
        public RedPackChannelType CurTabMainView { get { return curTabMainView; } }
        //发送界面
        public RedPackChannelType curTab = RedPackChannelType.World;
        public RedPackChannelType CurTab { get { return curTab; } }

        public List<RedPackDetailDto> _worldRedPacks = new List<RedPackDetailDto>();
        public List<RedPackDetailDto> _guildRedPacks = new List<RedPackDetailDto>();
        public IEnumerable<RedPackDetailDto> GetWorldRedPacks { get { return _worldRedPacks; } }
        public IEnumerable<RedPackDetailDto> GetGuildRedPacks { get { return _guildRedPacks; } }

        /// <summary>
        /// 是否打开
        /// </summary>
        public RedPackDetailDto IsOpen;
        /// <summary>
        /// 是否足够
        /// </summary>
        public RedPackDetailDto IsEnough;
        /// <summary>
        /// 分页
        /// </summary>
        public int index;
        /// <summary>
        /// 红包ID
        /// </summary>
        public long redPackId;
        // 当前默认打开分页： 帮派／世界       
        // 发红包界面
        /// <summary>
        /// 当前货币类型
        /// </summary>
        public VirtualItemEnum moneyType;
        /// <summary>
        /// 当前货币值
        /// </summary>
        public int curMoneyValue;
        
        /// <summary>
        /// 玩家所在公会ID
        /// </summary>
        private PlayerGuildInfoDto playerGuildInfo = null;
        public PlayerGuildInfoDto PlayerGuildInfo
        {
            get { return playerGuildInfo; }
            private set { }
        }
        /// <summary>
        /// 口令红包或普通红包
        /// </summary>
        private RedPack.RedPackType redPackType = new RedPack.RedPackType();
        public RedPack.RedPackType RedPackType
        { get { return redPackType; }
        set { redPackType = value; } }
        
        /// <summary>
        /// 当前发送金额
        /// </summary>
        private int curSendMoney;
        public int Total { get { return curSendMoney; }  }
        /// <summary>
        /// 红包数量
        /// </summary>
        private int redPackCount;        
        public int Count { get { return redPackCount; } }
        /// <summary>
        /// 祝福或口令
        /// </summary>
        private string redPackWord;
        public string Word { get{ return redPackWord; } }
        private string redPackTitle;
        public string Title { get { return redPackTitle; } }
        private string nameTitle;
        public string NameTitle { get { return nameTitle; } }
        //红包详情
        /// <summary>
        /// 发送人
        /// </summary>
        public ShortPlayerDto fromPlayer;
        /// <summary>
        /// 最好
        /// </summary>
        public PlayerRedPackDto best;
        /// <summary>
        /// 最差
        /// </summary>
        public PlayerRedPackDto worst;
        /// <summary>
        /// 详情
        /// </summary>
        private List<PlayerRedPackDto> detail;
        private RedPackInfoDto _redPackInfoDto;
        public RedPackInfoDto GetRedPackInfoDto { get { return _redPackInfoDto; } }

        
        public List<PlayerRedPackDto> GetShowDetail { get { return detail; } }
        
        private void InitRedPackListData()
        {
           
        }
        public void SetRedPackList(int index ,RedPackChannelType channel, RedPackInfoDto info)
        {
            _redPackDic[channel] = info.detailDtos;
            this.index = index;
        }
        public void RedPackDetailFunc(List<PlayerRedPackDto> dto)
        {
            detail = dto;
        }
        
        public void InitData()
        {
        }

        public void Dispose()
        {

        }
        public void ChangeSilver(VirtualItemEnum moneyType,int count)
        {

        }
    }
}
