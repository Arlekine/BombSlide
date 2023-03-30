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

    private RocketControl _rocketControl;

    private void Start()
    {
        _icon.color = _upColor;
    }

    public void SetRocket(RocketControl rocket)
    {
        _rocketControl = rocket;
        _rocketControl.FreeFlightStarted.AddListener(() => gameObject.SetActive(true));
        _rocketControl.ObstacleHitted.AddListener((t) =>  gameObject.SetActive(false));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_rocketControl)
        {
            _icon.color = _downColor;
            _rocketControl.StartBoost();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_rocketControl)
        {
            _icon.color = _upColor;
            _rocketControl.StopBoost();
        }
    }
}
