using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockController : MonoBehaviour
{
    //Public
    public float _PlaceRange = 5f;
    public int _BlockLimit = 1;
    public float _BlockSize = 2f;
    public string _BlockSurfaceTag = "Block Surface";
    public LayerMask _BlockSurfaceLayer;
    public int _MaxDetectableSurfaces = 5;
    public GameObject _TestBlock;

    //Private
    List<GameObject> _BlockList = new List<GameObject>();
    Collider[] _SurfaceColList;
    int _SurfaceTotal;
    GameObject _ClosestSurface;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpSurfaces();
        _SurfaceColList = new Collider[_MaxDetectableSurfaces];
    }

    // Update is called once per frame
    void Update()
    {
        if (CanPlace())
        {
            //Get closest surface
            float shortestDistance = 999f;
            foreach (var a in _SurfaceColList)
            {
                var col = a as Collider;
                if (col == null)
                    continue;

                GameObject surface = col.gameObject;
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
            
            _TestBlock.transform.rotation = _ClosestSurface.transform.rotation;
        } 
        else
        {
            _TestBlock.transform.position = Vector3.zero;
            _ClosestSurface = null;
        }
    }

    public void OnPlaceBlock(InputAction.CallbackContext context)
    {

    }

    private void SetUpSurfaces()
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

    private bool CanPlace()
    {
        _SurfaceTotal = Physics.OverlapSphereNonAlloc(transform.position, _PlaceRange, _SurfaceColList, _BlockSurfaceLayer);
        if (_SurfaceTotal > 0)
            return true;
        return false;
    }
}
