using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class ScreenMaskAction:PlotAction
	{
		public Color startColor = Color.clear;
		public Color endColor = Color.black;
		public bool fade;
		public float fadeInTime;
		public float fadeOutTime;
		public float fadeTweenTime = 0.4f;

		//屏幕蒙版内容参数
		public string content;
		public int fontSize = 20;
		public float msgStartTime;
		public float msgEndTime;

#if UNITY_EDITOR
		public override string GetOptionName(){
			return "蒙版";
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.startColor = EditorGUILayout.ColorField("开始颜色：",this.startColor);
			this.endColor = EditorGUILayout.ColorField("结束颜色：",this.endColor);

			this.fadeInTime = EditorGUILayout.FloatField("淡入开始时间：",this.fadeInTime);
			this.fadeInTime = Mathf.Clamp(this.fadeInTime,0f,this.duration);

			this.fadeOutTime = EditorGUILayout.FloatField("淡出开始时间：",this.fadeOutTime);
			this.fadeOutTime = Mathf.Clamp(this.fadeOutTime,this.fadeInTime,this.duration);
            
			this.fade = EditorGUILayout.Toggle("是淡入淡出：",this.fade);
			EditorGUI.BeginDisabledGroup(!this.fade);
			this.fadeTweenTime = EditorGUILayout.FloatField("淡入淡出动画时间：",this.fadeTweenTime);
			EditorGUI.EndDisabledGroup();

			this.fontSize = EditorGUILayout.IntField("字号：",this.fontSize);
			this.msgStartTime = EditorGUILayout.FloatField("文字开始时间：",this.msgStartTime);
			this.msgStartTime = Mathf.Clamp(this.msgStartTime,0f,this.duration);
			this.msgEndTime = EditorGUILayout.FloatField("文字结束时间：",this.msgEndTime);
			this.msgEndTime = Mathf.Clamp(this.msgEndTime,this.msgStartTime,this.duration);
			EditorGUILayout.PrefixLabel("内容：");
			this.content = EditorGUILayout.TextArea(this.content);
		}

		public override void DrawExtraTimeLine (float titleWidth)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Button("淡入淡出时间",GUILayout.Width(titleWidth));
			EditorGUILayout.MinMaxSlider(ref this.fadeInTime,ref this.fadeOutTime,0f,this.duration);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Button("文字时间",GUILayout.Width(titleWidth));
			EditorGUILayout.MinMaxSlider(ref this.msgStartTime,ref this.msgEndTime,0f,this.duration);
			GUILayout.EndHorizontal();
		}
#endif
	}
}
