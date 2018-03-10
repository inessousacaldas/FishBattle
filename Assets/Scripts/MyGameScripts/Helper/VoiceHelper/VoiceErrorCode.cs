public class VoiceErrorCode
{
    public const int NONE = 0;
    public const int ERROR_SHORT_VOICE = 1; // 语言太短
    public const int ERROR_NO_DATA = 2; // 没有录音数据
    public const int ERROR_TO_AMR = 3; // 转Amr失败
    public const int ERROR_LOAD_FILE = 4; // 加载音频文件失败
    public const int ERROR_SEND_FILE = 5; // 上传音频文件失败
    public const int ERROR_NO_MATCH = 6; // 没有语言识别结果
}