using UnityEngine;

public class WindowScript : MonoBehaviour
{
    public float _WindowSize = 5f;
    public float _WindowFade = 0.5f;
    public GameObject _Player;
    GameObject _Camera;


    private void Start()
    {
        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
        _Camera = Camera.main.gameObject;
    }

    void Update()
    {
        Vector3 playerCameraPos = Camera.main.transform.InverseTransformPoint(_Player.transform.position);
        Vector3 playerWorldPos = _Player.transform.position;
        Vector3 cameraPlayerRotation = Quaternion.LookRotation(_Player.transform.position - _Camera.transform.position, Vector3.up).eulerAngles;
        Vector3 cameraPos = _Camera.transform.position;
        Ray floorRay = new (_Player.transform.position, Vector3.down);
        Physics.Raycast(floorRay, out RaycastHit hitinfo);
        Vector3 floorPos = hitinfo.point;

        Shader.SetGlobalFloat("_Floor_Position", floorPos.y);
        Shader.SetGlobalVector("_Player_Camera_Position", playerCameraPos);
        Shader.SetGlobalVector("_Player_World_Position", playerWorldPos);
        Shader.SetGlobalVector("_Camera_Player_Rotation", cameraPlayerRotation);
        Shader.SetGlobalVector("_Camera_Position", cameraPos);
    }
} 