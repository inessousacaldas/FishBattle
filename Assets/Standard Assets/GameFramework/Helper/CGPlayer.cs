using System;
using System.Collections;
using UnityEngine;

public class CGPlayer : MonoBehaviour
{
    public static void PlayCG(string cgPath, Action onFinish)
    {
        var go = new GameObject("CGPlayer");
        var player = go.AddComponent<CGPlayer>();
        player.Setup(cgPath, onFinish);
    }

    private string _cgPath;
    private Action _onFinish;
#if UNITY_STANDALONE
    private MovieTexture _movTexture;
    private bool _playing;
#endif

    private void Setup(string cgPath, Action onFinish)
    {
        _cgPath = cgPath;
        _onFinish = onFinish;
    }

    private void Start()
    {
        TalkingDataHelper.OnEventSetp("PlayCG", "Play");

#if UNITY_STANDALONE
        _movTexture = EditorRuntimeHelper.LoadAssetAtPath<MovieTexture>(_cgPath);
        if (_movTexture == null)
        {
            StopMov();
            return;
        }

        _playing = true;
        _movTexture.loop = false;
        _movTexture.Play();
#else
		StartCoroutine (PlayCGOnMobile ());
#endif
    }

#if UNITY_STANDALONE
    private void Update()
    {
        if (_playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TalkingDataHelper.OnEventSetp("PlayCG", "Skip");
                StopMov();
                return;
            }
        }
        if (_movTexture != null && !_movTexture.isPlaying)
        {
            StopMov();
        }
    }

    private void OnGUI()
    {
        if (_playing)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _movTexture);
        }
    }
#endif

#if !UNITY_STANDALONE
    IEnumerator PlayCGOnMobile ()
	{
		Handheld.PlayFullScreenMovie (_cgPath, Color.black, FullScreenMovieControlMode.CancelOnInput);
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		StopMov ();
	}
#endif

    private void StopMov()
    {
#if UNITY_STANDALONE
        if (_movTexture != null)
        {
            _movTexture.Stop();
        }
        _playing = false;
#endif
        TalkingDataHelper.OnEventSetp("PlayCG", "Finish");
        Destroy(gameObject);

        if (_onFinish != null)
            _onFinish();
    }
}