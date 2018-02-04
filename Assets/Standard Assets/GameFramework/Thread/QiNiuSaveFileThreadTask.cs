using System;
using UnityEngine;

public  class QiNiuSaveFileThreadTask: ThreadTask
{
#pragma warning disable 
    private Action<bool, string> _onPutFinished;
#pragma warning restore
    private bool _isSuccessed;
	private string _key;

    public QiNiuSaveFileThreadTask(ByteArray byteArray, int buffSize, string scope, string key = null,
		bool overwrite = true, string mimeType = null, Action<bool, string> onPutFinished = null, int crc32 = 123)
    {
	    _targetAction = task =>
	    {
		    QiNiuFileExt.PutFileBuf(byteArray.bytes, buffSize, scope, key, overwrite, mimeType, QiNiuPutFinished, crc32);
	    };
		_finishedAction = PutFinished;
    }

	private void QiNiuPutFinished(bool successed, string key)
	{
		_isSuccessed = successed;
		_key = key;

		SetFinished();
	}

	private void PutFinished()
	{
		if (_onPutFinished != null)
		{
			_onPutFinished(_isSuccessed, _key);
		}
	}



	public static void SaveFileToQiNiu(ByteArray byteArray, int buffSize, string scope, string key = null,
		bool overwrite = true, string mimeType = null, Action<bool, string> onPutFinished = null, int crc32 = 123)
	{
		var threadTask = new QiNiuSaveFileThreadTask(byteArray, buffSize, scope, key, overwrite, mimeType, onPutFinished, crc32);
		ThreadManager.Instance.AddThread(threadTask);
	}


    public static void SaveAudio(AudioClip recondClip, int lastPos, string scope, string key = null,
        bool overwrite = true, string mimeType = null, Action<bool, string> onPutFinished = null, int crc32 = 123)
    {
        var samplesBuf = new float[recondClip.samples];
        recondClip.GetData(samplesBuf, 0);
        var amrData = HzamrPlugin.Encode(samplesBuf, recondClip.samples);

        SaveFileToQiNiu(new ByteArray(amrData), amrData.Length, scope, key, overwrite, mimeType, onPutFinished, crc32);
    }
}
