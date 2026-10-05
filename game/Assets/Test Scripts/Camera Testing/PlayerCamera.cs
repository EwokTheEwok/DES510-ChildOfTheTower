using DG.Tweening;
using DG.Tweening.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Public
    public Transform pivotObject;               //Object to pivot around
    public float rotationDegrees = 60f;            //Angle of rotation
    public float camRotationDuration = 0.5f;    //Smoothing
    public Ease ease = Ease.InOutSine;          //Interpolation
    public CinemachineCamera playerCamera;

    //Private
    bool isCameraRotating = false;
    float tweenRotation = 0f;
    float cameraMoveDir;
    PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.camera.transform.LookAt(pivotObject, Vector3.up);

    }

    void Update()
    {
        if (!isCameraRotating && cameraMoveDir != 0f)
        {
            OrbitCamera(cameraMoveDir * rotationDegrees);
        }
        if (isCameraRotating)
            playerCamera.transform.eulerAngles = new Vector3(playerCamera.transform.eulerAngles.x, tweenRotation, playerCamera.transform.eulerAngles.z);
    }

    void OrbitCamera(float degrees)
    {
        isCameraRotating = true;
        float endRotation = tweenRotation + degrees;
        DOTween.To(
            () => tweenRotation,
            lambaRotation => tweenRotation = lambaRotation,
            endRotation,
            camRotationDuration).SetEase(ease).OnComplete(() => isCameraRotating = false);
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        cameraMoveDir = context.ReadValue<float>();
    }
}
