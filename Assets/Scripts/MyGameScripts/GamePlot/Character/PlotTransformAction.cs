using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class PlotTransformAction:PlotAction
	{
		public enum TweenType
		{
			NavMove,
			Rotate,
			Scale,
			PosMove
		}
		public TweenType tweenType;
		public Vector3 endValue;
	    public float speed;

#if UNITY_EDITOR
		public override string GetOptionName(){
			if(tweenType == TweenType.NavMove)
				return "寻路移动";
			else if(tweenType == TweenType.Rotate)
				return "旋转";
			else if(tweenType == TweenType.Scale)
				return "缩放";
			else if(tweenType == TweenType.PosMove)
				return "平移";
			else
				return "PlotTransformAction";
		}

		public override bool IsPoint() {
			if(tweenType == TweenType.NavMove)
				return true;
			else
				return false;
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.tweenType = (TweenType)EditorGUILayout.EnumPopup("类型：",this.tweenType,GUILayout.MaxWidth(250f));
			this.endValue = PlotEntity.Vector3Field("目标值：",this.endValue,(int)tweenType);
            this.speed = Mathf.Max(EditorGUILayout.FloatField("移动速度：", this.speed), 0f);
        }
#endif
	}
}