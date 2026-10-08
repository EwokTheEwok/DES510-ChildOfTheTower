using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Public
    public float _Gravity = 20f;
    public float _MinSpeed = 1.0f;
    public float _MaxSpeed = 5.0f;
    public float _JumpSpeed = 7.0f;
    public float _MoveAccel = 10.0f;
    public float _MoveDeaccel = 20.0f;
    public Camera _Camera;

    //Private
    CharacterController _CharControl;
    PlayerInput _PlayerInput;
    Vector2 _MoveDir;
    Vector3 _Velocity;
    Vector3 _Displacement;
    bool _MoveInput = false;
    Vector3 _CameraSpaceMoveDir;

    //interpolation
    float _FixedTimeStep;
    float _AccumlatedTime = 0f;
    float _Alpha = 0f;
    Vector3 _CurrentPosition;
    Vector3 _PreviousPosition;

    void Start()
    {
        _CharControl = GetComponent<CharacterController>();
        _PlayerInput = GetComponent<PlayerInput>();
        _PlayerInput.actions.FindActionMap("Player").Enable();
        _CurrentPosition = transform.position;
        _PreviousPosition = transform.position;
        Time.maximumDeltaTime = 0.25f;
    }

    public void FixedUpdate()
    {
        _FixedTimeStep = Time.fixedDeltaTime;
        _CameraSpaceMoveDir = _Camera.transform.TransformDirection(new Vector3(_MoveDir.x, 0f, _MoveDir.y));

        //Vertical movement (jump/gravity)
        if (_CharControl.isGrounded && _Velocity.y < 0)
            _Velocity.y = -1.0f;
        else
            _Velocity.y -= _Gravity * _FixedTimeStep;

        //Horizontral movement
        if (_MoveInput)
            _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(_CameraSpaceMoveDir.x * _MaxSpeed, _Velocity.y, _CameraSpaceMoveDir.z * _MaxSpeed), _MoveAccel * _FixedTimeStep);
        else
            _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(0f, _Velocity.y, 0f), _MoveDeaccel * _FixedTimeStep);

        _Displacement = _Velocity * _FixedTimeStep;
        _PreviousPosition = _CurrentPosition;
        _CurrentPosition = transform.position + _Displacement;
        Debug.Log("step");
    }

    private void Update()
    {
        while (_AccumlatedTime >= Time.fixedDeltaTime)
            _AccumlatedTime -= Time.fixedDeltaTime;

        _Alpha = _AccumlatedTime / Time.fixedDeltaTime;
        Debug.Log(transform.position.x);
        Vector3 interpPos = (_CurrentPosition * _Alpha) + (_PreviousPosition * (1f - _Alpha));
        Vector3 newDisp = interpPos - _PreviousPosition;
        //_CharControl.Move(newDisp);

        _AccumlatedTime += Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _MoveDir = context.ReadValue<Vector2>();
        if (_MoveDir.magnitude > 0.0f)
            _MoveInput = true;
        else
            _MoveInput = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _CharControl.Move(new Vector3(1f, 0f, 0f));
        if (context.started && _CharControl.isGrounded)
        {
            //_Velocity.y = _JumpSpeed;
            
        }
    }
}

/*
public class Player : MonoBehaviour
{
    //Public
    public float _MaxSpeed = 5.0f;
    public float _JumpForce = 7.0f;
    public float _MoveAccel = 10.0f;
    public float _MoveDeaccel = 20.0f;
    public Camera _Camera;
    public Vector3 _Vel;

    //Private
    Rigidbody _Rigidbody;
    PlayerInput _PlayerInput;
    Vector2 _MoveDir;
    Vector3 _Displacement;
    bool _MoveInput = false;
    Vector3 _CameraSpaceMoveDir;

    void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _PlayerInput = GetComponent<PlayerInput>();
        _PlayerInput.actions.FindActionMap("Player").Enable();
    }

    private void FixedUpdate()
    {
        float timeStep = Time.fixedDeltaTime;
        _CameraSpaceMoveDir = _Camera.transform.TransformDirection(new Vector3(_MoveDir.x, 0f, _MoveDir.y));

        float velY = _Rigidbody.linearVelocity.y;
        _Vel = _Rigidbody.linearVelocity;

        if (_MoveInput)
        {
            Vector3 horizontalAccel = new (_CameraSpaceMoveDir.x * _MoveAccel, 0.0f, _CameraSpaceMoveDir.z * _MoveAccel);
            _Rigidbody.AddForce(horizontalAccel, ForceMode.Acceleration);
            if (new Vector3(_Rigidbody.linearVelocity.x, 0f, _Rigidbody.linearVelocity.z).magnitude > _MaxSpeed) {
                _Vel = _Rigidbody.linearVelocity.normalized * _MaxSpeed;
            }
        }
        else
        {

        }
            _Rigidbody.linearVelocity = new(_Vel.x, velY, _Vel.z);

        CharacterController a = new CharacterController();
        a.
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _MoveDir = context.ReadValue<Vector2>();
        if (_MoveDir.magnitude > 0.0f)
            _MoveInput = true;
        else
            _MoveInput = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            _Rigidbody.AddForce(new Vector3(0f, _JumpForce, 0f), ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        float checkDistance = GetComponent<CapsuleCollider>().height / 2f + 0.1f;
        return Physics.Raycast(transform.position, -Vector3.up, checkDistance);
    }
}
*/