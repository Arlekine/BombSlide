using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class Shaker : MonoBehaviour
{
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private Vector3 _strength = Vector3.one;
    [SerializeField] private int _frequency = 100;

    public void Shake(float delay = 0f)
    {
        transform.DOShakePosition(_duration, _strength, _frequency).SetDelay(delay);
    }
}
