using System.Collections;
using UnityEngine;

public class RendererFlicker : MonoBehaviour
{
    private Renderer[] _renderers;
    public float flashDelay = 0f;
    public float flashSpeed = 1f;

    // Use this for initialization
    private void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();

        if (_renderers != null)
        {
            StartCoroutine(Flicker());
        }
    }

    private IEnumerator Flicker()
    {
        yield return new WaitForSeconds(flashDelay);

        if (_renderers != null)
        {
            while (true)
            {
                if (_renderers != null)
                {
                    for (int i = 0, len = _renderers.Length; i < len; i++)
                    {
                        var smr = _renderers[i];
                        if (smr != null)
                        {
                            smr.enabled = !smr.enabled;
                        }
                    }
                }
                yield return new WaitForSeconds(flashSpeed);
            }
        }
    }

    public void Dispose()
    {
        StopCoroutine(Flicker());

        if (_renderers != null)
        {
            for (int i = 0, len = _renderers.Length; i < len; i++)
            {
                var smr = _renderers[i];
                if (smr != null)
                {
                    smr.enabled = true;
                }
            }
            _renderers = null;
        }
    }
}