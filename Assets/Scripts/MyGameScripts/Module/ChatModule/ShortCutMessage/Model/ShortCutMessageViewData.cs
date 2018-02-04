// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Cilu
// Created  : 11/14/2017 10:31:50 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public interface IShortCutMessageViewData
{
    ShortCutMessageEnum CurSelect { get; }
    bool isEdit { get; } //是在在编辑状态下
    List<ShortCutMessage> shortCutMessageList { get; }
}
public class ShortCutMessage
{
    public int Id;
    public string content;
    public ChatShortCutType shortCutType;
}
public enum ChatShortCutType
{
    System,//系统
    Custom,//自定义
    Empty,//空的，等待输入
}
public enum ShortCutMessageEnum
{
    ShorCut, //快捷短语
    PlayerAnim, //角色动作
}

public sealed partial class ShortCutMessageViewDataMgr
{
    public sealed partial class ShortCutMessageViewData:IShortCutMessageViewData
    {
        public ShortCutMessageEnum CurSelect { get; set; }

        //所有的快捷短语
        private List<ShortCutMessage> _shortCutMessageList = new List<ShortCutMessage>();
        public List<ShortCutMessage> shortCutMessageList
        {
            get
            {
                return _shortCutMessageList;
            }
        }
        public bool isEdit { get; set; }  

        public void InitData()
        {
            CurSelect = ShortCutMessageEnum.ShorCut;
            isEdit = false;

            var systemChatPhrase = DataCache.getArrayByCls<ChatPhrase>();
            systemChatPhrase.ForEach(x => AddShortCutMessage(new ShortCutMessage() { Id = x.id, content = x.chatContent, shortCutType = ChatShortCutType.System },false));
            //_shortCutMessageList.Add(new ShortCutMessage { Id = 1, content = "我爱你啊", shortCutType = ChatShortCutType.System });
            //_shortCutMessageList.Add(new ShortCutMessage { Id = 2, content = "我爱啊2", shortCutType = ChatShortCutType.System });
            //_shortCutMessageList.Add(new ShortCutMessage { Id = 3, content = "我爱你啊4", shortCutType = ChatShortCutType.System });
            //_shortCutMessageList.Add(new ShortCutMessage { Id = 4, content = "我不爱你啊", shortCutType = ChatShortCutType.System });
            //_shortCutMessageList.Add(new ShortCutMessage { Id=-1, content = "", shortCutType = ChatShortCutType.Empty });
        }

        /// <summary>
        /// 增加自定义的快捷短语
        /// </summary>
        public void AddShortCutMessage(ShortCutMessage message,bool isCustom)
        {
            _shortCutMessageList.Add(message);
            if(isCustom)
            {
                _shortCutMessageList.Add(new ShortCutMessage { Id = -1, content = "", shortCutType = ChatShortCutType.Empty });
            }
        }

        //删除快捷短语
        public void DelateShortCutMessage(int id)
        {
            var index= _shortCutMessageList.FindIndex(x => x.Id == id);
            _shortCutMessageList.RemoveAt(index);
        }

        public void Dispose()
        {

        }
    }
}
