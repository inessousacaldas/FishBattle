using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

public class FullServiceAnnouncementModel
{
    private static Subject<MarqueeNoticeNotify> stream;
    public static UniRx.IObservable<MarqueeNoticeNotify> Stream {
        get
        {
            if (stream == null)
            {
                stream = new Subject<MarqueeNoticeNotify>();
            }
            return stream;
        }
    }
    private static IDisposable _disposable;
    private static FullServiceAnnouncementModel _ins;
    public static FullServiceAnnouncementModel Instance {
        get
        {
            if (_ins == null)
            {
                _ins = new FullServiceAnnouncementModel();
                if (stream == null)
                {
                    stream = new Subject<MarqueeNoticeNotify>();
                }
                _disposable = NotifyListenerRegister.RegistListener<MarqueeNoticeNotify>(_ins.HandleMarqueeNoticeNotify);
            }
            
            return _ins;
        }
    }

    public void HandleMarqueeNoticeNotify(MarqueeNoticeNotify notify)
    {
        
    }

    private const int SystemAnnouncementMaxCount = 10;
    private const int BusinessAnnouncementMaxCount = 10;

    private Queue<MarqueeNoticeNotify> _systemAnnouncementQueue = new Queue<MarqueeNoticeNotify>();
    private Queue<MarqueeNoticeNotify> _businessAnnouncementQueue = new Queue<MarqueeNoticeNotify>();
	
    public event Action updateAnnouncement;

    public void HandleNotify(MarqueeNoticeNotify notify)
    {
//        if ( ModelManager.Marry.IsIMarry(ModelManager.Player.GetPlayerId()) )
//        {
//            GameDebuger.Log("本人正在结婚，不显示跑马灯");
//            return;
//        }
//        if (ModelManager.BridalSedan.IsMe())
//        {
//            return;
//        }

        if (string.IsNullOrEmpty(notify.title))
        {
            notify.title = "";
            GameDebuger.Log("MarqueeNoticeNotify 标题为空");

            if (string.IsNullOrEmpty(notify.content))
            {
                UnityEngine.Debug.LogError("MarqueeNoticeNotify 标题和内容为空");
                return;
            }
        }
        if (string.IsNullOrEmpty(notify.content))
        {
            notify.content = "";
            GameDebuger.Log("MarqueeNoticeNotify 内容为空");
        }

        GameDebuger.Log("收到跑马灯,标题:" + notify.title + ",内容:" + (notify.content.Length > 20 ? notify.content.Substring(0, 20) : notify.content));

        if (notify.noticeType == (int)MarqueeNoticeNotify.MarqueeNoticeType.System)
        {
            if (_systemAnnouncementQueue.Count >= SystemAnnouncementMaxCount)
                _systemAnnouncementQueue.Dequeue();

            _systemAnnouncementQueue.Enqueue(notify);
        }
        else
        {
            if (_businessAnnouncementQueue.Count >= BusinessAnnouncementMaxCount)
                _businessAnnouncementQueue.Dequeue();
            _businessAnnouncementQueue.Enqueue(notify);
        }
		
        if (updateAnnouncement != null)
        {
            updateAnnouncement();
        }
    }

    public MarqueeNoticeNotify GetMarqueeNoticeNotify()
    {
        MarqueeNoticeNotify marqueeNoticeNotify = null;
        if (_systemAnnouncementQueue.Count > 0)
        {
            marqueeNoticeNotify = _systemAnnouncementQueue.Dequeue();
        }
        else
        {
            if (_businessAnnouncementQueue.Count > 0)
            {
                marqueeNoticeNotify = _businessAnnouncementQueue.Dequeue();
            }
        }

        if (marqueeNoticeNotify != null)
        {
            marqueeNoticeNotify.title = marqueeNoticeNotify.title.RemoveNewline();
            marqueeNoticeNotify.content = marqueeNoticeNotify.content.RemoveNewline();

            stream.OnNext(marqueeNoticeNotify);
        }
        return marqueeNoticeNotify;
    }

    public void Dispose()
    {
        _systemAnnouncementQueue.Clear();
        _businessAnnouncementQueue.Clear();
        _disposable.Dispose();
        _disposable = null;
        stream = stream.CloseOnceNull();
    }
}
