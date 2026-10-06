using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Public
    public float gravity = 20f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 5.0f;
    public float jumpSpeed = 7.0f;
    public float moveAccel = 10.0f;
    public float moveDeaccel = 20.0f;
    //Private
    CharacterController charControl;
    PlayerInput playerInput;
    Vector2 moveDir;
    Vector3 velocity;
    Vector3 displacement;
    bool moveInput = false;
    Vector3 cameraSpaceMoveDir;


    void Start()
    {
        charControl = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Enable();
    }

    private void FixedUpdate()
    {
        float timeStep = Time.fixedDeltaTime;
        cameraSpaceMoveDir = playerInput.camera.transform.TransformDirection(new Vector3(moveDir.x, 0f, moveDir.y));

        //Vertical movement (jump/gravity)
        if (charControl.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -1.0f;
        }
        else
            velocity.y -= gravity * timeStep;

        if (moveInput)
            velocity = Vector3.MoveTowards(velocity, new Vector3(cameraSpaceMoveDir.x * maxSpeed, velocity.y, cameraSpaceMoveDir.z * maxSpeed), moveAccel * timeStep);
        else
            velocity = Vector3.MoveTowards(velocity, new Vector3(0f, velocity.y, 0f), moveDeaccel * timeStep);

        displacement = velocity * timeStep;
        charControl.Move(displacement);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        if (moveDir.magnitude > 0.0f)
            moveInput = true;
        else
            moveInput = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && charControl.isGrounded)
        {
            velocity.y = jumpSpeed;
        }
    }
}
