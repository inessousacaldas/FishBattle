using UnityEngine;
using UnityEditor;
using System.Collections;
using GamePlot;
using LITJson;
using System.IO;

public class GamePlotEditor : EditorWindow
{
	[MenuItem ("Window/GamePlotEditor  #&g")]
	public static void  ShowWindow ()
	{     
		var window = EditorWindow.GetWindow<GamePlotEditor> (false, "GamePlotEditor", true);
		window.minSize = new Vector2 (800, 500);
		window.Init();
		window.Show ();
	}

	private GamePlotInfo _curPlotInfo = new GamePlotInfo();
	private GUIStyle tabButtonStyle;
	private float _adjustTimePoint;
	private bool _isFront;
	private float _timeOffset;

	private void Init(){
		tabButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
		tabButtonStyle.fontSize = 14;
		tabButtonStyle.margin = new RectOffset(5,5,5,5);
	}

	private void Dispose(){
		SelectedAction = null;
		SelectedEntity = null;
	}

	void OnGUI ()
	{
		//剧情菜单
		DrawPlotMenu ();
		EditorGUILayout.Space();

		//参数设置区域
		DrawPropertyPanel();
		EditorGUILayout.Space();

		//时间轴区域
		DrawTimelinePanel();
	}

	#region PlotMenu
	private void DrawPlotMenu ()
	{
		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("创建剧情", GUILayout.Width (100f), GUILayout.Height (50f))) {
				CreatePlot ();
			}

			if (GUILayout.Button ("加载剧情", GUILayout.Width (100f), GUILayout.Height (50f))) {
				LoadPlot ();
			}

			if (GUILayout.Button ("保存剧情", GUILayout.Width (100f), GUILayout.Height (50f))) {
				SavePlot ();
			}

			EditorGUILayout.BeginVertical();
			if (_curPlotInfo != null) {
				EditorGUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 60f;
				_curPlotInfo.plotId = EditorGUILayout.IntField ("剧情ID:", _curPlotInfo.plotId);
				_curPlotInfo.sceneId = EditorGUILayout.IntField ("场景ID:", _curPlotInfo.sceneId);
				_curPlotInfo.plotTime = EditorGUILayout.FloatField ("总时长:", _curPlotInfo.plotTime);
				EditorGUILayout.EndHorizontal();
			}else{
				GUI.color = Color.red;
				GUILayout.Label("当前没有剧情数据，可创建剧情或加载剧情后继续编辑");
				GUI.color = Color.white;
			}

			GUILayout.Space(5f);
			EditorGUILayout.BeginHorizontal();
			int interval = EditorGUILayout.IntField ("时间段数:", _timeLineInterval);
			_timeLineInterval = Mathf.Clamp(interval,1,int.MaxValue);

			_adjustTimePoint = EditorGUILayout.FloatField ("调整点:", _adjustTimePoint);
			_isFront = EditorGUILayout.ToggleLeft(_isFront?"之前":"之后",_isFront,GUILayout.Width(40f));
			_timeOffset = EditorGUILayout.FloatField ("加减:", _timeOffset);
			if(GUILayout.Button("Do")){
				OffsetTimeLine();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}
		GUILayout.EndHorizontal ();
	}

	private void CreatePlot ()
	{
		Dispose();
		_curPlotInfo = new GamePlotInfo ();

		this.ShowNotification(new GUIContent("创建成功"));
	}

	private void LoadPlot ()
	{
		string filePath = EditorUtility.OpenFilePanel ("Select Plot File", "Assets/GameResources/"+GamePlotManager.PLOT_PATH, "bytes");
		if (!string.IsNullOrEmpty (filePath)) {
			//反序列化GamePlotInfo，构造GamePlotInfo对象
			try {
				string jsonText = File.ReadAllText (filePath);
				GamePlotInfo loadedPlot = JsonMapper.ToObject<GamePlotInfo>(jsonText);
				if(loadedPlot != null)
				{
					Dispose();
					_curPlotInfo = loadedPlot;
					SortAllPlotInfo(true);
					this.ShowNotification(new GUIContent("加载成功"));
				}
				
			} catch (System.Exception e) {
				Debug.LogError ("An error occurred while loading file: " + e);
			}
		}
	}

	private  void SavePlot ()
	{
		//序列化GamePlotInfo,并保存剧情数据
		if(_curPlotInfo != null){
			RebuildEntityActionList();
			string jsonText = JsonMapper.ToJson(_curPlotInfo);
			try {
				string filePath = string.Format("{0}/GamePlot_{1}.bytes","Assets/GameResources/"+GamePlotManager.PLOT_PATH,_curPlotInfo.plotId);
				File.WriteAllText (filePath, jsonText);
				this.ShowNotification(new GUIContent("保存成功"));
				AssetDatabase.Refresh ();

			} catch (System.Exception e) {
				Debug.LogError ("An error occurred while saving file: " + e);
			}
		}
	}
	#endregion

	#region Entity Properties Panel
	private readonly string[] PlotEntityTypes = new string[3]{"角色","场景特效","摄像机"};
	private readonly string[] GlobalActionTypes = new string[3]{"播放音频","蒙版","压屏"};

	private int _selectedEntityIndex;
	private PlotEntity _selectedEntity;
	public PlotEntity SelectedEntity {
		set {
			_selectedEntity = value;
			EditorGUIUtility.editingTextField = false;
		}
	}

	private Vector2 _entityPanelScrollPos;

	private int _selectedActionIndex;
	private PlotAction _selectedAction;

	public PlotAction SelectedAction {
		set {
			_selectedAction = value;
			EditorGUIUtility.editingTextField = false;
		}
	}

	private Vector2 _actionPanelScrollPos;

	public static System.Enum DrawEnumOptions(string title,System.Enum selectedEnum)
	{
		EditorGUIUtility.labelWidth = 80f;
		System.Enum result = EditorGUILayout.EnumPopup(title,selectedEnum,GUILayout.MaxWidth(250f));
		return result;
	}

	public static int DrawPopupOptions(string title,int selectedIndex,string[] options){
		EditorGUIUtility.labelWidth = 80f;
		int result = EditorGUILayout.Popup(title,selectedIndex,options,GUILayout.MaxWidth(250f));
		return result;
	}

	private void DrawPropertyPanel(){
		EditorGUILayout.BeginHorizontal();
		{
			//剧情实体参数设置面板
			GUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal();
				{
					_selectedEntityIndex = DrawPopupOptions("剧情实体类型：",_selectedEntityIndex,PlotEntityTypes);
					if(GUILayout.Button("Add",GUILayout.Width(100f))){
						AddPlotEntity(_selectedEntityIndex);
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical("HelpBox");
				GUI.color = Color.green;
				string title = _selectedEntity != null?_selectedEntity.GetOptionName():"PlotEntityPanel";
				GUILayout.Label(title,tabButtonStyle);
				GUI.color = Color.white;
				_entityPanelScrollPos = EditorGUILayout.BeginScrollView(_entityPanelScrollPos,GUILayout.MinHeight(200f));
				if(_selectedEntity != null)
					_selectedEntity.ShowPropertyParam();
				else
					GUILayout.Label("当前没有选中剧情实体");
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndVertical();
			
			GUILayout.Space(10f);
			//全局动作参数设置面板
			GUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal();
				{
					_selectedActionIndex = DrawPopupOptions("全局操作类型：",_selectedActionIndex,GlobalActionTypes);
					if(GUILayout.Button("Add",GUILayout.Width(100f))){
						AddGlobalAction(_selectedActionIndex);
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical("HelpBox");
				GUI.color = Color.red;
				string title = _selectedAction != null?_selectedAction.GetOptionName():"PlotActionPanel";
				GUILayout.Label(title,tabButtonStyle);
				GUI.color = Color.white;
				_actionPanelScrollPos = EditorGUILayout.BeginScrollView(_actionPanelScrollPos,GUILayout.MinHeight(200f));
				if(_selectedAction != null)
					_selectedAction.ShowPropertyParam();
				else
					GUILayout.Label("当前没有选中动作指令");
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();	//参数设置区域End
	}

	private void AddPlotEntity(int type){
		PlotEntity entity = null;
		switch(type){
		case 0:
			entity = new CharacterEntity();
			_curPlotInfo.characterList.Add(entity as CharacterEntity);
			break;
		case 1:
			entity = new SceneEffectEntity();
			_curPlotInfo.sceneEffectList.Add(entity as SceneEffectEntity);
			break;
		case 2:
			if(_curPlotInfo.cameraList.Count == 0){
				entity = new CameraEntity();
				_curPlotInfo.cameraList.Add(entity as CameraEntity);
			}
			break;
		}
		entity.endTime = _curPlotInfo.plotTime;
		SelectedEntity = entity;
	}

	private void AddGlobalAction(int type){
		PlotAction action = null;
		switch(type){
		case 0:
			action = new PlayAudioAction();
			_curPlotInfo.audioActionList.Add(action as PlayAudioAction);
			break;
		case 1:
			action = new ScreenMaskAction();
			_curPlotInfo.screenMaskActionList.Add(action as ScreenMaskAction);
			break;
		case 2:
			action = new ScreenPresureAction();
			_curPlotInfo.screenPresureActionList.Add(action as ScreenPresureAction);
			break;
		}
		action.duration = _curPlotInfo.plotTime;
		SelectedAction = action;
	}
	#endregion

	#region Timeline Panel
	public const float TIMELINE_OPTIONWIDTH = 150f;
	private int _timeLineInterval = 10;
	private Vector2 _timeLineScrollPos;
	private bool _entityToggle = true;
	private bool _audioToggle = true;
	private bool _uiToggle = true;

	private void DrawGlobalActionTimeLine(IList actionList){
		DrawActionTimeLine(actionList,0f,_curPlotInfo.plotTime,0f,null);
	}

	//绘制动作指令时间线
	private void DrawActionTimeLine(IList actionList,float start,float end,float offset,PlotEntity entity){
		for(int i=0;i<actionList.Count;++i){
			PlotAction action = actionList[i] as PlotAction;
			EditorGUILayout.BeginHorizontal();
			if(action == _selectedAction) GUI.color = Color.red;

			GUILayout.Space(offset);
			if (GUILayout.Button ("X",tabButtonStyle,GUILayout.Width (20f))) {
				if(EditorUtility.DisplayDialog("删除动作指令","确认移除该指令","是","否")){
					if(entity != null)
						entity.allActionList.RemoveAt(i);
					else
						actionList.RemoveAt(i);

					if(action == _selectedAction)
						SelectedAction = null;

					return;
				}
			}
			GUILayout.Space(5f);
			if(GUILayout.Button(action.GetOptionName(),tabButtonStyle,GUILayout.Width(TIMELINE_OPTIONWIDTH-offset-25f)))
				SelectedAction = action;

			float startPoint = start+action.startTime;
			float endPoint = startPoint+action.duration;
			if(action.IsPoint()){
				startPoint = GUILayout.HorizontalSlider(startPoint,0f,_curPlotInfo.plotTime);
			}else{
				EditorGUILayout.MinMaxSlider(ref startPoint,ref endPoint,0f,_curPlotInfo.plotTime);
			}
			if(GUI.changed && action == _selectedAction){
//				startPoint = Mathf.Clamp(startPoint,start,end);
//				endPoint = Mathf.Clamp(endPoint,startPoint,end);

				action.StartTime = startPoint - start;
				action.Duration = endPoint - startPoint;
			}

            EditorGUILayout.EndHorizontal();
			action.DrawExtraTimeLine(TIMELINE_OPTIONWIDTH);
			GUI.color = Color.white;
		}
	}

	//绘制剧情实体时间线
	private void DrawEntityTimeLine(IList entityList){
		for(int i=0;i<entityList.Count;++i){
			PlotEntity entity = entityList[i] as PlotEntity;
			EditorGUILayout.BeginHorizontal();

			if(entity == _selectedEntity) GUI.color = Color.green;

			if (GUILayout.Button ("X",tabButtonStyle,GUILayout.Width (20f))) {
				if(EditorUtility.DisplayDialog("删除剧情实体","删除后会把属于该npc的指令都删除","是","否")){
					entityList.Remove(entity);
					if(entity == _selectedEntity)
						SelectedEntity = null;

					return;
				}
			}
			GUILayout.Space(5f);
			if(GUILayout.Button(entity.GetOptionName(),tabButtonStyle,GUILayout.Width(TIMELINE_OPTIONWIDTH-25f))){
//				if(_selectedEntity == entity)
//					entity.showActions = !entity.showActions;

				SelectedEntity = entity;
			}

			float startTime = entity.startTime;
			float endTime = entity.endTime;
			EditorGUILayout.MinMaxSlider(ref startTime,ref endTime,0f,_curPlotInfo.plotTime);
			if(GUI.changed && _selectedEntity == entity){
				entity.StartTime = startTime;
				entity.EndTime = endTime;
			}

			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();

			if(entity.showActions){
				DrawActionTimeLine(entity.allActionList,entity.startTime,entity.endTime,20f,entity);
//				List<IList> actionLists = entity.GetActionLists();
//				if(actionLists != null){
//					for(int j=0;j<actionLists.Count;++j){
//						DrawActionTimeLine(actionLists[j],entity.startTime,entity.endTime,20f);
//					}
//				}
			}
		}
	}

	private void DrawTimelinePanel(){
		_timeLineScrollPos = EditorGUILayout.BeginScrollView(_timeLineScrollPos,"HelpBox");
		{
			//绘制时间轴刻度
			EditorGUILayout.BeginHorizontal();
			{
				GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				if(GUILayout.Button("一键整理","LargeButtonRight",GUILayout.Width(TIMELINE_OPTIONWIDTH))){
					SortAllPlotInfo(false);
				}

				float deltaTime = _curPlotInfo.plotTime / _timeLineInterval;
				for(int i=0;i<=_timeLineInterval;++i){
					EditorGUILayout.BeginVertical();
					GUILayout.Label(string.Format("{0:0.00}",deltaTime*i),style);
					GUILayout.Label("|",style);
					EditorGUILayout.EndVertical();
				}
			}
			EditorGUILayout.EndHorizontal();

			//剧情实体区域
			_entityToggle = EditorGUILayout.Foldout(_entityToggle,"Entity");
			if(_entityToggle){
				DrawEntityTimeLine(_curPlotInfo.characterList);
				DrawEntityTimeLine(_curPlotInfo.sceneEffectList);
				DrawEntityTimeLine(_curPlotInfo.cameraList);
			}

			//全局指令区域
			_audioToggle = EditorGUILayout.Foldout(_audioToggle,"Audio");
			if(_audioToggle){
				DrawGlobalActionTimeLine(_curPlotInfo.audioActionList);
			}

			_uiToggle = EditorGUILayout.Foldout(_uiToggle,"UI");
			if(_uiToggle){
				DrawGlobalActionTimeLine(_curPlotInfo.screenMaskActionList);
				DrawGlobalActionTimeLine(_curPlotInfo.screenPresureActionList);
			}
		}
		EditorGUILayout.EndScrollView();
	}
	#endregion

	void RebuildEntityActionList(){
		for(int i=0;i<_curPlotInfo.characterList.Count;++i){
			_curPlotInfo.characterList[i].RebuildActionList();
		}
		
		for(int i=0;i<_curPlotInfo.cameraList.Count;++i){
			_curPlotInfo.cameraList[i].RebuildActionList();
		}
	}

	void SortAllPlotInfo(bool rebuild){
		_curPlotInfo.characterList.Sort(SortByTime);
		_curPlotInfo.sceneEffectList.Sort(SortByTime);

		for(int i=0;i<_curPlotInfo.characterList.Count;++i){
			CharacterEntity character = _curPlotInfo.characterList[i];
			if(rebuild){
				character.allActionList.Clear();
				for(int j=0;j<character.animationActionList.Count;++j){
					character.allActionList.Add(character.animationActionList[j]);
				}

				for(int j=0;j<character.tweenActionList.Count;++j){
					character.allActionList.Add(character.tweenActionList[j]);
				}

				for(int j=0;j<character.talkActionList.Count;++j){
					character.allActionList.Add(character.talkActionList[j]);
				}

				for(int j=0;j<character.followEffectList.Count;++j){
					character.allActionList.Add(character.followEffectList[j]);
				}
			}
			character.allActionList.Sort(SortByTime);
		}

		for(int i=0;i<_curPlotInfo.cameraList.Count;++i){
			CameraEntity camera = _curPlotInfo.cameraList[i];
			if(rebuild){
				camera.allActionList.Clear();
				for(int j=0;j<camera.tweenActionList.Count;++j){
					camera.allActionList.Add(camera.tweenActionList[j]);
				}
				
				for(int j=0;j<camera.shakeActionList.Count;++j){
					camera.allActionList.Add(camera.shakeActionList[j]);
				}
				
				for(int j=0;j<camera.camPathActionList.Count;++j){
					camera.allActionList.Add(camera.camPathActionList[j]);
				}
			}
			camera.allActionList.Sort(SortByTime);
		}

		_curPlotInfo.audioActionList.Sort(SortByTime);
		_curPlotInfo.screenMaskActionList.Sort(SortByTime);
		_curPlotInfo.screenPresureActionList.Sort(SortByTime);
	}

	private static int SortByTime(PlotEntity a,PlotEntity b){
		if(a.startTime == b.startTime)
			return a.GetHashCode().CompareTo(b.GetHashCode());

		return a.startTime.CompareTo(b.startTime);
	}
	
	private static int SortByTime(PlotAction a,PlotAction b){
		if(a.startTime == b.startTime)
			return a.GetHashCode().CompareTo(b.GetHashCode());

		return a.startTime.CompareTo(b.startTime);
	}

	//根据调整点将所有指令做整体偏移
	private void OffsetTimeLine(){
		_curPlotInfo.plotTime += _timeOffset;
		OffsetActionStartTime(_curPlotInfo.audioActionList);
		OffsetActionStartTime(_curPlotInfo.screenMaskActionList);

		for(int i=0;i<_curPlotInfo.characterList.Count;++i){
			var entity = _curPlotInfo.characterList[i];
			if(entity.startTime > 0f){
				OffsetEntityStartTime(entity);
			}else{
				entity.endTime += _timeOffset;
				OffsetActionStartTime(entity.allActionList,entity.startTime);
			}
		}

		for(int i=0;i<_curPlotInfo.cameraList.Count;++i){
			var entity = _curPlotInfo.cameraList[i];
			if(entity.startTime > 0f){
				OffsetEntityStartTime(entity);
			}else{
				entity.endTime += _timeOffset;
				OffsetActionStartTime(entity.allActionList,entity.startTime);
			}
		}

		for(int i=0;i<_curPlotInfo.sceneEffectList.Count;++i){
			var entity = _curPlotInfo.sceneEffectList[i];
			OffsetEntityStartTime(entity);
		}
	}

	private void OffsetEntityStartTime(PlotEntity entity){
		if(_isFront){
			if(entity.startTime <= _adjustTimePoint){
				entity.startTime += _timeOffset;
				entity.endTime += _timeOffset;
			}
		}else{
			if(entity.startTime >= _adjustTimePoint){
				entity.startTime += _timeOffset;
				entity.endTime += _timeOffset;
			}
		}
	}

	private void OffsetActionStartTime(IList actionList,float start=0f){
		for(int i=0;i<actionList.Count;++i){
			var action = actionList[i] as PlotAction;
			float plotStartTime = action.startTime + start; //动作指令在剧情轴上的起点
			if(_isFront){
				if(plotStartTime <= _adjustTimePoint){
					action.startTime += _timeOffset;
				}
			}else{
				if(plotStartTime >= _adjustTimePoint){
					action.startTime += _timeOffset;
				}
			}
		}
	}
}

