using UnityEngine;

public class WindowScript : MonoBehaviour
{
    public float _WindowSize = 5f;
    public float _WindowFade = 0.5f;
    public GameObject _Player;
    public GameObject _CCamera;


    private void Start()
    {
        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
    }

    void Update()
    {
        Vector3 playerCameraPos = Camera.main.transform.InverseTransformPoint(_Player.transform.position);
        Vector3 playerWorldPos = _Player.transform.position;
        Vector3 cameraRotation = _CCamera.transform.eulerAngles;
        Vector3 cameraPos = _CCamera.transform.position;
        Ray floorRay = new (_Player.transform.position, Vector3.down);
        Physics.Raycast(floorRay, out RaycastHit hitinfo);
        Vector3 floorPos = hitinfo.point;

        Shader.SetGlobalFloat("_Floor_Position", floorPos.y);
        Shader.SetGlobalVector("_Player_Camera_Position", playerCameraPos);
        Shader.SetGlobalVector("_Player_World_Position", playerWorldPos);
        Shader.SetGlobalVector("_Camera_Rotation", cameraRotation);
        Shader.SetGlobalVector("_Camera_Position", cameraPos);
    }
} 