using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockController : MonoBehaviour
{
    //Public
    public GameObject _Player;
    public int _BlockLimit = 1;
    public float _BlockSize = 2f;
    public GameObject _TestBlock;
    public string _BlockSurfaceTag = "Block Surface";
    public Vector3 up;

    //Private
    bool _CanPlace = false;
    //int _BlockSurfaceLayerIndex;
    List<GameObject> _BlockList = new List<GameObject>();
    List<GameObject> _SurfaceList = new List<GameObject>();
    GameObject _ClosestSurface;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject surface in GameObject.FindGameObjectsWithTag(_BlockSurfaceTag))
        {
            BoxCollider validPlacementCollider = surface.GetComponent<BoxCollider>();
            Vector3 LocalSize = validPlacementCollider.size;
            Vector3 surfaceScale = surface.transform.lossyScale;
            Vector3 LocalBlockSize = Vector3.Scale(new Vector3(_BlockSize, 0f, _BlockSize), new Vector3(1f / surfaceScale.x, 0f, 1f / surfaceScale.z));
            validPlacementCollider.size = new Vector3(LocalSize.x - LocalBlockSize.x, 0f, LocalSize.z - LocalBlockSize.z);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_CanPlace)
        {
            //Get closest surface
            float shortestDistance = 999f;
            foreach (GameObject surface in _SurfaceList)
            {
                float distance = Vector3.Distance(transform.position, surface.GetComponent<BoxCollider>().ClosestPoint(transform.position));
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    _ClosestSurface = surface;
                }
            }


            //Placement
            Vector3 placementPoint = _ClosestSurface.GetComponent<BoxCollider>().ClosestPoint(transform.position);
            _TestBlock.transform.position = Vector3.MoveTowards(placementPoint, placementPoint + _ClosestSurface.transform.up, _BlockSize * 0.5f);
            //up = _ClosestSurface.transform.up;
            _TestBlock.transform.up = _ClosestSurface.transform.up;
            _TestBlock.transform.forward = _ClosestSurface.transform.forward;
            _TestBlock.transform.right = _ClosestSurface.transform.right;
        } 
        else
        {
            _TestBlock.transform.position = Vector3.zero;
            _ClosestSurface = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_BlockSurfaceTag))
        {
            _SurfaceList.Add(other.gameObject);
            _CanPlace = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(_BlockSurfaceTag))
        {
            _SurfaceList.Remove(other.gameObject);
            if (_SurfaceList.Count == 0)
                _CanPlace = false;
        }
    }

    public void OnPlaceBlock(InputAction.CallbackContext context)
    {

    }
}
