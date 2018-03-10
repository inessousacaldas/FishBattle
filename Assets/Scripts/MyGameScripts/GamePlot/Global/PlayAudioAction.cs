using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamePlot
{
	public class PlayAudioAction:PlotAction
	{
		public enum AudioType
		{
			Music,
			Sound
		}
		public AudioType audioType;
		public string audioPath;

#if UNITY_EDITOR
		public override string GetOptionName(){
			if(audioType == AudioType.Music)
				return "音乐";
			else
				return "音效";
		}

		public override bool IsPoint() {
			if(audioType == AudioType.Sound)
				return true;
			else
				return false;
		}

		protected override void DrawProperty(){
			base.DrawProperty();
			this.audioType = (AudioType)EditorGUILayout.EnumPopup("类型：",this.audioType,GUILayout.MaxWidth(250f));
			this.audioPath = EditorGUILayout.TextField("音频路径：",this.audioPath);
		}
#endif
	}
}
