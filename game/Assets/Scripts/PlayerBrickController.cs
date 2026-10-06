using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrickController : MonoBehaviour
{
    //Public
    public int _BlockLimit = 1;
    public float _BlockPlaceRange = 5f;
    public LayerMask _BlockSurfaceLayer;
    public GameObject testSphere;

    //Private
    int _BlockSurfaceLayerIndex;
    float _BlockSize = 1.0f;
    List<GameObject> _BlockList;
    [ReadOnly, SerializeField] List<GameObject> _BlockSurfaceList;
    [ReadOnly, SerializeField] GameObject _ClosestSurface;
    bool _CanPlace = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _BlockSurfaceLayerIndex = (int)Mathf.Log(_BlockSurfaceLayer.value, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (_CanPlace)
        {
            //Get closest surface
            //Could be optimised
            float shortestDistance = 999f;
            foreach (GameObject surface in _BlockSurfaceList)
            {
                float distance = Vector3.Distance(transform.position, surface.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    _ClosestSurface = surface;
                }
            }

            //Placement
            Vector3 placementPoint = _ClosestSurface.GetComponent<BoxCollider>().ClosestPoint(transform.position);
            testSphere.transform.position = placementPoint;
        } 
        else
        {
            testSphere.transform.position = Vector3.zero;
            _ClosestSurface = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _BlockSurfaceLayerIndex)
        {
            _BlockSurfaceList.Add(other.gameObject);
            _CanPlace = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _BlockSurfaceLayerIndex)
        {
            _BlockSurfaceList.Remove(other.gameObject);
            if (_BlockSurfaceList.Count == 0)
                _CanPlace = false;
        }
    }

    public void OnPlaceBlock(InputAction.CallbackContext context)
    {

    }
}
