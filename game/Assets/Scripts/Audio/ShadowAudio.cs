using AK.Wwise;
using System;
using Unity.Android.Types;
using UnityEngine;

public class ShadowAudio : MonoBehaviour
{
    public AK.Wwise.Event ShadowEvent;
    public AK.Wwise.Event StopShadows;
    public AK.Wwise.Event StopShadowsLongFade;

    private GameObject _player;

    private float _playerDistance;
    private float _playerDistanceThreshold = 13.1f;

    private bool _isPlaying = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", _playerDistance);

        //if (_playerDistance < _playerDistanceThreshold && !_isPlaying)
        //{
        //    ShadowEvent.Post(gameObject);
        //    _isPlaying = true;
        //}

        //else if (_playerDistance > _playerDistanceThreshold)
        //{
        //    ShadowEvent.Stop(gameObject);
        //    _isPlaying = false;
        //}
    }

    public void GetShadowDistance(float distance)
    {
        AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", distance);

        Debug.Log(distance);

        if (distance < _playerDistanceThreshold && !_isPlaying)
        {
            ShadowEvent.Post(gameObject);
            _isPlaying = true;
        }

        else if (distance > _playerDistanceThreshold)
        {
            StopShadowsLongFade.Post(gameObject);
            _isPlaying = false;
        }

        else if (distance < 2)
        {
            StopShadows.Post(gameObject);
            _isPlaying = false;
        }
    }
}
