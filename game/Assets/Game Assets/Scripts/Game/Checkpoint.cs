using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public bool _StartStopOcean = true;
    public ParticleSystem _FanfarePrefab;

    bool _Activated = false;
    Vector3 _RestartPosition;
    UnityEvent<GameObject, bool> _ActivateEvent = new();

    void Start()
    {
        _RestartPosition = transform.Find("Restart").position;
    }

    public Vector3 RestartPosition()
    {
        return _RestartPosition;
    }
    
    public void OnActivate(UnityAction<GameObject, bool> call)
    {
        _ActivateEvent.AddListener(call);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_Activated && other.CompareTag("Player"))
        {
            ParticleSystem fanfare = Instantiate(_FanfarePrefab);
            fanfare.transform.position = _RestartPosition;
            fanfare.Play();
            _Activated = true;
            _ActivateEvent?.Invoke(gameObject, _StartStopOcean);
        }
    }
}
