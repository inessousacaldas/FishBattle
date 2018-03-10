using UnityEngine;
using System.Collections.Generic;

public class EmojiAnimationController : UILabel
{

    #region 表情动画相关

    private const string EMOTION_FORMAT = "{0}_{1:00}";
    private System.Text.StringBuilder _strBuilder;
    //用于保存当前动态表情的数据,prefix,curFrameCount
    private Dictionary<string, int> _dynamicEmojiDic;
    private const float FRAME_INTERVAL = 0.25f;


    private readonly string TimerName;



    public EmojiAnimationController()
    {
        TimerName = "TimerName" + this.GetInstanceID();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="content">内容</param>
    /// <returns>是否包含表情</returns>
    public bool SetEmojiText(string content,UILabel mLabel = null)
    {
        JSTimer.Instance.CancelTimer(TimerName);
        //如果文本内容中没有“#”，直接设置文本内容
        if (!content.Contains(EmojiDataUtil.EMOTION_PREFIX))
        {
            _strBuilder = null;
            _dynamicEmojiDic = null;
            this.text = content;
            if(mLabel != null)
                mLabel.text = content;
            return false;
        }
        else
        {
            //初始化所有聊天表情动画帧数数据
            EmojiDataUtil.InitEmojiInfo(this.bitmapFont.emojiFont.atlas);

            //默认消息中只带表情symbol名的前缀，只替换动态表情的前缀名
            //原消息：#14#1abc#28
            //替换后：#14_00#1_00abc#28
            _dynamicEmojiDic = new Dictionary<string, int>(EmojiDataUtil.MAX_EMOJICOUNT);
            _strBuilder = new System.Text.StringBuilder(EmojiDataUtil.ReplaceEmojiPrefix(content, _dynamicEmojiDic));
            this.text = _strBuilder.ToString();

            PlayerEmotionAnimation();

            JSTimer.Instance.SetupCoolDown(
                TimerName
            , FRAME_INTERVAL
            , null
            , StartPlay);
            return true;
        }
    }

    private void StartPlay()
    {
        JSTimer.Instance.SetupCoolDown(
            TimerName
            , FRAME_INTERVAL
            , null
            , () =>
            {
                PlayerEmotionAnimation();
                StartPlay();
            });
    }

    private void PlayerEmotionAnimation()
    {
        if (_dynamicEmojiDic == null || this == null)
            return;
        var keyList = new List<string>(_dynamicEmojiDic.Keys);
        for (int i = 0; i < keyList.Count; ++i)
        {
            string prefix = keyList[i];
            int curFrame = _dynamicEmojiDic[prefix];
            int nextFrame = (curFrame + 1) < EmojiDataUtil.GetEmotionMaxFrameCount(prefix) ? curFrame + 1 : 0;
            _strBuilder.Replace(string.Format(EMOTION_FORMAT, prefix, curFrame), string.Format(EMOTION_FORMAT, prefix, nextFrame));
            _dynamicEmojiDic[prefix] = nextFrame;
        }
        this.text = _strBuilder.ToString();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        JSTimer.Instance.CancelTimer(TimerName);
    }
    #endregion
}
