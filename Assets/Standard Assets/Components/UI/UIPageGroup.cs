using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;

public class UIPageGroup : MonoBehaviour
{
    [SerializeField]
    private UIAtlas atlas;

    public UIAtlas Atlas
    {
        get { return atlas; }
        set
        {
            if (value != atlas)
            {
                AssetManager.RemoveAtlasRef(atlas, this);
                atlas = value;
                AssetManager.AddAtlasRef(atlas, this);
            }
        }
    }
    public string under = "Page_Point_normal";
    public string choice = "Page_Point_light";
    UIGrid mGrid; //当前页面管理的Grid, 这个Grid只是用于’点‘图片排版的//
    Transform mCachedTransform;
    List<UISprite> mPages; //所有的点图片//

    //维护一个当前页的变量//
    private int _mCurrentPage;
    public int mCurrentPage{ 
        get{return _mCurrentPage;}
        set{
            _mCurrentPage = value;
        }
    }

    void Awake()
    {
        AssetManager.AddAtlasRef(atlas, this);
        mGrid = gameObject.AddMissingComponent<UIGrid>();
		mGrid.cellWidth = 25;
		mGrid.pivot = UIWidget.Pivot.Center;
        mGrid.hideInactive = true;
        mCachedTransform = this.transform;
        mPages = new List<UISprite>();
        mCurrentPage = -1;
    }

    void OnDestroy()
    {
        AssetManager.RemoveAtlasRef(atlas, this);
        ClearPage();
    }

    public void AddPage()
    {
        UISprite sprite = NGUITools.AddWidget<UISprite>(mCachedTransform.gameObject);
        sprite.atlas = atlas;
		sprite.spriteName = under;
        //sprite.alpha = 0.5f;
        sprite.MakePixelPerfect();

        sprite.hideFlags = HideFlags.DontSave;

		sprite.depth = 5;

        //add the sprite into list.
        mPages.Add(sprite);

        //rename
        int pageCount = mPages.Count;
        sprite.cachedTransform.name = pageCount.ToString().PadLeft(10, '0');
    }

    public void UpdatePage(int pageCnt)
    {
        var temp = mPages.Count;
        while (temp < pageCnt)
        {
            AddPage();
            temp++;
        }
        HideBeyond(pageCnt);
    }

    public void HideBeyond(int index)
    {
        if(index <= mPages.Count)
        {
            for (int i = 0; i < index; i++)
            {
                mPages[i].gameObject.SetActive(true);
            }
        }

        while(index < mPages.Count)
        {
            mPages[index].gameObject.SetActive(false);
            index++;
        }

        RefreshLayout();
    }

    public void ClearPage()
    {
        for (int i = 0, imax = mPages.Count; i < imax; ++i)
        {
            UISprite s = mPages[i];
			if (s != null)
			{
				GameObject.DestroyImmediate(s.gameObject);
			}
        }
        mPages.Clear();

        this.mCurrentPage = -1;
    }


    public void RefreshLayout()
    {
		mGrid.sorting = UIGrid.Sorting.Alphabetic;
        mGrid.Reposition();
        //Bounds b = NGUIMath.CalculateRelativeWidgetBounds(mCachedTransform);
        //Vector3 pos = mCachedTransform.localPosition;
        //pos.x = -b.size.x / 2f;
        //mCachedTransform.localPosition = pos;
    }

    public void RefreshLayoutEx()
    {
        if (mPages != null && mPages.Count > 0)
        {
			mGrid.sorting = UIGrid.Sorting.Alphabetic;
            mGrid.Reposition();
            Vector3 pos = mCachedTransform.localPosition;
            int x = (mPages.Count - 1) * 25 / 2;
            pos.x = -x;
            mCachedTransform.localPosition = pos;
        }
    }

    public void SetCurrentPage(int page_)
    {
        if (mCurrentPage == page_) return;

        int pageCount = mPages.Count;

        if (page_ < 0 || page_ >= pageCount) return;

        UISprite newSelected = mPages[page_];

        if (mCurrentPage >= 0 && mCurrentPage < pageCount)
        {
            UISprite oldSelected = mPages[mCurrentPage];
            //oldSelected.color = Color.white;
            //oldSelected.alpha = 0.5f;
			oldSelected.spriteName = under;
        }

        //newSelected.color = Color.white;
        //newSelected.alpha = 1f;
		newSelected.spriteName = choice;

        mCurrentPage = page_;
    }

    public int GetPageCount()
    {
        return mPages.Count;
    }

	public int GetCurrentPage()
	{
		return mCurrentPage;
	}
}
