using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyTracker : MonoBehaviour
{
    private const string DistanceTextFormat = "{0:0} m";

    public Action<int> GotMoney;

    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _targetDestractionText;
    [SerializeField] private float _distanceMultiplicator = 1f;
    [SerializeField] private float _distanceToMoneyParameter = 2f;

    private RocketControl _currentRocket;
    private Vector3 _startMovePoint;
    private bool _isTracking;

    private Sequence _animation;

    private void Start()
    {
        _distanceText.text = "";
        _targetDestractionText.text = "";
    }

    public void SetRocket(RocketControl rocket)
    {
        _distanceText.text = "";
        _targetDestractionText.text = "";

        _currentRocket = rocket;
        _currentRocket.FreeFlightStarted.AddListener(StartTrackingDistance);
        _currentRocket.ObstacleHitted.AddListener(EndTrackingDistance);
    }

    private void StartTrackingDistance()
    {
        _isTracking = true;
        _startMovePoint = _currentRocket.transform.position;
    }

    private void EndTrackingDistance(Target target)
    {
        _isTracking = false;

        var distanceMoney = (int)(Vector3.Distance(_currentRocket.transform.position, _startMovePoint) *
                            _distanceMultiplicator * _distanceToMoneyParameter);

        var targetMoney = target != null ? target.Cost : 0;

        if (target != null)
            _targetDestractionText.text = $"For destruction: + {target.Cost} $!\nFor distance: + {distanceMoney} $!";
        else
            _targetDestractionText.text = $"For distance: + {distanceMoney} $!";

        _targetDestractionText.transform.localScale = Vector3.zero;
        var color = _targetDestractionText.color;
        color.a = 0f;
        _targetDestractionText.color = color;

        _animation = DOTween.Sequence();

        _animation.Append(_targetDestractionText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        _animation.Join(_targetDestractionText.DOFade(1f, 0.5f));

        GotMoney?.Invoke(distanceMoney + targetMoney);
    }

    private void Update()
    {
        if (_isTracking)
        {
            _distanceText.text = String.Format(DistanceTextFormat, Vector3.Distance(_currentRocket.transform.position, _startMovePoint) * _distanceMultiplicator);
        }
    }
}