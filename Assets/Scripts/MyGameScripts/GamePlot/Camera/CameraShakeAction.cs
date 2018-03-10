using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class CameraShakeAction:PlotAction
	{
		public Vector3 strength = Vector3.one;
		public int vibrato = 10;
		public float randomness = 90f;
		
#if UNITY_EDITOR
		public override string GetOptionName(){
			return "震屏";
		}
		
		protected override void DrawProperty(){
			base.DrawProperty();
			this.strength = EditorGUILayout.Vector3Field("晃动强度：",this.strength);
			this.vibrato = EditorGUILayout.IntField("晃动次数：",this.vibrato);
			this.randomness = EditorGUILayout.FloatField(new GUIContent("随机性：","设置为0时只会沿着单一方向晃动"),this.randomness);
		}
#endif
	}
}