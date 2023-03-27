using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action Clicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        Clicked?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}