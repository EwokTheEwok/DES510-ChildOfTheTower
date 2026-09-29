using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float camRotationSpeed = 90f;
    float camRotation = 0;

    float camRotationDuration = 0.5f;
    bool isCameraRotating = false;

    void Update()
    {
        RotateCamera();
    }
    void RotateCamera()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            if (!isCameraRotating)
            {
                camRotation += 45;
                transform.DORotate(new Vector3(0, camRotation, 0), camRotationDuration);
                isCameraRotating = true;
                Invoke("SetCameraRotateToFalse", camRotationDuration);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            if (!isCameraRotating)
            {
                camRotation -= 45;
                transform.DORotate(new Vector3(0, camRotation, 0), camRotationDuration);
                isCameraRotating = true;
                Invoke("SetCameraRotateToFalse", camRotationDuration);
            }
        }
    }

    private void SetCameraRotateToFalse()
    {
        isCameraRotating = false;
    }

}
