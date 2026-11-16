using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class BlockController2 : MonoBehaviour
{
    public CinemachineCamera _CinemachineCamera;
    public GameObject _MissingBlockSprite;
    CharacterController _CharacterController;

    [Header("Bubbles")]
    public float _MissingBubbleDistance = 15f;
    bool _MissingBubbleActive = false;

    [Header("Node")]
    public float _PlaceRange = 5f;
    public int _MaxNearbyNodes = 10;
    public LayerMask _BlockNodeLayer = 1 << 8;
    int _NearbyNodeTotal = 0;
    Collider[] _NearbyNodesList;

    [Header("Block")]
    public int _BlockLimit = 5;
    public float _BlockSize = 2f;
    public float _BlockRayScale = 0.95f;
    public Ease _BlockEasing = Ease.InOutSine;
    public float _TweenDuration = 0.25f;
    Tween _TweenObject;
    readonly List<GameObject> _BlockList = new();
    GameObject _Block;
    GameObject _GuideBlock;
    public Material _BlockMaterial;
    public Material _BlockSlowMaterial;

    [Header("Prefabs")]
    public GameObject _GuideBlockPrefab;
    public GameObject _BlockPrefab;
    public GameObject _AxisPlanePrefab;

    [Header("UI")]
    GameObject _AxisPlane;
    PlayerBlockUI _PlayerBlockUI;
    Renderer _AxisPlaneRenderer;
    Vector3 _AxisBlockPosition;
    float _ArrowUIDistance;

    [Header("Events")]
    public UnityEvent<int, GameObject> _BlockEvent = new();
    public UnityEvent<GameObject> _UndoEvent = new();
    public UnityEvent<GameObject> _LimitEvent = new();
    public UnityEvent<GameObject, bool> _BubbleEvent = new();
    public UnityEvent<int> _PlaceEvent = new();

    //Private
    [Header("States")]
    bool _Active = false;
    bool _Playing = true;
    bool _IsMovingBlock = false;
    bool _InputSwapAxis = false;
    bool _InputMoveVertical = false;
    bool _InputMoveHorizontal = false;
    bool _InputUndo = false;
    bool _FastRecover = false;
    const float _FastRecoverDistance = 7f;
    const float _FastRecoverTime = 0.1f;
    const float _SlowRecoverTime = 0.25f;
    const float _FarThreshold = 20f;

    Vector2 _InputDir;
    Vector2 _InputDirCombined;
    Vector3 _InputModifierX;
    Vector3 _InputModifierY;

    //Player
    PlayerMovement _PlayerMovement;
    Color _BlockColor;

    //Camera
    Camera _Camera;
    PlayerCamera _PlayerCamera;
    Transform _TrackingTarget;

    void Start()
    {
        GameManager.Instance.AddPauseListener(OnPause);
        _CharacterController = GetComponent<CharacterController>();

        _ArrowUIDistance = _BlockSize + 1f;
        _NearbyNodesList = new Collider[_MaxNearbyNodes];
        _Camera = Camera.main;
        _PlayerMovement = GetComponent<PlayerMovement>();
        _PlayerCamera = GetComponentInChildren<PlayerCamera>();

        _TrackingTarget = new GameObject("TrackingTarget").transform;

        _GuideBlock = Instantiate(_GuideBlockPrefab);
        _GuideBlock.transform.localScale = new Vector3(_BlockSize, _BlockSize, _BlockSize);

        _PlayerBlockUI = GetComponent<PlayerBlockUI>();
        _PlayerBlockUI.SetLimit(_BlockLimit);

        _AxisPlane = Instantiate(_AxisPlanePrefab);
        _AxisPlane.SetActive(false);
        _AxisPlaneRenderer = _AxisPlane.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_Playing)
        {
            
            if (_Active)
            {
                Vector3 cameraBlockRotation = Quaternion.LookRotation(_Block.transform.position - _Camera.transform.position, Vector3.up).eulerAngles;
                Shader.SetGlobalInt("_Active_Block", 1);
                Shader.SetGlobalVector("_Block_World_Position", _Block.transform.position);
                Shader.SetGlobalVector("_Camera_Block_Rotation", cameraBlockRotation);

                bool canControl = CanControl();
                if (!_InputUndo)
                {
                    _PlayerBlockUI.arrowPanel.SetActive(canControl);
                    _PlayerBlockUI.crossPanel.SetActive(!canControl);
                    
                }
                else
                {
                    _PlayerBlockUI.arrowPanel.SetActive(false);
                    _PlayerBlockUI.crossPanel.SetActive(true);
                    
                }
                _AxisPlane.SetActive(!_InputUndo && canControl);
                _PlayerBlockUI.crossPanel.transform.position = _Camera.WorldToScreenPoint(_Block.transform.position);

                DisplayBubble();
                UpdateControls(_Block.transform);

                if (MinDistance() > _FarThreshold)
                    _TrackingTarget.position = transform.position;
                else
                    _TrackingTarget.position = Vector3.Lerp(transform.position, _Block.transform.position, 0.5f);
                

                if (!_IsMovingBlock)
                {
                    if ((_InputMoveVertical || _InputMoveHorizontal) && canControl)
                        MoveBlock();
                    else if (_InputUndo)
                        UndoBlock();
                }

                RecoverColor();
            }
            else
            {
                Shader.SetGlobalInt("_Active_Block", 0);
                _MissingBlockSprite.SetActive(false);

                //Near a placement node
                if (NearNodes())
                {
                    GameObject nearestNode = NearestNode();
                    //Guide block
                    _GuideBlock.SetActive(true);
                    _GuideBlock.transform.position = nearestNode.transform.position;
                    _GuideBlock.transform.eulerAngles = new Vector3(0f, nearestNode.transform.rotation.eulerAngles.y, 0f);
                    //Placement button
                    _PlayerBlockUI.placePanel.SetActive(true);
                    _PlayerBlockUI.placePanel.transform.position = _Camera.WorldToScreenPoint(_GuideBlock.transform.position);
                }
                else
                {
                    _GuideBlock.SetActive(false);
                    _PlayerBlockUI.placePanel.SetActive(false);
                }
            }
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
        _NearbyNodeTotal = Physics.OverlapSphereNonAlloc(transform.position, _PlaceRange, _NearbyNodesList, _BlockNodeLayer);
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
            float distance = Vector3.Distance(transform.position, node.transform.position);
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
            if (!_Active && NearNodes())
            {   //place on node if block isnt active
                _CinemachineCamera.Target.TrackingTarget = _TrackingTarget;
                _Active = true;
                GameObject nearestNode = NearestNode();

                Vector3 nodeViewPos = _Camera.WorldToViewportPoint(nearestNode.transform.position);
                Vector3 playerViewPos = _Camera.WorldToViewportPoint(transform.position);
                Debug.Log(nodeViewPos.x + " " + playerViewPos.x);

                if (nodeViewPos.x < playerViewPos.x)
                    _PlaceEvent?.Invoke(0);
                else
                    _PlaceEvent?.Invoke(1);

                _AxisPlaneRenderer.material.SetVector("_Origin", new Vector3(
                            nearestNode.transform.position.x,
                            nearestNode.transform.position.y,
                            nearestNode.transform.position.z));
                _Block = Instantiate(_BlockPrefab);
                _Block.transform.position = nearestNode.transform.position;
                _Block.transform.eulerAngles = new Vector3(0f, nearestNode.transform.eulerAngles.y, 0f);
                _BlockList.Add(_Block);
                _PlayerBlockUI.placePanel.SetActive(false);
                _PlayerBlockUI.playerPanel.SetActive(true);
                _Block.SetActive(true);
                _AxisPlane.SetActive(true);
                _GuideBlock.SetActive(false);
                _BlockEvent?.Invoke(0, _Block);
                _PlayerCamera.SetCubeMode(_Block.transform.eulerAngles.y);
                _PlayerBlockUI.SetTile(_BlockList.Count - 1, true);
                _AxisBlockPosition = _Block.transform.position;
            }
            else if (_Active)
            {
                _PlayerBlockUI.StartFade();
            }
        }
    }

    public void OnMoveBlockVertical(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _InputMoveVertical = true;
            _InputDir = new Vector2(0f, context.ReadValue<float>());
            _InputDirCombined = new Vector2(_InputDirCombined.x, context.ReadValue<float>());
        }
        else if (context.canceled)
        {
            _InputMoveVertical = false;
            _InputDir = new Vector2(_InputDirCombined.x, 0f);
        }
    }

    public void OnMoveBlockHorizontal(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _InputMoveHorizontal = true;
            _InputDir = new Vector2(context.ReadValue<float>(), 0f);
            _InputDirCombined = new Vector2(context.ReadValue<float>(), _InputDirCombined.y);
        }
        else if (context.canceled)
        {
            _InputMoveHorizontal = false;
            _InputDir = new Vector2(0f, _InputDirCombined.y);
        }
    }

    public void MoveBlock()
    {
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

        bool limitHit = false;
        //can move block
        if (!hit && _BlockList.Count < _BlockLimit)
        {
            _IsMovingBlock = true;
            _BlockEvent?.Invoke(_BlockList.Count, _Block);
            Vector3 newBlockPlacement = blockTransform.position + blockTransform.TransformDirection(localXY);
            _Block = Instantiate(_BlockPrefab, _Block.transform.position, blockTransform.rotation);
            _TweenObject = _Block.transform.DOMove(newBlockPlacement, _TweenDuration).SetEase(_BlockEasing).OnComplete(() => {
                _IsMovingBlock = false;
                _AxisBlockPosition = _Block.transform.position;
            });
            _BlockList.Add(_Block);
            _Block.SetActive(true);
            _PlayerBlockUI.SetTile(_BlockList.Count - 1, true);
        }
        //hit block
        else if (hit && hitInfo.transform.gameObject.CompareTag("Block"))
        {
            int index = _BlockList.IndexOf(hitInfo.transform.gameObject);
            if (index == _BlockList.Count - 2)
            {   //hit previous block -> undo
                _IsMovingBlock = true;
                _UndoEvent?.Invoke(_Block);
                Vector3 previousPosition = _BlockList[^2].transform.position;
                _TweenObject = _Block.transform.DOMove(previousPosition, _TweenDuration).SetEase(_BlockEasing).OnComplete(() =>
                {
                    _IsMovingBlock = false;
                    _BlockList.RemoveAt(_BlockList.Count - 1);
                    Destroy(_Block);
                    _Block = _BlockList[^1];
                    _AxisBlockPosition = _Block.transform.position;
                });
                _PlayerBlockUI.SetTile(_BlockList.Count - 1, false);
            }
            else
            {
                limitHit = true;
            }
        }
        //at block limit or hit something else
        else
        {
            limitHit = true;
        }

        if (limitHit)
        {
            _LimitEvent?.Invoke(_Block);
            _IsMovingBlock = true;
            float v = 0f;
            DOTween.To(() => v, x => v = x, 1f, _TweenDuration).OnComplete(() => _IsMovingBlock = false);
        }
    }

    public void OnUndoBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _InputUndo = true;
            _FastRecover = false;
            foreach (GameObject block in _BlockList)
            {
                float distance = Vector3.Distance(transform.position, block.transform.position);
                if (distance < _FastRecoverDistance)
                {
                    _FastRecover = true;
                    break;
                }
            }
        }
        else if (context.canceled)
            _InputUndo = false;
    }

    public void UndoBlock()
    {
        if (_BlockList.Count > 1)
        {
            _UndoEvent?.Invoke(_Block);
            _IsMovingBlock = true;
            Vector3 previousPosition = _BlockList[^2].transform.position;

            float duration;
            if (_FastRecover)
                duration = _FastRecoverTime;
            else
                duration = _SlowRecoverTime;

            _TweenObject = _Block.transform.DOMove(previousPosition, duration).SetEase(_BlockEasing).OnComplete(() => {
                _IsMovingBlock = false;
                _BlockList.RemoveAt(_BlockList.Count - 1);
                Destroy(_Block);
                _Block = _BlockList[^1];
                _AxisBlockPosition = _Block.transform.position;
            });
            _PlayerBlockUI.SetTile(_BlockList.Count - 1, false);
        }
        else
        {
            _UndoEvent?.Invoke(_Block);
            _Active = false;
            _PlayerBlockUI.SetPanelsActive(false);
            _AxisPlane.SetActive(false);
            _PlayerCamera.SetOctaMode();
            _BlockList.RemoveAt(0);
            Destroy(_Block);
            _Block = null;
            _CinemachineCamera.Target.TrackingTarget = transform;
        }
    }

    public void OnSwapAxisBlock(InputAction.CallbackContext context)
    {
        if (context.started)
            _InputSwapAxis = true;
        else if (context.canceled)
            _InputSwapAxis = false;
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
                break;
            case 1:
                _InputModifierX = new(0f, 0f, -1f);
                Modify(new(1f, 0f, 0f), blockTransform, blockTransform.right, blockTransform.forward, 1, -1);
                break;
            case 2:
                _InputModifierX = new(-1f, 0f, 0f);
                Modify(new(0f, 0f, -1f), blockTransform, blockTransform.forward, blockTransform.right, -1, -1);
                break;
            case 3:
                _InputModifierX = new(0f, 0f, 1f);
                Modify(new(-1f, 0f, 0f), blockTransform, blockTransform.right, blockTransform.forward, -1, 1);
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
        if (_InputSwapAxis)
        {
            _InputModifierY = Vector3.up;
            vUI = transform.up;
            _AxisPlane.transform.SetPositionAndRotation(_AxisBlockPosition + (_BlockSize / 2f - 0.01f) * sign * zDir, Quaternion.LookRotation(Vector3.up, zDir));
        }
        else
        {
            _InputModifierY = inputModY;
            vUI = sign * zDir;
            _AxisPlane.transform.eulerAngles = transform.eulerAngles;
            _AxisPlane.transform.position = _AxisBlockPosition - new Vector3(0f, _BlockSize / 2f - 0.01f, 0f);
        }
        
        _PlayerBlockUI.blockUIUp.position = _Camera.WorldToScreenPoint(transform.position + vUI * _ArrowUIDistance);
        _PlayerBlockUI.blockUIDown.position = _Camera.WorldToScreenPoint(transform.position - vUI * _ArrowUIDistance);
        _PlayerBlockUI.blockUILeft.position = _Camera.WorldToScreenPoint(transform.position - _ArrowUIDistance * sign2 * xUI);
        _PlayerBlockUI.blockUIRight.position = _Camera.WorldToScreenPoint(transform.position + _ArrowUIDistance * sign2 * xUI);
    }

    private void RecoverColor()
    {
        
        bool displayRecover = IsClose(_FastRecoverDistance);
        if (displayRecover)
        {
            foreach (GameObject block in _BlockList)
            {
                block.GetComponent<Renderer>().material = _BlockMaterial;
            }

        }
        else
        {
            foreach (GameObject block in _BlockList)
            {
                block.GetComponent<Renderer>().material = _BlockSlowMaterial;
            }
        }
    }

    private bool IsClose(float threshold)
    {
        foreach (GameObject block in _BlockList)
        {
            float distance = Vector3.Distance(transform.position, block.transform.position);
            if (distance < threshold)
            {

                return true;
            }
        }
        return false;
    }

    private float MinDistance()
    {
        float distanceFromBlock = 999f;
        foreach (GameObject block in _BlockList)
        {
            float distance = Vector3.Distance(transform.position, block.transform.position);
            if (distance < distanceFromBlock)
            {
                distanceFromBlock = distance;
            }
        }
        return distanceFromBlock;
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
        _PlayerBlockUI.SetPanelsActive(false);
        _AxisPlane.SetActive(false);
        _PlayerCamera.SetOctaMode();
        _Block = null;
        _CinemachineCamera.Target.TrackingTarget = transform;
        _PlayerBlockUI.EmptyTiles();
    }

    public void Reset()
    {
        RetrieveBlock();
    }

    public void ResetCamera()
    {
        _CinemachineCamera.Target.TrackingTarget = transform;
    }

    private void DisplayBubble()
    {
        if (IsClose(_MissingBubbleDistance))
        {
            _MissingBubbleActive = false;
            _MissingBlockSprite.SetActive(_MissingBubbleActive);
        } 
        else
        {
            if (!_MissingBubbleActive && !_InputUndo)
            {
                _BubbleEvent?.Invoke(gameObject, true);
                _MissingBubbleActive = true;
                _MissingBlockSprite.SetActive(_MissingBubbleActive);
            }
        }
    }

    public void OnPause(bool value)
    {
        _Playing = !value;
    }
}