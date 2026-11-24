using UnityEngine;
using UnityEngine.Events;

public enum ShadowFlag
{
    Default,
    Start,
    Stop
}

public enum WorldFlag
{
    Default,
    Rain,
    Clear,
    End
}

public class Checkpoint : MonoBehaviour
{
    [SerializeField] ShadowFlag _ShadowFlag = ShadowFlag.Default;
    [SerializeField] WorldFlag _WorldFlag = WorldFlag.Default;

    public ParticleSystem _FanfarePrefab;

    bool _Activated = false;
    Vector3 _RestartPosition;
    UnityEvent<GameObject, ShadowFlag, WorldFlag> _ActivateEvent = new();

    void Start()
    {
        _RestartPosition = transform.Find("Restart").position;
    }

    public Vector3 RestartPosition()
    {
        return _RestartPosition;
    }
    
    public void OnActivate(UnityAction<GameObject, ShadowFlag, WorldFlag> call)
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
            _ActivateEvent?.Invoke(gameObject, _ShadowFlag, _WorldFlag);
        }
    }
}
