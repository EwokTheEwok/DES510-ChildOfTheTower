using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using EasyButtons;
using EasyButtons.Editor;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Instance
    {
        get { return _Instance; }
    }

    [Header("Checkpoint")]
    public string _CheckpointTag = "Checkpoint";
    private List<GameObject> _CheckpointList = new();
    private GameObject _CurrentCheckpoint = null;

    [Header("Shadow Ocean")]
    public float _MinSpeed = 0.5f;
    public float _MaxSpeed = 10f;
    public float _MinDistance = 5f;
    public float _MaxDistance = 30f;
    public float _ResetDistance = -30f;
    public float _StartHeight = -30f;
    private bool _Moving = false;

    [Header("References")]
    public GameObject _Player = null;
    public GameObject _ShadowOcean = null;

    [Header("Events")]
    public UnityEvent<float> _ShadowDistanceEvent = new();
    public UnityEvent<GameObject> _CheckpointEvent = new();
    public UnityEvent _FallInShadowEvent = new();
    public UnityEvent<bool> _PauseEvent = new();

    private bool _Paused = false;
    public bool Paused
    {
        get => _Paused;
        set
        {
            Debug.Log("Game Manager: Pause (" + value + ")");
            _Paused = value;
            _PauseEvent?.Invoke(value);
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _CheckpointList = GameObject.FindGameObjectsWithTag(_CheckpointTag).ToList();
        foreach (GameObject checkpointObject in _CheckpointList)
        {
            checkpointObject.GetComponent<Checkpoint>().OnActivate(CheckpointActivate);
        }

        _ShadowOcean.GetComponent<ShadowOcean>().OnActivate(Restart);
        _ShadowOcean.transform.position = new(0f, _StartHeight, 0f);
    }

    private void FixedUpdate()
    {
        float distanceFromPlayer = _Player.transform.position.y - _ShadowOcean.transform.position.y;
        _ShadowDistanceEvent?.Invoke(distanceFromPlayer);
        if (_Moving && _Paused)
        {
            float distanceFromMinimum = distanceFromPlayer - _MinDistance;
            float distanceScale = Mathf.Clamp(distanceFromMinimum / (_MaxDistance - _MinDistance), 0f, 1f);
            float speed = Mathf.SmoothStep(_MinSpeed, _MaxSpeed, distanceScale);
            _ShadowOcean.transform.position += new Vector3(0f, speed * Time.fixedDeltaTime, 0f);
        }
    }

    private void Restart()
    {
        _FallInShadowEvent?.Invoke();
        CharacterController chara = _Player.GetComponent<CharacterController>();
        BlockController2 controller2 = _Player.GetComponent<BlockController2>();
        controller2.Reset();
        Vector3 restartPosition = _CurrentCheckpoint.GetComponent<Checkpoint>().RestartPosition();
        _ShadowOcean.transform.position = restartPosition + new Vector3(0f, _ResetDistance, 0f);
        chara.enabled = false;
        _Player.transform.position = restartPosition;
        chara.enabled = true;
    }

    private void CheckpointActivate(GameObject checkpointObject, bool startStop)
    {
        _Moving = startStop;
        _CurrentCheckpoint = checkpointObject;
        _CheckpointEvent?.Invoke(checkpointObject);
        Debug.Log("Checkpoint: " + _CheckpointList.IndexOf(_CurrentCheckpoint) + " Activated");
    }

    public void AddPauseListener(UnityAction<bool> call)
    {
        _PauseEvent.AddListener(call);
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void PausePlayGame()
    {
        Paused = !Paused;
    }
}
