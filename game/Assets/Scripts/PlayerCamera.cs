using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Public
    public Transform pivotObject;               //Object to pivot around
    public float camRotationDuration = 0.5f;    //Smoothing
    public int rotationSides = 6;
    public Ease ease = Ease.InOutSine;          //Interpolation
    public CinemachineCamera playerCamera;

    //Private
    bool isCameraRotating = false;
    float rotationDegrees = 0f;
    float tweenRotation = 0f;
    float cameraMoveDir;
    Transform targetTransform;

    void Start()
    {
        targetTransform = playerCamera.Target.TrackingTarget;
        targetTransform.position = transform.position;
        //playerInput.camera.transform.LookAt(pivotObject, Vector3.up);
        if (rotationSides != 0)
            rotationDegrees = 360 / (float)rotationSides;
        else
            rotationDegrees = 360 / 6f;

    }

    void Update()
    {
        if (playerCamera.isActiveAndEnabled)
        {
            if (!isCameraRotating && cameraMoveDir != 0f)
            {
                OrbitCamera(cameraMoveDir * rotationDegrees);
            }
            //targetTransform.position = new Vector3(pivotObject.position.x, transform.position.y, pivotObject.position.z);
            if (isCameraRotating)
                playerCamera.transform.eulerAngles = new Vector3(playerCamera.transform.eulerAngles.x, tweenRotation, playerCamera.transform.eulerAngles.z);
        }
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
