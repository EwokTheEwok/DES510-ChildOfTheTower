using UnityEngine;
using UnityEngine.Events;

public class AudioTrigger : MonoBehaviour
{
    UnityEvent<GameObject> _ActivateEvent = new();
    public UnityEvent<GameObject> ActivateEvent { get => _ActivateEvent; }
    bool _Activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_Activated && other.CompareTag("Player"))
        {
            _Activated = true;
            _ActivateEvent?.Invoke(gameObject);
        }
    }
}
