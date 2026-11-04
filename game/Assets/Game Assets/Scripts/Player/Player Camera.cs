using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Public
    public float _CamRotationDuration = 0.5f;    //Smoothing
    public int _RotationSidesOcta = 8;
    public int _RotationSidesCube = 4;
    public Ease _Ease = Ease.InOutSine;          //Interpolation

    //Private
    CinemachineCamera _CCamera;
    bool _IsCameraRotating = false;
    float _RotationDegreesOcta = 0f;
    float _RotationDegreesCube = 0f;
    float _TweenRotation = 0f;
    float _CameraMoveDir;

    bool _CubeMode = false;
    bool _SwapToCube = false;
    float _BlockRotationY = 0f;
    float _CameraRotationY = 0f;

    void Start()
    {
        _CCamera = GetComponent<CinemachineCamera>();
        if (_RotationSidesOcta != 0)
            _RotationDegreesOcta = 360f / (float)_RotationSidesOcta;
        else
            _RotationDegreesOcta = 360f / 8f;

        if (_RotationDegreesCube != 0)
            _RotationDegreesCube = 360f / (float)_RotationDegreesCube;
        else
            _RotationDegreesCube = 360f / 4f;

    }

    void Update()
    {
        if (_CCamera.enabled && !_IsCameraRotating)
        {
            if (_CameraMoveDir != 0f)
            {
                if (!_CubeMode)
                    TweenCamera(_CameraMoveDir * _RotationDegreesOcta);
                else
                {
                    TweenCamera(_CameraMoveDir * _RotationDegreesCube);
                }
            }
            else if (_SwapToCube)
            {
                _SwapToCube = false;
                TweenNearest();
            }
        }
    }

    public void SetCubeMode(float _blockRotationY)
    {
        _CubeMode = true;
        _SwapToCube = true;
        _BlockRotationY = _blockRotationY;
        _CameraRotationY = _CCamera.transform.eulerAngles.y;
    }

    public void SetOctaMode()
    {
        _CubeMode = false;
    }

    void TweenNearest()
    {
        _IsCameraRotating = true;
        float differenceAngle = _CameraRotationY - _BlockRotationY;
        float nearestAngle = Mathf.Round((differenceAngle / 90f) + 0.01f) * 90f;
        float endRotation = nearestAngle + _BlockRotationY;
        _TweenRotation = _CCamera.transform.eulerAngles.y;
        //Debug.Log("(" + _BlockRotationY + " / " + _CCamera.transform.eulerAngles.y + " )/ " + differenceAngle + " / " + nearestAngle + " / " + endRotation);
        DOTween.To(
            () => _TweenRotation,
            lambaRotation => {
                _TweenRotation = lambaRotation;
                _CCamera.transform.eulerAngles = new Vector3(_CCamera.transform.eulerAngles.x, _TweenRotation, _CCamera.transform.eulerAngles.z);
            },
            endRotation,
            _CamRotationDuration).SetEase(_Ease).OnComplete(() => _IsCameraRotating = false);
    }

    void TweenCamera(float degrees)
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
