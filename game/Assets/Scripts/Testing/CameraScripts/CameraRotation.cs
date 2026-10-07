using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    //variables
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

        if (Input.GetKeyDown(KeyCode.E))
        {

            if (!isCameraRotating)
            {
                //add 45 to the rotation
                camRotation += 45; 
                isCameraRotating = true;

                //rotate +45 degrees over 0.5 seconds
                transform.DORotate(new Vector3(0, camRotation, 0), camRotationDuration); 
                Invoke("SetCameraRotateToFalse", camRotationDuration);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (!isCameraRotating)
            {
                camRotation -= 45;
                isCameraRotating = true;

                //rotate -45 degrees over 0.5 seconds
                transform.DORotate(new Vector3(0, camRotation, 0), camRotationDuration); 
                //wait until the camera has fully rotated and the change to false
                Invoke("SetCameraRotateToFalse", camRotationDuration); 
            }
        }
    }

    private void SetCameraRotateToFalse()
    {
        isCameraRotating = false;
    }

}
