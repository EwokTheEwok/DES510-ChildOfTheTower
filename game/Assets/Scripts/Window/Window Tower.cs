using UnityEngine;

public class WindowTower : MonoBehaviour
{
    public GameObject _CCamera;
    GameObject _Tower;
    Renderer _TowerRenderer;

    private void Start()
    {
        _Tower = GameObject.FindWithTag("Tower");
        _TowerRenderer = _Tower.GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 cameraRotation = _CCamera.transform.eulerAngles;
        Vector3 cameraPos = _CCamera.transform.position;
        Vector3 TowerPos = _Tower.transform.position;

        _TowerRenderer.material.SetVector("_Tower_Position", TowerPos);
        _TowerRenderer.material.SetVector("_Camera_Rotation", cameraRotation);
        _TowerRenderer.material.SetVector("_Camera_Position", cameraPos);
    }
} 