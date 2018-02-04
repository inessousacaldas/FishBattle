using UnityEngine;
using System.Collections;
using UniRx;
using AppDto;

public class CopyPanelItemViewController:MonolessViewController<CopyPanelItemView>
{
    public int tCopyID = 0;
    private Subject<CopyItemData> _clickEvt = new Subject<CopyItemData>();
    public UniRx.IObservable<CopyItemData> GetClickEvt { get { return _clickEvt; } }
    private string selectIcon = "bg_Option_1Select";
    private string UnselectIcon = "bg_Option_1Normal";
    private CopyItemData mCopyItemData;
    public void Init(int index,Copy copy) {
        tCopyID = index;
        EventDelegate.Add(View.CopyViewItem_UIButton.onClick,OnClickHandler);
        View.CopyName_UILabel.text = copy.name;
        View.CopyLevel_UILabel.text = copy.minGrade.ToString();
        View.CopyDeslLabel_UILabel.GetComponent<UIWidget>().UpdateAnchors();
        //UIHelper.SetSkillIcon(View.BossHead_UISprite,"Headicon_CopyPanal_" + copy.modelId);
        View.BossHead_UISprite.spriteName = "Headicon_CopyPanal_" + copy.modelId;
        if(copy.refreshId == 0)
            View.EliteIcon_UISprite.spriteName = "";
        else if(copy.refreshId == 1)
            View.EliteIcon_UISprite.spriteName = "Elite";
        mCopyItemData = new CopyItemData(tCopyID,copy);
    }

    public void OnClickHandler() {
        _clickEvt.OnNext(mCopyItemData);
    }


    protected override void OnDispose() {
        tCopyID = 0;
        View.CopyName_UILabel.text = "";
        View.SelectIcon_UISprite.spriteName = UnselectIcon;
        mCopyItemData = null;
    }

    public void ChangeSlectStyle(int index) {
        if(tCopyID == index)
        {
            View.SelectIcon_UISprite.spriteName = selectIcon;
        }
        else {
            View.SelectIcon_UISprite.spriteName = UnselectIcon;
        }
    }
}

public class CopyItemData {
    public int mCopyID;

    public Copy mCopy;
    public CopyItemData(int id,Copy copy) {
        mCopyID = id;
        mCopy = copy;
    }
}
