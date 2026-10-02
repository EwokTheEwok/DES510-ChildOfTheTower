using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    CharacterController charControl;
    PlayerInput playerInput;
    public float gravity = 20f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 5.0f;
    public float jumpSpeed = 7.0f;
    public float moveAccel = 10.0f;
    public float moveDeaccel = 20.0f;
    float speed;
    Vector2 accel;
    Vector2 moveDir;
    Vector3 velocity;
    Vector3 displacement;
    bool moveInput = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Enable();
        charControl = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        float timeStep = Time.fixedDeltaTime;

        //Vertical movement (jump/gravity)
        if (charControl.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -1.0f;
        }
        else
            velocity.y -= gravity * timeStep;
        
        /*
        speed = new Vector2(velocity.x, velocity.z).magnitude;
        accel = new Vector2(0.0f, 0.0f);
        if (moveInput) {
            accel = moveDir * new Vector2(moveAccel, moveAccel);
            if (Math.Abs(velocity.x) > maxSpeed)
                velocity.x = maxSpeed;
            if (Math.Abs(velocity.z) > maxSpeed)
                velocity.x = maxSpeed;
        }
        else if (!moveInput)
        {
            if (Math.Abs(velocity.x) > 0.0f)
                accel = new Vector2(Mathf.Sign(velocity.x) * -moveDeaccel, Mathf.Sign(velocity.z) * -moveDeaccel);
            else
                velocity.x = 0.0f;

            if (Math.Abs(velocity.z) > 0.0f)
                accel = new Vector2(Mathf.Sign(velocity.x) * -moveDeaccel, Mathf.Sign(velocity.z) * -moveDeaccel);
            else
                velocity.z = 0.0f;
        }
        */
        velocity = Vector3.MoveTowards(velocity, new Vector3(moveDir.x * maxSpeed, 0f, moveDir.y * maxSpeed), moveAccel * timeStep);//velocity + new Vector3(accel.x * timeStep, 0, accel.y * timeStep);
        displacement = velocity * timeStep;
        charControl.Move(displacement);
    }
    void Update()
    {
        
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
            velocity.y = jumpSpeed;//jumpHeight / 0.5f;
        }
    }
}
