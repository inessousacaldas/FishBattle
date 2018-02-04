#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class PlotAnimationAction:PlotAction
	{
		public string clip;
		
		#if UNITY_EDITOR
		public override string GetOptionName(){
			return string.IsNullOrEmpty(clip)?"动画名":this.clip;
		}
		
		public override bool IsPoint() {
			return true;
		}
		
		protected override void DrawProperty(){
			base.DrawProperty();
			this.clip = EditorGUILayout.TextField("动画名：",this.clip);
		}
		#endif
	}
}