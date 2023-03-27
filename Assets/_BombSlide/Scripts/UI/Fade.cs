using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Fade : MonoBehaviour
{
    [SerializeField] private CanvasGroup _fadePanel;
    [SerializeField] private float _fadeTime = 0.5f;

    private void Awake()
    {
        _fadePanel.alpha = 1f;
        FadeOut();
    }

    public void FadeIn(Action completed = null)
    {
        _fadePanel.DOFade(1f, _fadeTime).onComplete += () => completed?.Invoke();
    }

    public void FadeOut(Action completed = null)
    {
        _fadePanel.DOFade(0f, _fadeTime).onComplete += () => completed?.Invoke();
    }
}