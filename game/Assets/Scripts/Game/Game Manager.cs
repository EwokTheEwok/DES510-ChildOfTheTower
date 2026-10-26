using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Checkpoint settings")]
    public string _CheckpointTag = "Checkpoint";
    private List<GameObject> _CheckpointList = new();
    private GameObject _CurrentCheckpoint = null;

    [Header("Shadow Ocean settings")]
    public float _MinSpeed = 0.01f;
    public float _MaxSpeed = 0.05f;
    public float _MinDistance = 5f;
    public float _MaxDistance = 30f;
    public float _ResetDistance = -20f;
    public float _StartHeight = -10f;
    private bool _Moving = false;

    [Header("References")]
    public GameObject _Player = null;
    public GameObject _ShadowOcean = null;


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
        if (_Moving)
        {
            float distanceFromMinimum = (_Player.transform.position.y - _MinDistance) - _ShadowOcean.transform.position.y;
            float distanceScale = Mathf.Clamp(distanceFromMinimum / (_MaxDistance - _MinDistance), 0f, 1f);
            float speed = Mathf.SmoothStep(_MinSpeed, _MaxSpeed, distanceScale);
            _ShadowOcean.transform.position += new Vector3(0f, speed, 0f);
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
