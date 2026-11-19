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

    Vector3 _TowerPivotPosition;
    Transform _PlayerTowerTarget;
    Transform _PlayerBlockTarget;

    bool _InsideTower = false;
    bool _BlockTargetActive = false;
    GameObject _Player = null;

    void Start()
    {
        _Player = transform.parent.gameObject;

        _TweenRotation = transform.eulerAngles.y;
        _CCamera = GetComponent<CinemachineCamera>();
        if (_RotationSidesOcta != 0)
            _RotationDegreesOcta = 360f / (float)_RotationSidesOcta;
        else
            _RotationDegreesOcta = 360f / 8f;

        if (_RotationDegreesCube != 0)
            _RotationDegreesCube = 360f / (float)_RotationDegreesCube;
        else
            _RotationDegreesCube = 360f / 4f;

        _PlayerTowerTarget = new GameObject("Player Tower Target").transform;
        _PlayerBlockTarget = new GameObject("Player Block Target").transform;

        _TowerPivotPosition = GameManager.Instance.TowerPivot;
        _PlayerTowerTarget.position = _TowerPivotPosition;
        _PlayerBlockTarget.position = _TowerPivotPosition;
    }

    void Update()
    {
        if (_CCamera.enabled)
        {
            if (_InsideTower)
            {
                Vector3 newPosition = _PlayerTowerTarget.position;
                newPosition.y = transform.parent.position.y;
                _PlayerTowerTarget.position = newPosition;
            }

            if (!_IsCameraRotating)
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
    }

    public void SetPlayerBlockPosition(Vector3 _playerPposition, Vector3 _blockPosition, bool _farFromBlock)
    {
        if (_farFromBlock)
        {
            if (_InsideTower)
                _PlayerBlockTarget.position = _PlayerTowerTarget.position;
            else
                _PlayerBlockTarget.position = _Player.transform.position;
        }
        else
        {
            if (_InsideTower)
                _PlayerBlockTarget.position = Vector3.Lerp(_PlayerTowerTarget.position, _blockPosition, 0.5f);
            else
                _PlayerBlockTarget.position = Vector3.Lerp(_playerPposition, _blockPosition, 0.5f);
        }
    }

    public void SetBlockMode(bool _active)
    {
        _BlockTargetActive = _active;
        if (_BlockTargetActive)
        {
            _CCamera.Target.TrackingTarget = _PlayerBlockTarget;
        }
        else
        {
            if (_InsideTower)
                _CCamera.Target.TrackingTarget = _PlayerTowerTarget;
            else
                _CCamera.Target.TrackingTarget = _Player.transform;
        }
    }

    public void SetInsideMode(bool _inside)
    {
        _InsideTower = _inside;
        if (_InsideTower)
        {
            if (_BlockTargetActive)
                _CCamera.Target.TrackingTarget = _PlayerBlockTarget;
            else
                _CCamera.Target.TrackingTarget = _PlayerTowerTarget;
        }
        else
        {
            if (_BlockTargetActive)
                _CCamera.Target.TrackingTarget = _PlayerBlockTarget;
            else
                _CCamera.Target.TrackingTarget = _Player.transform;
        }
    }

    public void ResetTarget()
    {
        SetInsideMode(_InsideTower);
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
