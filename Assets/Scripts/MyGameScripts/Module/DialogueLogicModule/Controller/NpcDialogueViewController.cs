// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  NpcDialogueViewController.cs
// Author   : DM-PC092
// Created  : 9/1/2017 7:32:14 PM
// Porpuse  : 
// **********************************************************************

using UnityEngine;
using AppDto;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Random = UnityEngine.Random;
using AppServices;
using AssetPipeline;

public partial interface INpcDialogueViewController
{
    void Open(BaseNpcInfo npcinfo,IMissionData data);
    void OpenCommonDialogueByNpc(Npc npcinfo,string content,List<string> optionList,System.Action<int> onSelect);
}
public partial class NpcDialogueViewController {
    private enum DialogueProcess
    {
        Wait = 0,
        InProgress = 1,
        TheEnd = 2
    }
    private DialogueOption _missionOptionType;
    private BaseNpcInfo _npcInfo;
    private Npc _npc;
    private SceneNpcDto _npcStateDto;
    private List<MissionOption> _missionOptionList;
    private int _dialogMsgCount = 0,_optionActiveCount;
    private bool _isOnlyMission,_currMissionCanSubmitBySelf,_talkMissionFlag = false, _finisMissionFlag=false;
    private MissionOption _currMissionOption;
    private DialogueProcess _dialogueProcess = DialogueProcess.Wait;
    private IMissionData _data;
    private MissionDialog _currMissionDialog;
    private List<int> _functionIdList;
    private List<DialogueOptionItemController> _optionItemList;
    private Dictionary<int, MissionDialogOption> _missionDialogOptionDic = null;
    private NpcDialog _dialogueInfo;
    private string mFaceID,mModelID;    //缓存的显示模型ID，如果ID相同就不重复加载
    #region -----------------------	callback about -----------------------
    public Action talkCallback=null;
    public Action<Mission> finishCallback=null;
    public Action<Npc> submitCallback=null;
    private System.Action _closeClickCallback = null;
    private List<Texture2D> bodyTexture = new List<Texture2D>();
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(_view.ClickBtnBoxCollider_UIButton.onClick,ClickDialogueBtn);
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        ClearDialogueData();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IMissionData data) {

    }

    #region 关闭对话清除NPC信息
    private void ClearDialogueData()
    {
        _npcInfo = null;
        _npc = null;
        _npcStateDto = null;
        _dialogueProcess = DialogueProcess.Wait;
        _functionIdList = null;
        _missionOptionList = null;
        _dialogueInfo = null;
        _closeClickCallback = null;
        if(_isOnlyMission)
            WorldManager.Instance.GetView().SetShowPlayerView();
        _isOnlyMission = false;
        _currMissionDialog = null;
        _currMissionOption = null;
        _currMissionCanSubmitBySelf = false;
        if(_optionItemList != null)
        {
            for(int i = 0;i < _optionItemList.Count;++i)
            {
                _optionItemList[i].Dispose();
                Destroy(_optionItemList[i].cachedGameObject);
            }
            _optionItemList.Clear();
        }
        UIHelper.DisposeUITexture(View.FaceTexture);
        mModelID = null;
        mFaceID = null;
        JSTimer.Instance.CancelCd("__TrialMissionTalkAutoRead");
    }
    #endregion

    #region 聊天面板初始化
    /// <summary>
    /// 打开聊天面板
    /// </summary>
    /// <param name="npcInfo"></param>
    /// <param name="data"></param>
    public void Open(BaseNpcInfo npcInfo,IMissionData data)
    {
        _optionItemList = new List<DialogueOptionItemController>(8);
        this._data = data;
        _npcInfo = npcInfo;
        _npcStateDto = npcInfo.npcStateDto;
        _npc = _npcStateDto.npc;
        //寻找当前NPC的身上的所有任务
        _missionOptionList = data.GetMissionOptionListByNpcInternal(_npc);
        //寻找当前NPC的身上的所有选项
        _missionOptionType = data.GetDialogueOptionByMissionOptionList(_missionOptionList,_npc);
        //判断NPC是否可以进行任务
        _isOnlyMission = MissionHelper.IsOnlyMission(_npc,_missionOptionList,_missionOptionType);
        if(_isOnlyMission)
        {
            WorldManager.Instance.GetView().SetHideOtherView();
            //if(_missionOptionList[0].mission.type == (int)MissionType.MissionTypeEnum.Urgent)
            //{
            //    NormalHandle();
            //    return;
            //}
            //进行任务聊天面板的打开
            OpenMissionOption(_missionOptionList[0]);
        }
        else
        {
            // 正常的NPC处理流程
            NormalHandle();
        }
    }
    #endregion


    #region NPC对话任务选项按钮打开
    private void OpenMissionOption(MissionOption missionOption)
    {
        _currMissionOption = missionOption;
        if(_view == null)
            return;
        //	隐藏 Option Grid
        _view.OptionBg.gameObject.SetActive(false);
        _dialogMsgCount = 0;
        _dialogueProcess = DialogueProcess.InProgress;
        Mission tMission = missionOption.mission;
        Npc tMissionNpc = _data.GetMissionNpcByMission(tMission, missionOption.isExis);
        //	对话内容
        _currMissionDialog = new MissionDialog();
        _currMissionCanSubmitBySelf = false;
        if(missionOption.isExis)
        {
            //	SK -> NPC对话引起的NPC隐藏或显示处理
            WorldManager.Instance.GetNpcViewManager().HandlerMissionNpcStatus(missionOption.mission.clickSubmitNpcStatus);
            SubmitDto tSubmitDto=MissionHelper.GetSubmitDtoByMission(tMission);
            //	判断是否达成最后一个目标（trued：提交任务并return）并关闭对话界面
            if(tSubmitDto == null)
            {
                _currMissionCanSubmitBySelf = true;
                _currMissionDialog = tMission.dialog;
            }
            else
            {
                //	获取MissionDialogue
                _currMissionDialog = tSubmitDto.dialog;
                _currMissionCanSubmitBySelf = MissionHelper.IsFinishSubmitNeedSelf(tSubmitDto);
                if(_currMissionCanSubmitBySelf&&(MissionHelper.IsShowMonster(tSubmitDto)
                                || MissionHelper.IsCollectionItem(tSubmitDto)))
                {
                    Npc tCurNpc =_data.GetMissionNpcByMission(tMission, true);
                    _currMissionCanSubmitBySelf = tCurNpc.id == _npc.id;
                }
                if(tMissionNpc == null)
                {
                    //	获取追踪Npc(这里当NPC为空，已在上面tSubmitDto == null做了处理)
                    GameDebuger.Log(string.Format("ERROR -> 当前任务ID：{0} | Name：{1}-{2}任务提交NPC为空，当执行到这里表示出错 TADO",tMission.id,tMission.missionType.name,tMission.name));
                }
            }
        }
        else
        {
            _currMissionDialog = tMission.dialog;
        }
        _closeClickCallback = HandleOnCloseClick;
        PlayMissionDialogueMsg(missionOption,_currMissionDialog,_currMissionCanSubmitBySelf);
    }

    #endregion

    #region 设置详细对话内容
    /// <summary>
    /// 设置详细对话内容 -- Plaies the mission dialogue message.
    /// </summary>
    /// <param name="missionOption"></param>
    /// <param name="missionDialogue"></param>
    /// <param name="isSubmit"></param>
    private void PlayMissionDialogueMsg(MissionOption missionOption,MissionDialog missionDialogue,bool isSubmit)
    {
        Mission mission = missionOption.mission;
        bool isBind = missionOption.isExis;
        SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(mission);
        bool tSubmitSta = isSubmit || tSubmitDto is TalkSubmitDto  || tSubmitDto is ApplyItemSubmitDto;
        //  当进行中
        if(isBind && !isSubmit)
        {
            Npc tAccNpc=_data.GetMissionNpcByMission(mission,false);
            Npc tCurNpc=_data.GetMissionNpcByMission(mission,true);
            string tMsgStr=missionDialogue.progressNpc;
            if(tCurNpc != null && _npc.id == tCurNpc.id)
            {
            }
            else if(tAccNpc != null && _npc.id == tAccNpc.id)
            {
                bool tSetDialogueContentSta = true;
                if(string.IsNullOrEmpty(tMsgStr) && _npc is NpcGeneral)
                {
                    NpcGeneral tNpcGeneral = _npc as NpcGeneral;
                    if(tNpcGeneral.dialog != null)
                    {
                        tMsgStr = tNpcGeneral.dialog.dialogContent[Random.Range(0,tNpcGeneral.dialog.dialogContent.Count)];
                    }
                    else
                    {
                        tMsgStr = string.Format("##### 任务都没做完还想领便当? #####");
                    }
                    tSetDialogueContentSta = true;
                }
                if(tSetDialogueContentSta)
                {
                    if(tMsgStr == string.Empty)
                        tMsgStr = missionDialogue.tips;
                    SetNpcDialogueContent(tMsgStr,"default");
                }
                _closeClickCallback = null;
                return;
            }
            else if(tAccNpc == null)
            {
                return;
            }
        }

        List<MissionDialogSequence> tDialogSequenceList;
        if(isBind&&tSubmitSta)
        {
            tDialogSequenceList = missionDialogue.submitDialogSequence;
        }
        else
        {
            tDialogSequenceList = missionDialogue.acceptDialogSequence;
        }


        //	当没有对话的时候直接返回
        if(tDialogSequenceList == null || tDialogSequenceList.Count <= 0)
        {
            _dialogueProcess = DialogueProcess.TheEnd;
            if(_closeClickCallback != null)
            {
                _closeClickCallback();
            }
            return;
        }

        MissionDialogSequence tMissionDialogSequence=tDialogSequenceList[_dialogMsgCount];
        string tMsg=tMissionDialogSequence.dialog;
        View.FaceTexture.gameObject.SetActive(false);
        if(tMissionDialogSequence.position == 0)
        {
            View.FaceTexture.transform.localPosition = new Vector3(-569f,-320f,0);
            View.FaceTexture.transform.localScale = Vector3.one;
        }
        else
        {
            View.FaceTexture.transform.localPosition = new Vector3(595f,-320f,0);
            View.FaceTexture.transform.localScale = new Vector3(-1,1,1);
        }
        if(tMissionDialogSequence.type == (int)MissionDialogSequence.MissionDialogSequenceType.Player)
        {
            //轮到玩家说话了
            SetPlayerDialogueContent(tMsg,tMissionDialogSequence.countenance);
        }
        else if(tMissionDialogSequence.type==(int)MissionDialogSequence.MissionDialogSequenceType.Npc)
        {
            if(tMissionDialogSequence.npc == null)
            {
                SetNpcDialogueContent(tMsg,tMissionDialogSequence.countenance);
            }
        }
        else if(tMissionDialogSequence.type==(int)MissionDialogSequence.MissionDialogSequenceType.OtherNpc)
        {
            Npc tCurNpc=tMissionDialogSequence.npc;
            tCurNpc = tCurNpc == null ? DataCache.getDtoByCls<Npc>(tMissionDialogSequence.npcId) : tCurNpc;
            if(tCurNpc == null)
            {
                tCurNpc = new Npc();
                tMsg += string.Format("{0} | MissionDialogSequence NpcID:{1} | Current DialogueCount:{2}-Type:{3}",
                                      "\n第三者NpcID填写错误~!!",
                                      tMissionDialogSequence.npcId,
                                      _dialogMsgCount,tMissionDialogSequence.type);
            }
            else
            {
                tCurNpc = _data.NpcVirturlToEntity(tCurNpc);
            }
            SetupFaceInfo("npc_"+tCurNpc.modelId,tMissionDialogSequence.countenance,tMsg,tCurNpc.name);
        }

        if(++_dialogMsgCount >= tDialogSequenceList.Count)
        {
            _dialogueProcess = DialogueProcess.TheEnd;
        }
        if(missionOption.mission.type == (int)MissionType.MissionTypeEnum.Faction && _isOnlyMission) 
        {
            JSTimer.Instance.SetupCoolDown("__TrialMissionTalkAutoRead",5,null,ClickDialogueBtn);
        }
    }


    private void SetNpcDialogueContent(string content,string countenance)
    {
        if(_npc == null || _npcInfo == null || _npcStateDto == null || _npcStateDto.npc == null)
            return;
        string modelId = _npcInfo.modelId.ToString();
        if(countenance.Contains("default"))
        {
            SetupFaceInfo("npc_"+modelId,modelId,content,_npcInfo.name);
        }
        else
        {
            SetupFaceInfo("npc_"+modelId,countenance,content,_npcInfo.name);
        }
    }


    /// <summary>
    /// 设置对话时候的模型显示
    /// </summary>
    /// <param name="name">说话人名字</param>
    /// <param name="msg">话说内容</param>
    /// <param name="faceID"></param>
    /// <param name="inBlack"></param>
    /// <param name="pDiglogFaceID"></param>
    private void SetupFaceInfo(string modelId,string faceID,string msg,string NpcName)
    {
        //&& mFaceID != faceID && && mFaceID != string.Empty
        if(mModelID != modelId && modelId != String.Empty)
        {
            ResourcePoolManager.Instance.LoadImage(modelId,asset =>
            {
                if(asset != null)
                {
                    var tex = asset as Texture2D;
                    UIHelper.DisposeUITexture(View.FaceTexture);
                    View.FaceTexture.mainTexture = tex;
                    View.FaceTexture.MakePixelPerfect();
                    View.FaceTexture.gameObject.SetActive(true);
                }
            });
            mModelID = modelId;
            mFaceID = faceID;
        }
        else
        {
            View.FaceTexture.gameObject.SetActive(true);
        }
        View.NameLabel_UILabel.text = NpcName;
        View.NameLabel_UILabel.gameObject.SetActive(true);
        SetNpcDialogMsg(msg);
        _isChangePlayer = false;
    }


    /// <summary>
    /// 设置具体NPC对话显示
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="faceID"></param>
    private void SetNpcDialogMsg(string msg)
    {
        string tStr = Regex.Replace(msg, @"\\n", "\n");

        //特殊处理动作标签,格式 [action=show]
        string actionPattern = @"\[action=([^\]]+)\]";
        Match actionMatch = Regex.Match(tStr, actionPattern);
        if(actionMatch.Success)
        {
            tStr = Regex.Replace(tStr,actionPattern,"");
            string actionParam = actionMatch.Groups[1].ToString();
        }

        //特殊处理玩家名称标签,格式    [playername]
        string playernamePattern = @"\[playername]";
        Match playernameMatch = Regex.Match(tStr, playernamePattern);
        if(playernameMatch.Success)
        {
            tStr = Regex.Replace(tStr,playernamePattern,ModelManager.Player.GetPlayerName().WrapColor(ColorConstantV3.Color_Green_Str));
        }

        //特殊处理玩家性别标签,格式  [sexname=男,女]
        string sexnamePattern = @"\[sexname=([^\]]+)\]";
        Match sexnameMatch = Regex.Match(tStr, sexnamePattern);
        if(sexnameMatch.Success)
        {
            string sexnameParam = sexnameMatch.Groups[1].ToString();
            string[] sexnameParamList = sexnameParam.Split(',');
            string sexname = "";
            if(sexnameParamList.Length == 2)
            {
                //1是男，0是女
                sexname = (ModelManager.Player.GetPlayerGender() == 1) ? sexnameParamList[0] : sexnameParamList[1];
            }

            tStr = Regex.Replace(tStr,sexnamePattern,sexname);
        }

        //_emojiLbl.SetEmojiText(tStr);
        _view.MsgLabel_UILabel.text = tStr;
    }

    protected virtual void InitDialogueContent()
    {
        string content = "";
        if(_dialogueInfo != null && _dialogueInfo.dialogContent.Count > 0)
        {
            content = _dialogueInfo.dialogContent.Random();
        }
        SetNpcDialogueContent(content,"default");
    }
    #endregion

    #region 点击对话内容
    private void ClickDialogueBtn()
    {
        if(_view != null && _closeClickCallback != null)
        {
            _closeClickCallback();
        }
        else
        {
            if(JSTimer.Instance.IsCdExist("__TrialMissionTalkAutoRead"))
            {
                JSTimer.Instance.CancelCd("__TrialMissionTalkAutoRead");
            }
            ProxyNpcDialogueView.Close();
        }
    }
    #endregion

    #region 设置NPC聊天的时候轮到自己说话
    /// <summary>
    /// 设置角色自己说话
    /// </summary>
    /// <param name="npcMsg"></param>
    private bool _isChangePlayer=false;
    private void SetPlayerDialogueContent(string npcMsg,string dialogFunctionIds)
    {
        _isChangePlayer = true;
        // 特殊处理,要设置玩家染色数据
        //ModelController.SetupModel(ModelManager.Player.GetPlayer().charactor,true,ModelManager.Player.TransformModelId);
        //ModelController.CleanUpCustomAnimations();
        //View.NameLabel_UILabel.text = ModelManager.Player.GetPlayerName();
        //View.NameLabel_UILabel.gameObject.SetActive(true);
        //SetNpcDialogMsg(npcMsg);
        SetupFaceInfo("npc_"+ ModelManager.Player.GetPlayer().charactor.modelId,dialogFunctionIds,npcMsg,ModelManager.Player.GetPlayerName());
    }
    #endregion

    #region 没有任务的NPC点击事件
    //这里可以被子类重载，用来定制子类的NPC对话，如首席弟子
    private void NormalHandle()
    {
        if(_npc is NpcGeneral)
        {
            NpcGeneral npcGeneral=_npc as NpcGeneral;
            _functionIdList = new List<int>(npcGeneral.dialogFunctionIds);
            _dialogueInfo = npcGeneral.dialog;
        }
        //根据配表生成NPC的NPC列表（可以走动的）
        else if(_npc is NpcVariable)
        {
            if(_npc is NpcSceneMonster)
            {
                NpcSceneMonster npcSceneMonster = _npc as NpcSceneMonster;
                if(npcSceneMonster.npcAppearance != null)
                {
                    npcSceneMonster.modelId = npcSceneMonster.npcAppearance.modelId;
                    npcSceneMonster.model = npcSceneMonster.npcAppearance.model;
                }
                _functionIdList = new List<int>(npcSceneMonster.dialogFunctionIdStr.Count);
                for(int i = 0;i < npcSceneMonster.dialogFunctionIdStr.Count;++i)
                {
                    DialogFunction dialogFunction = DataCache.getDtoByCls<DialogFunction>(npcSceneMonster.dialogFunctionIdStr[i]);
                    if(dialogFunction != null)
                    {
                        //观战的typeID是8
                        if(dialogFunction.type == 6) {
                            if(_npcStateDto.battleId != 0)
                                _functionIdList.Add(npcSceneMonster.dialogFunctionIdStr[i]);
                        }
                        else
                        {
                            _functionIdList.Add(npcSceneMonster.dialogFunctionIdStr[i]);
                        }
                    }
                }
                _dialogueInfo = npcSceneMonster.dialog;
            }
            else if(_npc is NpcMonster)
            {
                if(_npc.type == (int)Npc.NpcType.Tower)
                {
                    TowerDataMgr.TowerNetMsg.TowerChallenge(TowerDataMgr.DataMgr.CurTowerId, _npc.id);
                    ProxyNpcDialogueView.Close();
                    return;
                }
            }
            else
            {
                if(_missionOptionList.Count == 0)
                {
                    ProxyNpcDialogueView.Close();
                    return;
                }
            }
        }
        SetupDialogueInfo();
    }
    #endregion

    #region 用于生成右边NPC对话子菜单的代码逻辑
    /// <summary>
    /// 用于生成右边NPC对话子菜单的代码逻辑
    /// </summary>
    protected virtual void SetupDialogueInfo()
    {
        bool isShowFaction = true;
        int functionOptionCount = _functionIdList != null ? _functionIdList.Count : 0;
        if(_missionOptionList == null)
        {
            _missionOptionList = new List<MissionOption>();
        }
        int allOptionCount = _missionOptionList.Count + functionOptionCount;
        _view.OptionBg.gameObject.SetActive(allOptionCount > 0);
        if(allOptionCount > 0)
        {
            RebuildOptionItemList(allOptionCount);

            if(_missionOptionList.Count > 0)
            {
                for(int i = 0;i < _missionOptionList.Count;++i)
                {
                    MissionOption tMissionOption = _missionOptionList[i];
                    Mission tMission = tMissionOption.mission;
                    SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(tMission);
                    //bool tIsLastFindMissionSta = ModelManager.MissionData.lastFindMissionID == tMission.id;
                    //    //	当历练任务，特殊处理一下 Bengin
                    //    if(tIsLastFindMissionSta && ModelManager.MissionData.IsTrialMissionAndTalkTarget(tMission,tSubmitDto))
                    //    {
                    //        OpenMissionOption(tMissionOption);
                    //        return;
                    //    }
                    //    //	End
                    //string optionName = MissionHelper.GetMissionTitleNameInDialogue(tMission, tMissionOption.isExis);
                    //显示任务信息
                    _optionItemList[i].SetData(i,tMission.name,OnSelectMissionOption,false);
                }
            }

            if(functionOptionCount > 0)
            {
                int openCount = 0;
                _data.GetMissionListDto.ForEach(e =>
                {
                    if(e.mission.type == (int)MissionType.MissionTypeEnum.Faction)
                    {
                        isShowFaction = false;
                    }
                });
                for(int i = 0;i < functionOptionCount;++i)
                {
                    //如果身上有委托任务，且NPC绑定了静态的委托选项就不显示
                    if(!isShowFaction && _functionIdList[i] == 4) { continue; }
                    //四轮之塔塔层Npc判断 如果没有全部通关关卡，则不显示传送下一层
                    if (!TowerDataMgr.DataMgr.IsTowerClear && _functionIdList[i] == 22) continue;

                    DialogFunction functionInfo = DataCache.getDtoByCls<DialogFunction>(_functionIdList[i]);
                    if (functionInfo != null && FunctionOpenHelper.isFuncOpen(functionInfo.functionOpenId,false))
                    {
                        int itemIndex = _missionOptionList.Count + openCount;
                        //string optionName = GetFunctionOptionName(functionInfo);
                        _optionItemList[itemIndex].SetData(i,functionInfo.name,OnSelectFunctionOption,false,false,functionInfo.type);
                        openCount++;
                    }
                }

                if(allOptionCount != _missionOptionList.Count + openCount)
                {
                    allOptionCount = _missionOptionList.Count + openCount;
                    RebuildOptionItemList(allOptionCount);
                    _view.OptionBg.gameObject.SetActive(allOptionCount > 0);
                }
            }
            UpdateOptionBgWidth();
        }

        InitDialogueContent();
    }

    /// <summary>
    /// 更新选项栏的高度
    /// </summary>
    private void UpdateOptionBgWidth()
    {
        UISprite OptionBgSprite= View.OptionBg.GetComponent<UISprite>();
        OptionBgSprite.topAnchor.absolute = 17 + _optionActiveCount * 44 + OptionBgSprite.bottomAnchor.absolute * _optionActiveCount;
        OptionBgSprite.UpdateAnchors();
    }

    protected void RebuildOptionItemList(int newCount)
    {
        _optionActiveCount = newCount;
        if(_optionItemList.Count < newCount)
        {
            for(int i = _optionItemList.Count;i < newCount;++i)
            {
                GameObject item = i == 0 ? View.FunctionCell : NGUITools.AddChild(View.FunctionGrid_UIGrid.gameObject, View.FunctionCell);
                DialogueOptionItemController com = item.GetMissingComponent<DialogueOptionItemController>();
                com.InitItem();
                _optionItemList.Add(com);
            }
        }

        for(int i = 0;i < newCount;++i)
        {
            _optionItemList[i].cachedGameObject.SetActive(true);
        }

        //隐藏多余选项
        for(int i = newCount;i < _optionItemList.Count;++i)
        {
            _optionItemList[i].cachedGameObject.SetActive(false);
        }

        View.FunctionGrid_UIGrid.Reposition();
    }


    #endregion

    #region 点击生成NPC子选项的回调事件
    /// <summary>
    /// 这个是配在NPC里面的菜单选项
    /// </summary>
    /// <param name="index"> 点击生成NPC子选项的回调事件</param>
    protected virtual void OnSelectFunctionOption(int index)
    {
        if(_functionIdList != null && index < _functionIdList.Count)
        {
            DialogFunction dialogFunction=DataCache.getDtoByCls<DialogFunction>(_functionIdList[index]);
            bool needClose=true;
            if(dialogFunction != null) {
                //if(dialogFunction.type == 7) {
                //捉鬼时候需要特殊判断
                //_data.GetMissionTypeGhostDelegateSubmit().AcceptMission(_npcInfo);
                //_data.Accpet(_currMissionOption.mission);
                //needClose = false;
                //}
                //else {
                //}
                needClose = DialogueHelper.OpenDialogueFunction(_npcStateDto,dialogFunction);
            }
            if(needClose)
                ProxyNpcDialogueView.Close();
        }
    }

    /// <summary>
    /// 这个服务器传过来的任务NPC子选项（多选项功能可以在这边入手考虑）
    /// </summary>
    /// <param name="index"></param>
    private void OnSelectMissionOption(int index)
    {
        if(_missionOptionList != null && index < _missionOptionList.Count)
        {
            if(_missionOptionList[index].isExis)
            {
                //NPC对话任务判断
                OpenMissionOption(_missionOptionList[index]);
            }
            else{
                MissionDataMgr.DataMgr.AcceptMission(_missionOptionList[index].mission);
                ProxyNpcDialogueView.Close();
            }
        }
        else
        {
            UIModuleManager.Instance.CloseModule(NpcDialogueView.NAME);
        }
    }
    #endregion


    #region 点击聊天面板的回调或者计时器的自动回调
    //四轮之塔的处理
    private void HandleTowerClick()
    {
        //Todol
    }
    //点击回调
    private void HandleOnCloseClick()
    {
        MissionOption missionOption=_currMissionOption;
        Mission tMission=missionOption.mission;
        switch(_dialogueProcess)
        {
            case DialogueProcess.Wait:
                break;
            case DialogueProcess.InProgress:
                //如果是任务状态就继续对话
                PlayMissionDialogueMsg(missionOption,_currMissionDialog,_currMissionCanSubmitBySelf);
                break;
            case DialogueProcess.TheEnd:
                _dialogueProcess = DialogueProcess.Wait;
                _closeClickCallback = null;
                if(_isOnlyMission)
                {
                    if(missionOption.isExis)
                    {
                        SubmitDto tSubmitDto=MissionHelper.GetSubmitDtoByMission(tMission);
                        //是不是自动提交任务
                        if((tSubmitDto==null&&!tMission.autoSubmit)||MissionHelper.IsFinishSubmitNeedSelf(tSubmitDto))
                        {
                            //GameDebuger.Log("完成任务，需要接受下个任务");
                            // 表示该任务可提交（非目标提交）
                            FinishNpcTargetSubmitDto(tMission,_npc,_npcInfo.submitIndex);
                        }
                        else
                        {
                            //GameDebuger.Log("完成任务，需要接受下个任务2");
                            HandleOpenMissionOptionCallBack();
                        }
                    }
                }
                else
                {
                    if(missionOption.isExis)
                    {
                        MissionDataMgr.DataMgr.FinishTargetSubmitDto(tMission,_npc,_npcInfo.submitIndex);
                        //	执行顺序（后） ProxyDialogueModule.Close 会情况 _npc 数据
                        ProxyNpcDialogueView.Close();
                    }
                    else
                    {
                        //MissionDataMgr.GetPlayerMissionDtoByMissionID
                        //	执行顺序（先）
                        if(MissionHelper.IsBuffyMissionType(missionOption.mission))
                        {
                            //_data.GetMissionTypeGhostDelegateSubmit().AcceptMission();
                        }
                        else
                        {
                            MissionDataMgr.DataMgr.AcceptMission(missionOption.mission);
                        }
                        ProxyNpcDialogueView.Close();
                    }
                }
                break;
        }
    }
    #endregion

    #region 获取任务提交具体条件（返回未完成的那一个目标,可提交任务时返回空）

    private void HandleOpenMissionOptionCallBack()
    {
        Mission mission=_currMissionOption.mission;
        List<int> tDialogOption=GetMissionDialogOptionsByMision(_currMissionOption.mission);
        int optionCount=tDialogOption.Count;
        // 处理是否直接进入任务流程,有对话,对话
        if(optionCount>0)
        {
            _view.OptionBg.gameObject.SetActive(true);
            MissionDialog missionDialog=MissionHelper.GetMissionSubmitDtoDialog(mission,_npcInfo.submitIndex);
            SetNpcDialogueContent(missionDialog.tips,"default");
            RebuildOptionItemList(optionCount);
            Action<int> initAction = i =>
            {
                int optionId = tDialogOption[i];
                MissionDialogOption dialogOption = GetMissionOptionByOptionID(optionId);
                string optionName = dialogOption.name;

                _optionItemList[i].SetData(i, optionName, index =>
                {
                    MissionDialogOptionFunctionOpen(mission, dialogOption, _npc, _npcInfo.submitIndex);
                    ProxyNpcDialogueView.Close();
                }, i == 0 /*&& ModelManager.MissionData.lastFindMissionID == _missionOptionList[i].mission.id*/);
            };

            for(int i = 0;i < optionCount;++i)
            {
                initAction(i);
                UpdateOptionBgWidth();
            }
        }
        else
        {
            //	直接执行接下来的任务情况（判断是否需要对话 \ 战斗 -> 对话\战斗结束后操作）
            FinishNpcTargetSubmitDto(mission,_npc,_npcInfo.submitIndex);
        }
    }


    private void FinishNpcTargetSubmitDto(Mission mission,Npc npc,int submitInde)
    {
        //GameDebuger.OrangeDebugLog(">>>>> FinishNpcTargetSubmitDto");
        //ModelManager.MissionData.FinishTargetSubmitDto(mission,npc,submitInde);
        MissionDataMgr.DataMgr.FinishTargetSubmitDto(mission,npc,submitInde);
        ProxyNpcDialogueView.Close();
        if(mission.type == (int)MissionType.MissionTypeEnum.Faction) {
            PlayerMissionDto tPlayerMissionDto = MissionDataMgr.DataMgr.GetPlayerMissionDtoByMissionID(mission.id);
            PlayerFactionMissionDto tPlayerFactionMissionDto = tPlayerMissionDto as PlayerFactionMissionDto;
            if(tPlayerFactionMissionDto.curRings >= tPlayerFactionMissionDto.ringCount)
            {
                EveryDayMissionDataMgr.EveryDayMissionPanelLogic.Open();
            }
        }
    }

    #endregion

    #region 取得该任务的对话选项
    //取得该任务的对话选项
    private List<int> GetMissionDialogOptionsByMision(Mission tMission)
    {
        if(tMission==null)
        {
            return new List<int>();
        }

        return tMission.dialogOptions;
    }
    #endregion

    #region 根据OptionID获取对话功能
    private MissionDialogOption GetMissionOptionByOptionID(int optionID)
    {
        Dictionary<int,MissionDialogOption> tMissionOptionDic=GetMissionDialogOptionDic();
        MissionDialogOption tMissionDialogOption=null;
        if(tMissionOptionDic != null && tMissionOptionDic.ContainsKey(optionID))
        {
            tMissionDialogOption = tMissionOptionDic[optionID];
        }
        return tMissionDialogOption;
    }
    #endregion

    #region 获取对话功能选项(静态表)
    private Dictionary<int,MissionDialogOption> GetMissionDialogOptionDic()
    {
        if(_missionDialogOptionDic == null)
        {
            _missionDialogOptionDic=DataCache.getDicByCls<MissionDialogOption>();
        }
        return _missionDialogOptionDic;
    }
    #endregion

    #region MissionDialogOption 对话功能选项(Mark一下，每次找不到)
    private void MissionDialogOptionFunctionOpen(Mission mission,MissionDialogOption missionDialogOption,Npc npc,int submitIndex)
    {
        //string tLabelStr=missionDialogOption.name;

        //	任务类型
        //bool tIsChainType = MissionHelper.IsTaskChainMissionType(mission);    连环任务
        // bool tIsGoodType = MissionHelper.IsGoodMissionType(mission);      美食任务
        //生成对应的选项调回调方法
        switch(missionDialogOption.type)
        {
            case (int)MissionDialogOption.DialogOptionEnum.Unknown:
                break;
            case (int)MissionDialogOption.DialogOptionEnum.MakeWar:
                //PlayerMissionDto tPlayerMissionDto = MissionDataMgr.DataMgr.GetPlayerMissionDtoByMissionID(mission.id);
                MissionDataMgr.DataMgr.FinishTargetSubmitDto(mission,npc,submitIndex);
                break;
            case (int)MissionDialogOption.DialogOptionEnum.MissionMulti:
                //分线任务
                MissionDataMgr.DataMgr.MultiAcceptMission(mission.id,missionDialogOption.id);
                break;
            //case 
        }
    }
    #endregion

    #region
    public void OpenCommonDialogueByNpc(Npc npcinfo,string content,List<string> optionList,System.Action<int> onSelect)
    {
        //OnDispose();
        bool showOption = optionList != null && optionList.Count > 0;
        _view.OptionBg.gameObject.SetActive(showOption);
        SetupFaceInfo("npc_" + npcinfo.modelId,npcinfo.modelId.ToString(),content,npcinfo.name);
    }
    #endregion

}



#region NPC选项的生成子菜单类
public class DialogueOptionItemController:MonoController
{
    private UIButton _button;
    private UISprite _buttonBg;
    private UILabel _btnLbl;
    private SurroundUIEffect _surroundEffect;
    private bool _slyteSta;

    public int LabelWidth
    {
        get { return _btnLbl.width; }
    }

    public void SetBtnLbl(string str)
    {
        _btnLbl.text = str;
        _btnLbl.text = str.WrapColor(ColorConstantV3.Color_White_Str);
        _buttonBg.spriteName = _slyteSta ? "green-button-004" : "button-004";
    }

    private int _index;
    private System.Action<int> _onSelect;
    private GameObject _mGo;

    public GameObject cachedGameObject
    {
        get { return _mGo; }
    }

    private Transform _mTrans;

    public Transform cachedTransform
    {
        get { return _mTrans; }
    }

    public void InitItem()
    {
        _mGo = this.gameObject;
        _mTrans = this.transform;
        _btnLbl = _mGo.GetComponentInChildren<UILabel>();
        _button = _mGo.GetMissingComponent<UIButton>();
        _buttonBg = _button.GetComponentInChildren<UISprite>();
        EventDelegate.Set(_button.onClick,OnClickBtn);
    }

    public void SetData(int index,string text,System.Action<int> onSelect,bool slyteSta = false,bool isEffect = false,int dialogType = 0)
    {
        _index = index;
        _slyteSta = slyteSta;
        SetText(dialogType, text);
        _onSelect = onSelect;
        //_buttonBg.spriteName = slyteSta ? "green-button-004" : "button-004";
        _buttonBg.spriteName = slyteSta ? "dropbox_btn_name2" : "dropbox_btn_name2";

        if(isEffect)
        {
            //_surroundEffect = SurroundUIEffect.Begin(BaseGuideLogic.EFFECT_NAME_2,_buttonBg,4f);
        }
    }

    private void SetText(int dialogType,string text)
    {
        switch (dialogType)
        {
            case (int)DialogueHelper.DialogType.EnterTower:
                _btnLbl.text = string.Format(text, TowerDataMgr.DataMgr.TowerName).WrapColor(ColorConstantV3.Color_White_Str);
                break;
            case (int)DialogueHelper.DialogType.ResetTower:
                _btnLbl.text = string.Format(text, TowerDataMgr.DataMgr.ResetCount).WrapColor(ColorConstantV3.Color_White_Str);
                break;
            default:
                _btnLbl.text = text.WrapColor(ColorConstantV3.Color_White_Str);
                break;
        }
    }

    public void UpdateAnchors()
    {
        _buttonBg.UpdateAnchors();
    }

    public void SetGrey(bool active)
    {
        _button.sprite.isGrey = active;
    }

    private void OnClickBtn()
    {
        if(_onSelect != null)
            _onSelect(_index);
    }

    public override void Dispose()
    {
        if(_surroundEffect != null)
            _surroundEffect.Dispose();
        base.Dispose();
    }
}
#endregion

