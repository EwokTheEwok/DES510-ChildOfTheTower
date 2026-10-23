using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArchwayManager : MonoBehaviour
{
    public string _ArchwayTag = "Archway";
    List<GameObject> _ArchwayList = new();

    private GameObject _CurrentArchway = null;

    void Start()
    {
        _ArchwayList = GameObject.FindGameObjectsWithTag(_ArchwayTag).ToList();
        foreach (GameObject archwayObject in _ArchwayList)
        {
            Debug.Log(archwayObject);
            archwayObject.GetComponent<Archway>().OnActivate(ArchwayActivate);
        }
        _CurrentArchway = _ArchwayList[0];
    }

    public Vector3 RestartPosition()
    {
        return _CurrentArchway.GetComponent<Archway>().RestartPosition();
    }

    public void ArchwayActivate(GameObject archwayObject)
    {
        _CurrentArchway = archwayObject;
        Debug.Log("Archway " + _ArchwayList.IndexOf(_CurrentArchway) + " Activated");
    }
}
