using UnityEngine;

public partial class ItemsPageContainer : BaseView
{
    private bool _isPageMoving = false;
    private const float Space = 420f;

    private int _pageOnCenter;
    public int pageOnCenter{get{ return _pageOnCenter;}}

    protected override void LateElementBinding()
    {
        ItemContainerGrid_UICenterOnChild.onCenter = OnCenter;
        Container_UIScrollView.onStoppedMoving = onStoppedMoving;
    }

    private void OnCenter (GameObject centeredObject)
    {
        if ( centeredObject == null )
            return;

        UIPageInfo pageInfo = centeredObject.GetComponent<UIPageInfo>();
        if ( pageInfo == null )
        {
            Debug.LogError ("pageInfo == null");
            return;
        }
        _pageOnCenter = pageInfo.page;
        PageGroup_UIPageGroup.SetCurrentPage(_pageOnCenter);
    }

    private void onStoppedMoving()
    {
        _isPageMoving = false;
    }

    public void SetPage(int page, int max)
    {
        int currentPage = PageGroup_UIPageGroup.GetCurrentPage();
        if (currentPage == page)
            return;

        PageGroup_UIPageGroup.SetCurrentPage(page);    

        var offset = 0f;
        if (page == 0)
            offset = 0.03f;
        else if (page >= max - 1)
            offset = 0.97f;
        else
            offset = (float)page / max;
        
        if (Container_UIScrollView.movement == UIScrollView.Movement.Vertical)  
        {
            Container_UIScrollView.SetDragAmount(0f, offset, false);
        }
        else
        {
            Container_UIScrollView.SetDragAmount(offset, 0f, false);
        }
        ItemContainerGrid_UICenterOnChild.Recenter();
    }

    public void PageMove(int page,float speed = 8f)
    {
        if (_isPageMoving)
            return;

        int currentPage = PageGroup_UIPageGroup.GetCurrentPage();
        if (currentPage != page)
        {
            _isPageMoving = true;

            PageGroup_UIPageGroup.SetCurrentPage(page);

            Vector3 localOffset = new Vector3(Space * (page - Mathf.Max(0, currentPage)), 0f, 0f);
            UIScrollView mScrollView = Container_UIScrollView;
            Transform panelTrans = mScrollView.panel.cachedTransform;
            SpringPanel.Begin(mScrollView.panel.cachedGameObject, panelTrans.localPosition - localOffset, speed).onFinished = OnClickDragFinished;
        }
    }

    private void OnClickDragFinished()
    {
        ItemContainerGrid_UICenterOnChild.Recenter();
        _isPageMoving = false;
    }

    protected override void OnDispose()
    {
        PageGroup_UIPageGroup.ClearPage();
    }
}