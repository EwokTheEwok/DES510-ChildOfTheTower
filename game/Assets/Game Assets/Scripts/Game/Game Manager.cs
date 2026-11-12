using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    [Header("Spawn")]
    public GameObject _Spawn;

    [Header("Shadow Ocean")]
    public GameObject _ShadowOcean = null;
    public float _MinSpeed = 0.5f;
    public float _MaxSpeed = 10f;
    public float _MinDistance = 5f;
    public float _MaxDistance = 30f;
    public float _ResetDistance = -30f;
    private bool _Moving = false;

    GameObject _Player = null;
    GameObject _DeathTrackingTarget;
    CharacterController _CharaController;
    BlockController2 _BlockController;
    InputActionMap _PlayerActionMap;
    CinemachineCamera _PlayerCCamera;

    [Header("Events")]
    public UnityEvent<float> _ShadowDistanceEvent = new();
    public UnityEvent<GameObject> _CheckpointEvent = new();
    public UnityEvent _FallInShadowEvent = new();
    public UnityEvent _RestartEvent = new();
    public UnityEvent<bool> _PauseEvent = new();

    IEnumerator _RestartCoroutine;

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

        _ShadowOcean.GetComponent<ShadowOcean>().OnActivate(FellInShadow);
        _ShadowOcean.transform.position = new(0f, _ShadowOcean.transform.position.y, 0f);

        _Player = GameObject.FindGameObjectWithTag("Player");
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

        _ShadowOcean.transform.position = restartPosition + new Vector3(0f, _ResetDistance, 0f);
        _CharaController.excludeLayers = 0;
        _CharaController.enabled = false;
        _Player.transform.position = restartPosition;
        _CharaController.enabled = true;
        _CharaController.detectCollisions = true;
        _Moving = true;
        _PlayerActionMap.Enable();
        _BlockController.ResetCamera();
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
