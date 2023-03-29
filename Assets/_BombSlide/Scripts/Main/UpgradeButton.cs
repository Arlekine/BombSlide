using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public UnityEvent<int, int> Clicked;

    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private int _baseCost;
    [SerializeField] private int _additionalCostForIteration;

    private int _currentCost;
    private int _currentCostInteration;

    private void Awake()
    {
        _button.onClick.AddListener(InvokeClick);
        _currentCost = _baseCost;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(InvokeClick);
    }

    private void InvokeClick()
    {
        _currentCostInteration++;

        Clicked?.Invoke(_currentCost, _currentCostInteration);

        _currentCost = _baseCost + _additionalCostForIteration * _currentCostInteration;
        _cost.text = $"{_currentCost} $";

        if (GameManager.Instance.HapticOn)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }

    public void SetCurrentCostIteration(int interation)
    {
        _currentCostInteration = interation;
        _currentCost = _baseCost + _additionalCostForIteration * _currentCostInteration;
        _cost.text = $"{_currentCost} $";
    }

    public void SetCurrentMoney(int currentMoney)
    {
        _button.interactable = currentMoney >= _currentCost;
    }
}