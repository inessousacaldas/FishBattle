using System;
using UnityEngine;

public class UIValueBar : UIWidgetContainer
{
    [SerializeField]
    private float _value = 5;
    [SerializeField]
    private float _minValue = 0;
    [SerializeField]
    private float _maxValue = 10;

    [SerializeField]
    private Transform _thumb;
    [SerializeField]
    private UIWidget _background;

    public event Action OnValueChanged;

    private bool _needUpdate;

    public float Value
    {
        get { return _value; }
        set
        {
            SetValue(value);
        }
    }

    public float MinValue
    {
        get { return _minValue; }
        set
        {
            MarkNeedUpdate();
            _minValue = value;
        }
    }
    public float MaxValue
    {
        get { return _maxValue; }
        set
        {
            MarkNeedUpdate();
            _maxValue = value;
        }
    }

    private void Start()
    {
        var listener = UIEventListener.Get(_thumb.gameObject);
        listener.onDrag += OnDragThumb;

        MarkNeedUpdate();
    }

    private void Update()
    {
        if (_needUpdate)
        {
            UpdateUI();
        }
    }

    private void OnDragThumb(GameObject go, Vector2 delta)
    {
        if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        {
            return;
        }
        SetValue(WorldToValue(delta));
    }

    private float WorldToValue(Vector2 delta)
    {
        var corner = _background.worldCorners;
        var size = corner[2] - corner[0];
        var worldDelta = _thumb.parent.TransformPoint(delta * UIRoot.GetPixelSizeAdjustment(_thumb.gameObject));
        var changeValue = worldDelta.x * (_maxValue - _minValue) / size.x;

        return _value + changeValue;
    }


    private void SetValue(float newValue)
    {
        _value = newValue;
        MarkNeedUpdate();

        ValueChange();
    }

    private void MarkNeedUpdate()
    {
        _needUpdate = true;
    }

    private void UpdateUI()
    {
        _needUpdate = false;

        var corner = _background.worldCorners;
        var offset = corner[0];
        var size = corner[2] - corner[0];

        var worldPos = new Vector3(offset.x + (_value - _minValue) / ((_maxValue - _minValue) / size.x), _thumb.position.y, 0);
        _thumb.position = worldPos;
    }

    private void ValueChange()
    {
        if (OnValueChanged != null)
        {
            OnValueChanged();
        }
    }
}
