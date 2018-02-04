using UnityEngine;
using System.Collections;

public class CdSpriteTween : MonoBehaviour
{
	private UISprite mSprite;
	private JSTimer.CdTask mTask;

	void Awake ()
	{
		mSprite = this.GetComponent<UISprite> ();
		if (mSprite != null) {
			mSprite.fillAmount = 1f;
			UIWidget parentWidget = this.transform.parent.GetComponentInParent<UIWidget> ();
			if (parentWidget != null) {
				mSprite.depth = parentWidget.depth + 1;
			} else
				mSprite.depth = 5;
		}
	}

	void Update ()
	{
		if (mSprite != null) {
			if (mTask != null) {
				if (mTask.isValid) {
					mSprite.fillAmount = mTask.remainTime / mTask.totalTime;
				} else {
					mTask = null;
					mSprite.fillAmount = 0f;
				}
			} else {
				mSprite.fillAmount = 0f;
			}
		}
	}

	public void Setup (JSTimer.CdTask task)
	{
		mTask = task;
		if (mTask != null)
			mSprite.fillAmount = 1f;
		else
			mSprite.fillAmount = 0f;
	}
}
