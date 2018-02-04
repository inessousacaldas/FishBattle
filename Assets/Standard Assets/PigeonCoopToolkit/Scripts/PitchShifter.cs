using UnityEngine;

public class PitchShifter : MonoBehaviour
{

    public PigeonCoopToolkit.Utillities.Range pitchRange;
    public AudioSource src;
	// Use this for initialization
	void Start ()
	{
	    src.pitch = Random.Range(pitchRange.Min, pitchRange.Max);
	}
}
