using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BlockController2 : MonoBehaviour
{
    public CinemachineCamera _CinemachineCamera;
    public GameObject _MissingBlockSprite;
    CharacterController _CharacterController;
    [Header("Bubbles")]
    public float _MissingBubbleDistance = 15f;
    public float _DistanceWarning = 17.5f;
    public float _DistanceLimit = 20f;
    public float _RetrievalDistance = 7.5f;

    [Header("Node")]
    public float _PlaceRange = 5f;
    public int _MaxNearbyNodes = 5;
    public LayerMask _BlockNodeLayer = 1 << 8;
    int _NearbyNodeTotal = 0;
    Collider[] _NearbyNodesList;

    [Header("Block")]
    public int _BlockLimit = 5;
    public float _BlockSize = 2f;
    public float _BlockRayScale = 1.25f;
    public Ease _BlockEasing = Ease.InOutSine;
    public float _TweenDuration = 0.25f;
    Tween _TweenObject;
    List<GameObject> _BlockList = new();
    GameObject _Block;

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
    [Header("States")]
    bool _Active = false;
    bool _IsMovingBlock = false;
    bool _SwapAxis = false;
    bool _PlaceHold = false;

    float _BlockHoldTimer = 0f;
    float _BlockHoldThreshold = 0.5f;

    Renderer _AxisPlaneRenderer;
    float _ArrowUIDistance;

    GameObject _GuideBlock;
    GameObject _BlockUI;
    GameObject _ArrowUI;
    GameObject _CrossUI;
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
    Transform _TrackingTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _CharacterController = GetComponent<CharacterController>();

        _ArrowUIDistance = _BlockSize + 1f;
        _NearbyNodesList = new Collider[_MaxNearbyNodes];
        _Camera = Camera.main;
        _PlayerMovement = GetComponent<PlayerMovement>();
        _PlayerCamera = GetComponentInChildren<PlayerCamera>();
        _PlayerTransform = transform;

        _TrackingTarget = new GameObject("TrackingTarget").transform;

        _GuideBlock = Instantiate(_GuideBlockPrefab);
        _GuideBlock.transform.localScale = new Vector3(_BlockSize, _BlockSize, _BlockSize);

        _BlockUI = Instantiate(_BlockUIPrefab);
        _ArrowUI = _BlockUI.transform.Find("Arrow Panel").gameObject;
        _CrossUI = _BlockUI.transform.Find("Cross").gameObject;
        _BlockUI.SetActive(false);
        _BlockUIUp = _BlockUI.transform.Find("Arrow Panel/Up").gameObject.transform as RectTransform;
        _BlockUIDown = _BlockUI.transform.Find("Arrow Panel/Down").gameObject.transform as RectTransform;
        _BlockUILeft = _BlockUI.transform.Find("Arrow Panel/Left").gameObject.transform as RectTransform;
        _BlockUIRight = _BlockUI.transform.Find("Arrow Panel/Right").gameObject.transform as RectTransform;

        _AxisPlane = Instantiate(_AxisPlanePrefab);
        _AxisPlane.SetActive(false);
        _AxisPlaneRenderer = _AxisPlane.GetComponent<Renderer>();
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
            _MissingBlockSprite.SetActive(false);
        }
        else if (_Active) {
            _BlockUI.SetActive(true);
            bool canControl = CanControl();
            _ArrowUI.SetActive(canControl);
            _CrossUI.SetActive(!canControl);
            _CrossUI.transform.position = _Camera.WorldToScreenPoint(_Block.transform.position);
            _TrackingTarget.position = Vector3.Lerp(transform.position, _Block.transform.position, 0.5f);
            _CinemachineCamera.Target.TrackingTarget = _TrackingTarget;
            UpdateControls(_Block.transform);
            MissingBlock();
            RetrieveVisual();
        }
        else
        {
            _GuideBlock.SetActive(false);
            _MissingBlockSprite.SetActive(false);
        }
    }

    private bool CanControl()
    {
        float playerSpeed = _CharacterController.velocity.magnitude;
        if (playerSpeed > 0.1f || !_CharacterController.isGrounded)
            return false;
        return true;
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
        if (context.started && !_Active && NearNodes())
        {   //place on node if block isnt active
            _Active = true;
            GameObject nearestNode = NearestNode();
            _AxisPlaneRenderer.material.SetVector("_Origin", new Vector3(
                    nearestNode.transform.position.x,
                    nearestNode.transform.position.y,
                    nearestNode.transform.position.z));
            _Block = Instantiate(_BlockPrefab);
            _Block.transform.position = nearestNode.transform.position;
            _Block.transform.eulerAngles = new Vector3(0f, nearestNode.transform.eulerAngles.y, 0f);
            _BlockList.Add(_Block);
            _Block.SetActive(true);
            _BlockUI.SetActive(true);
            _AxisPlane.SetActive(true);
            _GuideBlock.SetActive(false);
            _BlockEvent?.Invoke(0, _Block);
        
            //_CinemachineCamera.Target.TrackingTarget = _Block.transform;
            _PlayerCamera.SetCubeMode(_Block.transform.eulerAngles.y);
            //_PlayerMovement.SetControl(false);
        } //re-control
        /*
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
        */
    }

    public void OnMoveBlock(InputAction.CallbackContext context)
    {
        if (context.started && _Active && !_IsMovingBlock && CanControl())
        {
            _InputDir = context.ReadValue<Vector2>();
            Transform blockTransform = _Block.transform;

            Vector3 localX = _BlockSize * _InputDir.x * _InputModifierX;
            Vector3 localY = _BlockSize * _InputDir.y * _InputModifierY;
            Vector3 localXY = Vector3.zero;
            localXY = localX + localY;

            bool hit = Physics.BoxCast(
                blockTransform.position,
                blockTransform.localScale * 0.45f,
                blockTransform.TransformDirection(localXY),
                out RaycastHit hitInfo,
                blockTransform.rotation,
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
                _TweenObject = _Block.transform.DOMove(newBlockPlacement, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => _IsMovingBlock = false);
                _BlockList.Add(_Block);
                _Block.SetActive(true);
                //_CinemachineCamera.Target.TrackingTarget = _Block.transform;
            }
            else if (hit && hitInfo.transform.gameObject.CompareTag("Block"))
            {   //hit a block
                int index = _BlockList.IndexOf(hitInfo.transform.gameObject);
                if (index == _BlockList.Count - 2)
                {   //hit previous block -> undo
                    _IsMovingBlock = true;
                    _UndoEvent?.Invoke(_Block);
                    Vector3 previousPosition = _BlockList[^2].transform.position;
                    _TweenObject = _Block.transform.DOMove(previousPosition, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => {
                        _IsMovingBlock = false;
                        _BlockList.RemoveAt(_BlockList.Count - 1);
                        Destroy(_Block);
                        _Block = _BlockList[^1];
                        //_CinemachineCamera.Target.TrackingTarget = _Block.transform;
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
        
        if (context.started && _Active && !_IsMovingBlock)
        {
            
            
            if (_BlockList.Count > 1)
            {
                _UndoEvent?.Invoke(_Block);
                _IsMovingBlock = true;
                Vector3 previousPosition = _BlockList[^2].transform.position;
                _TweenObject = _Block.transform.DOMove(previousPosition, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => { 
                    _IsMovingBlock = false;
                    _BlockList.RemoveAt(_BlockList.Count - 1);
                    Destroy(_Block);
                    _Block = _BlockList[^1];
                    //_CinemachineCamera.Target.TrackingTarget = _Block.transform;
                });
            }
            else
            {
                _UndoEvent?.Invoke(_Block);
                _Active = false;
                _BlockUI.SetActive(false);
                _AxisPlane.SetActive(false);
                _PlayerMovement.SetControl(true);
                //_CinemachineCamera.Target.TrackingTarget = _PlayerTransform;
                _PlayerCamera.SetOctaMode();
                _BlockList.RemoveAt(0);
                Destroy(_Block);
                _Block = null;
                _CinemachineCamera.Target.TrackingTarget = transform;
            }
        }

        if (context.started)
            _PlaceHold = true;
        if (context.canceled)
        {   //Retrieve block after holding key
            if (_BlockHoldTimer > _BlockHoldThreshold && _BlockList.Count != 0)
            {
                RetrieveBlock();
            }

            _PlaceHold = false;
            _BlockHoldTimer = 0f;
        }
    }

    public void OnSwapAxisBlock(InputAction.CallbackContext context)
    {
        if (context.started)
            _SwapAxis = true;
        else if (context.canceled)
            _SwapAxis = false;
    }

    private void UpdateControls(Transform blockTransform)
    {
        Vector3 blockEulerRotation = blockTransform.eulerAngles;
        Vector3 cameraEulerRotation = _Camera.transform.eulerAngles;
        float angleDifference = cameraEulerRotation.y - blockEulerRotation.y;
        angleDifference = Mathf.Repeat(angleDifference, 360f);
        int relativeCardinalDirection = Mathf.RoundToInt(angleDifference / 90f);

        _AxisPlaneRenderer.material.SetFloat("_Rotation", _Block.transform.eulerAngles.y);

        switch (relativeCardinalDirection)
        {
            case 0: case 4:
                _InputModifierX = new(1f, 0f, 0f);
                Modify(new(0f, 0f, 1f), blockTransform, blockTransform.forward, blockTransform.right, 1, 1);
                /*
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = blockTransform.up;
                    _AxisPlane.transform.up = blockTransform.forward;
                    _AxisPlane.transform.position = _Block.transform.position + blockTransform.forward * (_BlockSize/2f - 0.01f);
                }
                else
                {
                    _InputModifierY = new(0f, 0f, 1f);
                    dir = blockTransform.forward;
                    _AxisPlane.transform.up = blockTransform.up;
                    _AxisPlane.transform.position = _Block.transform.position - new Vector3(0f, _BlockSize / 2f - 0.01f, 0f);
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(blockTransform.position + dir * _BlockUIDistance);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(blockTransform.position - dir * _BlockUIDistance);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(blockTransform.position - blockTransform.right * _BlockUIDistance);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(blockTransform.position + blockTransform.right * _BlockUIDistance);
                */
                break;
            case 1:
                _InputModifierX = new(0f, 0f, -1f);
                Modify(new(1f, 0f, 0f), blockTransform, blockTransform.right, blockTransform.forward, 1, -1);
                /*
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = blockTransform.up;
                    _AxisPlane.transform.up = blockTransform.right;
                    _AxisPlane.transform.position = _Block.transform.position + blockTransform.right * (_BlockSize / 2f - 0.01f);
                }
                else
                {
                    _InputModifierY = new(1f, 0f, 0f);
                    dir = blockTransform.right;
                    _AxisPlane.transform.up = blockTransform.up;
                    _AxisPlane.transform.position = _Block.transform.position - new Vector3(0f, _BlockSize / 2f - 0.01f, 0f);
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(blockTransform.position + dir * _BlockUIDistance);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(blockTransform.position - dir * _BlockUIDistance);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(blockTransform.position + blockTransform.forward * _BlockUIDistance);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(blockTransform.position - blockTransform.forward * _BlockUIDistance);
                */
                break;
            case 2:
                _InputModifierX = new(-1f, 0f, 0f);
                Modify(new(0f, 0f, -1f), blockTransform, blockTransform.forward, blockTransform.right, -1, -1);
                /*
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = blockTransform.up;
                    _AxisPlane.transform.up = blockTransform.forward;
                    _AxisPlane.transform.position = _Block.transform.position - blockTransform.forward * (_BlockSize / 2f - 0.01f);
                }
                else
                {
                    _InputModifierY = new(0f, 0f, -1f);
                    dir = -blockTransform.forward;
                    _AxisPlane.transform.up = blockTransform.up;
                    _AxisPlane.transform.position = _Block.transform.position - new Vector3(0f, _BlockSize / 2f - 0.01f, 0f);
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(blockTransform.position + dir * _BlockUIDistance);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(blockTransform.position - dir * _BlockUIDistance);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(blockTransform.position + blockTransform.right * _BlockUIDistance);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(blockTransform.position - blockTransform.right * _BlockUIDistance);
                */
                break;
            case 3:
                _InputModifierX = new(0f, 0f, 1f);
                Modify(new(-1f, 0f, 0f), blockTransform, blockTransform.right, blockTransform.forward, -1, 1);
                /*
                if (_SwapAxis)
                {
                    _InputModifierY = new(0f, 1f, 0f);
                    dir = blockTransform.up;
                    _AxisPlane.transform.up = blockTransform.right;
                    _AxisPlane.transform.position = _Block.transform.position - blockTransform.right * (_BlockSize / 2f - 0.01f);
                }
                else
                {
                    _InputModifierY = new(-1f, 0f, 0f);
                    dir = -blockTransform.right;
                    _AxisPlane.transform.up = blockTransform.up;
                    _AxisPlane.transform.position = _Block.transform.position - new Vector3(0f, _BlockSize / 2f - 0.01f, 0f);
                }
                _BlockUIUp.position = _Camera.WorldToScreenPoint(blockTransform.position + dir * _BlockUIDistance);
                _BlockUIDown.position = _Camera.WorldToScreenPoint(blockTransform.position - dir * _BlockUIDistance);
                _BlockUILeft.position = _Camera.WorldToScreenPoint(blockTransform.position - blockTransform.forward * _BlockUIDistance);
                _BlockUIRight.position = _Camera.WorldToScreenPoint(blockTransform.position + blockTransform.forward * _BlockUIDistance);
                */
                break;
            default:
                break;
        }

        Debug.DrawRay(blockTransform.position, blockTransform.right * 0.5f, Color.softRed);
        Debug.DrawRay(blockTransform.position, blockTransform.up * 0.5f, Color.softGreen);
        Debug.DrawRay(blockTransform.position, blockTransform.forward * 0.5f, Color.softBlue);
    }

    private void Modify(Vector3 inputModY, Transform transform, Vector3 zDir, Vector3 xUI, float sign, float sign2)
    {
        Vector3 vUI;
        if (_SwapAxis)
        {
            _InputModifierY = Vector3.up;
            vUI = transform.up;
            _AxisPlane.transform.up = zDir;
            _AxisPlane.transform.position = _Block.transform.position + (_BlockSize / 2f - 0.01f) * sign * zDir;
        }
        else
        {
            _InputModifierY = inputModY;
            vUI = sign * zDir;
            _AxisPlane.transform.up = transform.up;
            _AxisPlane.transform.position = _Block.transform.position - new Vector3(0f, _BlockSize / 2f - 0.01f, 0f);
        }
        _BlockUIUp.position = _Camera.WorldToScreenPoint(transform.position + vUI * _ArrowUIDistance);
        _BlockUIDown.position = _Camera.WorldToScreenPoint(transform.position - vUI * _ArrowUIDistance);
        _BlockUILeft.position = _Camera.WorldToScreenPoint(transform.position - _ArrowUIDistance * sign2 * xUI);
        _BlockUIRight.position = _Camera.WorldToScreenPoint(transform.position + _ArrowUIDistance * sign2 * xUI);
    }

    private void RetrieveVisual()
    {
        if (_PlaceHold)
        {
            if (_BlockHoldTimer > _BlockHoldThreshold)
            {
                foreach (GameObject block in _BlockList)
                {
                    block.GetComponent<Renderer>().material.color = Color.softRed;
                }
            }
            else if (_BlockHoldTimer > _BlockHoldThreshold / 2f)
            {
                foreach (GameObject block in _BlockList)
                {
                    block.GetComponent<Renderer>().material.color = Color.lightPink;
                }
            }
            _BlockHoldTimer += Time.deltaTime;
        }
        /*else
        {
            foreach (GameObject block in _BlockList)
            {
                block.GetComponent<Renderer>().material.color = Color.white;
            }
        }*/
    }

    private void MissingBlock()
    {
        float shortestDistance = 999f;
        foreach (GameObject block in _BlockList)
        {

            //distance to node
            float distance = Vector3.Distance(_PlayerTransform.position, block.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
            }
        }
        
        if (shortestDistance > _MissingBubbleDistance)
            _MissingBlockSprite.SetActive(true);
        else
            _MissingBlockSprite.SetActive(false);

        if (shortestDistance < _DistanceWarning)
        {
            foreach (GameObject block in _BlockList)
            {
                block.GetComponent<Renderer>().material.color = Color.white;
            }
        }
        else if (shortestDistance < _DistanceLimit)
        {
            foreach (GameObject block in _BlockList)
            {
                block.GetComponent<Renderer>().material.color = Color.pink;
            }
        }
        else
        {
            RetrieveBlock();
        }
    }

    private void RetrieveBlock()
    {
        _TweenObject.Complete();
        foreach (var block in _BlockList)
        {
            Destroy(block);
        }
        _BlockList.Clear();
        _Active = false;
        _BlockUI.SetActive(false);
        _AxisPlane.SetActive(false);
        _PlayerMovement.SetControl(true);
        _PlayerCamera.SetOctaMode();
        _Block = null;
        _CinemachineCamera.Target.TrackingTarget = transform;
    }
}