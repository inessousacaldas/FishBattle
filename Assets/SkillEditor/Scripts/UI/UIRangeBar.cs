using System;
using UnityEngine;

public class UIRangeBar : UIWidgetContainer
{
    [SerializeField]
    private UIValueBar _lowBar;
    [SerializeField]
    private UIValueBar _hightBar;

    public event Action OnLowValueChanged;
    public event Action OnHightValueChanged;

    public float LowValue
    {
        get { return _lowBar.Value; }
        set { _lowBar.Value = value; }
    }

    public float HightValue
    {
        get { return _hightBar.Value; }
        set { _hightBar.Value = value; }
    }

    public float MinValue
    {
        get { return _lowBar.MinValue; }
        set
        {
            _lowBar.MinValue = value;
            _hightBar.MinValue = value;
        }
    }
    public float MaxValue
    {
        get { return _lowBar.MaxValue; }
        set
        {
            _lowBar.MaxValue = value;
            _hightBar.MaxValue = value;
        }
    }

    private void Start()
    {
        _lowBar.OnValueChanged += LowValueChanged;
        _hightBar.OnValueChanged += HightValueChanged;
    }

    private void LowValueChanged()
    {
        if (OnLowValueChanged != null)
        {
            OnLowValueChanged();
        }
    }

    private void HightValueChanged()
    {
        if (OnHightValueChanged != null)
        {
            OnHightValueChanged();
        }
    }
}
