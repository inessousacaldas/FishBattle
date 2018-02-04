using UnityEngine;
using System.Collections;

public class EffectDelayPlayer : MonoBehaviour
{
    [SerializeField]
    public float delay;

    [SerializeField]
    public Transform target;

   
    private int _playCount = 1;
    private float _timeCount;
    public bool reset = false;

    void OnEnable()
    {
        ResetAndStart();
    } 

    void ResetAndStart()
    {
        if (this.target != null)
        {
            _timeCount = 0;
            _playCount = 1;
            this.target.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (reset)
        {
            reset = false;
            ResetAndStart();
        }

        if (this.target == null || this.target.gameObject.activeSelf)
            return;

        if (_playCount > 0)
        {
            _timeCount += Time.deltaTime;
            if (_timeCount > delay)
            {
                this.target.gameObject.SetActive(true);
                _timeCount = 0;
                _playCount--;
            }
        }
    }
}
