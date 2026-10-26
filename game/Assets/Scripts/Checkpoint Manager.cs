using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public string _CheckpointTag = "Archway";
    List<GameObject> _CheckpointList = new();

    private GameObject _CurrentCheckpoint = null;

    void Start()
    {
        _CheckpointList = GameObject.FindGameObjectsWithTag(_CheckpointTag).ToList();
        foreach (GameObject archwayObject in _CheckpointList)
        {
            Debug.Log(archwayObject);
            archwayObject.GetComponent<Checkpoint>().OnActivate(CheckpointActivate);
        }
        _CurrentCheckpoint = _CheckpointList[0];
    }

    public Vector3 RestartPosition()
    {
        return _CurrentCheckpoint.GetComponent<Checkpoint>().RestartPosition();
    }

    public void CheckpointActivate(GameObject archwayObject)
    {
        _CurrentCheckpoint = archwayObject;
        Debug.Log("Checkpoint: " + _CheckpointList.IndexOf(_CurrentCheckpoint) + " Activated");
    }
}
