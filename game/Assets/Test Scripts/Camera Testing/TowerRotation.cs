using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRotation : MonoBehaviour
{
    public Transform pivot;                  // the fixed point to orbit around (we can hook an object up to the script IN unity)
    public float rotationStep = 45f;         // degrees per key press
    public float camRotationDuration = 0.5f;
    public Ease ease = Ease.InOutSine;

    bool isCameraRotating = false;

    void Update()
    {
        if (isCameraRotating) return;

        //pass in the amount of degrees to rotate to OrbitCamera
        if (Input.GetKeyDown(KeyCode.Q))
            OrbitCamera(rotationStep);
        else if (Input.GetKeyDown(KeyCode.E))
            OrbitCamera(-rotationStep);
    }

    void OrbitCamera(float degrees)
    {
        isCameraRotating = true;

        float rotatedSoFar = 0f;

        /*rotatedSoFar is how much the camera has rotated 
        x is set to the number of degrees needed to turn
        the order is getter, setter, to, and duration
        getter is what the camera is currently at
        setter sets the value of the property
        to is the end value
        and duration is duration
        in this instance rotatedSoFar starts at 0
        x is our setter and will be set to 45 which is our to
        */
        DOTween.To(() => rotatedSoFar, x =>
        {
            /*x is the total we need to rotate rotatedSoFar is the amount we have rotated 
            (so like 45 - 23 = 22 we need to rotate 22 more degrees)*/
            float delta = x - rotatedSoFar;

            // this function updates by frame so this ensures the camera keeps updating its position
            rotatedSoFar = x;

            //actually rotates around the designated position
            transform.RotateAround(pivot.position, Vector3.up, delta);

            //degrees tells x what it should be
        }, degrees, camRotationDuration)
         //uses ease to smooth the turn and then sets rotating camera to false to stop
        .SetEase(ease)
        .OnComplete(() => isCameraRotating = false);
    }
}
