
using UnityEngine;

public class WindowTower : MonoBehaviour
{
    public GameObject _Tower;
    public GameObject _CCamera;

    void Update()
    {
        Vector3 cameraRotation = _CCamera.transform.eulerAngles;
        Vector3 cameraPos = _CCamera.transform.position;
        Vector3 TowerPos = _Tower.transform.position;

        Shader.SetGlobalVector("_Tower_Position", TowerPos);
        Shader.SetGlobalVector("_Camera_Rotation", cameraRotation);
        Shader.SetGlobalVector("_Camera_Position", cameraPos);
    }
} 