using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ModelVisibleChecker : MonoBehaviour
{
    public float fadeOut = 1f;
    public bool debug = false;
    private Action _onInvisible;
    private Action _onVisible;
    private bool _isVisible;
    private bool _fading;

    public void Setup(Action onVisible, Action onInvisible, float fadeOut = 1f)
    {
        this.fadeOut = fadeOut;
        _isVisible = false;
        _fading = false;
        _onVisible = onVisible;
        _onInvisible = onInvisible;
    }

    void OnDisable()
    {
        _isVisible = false;
        _fading = false;
    }

    void OnDestroy()
    {
        _onVisible = null;
        _onInvisible = null;
    }

    // OnBecameVisible is called when the renderer became visible by any camera
    void OnBecameVisible()
    {
        _isVisible = true;

        if (!_fading)
        {
            Log("name: " + name + " OnBecameVisible");
            if (_onVisible != null)
                _onVisible();
        }
    }

    // OnBecameInvisible is called when the renderer is no longer visible by any camera
    IEnumerator OnBecameInvisible()
    {
        _isVisible = false;

        if (!_fading)
        {
            _fading = true;
            yield return new WaitForSeconds(fadeOut);
            _fading = false;
            if (!_isVisible)
            {
                Log("name: " + name + " OnBecameInvisible");
                if (_onInvisible != null)
                    _onInvisible();
            }
        }
    }

    private void Log(string msg)
    {
        if (debug)
        {
            Debug.LogError(msg);
        }
    }
}