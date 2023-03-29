using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelTarget : MonoBehaviour
{
    private const string DistanceTextFormat = "{0:0} m";

    [SerializeField] private Camera _camera;
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private RectTransform _icon;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private float _hideDistance;

    private Target _target;
    private RocketControl _rocketControl;

    public void SetData(Target target, RocketControl rocketControl)
    {
        enabled = true;
        _panel.alpha = 1f;
        _target = target;
        _rocketControl = rocketControl;
    }

    private void Update()
    {
        _icon.anchoredPosition = _camera.WorldToScreenPoint(_target.transform.position);

        var targetDistance = Vector3.Distance(_target.transform.position, _rocketControl.transform.position);
        _distanceText.text = String.Format(DistanceTextFormat, targetDistance);

        if (targetDistance < _hideDistance)
        {
            enabled = false;
            _panel.DOFade(0f, 0.3f);
        }
    }
}
