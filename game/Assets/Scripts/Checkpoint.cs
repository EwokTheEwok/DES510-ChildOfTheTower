using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    bool _Activated = false;
    Vector3 _RestartPosition;
    public UnityEvent<GameObject> _ActivateEvent = new ();

    void Start()
    {
        _RestartPosition = transform.Find("Restart").position;
    }

    public Vector3 RestartPosition()
    {
        return _RestartPosition;
    }
    
    public void OnActivate(UnityAction<GameObject> call)
    {
        _ActivateEvent.AddListener(call);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_Activated && other.CompareTag("Player"))
        {
            _Activated = true;
            _ActivateEvent.Invoke(gameObject);
        }
    }
}
