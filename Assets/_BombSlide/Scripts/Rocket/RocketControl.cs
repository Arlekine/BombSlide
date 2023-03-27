using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using PathCreation;
using ShatterToolkit;
using UnityEngine;
using UnityEngine.Events;

public class RocketControl : MonoBehaviour
{
    public UnityEvent MovingStarted;
    public UnityEvent FreeFlightStarted;
    public UnityEvent<Target> ObstacleHitted;

    [SerializeField] private PathMover _initialPath;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _model;
    [SerializeField] private CameraControl _cameraControl;
    
    [Header("Free flight")]
    [SerializeField] private float _outputSpeed;
    [SerializeField] private float _gravity;

    [Header("Horizontal Move")]
    [SerializeField] private float _horizontalMoveLerpParameter;
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;

    [Header("Boost")] 
    [SerializeField] private float _boostSpeed;
    [SerializeField] private float _boostEnergyCost;
    [SerializeField] private float _maxBoostEnergy;

    [Header("Explosion radius")] 
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;

    [Header("Trajectory Gizmos")] 
    [SerializeField] private int _accuracy = 100;
    [SerializeField] private float _targetYToDraw = 0f;

    [Header("FX")] 
    [SerializeField] private ParticleSystem _drive;
    [SerializeField] private ParticleSystem _boostDrive;
    [SerializeField] private ParticleSystem _explosion;

    private Vector3 _forwardMoveDirection;

    private bool _isFreeFlight;
    private float _targetXPos;
    private float _moveStartTime;

    private bool _isBoosting;
    private float _currentBoostEnergy;

    private Coroutine _boostRoutine;

    private float CurrentForwardSpeed => _isBoosting ? _boostSpeed : _outputSpeed;
    public float CurrentBoostNormalized => _currentBoostEnergy / _maxBoostEnergy;

    private void Start()
    {
        _initialPath.PlaceAtStart();

        _currentBoostEnergy = _maxBoostEnergy;
    }

    public void ResetRocket()
    {
        _isFreeFlight = false;
        _currentBoostEnergy = _maxBoostEnergy;
        _initialPath.PlaceAtStart();

    }

    public void StartBoost()
    {
        if (_isFreeFlight == false || _currentBoostEnergy <= 0)
            return;

        _isBoosting = true;
        _boostRoutine = StartCoroutine(BoostRoutine());
    }

    public void StopBoost()
    {
        if (_isFreeFlight == false)
            return;

        if (_boostRoutine != null)
            StopCoroutine(_boostRoutine);

        _isBoosting = false;
    }

    [EditorButton]
    public void StartMoving()
    {
        _initialPath.StartMove();
        _initialPath.pathCompleted.AddListener(StartFreeFlight); 
        MovingStarted?.Invoke();
    }

    private void StartFreeFlight(Vector3 outputDirection)
    {
        _targetXPos = transform.position.x;
        _moveStartTime = Time.time;
        _isFreeFlight = true;
        _forwardMoveDirection = outputDirection;

        FreeFlightStarted?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isBoosting == false)
                StartBoost();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_isBoosting)
                StopBoost();
        }
    }

    private void FixedUpdate()
    {
        if (_isFreeFlight)
        {
            var forwardVector = _forwardMoveDirection * CurrentForwardSpeed * Time.fixedDeltaTime;
            var gravityVector = Vector3.up * GetVerticalSpeed(Time.time - _moveStartTime) * Time.fixedDeltaTime;

            var position = _rigidbody.position + forwardVector;
            position -= gravityVector;

            var rotation = Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(forwardVector - gravityVector), 4f * Time.deltaTime);

            var fingers = LeanTouch.GetFingers(true);

            if (fingers.Count > 0)
            {
                _targetXPos = Mathf.Lerp(_targetXPos, fingers[0].GetLastWorldPosition(_cameraControl.CameraDistance).x, _horizontalMoveLerpParameter * Time.deltaTime);
                _targetXPos = Mathf.Clamp(_targetXPos, _minX, _maxX);
            }

            position.x = _targetXPos;
            _rigidbody.MovePosition(position);
            _rigidbody.MoveRotation(rotation);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_isFreeFlight == false)
            return;

        var target = collider.gameObject.GetComponent<Target>();

        _isFreeFlight = false;
        _model.SetActive(false);

        if (target != null)
        {
            target.ShatterTool.Shatter(transform.position, 1);
            ObstacleHitted?.Invoke(target);

            StartCoroutine(ExplosionRoutine());
        }
        else
        {
            ObstacleHitted?.Invoke(null);
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return null;

        var colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        
        foreach (var col in colliders)
        {
            if (col.attachedRigidbody != null && col.attachedRigidbody.isKinematic == false)
            {
                var shatter = col.GetComponent<ShatterTool>();
                
                if(shatter != null)
                    shatter.Shatter(shatter.Center, 1);
            }
        }

        yield return null;

        colliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (var col in colliders)
        {
            if (col.attachedRigidbody != null && col.attachedRigidbody.isKinematic == false)
            {
                var shatter = col.GetComponent<ShatterTool>();

                if (shatter != null)
                    shatter.Shatter(shatter.Center, 1);
            }
        }

        yield return null;

        colliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (var col in colliders)
        {
            if (col.attachedRigidbody != null && col.attachedRigidbody.isKinematic == false)
            {
                col.attachedRigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }
    }

    private IEnumerator BoostRoutine()
    {
        print("Boost");
        while (_currentBoostEnergy > 0)
        {
            _currentBoostEnergy -= _boostEnergyCost * Time.deltaTime;
            yield return null;
        }

        _isBoosting = false;
        _boostRoutine = null;
    }

    private float GetVerticalSpeed(float flightTime)
    {
        return flightTime * _gravity;
    }

    private float GetYDistanceAfterTime(float time)
    {
        return (_gravity * time * time) / 2;
    }
    private float GetTimeToReachDistanceVertically(float distance, float startSpeed, float startY)
    {
        var a = -_gravity / 2;
        var b = startSpeed;
        var c = startY - distance;

        var D = b * b - 4 * a * c;

        if (D >= 0)
        {
            var t1 = (-b + Mathf.Sqrt(D)) / (2 * a);

            if (t1 >= 0) return t1;
            
            var t2 = (-b - Mathf.Sqrt(D)) / (2 * a);

            if (t2 >= 0) return t2;
        }
        
        throw new ArgumentException("Invalid values for reach time calculation");
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.red;
        var forwardVector = _forwardMoveDirection * CurrentForwardSpeed * Time.fixedDeltaTime;
        var gravityVector = Vector3.up * GetVerticalSpeed(_moveStartTime) * Time.fixedDeltaTime;

        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + forwardVector);
        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position - gravityVector);
        
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + forwardVector - gravityVector);
        Render(_initialPath.Path.path.GetPointAtDistance(_initialPath.Path.path.length - 0.01f), _initialPath.Path.path.GetDirectionAtDistance(_initialPath.Path.path.length - 0.01f).normalized, _outputSpeed, _targetYToDraw);
    }

    private void Render(Vector3 startPoint, Vector3 startDirection, float speed, float yStopDistance)
    {
        float flightTime = GetTimeToReachDistanceVertically(yStopDistance, speed * startDirection.y, startPoint.y);
        float stepTime = flightTime / _accuracy;

        Vector3 currentPosition = startPoint;
        Vector3 previousPosition = currentPosition;

        float currentTime = 0f;

        Gizmos.color = Color.green;

        for (int i = 0; i < _accuracy; i++)
        {
            currentTime += stepTime;
            var forwardVector = startDirection * speed * stepTime;
            var gravityVector = Vector3.up * GetVerticalSpeed(currentTime) * stepTime;

            currentPosition += forwardVector;
            currentPosition -= gravityVector;

            Gizmos.DrawLine(previousPosition, currentPosition);

            previousPosition = currentPosition;
        }
    }
}