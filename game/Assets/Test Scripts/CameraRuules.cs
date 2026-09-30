using UnityEngine;
using UnityEngine.UI;

public class CameraRuules : MonoBehaviour
{
    public Transform player;
    Camera cam;
    float size = 2.0f;
    Vector3 worldPivotPos = new (0,0,0);

    void Awake()
    {
        cam = GetComponent<Camera>();
        Shader.SetGlobalFloat("Size", size);
    }

    void LateUpdate()
    {
        Vector3 playerPos = player.position + Vector3.up;

        // Where the player is on screen
        Vector3 vp = cam.WorldToViewportPoint(playerPos);
        Shader.SetGlobalVector("PlayerScreenPos", new Vector4(vp.x, vp.y, 0, 0));

        // When the wall is infront of the player
        Shader.SetGlobalVector("PlayerWorldPos", playerPos);
        Shader.SetGlobalVector("CameraForward", transform.forward);
        Debug.Log(vp);

        // Flattening the camera veiw to stop weird graphical glitch effect on angled walls
        Vector3 flat = transform.forward;
        flat.y = 0;
        Shader.SetGlobalVector("CameraForwardFlat", flat.normalized);
    }
    void Update()
    {
        var cameraToPivot = worldPivotPos - transform.position;
        var pivotToPlayer = player.transform.position - worldPivotPos;
        var orthoCameraPos = pivotToPlayer + cameraToPivot;

        var dir = cameraToPivot.normalized;

        //var ray = new Ray(transform.position, dir);
        LayerMask wallMask = LayerMask.GetMask("Wall");


        Debug.DrawRay(orthoCameraPos, dir.normalized * 1000, Color.white, 0.1f, true);


        if (Physics.Raycast(orthoCameraPos, dir.normalized, Mathf.Infinity, wallMask))
        {
            Shader.SetGlobalInt("BehindWall", 1);
            Debug.Log(true);
        }
        else
        {
            Shader.SetGlobalInt("BehindWall", 0);
            Debug.Log(false);
        }
    }
}