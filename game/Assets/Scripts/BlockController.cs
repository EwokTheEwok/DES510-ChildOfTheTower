using UnityEngine;
using UnityEngine.InputSystem;

public class BlockController : MonoBehaviour
{
    //Public
    public float _PlaceRange = 5f;
    public int _MaxDetectableSurfaces = 5;
    public int _BlockLimit = 1;
    public float _BlockSize = 2f;
    public string _BlockSurfaceTag = "Block Surface";
    public LayerMask _BlockSurfaceLayer;
    public Camera _PlayerCamera;
    public GameObject _GuideBlockPrefab;
    public GameObject _BlockUIPrefab;

    //Private
    //List<GameObject> _BlockList;
    Collider[] _SurfaceColList;
    int _SurfaceTotal;
    GameObject _ClosestSurface;

    GameObject _GuideBlock;
    GameObject _BlockUI;
    RectTransform _BlockUIUp;
    RectTransform _BlockUIDown;
    RectTransform _BlockUILeft;
    RectTransform _BlockUIRight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpSurfaces();
        _SurfaceColList = new Collider[_MaxDetectableSurfaces];
        _GuideBlock = Instantiate(_GuideBlockPrefab);
        _GuideBlock.transform.localScale = new Vector3(_BlockSize, _BlockSize, _BlockSize);

        _BlockUI = Instantiate(_BlockUIPrefab);
        _BlockUI.SetActive(true);
        _BlockUIUp = _BlockUI.transform.Find("Up").gameObject.transform as RectTransform;
        _BlockUIDown = _BlockUI.transform.Find("Down").gameObject.transform as RectTransform;
        _BlockUILeft = _BlockUI.transform.Find("Left").gameObject.transform as RectTransform;
        _BlockUIRight = _BlockUI.transform.Find("Right").gameObject.transform as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanPlace())
        {
            if (_BlockUI != null)
                _BlockUI.SetActive(true);

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
            float placementDist = _BlockSize * 0.5f;
            _GuideBlock.transform.eulerAngles = new Vector3(0f, _ClosestSurface.transform.rotation.eulerAngles.y, 0f);
            _GuideBlock.transform.position = Vector3.MoveTowards(placementPoint, placementPoint + _ClosestSurface.transform.up, placementDist);
        } 
        else
        {
            _GuideBlock.transform.position = Vector3.zero;
            _ClosestSurface = null;
            _BlockUI.SetActive(false);
        }

        if (_ClosestSurface != null)
        {
            InputManipulation();
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

    private void InputManipulation()
    {
        Vector3 cameraLook = _PlayerCamera.transform.forward;
        Vector3 horizontalLook = new Vector3(cameraLook.x, 0f, cameraLook.z);
        horizontalLook.Normalize();

        Vector3 blockEulerRotation = _GuideBlock.transform.eulerAngles;
        Vector3 cameraEulerRotation = _PlayerCamera.transform.eulerAngles;
        float angleDifference = cameraEulerRotation.y - blockEulerRotation.y;
        angleDifference = Mathf.Repeat(angleDifference, 360f);
        int segment = Mathf.RoundToInt(angleDifference / 45f);

        float blockUIDist = _BlockSize + 1f;
        _BlockUIUp.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position + _GuideBlock.transform.up * blockUIDist);
        _BlockUIDown.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position - _GuideBlock.transform.up * blockUIDist);

        switch (segment)
        {
            case 0: case 8: case 1:
                _BlockUILeft.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position + _GuideBlock.transform.right * blockUIDist);
                _BlockUIRight.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position - _GuideBlock.transform.right * blockUIDist);
                break;
            case 2: case 3:
                _BlockUIRight.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position + _GuideBlock.transform.forward * blockUIDist);
                _BlockUILeft.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position - _GuideBlock.transform.forward * blockUIDist);
                break;
            case 4: case 5:
                _BlockUIRight.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position + _GuideBlock.transform.right * blockUIDist);
                _BlockUILeft.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position - _GuideBlock.transform.right * blockUIDist);
                break;
            case 6: case 7:
                _BlockUILeft.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position + _GuideBlock.transform.forward * blockUIDist);
                _BlockUIRight.position = _PlayerCamera.WorldToScreenPoint(_GuideBlock.transform.position - _GuideBlock.transform.forward * blockUIDist);
                break;
            default:
                break;
        }
        
        Debug.DrawRay(_GuideBlock.transform.position, _GuideBlock.transform.right * 5.0f, Color.softRed);
        Debug.DrawRay(_GuideBlock.transform.position, _GuideBlock.transform.up * 5.0f, Color.softGreen);
        Debug.DrawRay(_GuideBlock.transform.position, _GuideBlock.transform.forward * 5.0f, Color.softBlue);
    }
}
