using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	//场景特效实体
	public class SceneEffectEntity:PlotEntity
	{
		public string folderName;
		public string effPath;
		public Vector3 originPos;
	    public bool loop;
	    public bool rotate;
	    public Vector3 rotateValue;

#if UNITY_EDITOR
		public override string GetOptionName(){
			return string.Format("场景特效:{0}",this.effPath);
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.folderName = EditorGUILayout.TextField("目录名：",this.folderName);
			this.effPath = EditorGUILayout.TextField("特效名：",this.effPath);
			this.originPos = PlotEntity.Vector3Field("起始位置：",this.originPos,0);
            this.loop =  EditorGUILayout.Toggle("是否循环：", this.loop, GUILayout.Width(100f));
            this.rotate = EditorGUILayout.Toggle("旋转：", this.rotate, GUILayout.Width(100f));

            EditorGUI.BeginDisabledGroup(!this.rotate);
            this.rotateValue = EditorGUILayout.Vector3Field("旋转：", this.rotateValue);
            EditorGUI.EndDisabledGroup();
            
        }
#endif
	}
}