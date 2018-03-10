using UnityEngine;
using System.Collections;
using gcloud_voice;
using System;

public class GCloudVoiceHelper : MonoBehaviour {

    
    public static GCloudVoiceHelper Instance;
    public static void CreateInstance()
    {
        GameObject go = GameObject.Find("GCloudVoiceHelper");
        if (go == null)
        {
            go = new GameObject("GCloudVoiceHelper");
            DontDestroyOnLoad(go);
            go.AddComponent<GCloudVoiceHelper>();
        }
    } 
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetUpGCloudVoice();
    }
    private void Update()
    {
        if(m_voiceengine != null)
            m_voiceengine.Poll();
    }
    private void OnApplicationPause(bool pause)
    {
        
    }
    private void OnApplicationQuit()
    {
        
    }

    private IGCloudVoice m_voiceengine = null;
    public GCloudVoiceMode currentMode = GCloudVoiceMode.Messages;

    public void SetUpGCloudVoice()
    {
        if (m_voiceengine == null)
            m_voiceengine = m_voiceengine = GCloudVoice.GetEngine();
        m_voiceengine.SetAppInfo(GameConfig.TENCENT_GCLOUD_APPID, GameConfig.TENCENT_GCLOUD_APPKEY, ModelManager.Player.GetPlayerId().ToString());
        m_voiceengine.Init();
        GameDebuger.Log("初始化GCloude Engine 完成");
    }
    #region Test
    private string testRoomName = "WTFILoveyou";
    private void TestRealTimeVoice()
    {
        ChangeMode(GCloudVoiceMode.RealTime);
        JoinRoom(testRoomName);
    }
    #endregion
    #region Offline VoiceMessage
    private bool bIsGetAuthKey = false;
    private void InitOfflineMessageEvent()
    {
        if (m_voiceengine.SetMode(GCloudVoiceMode.Messages) == 0)
        {
            m_voiceengine.OnApplyMessageKeyComplete += M_voiceengine_OnApplyMessageKeyComplete;
            m_voiceengine.OnUploadReccordFileComplete += M_voiceengine_OnUploadReccordFileComplete;
            m_voiceengine.OnDownloadRecordFileComplete += M_voiceengine_OnDownloadRecordFileComplete;
            m_voiceengine.OnPlayRecordFilComplete += M_voiceengine_OnPlayRecordFilComplete;

            if (bIsGetAuthKey)
                m_voiceengine.ApplyMessageKey(6000);
        }
        else
            GameDebuger.LogError("初始化 离线语音消息失败..");
    }
    private void ClearOfflineMessageEvent()
    {
        m_voiceengine.OnApplyMessageKeyComplete -= M_voiceengine_OnApplyMessageKeyComplete;
        m_voiceengine.OnUploadReccordFileComplete -= M_voiceengine_OnUploadReccordFileComplete;
        m_voiceengine.OnDownloadRecordFileComplete -= M_voiceengine_OnDownloadRecordFileComplete;
        m_voiceengine.OnPlayRecordFilComplete -= M_voiceengine_OnPlayRecordFilComplete;
    }
    private void M_voiceengine_OnApplyMessageKeyComplete(IGCloudVoice.GCloudVoiceCompleteCode code)
    {
        Debug.Log("OnApplyMessageKeyComplete c# callback");
        if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_MESSAGE_KEY_APPLIED_SUCC)
        {
            bIsGetAuthKey = true;
            Debug.Log("OnApplyMessageKeyComplete succ11");
        }
        else
        {
            Debug.Log("OnApplyMessageKeyComplete error");
        }
    }
    private void M_voiceengine_OnUploadReccordFileComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid)
    {
        Debug.Log("OnUploadReccordFileComplete c# callback");
        //s_strLog += "\r\n"+" fileid len="+fileid.Length+"OnUploadReccordFileComplete ret="+code+" filepath:"+filepath+" fielid:"+fileid;
        if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_UPLOAD_RECORD_DONE)
        {
            Debug.Log("OnUploadReccordFileComplete succ, filepath:" + filepath + " fileid len=" + fileid.Length + " fileid:" + fileid + " fileid len=" + fileid.Length);
        }
        else
        {
            Debug.Log("OnUploadReccordFileComplete error");
        }
    }
    private void M_voiceengine_OnDownloadRecordFileComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid)
    {
        Debug.Log("OnDownloadRecordFileComplete c# callback");
        //s_strLog += "\r\n"+"OnDownloadRecordFileComplete ret="+code+" filepath:"+filepath+" fielid:"+fileid;
        if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_DOWNLOAD_RECORD_DONE)
        {
            Debug.Log("OnDownloadRecordFileComplete succ, filepath:" + filepath + " fileid:" + fileid);
        }
        else
        {
            Debug.Log("OnDownloadRecordFileComplete error");
        }
    }
    private void M_voiceengine_OnPlayRecordFilComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath)
    {
        Debug.Log("OnPlayRecordFilComplete c# callback");
        if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_PLAYFILE_DONE)
        {
            Debug.Log("OnPlayRecordFilComplete succ, filepath:" + filepath);
        }
        else
        {
            Debug.Log("OnPlayRecordFilComplete error");
        }
    }
    #endregion
    
    #region RealTime VoiceMessage

    private bool bHaveJoin = false;
    private bool bIsMicOpen = false;
    private bool bIsSpeakerOpen = false;
    private void InitRealTimeVoiceEvent()
    {
        if (m_voiceengine.SetMode(GCloudVoiceMode.RealTime) == 0)
        {

            Debug.Log("切换实时语音成功");
            m_voiceengine.OnJoinRoomComplete += M_voiceengine_OnJoinRoomComplete;
            m_voiceengine.OnQuitRoomComplete += M_voiceengine_OnQuitRoomComplete;
            m_voiceengine.OnMemberVoice += M_voiceengine_OnMemberVoice;
        }
        else {
            GameDebuger.LogError("初始化实时语音失败");
        }
    }

    private void ClearRealTimeVoiceEvent()
    {
        m_voiceengine.OnJoinRoomComplete -= M_voiceengine_OnJoinRoomComplete;
        m_voiceengine.OnQuitRoomComplete -= M_voiceengine_OnQuitRoomComplete;
        m_voiceengine.OnMemberVoice -= M_voiceengine_OnMemberVoice;
    }

    public bool OpenMic()
    {
        var code = m_voiceengine.OpenMic();
        if (code != 0)
            GameDebuger.Log("打开麦克风失败 code : " +code);
        return code == 0; //开启麦克风
    }
    public bool CloseMic()
    {
        var code = m_voiceengine.CloseMic();
        if (code != 0)
            GameDebuger.Log("关闭麦克风失败 code : " + code);
        return code == 0; //关闭麦克风
    }
    public bool OpenSpeaker()
    {
        var code = m_voiceengine.OpenSpeaker();
        if (code != 0)
            GameDebuger.Log("打开接收器失败 code : " + code);
        return code == 0; 
    }
    public bool CloseSpeaker()
    {
        var code = m_voiceengine.OpenSpeaker();
        if (code != 0)
            GameDebuger.Log("关闭接收器失败 code : " + code);
        return code == 0;
    }
    public bool JoinRoom(string roomName)
    {
        var code = m_voiceengine.JoinTeamRoom(roomName, 15000);
        if(code != 0)
        {
            GameDebuger.Log("加入语音房间失败 code : " + code);
        }
        return code == 0;
    }
    public bool QuitRoom(string roomName)
    {
        var code = m_voiceengine.QuitRoom(roomName, 15000);
        if (code != 0)
        {
            GameDebuger.Log("加入语音房间失败 code : " + code);
        }
        return code == 0;
    }
    private void M_voiceengine_OnJoinRoomComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID)
    {
        if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_JOINROOM_SUCC)
        {
            Debug.Log("OnJoinRoomComplete ret=" + code + " roomName:" + roomName + " memberID:" + memberID);
            bHaveJoin = true;


            OpenMic();
            OpenSpeaker();
        }
    }
    private void M_voiceengine_OnQuitRoomComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID)
    {
        bHaveJoin = false;
        Debug.Log("OnQuitRoomComplete ret=" + code + " roomName:" + roomName + " memberID:" + memberID);
    }
    private void M_voiceengine_OnMemberVoice(int[] members, int count)
    {

    }
    #endregion


    public void ChangeMode(GCloudVoiceMode mode)
    {
        GameDebuger.Log("GCloudeVoice ChangeMode :" + mode.ToString());
        if (currentMode == mode)
            return;
        switch(currentMode)
        {
            case GCloudVoiceMode.RealTime:
                ClearRealTimeVoiceEvent();
                break;
            case GCloudVoiceMode.Messages:
                ClearOfflineMessageEvent();
                break;
        }

        currentMode = mode;

        switch(mode)
        {
            case GCloudVoiceMode.RealTime:
                InitRealTimeVoiceEvent();
                break;
            case GCloudVoiceMode.Messages:
                InitOfflineMessageEvent();
                break;
        }
    }
}
