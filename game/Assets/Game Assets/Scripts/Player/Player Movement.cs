using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Public
    public float _Gravity = 30f;
    public float _MinSpeed = 1.0f;
    public float _MaxSpeed = 5.0f;
    public float _JumpSpeed = 12.0f;
    public float _MoveAccel = 10.0f;
    public float _MoveDeaccel = 20.0f;
    public float _FallingJumpTime = 0.25f;
    float _FallingTimer = 0f;
    bool _HasJumped = false;

    //Events
    [Header("Player Events")]
    public UnityEvent<GameObject> _JumpEvent;

    //Private
    Camera _Camera;
    CharacterController _CharControl;
    PlayerInput _PlayerInput;
    InputActionMap _PlayerActionMap;
    Vector2 _MoveDir;
    Vector3 _Velocity;
    bool _MoveInput = false;
    Vector3 _WorldMoveDir;
    bool _Playing = true;

    //Animator
    [Header("Animation Settings")]
    private Animator _Animator;
    private float _AirDistance;
    public float _MinFalldistance = 1.25f;
    public float _AnimDelay;
    private IEnumerator _NewIdleCoroutine;

    public BlockController2 blockController;
    public bool _BlockActive;


    private void Awake()
    {
        _Animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        GameManager.Instance.AddPauseListener(OnPause);
        _Camera = Camera.main;
        _CharControl = GetComponent<CharacterController>();
        _PlayerInput = GetComponent<PlayerInput>();
        _PlayerActionMap = _PlayerInput.actions.FindActionMap("Player");
        _PlayerActionMap.Enable();
        blockController = GetComponent<BlockController2>();

        _NewIdleCoroutine = NewIdle();
        StartCoroutine(_NewIdleCoroutine);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        _WorldMoveDir = Quaternion.AngleAxis(_Camera.transform.eulerAngles.y, new Vector3(0f, 1f, 0f)) * new Vector3(_MoveDir.x, 0f, _MoveDir.y);
        _Animator.SetBool("isGrounded", _CharControl.isGrounded);
        _BlockActive = blockController.IsBlockActive();

        if (_Playing)
        {
            //Vertical movement (gravity)
            if (_CharControl.isGrounded && _Velocity.y < 0)
                _Velocity.y = -1.0f;
            else
                _Velocity.y -= _Gravity * deltaTime;

            //Horizontral movement
            if (_MoveInput)
                _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(_WorldMoveDir.x * _MaxSpeed, _Velocity.y, _WorldMoveDir.z * _MaxSpeed), _MoveAccel * deltaTime);
            else
                _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(0f, _Velocity.y, 0f), _MoveDeaccel * deltaTime);

            _CharControl.Move(_Velocity * deltaTime);
        }

        if (!_CharControl.isGrounded)
        {
            _FallingTimer += deltaTime;
            _AirDistance += Mathf.Abs(_Velocity.y) * deltaTime;

            if (_AirDistance > _MinFalldistance && _Velocity.y < 0f)
            {
                _Animator.SetBool("Falling", true);
            }
        }
        else if (_CharControl.isGrounded)
        {

            _Animator.SetBool("Falling", false);
            if (_AirDistance > _MinFalldistance)
            {
                _Animator.SetTrigger("Landed");
            }

            _HasJumped = false;
            _FallingTimer = 0f;
            _AirDistance = 0.0f;
        }
        else
        {

        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_Playing)
        {
            StopCoroutine(_NewIdleCoroutine);
            _Animator.SetBool("isWalking", true);

            if (context.canceled)
            {
                _Animator.SetBool("isWalking", false);

                _Animator.SetFloat("lastInputX", _MoveDir.x);
                _Animator.SetFloat("lastInputY", _MoveDir.y);

                StartCoroutine(_NewIdleCoroutine);
            }

            _MoveDir = context.ReadValue<Vector2>();

            _Animator.SetFloat("inputX", _MoveDir.x);
            _Animator.SetFloat("inputY", _MoveDir.y);

            if (_MoveDir.magnitude > 0.0f)
                _MoveInput = true;
            else
                _MoveInput = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (_CharControl.isGrounded || _FallingTimer <= _FallingJumpTime) && !_HasJumped && _Playing)
        {
            _HasJumped = true;
            _Animator.SetTrigger("Jumping");
            _JumpEvent?.Invoke(gameObject);
            _Velocity.y = _JumpSpeed;

            _Animator.SetFloat("inputX", _MoveDir.x);
            _Animator.SetFloat("inputY", _MoveDir.y);
        }
    }

    public void OnPlaceBrick(int direction)
    {
        Debug.Log("function fires");
        _Animator.SetFloat("brickPlaceX", direction);
        _Animator.SetTrigger("Placing");

    }

    public void UndoBlock(int direction)
    {
        Debug.Log("function fires brick retrive");
        _Animator.SetFloat("brickPlaceX", direction);
        _Animator.SetTrigger("Retrive");

    }

    private IEnumerator NewIdle()
    {
        _AnimDelay = Random.Range(10.0f, 15.0f);
        yield return new WaitForSeconds(_AnimDelay);

        _Animator.SetTrigger("Anim2");


        //_Animator.ResetTrigger("Anim2");
    }

    public void OnPause(bool value)
    {
        _Playing = !value;
        if (!value) _PlayerActionMap.Enable();
        else _PlayerActionMap.Disable();
        Debug.Log(_PlayerActionMap.enabled);
    }
}