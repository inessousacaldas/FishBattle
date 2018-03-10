using UnityEngine;
using System.Collections;

public class QRCodeDownloadViewController : MonoViewController<QRCodeDownloadView>
{
    public void SetData()
    {
        
    }

    protected override void AfterInitView ()
    {
        View.DownloadTip_UILabel.text = string.Format("[{0}]搜索[{1}]{2}[-]进行下载[-]",
            ColorConstantV3.Color_SealBrown_Str, ColorConstantV3.Color_Green_Strong_Str, GameSetting.GameName);
    }

    protected override void RegistCustomEvent ()
    {
        base.RegistCustomEvent();

        EventDelegate.Set(View.CloseBtn_UIButton.onClick, OnCloseBtnClick);
    }

    private void OnCloseBtnClick()
    {
        ProxyQRCodeModule.CloseQRCodeDownloadView();
    }
}
