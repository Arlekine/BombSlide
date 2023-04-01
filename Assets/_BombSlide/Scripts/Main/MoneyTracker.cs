using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTracker : MonoBehaviour
{
    private const string DistanceTextFormat = "{0:0} m";

    public Action<int> GotMoney;

    [SerializeField] private CanvasGroup _prizePanel;
    [SerializeField] private TextMeshProUGUI _prizeText;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private float _distanceMultiplicator = 1f;

    private RocketControl _currentRocket;
    private Vector3 _startMovePoint;
    private bool _isTracking;

    private Sequence _animation;

    private void Start()
    {
        _prizePanel.gameObject.SetActive(false);
        _distanceText.text = "";
    }

    public void SetRocket(RocketControl rocket)
    {
        _prizePanel.gameObject.SetActive(false);
        _distanceText.text = "";

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
                            _distanceMultiplicator * ProgressionData.Instance.MoneyForDistanceInMeters);

        var tutorialMoney = (ProgressionData.Instance.BaseUpgradeCost * 3 - ProgressionData.Instance.StartMoney);
        if (GameManager.Instance.IsTutorial)
            distanceMoney = tutorialMoney / 2;

        var targetMoney = target != null ? GameManager.Instance.IsMainTarget(target) ? ProgressionData.Instance.BaseMoneyForLevelPass + ProgressionData.Instance.AdditionalMoneyForLevel * GameManager.Instance.CompletedLevels : ProgressionData.Instance.MoneyForDestraction : 0;
        
        if (GameManager.Instance.IsTutorial)
            targetMoney = tutorialMoney / 2;

        _prizeText.text = $"+ {targetMoney + distanceMoney}";
        _prizePanel.gameObject.SetActive(true);

        _prizePanel.transform.localScale = Vector3.zero;
        _prizePanel.alpha = 0f;

        _animation = DOTween.Sequence();

        _animation.Append(_prizePanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        _animation.Join(_prizePanel.DOFade(1f, 0.5f));

        GotMoney?.Invoke(distanceMoney + targetMoney);

        if (GameManager.Instance.IsTutorial)
            GameManager.Instance.IsUpgradesTutorial = true;

        StartCoroutine(UpdateCanvasesRoutine());
    }

    private void Update()
    {
        if (_isTracking)
        {
            _distanceText.text = String.Format(DistanceTextFormat, Vector3.Distance(_currentRocket.transform.position, _startMovePoint) * _distanceMultiplicator);
        }
    }

    private IEnumerator UpdateCanvasesRoutine()
    {
        var layoutGroup = _prizePanel.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.enabled = false;
        yield return null;
        layoutGroup.enabled = true;

    }
}