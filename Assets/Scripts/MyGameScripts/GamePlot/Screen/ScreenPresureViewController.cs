using UnityEngine;
using DG.Tweening;

namespace GamePlot
{
    public class ScreenPresureViewController : MonoViewController<ScreenPresure>
    {

        private Sequence _sequence;
        private int _mLength = 0;
        private System.Action _endCallback;

        #region IViewController implementation

        public void Open(ScreenPresureAction info, System.Action endCallback)
        {
            _endCallback = endCallback;
            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(info.duration).OnComplete(CloseView);

            View.Top.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
            View.Bottom.updateAnchors = UIRect.AnchorUpdate.OnUpdate;

            //淡入动画
            Tween fadeIn = DOTween.To(() => _mLength, (x) =>
                {
                    _mLength = x;
                    View.Top.bottomAnchor.absolute = -_mLength;
                    View.Bottom.topAnchor.absolute = _mLength;
                }, info.length, info.tweenTime);
            fadeIn.OnComplete(() =>
                {
                    View.Content.SetActive(true);
                    View.Top.updateAnchors = UIRect.AnchorUpdate.OnEnable;
                    View.Bottom.updateAnchors = UIRect.AnchorUpdate.OnEnable;
                });
            _sequence.Insert(0f, fadeIn);

            //淡出动画
            Tween fadeOut = DOTween.To(() => _mLength, (x) =>
                {
                    _mLength = x;
                    View.Top.bottomAnchor.absolute = -_mLength;
                    View.Bottom.topAnchor.absolute = _mLength;
                }, 0, info.tweenTime);
            fadeOut.OnStart(() =>
                {
                    View.Content.SetActive(false);
                });
            _sequence.Insert(info.duration - info.tweenTime, fadeOut);
        }



        protected override void AfterInitView ()
        {
            View.Content.SetActive(false);

        }

        protected override void RegistCustomEvent ()
        {
            EventDelegate.Set(View.EndBtn.onClick, OnEndBtnClick);
        }

        private void OnEndBtnClick()
        {
            if (_endCallback != null)
            {
                _endCallback();
                View.EndBtn.enabled = false;
            }
        }

        protected override void OnDispose()
        {
            _sequence.Kill();
            _sequence = null;
        }

        #endregion

        private void CloseView()
        {
            GamePlotManager.ClosePresureView();
        }
    }
}