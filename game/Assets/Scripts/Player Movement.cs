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
    //Events
    public UnityEvent<GameObject> _JumpEvent;

    //Private
    Camera _Camera;
    CharacterController _CharControl;
    PlayerInput _PlayerInput;
    Vector2 _MoveDir;
    Vector3 _Velocity;
    bool _MoveInput = false;
    Vector3 _WorldMoveDir;
    bool _CanControl = true;

    //Animator
    private Animator _Animator;


    private void Awake()
    {
        _Animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _Camera = Camera.main;
        _CharControl = GetComponent<CharacterController>();
        _PlayerInput = GetComponent<PlayerInput>();
        _PlayerInput.actions.FindActionMap("Player").Enable();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        _WorldMoveDir = Quaternion.AngleAxis(_Camera.transform.eulerAngles.y, new Vector3(0f, 1f, 0f)) * new Vector3(_MoveDir.x, 0f, _MoveDir.y);

        //Vertical movement (jump/gravity)
        if (_CharControl.isGrounded && _Velocity.y < 0)
            _Velocity.y = -1.0f;
        else
            _Velocity.y -= _Gravity * deltaTime;

        //Horizontral movement
        if (_MoveInput && _CanControl)
            _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(_WorldMoveDir.x * _MaxSpeed, _Velocity.y, _WorldMoveDir.z * _MaxSpeed), _MoveAccel * deltaTime);
        else
            _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(0f, _Velocity.y, 0f), _MoveDeaccel * deltaTime);

        _CharControl.Move(_Velocity * deltaTime);
    }

    public void SetControl(bool _value)
    {
        _CanControl = _value;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _Animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            _Animator.SetBool("isWalking", false);
            _Animator.SetFloat("lastInputX", _MoveDir.x);
            _Animator.SetFloat("lastInputY", _MoveDir.y);
        }

        _MoveDir = context.ReadValue<Vector2>();

        _Animator.SetFloat("inputX", _MoveDir.x);
        _Animator.SetFloat("inputY", _MoveDir.y);

        if (_MoveDir.magnitude > 0.0f)
            _MoveInput = true;
        else
            _MoveInput = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && _CharControl.isGrounded)
        {
            _JumpEvent?.Invoke(gameObject);
            _Velocity.y = _JumpSpeed;
        }
    }
}