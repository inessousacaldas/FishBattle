using System;
using UnityEngine;

public class BuiltInDialogueViewController : MonoBehaviour
{
    public const string BuiltInDialogueViewPath = "Built-inAssets/BuiltInDialogueView";
    private static BuiltInDialogueViewController _instance;

    public Action _cancelHandler;
    public Action _okHandler;

    private UISprite SimpleWin;
    private UIButton simple_RightButton;
    private UILabel simple_RightLabel;
    private UILabel simple_TitleLabel;
    private UILabel simple_InfoLabel;
    private UIButton simple_LeftButton;
    private UILabel simple_LeftLabel;
    private UIToggle auto_Toggle;

    private const int infoHeigh = 72;
    private const int pushHeight = 25;//有push btn下移像素
    private const int pushHeigher = 35; //push高出btn像素
    private Vector3 OKbtnPos;
    private Vector3 CancelbtnPos;

    private long _time;

    public static BuiltInDialogueViewController OpenView(string msg,
        Action okHandler = null,
        Action cancelHandler = null,
        UIWidget.Pivot pivot = UIWidget.Pivot.Center,
        string okLabelStr = "确定", string cancelLblStr = "取消")
    {
        if (_instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>(BuiltInDialogueViewPath);
            GameObject module = NGUITools.AddChild(UICamera.eventHandler.gameObject, prefab);
            var com = module.AddMissingComponent<BuiltInDialogueViewController>();
            com.InitView();
            _instance = com;
        }
        _instance.Open(msg, okHandler, cancelHandler, pivot, okLabelStr, cancelLblStr);

        return _instance;
    }

    public void Open(string msg,
        Action okHandler,
        Action cancelHandler,
        UIWidget.Pivot pivot,
        string okLabelStr, string cancelLblStr)
    {
        //simple_InfoLabel.pivot = pivot;
        simple_InfoLabel.text = msg;
     
        if (simple_InfoLabel.height > infoHeigh)
        {
            SimpleWin.height += simple_InfoLabel.height - infoHeigh;
            simple_RightButton.transform.localPosition -= new Vector3(0.0f, simple_InfoLabel.height - infoHeigh, 0.0f);
            simple_LeftButton.transform.localPosition -= new Vector3(0.0f, simple_InfoLabel.height - infoHeigh, 0.0f);
        }

        _okHandler = okHandler;
        _cancelHandler = cancelHandler;

        simple_LeftLabel.text = okLabelStr;
        simple_LeftLabel.spacingX = GetLabelSpacingX(okLabelStr);

        if (_cancelHandler != null)
        {
            simple_RightLabel.text = cancelLblStr;
            simple_RightLabel.spacingX = GetLabelSpacingX(cancelLblStr);
            simple_RightButton.gameObject.SetActive(true);
        }
        else
        {
            simple_LeftButton.transform.localPosition = new Vector3(0, -48, 0);
            simple_RightButton.gameObject.SetActive(false);
        }
    }

    public void InitView()
    {
        Transform root = transform;
        simple_TitleLabel = root.Find("SimpleWin/simple_TitleLabel").GetComponent<UILabel>();
        simple_InfoLabel = root.Find("SimpleWin/simple_InfoLabel").GetComponent<UILabel>();
        simple_LeftButton = root.Find("SimpleWin/LeftBtn").GetComponent<UIButton>();
        simple_LeftLabel = root.Find("SimpleWin/LeftBtn/LeftLabel").GetComponent<UILabel>();
        simple_RightButton = root.Find("SimpleWin/RightBtn").GetComponent<UIButton>();
        simple_RightLabel = root.Find("SimpleWin/RightBtn/RightLabel").GetComponent<UILabel>();
        auto_Toggle = root.Find("SimpleWin/autoMatchToggle").GetComponent<UIToggle>();
        SimpleWin = root.Find("SimpleWin").GetComponent<UISprite>();

        CancelbtnPos = simple_RightButton.transform.localPosition;
        OKbtnPos = simple_LeftButton.transform.localPosition;

        RegisterEvent();
    }

    public void RegisterEvent()
    {
        EventDelegate.Set(simple_LeftButton.onClick, OnClickLeftButton);
        EventDelegate.Set(simple_RightButton.onClick, OnClickRightButton);
        EventDelegate.Set(auto_Toggle.onChange, OnAutoClick);
    }

    public void Dispose()
    {
    }

    private void OnClickLeftButton()
    {
        simple_RightButton.transform.localPosition = CancelbtnPos;
        simple_LeftButton.transform.localPosition = OKbtnPos;

        CloseView();

        if (_okHandler != null)
        {
            _okHandler();
        }

        if (auto_Toggle.value)
            PlayerPrefs.SetString("SevenDayNoPushTips", _time.ToString());
    }

    private void OnClickRightButton()
    {
        CloseView();

        if (_cancelHandler != null)
        {
            _cancelHandler();
        }
    }

    private void OnAutoClick()
    {
        var pp = auto_Toggle.value;
    }

    private int GetLabelSpacingX(string text)
    {
        if (text.Length <= 2)
        {
            return 12;
        }
        if (text.Length <= 3)
        {
            return 6;
        }
        return 1;
    }

    private void CloseView()
    {
        if (_instance != null)
        {
            _instance.Dispose();
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    public void SetTitle(string str)
    {
        simple_TitleLabel.text = str;
    }

    public void SetIsPush(bool isPush, long time)
    {
        _time = time;
        if(isPush)
        {
            SimpleWin.height += pushHeight;
            simple_RightButton.transform.localPosition -= new Vector3(0, pushHeight);
            simple_LeftButton.transform.localPosition -= new Vector3(0, pushHeight);
            auto_Toggle.transform.localPosition = new Vector3(auto_Toggle.transform.localPosition.x, simple_LeftButton.transform.localPosition.y + pushHeigher);
            auto_Toggle.gameObject.SetActive(true);
        }
        else
            auto_Toggle.gameObject.SetActive(false);
    }
}