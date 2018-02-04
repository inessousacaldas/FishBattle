using UnityEngine;
using System.Collections;

public class QRCodeWaitPayViewController: MonoViewController<QRCodeWaitPayView>
{
    public void SetData()
    {
        
    }

    protected override void RegistCustomEvent ()
    {
        base.RegistCustomEvent();

        EventDelegate.Set(View.ReturnBtn_UIButton.onClick, OnReturnBtnClick);
        EventDelegate.Set(View.HelpBtn_UIButton.onClick, OnHelpBtnClick);
    }


    private void OnReturnBtnClick()
    {
        ProxyQRCodeModule.CloseQRCodeWaitPayView();
    }


    private void OnHelpBtnClick()
    {
        Application.OpenURL("http://xl.tiancity.com/homepage/article/2016/05/09/44684.html");
    }
}
