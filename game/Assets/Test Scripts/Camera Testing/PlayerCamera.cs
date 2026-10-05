using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Public
    public Transform pivotObject;               //Object to pivot around
    public float rotationDegrees = 60f;            //Angle of rotation
    public float camRotationDuration = 0.5f;    //Smoothing
    public Ease ease = Ease.InOutSine;          //Interpolation

    //Private
    bool isCameraRotating = false;
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
            OrbitCamera(cameraMoveDir * rotationDegrees);
    }

    void OrbitCamera(float degrees)
    {
        isCameraRotating = true;
        float Rotation = 0f;

        DOTween.To(
            () => Rotation,                                         //Getter (get rotation)
            newRotation => { float delta = newRotation - Rotation;  //Setter (set rotation and other functionality)
                             Rotation = newRotation;                //update rotation, then rotate around pivot using the tweened delta
                             playerInput.camera.transform.RotateAround(pivotObject.position, Vector3.up, delta); },
            degrees,
            camRotationDuration).SetEase(ease).OnComplete(() => isCameraRotating = false);
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        cameraMoveDir = context.ReadValue<float>();
        Debug.Log(cameraMoveDir);
    }
}
