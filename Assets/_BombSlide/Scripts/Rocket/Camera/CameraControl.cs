using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private RocketControl _rocket;
    [SerializeField] private float _transitionLerpParameter;

    [Space] 
    [SerializeField] private Transform _initialPos;
    [SerializeField] private Transform _sideViewPos;
    [SerializeField] private Transform _trackPos;
    [SerializeField] private Transform _flightPos;
    [SerializeField] private Transform _hitPos;

    private Transform _camera;

    public void SetCamera(Transform camera)
    {
        _camera = camera;
        _camera.position = _initialPos.position;

        _rocket.MovingStarted.AddListener(MoveToTrackPosition);
        _rocket.FreeFlightStarted.AddListener(MoveToFlightPosition);
        _rocket.ObstacleHitted.AddListener(MoveToHitPosition);

        _camera.parent = _sideViewPos;

    }

    public float CameraDistance => (transform.position - _camera.position).magnitude;

    private void ResetCamera()
    {
        _camera.position = _initialPos.position;
        _camera.parent = _sideViewPos;
    }

    private void MoveToTrackPosition()
    {
        _camera.parent = _trackPos;
    }
    private void MoveToFlightPosition()
    {
        _camera.parent = _flightPos;
    }

    private void MoveToHitPosition(Target target)
    {
        _camera.parent = _hitPos;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, _rocket.transform.position.y, _rocket.transform.position.z);
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, Vector3.zero, _transitionLerpParameter * Time.deltaTime);

        _camera.rotation = Quaternion.Lerp(_camera.rotation, Quaternion.LookRotation((transform.position - _camera.position).normalized), _transitionLerpParameter * 10 * Time.deltaTime);
    }
}
