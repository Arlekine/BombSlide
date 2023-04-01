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

    private int _currentCost;
    private int _currentCostInteration;

    private void Awake()
    {
        _button.onClick.AddListener(InvokeClick);
        _currentCost = ProgressionData.Instance.BaseUpgradeCost;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(InvokeClick);
    }

    private void InvokeClick()
    {
        _currentCostInteration++;
        var oldCost = _currentCost;
        _currentCost = ProgressionData.Instance.BaseUpgradeCost + ProgressionData.Instance.AdditionalUpgradeCost * _currentCostInteration;
        _cost.text = $"{_currentCost} $";

        Clicked?.Invoke(oldCost, _currentCostInteration);

        if (GameManager.Instance.HapticOn)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }

    public void SetCurrentCostIteration(int interation)
    {
        _currentCostInteration = interation;
        _currentCost = ProgressionData.Instance.BaseUpgradeCost + ProgressionData.Instance.AdditionalUpgradeCost * _currentCostInteration;
        _cost.text = $"{_currentCost} $";
    }

    public void SetCurrentMoney(int currentMoney)
    {
        _button.interactable = currentMoney >= _currentCost;
    }
}