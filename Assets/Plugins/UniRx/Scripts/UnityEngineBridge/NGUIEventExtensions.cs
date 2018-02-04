#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public static class NGUIEventExtensions
    {
        public static Subject<T> AddObservable<T>(this List<EventDelegate> delegates, Func<T> getValue)
        {
            var stream = new Subject<T>();
            EventDelegate.Add(delegates, delegate
                {
                    var v = getValue();
                    if (!v.Equals(stream.LastValue))
                        stream.OnNext(v);
                });
            return stream;
        }

        public static Subject<T> AsObservable<T>(this List<EventDelegate> delegates, Func<T> getValue)
        {
            var stream = new Subject<T>();
            EventDelegate.Set(delegates, delegate
                {
                    var v = getValue();
                    stream.OnNext(v);
                });
            return stream;
        }

        public static IObservable<T> AsObservable<T>(this List<EventDelegate> delegates, T value)
        {
            var stream = new Subject<T>();
            EventDelegate.Add(delegates, delegate
            {
                stream.OnNext(value);
            });
            return stream;
        }

        public static Subject<Unit> AsObservable(this UIButton btn)
        {
            if (btn == null)
                return null;
            return btn.gameObject.OnClickAsObservable();
        }

        public static Subject<Unit> OnDoubleClickAsObservable(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var list = UIEventListener.Get(gameObject);
            if (list == null) {
                Debug.LogError("can not create UIEventListener!");
                return null;
            }

            var observable = new Subject<Unit>();
            list.onDoubleClick = delegate(GameObject go) {
                observable.OnNext(new Unit());
            };

            return observable;
        }
        
        public static Subject<GameObject> AsObservableWithGameObject(this UIButton btn) 
        {
            return btn == null ? null : btn.gameObject.OnClickAsObservableWithGameObject();
        }
        
        public static Subject<GameObject> OnClickAsObservableWithGameObject(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var list = UIEventListener.Get(gameObject);
            if (list == null) {
                Debug.LogError("can not create UIEventListener!");
                return null;
            }

            var observable = new Subject<GameObject>();
            list.onClick = delegate(GameObject go) {
                observable.OnNext(go);
            };

            return observable;
        }
        
        public static Subject<Unit> OnClickAsObservable(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var list = UIEventListener.Get(gameObject);
            if (list == null) {
                Debug.LogError("can not create UIEventListener!");
                return null;
            }

            var observable = new Subject<Unit>();
            list.onClick = delegate(GameObject go) {
                observable.OnNext(new Unit());
            };

            return observable;
        }

        public static IDisposable SubscribeToText(this IObservable<string> source, UILabel text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, UILabel text, Func<T, string> selector)
        {
            return source.SubscribeWithState2(text, selector, (x, t, s) => t.text = s(x));
        }

        /// <summary>Observe onValueChanged with current `isOn` value on subscribe.</summary>
        public static Subject<bool> OnValueChangedAsObservable(this UIToggle toggle)
        {
            if (toggle == null) return null;
            return toggle.onChange.AsObservable(delegate{return toggle.value;});
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this UIProgressBar processbar)
        {
            if (processbar== null) return null;
            return processbar.onChange.AsObservable(delegate{return processbar.value;});
        }

        /// <summary>Observe onValueChanged with current `normalizedPosition` value on subscribe.</summary>
        public static IObservable<Vector2> OnValueChangedAsObservable(this UIScrollView scrollView)
        {
            //要是没人用就不do了－－－ todo fish
            return null;
        }

        public static IObservable<Tuple<GameObject, int, int>> OnUpdateAsObservable(this UIRecycledList recycledList)
        {
            var stream = new Subject<Tuple<GameObject, int, int>>();
            recycledList.onUpdateItem = delegate(GameObject item, int itemIndex, int dataIndex)
            {
                stream.OnNext(Tuple.Create(item, itemIndex, dataIndex));
            };
            return stream;
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnSubmitAsObservable(this UIInput inputField)
        {
            if (inputField == null) return null;
            return inputField.onSubmit.AsObservable(delegate{return inputField.value;});
        }

        /// <summary>Observe onValueChange with current `text` value on subscribe.</summary>
        #if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        [Obsolete("onValueChange has been renamed to onValueChanged")]
        #endif
        public static IObservable<string> OnValueChangeAsObservable(this UIInput inputField)
        {
            if (inputField == null) return null;
            return inputField.onChange.AsObservable(delegate{return inputField.value;});
        }

        public static Subject<int> OnValueChangedAsObservable(this UIPopupList popupList)
        {
            if (popupList == null) return null;
            return popupList.onChange.AddObservable(delegate{return popupList.CurSelectedItemIdx;});
        }
    }
}
#endif