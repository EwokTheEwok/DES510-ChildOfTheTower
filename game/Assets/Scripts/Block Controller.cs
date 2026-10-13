using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockController : MonoBehaviour
{
    //Public
    public float _PlaceRange = 5f;
    public int _MaxDetectableSurfaces = 5;
    public int _BlockLimit = 1;
    public float _BlockSize = 2f;
    public Ease _BlockEasing = Ease.InOutSine;
    public float _TweenDuration = 0.25f;
    public string _BlockSurfaceTag = "Block Surface";
    public LayerMask _BlockSurfaceLayer;
    public GameObject _GuideBlockPrefab;
    public GameObject _BlockUIPrefab;
    public GameObject _BlockPrefab;
    public Camera _Camera;
    public CinemachineCamera _CinemachineCamera;
    public Player _Player;

    //Private
    bool _IsPlacing = false;
    bool _ActiveSurface = false;
    bool _MovingBlock = false;
    public float _BlockRayScale = 1.25f;
    readonly List<GameObject> _BlockList = new();
    EntityId _ActiveSurfaceID;
    Collider[] _SurfaceColList;
    int _SurfaceTotal;
    GameObject _ClosestSurface;
    GameObject _GuideBlock;
    GameObject _Block;
    GameObject _BlockUI;
    RectTransform _BlockUIUp;
    RectTransform _BlockUIDown;
    RectTransform _BlockUILeft;
    RectTransform _BlockUIRight;
    Vector2 _InputDir;
    Vector3 _InputModifier;
    
    public 

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
        if (NearSurfaces())
        {
            //Get closest surface
            float shortestDistance = 999f;
            foreach (var a in _SurfaceColList)
            {
                var col = a as Collider;
                if (col == null)
                    continue;

                GameObject surface = col.gameObject;
                float distance = Vector3.Distance(_Player.transform.position, surface.GetComponent<BoxCollider>().ClosestPoint(_Player.transform.position));
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    _ClosestSurface = surface;
                }
            }
        } 
        else
        {
            _GuideBlock.transform.position = Vector3.zero;
            _ClosestSurface = null;
        }

        if (IsNewSurface())
        {
            _GuideBlock.SetActive(true);
            //Placement
            Vector3 placementPoint = _ClosestSurface.GetComponent<BoxCollider>().ClosestPoint(_Player.transform.position);
            Vector3 blockPosition = Vector3.MoveTowards(placementPoint, placementPoint + _ClosestSurface.transform.up, _BlockSize * 0.5f);
            _GuideBlock.transform.position = blockPosition;
            _GuideBlock.transform.eulerAngles = new Vector3(0f, _ClosestSurface.transform.rotation.eulerAngles.y, 0f); ;
            HandleRotationAndDisplayInput(_GuideBlock.transform);
        } 
        else if (_IsPlacing)
        {
            _BlockUI.SetActive(true);
            HandleRotationAndDisplayInput(_Block.transform);
        }
        else
        {
            _BlockUI.SetActive(false);
        }
    }

    public void OnPlaceBlock(InputAction.CallbackContext context)
    {
        //place on new surface & remove old placement
        if (context.started && IsNewSurface())
        {
            if (_BlockList.Count != 0)
            {
                foreach (var block in _BlockList)
                {
                    Destroy(block);
                }
                _BlockList.Clear();
            }

            _ActiveSurface = true;
            _IsPlacing = true;
            _ActiveSurfaceID = _ClosestSurface.GetEntityId();
            _Player._CanControl = false;
            _Block = Instantiate(_BlockPrefab, _GuideBlock.transform.position, _GuideBlock.transform.rotation);
            _BlockList.Add(_Block);

            _CinemachineCamera.Target.TrackingTarget = _Block.transform;

            _Block.SetActive(true);
            _GuideBlock.SetActive(false);
        } //re-control active surface
        else if (context.started && _ActiveSurface && !_IsPlacing)
        {
            _CinemachineCamera.Target.TrackingTarget = _Block.transform;

            _IsPlacing = true;
            _Player._CanControl = false;
        } //control player
        else if (context.started && _IsPlacing)
        {
            _CinemachineCamera.Target.TrackingTarget = _Player.transform;

            _BlockUI.SetActive(false);
            _IsPlacing = false;
            _Player._CanControl = true;
        }
    }

    public void OnBlockMove(InputAction.CallbackContext context)
    {
        if (context.started && _IsPlacing && _BlockList.Count < _BlockLimit && !_MovingBlock)
        {
            _InputDir = context.ReadValue<Vector2>();
            Transform blockTransform = _Block.transform;

            Vector3 localDirection = Vector3.zero;
            localDirection.x = _InputDir.x * _InputModifier.x * _BlockSize;
            localDirection.z = _InputDir.x * _InputModifier.z * _BlockSize;
            localDirection.y = _InputDir.y * _BlockSize;

            bool hit = Physics.Raycast(blockTransform.position, blockTransform.TransformDirection(localDirection), _BlockSize * _BlockRayScale);
            Debug.DrawRay(blockTransform.position, blockTransform.TransformDirection(localDirection) * _BlockRayScale, Color.white, 5);

            if (!hit)
            {
                _MovingBlock = true;
                Vector3 newBlockPlacement = blockTransform.position + blockTransform.TransformDirection(localDirection);
                _Block = Instantiate(_BlockPrefab, _Block.transform.position, blockTransform.rotation);
                _Block.transform.DOMove(newBlockPlacement, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => _MovingBlock = false);
                _BlockList.Add(_Block);
                _Block.SetActive(true);
                _CinemachineCamera.Target.TrackingTarget = _Block.transform;
            }
        }
    }

    public void OnUndoBlock(InputAction.CallbackContext context)
    {
        if (context.started && _IsPlacing && !_MovingBlock)
        {
            if (_BlockList.Count > 1)
            {
                _MovingBlock = true;
                Vector3 previousPosition = _BlockList[^2].transform.position;
                _Block.transform.DOMove(previousPosition, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => { 
                    _MovingBlock = false;
                    _BlockList.RemoveAt(_BlockList.Count - 1);
                    Destroy(_Block);
                    _Block = _BlockList[^1];
                    _CinemachineCamera.Target.TrackingTarget = _Block.transform;
                });
            }
            else
            {
                _BlockList.RemoveAt(0);
                Destroy(_Block);
                _IsPlacing = false;
                _ActiveSurface = false;
                _Player._CanControl = true;
                _CinemachineCamera.Target.TrackingTarget = _Player.transform;
            }
        }
    }

    private bool IsNewSurface()
    {
        if (_ClosestSurface == null)
            return false;
        else if (_ActiveSurface == false)
            return true;
        else if (_ActiveSurface == true && _ClosestSurface.GetEntityId().CompareTo(_ActiveSurfaceID) != 0)
            return true;
        return false;
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

    private bool NearSurfaces()
    {
        _SurfaceTotal = Physics.OverlapSphereNonAlloc(_Player.transform.position, _PlaceRange, _SurfaceColList, _BlockSurfaceLayer);
        if (_SurfaceTotal > 0)
            return true;
        return false;
    }

    private void HandleRotationAndDisplayInput(Transform _transform)
    {
        Vector3 blockEulerRotation = _transform.eulerAngles;
        Vector3 cameraEulerRotation = _Camera.transform.eulerAngles;
        float angleDifference = cameraEulerRotation.y - blockEulerRotation.y;
        angleDifference = Mathf.Repeat(angleDifference, 360f);
        int segment = Mathf.RoundToInt(angleDifference / 45f);

        float blockUIDist = _BlockSize + 1f;
        _BlockUIUp.position = _Camera.WorldToScreenPoint(_transform.position + _transform.up * blockUIDist);
        _BlockUIDown.position = _Camera.WorldToScreenPoint(_transform.position - _transform.up * blockUIDist);

        switch (segment)
        {
            case 0: case 8: case 1:
                _InputModifier = new (1f, 0f, 0f);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position + _transform.right * blockUIDist);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position - _transform.right * blockUIDist);
                break;
            case 2: case 3:
                _InputModifier = new(0f, 0f, -1f);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position + _transform.forward * blockUIDist);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position - _transform.forward * blockUIDist);
                break;
            case 4: case 5:
                _InputModifier = new(-1f, 0f, 0f);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position + _transform.right * blockUIDist);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position - _transform.right * blockUIDist);
                break;
            case 6: case 7:
                _InputModifier = new(0f, 0f, 1f);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position + _transform.forward * blockUIDist);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position - _transform.forward * blockUIDist);
                break;
            default:
                break;
        }
        
        Debug.DrawRay(_transform.position, _transform.right * 5.0f, Color.softRed);
        Debug.DrawRay(_transform.position, _transform.up * 5.0f, Color.softGreen);
        Debug.DrawRay(_transform.position, _transform.forward * 5.0f, Color.softBlue);
    }
}
