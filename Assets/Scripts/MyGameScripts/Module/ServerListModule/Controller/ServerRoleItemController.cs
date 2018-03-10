using System;
using UnityEngine;
using AppDto;

public class ServerRoleItemController : MonolessViewController<ServerRoleItem>
{
    private AccountPlayerDto _accountPlayerDto;

    public AccountPlayerDto PlayerDto
    {
        get { return _accountPlayerDto; }
    }

    #region IViewController

    

protected override void RegistCustomEvent ()
	{
		base.RegistCustomEvent ();        EventDelegate.Set(_view.DelButton_UIEventTrigger.onDrag, () =>
        {
            return;
			if (_accountPlayerDto != null) {
	            Vector2 delta = UICamera.currentTouch.delta;
	            if (delta.x > 2)
	            {
	                ShowDeleteBtn(false);
				}
				
			}
        });
        EventDelegate.Set(_view.BgSprite_UIEventTrigger.onDrag, delegate
        {
            return;
			if (_accountPlayerDto != null) {
	            Vector2 delta = UICamera.currentTouch.delta;
	            if (delta.x < -2)
	            {
	                ShowDeleteBtn(true);
	            }
	            if (delta.x > 2)
	            {
	                ShowDeleteBtn(false);
	            }
			}
        });
    }
    



    #endregion

    public void SetData(AccountPlayerDto accountPlayerDto, Action<ServerRoleItemController> onClickBg,
        Action<ServerRoleItemController> onClickDeletBtn)
    {
        _accountPlayerDto = accountPlayerDto;
        if (accountPlayerDto == null)
        {
            _view.NameLabel_UILabel.text = "";
            _view.FactionLabel_UILabel.text = "";
            _view.LvLabel_UILabel.cachedGameObject.SetActive(false);
            _view.DelButton_UIButton.gameObject.SetActive(false);
            _view.RoleIconSprite_UISprite.spriteName = "";
            _view.DelTipSprite.SetActive(false);
            _view.AddRoleISprite.SetActive(true);
        }
        else
        {
            _view.NameLabel_UILabel.text = accountPlayerDto.nickname;
            _view.FactionLabel_UILabel.text = GameHintManager.GetFactionName(accountPlayerDto.factionId);
            _view.LvLabel_UILabel.text = accountPlayerDto.grade.ToString();
            _view.LvLabel_UILabel.cachedGameObject.SetActive(true);
            _view.DelButton_UIButton.gameObject.SetActive(false);
            _view.DelTipSprite.SetActive(false);
            _view.AddRoleISprite.SetActive(false);
			UIHelper.SetPetIcon(_view.RoleIconSprite_UISprite, GetCharactorIcon(accountPlayerDto));
            if (accountPlayerDto.id == GameSetting.GetLastRolePlayerId())
            {
                //_view.BgSprite_UISprite.spriteName = "bg_Title";
                View.SelSp_Go.SetActive(true);
            }
            else
            {
                //_view.BgSprite_UISprite.spriteName = "bg_14";
                View.SelSp_Go.SetActive(false);
            }
        }

        EventDelegate.Set(_view.BgSprite_UIEventTrigger.onClick, () =>
        {
            if (onClickBg != null)
                onClickBg(this);
        });
        EventDelegate.Set(_view.DelButton_UIEventTrigger.onClick, () =>
        {
            if (onClickDeletBtn != null)
                onClickDeletBtn(this);
        });
    }

	private string GetCharactorIcon(AccountPlayerDto accountPlayerDto)
	{
		if (accountPlayerDto.charactorId > 0)
		{
			MainCharactor tCharactor = DataCache.getDtoByCls<GeneralCharactor>(accountPlayerDto.charactorId) as MainCharactor;
			if (tCharactor != null)
			{
				return tCharactor.texture.ToString();
			}
			else
			{
				return accountPlayerDto.icon.ToString();	
			}
		}
		else
		{
			return accountPlayerDto.icon.ToString();
		}
	}

    public void ShowDeleteBtn(bool show)
    {
        _view.DelButton_UIButton.gameObject.SetActive(show);
        _view.NameLabel_UILabel.gameObject.SetActive(!show);
        _view.FactionLabel_UILabel.gameObject.SetActive(!show);
        _view.DelTipSprite.SetActive(show);
    }
}