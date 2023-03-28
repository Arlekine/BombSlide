using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBahaviour : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ParticleSystem _speedEffect;
    [SerializeField] private Shaker _shaker;

    public Camera Camera => _camera;
    public ParticleSystem SpeedEffect => _speedEffect;
    public Shaker Shaker => _shaker;
}
