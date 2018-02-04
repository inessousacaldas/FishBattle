using Assets.Standard_Assets.NGUI.CustomExtension.Manager;
using UnityEngine;

namespace Assets.Standard_Assets.NGUI.CustomExtension.UI
{
    public class CProgressBar:UIProgressBar
    {
        public UILabel lbl;
        public bool needOverRideSetThumbPos = true;
        public UISprite.Type defaulteType = UISprite.Type.Sliced;
        private bool _IsMove_mFG;//进度条是否一直都滚动
        public float moveSpeed_mFG = -0.003f;
        public bool changeTypeOnMin = true;
        private bool isStart = false;
        public bool isForegroundZeroHide = true; //在A/B中，若A为0则默认隐藏进度条
        public bool isForegroundHideLessThanMinWidth = false;//当进度条目前长度小于进度条最小长度时，隐藏进度条
        protected override void OnStart()
        {
            if(isStart)
            {
                return;
            }
            isStart = true;
            base.OnStart();
            if(foregroundWidget != null)
            {
                UISprite foreground = foregroundWidget as UISprite;
                if(foreground != null)
                {
                    defaulteType = foreground.type;
                }
            }
        }

        protected override void SetThumbPosition(Vector3 worldPos)
        {
            if(needOverRideSetThumbPos == false)
            {
                base.SetThumbPosition(worldPos);
                return;
            }
            Transform t = thumb.parent;

            if(t != null)
            {
                worldPos = t.InverseTransformPoint(worldPos);
                worldPos.x = Mathf.Round(worldPos.x);
                worldPos.y = -12;//Mathf.Round(worldPos.y);
                worldPos.z = 0f;

                UIWidget w = thumb.GetComponent<UIWidget>();
                if(w != null)
                {
                    if(worldPos.x > mFG.transform.localPosition.x + mFG.width - w.width)
                    {
                        worldPos.x = mFG.transform.localPosition.x + mFG.width - w.width;
                    }
                }
                if(Vector3.Distance(thumb.localPosition,worldPos) > 0.001f)
                {
                    thumb.localPosition = worldPos;
                }
            }
            else if(Vector3.Distance(thumb.position,worldPos) > 0.00001f)
            {
                thumb.position = worldPos;
            }
        }

        new public float value
        {
            set
            {
                if(base.value == value)
                {
                    return;
                }
                UISprite foreground = foregroundWidget as UISprite;
                OnStart();
                //if (foreground != null) {
                //    foreground.type = defaulteType;
                //}
                if(foreground != null && defaulteType == UISprite.Type.Sliced && changeTypeOnMin)
                {
                    foreground.type = UISprite.Type.Sliced;
                    float now = value * foreground.width;
                    if(foreground.minWidth > now)
                    {
                        foreground.type = UISprite.Type.Simple;
                    }
                    else
                    {
                        foreground.type = defaulteType;
                    }
                }
                if(isForegroundZeroHide == true) foreground.gameObject.SetActive(value > 0.0001);
                if(isForegroundHideLessThanMinWidth == true) foreground.gameObject.SetActive(foreground.minWidth < value * foreground.width);
                base.value = value;
            }
            get
            {
                return base.value;
            }
        }

        public string GetLabelText()
        {
            return lbl.text;
        }

        /// <summary>
        /// 输出文字为 XXX10/100
        /// </summary>
        /// <param name="now"></param>
        /// <param name="max"></param>
        /// <param name="leftStr"></param>
        public void SetProgressValue(float now,float max,string leftStr = "")
        {
            value = now / max;
            lbl.text = leftStr + now + "/" + max;
        }

        /// <summary>
        /// 输出文字为 XXX100%
        /// </summary>
        /// <param name="now"></param>
        /// <param name="max"></param>
        /// <param name="leftStr"></param>
        public void SetProgressValue2(float now,float max,string leftStr = "")
        {
            value = now / max;
            lbl.text = leftStr + Mathf.RoundToInt(value * 100f) + "%";
        }

        /// <summary>
        /// 输出文字为 XXX100.00%
        /// </summary>
        /// <param name="now"></param>
        /// <param name="max"></param>
        /// <param name="leftStr"></param>
        /// <param name="baseNumStr">小数点后保留n位数字的格式字符，如0.00则保留2位</param>
        public void SetProgressValue3(float now,float max,string leftStr = "",string baseNumStr = "0.00")
        {
            value = now / max;
            lbl.text = leftStr + (value * 100).ToString(baseNumStr) + "%";
        }

        /// <summary>
        /// 输出文字为 XXX10/100  (int) lbl部分的数字可以超出最大范围
        /// </summary>
        /// <param name="now"></param>
        /// <param name="max"></param>
        /// <param name="leftStr"></param>
        public void SetProgressValue4(float now,float max,string leftStr = "")
        {
            value = now / max;
            lbl.text = leftStr + (int)now + "/" + (int)max;
        }

        private float rollSpeed;
        public float targetValue;
        public void RollTo(float target,int time = 20)
        {
            targetValue = target;
            rollSpeed = (target - value) / time;
            UILoopManager.AddToFrame(this,OnMove);
        }

        private void OnMove()
        {
            value += rollSpeed;
            if((value >= targetValue && rollSpeed > 0) || (value <= targetValue && rollSpeed < 0))
            {
                value = targetValue;
                UILoopManager.RemoveFromFrame(this);
            }
        }
    }
}