using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using MoreMountains.NiceVibrations;
using PathCreation;
using ShatterToolkit;
using UnityEngine;
using UnityEngine.Events;

public class RocketControl : MonoBehaviour
{
    public UnityEvent MovingStarted;
    public UnityEvent FreeFlightStarted;
    public UnityEvent BoostEnergyEnded;
    public UnityEvent<Target> ObstacleHitted;

    public UnityEvent BoostStart;
    public UnityEvent BoostStop;

    [SerializeField] private PathMover _initialPath;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _model;
    [SerializeField] private CameraControl _cameraControl;
    
    [Header("Free flight")]
    [SerializeField] private float _outputSpeed;
    [SerializeField] private float _gravity;

    [Header("Horizontal Move")]
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;

    [Header("Boost")] 
    [Min(1)][SerializeField] private float _boostSpeedModifier;
    [SerializeField] private float _boostEnergyCost;
    [SerializeField] private float _maxBoostEnergy;

    [Header("Explosion radius")] 
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;

    [Header("Trajectory Gizmos")] 
    [SerializeField] private int _accuracy = 100;
    [SerializeField] private float _targetYToDraw = 0f;

    [Header("FX")] 
    [SerializeField] private bool _useParticles;
    [SerializeField] private TrailRenderer _sliderTrail;
    [SerializeField] private ParticleSystem _drive;
    [SerializeField] private ParticleSystem _boostDrive;
    [SerializeField] private ParticleSystem _explosion;

    [Header("Sound")] 
    [SerializeField] private AudioSource _explosionAudio;
    [SerializeField] private AudioSource _flyAudio;
    [SerializeField] private float _soundStartingTime;
    [SerializeField] private float _defaultVolume;
    [SerializeField] private float _boostVolume;

    private Vector3 _forwardMoveDirection;

    private bool _isFreeFlight;
    private float _targetXPos;
    private float _moveStartTime;

    private bool _isBoosting;
    private float _currentBoostEnergy;

    private Coroutine _boostRoutine;
    private Tween _boostSoundRoutine;

    private float CurrentForwardSpeed => _isBoosting ? _outputSpeed * _boostSpeedModifier : _outputSpeed;
    public float CurrentBoostNormalized => _currentBoostEnergy / _maxBoostEnergy;
    public bool HasBoost => _currentBoostEnergy > 0;
    public bool IsMovingHorizontally;

    private IEnumerator Start()
    {
        _initialPath.PlaceAtStart();

        yield return null;

        _sliderTrail.gameObject.SetActive(false);
        _sliderTrail.Clear();
        _currentBoostEnergy = _maxBoostEnergy;
    }

    public void AddSpeed(float speed)
    {
        _outputSpeed += speed;
    }

    public void AddBoost(float boost)
    {
        _currentBoostEnergy += boost;
        _maxBoostEnergy += boost;
    }

    public void AddExplosion(float radius, float force)
    {
        _explosionForce += force;
        _explosionRadius += radius;
    }

    public void StartBoost()
    {
        if (_isFreeFlight == false || _currentBoostEnergy <= 0)
            return;

        _isBoosting = true;
        _boostRoutine = StartCoroutine(BoostRoutine());

        _cameraControl.Camera.Camera.DOFieldOfView(70f, 0.5f);
        _cameraControl.Camera.SpeedEffect.gameObject.SetActive(true);

        if (_useParticles)
        {
            _drive.gameObject.SetActive(false);
            _boostDrive.gameObject.SetActive(true);
        }

        BoostStart?.Invoke();

        _boostSoundRoutine?.Kill();
        _boostSoundRoutine = DOTween.To(() => _flyAudio.volume, v => _flyAudio.volume = v, _boostVolume, 1f);
    }

    public void StopBoost()
    {
        if (_isFreeFlight == false)
            return;

        if (_boostRoutine != null)
            StopCoroutine(_boostRoutine);

        _isBoosting = false;

        _cameraControl.Camera.Camera.DOFieldOfView(55f, 0.5f);
        _cameraControl.Camera.SpeedEffect.gameObject.SetActive(false);
        
        if (_useParticles)
        {
            _drive.gameObject.SetActive(true);
            _boostDrive.gameObject.SetActive(false);
        }

        BoostStop?.Invoke();

        _boostSoundRoutine?.Kill();
        _boostSoundRoutine = DOTween.To(() => _flyAudio.volume, v => _flyAudio.volume = v, _defaultVolume, 1f);
    }

    [EditorButton]
    public void StartMoving()
    {
        _sliderTrail.gameObject.SetActive(true);
        _sliderTrail.Clear();

        _initialPath.StartMove();
        _initialPath.pathCompleted.AddListener(StartFreeFlight); 
        MovingStarted?.Invoke();
        
        if (_useParticles)
        {
            _drive.gameObject.SetActive(true);
        }

        _flyAudio.Play();
        _flyAudio.loop = true;

        _boostSoundRoutine?.Kill();
        _boostSoundRoutine = DOTween.To(() => _flyAudio.volume, v => _flyAudio.volume = v, _defaultVolume, _soundStartingTime);
    }

    private void StartFreeFlight(Vector3 outputDirection)
    {
        _sliderTrail.transform.parent = transform.parent;
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

            var fingers = LeanTouch.GetFingers(true);

            if (fingers.Count > 0)
            {
                _targetXPos += fingers[0].ScaledDelta.normalized.x * _horizontalSpeed * Time.deltaTime;
                _targetXPos = Mathf.Clamp(_targetXPos, _minX, _maxX);
            }

            if (_isBoosting)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

            var position = _rigidbody.position + forwardVector;
            position -= gravityVector;

            //print("for " + (forwardVector + gravityVector));

            var rotation = Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(forwardVector + Vector3.right * (_targetXPos - position.x) / (Mathf.Sqrt(gravityVector.y) * 0.75f) - gravityVector), 4f * Time.deltaTime);

            position.x = _targetXPos;
            _rigidbody.MovePosition(position);
            _rigidbody.MoveRotation(rotation);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_isFreeFlight == false)
            return;

        var destractablePart = collider.gameObject.GetComponent<DestractablePart>();

        _isFreeFlight = false;
        _model.SetActive(false);

        if (destractablePart != null)
        {
            var target = destractablePart.Hit(transform.position);
            ObstacleHitted?.Invoke(target);

            StartCoroutine(ExplosionRoutine());
        }
        else
        {
            ObstacleHitted?.Invoke(null);
        }

        _flyAudio.Stop();
        _explosion.Play(true); 
        _explosionAudio.Play();

        if (GameManager.Instance.HapticOn)
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return null;

        var colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        
        foreach (var col in colliders)
        {
            if (col.attachedRigidbody != null)
            {
                var part = col.GetComponent<DestractablePart>();
                
                if(part != null)
                    part.Hit(part.transform.position);
            }
        }

        yield return null;

        colliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (var col in colliders)
        {
            if (col.attachedRigidbody != null)
            {
                var part = col.GetComponent<DestractablePart>();

                if (part != null)
                    part.Hit(part.transform.position);
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
        BoostEnergyEnded?.Invoke();
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