using UnityEngine;

public class CameraRuules : MonoBehaviour
{
    public Transform player;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 target = player.position + Vector3.up;

        // Where the player is on screen
        Vector3 vp = cam.WorldToViewportPoint(target);
        Shader.SetGlobalVector("Position", new Vector4(vp.x, vp.y, 0, 0));

        // When the wall is infront of the player
        Shader.SetGlobalVector("PlayerPos2", target);
        Shader.SetGlobalVector("CameraForward", transform.forward);
        Debug.Log(vp);

        // Flattening the camera veiw to stop weird graphical glitch effect on angled walls
        Vector3 flat = transform.forward;
        flat.y = 0;
        Shader.SetGlobalVector("CameraForwardFlat", flat.normalized);
    }
}