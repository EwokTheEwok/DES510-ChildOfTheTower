using UnityEngine;

public class WindowTower : MonoBehaviour
{
    GameObject _Player;
    Camera _Camera;
    GameObject _Tower;

    bool _Active = false;

    private void Start()
    {
        _Camera = Camera.main;
        _Tower = gameObject;
        _Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (_Active) {
            
            Vector3 cameraRotation = _Camera.transform.eulerAngles;
            Vector3 TowerPos = _Tower.transform.position;
            Shader.SetGlobalFloat("_Tower_Active", 1);
            Shader.SetGlobalVector("_Tower_Position", TowerPos);
            Shader.SetGlobalVector("_Tower_Camera_Rotation", cameraRotation);
        }
        else
        {
            Shader.SetGlobalFloat("_Tower_Active", 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _Active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _Active = false;
    }
} 