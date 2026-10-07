using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Public
    public Transform _PivotObject;               //Object to pivot around
    public float _CamRotationDuration = 0.5f;    //Smoothing
    public int _RotationSides = 6;
    public Ease _Ease = Ease.InOutSine;          //Interpolation
    public CinemachineCamera _PlayerCamera;

    //Private
    bool _IsCameraRotating = false;
    float _RotationDegrees = 0f;
    float _TweenRotation = 0f;
    float _CameraMoveDir;
    Transform _TargetTransform;

    void Start()
    {
        _TargetTransform = _PlayerCamera.Target.TrackingTarget;
        _TargetTransform.position = transform.position;
        //playerInput.camera.transform.LookAt(_PivotObject, Vector3.up);
        if (_RotationSides != 0)
            _RotationDegrees = 360 / (float)_RotationSides;
        else
            _RotationDegrees = 360 / 6f;

    }

    void Update()
    {
        if (_PlayerCamera.isActiveAndEnabled)
        {
            if (!_IsCameraRotating && _CameraMoveDir != 0f)
            {
                OrbitCamera(_CameraMoveDir * _RotationDegrees);
            }
            //targetTransform.position = new Vector3(pivotObject.position.x, transform.position.y, pivotObject.position.z);
            if (_IsCameraRotating)
                _PlayerCamera.transform.eulerAngles = new Vector3(_PlayerCamera.transform.eulerAngles.x, _TweenRotation, _PlayerCamera.transform.eulerAngles.z);
        }
    }

    void OrbitCamera(float degrees)
    {
        _IsCameraRotating = true;
        float endRotation = _TweenRotation + degrees;
        DOTween.To(
            () => _TweenRotation,
            lambaRotation => _TweenRotation = lambaRotation,
            endRotation,
            _CamRotationDuration).SetEase(_Ease).OnComplete(() => _IsCameraRotating = false);
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        _CameraMoveDir = context.ReadValue<float>();
    }
}
