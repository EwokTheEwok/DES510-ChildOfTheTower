using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Public
    public float _CamRotationDuration = 0.5f;    //Smoothing
    public int _RotationSides = 8;
    public Ease _Ease = Ease.InOutSine;          //Interpolation

    //Private
    CinemachineCamera _CCamera;
    bool _IsCameraRotating = false;
    float _RotationDegrees = 0f;
    float _TweenRotation = 0f;
    float _CameraMoveDir;

    void Start()
    {
        _CCamera = GetComponent<CinemachineCamera>();
        if (_RotationSides != 0)
            _RotationDegrees = 360f / (float)_RotationSides;
        else
            _RotationDegrees = 360f / 8f;

    }

    void Update()
    {
        if (_CCamera.isActiveAndEnabled)
        {
            if (!_IsCameraRotating && _CameraMoveDir != 0f)
            {
                OrbitCamera(_CameraMoveDir * _RotationDegrees);
            }
        }
    }

    void OrbitCamera(float degrees)
    {
        _IsCameraRotating = true;
        float endRotation = _TweenRotation + degrees;
        DOTween.To(
            () => _TweenRotation,
            lambaRotation => { _TweenRotation = lambaRotation;
                               _CCamera.transform.eulerAngles = new Vector3(_CCamera.transform.eulerAngles.x, _TweenRotation, _CCamera.transform.eulerAngles.z);
            },
            endRotation,
            _CamRotationDuration).SetEase(_Ease).OnComplete(() => _IsCameraRotating = false);
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        _CameraMoveDir = context.ReadValue<float>();
    }
}
