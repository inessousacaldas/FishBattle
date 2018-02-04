using UnityEngine;

public class OneShotSceneEffect : MonoBehaviour
{
	private GameObject _effGo;

	#region PlayFollowEffect

	public void Play (string effName, Vector3 worldPos, float effTime = 1f, float scaleFactor = 1f, System.Action onLoadFinish = null)
	{
        AssetPipeline.ResourcePoolManager.Instance.SpawnEffectAsync (effName,
			(effectGo) =>{
				if (effectGo == null)
					return;

				GameObjectExt.AddPoolChild (this.gameObject, effectGo);
				_effGo = effectGo;
				_effGo.transform.position = new Vector3 (worldPos.x, worldPos.y, worldPos.z);

				ParticleScaler scaler = _effGo.GetComponent<ParticleScaler> ();
				if (scaler != null) {
					scaler.SetScale (scaleFactor);
				}

                //if(is2DMap)
                //    _effGo.transform.eulerAngles = GridMapAgent.South;

                if (onLoadFinish != null)
					onLoadFinish ();

				JSTimer.Instance.SetupCoolDown (string.Format ("OneShotSceneEffect_{0}", _effGo.GetInstanceID ()), effTime, null, Dispose);
			});
	}

	public static OneShotSceneEffect Begin (string effName, Vector3 worldPos, float effTime = 1f, float scaleFactor = 1f, System.Action callBackFinish = null)
    {
		OneShotSceneEffect controller = LayerManager.Root.EffectsAnchor.AddComponent<OneShotSceneEffect> ();
		controller.Play (effName, worldPos, effTime, scaleFactor, callBackFinish);
		
		return controller;
	}

	#endregion

	#region PlayFollowEffect

	public void PlayFollowEffect (string effName, Transform folowTransform, float effTime = 1f, float scaleFactor = 1f, System.Action<GameObject> onLoadFinish = null)
	{
		AssetPipeline.ResourcePoolManager.Instance.SpawnEffectAsync (effName,
			(effectGo) =>{
				if (effectGo == null)
					return;

				if (folowTransform == null)
				{
                    AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(effectGo);
					return;
				}

				_effGo = effectGo;
				GameObjectExt.AddPoolChild (folowTransform.gameObject, effectGo);

				ParticleScaler scaler = _effGo.GetComponent<ParticleScaler> ();
				if (scaler != null) {
					scaler.SetScale (scaleFactor);
				}

				if (onLoadFinish != null)
					onLoadFinish (_effGo);

				JSTimer.Instance.SetupCoolDown (string.Format ("OneShotSceneEffect_{0}", _effGo.GetInstanceID ()), effTime, null, Dispose);
			});
	}

	public static OneShotSceneEffect BeginFollowEffect (string effName, Transform folowTransform, float effTime = 1f, float scaleFactor = 1f, System.Action<GameObject> callBackFinish = null)
	{
		if (string.IsNullOrEmpty (effName))
			return null;

		if (folowTransform == null)
			return null;

		OneShotSceneEffect controller = LayerManager.Root.EffectsAnchor.AddComponent<OneShotSceneEffect> ();
		controller.PlayFollowEffect (effName, folowTransform, effTime, scaleFactor, callBackFinish);
		return controller;
	}

	#endregion

	public void Dispose ()
	{
		if (_effGo != null) {
            AssetPipeline.ResourcePoolManager.Instance.DespawnEffect(_effGo);
			_effGo = null;
		}

		Destroy (this);
	}
}
