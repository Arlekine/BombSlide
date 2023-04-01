using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private Color _downColor;
    [SerializeField] private Color _upColor;
    [SerializeField] private Color _outOfEnergyColor;

    private RocketControl _rocketControl;

    public void SetRocket(RocketControl rocket)
    {
        _icon.color = _upColor;
        _rocketControl = rocket;
        _rocketControl.BoostEnergyEnded.AddListener(DeactivateButton);
        _rocketControl.FreeFlightStarted.AddListener(() => gameObject.SetActive(true));
        _rocketControl.ObstacleHitted.AddListener((t) =>  gameObject.SetActive(false));
    }

    public void DeactivateButton()
    {
        _icon.color = _outOfEnergyColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_rocketControl && _rocketControl.HasBoost)
        {
            _icon.color = _downColor;
            _rocketControl.StartBoost();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_rocketControl && _rocketControl.HasBoost)
        {
            _icon.color = _upColor;
            _rocketControl.StopBoost();
        }
    }
}
