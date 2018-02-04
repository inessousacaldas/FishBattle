using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class PlotAction
	{
		public bool active = true;
		//动作指令的开始时间是相对于其依赖的剧情实体来计算的
		//全局动作指令是相对于整个剧情时间来计算的
		public float startTime = 0f;
		public float duration = 1f;

#if UNITY_EDITOR
		public virtual string GetOptionName(){
			return "Action";
		}

		//true时表示当前动作指令的持续时间为0，只代表一个时间点
		public virtual bool IsPoint(){
			return false;
		}

		public float StartTime {
			set {
				this.startTime = (float)System.Math.Round(value,2);
			}
		}

		public float Duration {
			set {
				this.duration = (float)System.Math.Round(value,2);
			}
		}

		public void ShowPropertyParam(){
			this.active = EditorGUILayout.Toggle("是否激活：",this.active,GUILayout.Width(100f));
			EditorGUI.BeginDisabledGroup(!this.active);
			DrawProperty();
			EditorGUI.EndDisabledGroup();
		}

		protected virtual void DrawProperty(){

			this.StartTime = Mathf.Max(EditorGUILayout.FloatField("起始时间：",this.startTime),0f);
			if(IsPoint()){
				this.duration = 0f;
			}else{
				this.Duration = Mathf.Max(EditorGUILayout.FloatField("持续时间：",this.duration),0f);
			}
			EditorGUILayout.Space();
		}

		public virtual void DrawExtraTimeLine(float titleWidth){

		}
#endif
	}
}
