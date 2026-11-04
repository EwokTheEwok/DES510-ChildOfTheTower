using UnityEngine;
using UnityEngine.Events;

public class ShadowOcean : MonoBehaviour
{
    UnityEvent _RestartEvent = new();

    public void OnActivate(UnityAction call)
    {
        _RestartEvent.AddListener(call);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _RestartEvent?.Invoke();
        }
    }
}
