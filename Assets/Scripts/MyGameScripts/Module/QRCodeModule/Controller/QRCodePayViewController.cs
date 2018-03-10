using UnityEngine;
using System.Collections;

public class QRCodePayViewController: MonoViewController<QRCodePayView>
{
    private OrderItemJsonDto _itemDto;
    private int _quantity;
    private OrderJsonDto _orderDto;

    public void SetData(OrderItemJsonDto itemDto, int quantity, OrderJsonDto orderDto)
    {
		_itemDto = itemDto;
        _quantity = quantity;
		_orderDto = orderDto;

            

        UpdateQRCodeTexture();
    }


    protected override void AfterInitView ()
    {
        View.Title_UILabel.text = string.Format("请使用{0}移动端扫码支付", GameSetting.GameName);

    }

    protected override void RegistCustomEvent ()
    {
        base.RegistCustomEvent();
        EventDelegate.Set(View.EnsureBtn_UIButton.onClick, OnReturnBtnClick);
    }

    private void UpdateQRCodeTexture()
    {
        View.QRTexture_UITexture.mainTexture =
            AntaresQRCodeUtil.Encode(QRCodeHelper.EncodeProductQRCode(_itemDto, _quantity, _orderDto),
                View.QRTexture_UITexture.width);

        View.OverTimeGroup_Transform.gameObject.SetActive(false);
    }

    private void OnReturnBtnClick()
    {
        ProxyQRCodeModule.CloseQRCodePayView();
    }
}
