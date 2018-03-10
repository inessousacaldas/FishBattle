using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;

public partial class TeamApplicationUnitedItemController: MonolessViewController<TeamApplicationUnitedItem>
{
    private const int ColNum = 2;
    private int _lineNum;

    private Subject<int> _subject;

    private CompositeDisposable _disposable;
    
    public Subject<int> GetSubject { get { return _subject;} }

    private List<TeamApplicationItemController>  _controllerList ;

    protected override void AfterInitView()
    {
        _disposable = new CompositeDisposable();
        _controllerList = new List<TeamApplicationItemController>();
        for (int i = 0; i < ColNum; i++)
        {
            var controller = AddChild<TeamApplicationItemController, TeamApplicationItem>(
                View.ItemGrid_UIGrid.gameObject
                , TeamApplicationItem.NAME);
            _controllerList.Add(controller);
            var idx = i;
            _disposable.Add(controller.OnClickHandler.Subscribe(_ =>
            {
                var index = _lineNum * ColNum + idx;
                _subject.OnNext(index);
            }));
        }
        if (_subject == null)
            _subject = new Subject<int>();
    }

    #region TeamRequestNotify
    public void UpdateItemInfo(IEnumerable<TeamRequestNotify> dataSet, int lineNum)
    {
        if (dataSet == null)
            return;

        _lineNum = lineNum;
        _controllerList.ForEach(item => { item.gameObject.SetActive(false); });
        dataSet.ForEachI((dto, i) =>
        {
            var index = ColNum * lineNum + i;
            _controllerList[i].gameObject.SetActive(true);
            _controllerList[i].UpdateInfo(dto, index, "同意");
        });
    }
    #endregion

    #region TeamPlayerDto
    public void UpdateItemInfo(IEnumerable<TeamPlayerDto> dataSet, int lineNum)
    {
        if (dataSet == null) return;

        _controllerList.ForEach(item => { item.gameObject.SetActive(false); });
        _lineNum = lineNum;
        dataSet.ForEachI((dto, i) =>
        {
            _controllerList[i].gameObject.SetActive(dto != null);
            if (dto == null)
                return;

            _controllerList[i].UpdateInfo(dto);
        });
    }
    #endregion

    #region 公会成员
    public void UpdateItemInfo(IEnumerable<GuildMemberDto> data, int lineNum)
    {
        if (data == null)
            return;

        _controllerList.ForEach(item => { item.gameObject.SetActive(false); });
        _lineNum = lineNum;
        data.ForEachI((dto, i) =>
        {
            _controllerList[i].gameObject.SetActive(dto != null);
            if (dto == null)
                return;

            _controllerList[i].SetGuildMember(dto);
        });
    }
    #endregion


    protected override void OnDispose()
    {
        _subject.CloseOnceNull();
        _disposable = _disposable.CloseOnceNull();
    }
}
