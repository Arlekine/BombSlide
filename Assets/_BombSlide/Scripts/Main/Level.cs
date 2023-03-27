using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private CameraControl _cameraControl;
    [SerializeField] private RocketControl _rocketControl;
    [SerializeField] private Target _targetToPass;

    public CameraControl CameraControl => _cameraControl;
    public RocketControl RocketControl => _rocketControl;
    public Target TargetToPass => _targetToPass;
}