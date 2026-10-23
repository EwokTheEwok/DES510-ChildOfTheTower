using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> {}

public class Archway : MonoBehaviour
{
    bool _Activated = false;
    Vector3 _RestartPosition;
    public GameObjectEvent _ActivateEvent = new GameObjectEvent();

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
