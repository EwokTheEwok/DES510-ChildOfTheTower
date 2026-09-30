using UnityEngine;
using UnityEngine.UI;

public class CameraRuules : MonoBehaviour
{
    public Transform player;
    public MeshCollider towerCollider;
    public CapsuleCollider playerCollider;

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
        //Debug.Log(vp);

        // Flattening the camera veiw to stop weird graphical glitch effect on angled walls
        Vector3 flat = transform.forward;
        flat.y = 0;
        Shader.SetGlobalVector("CameraForwardFlat", flat.normalized);
    }
    void Update()
    {
        Vector3 playerCameraSpaceOffset = cam.worldToCameraMatrix.MultiplyPoint3x4(player.transform.position);
        Vector3 cameraHorizontalPos = transform.position + cam.cameraToWorldMatrix.MultiplyVector(new Vector3(playerCameraSpaceOffset.x, 0, 0));
        Vector3 cameraToPlayer = player.position - cameraHorizontalPos;
        var disp = cameraToPlayer;

        Debug.DrawRay(cameraHorizontalPos, disp, Color.white, 0.1f, true);

        /*if (towerCollider.bounds.Intersects(playerCollider.bounds))
        {
            Shader.SetGlobalInteger("InsideTower", 1);
        }
        else
        {
            
        }*/

            LayerMask wallMask = LayerMask.GetMask("Wall");
        if (Physics.Raycast(cameraHorizontalPos, disp, disp.magnitude, wallMask))
        {
            Shader.SetGlobalInteger("BehindWall", 1);
            //Debug.Log(true);
        }
        else
        {
            Shader.SetGlobalInteger("BehindWall", 0);
            //Debug.Log(false);
        }
    }
}