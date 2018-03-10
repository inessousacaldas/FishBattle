using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class ScreenPresureAction:PlotAction
	{
		//压屏的长度
		public int length = 80;
		//压屏动画时间
		public float tweenTime = 2f;

#if UNITY_EDITOR
		public override string GetOptionName(){
			return "压屏";
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.length = EditorGUILayout.IntField("压屏长度：",this.length);
			this.tweenTime = EditorGUILayout.FloatField("压屏动画时间：",this.tweenTime);
			this.tweenTime = Mathf.Clamp(this.tweenTime,0f,this.duration);
		}
#endif
	}
}
