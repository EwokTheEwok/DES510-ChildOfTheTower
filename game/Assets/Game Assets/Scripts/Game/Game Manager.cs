using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Instance { get => _Instance; }
    public Vector3 TowerPivot { get => _TowerCollider.transform.position; }
    public GameObject Player { get => _Player; }
    public GameObject Blocks { get => _Blocks; }
    public GameObject UI { get => _UI; }
    public PlayerCamera PlayerCamera { get => _PlayerCCamera.GetComponent<PlayerCamera>(); }

    [Header("Checkpoint")]
    [SerializeField] string _CheckpointTag = "Checkpoint";
    List<GameObject> _CheckpointList = new();
    GameObject _CurrentCheckpoint = null;

    [Header("Spawn")]
    [SerializeField] GameObject _Spawn;

    [Header("Tower")]
    [SerializeField] GameObject _TowerCollider = null;



    [Header("Shadow Ocean")]
    [SerializeField] GameObject _ShadowOcean = null;
    [SerializeField] float _MinSpeed = 0.5f;
    [SerializeField] float _MaxSpeed = 10f;
    [SerializeField] float _MinDistance = 5f;
    [SerializeField] float _MaxDistance = 30f;
    [SerializeField] float _ResetDistance = -30f;
    bool _Moving = false;
    Vector3 _ShadowInitialPosition;

    [Header("Rain")]
    [SerializeField] GameObject _Rain;
    [SerializeField] GameObject _RainEmitter;

    [Header("Player")]
    [SerializeField] GameObject _PlayerPrefab;
    GameObject _Player = null;

    GameObject _DeathTrackingTarget;
    CharacterController _CharaController;
    BlockController2 _BlockController;
    InputActionMap _PlayerActionMap;
    CinemachineCamera _PlayerCCamera;
    [SerializeField] GameObject _Blocks = null;


    [Header("UI")]
    [SerializeField] GameObject _UI;


    [Header("Events")]
    public UnityEvent<float> _ShadowDistanceEvent = new();
    public UnityEvent<GameObject> _CheckpointEvent = new();
    public UnityEvent _FallInShadowEvent = new();
    public UnityEvent _RestartEvent = new();
    public UnityEvent<bool> _PauseEvent = new();

    IEnumerator _RestartCoroutine;

    private bool _Raining = true;
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

        List<GameObject> playerList = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (GameObject player in playerList)
        {
            Destroy(player);
        }

        _Player = Instantiate(_PlayerPrefab);
    }
    void Start()
    {
        _CheckpointList = GameObject.FindGameObjectsWithTag(_CheckpointTag).ToList();
        foreach (GameObject checkpointObject in _CheckpointList)
        {
            checkpointObject.GetComponent<Checkpoint>().OnActivate(CheckpointActivate);
        }

        _ShadowOcean.GetComponent<ShadowOcean>().OnActivate(FellInShadow);
        _ShadowOcean.transform.position = new(0f, _ShadowOcean.transform.position.y, 0f);
        _ShadowInitialPosition = _ShadowOcean.transform.position;

        _Player.transform.position = _Spawn.transform.position;
        _CharaController = _Player.GetComponent<CharacterController>();
        _BlockController = _Player.GetComponent<BlockController2>();
        _PlayerActionMap = _Player.GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _PlayerCCamera = _Player.transform.Find("CCamera").GetComponent<CinemachineCamera>();
        _DeathTrackingTarget = new GameObject("Death Tracking Target");
    }

    private void FixedUpdate()
    {
        float distanceFromPlayer = _Player.transform.position.y - _ShadowOcean.transform.position.y;
        _ShadowDistanceEvent?.Invoke(distanceFromPlayer);
        if (_Moving && !_Paused)
        {
            float distanceFromMinimum = distanceFromPlayer - _MinDistance;
            float distanceScale = Mathf.Clamp(distanceFromMinimum / (_MaxDistance - _MinDistance), 0f, 1f);
            float speed = Mathf.SmoothStep(_MinSpeed, _MaxSpeed, distanceScale);
            _ShadowOcean.transform.position += new Vector3(0f, speed * Time.fixedDeltaTime, 0f);
        }

        if (_Raining)
        {
            Vector3 rainPosition = _Rain.transform.position;
            rainPosition.y = _Player.transform.position.y + 25f;
            Vector3 rainEmitterPosition = _RainEmitter.transform.position;
            rainEmitterPosition.x = _Player.transform.position.x;
            rainEmitterPosition.z = _Player.transform.position.z;

            _RainEmitter.transform.position = rainEmitterPosition;
            _Rain.transform.position = rainPosition;
        }
    }

    private void FellInShadow()
    {
        _FallInShadowEvent?.Invoke();
        _BlockController.Reset();
        _CharaController.excludeLayers = ~0;
        _Moving = false;
        _PlayerActionMap.Disable();
        _DeathTrackingTarget.transform.position = _Player.transform.position;
        _PlayerCCamera.Target.TrackingTarget = _DeathTrackingTarget.transform;

        _RestartCoroutine = Restart();
        StartCoroutine(_RestartCoroutine);  
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);

        Vector3 restartPosition;
        if (_CurrentCheckpoint != null)
            restartPosition = _CurrentCheckpoint.GetComponent<Checkpoint>().RestartPosition();
        else
            restartPosition = _Spawn.transform.position;

        Vector3 shadowRestartPosition = new (0.0f, restartPosition.y + _ResetDistance, 0.0f);
        if (_ShadowInitialPosition.y > shadowRestartPosition.y)
            _ShadowOcean.transform.position = shadowRestartPosition;
        else
            _ShadowOcean.transform.position = restartPosition + new Vector3(0f, _ResetDistance, 0f);

        _CharaController.excludeLayers = 0;
        _CharaController.enabled = false;
        _Player.transform.position = restartPosition;
        _CharaController.enabled = true;
        _CharaController.detectCollisions = true;
        _Moving = true;
        _PlayerActionMap.Enable();
        PlayerCamera.ResetTarget();
        _RestartEvent?.Invoke();

        yield return null;
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
}

[Serializable] // Makes the class serializable for Inspector
public class Event
{
    public Event(string _name, UnityEvent _uevent)
    {
        name = _name;
        onEvent = _uevent;
    }
    public string name; // Optional label for clarity
    public UnityEvent onEvent; // UnityEvent visible in Inspector
}