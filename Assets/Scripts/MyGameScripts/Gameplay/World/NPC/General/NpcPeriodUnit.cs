public class NpcPeriodUnit : GeneralUnit {

//	private DateTime _beginTime;
//	private DateTime _endTime;
//	private bool _isLoadModel = false;

//	public override void Load ()
//	{
//		SetUnitActive(WorldManager.Instance.GetModel().GetSceneNpcShowState(_npcInfo.npcStateDto.npcId));
//		base.Load();
//	}

	protected override void AfterInit ()
	{
		base.AfterInit ();
		//SetUnitActive(WorldManager.Instance.GetModel().GetSceneNpcShowState(_npcInfo.npcStateDto.npcId));
	}

//	private void LoadModel(){
//		//不在npc活动时间内,不加载模型
//		if(!_isLoadModel){
//			_isLoadModel = true;
//			base.Load();
//		}
//	}

//	//初始化npc活动时间信息
//	private void SetupActiveTimeInfo(){
//		NpcPeriod npcPeriod = _npc as NpcPeriod;
//		DateTime dateTime = SystemTimeManager.Instance.GetServerTime();
//		for(int i=0;i<npcPeriod.dayOfWeek.Count;++i){
//			//协议中(周日=1)，DateTime中DayOfWeek枚举(周日=0)
//			int dayOfWeek = npcPeriod.dayOfWeek[i] - 1;
//			if(dayOfWeek == (int)dateTime.DayOfWeek){
//				if(DateTime.TryParse(npcPeriod.beginTime[i],out _beginTime)
//				   && DateTime.TryParse(npcPeriod.endTime[i],out _endTime)){
//					OnSystemTimeChange(-1);
//					SystemTimeManager.Instance.OnSystemTimeChange -= OnSystemTimeChange;
//					SystemTimeManager.Instance.OnSystemTimeChange += OnSystemTimeChange;
//					return;
//				}
//			}
//		}
//
//		SetUnitActive(false);
//	}
//
//	private void OnSystemTimeChange(long unixTimeStamp){
//		DateTime curDateTime = SystemTimeManager.Instance.GetServerTime();
//		if(curDateTime >= _beginTime && curDateTime <= _endTime){
//			SetUnitActive(true);
//			LoadModel();
//		}else{
//			SetUnitActive(false);
//		}
//	}
//
//	override public void Destroy() {
//		SystemTimeManager.Instance.OnChangeNextDay -= SetupActiveTimeInfo;
//		SystemTimeManager.Instance.OnSystemTimeChange -= OnSystemTimeChange;
//		base.Destroy();
//	}
}
