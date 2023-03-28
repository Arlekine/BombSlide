using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private RocketControl _rocket;
    [SerializeField] private float _transitionLerpParameter;
    [SerializeField] private float _moveToTrackPositionOffset;

    [Space] 
    [SerializeField] private Transform _initialPos;
    [SerializeField] private Transform _sideViewPos;
    [SerializeField] private Transform _trackPos;
    [SerializeField] private Transform _flightPos;
    [SerializeField] private Transform _boostPos;
    [SerializeField] private Transform _hitPos;

    private CameraBahaviour _camera;

    public CameraBahaviour Camera => _camera;

    public void SetCamera(CameraBahaviour camera)
    {
        _camera = camera;
        _camera.transform.position = _initialPos.position;

        _rocket.MovingStarted.AddListener(MoveToTrackPosition);
        _rocket.FreeFlightStarted.AddListener(MoveToFlightPosition);
        _rocket.ObstacleHitted.AddListener(MoveToHitPosition);

        _rocket.BoostStart.AddListener(MoveToBoostPosition);
        _rocket.BoostStop.AddListener(MoveToFlightPosition);

        _camera.transform.parent = _sideViewPos;
    }

    public float CameraDistance => (transform.position - _camera.transform.position).magnitude;

    private void ResetCamera()
    {
        _camera.transform.position = _initialPos.position;
        _camera.transform.parent = _sideViewPos;
    }

    private void MoveToTrackPosition()
    {
        StartCoroutine(MoveToTrackPositionRoutine());
    }

    private void MoveToBoostPosition()
    {
        _camera.transform.parent = _boostPos;
    }

    private IEnumerator MoveToTrackPositionRoutine()
    {
        yield return new WaitForSeconds(_moveToTrackPositionOffset);

        _camera.SpeedEffect.gameObject.SetActive(true);
        _transitionLerpParameter = 0.3f;
        _camera.transform.parent = _trackPos;
        _camera.Camera.DOFieldOfView(60f, 2.5f).SetEase(Ease.InQuad);
    }

    private void MoveToFlightPosition()
    {
        _camera.SpeedEffect.gameObject.SetActive(false);
        _camera.transform.parent = _flightPos;
    }

    private void MoveToHitPosition(Target target)
    {
        _camera.SpeedEffect.gameObject.SetActive(false);
        _camera.Shaker.Shake(0.05f);
        _camera.transform.parent = _hitPos;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, _rocket.transform.position.y, _rocket.transform.position.z);
        _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, Vector3.zero, _transitionLerpParameter * Time.deltaTime);

        _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.LookRotation((transform.position - _camera.transform.position).normalized), _transitionLerpParameter * 10 * Time.deltaTime);
    }
}
