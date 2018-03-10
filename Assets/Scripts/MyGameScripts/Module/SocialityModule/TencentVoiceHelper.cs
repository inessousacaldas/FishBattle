using System;
using QCloud.CosApi.Api;
using LITJson;

/// <summary>
/// 注意，这里使用的是同步的方式~如果使用多线程，请自行分配消息
/// </summary>
public class TencentVoiceHelper {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileUrl">返回文件的路径</param>
    public delegate void SaveSuccess(string fileUrl);
    public delegate void Failed();
    public static void UpLoadVoice(byte[] _amrData,string fileKey, SaveSuccess onVoiceSaveSuccess, Failed onVoiceFailed)
    {
        GameDebuger.Log("开始发送到腾讯云");
        UpLoadFile(_amrData, fileKey, GameConfig.TENCENT_COS_VOICE_Folder, onVoiceSaveSuccess, onVoiceFailed);
    }



    public static void UpLoadFile(byte[] data,string fileName,string remotePath, SaveSuccess onSuccess,Failed onFailed)
    {
        CosCloud cos = new CosCloud(GameConfig.TENCENT_COS_APP_ID, GameConfig.TENCENT_COS_SECRET_ID, GameConfig.TENCENT_COS_SECRET_KEY, GameConfig.TENCENT_COSAPI_CGI_URL, 10);
        try
        {
            string res = cos.Upload(GameConfig.TENCENT_COS_BUCKET, data, fileName, remotePath + fileName);
            GameDebuger.LogError("SaveVoice : " + res);
            JsonData jo = JsonMapper.ToObject(res);
            if (jo["code"] != null)
            {
                var code = (int)jo["code"];
                var message = (string)jo["message"];
                //上传成功
                if (code >= 0 || message.Equals("SUCCESS"))
                {
                    var url = (string)jo["data"]["source_url"];

                    if (onSuccess != null)
                        onSuccess(url);
                }
                //上传失败
                else
                {
                    if (onFailed != null)
                        onFailed();
                }

            }
        }
        catch (Exception e)
        {
            GameDebuger.LogException(e);
            if (onFailed != null)
                onFailed();
        }
    }
}
