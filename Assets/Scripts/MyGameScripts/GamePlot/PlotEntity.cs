using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class PlotEntity
	{
		public bool active = true;
		public float startTime = 0f;
		public float endTime = 1f;

#if UNITY_EDITOR
		public virtual string GetOptionName(){
			return "Entity";
		}

		public float StartTime {
			set {
				startTime = (float)System.Math.Round(value,2);
			}
		}

		public float EndTime {
			set {
				endTime = (float)System.Math.Round(value,2);
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
			this.EndTime = Mathf.Max(EditorGUILayout.FloatField("结束时间：",this.endTime),0f);
			EditorGUILayout.Space();
		}

		[LITJson.JsonIgnore]
		public bool showActions = true;
		[LITJson.JsonIgnore]
		public List<PlotAction> allActionList = new List<PlotAction>();
		public virtual void RebuildActionList(){

		}
//		public virtual List<IList> GetActionLists(){
//			return null;
//		}

		//0--Pos 1--Rotation 2--Scale
		public static Vector3 Vector3Field(string title,Vector3 input,int type){
			GUILayout.Label(title);
			GUILayout.BeginHorizontal();
			input = EditorGUILayout.Vector3Field(GUIContent.none,input);
			if(GUILayout.Button("C",GUILayout.Width(20f)))
			{
				if(Selection.activeTransform != null){
					if(type == 0)
						input = Selection.activeTransform.localPosition;
					else if(type == 1)
						input = Selection.activeTransform.localEulerAngles;
					else if(type == 2)
						input = Selection.activeTransform.localScale;
					else
						input = Selection.activeTransform.localPosition;
				}
			}
			GUILayout.EndHorizontal();
			return input;
		}

		public static float OrientationField(string title,float input){
			GUILayout.BeginHorizontal();
			input = EditorGUILayout.FloatField(title,input);
			if(GUILayout.Button("C",GUILayout.Width(20f)))
			{
				if(Selection.activeTransform != null)
					input = Selection.activeTransform.localEulerAngles.y;
            }
            GUILayout.EndHorizontal();
            return input;
		}
#endif
	}
}