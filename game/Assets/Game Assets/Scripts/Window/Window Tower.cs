using UnityEngine;

public class WindowTower : MonoBehaviour
{
    GameObject _Player;
    Camera _Camera;
    GameObject _Tower;
    Renderer _TowerRenderer;

    public float _Radius = 20f;

    private void Start()
    {
        _Camera = Camera.main;
        _Tower = gameObject;
        _TowerRenderer = GetComponent<Renderer>();
        _Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 playerXZ = _Player.transform.position;
        playerXZ.y = 0f;
        Vector3 towerXZ = _Tower.transform.position;
        towerXZ.y = 0f;
        float distance = (playerXZ - towerXZ).magnitude;
        if (distance < _Radius) {

            Vector3 cameraRotation = _Camera.transform.eulerAngles;
            Vector3 cameraPos = _Camera.transform.position;
            Vector3 TowerPos = _Tower.transform.position;
            _TowerRenderer.material.SetFloat("_Active", 1);
            _TowerRenderer.material.SetVector("_Tower_Position", TowerPos);
            _TowerRenderer.material.SetVector("_Camera_Rotation", cameraRotation);
            _TowerRenderer.material.SetVector("_Camera_Position", cameraPos);
        }
        else
        {
            _TowerRenderer.material.SetFloat("_Active", 0);
        }
    }
} 