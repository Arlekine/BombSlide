using System;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action Clicked;

    [SerializeField] private GameObject _upgradesPanel;
    [SerializeField] private GameObject _touchTurorial;

    private void OnEnable()
    {
        _upgradesPanel.SetActive(!GameManager.Instance.IsTutorial || GameManager.Instance.IsUpgradesTutorial);
        _touchTurorial.SetActive(GameManager.Instance.IsTutorial && !GameManager.Instance.IsUpgradesTutorial);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.IsUpgradesTutorial)
            return;

        if (GameManager.Instance.HapticOn)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);

        gameObject.SetActive(false);
        Clicked?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}