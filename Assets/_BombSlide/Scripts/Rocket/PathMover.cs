using PathCreation;
using UnityEngine;
using UnityEngine.Events;

public class PathMover : MonoBehaviour
{
    public UnityEvent<Vector3> pathCompleted;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PathCreator _path;
    [SerializeField] private float _yOffset;
    [SerializeField] private float _pathSpeed;
    [SerializeField] private AnimationCurve _movingCurve;

    private float _currentDistance;
    private bool _isMoving;

    public void StartMove()
    {
        _currentDistance = 0f;
        _isMoving = true;
        SetPositionByDistance(_currentDistance);
    }

    public void SetPositionByDistance(float distance)
    {
        _rigidbody.MoveRotation(Quaternion.LookRotation(_path.path.GetDirectionAtDistance(distance)));
        _rigidbody.MovePosition(_path.path.GetPointAtDistance(distance) + transform.up * _yOffset);
    }

    private void Update()
    {
        if (_isMoving)
        {
            _currentDistance += _pathSpeed * Time.deltaTime * (_movingCurve.Evaluate(_currentDistance / _path.path.length + 0.01f));

            if (_currentDistance < _path.path.length)
            {
                SetPositionByDistance(_currentDistance);
            }
            else
            {
                _isMoving = false;
                pathCompleted?.Invoke(transform.forward);
            }
        }
    }
}