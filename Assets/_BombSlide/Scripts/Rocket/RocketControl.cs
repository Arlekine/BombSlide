using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using PathCreation;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private PathMover _initialPath;
    [SerializeField] private Rigidbody _rigidbody;
    
    [Header("Free flight")]
    [SerializeField] private Transform _outputDirection;
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
        _initialPath.SetPositionByDistance(0f);

        _currentBoostEnergy = _maxBoostEnergy;
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
        _rigidbody.isKinematic = true;

        _initialPath.StartMove();
        _initialPath.pathCompleted.AddListener(StartFreeFlight);
    }

    private void StartFreeFlight(Vector3 outputDirection)
    {
        _targetXPos = transform.position.x;
        _moveStartTime = Time.time;
        _isFreeFlight = true;
        _forwardMoveDirection = outputDirection;
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
            var gravityVector = Vector3.up * GetVerticalSpeed(_moveStartTime) * Time.fixedDeltaTime;

            var position = _rigidbody.position + forwardVector;
            position -= gravityVector;

            var rotation = Quaternion.LookRotation(forwardVector - gravityVector);

            var fingers = LeanTouch.GetFingers(true);

            if (fingers.Count > 0)
            {
                _targetXPos = Mathf.Lerp(_targetXPos, fingers[0].GetLastWorldPosition(10f).x, _horizontalMoveLerpParameter * Time.deltaTime);
                _targetXPos = Mathf.Clamp(_targetXPos, _minX, _maxX);
            }

            position.x = _targetXPos;
            _rigidbody.MovePosition(position);
            _rigidbody.MoveRotation(rotation);
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

    private float GetVerticalSpeed(float moveStartTime)
    {
        return (Time.time - moveStartTime) * _gravity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var forwardVector = _forwardMoveDirection * CurrentForwardSpeed * Time.fixedDeltaTime;
        var gravityVector = Vector3.up * GetVerticalSpeed(_moveStartTime) * Time.fixedDeltaTime;

        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + forwardVector);
        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position - gravityVector);
        
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + forwardVector - gravityVector);
    }
}