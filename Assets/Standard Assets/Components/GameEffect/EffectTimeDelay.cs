using UnityEngine;
using System.Collections;

public class EffectTimeDelay : MonoBehaviour 
{
	public enum EffectTimeDelayMode
	{
		None,
		GameObjectOn,
		GameObjectOff,
		EmitterOn,
		EmitterOff,
		AnimationStart,
		AnimationStop,
	}
	public EffectTimeDelayMode ModeSet;
	public float timeDelay;
	public Transform trans;
	// Use this for initialization
	void Start () 	{
		if (trans == null || ModeSet == EffectTimeDelayMode.None)
			return;
		Invoke("OnEffectEventFire", timeDelay);
	}
	
	void OnEffectEventFire()
	{
		switch (ModeSet)
		{
			case EffectTimeDelayMode.GameObjectOn:
				trans.gameObject.SetActive(true);
				break;
			case EffectTimeDelayMode.GameObjectOff:
				trans.gameObject.SetActive(false);
				break;
			case EffectTimeDelayMode.EmitterOn:
				if (trans.GetComponent<ParticleEmitter>() != null)
					trans.GetComponent<ParticleEmitter>().emit = true;
				break;
			case EffectTimeDelayMode.EmitterOff:
				if (trans.GetComponent<ParticleEmitter>() != null)
					trans.GetComponent<ParticleEmitter>().emit = false;
				break;
			case EffectTimeDelayMode.AnimationStart:
				if (trans.GetComponent<Animation>() != null)
					trans.GetComponent<Animation>().Play();
				break;
			case EffectTimeDelayMode.AnimationStop:
				if (trans.GetComponent<Animation>() != null)
					trans.GetComponent<Animation>().Stop();
				break;
			default:
				break;
		}
	}

}
