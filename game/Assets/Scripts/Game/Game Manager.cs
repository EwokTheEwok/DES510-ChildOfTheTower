using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
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
    public UnityEvent<float> _ShadowDistanceEvent = new ();


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
        if (_Moving)
        {
            float distanceFromMinimum = distanceFromPlayer - _MinDistance;
            float distanceScale = Mathf.Clamp(distanceFromMinimum / (_MaxDistance - _MinDistance), 0f, 1f);
            float speed = Mathf.SmoothStep(_MinSpeed, _MaxSpeed, distanceScale);
            _ShadowOcean.transform.position += new Vector3(0f, speed * Time.fixedDeltaTime, 0f);
        }
    }

    private void Restart()
    {
        CharacterController chara = _Player.GetComponent<CharacterController>();
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
        Debug.Log("Checkpoint: " + _CheckpointList.IndexOf(_CurrentCheckpoint) + " Activated");
    }
}
