using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [SerializeField] private float _rotationTime;

    private Sequence _rotationSequence;

    private void Awake()
    {
        StartRotation();
    }

    private void OnDestroy()
    {
        _rotationSequence?.Kill();
    }

    public void StartRotation()
    {
        _rotationSequence = DOTween.Sequence();
        _rotationSequence.Append(transform.DOLocalRotate(Vector3.right * 360f, _rotationTime).SetRelative().SetEase(Ease.Linear));
        _rotationSequence.SetLoops(-1);
    }
}