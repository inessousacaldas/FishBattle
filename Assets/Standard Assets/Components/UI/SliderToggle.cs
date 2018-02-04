using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliderToggle : MonoBehaviour 
{
    public System.Action<bool> OnSliderToggleClick;

    private UISprite _mSprite;
    private UISlider _slider;
    private UISprite _thumbSprite;
    private UIAnchor _thumbAnchor;

    void Awake()
    {
        UIButton btn = this.transform.GetComponent<UIButton>();
        EventDelegate.Set(btn.onClick, OnClickSliderToggle);

        _mSprite = this.transform.GetComponent<UISprite>();
        _slider = this.transform.Find("Slider").GetComponent<UISlider>();
        _thumbSprite = this.transform.Find("Thumb").GetComponent<UISprite>();
        _thumbAnchor = this.transform.Find("Thumb").GetComponent<UIAnchor>();
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public UISprite sprite
    {
        get
        {
            return _mSprite;
        }
    }

    public void SetState(bool isOpen)
    {
        if (isOpen)
        {
            _slider.value = 1;
            _thumbAnchor.side = UIAnchor.Side.Right;
            _thumbAnchor.enabled = true;
        }
        else
        {
            _slider.value = 0;
            _thumbAnchor.side = UIAnchor.Side.Left;
            _thumbAnchor.enabled = true;
        }
    }

    public bool GetState()
    {
        return _slider.value == 1;
    }

    private void OnClickSliderToggle()
    {
        SetState(_slider.value == 0);
        Debug.Log(_slider.value == 1);
        if (OnSliderToggleClick != null)
            OnSliderToggleClick(_slider.value == 1);
    }
}
