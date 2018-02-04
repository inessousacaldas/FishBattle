
using UnityEngine;
using AppServices;
using AppDto;

public class NpcSceneCampWarPeachBoxUnit : TriggerNpcUnit
{
	private const string _coolDownName = "__OpenNpcSceneCampWarPeachBox";
	private const string _openingBoxStr = "正在采集{0}......";
	private const string _minScordStr = "活动积分不足{0}，{1}不愿意跟你走！";
	private const string _openedStr = "{0}正在被他人采集";

	public override void DoTrigger()
	{
		waitingTrigger = false;
		touch = false;
		
		ShowOpening();
	}
	
	private void ShowOpening()
	{
		WorldModel worldModel = WorldManager.Instance.GetModel();
        if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
		{
			TipManager.AddTip(string.Format(_openedStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str)));
		} else {
			GameDebuger.TODO(@"CampWarPlayerDto tInfoDto = ModelManager.CampWarData.GetCampWarPlayerInfoDto();

            int tPickScore = (_npcInfo.npcStateDto.npc as NpcCampWarPeach).pickScore;
            if (tInfoDto != null && tInfoDto.score >= tPickScore) {
                ModelManager.CampWarData.CampWarOpenBox(_npcInfo.npcStateDto, OnOpenBoxSuccessCallback);
            } else {
                TipManager.AddTip(string.Format(_minScordStr, tPickScore.WrapColor(ColorConstantV3.Color_Green_Str), _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str)));
            }");
		}
	}

	/*
	private void OpenBoxLogic() {
		MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");
		
		WorldModel worldModel = WorldManager.Instance.GetModel();
		if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
		{
			TipManager.AddTip(string.Format(_openedStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str)));
		} else {
			ModelManager.CampWarData.CampWarOpenBox(_npcInfo.npcStateDto);
		}
	}
	*/
	
	private void CancelOpen()
	{
		TipManager.AddTip(string.Format("你放弃了{0}", _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str)));
		JSTimer.Instance.CancelCd(_coolDownName);
	}
	
	private void OnOpenBoxSuccessCallback() {
		string tMsg = string.Format(_openingBoxStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str));
		GameDebuger.TODO(@"MainUIViewController.Instance.SetMissionUsePropsProgress(true, tMsg);//, CancelOpen);

        JSTimer.Instance.SetupCoolDown(_coolDownName, 3f, (remainTime) => {
            MainUIViewController.Instance.SetMissionUsePropsProgress(1 - remainTime / 3f);
        }, () => {
            MainUIViewController.Instance.SetMissionUsePropsProgress(false, "");
            
            /*
                WorldModel worldModel = WorldManager.Instance.GetModel();
                if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
                {
                    TipManager.AddTip(string.Format(_openedStr, _npcInfo.name.WrapColor(ColorConstantV3.Color_Green_Str)));
                } else {
                    ModelManager.CampWarData.CampWarOpenBox(_npcInfo.npcStateDto);
                }
                */
        }, 0);");
	}
	
	protected override bool NeedTrigger()
	{
		return true;
	}
	
	protected override void SetupBoxCollider()
	{
		if (_unitGo != null)
		{
			_boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
			_boxCollider.isTrigger = true;
			_boxCollider.center = new Vector3(0f, 0.35f, 0f);
			_boxCollider.size = new Vector3(1f, 0.7f, 0.7f);
			_unitGo.tag = GameTag.Tag_Npc;
		}
		else
		{
			Debug.LogError("!!!!!! _unitGo = null");
		}
	}

	protected override void AfterInit()
	{
		base.AfterInit();
		
		InitPlayerName();
	}

	public override void Destroy() {
		base.Destroy();
	}
}
