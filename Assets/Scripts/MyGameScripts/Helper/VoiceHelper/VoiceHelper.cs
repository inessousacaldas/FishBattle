using UnityEngine;

public class VoiceHelper : MonoBehaviour
{
    public static float GetVoiceVolume(AudioSource audioSource)
    {
        if (Microphone.IsRecording(null))
        {
            var audioClip = audioSource.clip;
            if (audioClip == null)
            {
                Debug.LogError("GetVoiceVolume:audioClip is null.");
                return 0;
            }
            // 采样数
            int sampleSize = 8;
            float[] samples = new float[sampleSize];
            int startPosition = Microphone.GetPosition(null) - (sampleSize + 1);
            if (startPosition < 0)
            {
                startPosition = 0;
            }
            // 得到数据
            audioClip.GetData(samples, startPosition);

            // Getting a peak on the last 8 samples
            float levelMax = 0;
            for (int i = 0; i < sampleSize; ++i)
            {
                float wavePeak = samples[i];
                if (levelMax < wavePeak)
                    levelMax += wavePeak;
            }

            return levelMax; // *99; 
        }
        return 0;
    }

    public static bool ValidateVoiceData(AudioSource audioSource)
    {
        var audioClip = audioSource.clip;
        if (audioClip == null)
        {
            Debug.LogError("ValidateVoiceData:audioClip is null.");
            return false;
        }
        int lengthSample = audioClip.samples * audioClip.channels;
        float[] samplesBuf = new float[lengthSample];
        audioClip.GetData(samplesBuf, 0);

        bool hasData = false;
        for (int index = 0; index < samplesBuf.Length; index++)
        {
            if (samplesBuf[index] != 0)
            {
                hasData = true;
                break;
            }
        }
        return hasData;
    }

    public static AudioClip CopyToNewClip(AudioSource audioSource, int frequency)
    {
        var audioClip = audioSource.clip;
        if (audioClip == null)
        {
            Debug.LogError("CopyToNewClip:audioClip is null.");
            return null;
        }
        int lengthSample = audioClip.samples * audioClip.channels;
        float[] samples = new float[lengthSample];
        audioClip.GetData(samples, 0);

        if (lengthSample > 0)
        {
            AudioClip clip = AudioClip.Create("clip", lengthSample, audioClip.channels, frequency, false, false);
            clip.SetData(samples, 0);
            return clip;
        }
        return null;
    }
}