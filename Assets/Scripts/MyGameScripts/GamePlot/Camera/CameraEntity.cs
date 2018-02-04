using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR

#endif

namespace GamePlot
{
	//剧情镜头实体
	public class CameraEntity:PlotEntity
	{
		public Vector3 originPos;
		public Vector3 originRotation;
		public List<CameraPathAction> camPathActionList = new List<CameraPathAction>();
		public List<CameraShakeAction> shakeActionList = new List<CameraShakeAction>();
		public List<PlotTransformAction> tweenActionList = new List<PlotTransformAction>();

#if UNITY_EDITOR
		protected override void DrawProperty(){
			base.DrawProperty();

			this.originPos = PlotEntity.Vector3Field("起始位置：",this.originPos,0);
			this.originRotation = PlotEntity.Vector3Field("起始朝向：",this.originRotation,1);

//			GUILayout.Label(string.Format("当前路径动画指令数：{0}",camPathActionList.Count));
			if(GUILayout.Button("添加路径动画指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new CameraPathAction();
//				camPathActionList.Add(action);
				allActionList.Add(action);
			}
			
//			GUILayout.Label(string.Format("当前震屏指令数：{0}",shakeActionList.Count));
			if(GUILayout.Button("添加震屏指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new CameraShakeAction();
//				shakeActionList.Add(action);
				allActionList.Add(action);
			}

//			GUILayout.Label(string.Format("当前(平移●旋转●缩放)指令数：{0}",tweenActionList.Count));
			if(GUILayout.Button("添加(平移●旋转●缩放)指令",GUILayout.Width(100f),GUILayout.Height(40f))){
				var action = new PlotTransformAction();
//				tweenActionList.Add(action);
				allActionList.Add(action);
			}
		}

		public override string GetOptionName(){ 
			return "Camera";
		}

		public override void RebuildActionList ()
		{
			this.camPathActionList.Clear();
			this.shakeActionList.Clear();
			this.tweenActionList.Clear();
			
			for(int i=0,imax=this.allActionList.Count;i<imax;++i){
				PlotAction action = this.allActionList[i];
				if(action is CameraPathAction)
					this.camPathActionList.Add(action as CameraPathAction);
				else if(action is CameraShakeAction)
					this.shakeActionList.Add(action as CameraShakeAction);
				else if(action is PlotTransformAction)
					this.tweenActionList.Add(action as PlotTransformAction);
			}
		}
//		public override List<IList> GetActionLists(){
//			return new List<IList>{camPathActionList,shakeActionList,tweenActionList};
//		}
#endif
	}
}
