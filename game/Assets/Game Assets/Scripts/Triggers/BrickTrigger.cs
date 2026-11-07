using UnityEngine;
using UnityEngine.Events;

public class BrickTrigger : MonoBehaviour
{
    public UnityEvent onBrickDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            Debug.Log("HasBrick");
            onBrickDetected?.Invoke();
        }
    }
}
