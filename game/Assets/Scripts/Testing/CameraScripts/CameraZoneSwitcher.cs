using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Collections;


public class CameraZoneSwtcher : MonoBehaviour
{
    public string PlayerTag = "Player";

    public CinemachineCamera PrimaryCamera;
    public CinemachineCamera PlayerCamera;

    CinemachineCamera[] VirtualCameras = new CinemachineCamera[2];

    private void Start() {

        VirtualCameras[0] = PrimaryCamera;
        VirtualCameras[1] = PlayerCamera;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            CinemachineCamera targetCamera = other.GetComponentInChildren<CinemachineCamera>();
            SwitchToCamera(targetCamera);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag)) {
            SwitchToCamera(PrimaryCamera);
        }
    }

    private void SwitchToCamera(CinemachineCamera targetCamera)
    {
        foreach (CinemachineCamera cam in VirtualCameras)
        {
            cam.enabled = cam == targetCamera;
        }
    }
}
