using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BlockController2 : MonoBehaviour
{
    public CinemachineCamera _CinemachineCamera;

    [Header("Node")]
    public float _PlaceRange = 5f;
    public int _MaxNearbyNodes = 5;
    public LayerMask _BlockNodeLayer = 1 << 8;
    int _NearbyNodeTotal = 0;

    private Collider[] _NearbyNodesList;

    [Header("Block")]
    public int _BlockLimit = 5;
    public float _BlockSize = 2f;
    public float _BlockRayScale = 1.25f;
    public Ease _BlockEasing = Ease.InOutSine;
    public float _TweenDuration = 0.25f;
    List<GameObject> _BlockList = new();

    [Header("Prefabs")]
    public GameObject _GuideBlockPrefab;
    public GameObject _BlockUIPrefab;
    public GameObject _BlockPrefab;
    public GameObject _AxisPlanePrefab;


    [Header("Events")]
    public UnityEvent<int, GameObject> _BlockEvent = new();
    public UnityEvent<GameObject> _UndoEvent = new();
    public UnityEvent<GameObject> _LimitEvent = new();

    //Private
    bool _Active = false;
    bool _Control = false;
    bool _IsMovingBlock = false;
    bool _SwapAxis = false;

    bool _PlaceHold = false;
    float _PlaceHoldTimer = 0f;


    GameObject _GuideBlock;
    GameObject _Block;
    GameObject _BlockUI;
    GameObject _AxisPlane;
    RectTransform _BlockUIUp;
    RectTransform _BlockUIDown;
    RectTransform _BlockUILeft;
    RectTransform _BlockUIRight;
    Vector2 _InputDir;
    Vector3 _InputModifierX;
    Vector3 _InputModifierY;

    //Player
    Transform _PlayerTransform;
    PlayerMovement _PlayerMovement;

    //Camera
    Camera _Camera;
    PlayerCamera _PlayerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _NearbyNodesList = new Collider[_MaxNearbyNodes];
        _Camera = Camera.main;
        _PlayerMovement = GetComponent<PlayerMovement>();
        _PlayerCamera = GetComponentInChildren<PlayerCamera>();
        _PlayerTransform = transform;

        _GuideBlock = Instantiate(_GuideBlockPrefab);
        _GuideBlock.transform.localScale = new Vector3(_BlockSize, _BlockSize, _BlockSize);

        _BlockUI = Instantiate(_BlockUIPrefab);
        _BlockUI.SetActive(false);
        _BlockUIUp = _BlockUI.transform.Find("Up").gameObject.transform as RectTransform;
        _BlockUIDown = _BlockUI.transform.Find("Down").gameObject.transform as RectTransform;
        _BlockUILeft = _BlockUI.transform.Find("Left").gameObject.transform as RectTransform;
        _BlockUIRight = _BlockUI.transform.Find("Right").gameObject.transform as RectTransform;

        _AxisPlane = Instantiate(_AxisPlanePrefab);
        _AxisPlane.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (NearNodes() && !_Active)
        {
            GameObject nearestNode = NearestNode();
            _GuideBlock.SetActive(true);
            _GuideBlock.transform.position = nearestNode.transform.position;
            _GuideBlock.transform.eulerAngles = new Vector3(0f, nearestNode.transform.rotation.eulerAngles.y, 0f);
        }
        else if (_Active) {
            if (_PlaceHold)
            {
                if (_PlaceHoldTimer > 1f)
                {
                    foreach (GameObject block in _BlockList)
                    {
                        block.GetComponent<Renderer>().material.color = Color.softRed;
                    }
                } else if (_PlaceHoldTimer > 0.25f)
                {
                    foreach (GameObject block in _BlockList)
                    {
                        block.GetComponent<Renderer>().material.color = Color.lightPink;
                    }
                }
                _PlaceHoldTimer += Time.deltaTime;
            } else
            {
                foreach (GameObject block in _BlockList)
                {
                    block.GetComponent<Renderer>().material.color = Color.white;
                }
            }
            UpdateDirections(_Block.transform);
        }
        else
        {
            _GuideBlock.SetActive(false);
        }
    }

    private bool NearNodes()
    {
        _NearbyNodeTotal = Physics.OverlapSphereNonAlloc(_PlayerTransform.position, _PlaceRange, _NearbyNodesList, _BlockNodeLayer);
        if (_NearbyNodeTotal > 0)
            return true;
        return false;
    }

    private GameObject NearestNode()
    {
        float shortestDistance = 999f;
        GameObject nearestNode = null;
        foreach (var a in _NearbyNodesList)
        {
            var col = a as Collider;
            if (col == null)
                continue;

            //distance to node
            GameObject node = col.gameObject;
            float distance = Vector3.Distance(_PlayerTransform.position, node.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestNode = node;
            }
        }
        return nearestNode;
    }

    public bool IsBlockActive()
    {
        if (_BlockList.Count > 0)
        {
            return true;
        }
        return false;
    }

    public void OnPlaceBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _PlaceHold = true;
        }
        else if (context.canceled)
        {
            if (_PlaceHoldTimer > 1f && _BlockList.Count != 0)
            {
                foreach (var block in _BlockList)
                {
                    Destroy(block);
                }
                _BlockList.Clear();
                _Active = false;
                _Control = false;
                _BlockUI.SetActive(false);
                _AxisPlane.SetActive(false);
                _PlayerMovement.SetControl(true);
                _CinemachineCamera.Target.TrackingTarget = _PlayerTransform;
                _PlayerCamera.SetOctaMode();
                _Block = null;
            }
            else if (!_Active && NearNodes())
            {   //place on node if block isnt active
                _Active = true;
                _Control = true;
                GameObject nearestNode = NearestNode();
                _Block = Instantiate(_BlockPrefab);
                _Block.transform.position = nearestNode.transform.position;
                _Block.transform.eulerAngles = new Vector3(0f, nearestNode.transform.eulerAngles.y, 0f);
                _BlockList.Add(_Block);
                _Block.SetActive(true);
                _BlockUI.SetActive(true);
                _AxisPlane.SetActive(true);
                _GuideBlock.SetActive(false);
                _BlockEvent?.Invoke(0, _Block);
                _CinemachineCamera.Target.TrackingTarget = _Block.transform;
                _PlayerCamera.SetCubeMode(_Block.transform.eulerAngles.y);
                _PlayerMovement.SetControl(false);
            } //re-control
            else if (_Active && !_Control)
            {
                _Control = true;
                _CinemachineCamera.Target.TrackingTarget = _Block.transform;
                _AxisPlane.SetActive(true);
                _BlockUI.SetActive(true);
                _PlayerMovement.SetControl(false);
                _PlayerCamera.SetCubeMode(_Block.transform.eulerAngles.y);
            } //control player
            else if (_Control)
            {
                _Control = false;
                _CinemachineCamera.Target.TrackingTarget = _PlayerTransform.transform;
                _AxisPlane.SetActive(false);
                _BlockUI.SetActive(false);
                _PlayerMovement.SetControl(true);
                _PlayerCamera.SetOctaMode();
            }

            _PlaceHold = false;
            _PlaceHoldTimer = 0f;
        }
    }

    public void OnMoveBlock(InputAction.CallbackContext context)
    {
        if (context.started && _Control && !_IsMovingBlock)
        {
            _InputDir = context.ReadValue<Vector2>();
            Transform blockTransform = _Block.transform;

            Vector3 localX = _BlockSize * _InputDir.x * _InputModifierX;
            Vector3 localY = _BlockSize * _InputDir.y * _InputModifierY;
            Vector3 localXY = Vector3.zero;
            localXY = localX + localY;

            bool hit = Physics.Raycast(
                blockTransform.position,
                blockTransform.TransformDirection(localXY),
                out RaycastHit hitInfo,
                _BlockSize * _BlockRayScale,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            Debug.DrawRay(blockTransform.position, blockTransform.TransformDirection(localXY) * _BlockRayScale, Color.white, 5);

            if (!hit && _BlockList.Count < _BlockLimit)
            {   //can move block
                _IsMovingBlock = true;
                _BlockEvent?.Invoke(_BlockList.Count, _Block);
                Vector3 newBlockPlacement = blockTransform.position + blockTransform.TransformDirection(localXY);
                _Block = Instantiate(_BlockPrefab, _Block.transform.position, blockTransform.rotation);
                _Block.transform.DOMove(newBlockPlacement, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => _IsMovingBlock = false);
                _BlockList.Add(_Block);
                _Block.SetActive(true);
                _CinemachineCamera.Target.TrackingTarget = _Block.transform;
            }
            else if (hit && hitInfo.transform.gameObject.CompareTag("Block"))
            {   //hit a block
                int index = _BlockList.IndexOf(hitInfo.transform.gameObject);
                if (index == _BlockList.Count - 2)
                {   //hit previous block -> undo
                    _IsMovingBlock = true;
                    _UndoEvent?.Invoke(_Block);
                    Vector3 previousPosition = _BlockList[^2].transform.position;
                    _Block.transform.DOMove(previousPosition, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => {
                        _IsMovingBlock = false;
                        _BlockList.RemoveAt(_BlockList.Count - 1);
                        Destroy(_Block);
                        _Block = _BlockList[^1];
                        _CinemachineCamera.Target.TrackingTarget = _Block.transform;
                    });
                }
                else
                {   //hit non-block
                    _LimitEvent?.Invoke(_Block);
                }
            }
            else
            {   //at block limit or hit non-block
                _LimitEvent?.Invoke(_Block);
            }
        }
    }

    public void OnUndoBlock(InputAction.CallbackContext context)
    {
        if (context.started && _Control && !_IsMovingBlock)
        {
            _UndoEvent?.Invoke(_Block);
            if (_BlockList.Count > 1)
            {
                _IsMovingBlock = true;
                Vector3 previousPosition = _BlockList[^2].transform.position;
                _Block.transform.DOMove(previousPosition, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => { 
                    _IsMovingBlock = false;
                    _BlockList.RemoveAt(_BlockList.Count - 1);
                    Destroy(_Block);
                    _Block = _BlockList[^1];
                    _CinemachineCamera.Target.TrackingTarget = _Block.transform;
                });
            }
            else
            {
                _Active = false;
                _Control = false;
                _BlockUI.SetActive(false);
                _AxisPlane.SetActive(false);
                _PlayerMovement.SetControl(true);
                _CinemachineCamera.Target.TrackingTarget = _PlayerTransform;
                _PlayerCamera.SetOctaMode();
                _BlockList.RemoveAt(0);
                Destroy(_Block);
                _Block = null;
            }
        }
    }

    public void OnSwapAxisBlock(InputAction.CallbackContext context)
    {
        if (context.started)
            _SwapAxis = true;
        else if (context.canceled)
            _SwapAxis = false;
    }

    private void UpdateDirections(Transform _transform)
    {
        Vector3 blockEulerRotation = _transform.eulerAngles;
        Vector3 cameraEulerRotation = _Camera.transform.eulerAngles;
        float angleDifference = cameraEulerRotation.y - blockEulerRotation.y;
        angleDifference = Mathf.Repeat(angleDifference, 360f);
        int relativeCardinalDirection = Mathf.RoundToInt(angleDifference / 90f);

        float blockUIDist = _BlockSize + 1f;
        Vector3 dir;

        _AxisPlane.transform.position = _Block.transform.position;

        switch (relativeCardinalDirection)
        {
            case 0: case 4:
                _InputModifierX = new(1f, 0f, 0f);
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = _transform.up;
                    _AxisPlane.transform.up = _transform.forward;
                }
                else
                {
                    _InputModifierY = new(0f, 0f, 1f);
                    dir = _transform.forward;
                    _AxisPlane.transform.up = _transform.up;

                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(_transform.position + dir * blockUIDist);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(_transform.position - dir * blockUIDist);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position - _transform.right * blockUIDist);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position + _transform.right * blockUIDist);

                break;
            case 1:
                _InputModifierX = new(0f, 0f, -1f);
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = _transform.up;
                    _AxisPlane.transform.up = _transform.right;
                }
                else
                {
                    _InputModifierY = new(1f, 0f, 0f);
                    dir = _transform.right;
                    _AxisPlane.transform.up = _transform.up;
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(_transform.position + dir * blockUIDist);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(_transform.position - dir * blockUIDist);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position - _transform.forward * blockUIDist);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position + _transform.forward * blockUIDist);

                break;
            case 2:
                _InputModifierX = new(-1f, 0f, 0f);
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = _transform.up;
                    _AxisPlane.transform.up = _transform.forward;
                }
                else
                {
                    _InputModifierY = new(0f, 0f, -1f);
                    dir = -_transform.forward;
                    _AxisPlane.transform.up = _transform.up;
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(_transform.position + dir * blockUIDist);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(_transform.position - dir * blockUIDist);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position - _transform.right * blockUIDist);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position + _transform.right * blockUIDist);

                break;
            case 3:
                _InputModifierX = new(0f, 0f, 1f);
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = _transform.up;
                    _AxisPlane.transform.up = _transform.right;
                }
                else
                {
                    _InputModifierY = new(-1f, 0f, 0f);
                    dir = -_transform.right;
                    _AxisPlane.transform.up = _transform.up;
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(_transform.position + dir * blockUIDist);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(_transform.position - dir * blockUIDist);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(_transform.position - _transform.forward * blockUIDist);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(_transform.position + _transform.forward * blockUIDist);
                break;
            default:
                break;
        }

        Debug.DrawRay(_transform.position, _transform.right * 5.0f, Color.softRed);
        Debug.DrawRay(_transform.position, _transform.up * 5.0f, Color.softGreen);
        Debug.DrawRay(_transform.position, _transform.forward * 5.0f, Color.softBlue);
    }
}