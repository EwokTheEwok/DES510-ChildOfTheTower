using AK.Wwise;
using UnityEngine;

public class ShadowAudio : MonoBehaviour
{
    public AK.Wwise.Event ShadowEvent;

    private GameObject _player;

    private float _playerDistance = 10;
    private float _playerDistanceThreshold = 100;

    private bool _isPlaying = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", _playerDistance);

        if (_playerDistance < _playerDistanceThreshold && !_isPlaying)
        {
            ShadowEvent.Post(gameObject);
            _isPlaying = true;
        }

        else if (_playerDistance > _playerDistanceThreshold)
        {
            ShadowEvent.Stop(gameObject);
            _isPlaying = false;
        }
    }
}
