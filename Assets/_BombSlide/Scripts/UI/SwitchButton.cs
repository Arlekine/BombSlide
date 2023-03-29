using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    public Action<bool> OnSwitch;

    [SerializeField] private Image _image;
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Sprite _inactiveSprite;

    private bool _isActive;

    public void SetState(bool isActive)
    {
        _isActive = isActive;
        _image.sprite = _isActive ? _activeSprite : _inactiveSprite;
    }

    public void Click()
    {
        _isActive = !_isActive;
        _image.sprite = _isActive ? _activeSprite : _inactiveSprite;
        OnSwitch?.Invoke(_isActive);
    }
}