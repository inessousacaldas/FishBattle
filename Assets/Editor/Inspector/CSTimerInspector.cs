using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(CSTimer))]
public class CsTimerInspector : Editor
{
	CSTimer mCSTimer;
	private bool _coolDownToggle = true;
	private bool _timerToggle = true;

	public override void OnInspectorGUI ()
	{
		mCSTimer = target as CSTimer;
		if (mCSTimer == null)
			return;
		
		var cdTasks = mCSTimer.CdTasks;
		GUILayout.Label ("=========================");
		_coolDownToggle = EditorGUILayout.Foldout (_coolDownToggle, string.Format ("CoolDown:{0}", cdTasks.Count));
		GUILayout.Label ("=========================");
		if (_coolDownToggle) {
			for (int i = 0; i < cdTasks.Count; ++i) {
				DrawCoolDownTask (cdTasks [i]);
			}
		}

		var timerTasks = mCSTimer.TimerTasks;
		GUILayout.Label ("=========================");
		_timerToggle = EditorGUILayout.Foldout (_timerToggle, string.Format ("Timer:{0}", timerTasks.Count));
		GUILayout.Label ("=========================");
		if (_timerToggle) {
			for (int i = 0; i < timerTasks.Count; ++i) {
				DrawTimerTask (timerTasks [i]);
			}
		}
		
		this.Repaint ();
	}

	void DrawCoolDownTask (CSTimer.CdTask coolDownTask)
	{
		GUILayout.BeginVertical ("GroupBox");
		{
			GUILayout.Label ("name:" + coolDownTask.taskName);
			if (coolDownTask.isValid) {
				GUILayout.Label (string.Format ("剩余时间:{0}/{1}", coolDownTask.remainTime, coolDownTask.totalTime));
				GUILayout.Label ("更新频率:" + coolDownTask.updateFrequence);
				GUILayout.Label ("timeScale:" + coolDownTask.timeScale);
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Finish")) {
					coolDownTask.remainTime = 0f;
				}

				if (GUILayout.Button ("Cancel")) {
					mCSTimer.CancelCd (coolDownTask.taskName);
				}

				if (GUILayout.Button (coolDownTask.isPause ? "Resume" : "Pause")) {
					coolDownTask.isPause = !coolDownTask.isPause;
				}
				GUILayout.EndHorizontal ();
			} else
				GUILayout.Label ("已失效");
		}
		GUILayout.EndVertical ();
	}

	void DrawTimerTask (CSTimer.TimerTask timerTask)
	{
		GUILayout.BeginVertical ("GroupBox");
		{
			GUILayout.Label ("name:" + timerTask.taskName);
			if (timerTask.isValid) {
				GUILayout.Label ("累计时间:" + timerTask.cumulativeTime);
				GUILayout.Label ("更新频率:" + timerTask.updateFrequence);
				GUILayout.Label ("timeScale:" + timerTask.timeScale);
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Cancel")) {
					mCSTimer.CancelTimer (timerTask.taskName);
				}

				if (GUILayout.Button (timerTask.isPause ? "Resume" : "Pause")) {
					timerTask.isPause = !timerTask.isPause;
				}
				GUILayout.EndHorizontal ();
			} else
				GUILayout.Label ("已失效");
		}
		GUILayout.EndVertical ();
	}
}