#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class PlotTalkAction:PlotAction
	{
		public string content;
		public float offsetY;

#if UNITY_EDITOR
		public override string GetOptionName(){
			return "对话";
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.offsetY = EditorGUILayout.FloatField("位置偏移：",this.offsetY);
			EditorGUILayout.PrefixLabel("内容：");
			this.content = EditorGUILayout.TextArea(this.content);
		}
#endif
	}
}


