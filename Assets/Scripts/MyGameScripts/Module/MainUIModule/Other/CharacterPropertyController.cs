using UnityEngine;
using System.Collections.Generic;
using AppDto;
using System;
using System.Linq;
using UniRx;

public interface ICharacterPropertyController
{
	UniRx.IObservable<Unit> OnIconBtnClick { get; }
    UniRx.IObservable<Unit> OnBuffBtnClick { get; }
    void UpdateBuffList(IEnumerable<int> buffList = null);
}

public class CharacterPropertyController : MonolessViewController<CharacterPropertyView>,ICharacterPropertyController
{
    private CompositeDisposable _disposable = new CompositeDisposable();
	private List<UISprite> _buffIconList = new List<UISprite>(4);   //最多显示4个Buff类型

    private Subject<Unit> _onIconBtnClickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnIconBtnClick{ get { return _onIconBtnClickEvt; }}

    private Subject<Unit> _onBuffBtnClickEvt = new Subject<Unit>(); 
    public UniRx.IObservable<Unit> OnBuffBtnClick { get { return _onBuffBtnClickEvt; } }
    protected override void AfterInitView()
    {
        for (int i = 0; i < _view.BuffGroup_UIGrid.transform.childCount; i++)
        {
            var go = _view.BuffGroup_UIGrid.GetChild(i);
            var sprite = go.GetComponent<UISprite>();
            var btn = go.GetComponent<UIButton>();
            _buffIconList.Add(sprite);
            _disposable.Add(btn.AsObservable().Subscribe(_ => { OnClickBuffBtn(); }));
        }
    }

    protected override void InitReactiveEvents()
    {
        base.InitReactiveEvents();
    }

    protected override void ClearReactiveEvents()
    {
        base.ClearReactiveEvents();
    }

    public void ChangeMode(UIMode mode)
    {
        this.gameObject.SetActive(mode != UIMode.BATTLE);
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

	public void UpdateBuffList(IEnumerable<int> buffList = null)
	{
	    if (buffList == null) return;

        _buffIconList.ForEachI((sprite, idx)=>
        {
            if (idx < buffList.Count())
            {
                sprite.gameObject.SetActive(true);
                UIHelper.SetItemIcon(sprite, "");
            }
            else
                sprite.gameObject.SetActive(false);
        });
		View.BuffGroup_UIGrid.Reposition();
	}

	private void OnClickBuffBtn()
    {
        _onBuffBtnClickEvt.OnNext(new Unit());
    }

    protected override void RegistCustomEvent()
    {
        _disposable.Add(View.PlayerIcon_UIButton.AsObservable().Subscribe(_ => { _onIconBtnClickEvt.OnNext(new Unit()); }));
        base.RegistCustomEvent();
    }

	public void SetIcon(int iconId)
    {
		UIHelper.SetPetIcon(View.PlayerIcon_UISprite,iconId.ToString());
	}

	public void SetIcon(string icon)
    {
		UIHelper.SetPetIcon(View.PlayerIcon_UISprite, icon);
	}

	public void SetLvLbl(int lv)
    {
		View.Lv_UILabel.text = string.Format("[b]{0}[-]",lv);
	}

    public void UpdateView(IMainUIData data)
    {
        
    }
}
