using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using AppDto;

namespace AppDto
{  
    /** 跑马灯公告通知 */
    //public class MarqueeNoticeNotify 
    //{
    //    // 系统
    //    public const int MarqueeNoticeType_System = 0;
    
    //    // 业务
    //    public const int MarqueeNoticeType_Business = 1;
    

    //    /** 标题 */
    //    public string title;
    
    //    /** 内容 */
    //    public string content;
    
    //    /** 公告类型 MarqueeNoticeType */
    //    public int noticeType;
    

    //}

}

public partial class FullServiceAnnouncementView{
    #region 顶部全服公告

    //与前一条之间的距离偏移
    private const float OFFSET = 20f;
    //倍数
    private const int MULTIPLE = 5;
    //每秒移动的距离
    private const int SPEED = 80;

    private UILabel _lastShowAnnouncementLbl;
   
    protected override void OnDispose(){
        _lastShowAnnouncementLbl = null;
        AnnounceLblsBg_UISprite.cachedGameObject.RemoveChildren();
        _fullServiceAnnouncementLblQueue.Clear();
        AnnounceLblsBg_UISprite.cachedGameObject.SetActive(false);
    }
        
    #endregion
    public float CalLabelTweenFinish(){
        if (_lastShowAnnouncementLbl == null) {
            return 0f;
        } else {
            return (_lastShowAnnouncementLbl.transform.localPosition.x + _lastShowAnnouncementLbl.printedSize.x + FullServiceAnnouncementView_UIPanel.width * 0.5f)/ SPEED;
        }
            
    }
    public float CalculateNextShowTime(){
        if (_lastShowAnnouncementLbl != null) {
            var time = ((_lastShowAnnouncementLbl.transform.localPosition.x 
                         + _lastShowAnnouncementLbl.printedSize.x
                         + MULTIPLE * OFFSET)
                        - FullServiceAnnouncementView_UIPanel.width * 0.5f)
                       / SPEED ;
            return Math.Max (0f, time);
        }
        else{
            return 0f;
        }
    }

    public bool CheckCanShowAnnouncement(out float leftTime){
        leftTime = CalculateNextShowTime ();
        return _lastShowAnnouncementLbl == null
               ||  (leftTime - 0.001f) <= 0;
    }

    public float ShowAnnouncement(MarqueeNoticeNotify marqueeNoticeNotify)
    {
        if (marqueeNoticeNotify != null) {
            AnnounceLblsBg_UISprite.cachedGameObject.SetActive (true);

            UILabel label = PopLabel ();

            label.gameObject.SetActive (true);
            label.spacingY = 0;
            label.overflowMethod = UILabel.Overflow.ResizeFreely;
            label.text = marqueeNoticeNotify.title + marqueeNoticeNotify.content;
            label.pivot = UIWidget.Pivot.Left;
            label.transform.localPosition =
                new Vector3 (FullServiceAnnouncementView_UIPanel.width * 0.5f,
                    0,
                    0);
            float time = (label.printedSize.x + FullServiceAnnouncementView_UIPanel.width) /
                         SPEED;

            Tweener tweener =
                label.transform.DOLocalMoveX (
                        label.transform.localPosition.x -
                        (label.printedSize.x + FullServiceAnnouncementView_UIPanel.width) - OFFSET, time)
                    .SetEase (Ease.Linear);

            tweener.OnComplete (() => {
                PushLabel (label);
            });
            tweener.SetAutoKill (true);
            _lastShowAnnouncementLbl = label;
        } else {
            AnnounceLblsBg_UISprite.cachedGameObject.SetActive (false);
            _lastShowAnnouncementLbl = null;
        }
        var leftTime = 0f;
        CheckCanShowAnnouncement(out leftTime);
        return leftTime;
    }

    #region 缓冲池

    private Queue<UILabel> _fullServiceAnnouncementLblQueue = new Queue<UILabel>();

    public UILabel PopLabel()
    {
        return _fullServiceAnnouncementLblQueue.Count > 0
            ? _fullServiceAnnouncementLblQueue.Dequeue()
            : UIHelper.AddDescLbl(AnnounceLblsBg_UISprite.cachedGameObject, MarqueeLbl_UILabel.bitmapFont, "");
    }

    public void PushLabel(UILabel label)
    {
        label.gameObject.SetActive(false);
        _fullServiceAnnouncementLblQueue.Enqueue(label);
    }
    #endregion
}