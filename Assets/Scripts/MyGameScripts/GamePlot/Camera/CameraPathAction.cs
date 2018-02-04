using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class CameraPathAction:PlotAction
	{
		public string prefabName ="路径动画";
		
#if UNITY_EDITOR
		public override string GetOptionName(){
			return this.prefabName;
		}

		public override bool IsPoint ()
		{
			return true;
		}

		[LITJson.JsonIgnore]
		public CameraPathAnimator camPathAnimator;
		protected override void DrawProperty(){
			base.DrawProperty();
			GUILayout.Label("CameraPath名称："+this.prefabName);
			this.camPathAnimator = (CameraPathAnimator)EditorGUILayout.ObjectField(this.camPathAnimator, typeof(CameraPathAnimator),false);
			if(this.camPathAnimator != null)
			{
				this.prefabName = this.camPathAnimator.name;
			}
		}
#endif
	}
}