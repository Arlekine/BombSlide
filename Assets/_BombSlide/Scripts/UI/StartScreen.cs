using System;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action Clicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.HapticOn)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);

        gameObject.SetActive(false);
        Clicked?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}