using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Collections;


public class CameraZoneSwtcher : MonoBehaviour
{
    public string TriggerTag;

    public CinemachineCamera PrimaryCamera;
    public CinemachineCamera[] VirtualCameras;

    private void Start() {

        SwitchToCamera(PrimaryCamera);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TriggerTag))
        {
            CinemachineCamera targetCamera = other.GetComponentInChildren<CinemachineCamera>();
            SwitchToCamera(targetCamera);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TriggerTag)) {
            SwitchToCamera(PrimaryCamera);
        }
    }

        private void SwitchToCamera(CinemachineCamera targetCamera)
    {
        foreach (CinemachineCamera camera in VirtualCameras) {
            camera.enabled = camera == targetCamera;
        }

    }
}
