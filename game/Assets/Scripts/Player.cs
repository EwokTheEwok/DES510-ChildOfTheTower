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

    void Start()
    {
        _CharControl = GetComponent<CharacterController>();
        _PlayerInput = GetComponent<PlayerInput>();
        _PlayerInput.actions.FindActionMap("Player").Enable();
    }

    private void FixedUpdate()
    {
        float timeStep = Time.fixedDeltaTime;
        _CameraSpaceMoveDir = _Camera.transform.TransformDirection(new Vector3(_MoveDir.x, 0f, _MoveDir.y));

        //Vertical movement (jump/gravity)
        if (_CharControl.isGrounded)
        {
            if (_Velocity.y < 0)
                _Velocity.y = -1.0f;
        }
        else
            _Velocity.y -= _Gravity * timeStep;

        if (_MoveInput)
            _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(_CameraSpaceMoveDir.x * _MaxSpeed, _Velocity.y, _CameraSpaceMoveDir.z * _MaxSpeed), _MoveAccel * timeStep);
        else
            _Velocity = Vector3.MoveTowards(_Velocity, new Vector3(0f, _Velocity.y, 0f), _MoveDeaccel * timeStep);

        _Displacement = _Velocity * timeStep;
        _CharControl.Move(_Displacement);
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
        if (context.started && _CharControl.isGrounded)
        {
            _Velocity.y = _JumpSpeed;
        }
    }
}
