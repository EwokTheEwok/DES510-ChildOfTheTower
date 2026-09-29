using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float camRotationSpeed = 90f;
    float camRotation;

    float camRotationDuration = 0.5f;
    bool isCameraRotating = false;

    void Update()
    {
        RotateCamera();
    }
    void RotateCamera()
    {
        camRotation = Input.GetAxis("Horizontal") * camRotationSpeed;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            if (!isCameraRotating)
            {
                camRotation += 45;
                isCameraRotating = true;
                transform.DORotate(new Vector3(40, camRotation, 0), camRotationDuration);
                Invoke("SetCameraRotateToFalse", camRotationDuration);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            if (!isCameraRotating)
            {
                camRotation -= 45;
                isCameraRotating = true;
                transform.DORotate(new Vector3(40, -camRotation, 0), camRotationDuration);
                Invoke("SetCameraRotateToFalse", camRotationDuration);
            }
        }
    }

    private void SetCameraRotateToFalse()
    {
        isCameraRotating = false;
    }

}
