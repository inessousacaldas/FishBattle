using System;

namespace Assets.Standard_Assets.NGUI.CustomExtension.UI
{
    public class CTextInput : UIInput {
        public bool numOnly;
        public Action onSelected;
        public Action onUnselectted;
        public string lastValue = "";

        public string Text {
            set { base.value = value; }
            get { return base.value; }
        }

        public int MaxChar {
            set { characterLimit = value; }
            get { return characterLimit; }
        }

        protected override void Insert(string text) {
            if (numOnly) {
                lastValue = value;
                try {
                    if (text == "") {
                        base.Insert(text);
                        return;
                    } else if (text != "") {
                        Convert.ToInt32(text);
                    }
                } catch {
                    //FuncUtil.AddTip("只能输入数字哦");
                    return;
                }
            }
            base.Insert(text);
        }



        protected override void OnSelectEvent()
        {
            base.OnSelectEvent();
            if (onSelected != null)
                onSelected();
        }

        protected override void OnDeselectEvent() {
            if (numOnly && value =="") {
                value = lastValue;
            }
            base.OnDeselectEvent();
            if (onUnselectted != null)
                onUnselectted();
        }

        public int valueNum {
            get {
                if (numOnly) {
                    return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                } else {
                    return 0;
                }
            }
        }

        public void ClearSelectTime() {
            mSelectMe = -1;
        }
    }
}