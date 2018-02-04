using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	//剧情角色实体
	public class CharacterEntity:PlotEntity
	{
		public string name = "无";
		//使用玩家模型
		public bool isHero;

		//角色外观参数
		public int modelId;
		public int mutateTexture = 0;
		public string mutateColor = "";
		public int wpModel = 0;
        public int hallowSpriteId = 0;
        public float scale = 1f;
        public int ornamentId = 0;

		//位置朝向参数
		public Vector3 originPos;
		public float rotateY;
		public string defaultAnim;

        //npcId 暂时用于结婚剧情 固定死 1 新郎 2 新娘 默认为0
	    public int npcId;

		public List<PlotAnimationAction> animationActionList = new List<PlotAnimationAction>();
		public List<PlotTransformAction> tweenActionList = new List<PlotTransformAction>();
		public List<PlotTalkAction> talkActionList = new List<PlotTalkAction>();
		public List<PlotFollowEffectAction> followEffectList = new List<PlotFollowEffectAction>();

#if UNITY_EDITOR
		public override string GetOptionName(){
			return string.Format("角色:{0}",this.name);
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.isHero = EditorGUILayout.Toggle("使用玩家模型：",this.isHero,GUILayout.Width(100f));
			EditorGUI.BeginDisabledGroup(this.isHero);
			this.name = EditorGUILayout.TextField("名称：",this.name);
			this.modelId = EditorGUILayout.IntField("modelId:",this.modelId);
			this.mutateTexture = EditorGUILayout.IntField("变异贴图ID:",this.mutateTexture);
			this.mutateColor = EditorGUILayout.TextField("变色参数：",this.mutateColor);
			this.wpModel = EditorGUILayout.IntField("武器ID:",this.wpModel);
            this.hallowSpriteId = EditorGUILayout.IntField("器灵ID:", this.hallowSpriteId);
            EditorGUI.EndDisabledGroup();
			this.scale = EditorGUILayout.FloatField("缩放：",this.scale);
		    this.npcId = EditorGUILayout.IntField("新郎1新娘2", this.npcId);

			EditorGUILayout.Space();

			this.originPos = PlotEntity.Vector3Field("起始位置：",this.originPos,0);
			this.rotateY = PlotEntity.OrientationField("朝向：",this.rotateY);
			this.defaultAnim = EditorGUILayout.TextField("默认动作：",this.defaultAnim);

//			GUILayout.Label(string.Format("当前动画指令数：{0}",animationActionList.Count));
			if(GUILayout.Button("添加动画指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new PlotAnimationAction();
//				animationActionList.Add(new PlotAnimationAction());
				allActionList.Add(action);
			}

//			GUILayout.Label(string.Format("当前(平移●旋转●缩放)指令数：{0}",tweenActionList.Count));
			if(GUILayout.Button("添加(平移●旋转●缩放)指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new PlotTransformAction();
//				tweenActionList.Add(action);
				allActionList.Add(action);
			}

//			GUILayout.Label(string.Format("当前对话指令数：{0}",talkActionList.Count));
			if(GUILayout.Button("添加对话指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new PlotTalkAction();
//				talkActionList.Add(new PlotTalkAction());
				allActionList.Add(action);
			}

//			GUILayout.Label(string.Format("当前特效指令数：{0}",followEffectList.Count));
			if(GUILayout.Button("添加特效指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new PlotFollowEffectAction();
//				followEffectList.Add(new PlotFollowEffectAction());
				allActionList.Add(action);
			}
		}

		public override void RebuildActionList ()
		{
			this.animationActionList.Clear();
			this.tweenActionList.Clear();
			this.talkActionList.Clear();
			this.followEffectList.Clear();

			for(int i=0,imax=this.allActionList.Count;i<imax;++i){
				PlotAction action = this.allActionList[i];
				if(action is PlotAnimationAction)
					this.animationActionList.Add(action as PlotAnimationAction);
				else if(action is PlotTransformAction)
					this.tweenActionList.Add(action as PlotTransformAction);
				else if(action is PlotTalkAction)
					this.talkActionList.Add(action as PlotTalkAction);
				else if(action is PlotFollowEffectAction)
					this.followEffectList.Add(action as PlotFollowEffectAction);
			}
		}
//		public override List<IList> GetActionLists(){
//			return new List<IList>{animationActionList,tweenActionList,talkActionList,followEffectList};
//		}
#endif
	}
}
