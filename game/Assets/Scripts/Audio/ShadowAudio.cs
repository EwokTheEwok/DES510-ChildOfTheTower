using AK.Wwise;
using Unity.Android.Types;
using UnityEngine;

public class ShadowAudio : MonoBehaviour
{
    public AK.Wwise.Event ShadowEvent;

    private GameObject _player;

    //test objects
    public GameObject _cube;
    public bool off;

    private float _playerDistance = 99;
    private float _playerDistanceThreshold = 100;

    private bool _isPlaying = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
       
        //for testing

        AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", _playerDistance);

        if (off)
        {
            _cube.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", _playerDistance);

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
