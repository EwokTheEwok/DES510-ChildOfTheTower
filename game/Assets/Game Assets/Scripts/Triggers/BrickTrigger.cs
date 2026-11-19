using UnityEngine;
using UnityEngine.Events;

public class BrickTrigger : MonoBehaviour
{
    public UnityEvent onBrickDetected;
    bool _Triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block") && !_Triggered)
        {
            _Triggered = true;
            //Debug.Log("HasBrick");
            onBrickDetected?.Invoke();
        }
    }
}
