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

        // Where the player is on screen, 0-1 (feeds the Remap node)
        Vector3 vp = cam.WorldToViewportPoint(target);
        Shader.SetGlobalVector("Position", new Vector4(vp.x, vp.y, 0, 0));

        // For the "is this wall in front of the player" test
        Shader.SetGlobalVector("PlayerPos2", target);
        Shader.SetGlobalVector("CameraForward", transform.forward);
        Debug.Log(vp);
    }
}