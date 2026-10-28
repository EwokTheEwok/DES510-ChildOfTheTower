using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerShadow : MonoBehaviour
{
    public DecalProjector projector;
    void Update()
    {
        bool hit = Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit hitInfo,
                1000f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
        float distance = hitInfo.distance + 0.5f;
        projector.size = new Vector3(1f, 1f, distance);
        projector.pivot = new Vector3(0f, 0f, distance / 2f);
    }
}
